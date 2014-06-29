using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atomix.Assembler
{
    public class VirtualStack
    {
        protected Stack<Items> aStack;
        public VirtualStack()
        {
            aStack = new Stack<Items>();
        }

        public int Count
        { get { return aStack.Count; } }

        public void Clear()
        {
            aStack.Clear();
        }

        public Items Pop()
        {
            return aStack.Pop();
        }

        public Items Peek()
        {
            return aStack.Peek();
        }

        public void Push(int aSize, Type aType = null)
        {
            aStack.Push(new Items(aSize, aType));
        }
    }

    public class Items
    {
        public readonly int Size;
        public readonly Type Type;
        public readonly bool IsFloat;
        public readonly bool IsSigned;

        public readonly bool IsInteger;

        public Items(int aSize, Type aType)
        {
            this.Size = aSize;
            this.Type = aType;
            //C# Data Types sbyte, byte, short, ushort, int, uint, long, ulong, char, float, double, decimal, bool
            //http://www.blackwasp.co.uk/CSharpNumericDataTypes.aspx

            IsInteger = (aType == typeof(bool)
                || aType == typeof(byte)
                || aType == typeof(sbyte)
                || aType == typeof(short)
                || aType == typeof(ushort)
                || aType == typeof(int)
                || aType == typeof(uint)
                || aType == typeof(long)
                || aType == typeof(ulong));
            IsFloat = (aType == typeof(float) 
                || aType == typeof(double)
                || aType == typeof(decimal));
            IsSigned = (aType == typeof(sbyte)
                || aType == typeof(short)
                || aType == typeof(int)
                || aType == typeof(long)
                || aType == typeof(float)
                || aType == typeof(double)
                || aType == typeof(decimal));
        }
    }
}
