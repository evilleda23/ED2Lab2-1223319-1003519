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
        public int ValueTextLength;

        public Node(int id, int deg, int valLength)
        {
            ID = id;
            Values = new T[deg - 1];
            Sons = new int[deg];
            ValueTextLength = valLength;
        }

        public string ToFixedString()
        {
            string text = $"{ID:00000000000;-0000000000}";
            foreach (int item in Sons)
            {
                text += $"|{item:00000000000;-0000000000}";
            }
            foreach (T item in Values)
            {
                text += "|" + FixValue(item);
            }
            return text;
        }

        public bool IsFull()
        {
            foreach (T item in Values)
            {
                if (item == null)
                    return false;
            }
            return true;
        }

        private string FixValue(T val)
        {
            if (val != null)
                return val.ToFixedString();
            else
                return new string(' ', ValueTextLength);
        }
    }
}
