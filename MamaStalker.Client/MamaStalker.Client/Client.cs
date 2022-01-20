using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MamaStalker.Client
{
    public class Client
    {
        private readonly TcpClient _client;

        public Client()
        {
            _client = new TcpClient();
        }        

        public async Task Start(IPAddress ip, int port)
        {
            _client.Connect(ip, port);
            while (_client.Connected)
            {
                byte[] receivedData = await GetData();

            }
        }

        private async Task<byte[]> GetData()
        {
            NetworkStream stream = _client.GetStream();
            if (!stream.CanRead) return new byte[] { 0 };
            byte[] receivedData = new byte[_client.ReceiveBufferSize];
            await stream.ReadAsync(receivedData, 0, receivedData.Length);
            return receivedData.Where(byteValue => byteValue != 0).ToArray();
        }
    }
}
