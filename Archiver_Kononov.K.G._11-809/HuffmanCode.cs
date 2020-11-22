using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace Archiver_Kononov.K.G._11_809
{
    public class HuffmanCode
    {
        public Dictionary<char, int> Dictionary = new Dictionary<char, int>();
        public LinkedList<Node> LinkedList = new LinkedList<Node>();
        public Dictionary<char, List<bool>> EncodedSymbols = new Dictionary<char, List<bool>>();

        public void CountSymbols(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                PutSymbolToDictionary(text[i]);
            }

            Dictionary = Dictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public void PutSymbolToDictionary(char symbol)
        {
            if (Dictionary.ContainsKey(symbol))
            {
                Dictionary[symbol]++;
            }
            else
            {
                Dictionary.Add(symbol, 1);
            }
        }

        public Node CreateHuffmanTree()
        {
            foreach (var symbol in Dictionary)
            {
                LinkedList.AddFirst(new Node(symbol.Key, symbol.Value));
            }

            Node node = LinkedList.First.Value;

            while (LinkedList.Count >= 2)
            {
                node = new Node();

                node.Frequency = LinkedList.First.Value.Frequency + LinkedList.First.Next.Value.Frequency;

                node.LeftChild = LinkedList.First.Value;
                LinkedList.Remove(LinkedList.First);

                node.RightChild = LinkedList.First.Value;
                LinkedList.Remove(LinkedList.First);

                if (LinkedList.Count == 0)
                {
                    break;
                }

                if (LinkedList.Count == 1)
                {
                    if (node.Frequency > LinkedList.First.Value.Frequency)
                    {
                        LinkedList.AddLast(node);
                    }
                    else
                    {
                        LinkedList.AddFirst(node);
                    }

                    continue;
                }

                var iterationNode = LinkedList.First;
                while (iterationNode != LinkedList.Last && node.Frequency > iterationNode.Value.Frequency)
                {
                    iterationNode = iterationNode.Next;
                }

                LinkedList.AddBefore(iterationNode, node);
            }

            return node;
        }

        public void StartSymbolsEncoding(Node node)
        {
            var n = new Node();
            n.LeftChild = new Node();
            n.LeftChild.Frequency = -1;
            n.RightChild = node;
            n.RightChild.Frequency = node.Frequency;
            EncodeSymbol(n, new List<bool>());
        }

        public void EncodeSymbol(Node node, List<bool> path)
        {
            if (node.Frequency != -1)
            {
                if (node.LeftChild == null && node.RightChild == null)
                {
                    EncodedSymbols.Add(node.Symbol.Value, path);
                }
                else
                {

                    var leftPath = new List<bool>(path);
                    leftPath.Add(false);

                    var rightPath = new List<bool>(path);
                    rightPath.Add(true);

                    EncodeSymbol(node.LeftChild, leftPath);
                    EncodeSymbol(node.RightChild, rightPath);
                }
            }
        }

        public byte[] ToByteArray(List<bool> bits)
        {
            byte[] output = new byte[bits.Count / 8];

            for (int i = 0; i < output.Length; i++)
            {
                for (int b = 0; b < 8; b++)
                {
                    output[i] |= (byte) ((bits[i * 8 + b] ? 1 : 0) << (7 - b));
                }
            }

            return output;
        }

        public void WriteToFile(string text)
        {
            var encodedText = new List<bool>();

            foreach (var symbol in text)
            {
                encodedText.AddRange(EncodedSymbols[symbol]);
            }

            int iterator = 0;
            
            using (BinaryWriter writer = new BinaryWriter(new FileStream("result.txt", FileMode.OpenOrCreate)))
            {
                foreach (var bit in encodedText)
                {
                    iterator = iterator == 8 ? 0 : iterator + 1;
                }
                
                for (int i = 0; i < iterator; i++)
                {
                    encodedText.Add(false);
                }

                writer.Write(ToByteArray(encodedText));
            }
        }

        public void DecodeText(string path)
        {
            string text = "";
            var bits = BackToBitsList();

            int length = bits.Length;
            ulong count = 0;
            var encodedSymbol = new List<bool>();
            char symbol;
            
            for (int i = 0; i < bits.Length; i++)
            {
                encodedSymbol.Add(bits[i]);
                count++;
                symbol = FindSymbol(encodedSymbol);
                if (symbol != '\0')
                {
                    text += symbol;
                    encodedSymbol.Clear();
                    Console.WriteLine($"Encoded {count} of {length}");
                }
            }
        }

        public char FindSymbol(List<bool> bits)
        {
            foreach (var symbol in EncodedSymbols)
            {
                if (bits.Count == symbol.Value.Count)
                {
                    for (int i = 0; i < bits.Count; i++)
                    {
                        if (bits[i] != symbol.Value[i])
                            return '\0';
                    }

                    return symbol.Key;
                }
            }

            return '\0';
        }

        public bool[] BackToBitsList()
        {
            var bytes = File.ReadAllBytes("result.txt");
            var bits = bytes.SelectMany(GetBits).ToArray();
            return bits;
        }
        
        IEnumerable<bool> GetBits(byte b)
        {
            for(int i = 0; i < 8; i++)
            {
                yield return (b & 0x80) != 0;
                b *= 2;
            }
        }
    }
}