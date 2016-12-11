using System;

namespace CITYMumbler.Common.Contracts.Services.Logger
{
    /// <summary>
    /// A logger used to create log entries
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// The name of the logger. Matches the name of the class that uses it.
        /// </summary>
        string Name { get; }

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
        /// Creates a log entry.
        /// </summary>
        /// <param name="level">The logging level of the entry</param>
        /// <param name="message">The message of the entry</param>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Creates a log entry. Will use string.Format to format the message.
        /// </summary>
        /// <param name="level">The logging level of the entry</param>
        /// <param name="message">The string format</param>
        /// <param name="args">The arguments used to format the string.</param>
        void Log(LogLevel level, string message, params object[] args);
    }
}