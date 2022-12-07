using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SyntaxAn.BinaryTree
{
    public class Node<T>
    {
        public T Data;
        public List<Node<T>> Children;
        public Node()
        {
            Children = new List<Node<T>>();
        }
        public Node(T data)
        {
            Data = data;
            Children = new List<Node<T>>();
        }
        public void PrintPretty(string indent, bool last, Action<T> printNode = null)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\-");
                indent += "  ";
            }
            else
            {
                Console.Write("|-");
                indent += "| ";
            }
            Console.ForegroundColor = ConsoleColor.Green;
            if (printNode != null)
                printNode(Data);
            else
                Console.WriteLine(Data);
            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < Children.Count; i++)
                Children[i].PrintPretty(indent, i == Children.Count - 1, printNode);
        }
        public Node<T> Copy()
        {
            var node = new Node<T>(Data);
            foreach(var child in Children)
                node.Children.Add(child.Copy());
            return node;
        }
    }
}
