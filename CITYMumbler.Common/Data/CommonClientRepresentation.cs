using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Common.Data
{
	/// <summary>
	/// Used a common traslation between the client side and server side representation of the client.
	/// </summary>
	public class CommonClientRepresentation
    {
		/// <summary>
		/// The id of the client
		/// </summary>
        public ushort ID { get; set; }
		/// <summary>
		/// The name of the client
		/// </summary>
        public string  Name { get; set; }
    }
}
