using System;
using System.Collections.Generic;
using System.Text;

namespace Generics
{
    class Node<T> : IFixedSizeText where T : IComparable, IFixedSizeText, new()
    {
        public int ID;
        public int Degree;
        public T[] Values;
        public int[] Sons;
        public int TextLength => ToFixedString().Length;
        public int ValueTextLength;

        public Node(int id, int deg, int valLength)
        {
            ID = id;
            Degree = deg;
            Values = new T[Degree - 1];
            Sons = new int[Degree];
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

        public bool IsLeaf()
        {
            foreach (int item in Sons)
            {
                if (item != 0)
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

        public IFixedSizeText CreateFromFixedText(string text)
        {
            Node<T> aux = new Node<T>(ID, Degree, ValueTextLength);
            text = text.Remove(0, 12);
            for (int i = 0; i < Degree; i++)
            {
                aux.Sons[i] = int.Parse(text.Substring(0, 11));
                text = text.Remove(0, 12);
            }
            for (int i = 0; i < Degree - 1; i++)
            {
                T aux2 = new T();
                aux.Values[i] = (T)aux2.CreateFromFixedText(text.Substring(0, ValueTextLength));
                text = text.Remove(0, ValueTextLength + 1);
            }
            return aux;
        }
    }
}
