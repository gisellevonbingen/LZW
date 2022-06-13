using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZW
{
    public class LZWDecompressor
    {
        public BidirectionalDictionary<int, List<byte>> Table { get; }
        public int LastCode { get; private set; } = -1;

        public LZWDecompressor()
        {
            this.Table = new BidirectionalDictionary<int, List<byte>>(null, new EnumerableEqualityComparer<byte>());

            for (int i = byte.MinValue; i <= byte.MaxValue; i++)
            {
                this.Table[i] = new List<byte>() { (byte)i };
            }

        }

        public List<byte> Read(int code)
        {
            var builder = new List<byte>();

            if (code == -1)
            {
                return builder;
            }
            else
            {
                if (this.LastCode > -1)
                {
                    builder.AddRange(this.Table[this.LastCode]);
                }

                if (this.Table.TryGetB(code, out var values) == true)
                {
                    if (this.LastCode > -1)
                    {
                        builder.Add(this.Table[code][0]);
                        this.Table[this.Table.Count] = builder;
                    }

                    this.LastCode = code;
                    return values;
                }
                else
                {
                    builder.Add(this.Table[this.LastCode][0]);
                    this.Table[this.Table.Count] = builder;

                    this.LastCode = code;
                    return builder;
                }

            }

        }

    }

}
