using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Graph.Container;
using Graph.Models;

namespace Graph
{
    public partial class Form1 : Form
    {
        public GraphNodes Nodes { get; set; }
        public Form1()
        {
            InitializeComponent();
            Nodes = new GraphNodes();
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Node newPoint = new Node();
                newPoint.Dot = new Rectangle(e.Location, new Size(10, 10));
                newPoint.DrawingPen = new Pen(Color.Red, 2);
                Nodes.Nodes.Add(newPoint);
                ((Control)sender).Invalidate();
            }
            else if(e.Button == MouseButtons.Right)
            {
                Nodes.Delete(Nodes.Nodes.Find(n => n.Dot.Contains(e.Location)));
                ((Control)sender).Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Node node in Nodes.Nodes)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(node.DrawingPen, node.Dot);
            }
        }
    }
}
