using Graph.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graph.Action
{
    public class SaveLoad
    {
        public async Task SaveAsync(List<Node> nodes)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save text Files";
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "json";
            saveFileDialog1.Filter = "Text files (*.json)|*.json";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                using (Stream fs = saveFileDialog1.OpenFile())
                {
                    List<ModelSave> list = new List<ModelSave>();
                    foreach (Node node in nodes)
                    {
                        list.Add(new ModelSave(node));
                    }
                    await JsonSerializer.SerializeAsync<List<ModelSave>>(fs, list);
                    fs.Close();
                }
            }
        }

        public bool Load(out List<Line> Lines, out List<Node> Nodes, Color color)
        {
            Nodes = new List<Node>();
            Lines = new List<Line>();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.Title = "Load text Files";
            openFileDialog.CheckPathExists = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.DefaultExt = "json";
            openFileDialog.Filter = "Text files (*.json)|*.json";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
            if (flag)
            {
                List<ModelSave> list = new List<ModelSave>();
                using (Stream fs = openFileDialog.OpenFile())
                {
                    list = JsonSerializer.Deserialize<List<ModelSave>>(fs);
                    fs.Close();
                }
                foreach (ModelSave model in list)
                {
                    Nodes.Add(new Node(color, new Point(model.X, model.Y), model.Id));
                }
                foreach (ModelSave model in list)
                {
                    foreach (var i in model.Connection)
                    {
                        Nodes.Find(n => n.Id == model.Id).Сonnection.Add(new Tuple<Node, double>(Nodes.Find(n => n.Id == i.Item1), i.Item2));
                        Lines.Add(new Line(Nodes.Find(n => n.Id == model.Id), Nodes.Find(n => n.Id == i.Item1), color, i.Item2));
                    }
                }
            }

            return flag;
        }
    }
}
