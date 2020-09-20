using System;
using System.IO;
using System.Text;

namespace Generics
{
    public class BTree<T> where T : IComparable, IFixedSizeText, new()
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
                if (Degree != int.Parse(data[0]))
                {
                    using StreamWriter writer = new StreamWriter(file, Encoding.ASCII);
                    writer.WriteLine("5|1|2");
                    var sample = new Node<T>(1, Degree, ValueTextLength);
                    writer.WriteLine(sample.ToFixedString());
                    Root = 1;
                    NextPosition = 2;
                }
                else
                {
                    Root = int.Parse(data[1]);
                    NextPosition = int.Parse(data[2]);
                }
            }
            else
            {
                using StreamWriter writer = new StreamWriter(file, Encoding.ASCII);
                writer.WriteLine("5|1|2");
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
            var node = ChargeNode(pos);
            if (node.IsLeaf())
                Insert(node, val);
            else
            {
                int i = 0;
                while (node.Values[i] != null && i < Degree - 1)
                {
                    if (val.CompareTo(node.Values[i]) < 0)
                        Add(val, node.Sons[i]);
                    else if (val.CompareTo(node.Values[i]) == 0)
                        break;
                    i++;
                }
                if (val.CompareTo(node.Values[i]) > 0)
                    Add(val, node.Sons[i + 1]);
            }
        }

        private void Insert(Node<T> node, T val)
        {
            if (node.IsFull())
            {

            }
            else
            {
                int i = 0;
                while (node.Values[i] != null && i < Degree - 1)
                {
                    if (val.CompareTo(node.Values[i]) < 0)
                    {
                        T aux = node.Values[i];
                        node.Values[i] = val;
                        val = aux;
                    }
                    else if (val.CompareTo(node.Values[i]) == 0)
                        break;
                    i++;
                }
                if (node.Values[i] == null)
                    node.Values[i] = val;
                Write(node.ToFixedString(), node.ID);
            }
        }

        private Node<T> ChargeNode(int id)
        {
            var file = new FileStream(Path, FileMode.OpenOrCreate);
            using StreamReader reader = new StreamReader(file, Encoding.ASCII);
            for (int i = 0; i < id; i++)
            {
                reader.ReadLine();
            }
            string text = reader.ReadLine() + "|";
            var node = new Node<T>(id, Degree, ValueTextLength);
            var aux = (Node<T>)node.CreateFromFixedText(text);
            return aux;
        }

        private void Write(string text, int id)
        {
            var file = new FileStream(Path, FileMode.OpenOrCreate);
            using StreamReader reader = new StreamReader(file, Encoding.ASCII);
            string tree = "";
            for (int i = 0; i < id; i++)
                tree += reader.ReadLine() + "\r\n";
            tree += text + "\r\n";
            reader.ReadLine();
            while (!reader.EndOfStream)
                tree += reader.ReadLine() + "\r\n";
            file.Close();
            reader.Close();
            using StreamWriter writer = new StreamWriter(Path, false);
            writer.Write(tree);
        }
    }
}
