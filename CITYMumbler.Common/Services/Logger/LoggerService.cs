﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Contracts.Services;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Common.Services.ConfigMonitor;
using HelperTrinity;

namespace CITYMumbler.Common.Services.Logger
{
    /// <summary>
    /// An Implementation of the ILoggerService using a ConfigMonitor to reconfigure itself at runtime
    /// </summary>
    public class LoggerService: ILoggerService
    {
        private LogLevel _threshold;
        private IDictionary<string, ILogger> loggers;
        private readonly object sync;
        private readonly ReplaySubject<LogEntry> entries;
        private readonly IMonitorConfig _configMonitor;

        public IObservable<LogEntry> Entries => this.entries.Where(entry => entry.Level >= this.Threshold);
        //public IObservable<LogEntry> Entries => this.entries;
        public bool IsDebugEnabled => this._threshold <= LogLevel.Debug;
        public bool IsInfoEnabled => this._threshold <= LogLevel.Info;
        public bool IsWarnEnabled => this._threshold <= LogLevel.Warn;
        public bool IsErrorEnabled => this._threshold <= LogLevel.Error;
        public LogLevel Threshold
        {
            get { return this._threshold; } 
            set { this._threshold = value; }
        }

        public LoggerService() {
            this.entries = new ReplaySubject<LogEntry>();
            this.loggers = new Dictionary<string, ILogger>();
            this.sync = new object();
            this._configMonitor = new ConfigMonitorService();
            var logSetting = ConfigurationManager.AppSettings["logLevel"];
            this._threshold = settingToLogLevel(logSetting);

            this._configMonitor.OnConfigChanged += (s, e) =>
            {
                var setting = ConfigurationManager.AppSettings["logLevel"];
                this._threshold = settingToLogLevel(logSetting);
            };
        }

        public ILogger GetLogger(Type forType)
        {
            forType.AssertNotNull("forType");
            if (forType.IsConstructedGenericType)
            {
                forType = forType.GetGenericTypeDefinition();
            }

            return this.GetLogger(forType.FullName);
        }

        public ILogger GetLogger(string name)
        {
            name.AssertNotNull("name");
            lock (this.sync)
            {
                ILogger logger;

                if (!this.loggers.TryGetValue(name, out logger))
                {
                    logger = new Logger(this, name);
                    this.loggers.Add(name, logger);
                }
                return logger;
            }
        }

        private LogLevel settingToLogLevel(string setting)
        {
            setting.AssertNotNull("setting");
            var num = (LogLevel)Enum.Parse(typeof(LogLevel), setting);
            if (num == LogLevel.Debug || num == LogLevel.Info || num == LogLevel.Warn || num == LogLevel.Error)
                return num;
            return LogLevel.Debug;
        }

        private sealed class Logger : ILogger
        {
            private readonly LoggerService owner;
            private readonly string name;

            public string Name => this.name;
            public bool IsDebugEnabled => this.owner.IsDebugEnabled;
            public bool IsInfoEnabled => this.owner.IsInfoEnabled;
            public bool IsWarnEnabled => this.owner.IsWarnEnabled;
            public bool IsErrorEnabled => this.owner.IsErrorEnabled;

            public Logger(LoggerService owner, string name)
            {
                this.owner = owner;
                this.name = name;
            }
            public void Log(LogLevel level, string message)
            {
                var entry = new LogEntry(DateTime.Now, this.name, level, Environment.CurrentManagedThreadId, message);
                this.owner.entries.OnNext(entry);
            }

            public void Log(LogLevel level, string message, params object[] args)
            {
                var formatted = string.Format(message, args);
                Log(level, formatted);
            }

            
        }
    }
}
