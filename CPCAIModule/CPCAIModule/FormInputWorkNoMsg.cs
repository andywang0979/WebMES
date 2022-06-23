using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class InputBox2 : Form
    {
        public static string Msg = "";

        public InputBox2()
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

        private void InputBox2_Load(object sender, EventArgs e)
        {
            string SqlStr = "Select Work_No as '工單編號',goods_No as '物料編號',goods_name as '物料名稱'"
            + " from ProductWork where goods_Name <> ''"
            + " order by Work_No";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;
            dgvDetail2.Columns[0].Width = 80;
            dgvDetail2.Columns[1].Width = 120;
            dgvDetail2.Columns[2].Width = 160;
        }

        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["工單編號"].Value.ToString().Trim();
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["物料編號"].Value.ToString().Trim();
        }

        private void dgvDetail2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["工單編號"].Value.ToString().Trim();
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["物料編號"].Value.ToString().Trim();
            Form14.msg = txt1.Text;
            this.Hide();
        }        
    }
}
