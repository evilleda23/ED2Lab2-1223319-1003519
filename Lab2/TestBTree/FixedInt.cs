using Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestBTree
{
    public class FixedInt : IFixedSizeText, IComparable
    {
        public int Value { get; set; }

        public int TextLength => ToFixedString().Length;

        public int CompareTo(object obj)
        {
            return this.Value.CompareTo(((FixedInt)obj).Value);
        }

        public IFixedSizeText CreateFromFixedText(string text)
        {
            if (text.Trim() != "")
                return new FixedInt { Value = int.Parse(text.Trim()) };
            else
                return null;
        }

        public string ToFixedString()
        {
            return $"{Value:0000;-000}";
        }
    }
}
