using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Core
{
    /// <summary>
    /// 数据流
    /// 采用大端模式存储
    /// </summary>
    public class DataStream : MemoryStream
    {
        //对象池
        private static Queue<DataStream> pool = new Queue<DataStream>();
        public static int PoolMaxCount = 200;

        private DataStream()
        {
        }

        /// <summary>
        /// 从对象池中获取DataStream
        /// </summary>
        public static DataStream Allocate()
        {
            //从对象池中获取DataStream
            lock (pool)
            {
                if (pool.Count > 0)
                {
                    return pool.Dequeue();
                }
            }
            //如果对象池中没有DataStream，则创建一个新的DataStream
            return new DataStream();
        }

        public static DataStream Allocate(byte[] bytes)
        {
            DataStream stream = Allocate();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return stream;
        }



        public ushort ReadUShort()
        {
            byte[] bytes = new byte[2];
            this.Read(bytes, 0, 2);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes, 0);
        }
        public uint ReadUInt()
        {
            byte[] bytes = new byte[4];
            this.Read(bytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
        public ulong ReadULong()
        {
            byte[] bytes = new byte[8];
            this.Read(bytes, 0, 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
        public short ReadShort()
        {
            byte[] bytes = new byte[2];
            this.Read(bytes, 0, 2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }
        public int ReadInt()
        {
            byte[] bytes = new byte[4];
            this.Read(bytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        public long ReadLong()
        {
            byte[] bytes = new byte[8];
            this.Read(bytes, 0, 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
        public void WriteUShort(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 2);
        }
        public void WriteUInt(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 4);
        }
        public void WriteULong(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 8);
        }
        public void WriteShort(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 2);
        }
        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 4);
        }
        public void WriteLong(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            this.Write(bytes, 0, 8);
        }






        protected override void Dispose(bool disposing)
        {
            //Log.Information("DataStream自动释放");
            lock (pool)
            {
                if (pool.Count < PoolMaxCount)
                {
                    this.Position = 0;
                    this.SetLength(0);
                    pool.Enqueue(this);
                }
                else
                {
                    base.Dispose(disposing);
                    this.Close();
                }
            }

        }
    }
}
