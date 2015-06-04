using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.io.FileSystem;

namespace Atomix.Kernel_H.io.Streams
{
    public class Client
    {
        public readonly int UniqueID;
        private readonly Stream Stream;
        private readonly Server Connection;
        public readonly uint MagicNumber;

        private uint ReadPointer;
        private uint WritePointer;

        bool Connected;
        byte[] Packet;

        public bool IsConnected
        { get { return Connected; } }

        public Client(Stream ss, Server Parent, int uid, uint Magic)
        {
            this.UniqueID = uid;
            this.Stream = ss;
            this.Connection = Parent;
            this.MagicNumber = Magic;
            this.Packet = new byte[Parent.ChunkSize];
            Connected = true;

            //Unique ID
            Packet[0] = (byte)UniqueID;
            Packet[1] = (byte)(UniqueID >> 8);
            Packet[2] = (byte)(UniqueID >> 16);
            Packet[3] = (byte)(UniqueID >> 24);

            //Magic
            Packet[4] = (byte)Magic;
            Packet[5] = (byte)(Magic >> 8);
            Packet[6] = (byte)(Magic >> 16);
            Packet[7] = (byte)(Magic >> 24);

            ReadPointer = 0;
            WritePointer = 0;
        }

        public void DisConnect()
        {
            Connected = false;
            Connection.DisConnect(UniqueID);
#warning TODO: Close Stream and deallocate memory
        }

        /// <summary>
        /// Send message to server (by client)
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool SendtoServer(byte[] Data)
        {
            if (!Connected)
                return false;

            int p = 0;
            for (int i = 8; i < Connection.ChunkSize; i++)
                Packet[i] = Data[p++];
            return Connection.Send(Packet);
        }

        /// <summary>
        /// Send reply to client (by server)
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool SendReply(byte[] Data)
        {
            if (Data.Length != Connection.ChunkSize)
                return false;

            if (WritePointer + Connection.ChunkSize >= 0x1000)
                return false;

            if (Stream.Write(Data, WritePointer))
            {
                WritePointer += (uint)Data.Length;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reply recieved by the client (from server)
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool RecieveReply(byte[] Data)
        {
            if (!Connected)
                return false;

            if (Data.Length != Connection.ChunkSize)
                return false;

            if (ReadPointer + Connection.ChunkSize >= 0x1000)
                return false;

            if (Stream.Read(Data, ReadPointer))
            {
                ReadPointer += (uint)Data.Length;
                return true;
            }
            return false;
        }
    }
}
