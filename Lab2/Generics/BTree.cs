using System;
using System.IO;
using System.Text;

namespace Generics
{
    public class BTree<T> where T : IComparable, IFixedSizeText
    {
        private string Path;
        private int Degree;
        private int NextPosition;
        private int Root;

        public BTree(string path, int degree)
        {
            Path = path;
            Degree = degree;
            var file = new FileStream(path, FileMode.OpenOrCreate);
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
                writer.WriteLine("0|1");
            }
        }
    }
}
