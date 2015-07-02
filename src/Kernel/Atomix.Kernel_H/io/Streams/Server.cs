using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.io.FileSystem;

namespace Atomix.Kernel_H.io.Streams
{
    public class Server
    {
        public readonly Pipe ServerStream;
        public readonly uint ChunkSize;
        public readonly uint PacketMagic;

        public readonly IList<Client> Connections;
        private int ClientsCount;
        
        public Server(string path, uint packetSize, int MaximumClient, uint MagicNo)
        {
            this.ServerStream = new Pipe(packetSize, 0x10000, FileAttribute.READ_WRITE_CREATE);//VirtualFileSystem.Open(path, FileAttribute.READ_WRITE_CREATE);
            if (!VirtualFileSystem.Mount(path, ServerStream))
                Debug.Write("Server Stream Mount Failed!\n");
            else
                VirtualFileSystem.Open(path, FileAttribute.READ_WRITE_CREATE);

            this.ChunkSize = packetSize;
            this.Connections = new IList<Client>(MaximumClient);
            this.PacketMagic = MagicNo;
            this.Connections.Add(null);
        }

        /// <summary>
        /// Data to be sent to server by the client
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(byte[] data)
        {
            return ServerStream.Write(data, 0);
        }

        /// <summary>
        /// Data to be recieved by the server
        /// </summary>
        /// <returns></returns>
        public bool Receive(byte[] packet)
        {
            while (!ServerStream.Read(packet, 0)) ;
            return true;
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
            var Stream = new Pipe(8, 0x10000, FileAttribute.READ_WRITE_CREATE);
            if (!VirtualFileSystem.Mount(path, Stream))
                Debug.Write("Client Stream Mount Failed!\n");
            else
                VirtualFileSystem.Open(path, FileAttribute.READ_WRITE_CREATE);//Mark it in use
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
