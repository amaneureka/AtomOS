using System;
using System.Collections.Generic;
using System.Linq;

using Atomixilc.Machine;

namespace Atomixilc
{
    internal class Optimizer
    {
        internal Stack<StackItem> vStack;
        internal readonly Options Config;
        internal readonly Queue<int> ILQueue;
        internal readonly Dictionary<int, Stack<StackItem>> StackState;

        internal Optimizer(Options aConfig, Queue<int> aILQueue)
        {
            Config = aConfig;
            ILQueue = aILQueue;
            StackState = new Dictionary<int, Stack<StackItem>>();

            StackState.Add(0, new Stack<StackItem>());
        }

        internal void SaveStack(int aNextPosition)
        {
            if (aNextPosition < 0) return;

            if (vStack == null)
                throw new Exception("Invalid SaveStack call!");

            if (!StackState.ContainsKey(aNextPosition))
                StackState.Add(aNextPosition, vStack);
            else
            {
                var oldStack = StackState[aNextPosition];
                StackState[aNextPosition] = Merge(oldStack, vStack, aNextPosition);
            }
            ILQueue.Enqueue(aNextPosition);
        }

        internal void LoadStack(int aPosition)
        {
            vStack = null;
            StackState.TryGetValue(aPosition, out vStack);
            if (vStack == null)
                throw new Exception(string.Format("StackState not found! '{0}'", aPosition));
            vStack = new Stack<StackItem>(vStack.Reverse());
        }

        private Stack<StackItem> Merge(Stack<StackItem> itemA, Stack<StackItem> itemB, int aNextPosition)
        {
            var listA = itemA.ToList();
            var listB = itemB.ToList();

            if (listA.Count != listB.Count)
                throw new Exception(string.Format("Can't merge two different size stacks '{0}' => {1} {2}", aNextPosition, listA.Count, listB.Count));

            var aStack = new Stack<StackItem>();

            int n = listA.Count;
            for (int i = 0; i < n; i++)
            {
                if (listA[i].Equals(listB[i]))
                {
                    aStack.Push(listA[i]);
                    continue;
                }
                aStack.Push(new StackItem(listA[i].OperandType));
            }

            return aStack;
        }
    }
}
