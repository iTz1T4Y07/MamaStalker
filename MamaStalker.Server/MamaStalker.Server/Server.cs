using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

        public void Start()
        {
            _listener.Start(50);
            _listener.BeginAcceptTcpClient(new AsyncCallback(AcceptNewConnection), _listener);
            
        }

        private void AcceptNewConnection(IAsyncResult asyncResult)
        {
            TcpListener listener = (TcpListener) asyncResult.AsyncState;
            TcpClient newClient = listener.EndAcceptTcpClient(asyncResult);
            _clients.Add(newClient);            
        }

        private void SendDataToClients(byte[] data)
        {
            foreach (TcpClient client in _clients)
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    stream.WriteAsync(data, 0, data.Length);
                }
            }
        }
    }
}
