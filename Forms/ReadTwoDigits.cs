using System;
using System.Windows.Forms;

namespace Graph.Forms
{
    public partial class ReadTwoDigits : Form
    {
        private Form4 form4 = null;
        public ReadTwoDigits(Form4 form4)
        {
            InitializeComponent();
            this.form4 = form4;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (double.TryParse(textBox1.Text.ToString(), out double x) && double.TryParse(textBox2.Text.ToString(), out double y) && !(x <= 0 || y <= 0))
            {
                form4.setXandY(x, y);

                this.Close();
            }
            else
            {
                DialogResult result = MessageBox.Show(
                                    "Вы не можете указать такой масштаб!",
                                    "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                textBox1.Text = "";
                textBox2.Text = "";
            }
        }

        private void ReadTwoDigits_Load(object sender, EventArgs e)
        {
            form4.closeReadTwoDigitsForm();
        }
    }
}
