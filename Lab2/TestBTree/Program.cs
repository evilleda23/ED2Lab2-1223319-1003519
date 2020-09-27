using System;
using System.Collections.Generic;
using Generics;
using System.IO;

namespace TestBTree
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Ingrese el numero del grado para el arbol");
                int grado = int.Parse(Console.ReadLine());
                BTree<FixedInt> tree = new BTree<FixedInt>("..//..//..//test.txt", grado, new FixedInt().TextLength);
                tree.Clear();
                int backmenu = 0;
                do
                {
                    Console.WriteLine("¿Qué metodo desea utilizar para la inserción? \n 1.Ingresarlos a través de comas \n 2. Cargar un archivo de texto");
                    int op = int.Parse(Console.ReadLine());
                    switch (op)
                    {
                        case 1:
                            Console.WriteLine("Ingrese los valores");
                            var val = Console.ReadLine().Split(',');
                            foreach (var n in val)
                                tree.Add(new FixedInt { Value = int.Parse(n) });
                            break;
                        case 2:
                            Console.WriteLine("Coloque el txt encima de la consola");
                            string dir = Console.ReadLine();
                            Insertartxt(dir, tree);
                            break;
                    }
                    Console.WriteLine("¿Desea seguir ingresando valores? 1.Si 2.No");
                    backmenu = int.Parse(Console.ReadLine());
                } while (backmenu == 1);
                Console.WriteLine("Preorden:");
                Console.WriteLine(ImprimirListado(tree.Preorden()));
                Console.WriteLine("Inorden:");
                Console.WriteLine(ImprimirListado(tree.Inorden()));
                Console.WriteLine("Postorden:");
                Console.WriteLine(ImprimirListado(tree.Postorden()));
                backmenu = 0;
                do
                {
                    Console.WriteLine("Ingrese los valores por eliminar separados por comas");
                    var val = Console.ReadLine().Split(',');
                    foreach (var n in val)
                    {
                        if (tree.Delete(new FixedInt { Value = int.Parse(n) }))
                            Console.WriteLine($"El número {n} fue eliminado del árbol.");
                        else
                            Console.WriteLine($"El número {n} no se encontró en el árbol.");
                    }
                    Console.WriteLine("¿Desea seguir eliminando valores? 1.Si 2.No");
                    backmenu = int.Parse(Console.ReadLine());
                } while (backmenu == 1);
                Console.WriteLine("Preorden:");
                Console.WriteLine(ImprimirListado(tree.Preorden()));
                Console.WriteLine("Inorden:");
                Console.WriteLine(ImprimirListado(tree.Inorden()));
                Console.WriteLine("Postorden:");
                Console.WriteLine(ImprimirListado(tree.Postorden()));
                Console.ReadLine();
            }
            catch
            {
                Console.WriteLine("Error producido no controlado");
            }
        }

        static string ImprimirListado(List<FixedInt> val)
        {
            string text = "";
            foreach (var n in val)
            {
                text += n.Value.ToString() + ",";
            }
            if (text.EndsWith(','))
                text = text.Remove(text.Length - 1);
            return text;
        }

        static public void Insertartxt(string dir, BTree<FixedInt> tree)
        {
            try
            {
                string archivo;
                using (StreamReader lector = new StreamReader(dir))
                {
                    archivo = lector.ReadToEnd();
                }
                string[] list;
                list = archivo.Split(new char[] { '\r', '\t', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] != "")
                    {
                        tree.Add(new FixedInt { Value = int.Parse(list[i]) });
                    }
                }
            }
            catch
            {
                Console.WriteLine("Carga del archivo erronea, verifique el formato del archivo");
            }
        }
    }
}
