using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Models
{
    public class Style
    {
        public Color Color { get; set; }
        public Size Size { get; set; }

        public Style()
        {
            Size = new Size(14, 14);
            Color = Color.Blue;
        }

        public Style(Size size, Color color)
        {
            Size = size;
            Color = color;
        }
    }
}
