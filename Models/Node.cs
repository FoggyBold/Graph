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
        private Rectangle m_Dot = Rectangle.Empty;
        public List<Tuple<Node, double>> Сonnection { get; set; }
        public Node(Color color, Point newPoint, int id)
        {
            Id = id;
            style = new Style(new Size(14, 14), color == Color.White ? Color.Black : Color.White);
            this.m_Pen = new Pen(style.Color, 2);
            this.m_Dot = new Rectangle(newPoint, style.Size);
            Сonnection = new List<Tuple<Node, double>>();
        }
        public Pen DrawingPen { get => m_Pen; set => m_Pen = value; }
        public Rectangle Dot { get => m_Dot; set => m_Dot = value; }
    }
}
