using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graph.Models
{
    public class WindowObject
    {
        public Pen m_Pen = null;
        public void Delete()
        {
            if (m_Pen != null)
            {
                m_Pen.Dispose();
            }
        }
    }
}
