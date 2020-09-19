using System;
using System.IO;
using System.Text;

namespace Generics
{
    public class BTree<T> where T : IComparable, IFixedSizeText
    {
        private readonly string Path;
        private readonly int Degree;
        private readonly int ValueTextLength;
        private int NextPosition;
        private int Root;

        public BTree(string path, int degree, int valLength)
        {
            Path = path;
            Degree = degree;
            ValueTextLength = valLength;
            var file = new FileStream(Path, FileMode.OpenOrCreate);
            using StreamReader reader = new StreamReader(file, Encoding.ASCII);
            string metadata = reader.ReadLine();
            if (metadata != null)
            {
                var data = metadata.Split("|");
                Root = int.Parse(data[0]);
                NextPosition = int.Parse(data[1]);
            }
            else
            {
                using StreamWriter writer = new StreamWriter(file, Encoding.ASCII);
                writer.WriteLine("1|2");
                var sample = new Node<T>(1, Degree, ValueTextLength);
                writer.WriteLine(sample.ToFixedString());
                Root = 1;
                NextPosition = 2;
            }
        }

        public void Add(T val)
        {
            Add(val, Root);
        }

        private void Add(T val, int pos)
        {
            var node = ChargeNode(Root);
        }

        private Node<T> ChargeNode(int id)
        {
            return null;
        }
    }
}
