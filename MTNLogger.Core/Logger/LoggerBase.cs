using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerCore.Logger
{
    /// <summary>
    /// Base logger
    /// </summary>
    public abstract class LoggerBase : IDisposable
    {
        #region Loggers common properties
        public Guid LoggerId { get; private set; }
        protected Message inMessage, outMessage;

        protected AutoResetEvent askDequeue = new AutoResetEvent(false);
        protected CancellationTokenSource cancConsumerToken = new CancellationTokenSource();
        #endregion

        internal LoggerBase(Guid id)
        {
            LoggerId = id;
        }

        public virtual void Log(Level level, string msg)
        {
            inMessage = PrepareMessage(level, msg);
        }

        public virtual void LogInfo(string msg)
        {
            Log(Level.Info, msg);
        }

        public virtual void LogWarn(string msg)
        {
           Log(Level.Warning, msg);
        }

        public virtual void LogErr(string msg)
        {
            Log(Level.Error, msg);
        }

        public virtual void LogCritic(string msg)
        {
           Log(Level.Critical, msg);
        }

        protected Message PrepareMessage(Level level, string msg)
        {
            StringBuilder sb = new StringBuilder();
            Message message = new Message();

            message.id = this.LoggerId;
            message.level = level;
            message.timeNow = DateTime.Now.ToUniversalTime().ToString();
            message.message = msg;

            sb.AppendJoin("-", message.timeNow, this.LoggerId, level, msg);
            message.outputMessage = sb.ToString();

            return message;
        }

        protected abstract void workQueue();

        public virtual void Dispose()
        {
            cancConsumerToken.Cancel();
        }
    }

    /// <summary>
    /// Extension of ConcurrentQueue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class LogQueue<T> : ConcurrentQueue<T>
    {
        AutoResetEvent arevent = new AutoResetEvent(false);

        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            arevent.Set();
        }

        public void CloseEvent()
        {
            arevent.Close();
        }

        public void ResetEvent()
        {
            arevent.Reset();
        }

        public bool WaitEvent()
        {
            return arevent.WaitOne();
        }
    }
}
