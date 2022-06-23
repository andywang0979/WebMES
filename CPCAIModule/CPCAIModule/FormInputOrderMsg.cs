using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class InputBox1 : Form
    {
        public static string Msg;

        public InputBox1()
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

        private void InputBox1_Load(object sender, EventArgs e)
        {
            string SqlStr = "Select Order_No as '訂單編號',goods_Name as '物料名稱',goods_Spec as '規格',Order_num as '需求量'"
            + " from Cus_Order where goods_Name <> ''"
            + " order by goods_No";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;
            dgvDetail2.Columns[0].Width = 80;
            dgvDetail2.Columns[2].Width = 160;
            dgvDetail2.Columns[3].Width = 80;
        }

        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["訂單編號"].Value.ToString().Trim();
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["物料名稱"].Value.ToString().Trim();
        }

        private void dgvDetail2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["訂單編號"].Value.ToString().Trim();
            Form14.msg = txt1.Text;
            this.Hide();
        }        
    }
}
