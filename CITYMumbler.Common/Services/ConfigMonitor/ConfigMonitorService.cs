using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Contracts.Services;

namespace CITYMumbler.Common.Services.ConfigMonitor
{
    public class ConfigMonitorService: IMonitorConfig
    {
        private FileSystemWatcher _watcher;
        private DateTime _lastChange;

        public event EventHandler OnConfigChanged;

        public ConfigMonitorService()
        {
            _lastChange = DateTime.MinValue;
            string configFile = string.Concat(System.Reflection.Assembly.GetEntryAssembly().Location, ".config");
            if (File.Exists(configFile))
            {
                _watcher = new FileSystemWatcher(Path.GetDirectoryName(configFile), Path.GetFileName(configFile));
                _watcher.EnableRaisingEvents = true;
                _watcher.Changed += watcher_Changed;
            }
        }

        private void watcher_Changed(object sender, FileSystemEventArgs args)
        {
            if ((DateTime.Now - _lastChange).Seconds > 3)
            {
                ConfigurationManager.RefreshSection("appSettings");
                this.OnConfigChanged?.Invoke(this, EventArgs.Empty);
            }
            _lastChange = DateTime.Now;
        }
    }
}
