using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.io.FileSystem;

namespace Atomix.Kernel_H.io.Streams
{
    public class Server
    {
        public readonly Stream ServerStream;
        public readonly uint ChunkSize;
        public readonly uint PacketMagic;
        private uint ReadPointer;
        private uint WritePointer;

        public readonly IList<Client> Connections;
        private int ClientsCount;
        
        public Server(string path, uint packetSize, int MaximumClient, uint MagicNo)
        {
            this.ServerStream = VirtualFileSystem.Open(path, FileAttribute.READ_WRITE_CREATE);
            this.ChunkSize = packetSize;
            this.Connections = new IList<Client>(MaximumClient);
            this.PacketMagic = MagicNo;
            this.Connections.Add(null);

            ReadPointer = 0;
            WritePointer = 0;
            ClientsCount = 0;
        }

        /// <summary>
        /// Data to be sent to server by the client
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(byte[] data)
        {
            if (data.Length != ChunkSize)
                return false;

            if (WritePointer + ChunkSize >= 0x1000)
                WritePointer = 0;

            if (ServerStream.Write(data, WritePointer))
            {
                WritePointer += ChunkSize;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Data to be recieved by the server
        /// </summary>
        /// <returns></returns>
        public bool Receive(byte[] packet)
        {
            if (packet.Length != ChunkSize)
                return false;

            if (ReadPointer + ChunkSize >= 0x1000)
                ReadPointer = 0;

            while (ReadPointer == WritePointer) ;//Hang up server if no message to read

            if (ServerStream.Read(packet, ReadPointer))
            {
                ReadPointer += ChunkSize;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Data to be sent by the server and recieved by the clients
        /// </summary>
        /// <returns></returns>
        public bool BroadCast(int uid, byte[] packet)
        {
            if (uid == 0)
            {
                //Send to everybody
                for (int i = 1; i <= ClientsCount; i++)
                {
                    var client = Connections[i];
                    if (client != null)
                        client.SendReply(packet);
                }
                return true;//Assuming all will get the message
            }

            return Connections[uid].SendReply(packet);
        }

        public Client CreateConnection(string path)
        {
#warning check permissions for server on stream also check for ACK
            var Stream = VirtualFileSystem.Open(path, FileAttribute.READ_WRITE_CREATE);
            if (Stream == null)
                return null;
            var client = new Client(Stream, this, ++ClientsCount, PacketMagic);
            Connections.Add(client);
            return client;
        }

        public void DisConnect(int uid)
        {
            Connections[uid] = null;
        }
    }
}
