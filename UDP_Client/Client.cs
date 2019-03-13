using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Client
{
    

    


    public class Client
    {
        private const int bufSize = 8 * 1024;
        private Socket _socket;
        private State _state;
        private EndPoint epFrom;
        private AsyncCallback recv = null;
        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public Client(string address, int port)
        {
            epFrom = new IPEndPoint(IPAddress.Any, 0);
            _state = new State();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _socket.Connect(IPAddress.Parse(address), port);

            Receive();
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(_state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes,
                    Encoding.ASCII.GetString(so.buffer, 0, bytes));
            }, _state);

        }

    }
}
