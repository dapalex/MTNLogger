using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerCore.Logger
{
    /// <summary>
    /// Logger writing on Console
    /// </summary>
    public class ConsoleLogger : LoggerBase
    {
        static LogQueue<Message> currentQueue = new LogQueue<Message>();
        internal ConsoleLogger(Guid id) : base(id)
        {
            Task queueWorker = new Task(workQueue, cancConsumerToken.Token);
            queueWorker.Start();
        }

        public override void Log(Level level, string msg)
        {
            base.Log(level, msg);
            currentQueue.Enqueue(inMessage);
        }


        protected override void workQueue()
        {

            while (currentQueue.WaitEvent())
            {
                try
                {
                    while (!currentQueue.IsEmpty)
                    {
                        if (currentQueue.TryDequeue(out outMessage))
                            Console.WriteLine(outMessage.outputMessage);
                    }

                    currentQueue.ResetEvent();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                { }
            }
        }
        public override void Dispose()
        {
            currentQueue.CloseEvent();
            base.Dispose();
        }
    }
}

