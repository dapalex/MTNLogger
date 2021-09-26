using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;

namespace LoggerCore.Logger
{
    /// <summary>
    /// Logger writing to Event Viewer
    /// </summary>
    public class EventViewerLogger : LoggerBase
    {
        EventSource mySource;
        internal EventViewerLogger(Guid id) : base(id)
        {
            inQueue = outQueue = new LogQueue<Message>();
            Task queueWorker = new Task(workQueue, cancConsumerToken.Token);
            queueWorker.Start();
        }

        public override void Log(Level level, string msg)
        {
            base.Log(level, msg);
            inQueue.Enqueue(inMessage);
        }


        protected override void workQueue()
        {
            mySource = new EventSource("Application");

            while (outQueue.WaitEvent())
            {
                try
                {
                    while (!outQueue.IsEmpty)
                    {
                        if (outQueue.TryDequeue(out outMessage))
                            WriteLogMessage();
                    }

                    outQueue.ResetEvent();
                }
                catch (OperationCanceledException)
                {
                    mySource.Dispose();
                    break;
                }
                catch (Exception)
                { }
            }
        }

        protected override void WriteLogMessage()
        {
            EventSourceOptions eso = new EventSourceOptions();

            switch (outMessage.level)
            {
                case Level.Info:
                    eso.Level = EventLevel.Informational;
                    break;
                case Level.Warning:
                    eso.Level = EventLevel.Warning;
                    break;
                case Level.Error:
                    eso.Level = EventLevel.Error;
                    break;
                case Level.Critical:
                    eso.Level = EventLevel.Critical;
                    break;
            }
            mySource.Write(outMessage.outputMessage, eso);
        }

        public override void Dispose()
        {
                fileQueue[key].CloseEvent();
            base.Dispose();
        }
    }
}
