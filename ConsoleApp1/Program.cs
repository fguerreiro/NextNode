using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var root = new Node(
                1,
                new Node(
                    2,
                    new Node(3),
                    new Node(4)),
                new Node(
                    5,
                    new Node(6),
                    new Node(7)));

            
            var n = root;
            while (n != null)
            {
                var data = n.Data;
                Console.WriteLine(data);
                n = n.Next();
            }

            Console.ReadLine();
        }
    }

    public class Node
    {
        public Node(int data, params Node[] nodes)
        {
            Data = data;
            AddRange(nodes);
        }

        public Node Parent { get; set; }
        public IEnumerable<Node> Children
        {
            get
            {
                return _children != null
                    ? _children
                    : Enumerable.Empty<Node>();
            }
        }
        public int Data { get; private set; }

        public List<Node> Siblings()
        {
            if (Parent != null)
            {
                return Parent.Children.Except(new List<Node> { this }).ToList();
            }
            return new List<Node>();
        }

        public void Add(Node node)
        {
            Debug.Assert(node.Parent == null);

            if (_children == null)
            {
                _children = new List<Node>();
            }
            _children.Add(node);

            node.Parent = this;
        }
        public void AddRange(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }

        public List<Node> GetSiblings()
        {
            var siblings = Parent.Children.Except(new List<Node> { this });
            return siblings.ToList();
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        private List<Node> _children;
    }

    public static class NodeExtensions
    {
        private static Node[] PreOrderedArray { get; set; }

        public static Node[] ToPreOrderedArray(this Node n)
        {
            // For performance, cache tree as pre-ordered array once, when traversing from root; otherwise return cached array. 
            if (n.IsRoot())
            {
                var visited = new List<Node>();
                n.VisitNodes(visited);

                PreOrderedArray = visited.ToArray();
            }

            return PreOrderedArray;
        }

        public static void VisitNodes(this Node n, List<Node> visited)
        {
            visited.Add(n);

            foreach (var nonVisitedChild in n.UnvisitedChildren(visited))
            {
                VisitNodes(nonVisitedChild, visited);
            }
        }

        public static Node Next(this Node n)
        {
            var preOrdered = n.ToPreOrderedArray();
            var currentIndex = Array.IndexOf(preOrdered, n);

            var upperBoundNextElement = currentIndex + 1;
            Node next = preOrdered.Length > upperBoundNextElement ? preOrdered[upperBoundNextElement] : null;

            return next;
        }

        public static bool IsRoot(this Node n)
        {
            return n.Parent == null;
        }

        public static IEnumerable<Node> UnvisitedChildren(this Node parent, IEnumerable<Node> visited)
        {
            return parent.Children.Except(visited);
        }
    }
}
