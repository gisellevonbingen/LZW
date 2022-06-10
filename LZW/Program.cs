using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZW
{
    public static class Program
    {
        public static int Next(BidirectionalDictionary<int, List<byte>> encodeTable, ref int gen, Stream stream, ref int last)
        {
            var builder = new List<byte>();

            if (last > -1)
            {
                builder.Add((byte)last);
            }

            var lastCode = last;

            while (true)
            {
                var c = stream.ReadByte();
                last = c;

                if (c == -1)
                {
                    return lastCode;
                }
                else
                {
                    builder.Add((byte)c);

                    if (builder.Count == 1)
                    {
                        lastCode = c;
                    }
                    else if (encodeTable.TryGetA(builder, out var code) == false)
                    {
                        encodeTable[gen++] = builder;
                        return lastCode;
                    }
                    else
                    {
                        lastCode = code;
                    }

                }

            }

        }

        public static void Main(string[] args)
        {
            var input = "BABAABAAA";
            var inputBytes = Encoding.Default.GetBytes(input);
            var table = new BidirectionalDictionary<int, List<byte>>(null, new EnumerableEqualityComparer<byte>());

            var encodes = new List<int>();

            using (var encodeStream = new MemoryStream(inputBytes))
            {
                var gen = byte.MaxValue + 1;
                var last = -1;

                while (true)
                {
                    var tableValue = Next(table, ref gen, encodeStream, ref last);

                    if (tableValue == -1)
                    {
                        break;
                    }
                    else
                    {
                        encodes.Add(tableValue);
                    }

                }

            }

            Console.WriteLine();
            Console.WriteLine("===== Original String =====");
            Console.WriteLine(input);

            Console.WriteLine();
            Console.WriteLine("===== Original Bytes =====");

            foreach (var c in input)
            {
                Console.WriteLine($"0x{(int)c:X2}");
            }

            Console.WriteLine();
            Console.WriteLine("===== LZW Table =====");

            foreach (var pair in table)
            {
                Console.WriteLine($"0x{pair.Item1:X}: {string.Join(", ", pair.Item2.Select(e => $"0x{e:X2}"))}");
            }

            Console.WriteLine();
            Console.WriteLine("===== Encoded Data =====");

            foreach (var e in encodes)
            {
                Console.WriteLine($"0x{e:X}");
            }

            Console.WriteLine();
            Console.WriteLine("===== Decoded Bytes =====");

            byte[] decodedBytes = null;

            using (var decodeStream = new MemoryStream())
            {
                foreach (var e in encodes)
                {
                    if (e <= byte.MaxValue)
                    {
                        decodeStream.WriteByte((byte)e);
                    }
                    else
                    {
                        var bytes = table[e].ToArray();
                        decodeStream.Write(bytes, 0, bytes.Length);
                    }

                }

                decodedBytes = decodeStream.ToArray();
            }

            foreach (var c in decodedBytes)
            {
                Console.WriteLine($"0x{c:X2}");
            }

            var decoded = Encoding.Default.GetString(decodedBytes, 0, decodedBytes.Length);

            Console.WriteLine();
            Console.WriteLine("===== Decoded String =====");
            Console.WriteLine(decoded);

            Console.WriteLine();
            Console.WriteLine("===== Result =====");
            Console.WriteLine($"Eqauls: {input.Equals(decoded)}");
            Console.WriteLine($"Size: {inputBytes.Length} => {encodes.Count} ({encodes.Count / (inputBytes.Length / 100.0D):F2}%)");
        }

    }

}
