using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MamaStalker.Server
{
    public class Server
    {
        private readonly TcpListener _listener;

        private readonly IList<TcpClient> _clients;

        private readonly int _refreshInterval;

        public Server(int port, int RefreshInterval)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _clients = new List<TcpClient>();
            _refreshInterval = RefreshInterval;
        }

        public async Task Start()
        {
            _listener.Start(50);
            _listener.BeginAcceptTcpClient(new AsyncCallback(AcceptNewConnection), _listener);

            Task Timer = Task.Delay(_refreshInterval);
            await Timer.ContinueWith(task => SendDataToClients(GetScreenshotBytes())).ContinueWith(task => Timer);
            await Task.Delay(10000);

        }

        private void AcceptNewConnection(IAsyncResult asyncResult)
        {
            TcpListener listener = (TcpListener)asyncResult.AsyncState;
            TcpClient newClient = listener.EndAcceptTcpClient(asyncResult);
            _clients.Add(newClient);
        }

        private async Task SendDataToClients(byte[] data)
        {
            foreach (TcpClient client in _clients)
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    byte[] length = BitConverter.GetBytes(data.Length); // Getting data length
                    byte[] messageBuffer = new byte[sizeof(int) + data.Length];
                    for (int i = 0; i < sizeof(int); i++) //Adding data length to messageBuffer
                    {
                        messageBuffer[i] = length[i];
                    }
                    for (int i = 0; i < data.Length; i++) //Adding data to messageBuffer
                    {
                        messageBuffer[i + sizeof(int)] = data[i];
                    }

                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
        }

        private byte[] GetScreenshotBytes()
        {
            byte[] data;
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                data = (byte[])new ImageConverter().ConvertTo(bitmap, typeof(byte[]));
            }
            return data;
        }
    }
}
