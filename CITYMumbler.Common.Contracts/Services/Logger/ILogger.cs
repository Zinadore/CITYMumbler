using System;

namespace CITYMumbler.Common.Contracts.Services.Logger
{
    public interface ILogger
    {
        string Name { get; }

        bool IsDebugEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        bool IsErrorEnabled { get; }

        void Log(LogLevel level, string message);
        void Log(LogLevel level, string message, params object[] args);
    }
}