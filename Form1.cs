using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Graph.Container;
using Graph.Models;
using System.Text.Json;
using Graph.Action;

namespace Graph
{
    public partial class Form1 : Form
    {
        private SaveLoad saveLoad = new SaveLoad();
        private MathematicalFunctions functions = new MathematicalFunctions();
        public GraphNodes Nodes { get; set; }
        public GraphLines Lines { get; set; }
        private Node CurrNode { get; set; }
        private Node CurrNodeForConnection { get; set; }
        public Form1()
        {
            InitializeComponent();
            Nodes = new GraphNodes();
            Lines = new GraphLines();
            CurrNode = null;
            CurrNodeForConnection = null;
        }

        public Form1(List<Node> nodes, List<Line> lines)
        {
            InitializeComponent();
            Nodes = new GraphNodes(nodes);
            Lines = new GraphLines(lines);
            CurrNode = null;
            CurrNodeForConnection = null;
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Nodes.Nodes.Count > 0)
                {
                    Nodes.addNode(new Node(this.BackColor, e.Location, Nodes.Nodes[Nodes.Nodes.Count - 1].Id + 1));
                }
                else
                {
                    Nodes.addNode(new Node(this.BackColor, e.Location, 1));
                }
                ((Control)sender).Invalidate();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Node node in Nodes.Nodes)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.DrawEllipse(node.DrawingPen, node.Dot);

                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
            }
            for (int i = 0; i < Lines.Lines.Count; i++)
            {
                int width = Lines.Lines[i].Start.Dot.Width;

                Point point1 = new Point(Lines.Lines[i].Start.Dot.Left + width / 2, Lines.Lines[i].Start.Dot.Y + width / 2);
                Point point2 = new Point(Lines.Lines[i].End.Dot.Left + width / 2, Lines.Lines[i].End.Dot.Y + width / 2);

                e.Graphics.DrawLine(Lines.Lines[i].DrawingPen, point1, point2);
                bool flag = true;
                for(int j = 0; j < i && flag; j++)
                {
                    if(Lines.Lines[j].Start == Lines.Lines[i].End && Lines.Lines[j].End == Lines.Lines[i].Start)
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    drawString(Lines.Lines[i], e);
                }
            }
        }

        private void drawString(Line line, PaintEventArgs e)
        {
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(line.style.Color);
            StringFormat drawFormat = new StringFormat();
            e.Graphics.DrawString(line.text.TextInLable, drawFont, drawBrush, line.text.Point, drawFormat);
            drawFont.Dispose();
            drawBrush.Dispose();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrNode == null && e.Button == MouseButtons.Left)
            {
                CurrNode = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
            }
            else if(e.Button == MouseButtons.Right)
            {
                if (CurrNodeForConnection == null)
                {
                    CurrNodeForConnection = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
                }
                else
                {
                    Node tempNode = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
                    if (tempNode != null && CurrNodeForConnection != tempNode)
                    {
                        //больше одной дороги из одной точки в другую быть не может
                        if (Lines.Lines.Find(l => l.Start == CurrNodeForConnection && l.End == tempNode) != null)
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
                            Line check = Lines.Lines.Find(l => l.End == CurrNodeForConnection && l.Start == tempNode);
                            if (check != null)
                            {
                                length = check.Start.Сonnection.Find(n => n.Item1 == CurrNodeForConnection).Item2;
                                CurrNodeForConnection.Сonnection.Add(new Tuple<Node, double>(tempNode, length));
                                Lines.addLine(new Line(CurrNodeForConnection, tempNode, this.BackColor, length));
                            }
                            else
                            {
                                bool flag = true;
                                while (length == 0 && flag)
                                {
                                    string inputValue = Microsoft.VisualBasic.Interaction.InputBox("Введите длинну:");
                                    if (inputValue != "")
                                    {
                                        double.TryParse(inputValue, out length);
                                    }
                                    else
                                    {
                                        flag = false;
                                    }
                                }
                                if (flag)
                                {
                                    CurrNodeForConnection.Сonnection.Add(new Tuple<Node, double>(tempNode, length));
                                    Lines.addLine(new Line(CurrNodeForConnection, tempNode, this.BackColor, length));
                                }
                            }
                            ((Control)sender).Invalidate();
                        }
                    }
                    CurrNodeForConnection = null;
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
            if(CurrNode != null && e.Button == MouseButtons.Left)
            {
                CurrNode.Dot = new Rectangle(e.Location, CurrNode.Dot.Size);
                CurrNode = null;
                ((Control)sender).Invalidate();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CurrNode != null)
            {
                CurrNode.Dot = new Rectangle(e.Location, CurrNode.Dot.Size);
                foreach (Line line in Lines.Lines)
                {
                    foreach(Node node in Nodes.Nodes.FindAll(n => n == line.Start || n == line.End))
                    {
                        line.updatePositionText();
                    }
                }
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
            this.BackColor = Color.FromArgb(34, 38, 41);
            Graphics graphics = CreateGraphics();
            graphics.Clear(this.BackColor);
            Form1_Paint(sender, new PaintEventArgs(graphics, ClientRectangle));
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = saveLoad.SaveAsync(Nodes.Nodes);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Line> lines = new List<Line>();
            List<Node> nodes = new List<Node>();
            if (saveLoad.Load(out lines, out nodes, this.BackColor))
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
