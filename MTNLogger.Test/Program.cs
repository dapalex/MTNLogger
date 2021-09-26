using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoggerCore;
using LoggerCore.Logger;

namespace LoggerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! Let's get started pushing this logger! (More than 20 threads)");
            Console.WriteLine("");

            List<Task> loggers = new List<Task>();
            TaskFactory tf = new TaskFactory();

            for (int i = 0; i < 10; i++)
            {
                loggers.Add(new Task(CreateConsoleLogger));
                loggers.Add(new Task(CreateFileLogger));
            }

            loggers.ForEach(tsk =>
            { if (tsk.Status != TaskStatus.Running) { 
                    tsk.Start();
                } });

            Task.WaitAll(loggers.ToArray());
            Console.WriteLine("The job is done. Press ENTER");
            Console.ReadLine();
        }

        private static void CreateConsoleLogger()
        {
            ConsoleLogger cl = (ConsoleLogger)LogManager.InstantiateLogger(Destination.Console, null);

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(200);
                cl.Log(Level.Info, "I am " + Task.CurrentId + " with logger " + cl.LoggerId + " and I write to Console: This is my " + i + " message");
                cl.LogWarn("I am " + Task.CurrentId + " with logger " + cl.LoggerId + " and I write to Console: This is the " + i + " test");
            }
        }

        private static void CreateFileLogger()
        {
            FileLogger cl = (FileLogger)LogManager.InstantiateLogger(Destination.File, AppContext.BaseDirectory + "Logs.log");

            for (int i = 0; i < 1000; i++)
            {
                Thread.Sleep(200);
                cl.Log(Level.Info, "I am " + Task.CurrentId + " with logger " + cl.LoggerId + "  and I write to File: This is my " + i + " message");
                cl.LogWarn("I am " + Task.CurrentId + " with logger " + cl.LoggerId + " and I write to Console: This is the " + i + " test");
            }
        }

        //private static void CreateDBLogger()
        //{
        //    DBLogger cl = (DBLogger)LogManager.InstantiateLogger(Destination.Database, "MyConnection");

        //    for (int i = 0; i < 1000; i++)
        //        cl.Log(Level.Info, "I am " + Task.CurrentId + " with logger " + cl.LoggerId + "  and I write to DB: This is my " + i + " message");
        //}

        //private static void CreateEWLogger()
        //{
        //    EventViewerLogger cl = (EventViewerLogger)LogManager.InstantiateLogger(Destination.EventViewer, null);

        //    for (int i = 0; i < 1000; i++)
        //    {
        //        Thread.Sleep(100);
        //        cl.Log(Level.Info, "I am " + Task.CurrentId + " with logger " + cl.LoggerId + "  and I write to DB: This is my " + i + " message");
        //        cl.LogWarn("I am " + Task.CurrentId + " with logger " + cl.LoggerId + " and I write to Console: This is the " + i + " test");
        //    }
        //}
    }
}
