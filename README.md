# MTLogger

## Introduction

This is a simple logging framework able to work in a multithreaded environment.

## Technologies

.Net Core 3.1

## Launch

The project is a class library so it can be attached to an existing solution.

# Project structure

The project is composed of:

- LogManager: A static class which works as entry point for external applications using the logger
- LoggerBase: Base class containing the common properties and methods for the logger implementations
- ConsoleLogger: A logger writing to the console
- FileLogger: A logger writing to a file

The loggers can log messages with 1 of 4 available levels:

- Info
- Warning
- Error
- Critical

## LogManager

It is a static class with the role of instantiating the logger requested by an external application. 
Loggers are kept in a concurrent dictionary.

## LoggerBase

This class contains the base class for loggers and a further implementation of the .NET class ConcurrentQueue<T>.
PrepareMessage() is a method creating a struct containing the message and pertaining information with the best consistency possible, e.g.: the DateTime of the message (in UTC) is prepared before enqueuing hence dequeuing/actually writing.
IDisposable has been implemented in order to kill the Queue Tasks when the logger is not anymore in use or the "father"class gets unreferenced.

### Class LogQueue<T>

This class extends the ConcurrentQueue<T> class making use of an AutoResetEvent to synchronize the producer/consumer operations avoiding a high usage of CPU.

## ConsoleLogger

This class contains a static LogQueue in order to keep all messages from eventual multiple threads in a single (ordered) queue.
The constructor is in charge of creating the Task (System.Threading) that will work as consumer of the queue.
The producer of the queue is the class itself in the parent thread of the consumer.

## FileLogger

This class makes use of a static ConcurrentDictionary containing a queue for each file that will contain logs.
It is possible also log messages in the same files by different threads.


#Flow Sample

![Flow]()



This project has been written in a very short time so it is of course open to improvements and further stress test, anyone contributing is welcome.
