using System.Drawing;

namespace Graph.Models
{
    public class Style
    {
        public Color Color { get; set; }
        public Size Size { get; set; }
        public Style(Size size, Color color)
        {
            Size = size;
            Color = color;
        }
    }
}
