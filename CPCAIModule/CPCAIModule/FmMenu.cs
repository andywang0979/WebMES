using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class FmMenu : Form
    {
        public FmMenu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 fm = new Form3();
            fm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void FmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form5 fm = new Form5();
            fm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form7 fm = new Form7();
            fm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 fm = new Form4();
            fm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form6 fm = new Form6();
            fm.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form8 fm = new Form8();
            fm.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form9 fm = new Form9();
            fm.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 fm = new Form1();
            fm.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 fm = new Form2();
            fm.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form10 fm = new Form10();
            fm.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form11 fm = new Form11();
            fm.Show();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.Hide();
            FmBase1 fm = new FmBase1();
            fm.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form12 fm = new Form12();
            fm.Show();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form13 fm = new Form13();
            fm.Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form14 fm = new Form14();
            fm.Show();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form15 fm = new Form15();
            fm.Show();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form16 fm = new Form16();
            fm.Show();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form17 fm = new Form17();
            fm.Show();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form18 fm = new Form18();
            fm.Show();
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form19 fm = new Form19();
            fm.Show();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form20 fm = new Form20();
            fm.Show();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form21 fm = new Form21();
            fm.Show();

        }
    }
}
