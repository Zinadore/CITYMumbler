using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Client;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            MumblerClient client = new MumblerClient();

            client.Connect();

            while (running)
            {
                var input = Console.ReadLine();
                if (input.Equals("/exit"))
                {
                    running = false;
                }
                else
                {
                    client.Send(input);
                }
            }
        }
    }
}
