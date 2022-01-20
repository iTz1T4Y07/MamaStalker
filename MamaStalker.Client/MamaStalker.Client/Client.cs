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
            byte[] dataLength = new byte[sizeof(int)];
            int recv = await stream.ReadAsync(dataLength, 0, dataLength.Length);
            if (recv == sizeof(int))
            {
                int messageLen = BitConverter.ToInt32(dataLength, 0);
                byte[] receivedData = new byte[messageLen];
                recv = await stream.ReadAsync(receivedData, 0, receivedData.Length);
                return receivedData;
            }
            return new byte[] { 0 };
            
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

            string fileName = DateTime.Now.ToString("dd.MM.yyyy hh..mm..ss");
            bitmap.Save($"{fileName}.jpg", ImageFormat.Jpeg);
            return true;

        }
    }
}
