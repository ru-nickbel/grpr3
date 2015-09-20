using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace grpr3
{
    public partial class FormZ : Form
    {
        FormMain mainForm;

        public int z;
        public FormZ()
        {
            InitializeComponent();
        }

        public FormZ(FormMain mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();

        }   

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                radButton1_Click(sender, e);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.SelectAll();
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            z = int.Parse(textBox1.Text);
            DialogResult = DialogResult.OK;
            this.Close();
        }


    }
}
