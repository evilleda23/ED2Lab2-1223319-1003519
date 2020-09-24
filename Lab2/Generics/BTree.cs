using System;
using System.Collections.Generic;
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
            if (degree < 3)
                degree = 3;
            Degree = degree;
            ValueTextLength = valLength;
            CreateFile();
        }

        public void Add(T val)
        {
            var file = new FileStream(Path, FileMode.OpenOrCreate);
            using StreamReader reader = new StreamReader(file, Encoding.ASCII);
            string metadata = reader.ReadLine();
            file.Close();
            if (metadata == null)
                CreateFile();
            Add(val, Root);
        }

        private void Add(T val, int pos)
        {
            var node = ChargeNode(pos);
            if (node.IsLeaf())
                Insert(node, val, 0);
            else
            {
                int i = 0;
                bool insert = true;
                while (i < Degree - 1)
                {
                    if (node.Values[i] != null)
                    {
                        if (val.CompareTo(node.Values[i]) < 0)
                        {
                            Add(val, node.Sons[i]);
                            insert = false;
                            break;
                        }
                        else if (val.CompareTo(node.Values[i]) == 0)
                        {
                            insert = false;
                            break;
                        }
                        i++;
                    }
                    else
                        break;
                }
                if (insert)
                    Add(val, node.Sons[i]);
            }
        }

        private void Insert(Node<T> node, T val, int newSon)
        {
            if (node.IsFull())
            {
                bool insert = true;
                for (int i = 0; i < Degree - 1; i++)
                {
                    if (val.CompareTo(node.Values[i]) < 0)
                    {
                        T aux = node.Values[i];
                        node.Values[i] = val;
                        val = aux;
                        int aux3 = node.Sons[i + 1];
                        node.Sons[i + 1] = newSon;
                        newSon = aux3;
                    }
                    else if (val.CompareTo(node.Values[i]) == 0)
                    {
                        insert = false;
                        break;
                    }
                }
                if (insert)
                {
                    Node<T> aux2 = new Node<T>(NextPosition, Degree, ValueTextLength);
                    for (int i = 0; i < (Degree / 2) - 1; i++)
                    {
                        aux2.Values[i] = node.Values[((Degree + 1) / 2) + i];
                        node.Values[((Degree + 1) / 2) + i] = default;
                        aux2.Sons[i] = node.Sons[((Degree + 1) / 2) + i];
                        node.Sons[((Degree + 1) / 2) + i] = 0;
                    }
                    aux2.Values[(Degree / 2) - 1] = val;
                    aux2.Sons[(Degree / 2) - 1] = node.Sons[Degree - 1];
                    node.Sons[Degree - 1] = 0;
                    aux2.Sons[(Degree / 2)] = newSon;
                    NextPosition++;
                    Write($"{Degree}|{Root}|{NextPosition}", 0);
                    val = node.Values[(Degree - 1) / 2];
                    node.Values[(Degree - 1) / 2] = default;
                    for (int i = 0; i < Degree; i++)
                    {
                        if (aux2.Sons[i] != 0)
                        {
                            Node<T> aux4 = ChargeNode(aux2.Sons[i]);
                            aux4.Father = aux2.ID;
                            Write(aux4.ToFixedString(), aux4.ID);
                        }
                    }
                    if (node.Father != 0)
                    {
                        aux2.Father = node.Father;
                        Write(node.ToFixedString(), node.ID);
                        Write(aux2.ToFixedString(), aux2.ID);
                        Insert(ChargeNode(node.Father), val, aux2.ID);
                    }
                    else
                    {
                        Node<T> newRoot = new Node<T>(NextPosition, Degree, ValueTextLength);
                        newRoot.Values[0] = val;
                        newRoot.Sons[0] = node.ID;
                        newRoot.Sons[1] = aux2.ID;
                        node.Father = NextPosition;
                        aux2.Father = NextPosition;
                        Root = NextPosition;
                        NextPosition++;
                        Write($"{Degree}|{Root}|{NextPosition}", 0);
                        Write(node.ToFixedString(), node.ID);
                        Write(aux2.ToFixedString(), aux2.ID);
                        Write(newRoot.ToFixedString(), newRoot.ID);
                    }
                }
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
                        int aux2 = node.Sons[i + 1];
                        node.Sons[i + 1] = newSon;
                        newSon = aux2;
                    }
                    else if (val.CompareTo(node.Values[i]) == 0)
                        break;
                    i++;
                }
                if (node.Values[i] == null)
                {
                    node.Values[i] = val;
                    node.Sons[i + 1] = newSon;
                }
                Write(node.ToFixedString(), node.ID);
            }
        }

        public T Search(T val)
        {
            if (Root != 0)
            {
                int pos = SearchNode(Root, val);
                if (pos != 0)
                {
                    var node = ChargeNode(pos);
                    for (int i = 0; i < Degree - 1; i++)
                    {
                        if (node.Values[i] != null)
                        {
                            if (val.CompareTo(node.Values[i]) == 0)
                                return val;
                        }
                    }
                    return default;
                }
                else
                    return default;
            }
            else
                return default;
        }

        private int SearchNode(int pos, T val)
        {
            var node = ChargeNode(pos);
            for (int i = 0; i < Degree - 1; i++)
            {
                if (node.Values[i] != null)
                {
                    if (val.CompareTo(node.Values[i]) == 0)
                        return pos;
                    else if (val.CompareTo(node.Values[i]) < 0 && node.Sons[i] != 0)
                        return SearchNode(node.Sons[i], val);
                }
                else if (node.Sons[i] != 0)
                    return SearchNode(node.Sons[i], val);
            }
            if (node.Sons[Degree - 1] != 0)
                return SearchNode(node.Sons[Degree - 1], val);
            return 0;
        }

        public bool Delete(T val)
        {
            if (Root != 0)
            {
                int pos = SearchNode(Root, val);
                if (pos != 0)
                {
                    Remove(ChargeNode(pos), val);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private void Remove(Node<T> node, T val)
        {
            bool deleted = false;
            int originalPosition = 0;
            for (int i = 0; i < Degree - 1; i++)
            {
                if (node.Values[i] != null)
                {
                    if (deleted && node.IsLeaf())
                    {
                        node.Values[i - 1] = node.Values[i];
                        node.Values[i] = default;
                    }
                    if (node.Values[i] != null)
                    {
                        if (val.CompareTo(node.Values[i]) == 0)
                        {
                            originalPosition = i;
                            node.Values[i] = default;
                            deleted = true;
                        }
                    }
                }
            }
            if (node.IsLeaf())
            {
                if (node.IsInUnderflow() && node.ID != Root)
                    Rearrange(node);
                else
                    Write(node.ToFixedString(), node.ID);
            }
            else
            {
                var aux = ChargeNode(node.Sons[originalPosition + 1]);
                while(!aux.IsLeaf())
                {
                    aux = ChargeNode(aux.Sons[0]);
                }
                node.Values[originalPosition] = aux.Values[0];
                Write(node.ToFixedString(), node.ID);
                Remove(aux, aux.Values[0]);
            }
        }

        private void Rearrange(Node<T> node)
        {
            var father = ChargeNode(node.Father);
            int leftBrotherID = 0;
            int rightBrotherID = 0;
            for (int i = 0; i < Degree; i++)
            {
                if (father.Sons[i] == node.ID)
                {
                    if (i > 0)
                        leftBrotherID = father.Sons[i - 1];
                    if (i < Degree - 1)
                        rightBrotherID = father.Sons[i + 1];
                }
            }
            if (rightBrotherID != 0)
            {
                var brother = ChargeNode(rightBrotherID);
                if (brother.CanLend())
                    LendFromBrother(node, brother, father, true);
                else
                {
                    if (leftBrotherID != 0)
                    {
                        var leftBrother = ChargeNode(leftBrotherID);
                        if (leftBrother.CanLend())
                            LendFromBrother(node, leftBrother, father, false);
                        else
                            JoinNodes(node, brother, father);
                    }
                    else
                        JoinNodes(node, brother, father);
                }
            }
            else
            {
                var brother = ChargeNode(leftBrotherID);
                if (brother.CanLend())
                    LendFromBrother(node, brother, father, false);
                else
                    JoinNodes(brother, node, father);
            }
        }

        private void LendFromBrother(Node<T> node, Node<T> brother, Node<T> father, bool fromRight)
        {
            if (fromRight)
            {
                for (int i = 0; i < Degree - 1; i++)
                {
                    if (father.Sons[i] == node.ID)
                    {
                        for (int j = 0; j < Degree - 1; j++)
                        {
                            if (node.Values[j] == null)
                            {
                                node.Values[j] = father.Values[i];
                                father.Values[i] = brother.Values[0];
                                node.Sons[j + 1] = brother.Sons[0];
                                brother.Sons[0] = brother.Sons[1];
                                for (int k = 1; k < Degree - 1; k++)
                                {
                                    if (brother.Values[k] != null)
                                    {
                                        brother.Values[k - 1] = brother.Values[k];
                                        brother.Sons[k] = brother.Sons[k + 1];
                                        brother.Values[k] = default;
                                        brother.Sons[k + 1] = 0;
                                    }
                                    else
                                        break;
                                }
                                Write(node.ToFixedString(), node.ID);
                                Write(father.ToFixedString(), father.ID);
                                Write(brother.ToFixedString(), brother.ID);
                                if (node.Sons[j + 1] != 0)
                                {
                                    var son = ChargeNode(node.Sons[j + 1]);
                                    son.Father = node.ID;
                                    Write(son.ToFixedString(), son.ID);
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Degree - 1; i++)
                {
                    if (father.Sons[i] == brother.ID)
                    {
                        for (int j = 0; j < Degree - 1; j++)
                        {
                            if (brother.Values[j] == null || j == Degree - 2)
                            {
                                if (brother.Values[j] == null)
                                    j--;
                                for (int k = Degree - 2; k > 0; k--)
                                {
                                    if (node.Values[k - 1] != null)
                                    {
                                        node.Values[k] = node.Values[k - 1];
                                        node.Sons[k + 1] = node.Sons[k];
                                    }
                                }
                                node.Sons[1] = node.Sons[0];
                                node.Values[0] = father.Values[i];
                                father.Values[i] = brother.Values[j];
                                node.Sons[0] = brother.Sons[j + 1];
                                brother.Values[j] = default;
                                brother.Sons[j + 1] = 0;
                                Write(node.ToFixedString(), node.ID);
                                Write(father.ToFixedString(), father.ID);
                                Write(brother.ToFixedString(), brother.ID);
                                if (node.Sons[0] != 0)
                                {
                                    var son = ChargeNode(node.Sons[0]);
                                    son.Father = node.ID;
                                    Write(son.ToFixedString(), son.ID);
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void JoinNodes(Node<T> left, Node<T> right, Node<T> father)
        {
            for (int i = 0; i < Degree - 1; i++)
            {
                if (left.Values[i] == null)
                {
                    for (int j = 0; j < Degree - 1; j++)
                    {
                        if (father.Sons[j] == left.ID)
                        {
                            left.Values[i] = father.Values[j];
                            father.Values[j] = default;
                            father.Sons[j + 1] = 0;
                            for (int k = j + 1; k < Degree - 1; k++)
                            {
                                if (father.Values[k] != null)
                                {
                                    father.Values[k - 1] = father.Values[k];
                                    father.Sons[k] = father.Sons[k + 1];
                                    father.Values[k] = default;
                                    father.Sons[k + 1] = 0;
                                }
                                else
                                    break;
                            }
                            break;
                        }
                    }
                    i++;
                    left.Sons[i] = right.Sons[0];
                    right.Sons[0] = 0;
                    if (left.Sons[i] != 0)
                    {
                        var son = ChargeNode(left.Sons[i]);
                        son.Father = left.ID;
                        Write(son.ToFixedString(), son.ID);
                    }
                    for (int j = 0; j < Degree - 1; j++)
                    {
                        if (right.Values[j] != null)
                        {
                            left.Values[i + j] = right.Values[j];
                            left.Sons[i + j + 1] = right.Sons[j + 1];
                            right.Values[j] = default;
                            right.Sons[j + 1] = 0;
                            if (left.Sons[i + j + 1] != 0)
                            {
                                var son1 = ChargeNode(left.Sons[i + j + 1]);
                                son1.Father = left.ID;
                                Write(son1.ToFixedString(), son1.ID);
                            }
                        }
                        else
                            break;
                    }
                    right.Father = 0;
                    Write(left.ToFixedString(), left.ID);
                    Write(right.ToFixedString(), right.ID);
                    Write(father.ToFixedString(), father.ID);
                    if (father.IsInUnderflow() && father.ID != Root)
                        Rearrange(father);
                    else if (father.IsEmpty())
                    {
                        Root = left.ID;
                        left.Father = 0;
                        father.Sons[0] = 0;
                        Write($"{Degree}|{Root}|{NextPosition}", 0);
                        Write(left.ToFixedString(), left.ID);
                        Write(father.ToFixedString(), father.ID);
                    }
                    break;
                }
            }
        }

        public List<T> Preorden()
        {
            if (Root != 0)
            {
                List<T> path = new List<T>();
                Preorden(ChargeNode(Root), path);
                return path;
            }
            else
                return new List<T>();
        }

        private void Preorden(Node<T> pos, List<T> path)
        {
            for (int i = 0; i < Degree - 1; i++)
            {
                if (pos.Values[i] != null)
                    path.Add(pos.Values[i]);
            }
            for (int i = 0; i < Degree; i++)
            {
                if (pos.Sons[i] != 0)
                    Preorden(ChargeNode(pos.Sons[i]), path);
            }
        }

        public List<T> Inorden()
        {
            if (Root != 0)
            {
                List<T> path = new List<T>();
                Inorden(ChargeNode(Root), path);
                return path;
            }
            else
                return new List<T>();
        }

        private void Inorden(Node<T> pos, List<T> path)
        {
            for (int i = 0; i < Degree - 1; i++)
            {
                if (pos.Sons[i] != 0)
                    Inorden(ChargeNode(pos.Sons[i]), path);
                if (pos.Values[i] != null)
                    path.Add(pos.Values[i]);
            }
            if (pos.Sons[Degree - 1] != 0)
                Inorden(ChargeNode(pos.Sons[Degree - 1]), path);
        }

        public List<T> Postorden()
        {
            if (Root != 0)
            {
                List<T> path = new List<T>();
                Postorden(ChargeNode(Root), path);
                return path;
            }
            else
                return new List<T>();
        }

        private void Postorden(Node<T> pos, List<T> path)
        {
            for (int i = 0; i < Degree; i++)
            {
                if (pos.Sons[i] != 0)
                    Postorden(ChargeNode(pos.Sons[i]), path);
            }
            for (int i = 0; i < Degree - 1; i++)
            {
                if (pos.Values[i] != null)
                    path.Add(pos.Values[i]);
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

        private void CreateFile()
        {
            var file = new FileStream(Path, FileMode.OpenOrCreate);
            using StreamReader reader = new StreamReader(file, Encoding.ASCII);
            string metadata = reader.ReadLine();
            if (metadata != null)
            {
                var data = metadata.Split("|");
                if (Degree != int.Parse(data[0]))
                {
                    file.Close();
                    reader.Close();
                    using StreamWriter writer = new StreamWriter(Path, false);
                    writer.WriteLine($"{Degree}|1|2");
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
                writer.WriteLine($"{Degree}|1|2");
                var sample = new Node<T>(1, Degree, ValueTextLength);
                writer.WriteLine(sample.ToFixedString());
                Root = 1;
                NextPosition = 2;
            }
        }

        public void Clear()
        {
            File.Delete(Path);
            Root = 0;
            NextPosition = 1;
        }
    }
}
