using System;
using System.Collections.Generic;
using System.Text;

namespace Generics
{
    class Node<T> : IFixedSizeText where T : IComparable, IFixedSizeText
    {
        public int ID;
        public T[] Values;
        public int[] Sons;
        public int TextLength => ToFixedString().Length;

        public Node(int id, int deg)
        {
            ID = id;
            Values = new T[deg - 1];
            Sons = new int[deg];
        }

        public string ToFixedString()
        {
            throw new NotImplementedException();
        }
    }
}
