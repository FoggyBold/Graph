using Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Container
{
    public class GraphLines : IDisposable
    {
        bool IsDisposed = false;
        public List<Line> Lines { get; set; }
        public GraphLines() => Lines = new List<Line>();
        public void addLine(Line newLine)
        {
            Lines.Add(newLine);
        }
        public void Delete(Line line)
        {
            if (line != null)
            {
                line.Delete();
                Lines.Remove(line);
            }
        }
        public void Clear()
        {
            Dispose();
            Lines.Clear();
            Lines = new List<Line>();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool IsSafeDisposing)
        {
            if (IsSafeDisposing && (!this.IsDisposed) && (this.Lines.Count > 0))
            {
                foreach (Line line in this.Lines)
                {
                    if (line != null)
                    {
                        line.Delete();
                    }
                }
            }
        }
    }
}
