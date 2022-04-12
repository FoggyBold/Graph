using System.Drawing;

namespace Graph.Models
{
    public class WindowObject
    {
        public Pen Pen { get; set; }
        public Style Style { get; set; }
        public Text Text { get; set; }
        public bool Highlighted { get; set; }
        public void updateColor(Color color)
        {
            Pen.Color = color;
            Style.Color = color;
        }
        public void delete()
        {
            if (Pen != null)
            {
                Pen.Dispose();
            }
        }
        public void highlighting(Color color)
        {
            if (!Highlighted)
            {
                updateColor(Color.Red);
                Highlighted = true;
            }
            else
            {
                updateColor(color);
                Highlighted = false;
            }
        }
    }
}
