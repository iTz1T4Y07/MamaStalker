using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
                Console.WriteLine($"Tried to save screenshot. result: [{SaveBitmap(receivedData)}]");
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

        private bool SaveBitmap(byte[] data)
        {
            ImageConverter converter = new ImageConverter();
            Bitmap bitmap;
            try
            {
                bitmap = (Bitmap) converter.ConvertFrom(data);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine($"Failed to convert received data to bitmap. {e}");
                // Change to logger
                return false;
            }
            
            bitmap.Save(DateTime.Now.ToString("F"), ImageFormat.Jpeg);
            return true;

        }
    }
}
