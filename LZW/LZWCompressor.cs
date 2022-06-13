using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZW
{
    public class LZWCompressor
    {
        public BidirectionalDictionary<int, List<byte>> Table { get; }
        private readonly List<byte> Builder;
        public int LastCode { get; private set; } = -1;

        public LZWCompressor()
        {
            this.Table = new BidirectionalDictionary<int, List<byte>>(null, new EnumerableEqualityComparer<byte>());
            this.Builder = new List<byte>();

            for (int i = byte.MinValue; i <= byte.MaxValue; i++)
            {
                this.Table[i] = new List<byte>() { (byte)i };
            }

        }

        public int Write(int value)
        {
            if (value <= -1)
            {
                return this.LastCode;
            }
            else
            {
                var byteValue = (byte)value;
                this.Builder.Add(byteValue);

                if (this.Table.TryGetA(this.Builder, out var code) == true)
                {
                    this.LastCode = code;
                    return -1;
                }
                else
                {
                    this.Table[this.Table.Count] = this.Builder.ToList();
                    this.Builder.Clear();
                    this.Builder.Add(byteValue);

                    var lastCode = this.LastCode;
                    this.LastCode = value;
                    return lastCode;
                }

            }

        }

    }

}
