using Graph.Action;
using Graph.Container;
using Graph.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Graph.Forms
{
    public partial class Form3 : Form1
    {
        private double step;

        public Form3(Form2 form2) : base(form2)
        {
            InitializeComponent();
            step = 0;
        }

        private void updateLength(Line line)
        {
            double length = Math.Sqrt(Math.Pow((line.Start.Center.X - line.End.Center.X), 2) + Math.Pow((line.Start.Center.Y - line.End.Center.Y), 2));
            line.updateLength(Math.Round(length * step, 1));
        }

        protected override void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (currNode != null)
                {
                    currNode.changeCenter(e.Location);
                    foreach (Line line in Lines.Lines)
                    {
                        line.updatePositionText();
                        updateLength(line);
                    }
                    ((Control)sender).Invalidate();
                }
            }
        }

        protected override void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (currNode == null)
                {
                    currNode = Nodes.Nodes.Find(n => n.Dot.Contains(e.X, e.Y));
                }
            }
            else if (e.Button == MouseButtons.Right)
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
                            Line check = Lines.Lines.Find(l => l.End == currNodeForConnection && l.Start == secondNodeForConnection);
                            if (check != null)
                            {
                                double length = check.Start.Connection.Find(n => n.Item1 == currNodeForConnection).Item2;
                                currNodeForConnection.Connection.Add(new Tuple<Node, double>(secondNodeForConnection, length));
                                Lines.addLine(new Line(currNodeForConnection, secondNodeForConnection, this.BackColor != Color.Black ? Color.White : Color.Black, length));
                            }
                            else
                            {
                                if (step == 0)
                                {
                                    double length = 0;
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
                                                step = Math.Round(length / Math.Sqrt(Math.Pow((secondNodeForConnection.Center.X - currNodeForConnection.Center.X),2) + Math.Pow((secondNodeForConnection.Center.Y - currNodeForConnection.Center.Y), 2)), 2);
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            length = 1;
                                        }
                                    }
                                }
                                else
                                {
                                    double length = Math.Sqrt(Math.Pow((secondNodeForConnection.Center.X - currNodeForConnection.Center.X), 2) + Math.Pow((secondNodeForConnection.Center.Y - currNodeForConnection.Center.Y), 2));
                                    currNodeForConnection.Connection.Add(new Tuple<Node, double>(secondNodeForConnection, Math.Round(length * step, 1)));
                                    Lines.addLine(new Line(currNodeForConnection, secondNodeForConnection, this.BackColor != Color.Black ? Color.White : Color.Black, Math.Round(length * step, 1)));
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
            else if (e.Button == MouseButtons.Middle)
            {
                setDefaultStyle(sender);
                Node node = Nodes.Nodes.Find(n => n.Dot.Contains(e.Location));
                if (node != null)
                {
                    foreach (Line line in Lines.Lines.FindAll(l => l.Start == node || l.End == node))
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
                    foreach (Line l in Lines.Lines)
                    {
                        int width = l.Start.Dot.Width;
                        Point point1 = new Point(l.Start.Center.X, l.Start.Center.Y);
                        Point point2 = new Point(l.End.Center.X, l.End.Center.Y);
                        if (Math.Abs((double)(e.Location.X - point1.X) / (point2.X - point1.X) - (double)(e.Location.Y - point1.Y) / (point2.Y - point1.Y)) <= 0.1)
                        {
                            lines.Add(l);
                        }
                    }
                    if (lines.Count > 0)
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

        protected override void search_Click(object sender, EventArgs e)
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
                DialogResult question = MessageBox.Show(
                                    "Попробовать уменьшить найденный минимальный путь?",
                                    "Вопрос",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (question == DialogResult.Yes)
                {
                    NewRoad newRoad = new NewRoad(Nodes.Nodes, domainUpDown1.SelectedIndex, domainUpDown2.SelectedIndex, step, min);
                    Line newLine = newRoad.findNewRoad();
                    if (newLine != null)
                    {
                        label1.Text = "";
                        label7.Text = "";
                        newLine.updateColor(BackColor == Color.White ? Color.Black : Color.White);
                        Lines.addLine(newLine);
                        clearAndPaint(sender);
                    }
                    else
                    {
                        DialogResult result = MessageBox.Show(
                                    "Мы не смогли найти способы уменьшения пути!",
                                    "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        protected override void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveLoad.Load(out List<Line> lines, out List<Node> nodes, this.BackColor != Color.White ? Color.White : Color.Black))
            {
                if (lines != null)
                {
                    step = Math.Round(lines[0].Length / Math.Sqrt(Math.Pow((lines[0].Start.Center.X - lines[0].End.Center.X), 2) + Math.Pow((lines[0].Start.Center.Y - lines[0].End.Center.Y), 2)), 2);
                }
                Nodes.Nodes = nodes;
                Lines.Lines = lines;

                fillingDomainUpDown(nodes);

                clearAndPaint(sender);
            }
        }

        protected override void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            form2.closeForm3();
        }
    }
}
