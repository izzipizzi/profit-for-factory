using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dolotniy_postavka
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Form1 main = this.Owner as Form1;
            if (main != null)
            {
                string pass = textBox1.Text;
                if (pass == "1234")
                {
                    main.controll_pass = true;


                }
                else
                {
                    main.controll_pass = false;

                }
                this.Close();
            }

        }
    }
}
