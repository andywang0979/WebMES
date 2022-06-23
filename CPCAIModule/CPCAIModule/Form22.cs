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
                + "A.Ws_Code as '工站代號',"
                + "A.Ws_Name as '工站名稱',"
                + "A.Emp_Code as '員工代號',"
                + "A.Emp_Name as '員工名稱',"
                + "A.State as '狀態',"
                + "A.Time as '時間'"
                + "FROM Staff_Time_Data_Work A "
                + "WHERE 1 = 1";

            //如果查詢字串是空值
            if (where_str.Trim() != "")
            {
                SqlStr = SqlStr + where_str;                
            }

        }



    }
}
