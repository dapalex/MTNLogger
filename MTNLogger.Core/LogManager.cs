using LoggerCore.Logger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LoggerCore
{
    public static class LogManager
    {

        private static ConcurrentDictionary<Guid, Logger.LoggerBase> loggers = new ConcurrentDictionary<Guid, Logger.LoggerBase>();

        public static Logger.LoggerBase InstantiateLogger(Destination destinationType, string destination)
        {
            Guid idLogger = Guid.NewGuid();

            switch(destinationType)
            {
                case Destination.Console:
                    loggers.TryAdd(idLogger, new ConsoleLogger(idLogger));
                    break;
                //case Destination.Database:
                //    loggers.TryAdd(idLogger, new DBLogger(idLogger, destination));
                //    break;
                //case Destination.EventViewer:
                //    loggers.TryAdd(idLogger, new EventViewerLogger(idLogger));
                //    break;
                case Destination.File:
                    loggers.TryAdd(idLogger, new FileLogger(idLogger, destination));
                    break;
            }

            return loggers[idLogger];
        }
    }
}
