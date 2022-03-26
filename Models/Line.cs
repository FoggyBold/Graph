using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Graph.Models
{
    public class Line : WindowObject
    {
        public Node Start { get; }
        public Node End { get; }
        public double Length { get; set; }
        public Line(Node start, Node end, Color color, double length)
        {
            Style = new Style(new Size(6, 6), color);
            Pen = new Pen(Style.Color, 2);
            //установка стрелочки в конце линии
            Pen.CustomEndCap = new AdjustableArrowCap(Style.Size.Height, Style.Size.Width);
            Start = start;
            End = end;
            Length = length;
            Text = new Text(new Point(0, 0), length.ToString());
            updatePositionText();
        }
        public void updatePositionText()
        {
            //расчет координат середины линии с отступом по Y
            Point tempPointX = new Point(End.Dot.X, Start.Dot.Y);
            int lengthX = (int)Math.Sqrt((Start.Dot.X - tempPointX.X) * (Start.Dot.X - tempPointX.X) + (Start.Dot.Y - tempPointX.Y) * (Start.Dot.Y - tempPointX.Y));
            int lengthY = (int)Math.Sqrt((End.Dot.X - tempPointX.X) * (End.Dot.X - tempPointX.X) + (End.Dot.Y - tempPointX.Y) * (End.Dot.Y - tempPointX.Y));
            int newX = End.Dot.X > Start.Dot.X ? Start.Dot.X + lengthX / 2 : Start.Dot.X - lengthX / 2;
            int newY = End.Dot.Y > Start.Dot.Y ? Start.Dot.Y + lengthY / 2 + 10 : Start.Dot.Y - lengthY / 2 - 10;

            Text.Point = new Point(newX, newY);
        }
    }

    public class LineComparer : IComparer<Line>
    {
        public int Compare(Line x, Line y)
        {
            if (x.Start == y.End && x.End == y.Start)
                return 1;
            return -1;
        }
    }
}
