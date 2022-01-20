using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MamaStalker.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter required port");
            int port;
            if (!int.TryParse(Console.ReadLine(), out port))
            {
                Console.WriteLine("Invalid port. Closing program!");
                return;
            }

            Console.WriteLine("Please enter a refresh interval in milliseconds.");
            int refreshInterval;
            if (!int.TryParse(Console.ReadLine(), out refreshInterval))
            {
                Console.WriteLine("Invalid refresh rate. Closing program!");
                return;
            }

            if (refreshInterval <= 0)
            {
                Console.WriteLine("Invalid refresh rate. Closing program!");
                return;
            }

            Server server = new Server(port, refreshInterval);
            server.Start().GetAwaiter().GetResult();
        }
    }
}
