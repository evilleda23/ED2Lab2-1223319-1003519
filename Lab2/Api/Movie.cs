using Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api
{
    public class Movie : IComparable, IFixedSizeText
    {
        public string Director { get; set; }
        public double ImdbRating { get; set; }
        public string Genre { get; set; }
        public string ReleaseDate { get; set; }
        public int RottenTomatoesRating { get; set; }
        public string Title { get; set; }

        public int TextLength => ToFixedString().Length;

        public int CompareTo(object obj)
        {
            if (((Movie)obj).Title != null)
                return this.Title.CompareTo(((Movie)obj).Title);
            else
                return 1;
        }

        public IFixedSizeText CreateFromFixedText(string text)
        {
            if (text.Trim() != "")
            {
                Movie item = new Movie();
                item.Director = text.Substring(0, 50);
                text = text.Remove(0, 51);
                if (item.Director.Trim() == "")
                    item.Director = null;
                item.ImdbRating = double.Parse(text.Substring(0, 3));
                text = text.Remove(0, 4);
                item.Genre = text.Substring(0, 50);
                text = text.Remove(0, 51);
                if (item.Genre.Trim() == "")
                    item.Genre = null;
                item.ReleaseDate = text.Substring(0, 11);
                text = text.Remove(0, 12);
                if (item.ReleaseDate.Trim() == "")
                    item.ReleaseDate = null;
                item.RottenTomatoesRating = int.Parse(text.Substring(0, 3));
                text = text.Remove(0, 4);
                item.Title = text.Substring(0, 50);
                if (item.Title.Trim() == "")
                    item.Title = null;
                return item;
            }
            else
                return null;
        }

        public string ToFixedString()
        {
            string text = "";
            if (Director != null)
                text += string.Format("{0, -50}", Director) + ",";
            else
                text += new string(' ', 50) + ",";
            text += ImdbRating.ToString("0.0") + ",";
            if (Genre != null)
                text += string.Format("{0, -50}", Genre) + ",";
            else
                text += new string(' ', 50) + ",";
            if (ReleaseDate != null)
                text += string.Format("{0, -11}", ReleaseDate) + ",";
            else
                text += new string(' ', 11) + ",";
            text += RottenTomatoesRating.ToString("000") + ",";
            if (Title != null)
                text += string.Format("{0, -50}", Title) + ",";
            else
                text += new string(' ', 50) + ",";
            return text;
        }
    }
}
