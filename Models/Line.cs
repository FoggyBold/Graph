using System;
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
            Highlighted = false;
            Text = new Text(new Point(0, 0), length.ToString());
            updatePositionText();
        }

        public void updateLength(double length)
        {
            Length = length;
            Start.updateLength(length, End);
            Text.TextInLable = Length.ToString();
        }

        public void updatePositionText()
        {
            Point tempPointX = new Point(End.Dot.X, Start.Dot.Y);
            int lengthX = (int)Math.Sqrt((Start.Dot.X - tempPointX.X) * (Start.Dot.X - tempPointX.X) + (Start.Dot.Y - tempPointX.Y) * (Start.Dot.Y - tempPointX.Y));
            int lengthY = (int)Math.Sqrt((End.Dot.X - tempPointX.X) * (End.Dot.X - tempPointX.X) + (End.Dot.Y - tempPointX.Y) * (End.Dot.Y - tempPointX.Y));
            int newX, newY;
            if (Math.Abs(End.Dot.X - Start.Dot.X) >= Math.Abs(End.Dot.Y - Start.Dot.Y))
            {
                newX = End.Dot.X > Start.Dot.X ? Start.Dot.X + lengthX / 2 : Start.Dot.X - lengthX / 2;
                newY = End.Dot.Y > Start.Dot.Y ? Start.Dot.Y + lengthY / 2 + 10 : Start.Dot.Y - lengthY / 2 - 10;
            }
            else
            {
                newX = End.Dot.X > Start.Dot.X ? Start.Dot.X + lengthX / 2 + 10 : Start.Dot.X - lengthX / 2 - 10;
                newY = End.Dot.Y > Start.Dot.Y ? Start.Dot.Y + lengthY / 2 : Start.Dot.Y - lengthY / 2;
            }

            Text.Point = new Point(newX, newY);
        }
    }
}
