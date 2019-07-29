using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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
        public static Node Next(this Node n)
        {
            var preOrdered = n.ToPreOrderedArray();
            var currentNodeIndex = Array.IndexOf(preOrdered, n);

            var upperBoundNextElement = currentNodeIndex + 1;
            Node next = preOrdered.Length > upperBoundNextElement ? preOrdered[upperBoundNextElement] : null;

            return next;
        }

        public static string VisitAllNodes(this Node root)
        {
            var visited = root.ToPreOrderedArray().ToList();

            return string.Join(",", visited);
        }

        public static bool IsRoot(this Node n)
        {
            return n.Parent == null;
        }

        private static IEnumerable<Node> UnvisitedChildren(this Node parent, IEnumerable<Node> visited)
        {
            return parent.Children.Except(visited);
        }

        private static void VisitNodes(this Node currentNode, List<Node> visited)
        {
            visited.Add(currentNode);

            foreach (var nonVisitedChild in currentNode.UnvisitedChildren(visited))
            {
                VisitNodes(nonVisitedChild, visited);
            }
        }

        private static Node[] ToPreOrderedArray(this Node n)
        {
            // For performance, store cache tree as pre-ordered array once, when traversing from root; otherwise return cached array. 
            if (n.IsRoot())
            {
                var visited = new List<Node>();
                n.VisitNodes(visited);

                _preOrderedTreeCache = visited.ToArray();
            }

            return _preOrderedTreeCache;
        }

        private static Node[] _preOrderedTreeCache { get; set; } = new Node[0];
    }

    [TestFixture]
    class NodeTests
    {
        public Node Tree => new Node(
            1,
            new Node(
                2,
                new Node(3),
                new Node(4)),
            new Node(
                5,
                new Node(6),
                new Node(7)));

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        public void NodeGetsNextChild(int nodeIndex)
        {
            var root = Tree;

            var n = root;
            var currentData = 0;
            var currentIndex = 1;
            while (n != null && currentIndex++ <= nodeIndex)
            {
                currentData = n.Data;
                TestContext.WriteLine($"Current node: {currentData}");
                n = n.Next();
            }
            Assert.AreEqual(nodeIndex, currentData);
        }

        [Test]
        public void UnbalanceTree()
        {
            var unbalancedTree = 
                new Node(1, 
                    new Node(2),
                new Node(3),
                new Node(4,
                    new Node(5,
                        new Node(6))),
                new Node(7,
                    new Node(8), 
                    new Node(9)));

            var visited = unbalancedTree.VisitAllNodes();

            Assert.AreEqual("1,2,3,4,5,6,7,8,9", visited);
        }

        [Test]
        public void UnbalanceTree2()
        {
            var unbalancedTree =
                new Node(1,
                    new Node(2),
                    new Node(3,
                        new Node(4)),
                    new Node(5,
                        new Node(6),
                        new Node(7,
                            new Node(8))),
                    new Node(9,
                        new Node(10)));

            var visited = unbalancedTree.VisitAllNodes();

            Assert.AreEqual("1,2,3,4,5,6,7,8,9,10", visited);
        }


    }
}
