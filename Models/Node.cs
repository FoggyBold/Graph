using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Style = new Style(new Size(20, 20), color);
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
}
