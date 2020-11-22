using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Archiver_Kononov.K.G._11_809
{
    class Program
    {
        //TODO: Add chunks for decoder
        static void Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();
            watch.Stop();
            watch.Reset();
            
            var code = new HuffmanCode();
            string text;
            
            watch.Start();
            using (FileStream stream = new FileStream("text.txt", FileMode.Open))
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
                text = System.Text.Encoding.Default.GetString(array);
            }
            
            code.CountSymbols(text);
            var tree = code.CreateHuffmanTree();
            code.StartSymbolsEncoding(tree);
            
            
            code.WriteToFile(text);

            Console.WriteLine($"Packed in: " + watch.ElapsedMilliseconds);
            watch.Reset();
            watch.Start();
            code.DecodeText("result.txt");
            
            Console.WriteLine($"Unpacked in: " + watch.ElapsedMilliseconds);
        }
    }
}