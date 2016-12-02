using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Common.Services.Logger;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace CITYMumbler.UnitTests.Services
{
    [TestFixture]
    public class LogServiceFixture
    {
        [Test]
        public void is_debug_enabled_reflects_the_threshold_set()
        {
            var service = new LoggerService();

            service.Threshold = LogLevel.Debug;
            Assert.True(service.IsDebugEnabled);

            service.Threshold = LogLevel.Info;
            Assert.False(service.IsDebugEnabled);
        }
        [Test]
        public void is_info_enabled_reflects_the_threshold_set()
        {
            var service = new LoggerService();

            service.Threshold = LogLevel.Info;
            Assert.True(service.IsInfoEnabled);

            service.Threshold = LogLevel.Warn;
            Assert.False(service.IsInfoEnabled);
        }
        [Test]
        public void is_warn_enabled_reflects_the_threshold_set()
        {
            var service = new LoggerService();

            service.Threshold = LogLevel.Warn;
            Assert.True(service.IsWarnEnabled);

            service.Threshold = LogLevel.Error;
            Assert.False(service.IsWarnEnabled);
        }
        [Test]
        public void is_error_enabled_is_true_on_every_threshold()
        {
            var service = new LoggerService();

            service.Threshold = LogLevel.Debug;
            Assert.True(service.IsErrorEnabled);
            service.Threshold = LogLevel.Info;
            Assert.True(service.IsErrorEnabled);
            service.Threshold = LogLevel.Warn;
            Assert.True(service.IsErrorEnabled);
            service.Threshold = LogLevel.Error;
            Assert.True(service.IsErrorEnabled);
        }
        [Test]
        public void get_logger_throws_on_null_type()
        {
            var service = new LoggerService();
            Assert.Throws<ArgumentNullException>(() => service.GetLogger((Type) null));
        }
        [Test]
        public void get_logger_throws_on_null_name()
        {
            var service = new LoggerService();
            Assert.Throws<ArgumentNullException>(() => service.GetLogger((string) null));
        }
        [Test]
        public void get_logger_returns_a_logger_with_the_full_name_of_the_type_as_its_name()
        {
            var service = new LoggerService();
            var logger = service.GetLogger(this.GetType());
            Assert.AreEqual(this.GetType().FullName, logger.Name);
        }
        [Test]
        public async Task log_entries_ticks_for_calls_within_the_threshold()
        {
            var service = new LoggerService();
            service.Threshold = LogLevel.Debug;
            var logger = service.GetLogger("test");

            var entriesTask = service
                .Entries
                .Take(3)
                .ToListAsync()
                .ToTask();

            service.Threshold = LogLevel.Info;
            logger.Log(LogLevel.Debug, "Debug1");
            logger.Log(LogLevel.Debug ,"Debug2");
            logger.Log(LogLevel.Debug ,"Debug3");
            logger.Log(LogLevel.Info ,"Info1");
            logger.Log(LogLevel.Debug ,"Debug3");
            logger.Log(LogLevel.Warn ,"Warn1");
            logger.Log(LogLevel.Debug ,"Debug3");
            logger.Log(LogLevel.Error ,"Error1");

            var entries = await entriesTask;

            Assert.AreEqual("Info1", entries[0].Message);
            Assert.AreEqual(LogLevel.Info, entries[0].Level);
            Assert.AreEqual("Warn1", entries[1].Message);
            Assert.AreEqual(LogLevel.Warn, entries[1].Level);
            Assert.AreEqual("Error1", entries[2].Message);
            Assert.AreEqual(LogLevel.Error, entries[2].Level);
        }
        [Test]
        public async Task log_entries_can_be_formatted()
        {
            var service = new LoggerService();
            var logger = service.GetLogger("test");

            var entryTask = service
                .Entries
                .Take(1)
                .FirstAsync()
                .ToTask();

            logger.Log(LogLevel.Debug, "A message with a parameter: {0}", 42);

            var entry = await entryTask;

            Assert.AreEqual("A message with a parameter: 42", entry.Message);
        }


    }
}
