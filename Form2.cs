using Graph.Forms;
using System;
using System.Windows.Forms;

namespace Graph
{
    public partial class Form2 : Form
    {
        private Form1 form1;
        private Form3 form3;
        public Form2()
        {
            InitializeComponent();
            form1 = new Form1(this) { Visible = false };
            form3 = new Form3(this) { Visible = false };
        }

        public void closeForm1()
        {
            form1 = null;
        }

        public void closeForm3()
        {
            form3 = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (form1 == null)
            {
                form1 = new Form1(this) { Visible = false };
            }
            form1.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (form3 == null)
            {
                form3 = new Form3(this) { Visible = false };
            }
            form3.Visible = true;
        }
    }
}
