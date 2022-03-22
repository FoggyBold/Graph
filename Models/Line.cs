using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Models
{
    public class Line : WindowObject
    {
        private Node start = null;
        private Node end = null;
        public Style style;
        public Text text;
        public Line(Node start, Node end, Color color, double length)
        {
            style = new Style(new Size(6, 6), color == Color.White ? Color.Black : Color.White);

            this.m_Pen = new Pen(style.Color, 2);
            m_Pen.CustomEndCap = new AdjustableArrowCap(style.Size.Height, style.Size.Width);

            this.start = start;
            this.end = end;

            text = new Text(new Point(0, 0), length.ToString());
            updatePositionText();
        }
        public Pen DrawingPen 
        { 
            get => m_Pen; 
            set 
            {
                m_Pen = value;
                m_Pen.CustomEndCap = new AdjustableArrowCap(style.Size.Height, style.Size.Width);
            }
        }
        public Node Start { get => start; }
        public Node End { get => end; }

        public void updatePositionText()
        {
            Point tempPointX = new Point(end.Dot.X, start.Dot.Y);
            int lengthX = (int)Math.Sqrt((start.Dot.X - tempPointX.X) * (start.Dot.X - tempPointX.X) + (start.Dot.Y - tempPointX.Y) * (start.Dot.Y - tempPointX.Y));
            int lengthY = (int)Math.Sqrt((end.Dot.X - tempPointX.X) * (end.Dot.X - tempPointX.X) + (end.Dot.Y - tempPointX.Y) * (end.Dot.Y - tempPointX.Y));
            int newX = end.Dot.X > start.Dot.X ? start.Dot.X + lengthX / 2 : start.Dot.X - lengthX / 2;
            int newY = end.Dot.Y > start.Dot.Y ? start.Dot.Y + lengthY / 2 + 10 : start.Dot.Y - lengthY / 2 - 10;

            text.Point = new Point(newX, newY);
        }
    }
}
