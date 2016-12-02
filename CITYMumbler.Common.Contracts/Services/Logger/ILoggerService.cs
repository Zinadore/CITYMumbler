using System;

namespace CITYMumbler.Common.Contracts.Services.Logger
{
    public interface ILoggerService
    {
        IObservable<LogEntry> Entries { get; }
        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        bool IsErrorEnabled { get; }

        LogLevel Threshold { get; set; }
        ILogger GetLogger(Type ofType);

        ILogger GetLogger(string name);
    }

}
