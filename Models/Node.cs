using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Models
{
    public class Node
    {
        private Pen m_Pen = null;
        private Rectangle m_Dot = Rectangle.Empty;

        public Node() : this(Point.Empty) { }
        public Node(Point newPoint)
        {
            this.m_Pen = new Pen(Color.Red, 1);
            this.m_Dot = new Rectangle(newPoint, new Size(2, 2));
            //Сonnection = new List<Node>();
        }

        public Pen DrawingPen { get => m_Pen; set => m_Pen = value; }
        public Rectangle Dot { get => m_Dot; set => m_Dot = value; }

        public void Delete()
        {
            if (m_Pen != null)
            {
                m_Pen.Dispose();
            }
        }
    }
}
