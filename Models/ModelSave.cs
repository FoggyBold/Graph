using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Graph.Models
{
    public class ModelSave
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<Tuple<int, double>> Connection { get; set; }
        public ModelSave(Node point)
        {
            Id = point.Id;
            X = point.Dot.X;
            Y = point.Dot.Y;
            Connection = new List<Tuple<int, double>>();
            for (int i = 0; i < point.Сonnection.Count; i++)
            {
                Connection.Add(new Tuple<int, double>(point.Сonnection[i].Item1.Id, point.Сonnection[i].Item2));
            }
        }

        public ModelSave(int Id, int X, int Y, List<Tuple<int, double>> Connection)
        {
            this.Id = Id;
            this.X = X;
            this.Y = Y;
            this.Connection = Connection;
        }
    }
}