using System;
using System.Collections.Generic;
using System.Drawing;

namespace Graph.Models
{
    public class Node : WindowObject
    {
        public int Id { get; set; }
        public List<Tuple<Node, double>> Connection { get; set; }
        public Rectangle Dot { get; set; }
        public Point Center { get; set; }
        public Node(Color color, Point newPoint, int id)
        {
            Id = id;
            Style = new Style(new Size(24, 24), color);
            Pen = new Pen(Style.Color, 2);
            Connection = new List<Tuple<Node, double>>();
            Highlighted = false;
            changeCenter(newPoint);
        }
        public void changeCenter(Point newCenter)
        {
            Point location = new Point(newCenter.X - Style.Size.Width / 2, newCenter.Y - Style.Size.Width / 2);
            Dot = new Rectangle(location, Style.Size);
            Center = newCenter;
        }
        public void updateLength(double length, Node node)
        {
            Connection.Remove(Connection.Find(u => u.Item1 == node));
            Connection.Add(new Tuple<Node, double>(node, length));
        }
    }
}
