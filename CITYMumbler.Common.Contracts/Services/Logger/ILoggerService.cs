using System;

namespace CITYMumbler.Common.Contracts.Services.Logger
{
    /// <summary>
    /// A logging service used to create loggers and observe the entries they create.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// An observable of the entries created by all the loggers owned by this service.
        /// </summary>
        IObservable<LogEntry> Entries { get; }

        /// <summary>
        /// A boolean value that denotes whether Debug level logging is enabled
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// A boolean value that denotes whether Info level logging is enabled
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// A boolean value that denotes whether Warning level logging is enabled
        /// </summary>
        bool IsWarnEnabled { get; }

        /// <summary>
        /// A boolean value that denotes whether Error level logging is enabled
        /// </summary>
        bool IsErrorEnabled { get; }

        /// <summary>
        /// The logging level threshold of the service. No entries under this level will be produced.
        /// </summary>
        LogLevel Threshold { get; set; }

        /// <summary>
        /// Returns a logger to be used by the specified type
        /// </summary>
        /// <param name="ofType">The Type of the class using the logger</param>
        /// <returns>An implementation of ILogger with the name matching the calling type</returns>
        ILogger GetLogger(Type ofType);

        /// <summary>
        ///  Returns a logger to be used by the specified type
        /// </summary>
        /// <param name="name">The name of the class using the logger</param>
        /// <returns>An implementation of ILogger with the name matching the calling type</returns>
        ILogger GetLogger(string name);
    }

}
