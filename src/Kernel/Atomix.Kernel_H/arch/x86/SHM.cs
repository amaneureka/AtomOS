/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, December 2015                                       *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.arch.x86                                                             *
*   File          ::  SHM.cs                                                                               *
*                                                                                                          *
*   Description                                                                                            *
*       Handles Shared memory allocation stuffs                                                            *
*                                                                                                          *
*   History                                                                                                *
*       20-12-2015      Aman Priyadarshi      Added Methods                                                *
*       04-01-2016      Aman Priyadarshi      CreateIfNotExist                                             *
*       22-01-2016      Aman Priyadarshi      Fixes                                                        *
*       24-03-2016      Aman Priyadarshi      File Header                                                  *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

using Atomix.Kernel_H.lib;
using Atomix.Kernel_H.core;
using Atomix.Kernel_H.lib.crypto;

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
        
        static IDictionary<string, shm_chunk> Nodes;
        
        public static void Install()
        {
            Nodes = new IDictionary<string, shm_chunk>(sdbm.GetHashCode, string.Equals);
        }

        public static unsafe uint Obtain(string aID, int aSize = -1)
        {
            Monitor.AcquireLock(Nodes);
            
            if (!Nodes.ContainsKey(aID))
            {
                if (aSize == -1)
                {
                    Monitor.ReleaseLock(Nodes);
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
                    if ((uint)(shm_mapping[i] & (0x1 << j)) == 0)
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
                                
                                //Also Mark in shm_mapping
                                shm_mapping[(xOffset >> 5)] |= (uint)(0x1 << (xOffset & 31));

                                xVirtualAddress += 0x1000;
                                xOffset++;
                                Index++;
                            }
                            Monitor.ReleaseLock(Nodes);
                            return xReturnAddress;
                        }
                    }
                    else
                    {
                        CurrentFrameCount = 0;
                    }
                }
            }

            Monitor.ReleaseLock(Nodes);
            Debug.Write("shm_mapping failed, Process id:=%d ", ParentProcess.pid);
            Debug.Write("shm_id := %s\n", aID);            
            return 0;
        }

        private static void CreateNew(string aID, int Size)
        {
            int NumberOfFrames = (Size / 0x1000) + (Size % 0x1000 == 0 ? 0 : 1);

            var NewChunk = new shm_chunk();
            NewChunk.RefCount = 0;
            NewChunk.Frames = new uint[NumberOfFrames];
            
            for (int index = 0; index < NumberOfFrames; index++)
            {
                //Allocate New Frame to this guy!
                uint NewFrame = Paging.FirstFreeFrame();
                Paging.SetFrame(NewFrame);
#warning Check If we are not out of run of memory
                NewChunk.Frames[index] = NewFrame;
                //Debug.Write(0x0);
            }
            Nodes.Add(aID, NewChunk);
        }
    }
}
