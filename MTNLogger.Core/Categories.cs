using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerCore
{
    /// <summary>
    /// Struct used among queues to bear a complete set of information
    /// </summary>
    public struct Message
    {
        public string timeNow;
        public Guid id;
        public Level level;
        public string message;
        public string outputMessage;
    }

    public enum Destination
    {
        File,
        Console,
        EventViewer,
        Database
    }

    public enum Level
    {
        Info,
        Warning,
        Error,
        Critical
    }
}
