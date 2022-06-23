using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class Loginfm : Form
    {
        public static string id = "";
        public static string connstring = "";

        public Loginfm()
        {
            InitializeComponent();
            connstring = this.txtDB_NB3A120.Text.Trim();//DB=>.\SQLEXPRE
        }

        private void button3_Click(object sender, EventArgs e)
        {
            id = this.txt1.Text.Trim();
            string pwd = this.txt2.Text.Trim();
            string SqlStr = "Update Employee set Pwd='" + pwd + "' where EmpNo='" + id + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("『新密碼』建立成功，請重新登入!!");
            this.txt1.Text = "";
            this.txt2.Text = "";
            button3.Visible = false;
        }

        private void Loginfm_Load(object sender, EventArgs e)
        {
            string computername = Environment.GetEnvironmentVariable("COMPUTERNAME");
            if (computername.Trim() == "NB-3A120")
            {
                this.comboBox1.SelectedIndex = 0;
                txt1.Text = "02465";//本機
                txt2.Text = "12345";
                this.button1.Focus();
                return;
            }
            else
            {
                this.comboBox1.SelectedIndex = 1;//本機
                txt1.Text = "02465";
                txt2.Text = "12345";
            }
            this.txt1.Focus();
            //this.ImeMode = ImeMode.Disable;
        }

        private void txt1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                txt2.Focus();
        }

        private void txt2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                this.button1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             id = this.txt1.Text.Trim();//帳號
            string pwd = this.txt2.Text.Trim();//密碼
            if (this.comboBox1.SelectedIndex == 0)
                connstring = this.txtDB_NB3A120.Text.Trim();//DB=NB-3A120\SQLEXPRESS
            else if (this.comboBox1.SelectedIndex == 2)
                connstring = this.txtDB_Barry.Text.Trim();//DB=Barry\SQLEXPRESS
            else
                connstring = this.txtDB_Local.Text.Trim();//本機//或其他主機

            if (id == "") { MessageBox.Show("請先輸入帳號..."); return; };
            if (pwd == "") { MessageBox.Show("請先輸入密碼..."); return; };

            if (id == pwd)
            {
                MessageBox.Show("『注意』您的帳號及密碼相同，請建『新密碼』後，按設定，重新登入!!");
                this.txt2.Text = "";
                button3.Visible = true;
                return;
            }

            string SqlStr = "Select * from Employee where EmpNo='" + id + "' and Pwd='" + pwd + "'";
            if (Class1.GetDataTable(SqlStr).Rows.Count >= 1)
            {
                this.Hide();
                FmMenu fm = new FmMenu();
                fm.Show();
            }
            else
            {
                MessageBox.Show("『帳號』或『密碼』錯誤，請重新登入!!");
            }
        }
    }
}
