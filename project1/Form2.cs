using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project1
{
    public partial class Form2 : Form
    {
        string str1, str2;
        int i = 1;

        public Form2()
        {
            InitializeComponent();
            textBox1.Focus();
        }

        //Ввод графа с клавиатуры
        private void button1_Click(object sender, EventArgs e)
        {
            str1 = textBox1.Text;
            str2 = textBox2.Text;
            if (str1 == "")
            {
                textBox1.BackColor = System.Drawing.Color.FromArgb(232, 86, 83);
                textBox2.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                textBox1.Focus();
            }
            else if (str2 == "")
            {
                textBox2.BackColor = System.Drawing.Color.FromArgb(232, 86, 83);
                textBox1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
                textBox2.Focus();
            }
            else
            {
                if (radioButton1.Checked == true)
                    i = 1;
                else
                    i = 2;
                this.DialogResult = DialogResult.OK;
            }
        }

        public string ReturnData1()
        {
            return (str1);
        }

        public string ReturnData2()
        {
            return (str2);
        }

        public int ReturnData3()
        {
            return (i);
        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            Func(e, textBox1);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Func(e, textBox2);
        }

        private void Func(KeyPressEventArgs e, TextBox textBox)
        {
            textBox.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != ' ') && (e.KeyChar != '\b') && e.KeyChar != 13)
            {
                MessageBox.Show("Invalid input format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.KeyChar = '\0';
                textBox.Focus();
            }
        }
    }
}
