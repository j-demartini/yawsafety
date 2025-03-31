using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YawSafety
{
    internal class YawController
    {
 
        public static YawController Instance { get; private set; }
        public float ChairYaw { get; private set; }

        private TcpClient client;
        bool active;

        public YawController()
        {
            Instance = this;
            Task.Run(() => { ConnectToChair(); });
            Task.Run(() => { ReceiveData(); });
        }

        public void ConnectToChair()
        {
            client = new TcpClient();
            Console.WriteLine("Connecting...");
            client.Connect("10.33.7.22", 50020);
            List<byte> data = new List<byte>();
            data.Add(0x33);
            byte[] commandData = BitConverter.GetBytes(28067);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(commandData);

            data.AddRange(commandData);

            client.Client.Send(data.ToArray());
            Console.WriteLine("Connected.");
            active = true;
        }

        public void StopChair()
        {
            if(active)
            {
                active = false;
                client.Client.Send([0xA2]);
                Console.WriteLine("Chair disconnected.");
            }

        }

        public void ReceiveData()
        {
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 28067));
            while (true)
            {
                byte[] data = new byte[1024];
                socket.Receive(data, SocketFlags.None);
                ParseYaw(Encoding.ASCII.GetString(data));
            }
        }

        public void ParseYaw(string message)
        {
            string split = message.Split("SY[")[1];
            int index = split.IndexOf("]SP");
            string splitAgain = split.Substring(0, index);
            Console.WriteLine(splitAgain);
        }


    }
}
