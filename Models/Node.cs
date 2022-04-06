using System;
using System.Collections.Generic;
using System.Drawing;

namespace Graph.Models
{
    public class Node : WindowObject
    {
        public int Id { get; set; }
        public List<Tuple<Node, double>> Сonnection { get; set; }
        public Rectangle Dot { get; set; }
        public Point Center { get; set; }
        public Node(Color color, Point newPoint, int id)
        {
            Id = id;
            Style = new Style(new Size(24, 24), color);
            Pen = new Pen(Style.Color, 2);
            Сonnection = new List<Tuple<Node, double>>();
            changeCenter(newPoint);
        }
        public void changeCenter(Point newCenter)
        {
            Point location = new Point(newCenter.X - Style.Size.Width / 2, newCenter.Y - Style.Size.Width / 2);
            Dot = new Rectangle(location, Style.Size);
            Center = newCenter;
        }
    }

    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node x, Node y)
        {
            if (x.Id < y.Id)
                return 1;
            return -1;
        }
    }
}
