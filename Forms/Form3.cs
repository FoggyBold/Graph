using Graph.Container;
using Graph.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graph.Forms
{
    public partial class Form3 : Form1
    {
        private double step;

        public Form3(Form2 form2)
        {
            InitializeComponent();
            step = 0;
            Nodes = new GraphNodes();
            Lines = new GraphLines();
            this.form2 = form2;
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
                                double length = check.Start.Сonnection.Find(n => n.Item1 == currNodeForConnection).Item2;
                                currNodeForConnection.Сonnection.Add(new Tuple<Node, double>(secondNodeForConnection, length));
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
                                                currNodeForConnection.Сonnection.Add(new Tuple<Node, double>(secondNodeForConnection, length));
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
                                    currNodeForConnection.Сonnection.Add(new Tuple<Node, double>(secondNodeForConnection, Math.Round(length * step, 1)));
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

        protected override void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            form2.closeForm3();
        }
    }
}
