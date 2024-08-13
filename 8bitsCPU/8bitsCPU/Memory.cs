using System;
using System.IO;

namespace _8bitsCPU
{
    public class Memory
    {
        public static byte Data = 1;

        public static byte Code = 2;

        public byte[] MemoryData;

        public byte Segments;

        public Memory(int length, byte Segments)
        {
            MemoryData = new byte[length * Segments];
            this.Segments = Segments;
        }

        public ushort read(int address, byte Segment)
        {
            if(Segment == Code)
            {
                address += MemoryData.Length / 2;
            }

            //byte byte1 = MemoryData[address];
            //byte byte2 = MemoryData[address + 1];
            //ushort result = (ushort)((byte1 << 8) | byte2);

            return BitConverter.ToUInt16(MemoryData, address);
        }

        public void write(int address, byte Segment, ushort data)
        {
            if (Segment == Code)
            {
                address += MemoryData.Length / 2;
            }
            byte[] bytes = BitConverter.GetBytes(data);
            MemoryData[address] = bytes[0];
            MemoryData[address + 1] = bytes[1];

            //MemoryData[address] = (byte)(data >> 8);
            //MemoryData[(address + 1)] = (byte)(data & 0xFF);
        }

        public byte readByte(int address, byte Segment)
        {
            if (Segment == Code)
            {
                address += MemoryData.Length / 2;
            }

            return MemoryData[address];
        }

        public void writeByte(int address, byte Segment, byte data)
        {
            if (Segment == Code)
            {
                address += MemoryData.Length / 2;
            }

            MemoryData[address] = data;
        }


        public void load(string fileName)
        {
            fileName += ".rmy";
            try 
            { 
                MemoryData = File.ReadAllBytes(fileName);
            }
            catch(Exception e)
            {
                File.Create(fileName, 65535);
            }
        }

        public void save(string fileName)
        {
            fileName += ".rmy";
            File.WriteAllBytes(fileName, MemoryData);
        }
    }
}
