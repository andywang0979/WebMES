using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class InputBox_MProcess : Form
    {
        public static string Msg;

        public InputBox_MProcess()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Msg = txt1.Text;
        }

        public string GetMsg()
        {
            return Msg;
        }

        private void InputBox_Load(object sender, EventArgs e)
        {
            string SqlStr = "Select MProcess_No as '製程編號',MProcess_Name as '製程名稱',MProcess_Spec as '製程規格' from MProcess order by MProcess_No";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;
            dgvDetail2.Columns[0].Width = 80;
            dgvDetail2.Columns[1].Width = 150;
            dgvDetail2.Columns[2].Width = 200;
        }

        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["製程編號"].Value.ToString().Trim();
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["製程名稱"].Value.ToString().Trim();
        }

        private void dgvDetail2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["製程編號"].Value.ToString().Trim();
            Form18.msg = txt1.Text;
            this.Hide();
        }        
    }
}
