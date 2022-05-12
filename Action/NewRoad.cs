using Graph.Models;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Graph.Action
{
    public class NewRoad
    {
        private readonly double sizeOfOneStep;
        private readonly int start;
        private readonly int end;
        private readonly double startLength;
        private List<Node> nodes;

        public NewRoad(List<Node> nodes, int start, int end, double sizeOfOneStep, double startLength)
        {
            this.start = start;
            this.end = end;
            this.sizeOfOneStep = sizeOfOneStep;
            this.startLength = startLength;
            this.nodes = nodes;
        }

        public Line findNewRoad()
        {
            Line newLine = null;
            foreach (Node node in nodes)
            {
                foreach (Node node1 in nodes)
                {
                    if (node != node1 && node1.Id != end)
                    {
                        if (node.Connection.Find(u => u.Item1 == node1) == null)
                        {
                            Tuple<Node, double> newRoad = createNewRoad(node, node1);
                            node.Connection.Add(newRoad);
                            newLine = new Line(node, node1, Color.Black, newRoad.Item2);
                            ShortestPath temp = new ShortestPath(nodes, start, end);
                            if (temp.minimumPath() < startLength)
                            {
                                break;
                            }
                            else
                            {
                                node.Connection.Remove(newRoad);
                                newLine = null;
                            }
                        }
                    }
                }
                if(newLine != null)
                {
                    break;
                }
            }
            if (newLine == null)
            {
                Node startNode = nodes.Find(u => u.Id == start);
                Node endNode = nodes.Find(u => u.Id == end);
                if (startNode.Connection.Find(u => u.Item1 == endNode) == null)
                {
                    Tuple<Node, double> newRoad = createNewRoad(startNode, endNode);
                    startNode.Connection.Add(newRoad);
                    newLine = new Line(startNode, endNode, Color.Black, newRoad.Item2);
                    ShortestPath temp = new ShortestPath(nodes, start, end);
                }
            }
            return newLine;
        }

        private Tuple<Node, double> createNewRoad(Node start, Node end)
        {
            double length = Math.Sqrt(Math.Pow((start.Center.X - end.Center.X), 2) + Math.Pow((start.Center.Y - end.Center.Y), 2));
            Tuple<Node, double> res = new Tuple<Node, double>(end, Math.Round(length * sizeOfOneStep, 1));
            return res;
        }
    }
}
