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
        public int LastCode { get; private set; } = -1;

        public LZWProcessor()
        {
            this.Table = new BidirectionalDictionary<int, List<byte>>(null, new EnumerableEqualityComparer<byte>());
            this.EncodeBuilder = new List<byte>();

            for (int i = byte.MinValue; i <= byte.MaxValue; i++)
            {
                this.Table[i] = new List<byte>() { (byte)i };
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns>Table Key of Inserted values</returns>
        public int InsertToTable(List<byte> values)
        {
            var key = this.Table.Count;
            this.Table[key] = values.ToList();
            return key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns>Table Key of Inserted value, -1 mean 'Require More Values'</returns>
        public int Encode(int values)
        {
            var lastCode = this.LastCode;

            if (values <= -1)
            {
                this.LastCode = -1;
                return lastCode;
            }
            else
            {
                var table = this.Table;
                var byteValue = (byte)values;
                var builder = this.EncodeBuilder;
                builder.Add(byteValue);

                if (table.TryGetA(builder, out var code) == true)
                {
                    this.LastCode = code;
                    return -1;
                }
                else
                {
                    this.InsertToTable(builder);
                    builder.Clear();
                    builder.Add(byteValue);

                    this.LastCode = values;
                    return lastCode;
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
            var builder = new List<byte>();

            if (code <= -1)
            {
                return -1;
            }
            else
            {
                var table = this.Table;
                var lastCode = this.LastCode;
                this.LastCode = code;

                if (lastCode > -1)
                {
                    builder.AddRange(table[lastCode]);
                }

                if (table.ContainsA(code) == true)
                {
                    if (lastCode > -1)
                    {
                        builder.Add(table[code][0]);
                        this.InsertToTable(builder);
                    }

                    return code;
                }
                else
                {
                    builder.Add(table[lastCode][0]);
                    var insertedKey = this.InsertToTable(builder);

                    return insertedKey;
                }

            }

        }

    }

}
