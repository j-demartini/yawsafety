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
            Activated = true;
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

            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Broadcast, 25565));    
            while(Activated)
            {
                Console.WriteLine("Transmitted");
                byte[] statusData = Encoding.ASCII.GetBytes("YAWSAFETY:10.33.7.22");
                socket.Send(statusData);
            }

        }

        public void StopChair()
        {
            client.Client.Send([0xA2]);
            Console.WriteLine("Chair disconnected.");
            Thread.Sleep(1000);
            client.Close();
            Activated = false;
        }

        public void ReceiveData()
        {
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 28067));
            while (Activated)
            {
                byte[] data = new byte[1024];
                socket.Receive(data, SocketFlags.None);
                ParseYaw(Encoding.ASCII.GetString(data));
            }
            socket.Close();
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
                PreviousEntry = now;
                lastChairYaw = ChairYaw;
                Moving = MathF.Abs(vel) > 12;
            }

        }


    }
}
