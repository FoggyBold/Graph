﻿using Graph.Action;
using Graph.Container;
using Graph.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Graph
{
    public partial class Form1 : Form
    {
        protected SaveLoad saveLoad = new SaveLoad();
        protected Form2 form2;
        public GraphNodes Nodes { get; set; }
        public GraphLines Lines { get; set; }
        protected Node currNode = null;
        protected Node currNodeForConnection = null;
        protected Node secondNodeForConnection = null;
        public Form1(Form2 form2)
        {
            InitializeComponent();
            Nodes = new GraphNodes();
            Lines = new GraphLines();
            this.form2 = form2;
        }
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
                Node newNode = null;
                if (Nodes.Nodes.Count > 0)
                {
                    newNode = new Node(this.BackColor != Color.Black ? Color.White : Color.Black, e.Location, Nodes.Nodes[Nodes.Nodes.Count - 1].Id + 1);
                }
                else
                {
                    newNode = new Node(this.BackColor != Color.Black ? Color.White : Color.Black, e.Location, 0);
                }
                Nodes.addNode(newNode);
                domainUpDown1.Items.Add(newNode.Id);
                domainUpDown2.Items.Add(newNode.Id);
                ((Control)sender).Invalidate();
            }
        }

        private void drawTextInRectangle(Rectangle dot, string text, Style style, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            TextRenderer.DrawText(e.Graphics, text, this.Font, dot, style.Color, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
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
                drawTextInRectangle(node.Dot, node.Id.ToString(), node.Style, e);
            }
            for (int i = 0; i < Lines.Lines.Count; ++i)
            {

                findDotesForLine(out Point start, out Point end, Lines.Lines[i]);
                e.Graphics.DrawLine(Lines.Lines[i].Pen, start, end);
                bool flag = false;
                for (int j = 0; j < i && !flag; ++j)
                {
                    flag = Lines.Lines[i].Start == Lines.Lines[j].End && Lines.Lines[i].End == Lines.Lines[j].Start;
                }
                if (!flag)
                {
                    drawTextInRectangle(new Rectangle(Lines.Lines[i].Text.Point, new Size(24, 24)), Lines.Lines[i].Text.TextInLable, Lines.Lines[i].Style, e);
                }
            }
        }

        protected virtual void Form1_MouseDown(object sender, MouseEventArgs e)
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
                                length = check.Start.Connection.Find(n => n.Item1 == currNodeForConnection).Item2;
                                currNodeForConnection.Connection.Add(new Tuple<Node, double>(secondNodeForConnection, length));
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
                                            currNodeForConnection.Connection.Add(new Tuple<Node, double>(secondNodeForConnection, length));
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
                setDefaultStyle(sender);
                Node node = Nodes.Nodes.Find(n => n.Dot.Contains(e.Location));
                if(node != null)
                {
                    foreach(Line line in Lines.Lines.FindAll(l => l.Start == node || l.End == node))
                    {
                        Lines.Delete(line);
                    }
                    Nodes.Delete(node);
                    changingValuesInDomainUpDown();
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
                                    Tuple<Node, double> connection = tempNode.Connection.Find(n => n.Item1 == line.End);
                                    tempNode.Connection.Remove(connection);
                                }
                                else
                                {
                                    Tuple<Node, double> connection = tempNode.Connection.Find(n => n.Item1 == line.Start);
                                    tempNode.Connection.Remove(connection);
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

        protected void changingValuesInDomainUpDown()
        {
            domainUpDown1.Items.Clear();
            domainUpDown2.Items.Clear();

            domainUpDown1.Text = "";
            domainUpDown2.Text = "";

            foreach (var node in Nodes.Nodes)
            {
                domainUpDown1.Items.Add(node.Id);
                domainUpDown2.Items.Add(node.Id);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                currNode = null;
            }
        }

        protected virtual void Form1_MouseMove(object sender, MouseEventArgs e)
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
            DialogResult clearMessageBox = MessageBox.Show("Вы действительно хотите очистить это окно?",
            "Очистка", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (clearMessageBox == DialogResult.Yes)
            {
                changingValuesInDomainUpDown();
                label7.Text = "";
                label1.Text = "";
                Nodes.Clear();
                Lines.Clear();
                CreateGraphics().Clear(this.BackColor);
            }
        }

        private void changeTheme(Color color)
        {
            foreach (Node node in Nodes.Nodes)
            {
                node.updateColor(color);
            }
            foreach (Line line in Lines.Lines)
            {
                line.updateColor(color);
            }
        }
        private void lightThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeTheme(Color.Black);
            this.BackColor = Color.White;

            clearAndPaint(sender);
        }

        private void darkThemeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            changeTheme(Color.White);
            this.BackColor = Color.FromArgb(34, 38, 41);

            clearAndPaint(sender);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ = saveLoad.SaveAsync(Nodes.Nodes);
        }

        protected void clearAndPaint(object sender)
        {
            currNode = null;
            currNodeForConnection = null;
            secondNodeForConnection = null;
            Graphics graphics = CreateGraphics();
            graphics.Clear(this.BackColor);
            Form1_Paint(sender, new PaintEventArgs(graphics, ClientRectangle));
        }

        protected void fillingDomainUpDown(List<Node> nodes)
        {
            foreach(var i in nodes)
            {
                domainUpDown1.Items.Add(i.Id);
                domainUpDown2.Items.Add(i.Id);
            }
        }

        protected virtual void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveLoad.Load(out List<Line> lines, out List<Node> nodes, this.BackColor != Color.White ? Color.White : Color.Black))
            {
                Nodes.Nodes = nodes;
                Lines.Lines = lines;

                fillingDomainUpDown(nodes);

                clearAndPaint(sender);
            }
        }

        protected virtual void search_Click(object sender, EventArgs e)
        {
            setDefaultStyle(sender);
            if (domainUpDown1.SelectedIndex != -1 && domainUpDown2.SelectedIndex != -1)
            {
                ShortestPath shortestPath = new ShortestPath(Nodes.Nodes, domainUpDown1.SelectedIndex, domainUpDown2.SelectedIndex);
                double min = shortestPath.minimumPath();
                if (min != int.MaxValue)
                {
                    List<int> path = shortestPath.path();
                    label1.Text = min.ToString();
                    label7.Text = "";
                    for (int i = 0; i < path.Count; i++)
                    {
                        label7.Text += path[i].ToString();
                        if (i != path.Count - 1)
                        {
                            label7.Text += "->";
                        }
                    }
                }
            }
        }

        protected void setDefaultStyle(object sender)
        {
            foreach (Node node in Nodes.Nodes)
            {
                if (node.Highlighted)
                {
                    node.highlighting(this.BackColor != Color.White ? Color.White : Color.Black);
                }
            }
            foreach(Line line in Lines.Lines)
            {
                if (line.Highlighted)
                {
                    line.highlighting(this.BackColor != Color.White ? Color.White : Color.Black);
                }
            }
            label7.Text = "";
            label1.Text = "";
            clearAndPaint(sender);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            var arrOfSubstr = label7.Text.Split('-', '>');
            string prev = "";
            foreach(var substr in arrOfSubstr)
            {
                if(substr.Length > 0)
                {
                    Nodes.Nodes.Find(n => n.Id.ToString() == substr).highlighting(this.BackColor != Color.White ? Color.White : Color.Black);

                    if (prev == "")
                    {
                        prev = substr;
                    }
                    else
                    {
                        var line = Lines.Lines.Find(l => l.Start.Id.ToString() == prev && l.End.Id.ToString() == substr);
                        if(line != null)
                        {
                            line.highlighting(this.BackColor != Color.White ? Color.White : Color.Black);
                        }
                        line = Lines.Lines.Find(l => l.Start.Id.ToString() == substr && l.End.Id.ToString() == prev);
                        if (line != null)
                        {
                            line.highlighting(this.BackColor != Color.White ? Color.White : Color.Black);
                        }
                        prev = substr;
                    }
                }
            }
            clearAndPaint(sender);
        }

        protected virtual void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            form2.closeForm1();
        }
    }
}
