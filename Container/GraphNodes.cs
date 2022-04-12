using System;
using System.Collections.Generic;
using Graph.Models;

namespace Graph.Container
{
    public class GraphNodes : IDisposable
    {
        bool IsDisposed = false;
        public List<Node> Nodes { get; set; }
        public GraphNodes() => Nodes = new List<Node>();
        public GraphNodes(List<Node> nodes) => Nodes = nodes;
        public void addNode(Node newNode)
        {
            if (newNode.Dot.Size.Width > 1 && newNode.Dot.Size.Height > 1)
                Nodes.Add(newNode);
        }
        public void Delete(Node node)
        {
            if (node != null)
            {
                foreach (Node temp in Nodes)
                {
                    temp.Сonnection.Remove(temp.Сonnection.Find(m => m.Item1.Id == node.Id));
                }

                foreach (Node temp in Nodes)
                {
                    if (temp.Id > node.Id)
                    {
                        temp.Id--;
                    }
                }
                node.delete();
                Nodes.Remove(node);
            }
        }
        public void Clear()
        {
            Dispose();
            Nodes.Clear();
            Nodes = new List<Node>();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool IsSafeDisposing)
        {
            if (IsSafeDisposing && (!this.IsDisposed) && (this.Nodes.Count > 0))
            {
                foreach (Node node in this.Nodes)
                {
                    if (node != null)
                    {
                        node.delete();
                    }
                }
            }
        }
    }
}
