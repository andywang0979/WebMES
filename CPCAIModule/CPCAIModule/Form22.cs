using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class Form22 : Form
    {
        //共用變數
        string SqlStr = "";

        public Form22()
        {
            InitializeComponent();
        }
        //定義每個欄位的寬度
        private void Columns_for(int column_num,int[] column_width)
        {
            //column_num:總共幾個欄位
            //C# 數字陣列 column_width
            for (int i = 0; i < column_num; i++)
			{
                if (dgvDetail2.Columns[i].ValueType == typeof(string))//若是字串
	            {
		            dgvDetail2.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
	            }
                else
                {
                    dgvDetail2.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                //定義每個欄位寬度
                dgvDetail2.Columns[i].Width = column_width[i];
			}
        }
        //查詢函式
        private void Form_Query(string where_str) 
        {
            SqlStr = "SELECT A.Tic_Number as '工單號碼',"
                + "A.Ws_Code as '工站編號',"
                + "A.Ws_Name as '工站名稱',"
                + "A.Emp_Code as '員工編號',"
                + "A.Emp_Name as '員工名稱',"
                + "A.State as '狀態',"
                + "A.SNo as '建檔者編號',"
                + "A.SDate as '建檔者時間',"
                + "A.Time as '時間',"
                + "A.Time as '時間'"
                + "FROM Staff_Time_Data_Work A "
                + "WHERE 1 = 1";

            //如果查詢字串是空值
            if (where_str.Trim() != "")
            {
                SqlStr = SqlStr + where_str;                
            }
            SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + "order by Time asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 9;//總共0-8共9個欄位
            int[] column_width = { 110, 80, 40, 200, 100, 300, 70, 80, 80};//欄寬值
            Columns_for(column_num, column_width);

            if (dt.Rows.Count >= 1)
            {
                this.label10.Text = "符合查詢共" + dt.Rows.Count.ToString() + "筆";
            }
            else 
            {
                this.label10.Text = "符合查詢共0筆";
                MessageBox.Show("查無相關資料...");
            }
        }
        //速查
        private void button2_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.comboBox1.SelectedIndex > 0)
            {
                where_A = "and A.goods_No = '" +
                    this.comboBox1.SelectedIndex.ToString().Trim() + "'";
            }

            if (this.textBox1.Text.Trim() != "") 
            {
                where_A = where_A + "and A.goods_Name like '%" + textBox1.Text.Trim() + "%'";
            }

            Form_Query(where_A);
        }
        //初始化
        private void Form22_Load(object sender, EventArgs e)
        {
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 13 / 25;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 15 / 36;//功能鍵高，比例法，離上方往下182
            //下拉選項
            //Class1.DropDownList_A("")

        }
        



    }
}
