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
        public GraphLines Lines { get; set; }
        private Node currNode { get; set; }
        private Node currNodeForConnection { get; set; }
        public Form1()
        {
            InitializeComponent();
            Nodes = new GraphNodes();
            Lines = new GraphLines();
            currNode = null;
            currNodeForConnection = null;
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Node newPoint = new Node();
                newPoint.Dot = new Rectangle(e.Location, new Size(10, 10));
                //newPoint.DrawingPen = new Pen(Color.White, 2);
                Nodes.addNode(newPoint);
                ((Control)sender).Invalidate();
            }
            //else if(e.Button == MouseButtons.Right)
            //{
            //Nodes.Delete(Nodes.Nodes.Find(n => n.Dot.Contains(e.Location)));
            //((Control)sender).Invalidate();
            //}
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Node node in Nodes.Nodes)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(node.DrawingPen, node.Dot);
            }
            foreach (Line line in Lines.Lines)
            {
                int width = line.Start.Dot.Width;
                Point point1 = new Point(line.Start.Dot.Left + width / 2, line.Start.Dot.Y + width / 2);
                Point point2 = new Point(line.End.Dot.Left + width / 2, line.End.Dot.Y + width / 2);
                e.Graphics.DrawLine(line.DrawingPen, point1, point2);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (currNode == null && e.Button == MouseButtons.Left)
            {
                currNode = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
            }
            else if(e.Button == MouseButtons.Right)
            {
                if (currNodeForConnection == null)
                {
                    currNodeForConnection = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
                }
                else
                {
                    Node tempNode = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
                    if (tempNode!=null)
                    {
                        currNodeForConnection.Сonnection.Add(tempNode);
                        tempNode.Сonnection.Add(currNodeForConnection);
                        Lines.addLine(new Line(currNodeForConnection, tempNode));
                        ((Control)sender).Invalidate();
                    }
                    currNodeForConnection = null;
                }
            }
            else if(e.Button == MouseButtons.Middle)
            {
                Node node = Nodes.Nodes.Find(n => n.Dot.Contains(e.Location));
                if(node != null)
                {
                    foreach(Line line in Lines.Lines.FindAll(l => l.Start == node || l.End == node))
                    {
                        Lines.Delete(line);
                    }
                    Nodes.Delete(node);
                }
                else
                {

                    //Line line = Lines.Lines.Find(n => )
                }
                //Lines.Delete(Lines.Lines.)
                ((Control)sender).Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if(currNode != null && e.Button == MouseButtons.Left)
            {
                currNode.Dot = new Rectangle(e.Location, new Size(10, 10));
                currNode = null;
                ((Control)sender).Invalidate();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && currNode != null)
            {
                currNode.Dot = new Rectangle(e.Location, new Size(10, 10));
                ((Control)sender).Invalidate();
            }
        }

        //private void Form1_MouseMove(object sender, MouseEventArgs e)
        //{
            //if (e.Button == MouseButtons.Left && currNode != null)
            //{

            //}
        //}
    }
}
