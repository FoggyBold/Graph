using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //if(Nodes.Find(x => x == newNode && x.Y == newNode.Y) == null)
            //{
            //    Nodes.Add(newNode);
            //}
            if (newNode.Dot.Size.Width > 1 && newNode.Dot.Size.Height > 1)
                Nodes.Add(newNode);
        }
        public void Delete(Node node)
        {
            if (node != null)
            {
                node.Delete();
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
                        node.Delete();
                    }
                }
            }
        }
    }
}
