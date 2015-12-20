using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.lib.ds;

namespace Atomix.Kernel_H.arch.x86
{
    public class shm_chunk
    {
        public uint RefCount;
        public uint[] Frames;
    }

    public class SHM
    {
        public const uint START = 0xB0000000;
        public const int LIMIT_TO_PROCESS = 0x10000 >> 5;//Maximum of 0x10000 frames starting from SHM_Start to any process

        static int ResourceKey;
        static IDictionary<shm_chunk> Nodes;
    
        public static void Install()
        {
            Nodes = new IDictionary<shm_chunk>();
            ResourceKey = Scheduler.GetResourceID();
        }

        public static unsafe uint Obtain(string aID, int Size)
        {
            Scheduler.SpinLock(ResourceKey);

            shm_chunk Current;
            if (!Nodes.Contains(aID))
                CreateNew(aID, Size);

            Current = Nodes[aID];
            Current.RefCount++;

            var ParentProcess = Scheduler.CurrentThread.Process;
            var shm_mapping = ParentProcess.shm_mapping;

            int FramesRequired = Current.Frames.Length;
            
            int CurrentFrameCount = 0;
            for (int i = 0; i < LIMIT_TO_PROCESS; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if ((shm_mapping[i] & (0x1 << j)) == 0)
                    {
                        CurrentFrameCount++;
                        if (CurrentFrameCount == FramesRequired)
                        {
                            int xOffset = (i << 5) + j - FramesRequired + 1;
                            uint xVirtualAddress = START + (uint)(xOffset * 0x1000);
                            uint xReturnAddress = xVirtualAddress;
                            UInt32* CurrentDirectory = Paging.CurrentDirectory;
                            var Frames = Current.Frames;
                            
                            int Index = 0;
                            while (Index++ < CurrentFrameCount)
                            {
                                Paging.AllocateFrame(Paging.GetPage(CurrentDirectory, xVirtualAddress, true), Frames[Index] * 0x1000, false);
                                Paging.InvalidatePageAt(xVirtualAddress);
                                
                                //Also Mark in shm_mapping
                                shm_mapping[(xOffset >> 5)] |= (uint)(0x1 << (xOffset & 31));

                                xVirtualAddress += 0x1000;
                                xOffset++;
                            }
                            Scheduler.SpinUnlock(ResourceKey);
                            return xReturnAddress;
                        }
                    }
                    else
                    {
                        CurrentFrameCount = 0;
                    }
                }
            }

            Debug.Write("shm_mapping failed, Process id:=%d ", ParentProcess.pid);
            Debug.Write("shm_id := %s\n", aID);
            Scheduler.SpinUnlock(ResourceKey);

            return 0;
        }

        private static void CreateNew(string aID, int Size)
        {
            int NumberOfFrames = (Size / 0x1000) + (Size % 0x1000 == 0 ? 0 : 1);

            var NewChunk = new shm_chunk();
            NewChunk.RefCount = 0;
            NewChunk.Frames = new uint[NumberOfFrames];

            for (int i = 0; i < NumberOfFrames; i++)
            {
                //Allocate New Frame to this guy!
                uint NewFrame = Paging.FirstFreeFrame();
                Paging.SetFrame(NewFrame);

#warning Check If we are not out of run of memory
                NewChunk.Frames[i] = NewFrame;
            }

            Nodes.Add(aID, NewChunk);
        }
    }
}
