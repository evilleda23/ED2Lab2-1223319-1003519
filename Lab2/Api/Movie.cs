using Generics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api
{
    public class Movie : IComparable, IFixedSizeText
    {
        private string ID;
        public string Director { get; set; }
        public double ImdbRating { get; set; }
        public string Genre { get; set; }
        public string ReleaseDate { get; set; }
        public int RottenTomatoesRating { get; set; }
        public string Title { get; set; }

        public int TextLength => ToFixedString().Length;

        public void SetID()
        {
            ID = Title + "-" + ReleaseDate.Substring(7);
        }

        public void SetID(string id)
        {
            ID = id;
        }

        public string GetID()
        {
            return ID;
        }

        public int CompareTo(object obj)
        {
            if (((Movie)obj).GetID() != null)
                return this.ID.CompareTo(((Movie)obj).GetID());
            else
                return 1;
        }

        public IFixedSizeText CreateFromFixedText(string text)
        {
            if (text.Trim() != "")
            {
                Movie item = new Movie();
                item.Director = text.Substring(0, 50).Trim();
                text = text.Remove(0, 51);
                if (item.Director == "")
                    item.Director = null;
                item.ImdbRating = double.Parse(text.Substring(0, 3));
                text = text.Remove(0, 4);
                item.Genre = text.Substring(0, 50).Trim();
                text = text.Remove(0, 51);
                if (item.Genre == "")
                    item.Genre = null;
                item.ReleaseDate = text.Substring(0, 11).Trim();
                text = text.Remove(0, 12);
                if (item.ReleaseDate == "")
                    item.ReleaseDate = null;
                item.RottenTomatoesRating = int.Parse(text.Substring(0, 3));
                text = text.Remove(0, 4);
                item.Title = text.Trim();
                if (item.Title == "")
                    item.Title = null;
                item.SetID();
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
                text += string.Format("{0, -50}", Title);
            else
                text += new string(' ', 50);
            return text;
        }
    }
}
