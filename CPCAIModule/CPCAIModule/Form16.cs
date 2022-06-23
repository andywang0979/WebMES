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
    public partial class Form16 : Form
    {
        string SqlStr = "";
        //共用變數
        public Form16()
        {
            InitializeComponent();
        }
        //定義每個欄位寬度
        private void Columns_for(int column_num, int[] column_width)
        {
            //column_num : 總共欄位幾個
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
                dgvDetail2.Columns[i].Width = column_width[i];//定義每個欄位寬度
            }
        }
        //查詢函式
        private void Form_Query(string where_str)
        {
            //PARSENAME('$'+ Convert(varchar,Convert(money,A.Pay),1),2) as '產品單價'//轉貨幣符號
            //+ "REPLACE(Convert(varchar,A.Price),'.00','') as '產品單價',"//取代
            SqlStr = "Select A.MProcess_No as '製程編號',"
            + "A.MProcess_Name as '製程名稱',"
            + "A.MProcess_Spec as '製程說明',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from MProcess A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by convert(varchar,A.SDate,120) desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 5;//總共0-13共14個欄位
            int[] column_width = { 200, 200, 200, 80, 160 };//欄寬值
            Columns_for(column_num, column_width);
            
            if (dt.Rows.Count >= 1)
                this.label10.Text = "符合查詢共 " + dt.Rows.Count.ToString() + " 筆";
            else
            {
                this.label10.Text = "符合查詢共 0 筆";
                MessageBox.Show("查無相關資料...");
            }
        }
        //速查
        private void button2_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.comboBox1.SelectedIndex > 0)
                where_A = " and A.MProcess_No ='" + this.comboBox1.SelectedValue.ToString().Trim() + "'";

            if (this.textBox1.Text.Trim() != "")
                where_A = where_A + " and A.MProcess_No like '%" + textBox1.Text.Trim() + "%' or A.MProcess_Name like '%" + textBox1.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 8 / 25;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 8 / 37;//功能鍵高，比例法，離上方往下182
            //下拉選項
            Class1.DropDownList_B("MProcess_No", "MProcess", comboBox1, "where MProcess_Name<>''");
            if (comboBox1.SelectedIndex > -1)//當DB沒半比時不執行下拉選項
                this.comboBox1.SelectedIndex = 0;
            button2_Click(sender, e);
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //物料代碼
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["製程編號"].Value.ToString().Trim();
            this.label_No.Text = this.txt1.Text;
            //物料名稱
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["製程名稱"].Value.ToString().Trim();
            //產品規格
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["製程說明"].Value.ToString().Trim();
            //異動人員
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動人員"].Value.ToString().Trim();
            //異動時間
            this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動時間"].Value.ToString().Trim();
            /*
            //發料方式//下拉選項
            string kind1 = dgvDetail2.Rows[e.RowIndex].Cells["大分類"].Value.ToString().Trim();
            string[,] arr_kind1 = { { "0", "" }, { "1", "原料" }, { "2", "物料" }, { "3", "耗材" } };
            Class1.cbo_choose(arr_kind1, kind1, CB1);
            
            string type = dgvDetail2.Rows[e.RowIndex].Cells["發料方式"].Value.ToString().Trim();
            string[,] arr_Out = { { "0", "" }, { "1", "一次發料" }, { "2", "分批發料" }, { "3", "現場領料" } };
            Class1.cbo_choose(arr_Out, type, CB2);

            string kind2 = dgvDetail2.Rows[e.RowIndex].Cells["中分類"].Value.ToString().Trim();
            string[,] arr_kind2 = { { "0", "" }, { "1", "油品" }, { "2", "零件" }, { "3", "棒材" }, { "4", "線材" } };
            Class1.cbo_choose(arr_kind2, kind2, CB3);
            
            type = dgvDetail2.Rows[e.RowIndex].Cells["檢驗方式"].Value.ToString().Trim();
            string[,] arr_QC = { { "0", "" }, { "1", "免驗" }, { "2", "抽驗" }, { "3", "全檢" } };
            Class1.cbo_choose(arr_QC, type, CB4);
            */
        }
        //查詢
        private void button3_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "")//料號
                where_A = " and A.MProcess_No like '%" + txt1.Text.Trim() + "%'";

            if (this.txt5.Text.Trim() != "")//名稱
                where_A = " and A.MProcess_Name like '%" + txt5.Text.Trim() + "%'";

            if (this.txt6.Text.Trim() != "")//規格
                where_A = where_A + " and A.MProcess_Spec like '%" + txt6.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button7_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt2.Text = ""; this.txt3.Text = ""; this.txt4.Text = "";
            this.txt5.Text = ""; this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = "";
            this.txt9.Text = ""; this.txt10.Text = "";
            if (comboBox1.SelectedIndex>-1)//當DB沒半比時不執行下拉選項
                this.comboBox1.SelectedIndex = 0; 
            this.textBox1.Text = "";
            this.CB1.SelectedIndex = 0; this.CB2.SelectedIndex = 0; this.CB3.SelectedIndex = 0; this.CB4.SelectedIndex = 0;
            this.label_No.Text = "";
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製程編號』資料...");
                return false;
            }            
            
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製程名稱』資料...");
                return false;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製程說明』資料...");
                return false;
            }
            return isOK;
        }
        //新增
        private void button4_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("MProcess", "MProcess_No='" + txt1.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『製程編號：" + txt1.Text.Trim() + "』資料，已存在...");
                return;
            }
            //Insert
            SqlStr = "Insert into [MProcess]"
            + " (MProcess_No, MProcess_Name, MProcess_Spec, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt5.Text.Trim() + "',"
            + "'" + txt6.Text.Trim() + "',"
            //+ "'" + CB1.SelectedItem.ToString().Trim() + "',"//製品分類
            //+ "'" + CB3.SelectedItem.ToString().Trim() + "',"//發料方式
            //+ "'" + CB2.SelectedItem.ToString().Substring(0, 1) + "',"//發料方式
            //+ "'" + CB4.SelectedItem.ToString().Substring(0, 1) + "',"//檢驗方式
            + "'" + Loginfm.id.Trim() + "',"
            + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + ")";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("新增資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //刪除
        private void button6_Click(object sender, EventArgs e)
        {
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『製程編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『製程編號』為:" + this.txt1.Text.Trim() + "的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from [MProcess]"
                + " where MProcess_No='" + this.txt1.Text.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成，請重新查詢...");
                button7_Click(sender, e);//清除
                button3_Click(sender, e);//查詢
            }
        }
        //修改
        private void button5_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //update
            SqlStr = "update [MProcess] set "
            + "MProcess_No = '" + txt1.Text.Trim() + "',"
            + "MProcess_Name = '" + txt5.Text.Trim() + "',"
            + "MProcess_Spec = '" + txt6.Text.Trim() + "',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where MProcess_No = '" + this.label_No.Text + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.txt3.Text = "---";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.txt2.Text = "---";
        }
    }
}
