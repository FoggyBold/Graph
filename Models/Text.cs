using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Models
{
    public class Text
    {
        public Point Point { get; set; }
        public string TextInLable { get; set; }
        public Text(Point point, string text)
        {
            Point = point;
            TextInLable = text;
        }
    }
}
