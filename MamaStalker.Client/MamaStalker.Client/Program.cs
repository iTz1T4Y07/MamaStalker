using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MamaStalker.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.Start(IPAddress.Loopback, 9090).GetAwaiter().GetResult();
        }
    }
}
