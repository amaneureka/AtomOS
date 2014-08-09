using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.Lib
{
    public class BinaryFormat
    {
        private byte[] data;

        public uint Length
        {
            get
            {
                return (uint)this.data.Length;
            }
        }

        public byte[] Data
        {
            get
            {
                return this.data;
            }
        }

        public byte this[int index]
        {
            get
            {
                return this.data[index];
            }
            set
            {
                this.data[index] = value;
            }
        }

        public BinaryFormat(byte[] data)
        {
            this.data = data;
        }

        public BinaryFormat(uint length)
        {
            this.data = new byte[(int)length];
        }

        public char GetChar(uint offset)
        {
            return (char)this.data[(int)offset];
        }

        public void SetChar(uint offset, char value)
        {
            this.data[(int)offset] = (byte)value;
        }

        public char[] GetChars(uint offset, uint length)
        {
            char[] chArray = new char[(int)length];
            for (uint index = 0U; index < length; ++index)
                chArray[(int)index] = this.GetChar(offset + index);
            return chArray;
        }

        public byte[] GetBytes(uint offset, uint length)
        {
            byte[] numArray = new byte[(int)length];
            for (uint index = 0U; index < length; ++index)
                numArray[(int)index] = this.data[(int)(offset + index)];
            return numArray;
        }

        public void SetChars(uint offset, char[] value)
        {
            for (uint index = 0U; (long)index < (long)value.Length; ++index)
                this.data[(int)(offset + index)] = (byte)value[(int)index];
        }

        public void SetString(uint offset, string value)
        {
            for (int index = 0; index < value.Length; ++index)
                this.data[(uint)offset + (uint)index] = (byte)value[index];
        }

        public void SetString(uint offset, string value, uint length)
        {
            for (int index = 0; (uint)index < (uint)length; ++index)
                this.data[(uint)offset + (uint)index] = (byte)value[index];
        }

        public uint GetUInt(uint offset)
        {
            return (uint)((int)this.data[(int)offset++] + ((int)this.data[(int)offset++] << 8) + ((int)this.data[(int)offset++] << 16) + ((int)this.data[(int)offset++] << 24));
        }

        public void SetUInt(uint offset, uint value)
        {
            this.data[(int)offset++] = (byte)(value & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 8 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 16 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 24 & (uint)byte.MaxValue);
        }

        public void SetUIntReversed(uint offset, uint value)
        {
            this.data[(int)offset++] = (byte)(value >> 24 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 16 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 8 & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value & (uint)byte.MaxValue);
        }

        public void SetULong(uint offset, ulong value)
        {
            this.data[(int)offset++] = (byte)(value & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 8 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 16 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 24 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 32 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 40 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 48 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 56 & (ulong)byte.MaxValue);
        }

        public void SetULongReversed(uint offset, ulong value)
        {
            this.data[(int)offset++] = (byte)(value >> 56 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 48 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 40 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 32 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 24 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 16 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value >> 8 & (ulong)byte.MaxValue);
            this.data[(int)offset++] = (byte)(value & (ulong)byte.MaxValue);
        }

        public ushort GetUShort(uint offset)
        {
            return (ushort)((int)this.data[(int)offset++] | (int)this.data[(int)offset++] << 8);
        }

        public void SetUShort(uint offset, ushort value)
        {
            this.data[(int)offset++] = (byte)((uint)value & (uint)byte.MaxValue);
            this.data[(int)offset++] = (byte)((int)value >> 8 & (int)byte.MaxValue);
        }

        public void SetUShortReversed(uint offset, ushort value)
        {
            this.data[(int)offset++] = (byte)((int)value >> 8 & (int)byte.MaxValue);
            this.data[(int)offset++] = (byte)((uint)value & (uint)byte.MaxValue);
        }

        public ulong GetULong(uint offset)
        {
            return (ulong)this.data[(int)offset++] + (ulong)((uint)this.data[(int)offset++] << 8) + (ulong)((uint)this.data[(int)offset++] << 16) + (ulong)((uint)this.data[(int)offset++] << 24) + (ulong)this.data[(int)offset++] + (ulong)((uint)this.data[(int)offset++] << 8) + (ulong)((uint)this.data[(int)offset++] << 16) + (ulong)((uint)this.data[(int)offset++] << 24);
        }

        public void SetULong(ulong offset, ulong value)
        {
            this.data[offset++] = (byte)(value & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 8 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 16 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 24 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 32 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 40 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 48 & (ulong)byte.MaxValue);
            this.data[offset++] = (byte)(value >> 56 & (ulong)byte.MaxValue);
        }

        public byte GetByte(uint offset)
        {
            return this.data[(int)offset];
        }

        public void SetByte(uint offset, byte value)
        {
            this.data[(int)offset] = value;
        }

        public void SetBytes(uint offset, byte[] value)
        {
            for (uint index = 0U; (long)index < (long)value.Length; ++index)
                this.data[(int)(offset + index)] = value[(int)index];
        }

        public void SetBytes(uint offset, byte[] value, uint start, uint length)
        {
            for (uint index = 0U; index < length; ++index)
                this.data[(int)(offset + index)] = value[(int)(start + index)];
        }

        public void Fill(uint offset, byte value, uint length)
        {
            for (uint index = 0U; index < length; ++index)
                this.data[(int)(offset + index)] = value;
        }
    }
}
