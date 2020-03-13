using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DNA
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = @"dna.txt";
            var dnaReader = new DNAReader(filePath);
            var mers = new Dictionary<string, int>();
            foreach (var mer in dnaReader.ReadMer())
            {
                if (!mers.ContainsKey(mer)) mers[mer] = 0;
                mers[mer] += 1;
            }

            var separator = new string('*', Console.BufferWidth - 4);
            Console.WriteLine(separator);
            Console.WriteLine("\tNUCLEOTIDES");
            Console.WriteLine(separator);
            Console.WriteLine();
            foreach (var x in dnaReader.Nucleotides)
            {
                Console.WriteLine($"{x.Key}: {x.Value}");
            }

            Console.WriteLine();
            Console.WriteLine(separator);
            Console.WriteLine("\tMERS");
            Console.WriteLine(separator);
            Console.WriteLine();

            foreach (var x in mers)
            {
                var complementary = new string(x.Key.Select(m => dnaReader.GetComplementary(m)).ToArray());
                Console.WriteLine($"{x.Key} [ {complementary} ]: {x.Value}");
            }

            {
                var original = dnaReader.Read().ToList();
                var complementary = original.Select(x => dnaReader.GetComplementary(x)).ToList();

                var diff = complementary.Except(original);
                Console.WriteLine();
                Console.WriteLine(separator);
                Console.WriteLine("\tDIFF");
                Console.WriteLine(separator);
                Console.WriteLine(string.Join(",", diff));
            }

            Console.ReadLine();
        }
    }

    class DNAReader
    {
        private readonly string path;
        private readonly Dictionary<char, int> nucleotides;

        public IReadOnlyDictionary<char, int> Nucleotides => this.nucleotides;

        public DNAReader(string path)
        {
            this.path = path;
            this.nucleotides = new Dictionary<char, int>(4)
            {
                { 'A', 0 }, { 'T', 0 }, { 'C', 0 }, { 'G', 0 }
            };
        }

        public IEnumerable<char> Read()
        {
            this.nucleotides.Clear();
            using var sr = new StreamReader(this.path);
            while (!sr.EndOfStream)
            {
                var nucleotide = (char)sr.Read();
                if (!char.IsWhiteSpace(nucleotide))
                {
                    this.RegisterNucleotide(nucleotide);
                    yield return nucleotide;
                }
            }
        }

        public IEnumerable<string> ReadMer()
        {
            var mer = new List<char>(3);

            foreach (var x in this.Read())
            {
                mer.Add(x);
                if (mer.Count == 3)
                {
                    yield return string.Join("", mer);
                    mer.Clear();
                }
            }

            if (mer.Count > 0)
            {
                throw new InvalidDataException("Incomplete mer");
            }
        }

        public char GetComplementary(char nucleotide)
        {
            return nucleotide switch
            {
                'A' => 'T',
                'T' => 'A',
                'C' => 'G',
                'G' => 'C',
                _ => (char)((int)nucleotide + 1)
            };
        }

        private void RegisterNucleotide(char nucleotide)
        {
            if (!this.nucleotides.ContainsKey(nucleotide))
            {
                this.nucleotides.Add(nucleotide, 0);
            }

            this.nucleotides[nucleotide] += 1;
        }
    }
}