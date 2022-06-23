using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class InputBox : Form
    {
        public static string Msg;

        public InputBox()
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
            string SqlStr = "Select goods_No as '物料編號',goods_Name as '物料名稱',goods_Level as '物料階層',goods_Spec as '物料規格'"
            + " from goods_Basic where goods_Level='1'"
            + " order by goods_No";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;
            dgvDetail2.Columns[0].Width = 80;
            dgvDetail2.Columns[2].Width = 80;
            dgvDetail2.Columns[3].Width = 160;
        }

        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["物料編號"].Value.ToString().Trim();
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["物料名稱"].Value.ToString().Trim();
        }

        private void dgvDetail2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["物料編號"].Value.ToString().Trim();
            Form14.msg = txt1.Text;
            this.Hide();
        }        
    }
}
