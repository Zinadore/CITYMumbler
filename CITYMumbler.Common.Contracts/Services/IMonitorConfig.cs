using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Common.Contracts.Services
{
    /// <summary>
    /// A Service that monitors the application config and reload the definded sections on runtime
    /// </summary>
    public interface IMonitorConfig
    {
        /// <summary>
        /// Fires when the application config has changed.
        /// </summary>
        event EventHandler OnConfigChanged;
    }
}
