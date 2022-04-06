using Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Action
{
    public class ShortestPath
    {
        private AdjacencyMatrix adjacencyMatrix;
        private readonly int start;
        private readonly int end;
        private List<double> weights = null;
        private List<bool> visited = null;
        private List<int> ancestors = null;
        public ShortestPath(List<Node> nodes, int start, int end)
        {
            adjacencyMatrix = new AdjacencyMatrix(nodes);
            this.start = start;
            this.end = end;

            weights = new List<double>(nodes.Count);
            visited = new List<bool>(nodes.Count);
            ancestors = new List<int>(nodes.Count);
            for (int i = 0; i < nodes.Count; i++)
            {
                weights.Add(int.MaxValue);
                visited.Add(false);
                ancestors.Add(-1);
            }
            visited[start] = true;
            weights[start] = 0;
        }

        private void countMinimumPath(int curr)
        {
            for (int i = 0; i < adjacencyMatrix.Matrix.GetLength(0); ++i)
            {
                if (adjacencyMatrix.Matrix[curr, i] != 0) 
                {
                    if (weights[i] > weights[curr] + adjacencyMatrix.Matrix[curr, i])
                    {
                        weights[i] = weights[curr] + adjacencyMatrix.Matrix[curr, i];
                        ancestors[i] = curr;
                    }
                }
            }
            visited[curr] = true;
            int nextIndex = findMinIndex();
            if(!visited[nextIndex])
            {
                countMinimumPath(findMinIndex());
            }
        }

        private int findMinIndex()
        {
            int min = 0;
            for (int i = 0; i < weights.Count; ++i)
            {
                if(!visited[i] && (weights[i] < weights[min] || visited[min]))
                {
                    min = i;
                }
            }
            return min;
        }

        public double minimumPath()
        {
            countMinimumPath(start);
            return weights[end];
        }

        public List<int> path()
        {
            List<int> path = new List<int>();
            for (int v = end; v != start; v = ancestors[v])
            {
                path.Add(v);
            }
            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}
