/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Handles Shared memory allocation stuffs
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomixilc.Lib;

using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.Arch.x86
{
    internal class shm_chunk
    {
        public uint RefCount;
        public uint[] Frames;
    }

    internal class SHM
    {
        public const uint START = 0xB0000000;

        // Maximum of 0x10000 frames starting from SHM_Start to any process
        public const int LIMIT_TO_PROCESS = 0x10000 >> 5;

        static uint aLock;
        static IDictionary<string, shm_chunk> Nodes;

        internal static void Install()
        {
            Nodes = new IDictionary<string, shm_chunk>(Internals.GetHashCode, string.Equals);
        }

        internal static unsafe uint Obtain(string aID, uint aSize, bool aDoAllocate = false)
        {
            Monitor.AcquireLock(ref aLock);

            if (!Nodes.ContainsKey(aID))
            {
                if (!aDoAllocate)
                {
                    Monitor.ReleaseLock(ref aLock);
                    return 0;
                }
                CreateNew(aID, aSize);
            }

            shm_chunk Current;
            Current = Nodes[aID];
            Current.RefCount++;

            var ParentProcess = Scheduler.RunningThread.Process;
            var shm_mapping = ParentProcess.shm_mapping;

            int FramesRequired = Current.Frames.Length;
            int CurrentFrameCount = 0;
            for (int i = 0; i < LIMIT_TO_PROCESS; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    uint index = (uint)(1 << j);
                    if ((shm_mapping[i] & index) == 0)
                    {
                        CurrentFrameCount++;
                        if (CurrentFrameCount == FramesRequired)
                        {
                            int xOffset = (i << 5) + j - FramesRequired + 1;
                            uint xVirtualAddress = START + (uint)(xOffset << 12);
                            uint xReturnAddress = xVirtualAddress;
                            var CurrentDirectory = Paging.CurrentDirectory;
                            var Frames = Current.Frames;

                            int Index = 0;
                            while (Index < FramesRequired)
                            {
                                Paging.AllocateFrame(Paging.GetPage(CurrentDirectory, xVirtualAddress, true), (Frames[Index] << 12), false);
                                Paging.InvalidatePageAt(xVirtualAddress);

                                // Also Mark in shm_mapping
                                shm_mapping[(xOffset >> 5)] |= (uint)(0x1 << (xOffset & 31));

                                xVirtualAddress += 0x1000;
                                xOffset++;
                                Index++;
                            }
                            Monitor.ReleaseLock(ref aLock);
                            return xReturnAddress;
                        }
                    }
                    else
                    {
                        CurrentFrameCount = 0;
                    }
                }
            }

            Monitor.ReleaseLock(ref aLock);
            Debug.Write("shm_mapping failed, Process id:=%d ", ParentProcess.ID);
            Debug.Write("shm_id := %s ", aID);
            Debug.Write("FramesRequired: %d\n", (uint)FramesRequired);
            return 0;
        }

        private static void CreateNew(string aID, uint Size)
        {
            uint NumberOfFrames = Size / 0x1000;
            if ((Size % 0x1000) != 0) NumberOfFrames++;

            var NewChunk = new shm_chunk();
            NewChunk.RefCount = 0;
            NewChunk.Frames = new uint[NumberOfFrames];

            for (uint index = 0; index < NumberOfFrames; index++)
            {
                // Allocate New Frame to this guy!
                uint NewFrame = Paging.FirstFreeFrame();
                Paging.SetFrame(NewFrame);

#warning [SHM] : Check for memory out of run condition.

                NewChunk.Frames[index] = NewFrame;
            }
            Nodes.Add(aID, NewChunk);
        }
    }
}
