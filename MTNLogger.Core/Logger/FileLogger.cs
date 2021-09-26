using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace LoggerCore.Logger
{
    /// <summary>
    /// Logger writing to files
    /// </summary>
    public class FileLogger : LoggerBase
    {

        /// <summary>
        /// For each File there is a Queue
        /// </summary>
        private static ConcurrentDictionary<string, LogQueue<Message>> fileQueue = new ConcurrentDictionary<string, LogQueue<Message>>();
        string destinationFile;
        StreamWriter fileStream;

        internal FileLogger(Guid id, string path) : base(id)
        {
            destinationFile = path;

            if (!fileQueue.ContainsKey(destinationFile))
            {
                if (fileQueue.TryAdd(destinationFile, new LogQueue<Message>()))
                {
                    Task queueWorker = new Task(workQueue, cancConsumerToken.Token);
                    queueWorker.Start();
                }
            }
        }


        protected override void workQueue()
        {
            fileStream = File.CreateText(destinationFile);
            fileStream.AutoFlush = true;
            LogQueue<Message> outQueue = fileQueue[destinationFile];

            while (outQueue.WaitEvent())
            {
                try
                {
                    while (!outQueue.IsEmpty)
                    {
                        if (outQueue.TryDequeue(out outMessage))
                            fileStream.WriteLine(outMessage.outputMessage);
                    }

                    outQueue.ResetEvent();
                }
                catch (OperationCanceledException)
                {
                    fileStream.Close();
                    break;
                }
                catch (Exception)
                { }
            }
        }

        public override void Log(Level level, string msg)
        {
            base.Log(level, msg);

            LogQueue<Message> currentQueue;
            if (fileQueue.TryGetValue(destinationFile, out currentQueue))
                currentQueue.Enqueue(inMessage);
        }

        public override void Dispose()
        {
            foreach (string key in fileQueue.Keys)
                fileQueue[key].CloseEvent();
            base.Dispose();
        }
    }
}
