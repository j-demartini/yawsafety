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
        public bool Moving { get; private set; }
        public bool Activated { get; private set; }

        private TcpClient client;
        private float lastChairYaw;
        private DateTime PreviousEntry;

        public YawController()
        {
            Instance = this;
            Task.Run(() => { ConnectToChair(); });
            Task.Run(() => { ReceiveData(); });
            Activated = true;
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
            Activated = true;
        }

        public void StopChair()
        {
            client.Client.Send([0xA2]);
            Console.WriteLine("Chair disconnected.");
            Activated = false;
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

            DateTime now = DateTime.Now;

            string split = message.Split("SY[")[1];
            int index = split.IndexOf("]SP");
            string splitAgain = split.Substring(0, index);
            ChairYaw = float.Parse(splitAgain);

        }

        public void Tick()
        {
            DateTime now = DateTime.Now;
            if(now.Subtract(PreviousEntry).Milliseconds >= 50)
            {
                float vel = (ChairYaw - lastChairYaw) / .05f;
                Console.WriteLine(vel);
                PreviousEntry = now;
                lastChairYaw = ChairYaw;
                Moving = MathF.Abs(vel) > 5;
            }

        }


    }
}
