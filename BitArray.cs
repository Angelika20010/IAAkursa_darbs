using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kursaDarbs
{
    public class BitArray
    {
        public int size;
        public byte[] bytes;

        private byte[] masks = new byte[] {0b00000001, 0b00000010, 0b00000100, 0b00001000,
                0b00010000, 0b00100000, 0b01000000, (byte) 0b10000000};

        public BitArray(int size)
        {
            this.size = size;
            int sizeInBytes = size / 8;
            if (size % 8 > 0)
            {
                sizeInBytes = sizeInBytes + 1;
            }
            bytes = new byte[sizeInBytes];
        }

        public BitArray(int size, byte[] bytes)
        {
            this.size = size;
            this.bytes = bytes;
        }

        public int get(int index)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            return (bytes[byteIndex] & masks[bitIndex]) != 0 ? 1 : 0;
        }

        public void set(int index, int value)
        {
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            if (value != 0)
            {
                bytes[byteIndex] = (byte)(bytes[byteIndex] | masks[bitIndex]);
            }
            else
            {
                bytes[byteIndex] = (byte)(bytes[byteIndex] & ~masks[bitIndex]);
            }
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                sb.Append(get(i) > 0 ? '1' : '0');
            }
            return sb.ToString();
        }

        public int getSize()
        {
            return size;
        }

        public int getSizeInBytes()
        {
            return bytes.Length;
        }

        public byte[] getBytes()
        {
            return bytes;
        }
    }
}
