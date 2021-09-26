using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Threading.Tasks;

namespace LoggerCore.Logger
{
    /// <summary>
    /// Logger writing on databases 
    /// CURRENTLY IT IS JUST A DRAFT - not tested -
    /// </summary>
    public class DBLogger : LoggerBase
    {
        /// <summary>
        /// For each Database there is a LogQueue
        /// </summary>
        private static ConcurrentDictionary<string, LogQueue<Message>> dbQueue = new ConcurrentDictionary<string, LogQueue<Message>>();

        /// <summary>
        /// Actual connection string to a DB, currently used as LogQueue Key
        /// </summary>
        private string connectionString;
        DbConnection currentConnection = null;

        //dummy
        private string fixedTable = "MYSCHEMA.MYTABLE";

        internal DBLogger(Guid id, string connString) : base(id)
        {
            connectionString = connString;
            if (dbQueue.TryAdd(connectionString, new LogQueue<Message>()))
            {
                Task queueWorker = new Task(workQueue, cancConsumerToken.Token);
                queueWorker.Start();
            }
        }

        public override void Log(Level level, string msg)
        {
            base.Log(level, msg);
            LogQueue<Message> inQueue;
            if(dbQueue.TryGetValue(connectionString, out inQueue))
                inQueue.Enqueue(inMessage);
        }

        protected override void workQueue()
        {
            currentConnection.ConnectionString = connectionString;
            LogQueue<Message> outQueue = dbQueue[connectionString];

            if (currentConnection.State == System.Data.ConnectionState.Closed)
                currentConnection.Open();

            while (outQueue.WaitEvent())
            {
                try
                {
                    while (!outQueue.IsEmpty)
                    {
                        if (outQueue.TryDequeue(out outMessage))
                        {
                            DbCommand command = currentConnection.CreateCommand();
                            command.CommandText = string.Format("INSERT INTO {0} VALUES({1}, {2}, {3})", fixedTable, outMessage.id, outMessage.timeNow, outMessage.message);
                            command.ExecuteNonQuery();
                        }
                    }

                    outQueue.ResetEvent();
                }
                catch (OperationCanceledException)
                {
                    currentConnection.Close();
                    break;
                }
                catch (Exception)
                { }
            }
        }

        public override void Dispose()
        {
            foreach(string key in dbQueue.Keys)
                dbQueue[key].CloseEvent();
            base.Dispose();
        }
    }
}
