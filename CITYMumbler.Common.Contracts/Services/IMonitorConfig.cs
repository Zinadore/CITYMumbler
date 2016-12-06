using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Common.Contracts.Services
{
    public interface IMonitorConfig
    {
        event EventHandler OnConfigChanged;
    }
}
