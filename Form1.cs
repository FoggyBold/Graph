using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Graph.Container;
using Graph.Models;
using Graph.Action;

namespace Graph
{
    public partial class Form1 : Form
    {
        private SaveLoad saveLoad = new SaveLoad();

        private MathematicalFunctions functions = new MathematicalFunctions();
        public GraphNodes Nodes { get; set; }
        public GraphLines Lines { get; set; }
        private Node currNode = null;
        private Node currNodeForConnection = null;
        private Node secondNodeForConnection = null;
        public Form1()
        {
            InitializeComponent();
            Nodes = new GraphNodes();
            Lines = new GraphLines();
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Nodes.Nodes.Count > 0)
                {
                    Nodes.addNode(new Node(this.BackColor != Color.Black ? Color.White : Color.Black, e.Location, Nodes.Nodes[Nodes.Nodes.Count - 1].Id + 1));
                }
                else
                {
                    Nodes.addNode(new Node(this.BackColor != Color.Black ? Color.White : Color.Black, e.Location, 1));
                }
                ((Control)sender).Invalidate();
            }
        }

        private void findDotesForLine(out Point start, out Point end, Line line)
        {
            int width = line.Start.Dot.Width;
            double theta = Math.Atan2(line.End.Center.Y - line.Start.Center.Y, line.End.Center.X - line.Start.Center.X);
            int x1 = (int)(line.Start.Center.X + width / 2 * Math.Cos(theta));
            int y1 = (int)(line.Start.Center.Y + width / 2 * Math.Sin(theta));
            start = new Point(x1, y1);
            int x2 = (int)(line.End.Center.X - width / 2 * Math.Cos(theta));
            int y2 = (int)(line.End.Center.Y - width / 2 * Math.Sin(theta));
            end = new Point(x2, y2);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Node node in Nodes.Nodes)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(node.Pen, node.Dot);
            }
            foreach (Line line in Lines.Lines)
            {

                findDotesForLine(out Point start, out Point end, line);
                e.Graphics.DrawLine(line.Pen, start, end);
            }
        }

        //private void drawString(Line line, PaintEventArgs e)
        //{
        //    Font drawFont = new Font("Arial", 14);
        //    SolidBrush drawBrush = new SolidBrush(line.Style.Color);
        //    StringFormat drawFormat = new StringFormat();
        //    e.Graphics.DrawString(line.Text.TextInLable, drawFont, drawBrush, line.Text.Point, drawFormat);
        //    drawFont.Dispose();
        //    drawBrush.Dispose();
        //}

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (currNode == null)
                {
                    currNode = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                if (currNodeForConnection == null)
                {
                    currNodeForConnection = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
                }
                else
                {
                    secondNodeForConnection = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
                    if (secondNodeForConnection != null && currNodeForConnection != secondNodeForConnection)
                    {
                        //больше одной дороги из одной точки в другую быть не может
                        if (Lines.Lines.Find(l => l.Start == currNodeForConnection && l.End == secondNodeForConnection) != null)
                        {
                            DialogResult result = MessageBox.Show(
                                "Вы не можете провести больше одной дороги в этом направлении!",
                                "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            //если у нас есть уже дорога в обратном направлении, то нам не нужно спрашивать длинну, можно просто ее взять из существующей связи
                            double length = 0;
                            Line check = Lines.Lines.Find(l => l.End == currNodeForConnection && l.Start == secondNodeForConnection);
                            if (check != null)
                            {
                                length = check.Start.Сonnection.Find(n => n.Item1 == currNodeForConnection).Item2;
                                currNodeForConnection.Сonnection.Add(new Tuple<Node, double>(secondNodeForConnection, length));
                                Lines.addLine(new Line(currNodeForConnection, secondNodeForConnection, this.BackColor != Color.Black ? Color.White : Color.Black, length));
                            }
                            else
                            {
                                while (length <= 0)
                                {
                                    string inputValue = Microsoft.VisualBasic.Interaction.InputBox("Введите длинну:");
                                    if (inputValue != "")
                                    {
                                        double.TryParse(inputValue, out length);
                                        if (length > 0)
                                        {
                                            currNodeForConnection.Сonnection.Add(new Tuple<Node, double>(secondNodeForConnection, length));
                                            Lines.addLine(new Line(currNodeForConnection, secondNodeForConnection, this.BackColor != Color.Black ? Color.White : Color.Black, length));
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        length = 1;
                                    }
                                }
                            }
                            ((Control)sender).Invalidate();
                        }
                    }
                    currNodeForConnection = null;
                    secondNodeForConnection = null;
                }
            }
            //удаление вершин или дуг
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
                        Point point1 = new Point(l.Start.Center.X, l.Start.Center.Y);
                        Point point2 = new Point(l.End.Center.X, l.End.Center.Y);
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
                                    Tuple<Node, double> connection = tempNode.Сonnection.Find(n => n.Item1 == line.End);
                                    tempNode.Сonnection.Remove(connection);
                                }
                                else
                                {
                                    Tuple<Node, double> connection = tempNode.Сonnection.Find(n => n.Item1 == line.Start);
                                    tempNode.Сonnection.Remove(connection);
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
            if (e.Button == MouseButtons.Left)
            {
                currNode = null;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (currNode != null)
                {
                    currNode.changeCenter(e.Location);
                    foreach (Line line in Lines.Lines)
                    {
                        foreach (Node node in Nodes.Nodes.FindAll(n => n == line.Start || n == line.End))
                        {
                            line.updatePositionText();
                        }
                    }
                    ((Control)sender).Invalidate();
                }
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
            foreach (Node node in Nodes.Nodes)
            {
                node.updateColor(Color.Black);
            }
            foreach (Line line in Lines.Lines)
            {
                line.updateColor(Color.Black);
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
                node.updateColor(Color.White);
            }
            foreach (Line line in Lines.Lines)
            {
                line.updateColor(Color.White);
            }
            this.BackColor = Color.FromArgb(34, 38, 41);
            Graphics graphics = CreateGraphics();
            CreateGraphics().Clear(this.BackColor);
            Form1_Paint(sender, new PaintEventArgs(graphics, ClientRectangle));
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = saveLoad.SaveAsync(Nodes.Nodes);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveLoad.Load(out List<Line> lines, out List<Node> nodes, this.BackColor != Color.Black ? Color.White : Color.Black))
            {
                Nodes.Nodes = nodes;
                Lines.Lines = lines;
                Graphics graphics = CreateGraphics();
                graphics.Clear(this.BackColor);
                Form1_Paint(sender, new PaintEventArgs(graphics, ClientRectangle));
            }
        }
    }
}
