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
        public static void Main(string[] args)
        {
            var input = "BABAABAAA";
            var inputBytes = Encoding.Default.GetBytes(input);
            var compressor = new LZWProcessor();
            var encodedCodes = new List<int>();

            using (var encodeStream = new MemoryStream(inputBytes))
            {
                while (true)
                {
                    var b = encodeStream.ReadByte();
                    var code = compressor.Encode(b);

                    if (code > -1)
                    {
                        encodedCodes.Add(code);
                    }

                    if (b == -1)
                    {
                        break;
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
            Console.WriteLine("===== Encode Table =====");

            foreach (var pair in compressor.Table.Where(p => p.Item1 > byte.MaxValue))
            {
                Console.WriteLine($"0x{pair.Item1:X}: {string.Join(", ", pair.Item2.Select(e => $"0x{e:X2}"))}");
            }

            Console.WriteLine();
            Console.WriteLine("===== Encoded Data =====");

            foreach (var e in encodedCodes)
            {
                Console.WriteLine($"0x{e:X}");
            }

            Console.WriteLine();
            Console.WriteLine("===== Decoded Bytes =====");

            byte[] decodedBytes = null;
            var decompressor = new LZWProcessor();

            using (var decodeStream = new MemoryStream())
            {
                foreach (var e in encodedCodes)
                {
                    var tableKey = decompressor.Decode(e);
                    var bytes = decompressor.Table[tableKey].ToArray();
                    decodeStream.Write(bytes, 0, bytes.Length);
                }

                decodedBytes = decodeStream.ToArray();
            }

            foreach (var c in decodedBytes)
            {
                Console.WriteLine($"0x{c:X2}");
            }

            var decoded = Encoding.Default.GetString(decodedBytes, 0, decodedBytes.Length);

            Console.WriteLine();
            Console.WriteLine("===== Decode Table =====");

            foreach (var pair in decompressor.Table.Where(p => p.Item1 > byte.MaxValue))
            {
                Console.WriteLine($"0x{pair.Item1:X}: {string.Join(", ", pair.Item2.Select(e => $"0x{e:X2}"))}");
            }

            Console.WriteLine();
            Console.WriteLine("===== Decoded String =====");
            Console.WriteLine(decoded);

            Console.WriteLine();
            Console.WriteLine("===== Result =====");
            Console.WriteLine($"Eqauls: {input.Equals(decoded)}");
            Console.WriteLine($"Size: {inputBytes.Length} => {encodedCodes.Count} ({encodedCodes.Count / (inputBytes.Length / 100.0D):F2}%)");
        }

    }

}
