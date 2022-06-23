using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class InputBox_Emp : Form
    {
        public static string Msg;

        public InputBox_Emp()
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
            string SqlStr = "Select EmpNo as '員工編號',Name as '員工姓名',Department as '隸屬部門' from Employee order by EmpNo";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;
            dgvDetail2.Columns[0].Width = 80;
            dgvDetail2.Columns[2].Width = 80;
        }

        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["員工編號"].Value.ToString().Trim();
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["員工姓名"].Value.ToString().Trim();
        }

        private void dgvDetail2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["員工編號"].Value.ToString().Trim();
            Form13.msg = txt1.Text;
            this.Hide();
        }        
    }
}
