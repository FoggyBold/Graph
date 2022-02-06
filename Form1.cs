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
                Node newPoint = new Node(this.BackColor);
                newPoint.Dot = new Rectangle(e.Location, new Size(10, 10));
                Nodes.addNode(newPoint);
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
                        Lines.addLine(new Line(currNodeForConnection, tempNode, this.BackColor));
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
                    ((Control)sender).Invalidate();
                }
                else
                {
                    //удаление связей, использую общее уравнение прямой, чтоб понять, лежит ли курсор на линии 
                    List<Line> lines = new List<Line>();
                    foreach(Line l in Lines.Lines)
                    {
                        int width = l.Start.Dot.Width;
                        Point point1 = new Point(l.Start.Dot.Left + width / 2, l.Start.Dot.Y + width / 2);
                        Point point2 = new Point(l.End.Dot.Left + width / 2, l.End.Dot.Y + width / 2);
                        if (Math.Abs((double)(e.Location.X - point1.X) / (point2.X - point1.X) - (double)(e.Location.Y - point1.Y) / (point2.Y - point1.Y)) <= 0.1)
                        {
                            lines.Add(l);
                        }
                    }
                    if(lines.Count > 0)
                    {
                        foreach (Line line in lines)
                        {
                            foreach (Node tempNode in Nodes.Nodes.FindAll(n => n == line.Start || n == line.End))
                            {
                                if (tempNode == line.Start)
                                {
                                    tempNode.Сonnection.Remove(line.End);
                                }
                                else
                                {
                                    tempNode.Сonnection.Remove(line.Start);
                                }
                            }
                            Lines.Delete(line);
                        }
                        lines.Clear();
                        ((Control)sender).Invalidate();
                    }
                }
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

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult clearMessageBox = MessageBox.Show("Do you really want to clear this form?",
            "Reset Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (clearMessageBox == DialogResult.Yes)
            {
                Nodes.Clear();
                Lines.Clear();
                CreateGraphics().Clear(this.BackColor);
            }
            
        }

        private void lightThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(Node node in Nodes.Nodes)
            {
                node.DrawingPen = new Pen(Color.Black, 2);
            }
            foreach(Line line in Lines.Lines)
            {
                line.DrawingPen = new Pen(Color.Black, 2);
            }
            this.BackColor = Color.White;
            Graphics graphics = CreateGraphics();
            graphics.Clear(this.BackColor);
            Form1_Paint(sender, new PaintEventArgs(graphics, ClientRectangle));
        }

        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Node node in Nodes.Nodes)
            {
                node.DrawingPen = new Pen(Color.White, 2);
            }
            foreach (Line line in Lines.Lines)
            {
                line.DrawingPen = new Pen(Color.White, 2);
            }
            this.BackColor = Color.Black;
            Graphics graphics = CreateGraphics();
            graphics.Clear(this.BackColor);
            Form1_Paint(sender, new PaintEventArgs(graphics, ClientRectangle));
        }
    }
}
