using Graph.Models;
using System.Collections.Generic;

namespace Graph.Action
{
    public class AdjacencyMatrix
    {
        public double[,] Matrix { get; set; }
        public AdjacencyMatrix(List<Node> nodes)
        {
            Matrix = convertToMatrix(nodes);
        }
        private double[,] convertToMatrix(List<Node> nodes)
        {
            nodes.Sort(new NodeComparer());

            double[,] res = new double[nodes.Count, nodes.Count];
            setNulls(res);

            foreach(Node node in nodes)
            {
                foreach(var connectNode in node.Сonnection)
                {
                    res[node.Id, connectNode.Item1.Id] = connectNode.Item2;
                }
            }

            return res;
        }
        private void setNulls(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = 0;
                }
            }
        }
    }
}
