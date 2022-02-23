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
        public Line(Node start, Node end, Color color)
        {
            this.m_Pen = new Pen(color == Color.White ? Color.Black : Color.White, 2);
            m_Pen.CustomEndCap = new AdjustableArrowCap(6, 6);
            this.start = start;
            this.end = end;
        }
        public Pen DrawingPen 
        { 
            get => m_Pen; 
            set 
            {
                m_Pen = value;
                m_Pen.CustomEndCap = new AdjustableArrowCap(6, 6);
            }
        }
        public Node Start { get => start; }
        public Node End { get => end; }
    }
}
