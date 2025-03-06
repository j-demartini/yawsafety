using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace YawSafety
{
    internal class YawController
    {
        public YawController()
        {
            Task.Run(() => { ConnectToChair(); });
            Task.Run(() => { ReceiveData(); });
        }

        public void ConnectToChair()
        {
            TcpClient client = new TcpClient();
            Console.WriteLine("Connecting...");
            client.Connect("10.33.5.135", 50020);
            List<byte> data = new List<byte>();
            data.Add(0x33);
            byte[] commandData = BitConverter.GetBytes(28067);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(commandData);

            data.AddRange(commandData);

            client.Client.Send(data.ToArray());
            Console.WriteLine("Connected.");
            //while (true)
            //{
            //    Console.WriteLine("Receiving...");
            //    byte[] buffer = new byte[1024];
            //    client.Client.Receive(buffer);
            //    Console.WriteLine("Received: " + System.Text.Encoding.ASCII.GetString(buffer));
            //    client.Close();
            //    break;
            //}
        }

        public void ReceiveData()
        {
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 28067));
            while (true)
            {
                byte[] data = new byte[1024];
                socket.Receive(data, SocketFlags.None);
                Console.WriteLine("RECEIVED: " + Encoding.ASCII.GetString(data));
            }
        }


    }
}
