using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZW
{
    public class LZWProcessor
    {
        public BidirectionalDictionary<int, List<byte>> Table { get; }
        private readonly List<byte> EncodeBuilder;
        public int LastKey { get; private set; } = -1;

        public LZWProcessor()
        {
            this.Table = new BidirectionalDictionary<int, List<byte>>(null, new EnumerableEqualityComparer<byte>());
            this.EncodeBuilder = new List<byte>();

            for (int i = byte.MinValue; i <= byte.MaxValue; i++)
            {
                this.Table[i] = new List<byte>() { (byte)i };
            }

        }

        public virtual int NextKey => this.Table.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns>Table Key of Inserted values</returns>
        public int InsertToTable(List<byte> values)
        {
            var key = this.NextKey;
            this.Table[key] = values.ToList();
            return key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Table Key of Inserted value, -1 mean 'Require More Values'</returns>
        public int Encode(int value)
        {
            var lastKey = this.LastKey;

            if (value <= -1)
            {
                this.LastKey = -1;
                return lastKey;
            }
            else
            {
                var byteValue = (byte)value;
                var builder = this.EncodeBuilder;
                builder.Add(byteValue);

                if (this.Table.TryGetA(builder, out var key) == true)
                {
                    this.LastKey = key;
                    return -1;
                }
                else
                {
                    this.InsertToTable(builder);
                    builder.Clear();
                    builder.Add(byteValue);

                    this.LastKey = value;
                    return lastKey;
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Table Key of decoded data, -1 mean 'End Of Decode'</returns>
        public int Decode(int code)
        {
            if (code <= -1)
            {
                return -1;
            }
            else
            {
                var table = this.Table;
                var lastKey = this.LastKey;
                var builder = new List<byte>();

                if (lastKey > -1)
                {
                    builder.AddRange(table[lastKey]);
                }

                if (table.ContainsA(code) == true)
                {
                    if (lastKey > -1)
                    {
                        builder.Add(table[code][0]);
                        this.InsertToTable(builder);
                    }

                    this.LastKey = code;
                    return code;
                }
                else
                {
                    builder.Add(table[lastKey][0]);
                    var key = this.InsertToTable(builder);
                    this.LastKey = key;
                    return key;
                }

            }

        }

    }

}
