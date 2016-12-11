using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Client
{
    /// <summary>
    /// An object representation of a client. Used client-side.
    /// </summary>
    public class Client
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
    }
}
