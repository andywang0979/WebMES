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
    public partial class Form21 : Form
    {
        //共用變數
        string SqlStr = "";

        public Form21()
        {
            InitializeComponent();
        }
        //定義每個欄位寬度
        private void Columns_for(int column_num, int[] column_width)
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
            SqlStr = "SELECT A.Ws_Code as '工站代號',"
                + "A.Ws_Name as '工站名稱',"
                + "A.Dis_Staff_Code as '派工員代號',"
                + "A.Staff_Name as '派工員名稱',"
                + "A.Fun_need_level as '職能需求等級',"
                + "convert(varchar,A.Time,120) as '時間',"
                + "A.Emp_Code as '員工代號',"
                + "A.Fun_Qua_Emp_Name as '職能資格員工姓名',"
                + "A.Send_Workers_to_Check as '派工勾選'"
                + "FROM Per_Ass_Inf A "
                + "where 1=1 ";

            //如果查詢字串是空值
            if (where_str.Trim() != "")
            {
                SqlStr = SqlStr + where_str;
            }
            SqlStr = SqlStr + "order by A.Time desc";
            DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 7;//總共0-7共8個欄位
            int[] column_width = { 120, 250, 80, 250, 80, 100, 80, 80 };//欄寬值

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
        private void button6_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.textBox8.Text.Trim() != "")
            {
                where_A = "and A.Ws_Code like'%" + textBox8.Text.Trim() + "%'";
            }
            Form_Query(where_A);
        }
        //初始化
        private void Form21_Load(object sender, EventArgs e)
        {
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 12 / 25;
            //上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 10 / 37;
            //功能鍵高，比例法，離上方往下182
            //button2
            //Class1.DropDownList_B("goods_MachineNo", "goods_Basic", comboBox1, "where goods_MachineNo<>''");
            // this.comboBox1.SelectedIndex = 0;
            button6_Click(sender, e);
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //如果沒有欄位資料就中斷
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //工站代號
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["工站代號"].Value.ToString().Trim();

            this.label_No.Text = this.txt1.Text;

            //工站名稱
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["工站名稱"].Value.ToString().Trim();

            //派工員代號
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["派工員代號"].Value.ToString().Trim();
            //派工員名稱
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["派工員名稱"].Value.ToString().Trim();
            //職能需求等級
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["職能需求等級"].Value.ToString().Trim();
            //時間
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["時間"].Value.ToString().Trim();
            //員工代號
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["員工代號"].Value.ToString().Trim();
            //職能資格員工姓名
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["職能資格員工姓名"].Value.ToString().Trim();
            //派工勾選
            //this.ckb1.Text = dgvDetail2.Rows[e.RowIndex].Cells["派工勾選"].Value.ToString().Trim();
            this.ckb1.Checked = (dgvDetail2.Rows[e.RowIndex].Cells["派工勾選"].Value.ToString().Trim() == "Y") ? true : false;

        }
        //查詢
        private void button1_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "")//員工代號
                where_A = "and A.Ws_Code like '%" + txt1.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button5_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt4.Text = ""; this.txt2.Text = "";
            this.txt3.Text = ""; this.txt5.Text = ""; this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = ""; this.ckb1.Text = "";
            this.label_No.Text = "";

        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工站代號』資料...");
                return false;
            }
            if (txt4.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工站名稱』資料...");
                return false;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『派工員代號』資料...");
                return false;
            }
            if (txt3.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『派工員名稱』資料...");
                return false;
            }
            return isOK;
        }
        //新增
        private void button2_Click(object sender, EventArgs e)
        {
            //檢查欄位是否有填
            if (Form_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("Per_Ass_Inf", "Ws_Code='" + txt1.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『工站代號：" + txt1.Text.Trim() + "』資料，已存在..."); 
                return;
            }

            string isYorN = (this.ckb1.Checked == true) ? "Y" : "N";//派工勾選

            //Insert
            SqlStr = "INSERT INTO Per_Ass_Inf" + " (Ws_Code, Ws_Name, Dis_Staff_Code, Staff_Name, Fun_need_level, Time, Emp_Code, Fun_Qua_Emp_Name,Send_Workers_to_Check) " 
                + "VALUES ("
                + "'" + txt1.Text.Trim() + "',"
                + "'" + txt4.Text.Trim() + "',"
                + "'" + txt2.Text.Trim() + "',"
                + "'" + txt3.Text.Trim() + "',"
                + "'" + txt5.Text.Trim() + "',"
                + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"
                + "'" + txt7.Text.Trim() + "',"
                + "'" + txt8.Text.Trim() + "',"
                + "'" + isYorN + "'"
                +")";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("新增資料完成...");
            button5_Click(sender, e);//清除
            button1_Click(sender, e);//查詢
        }
        //刪除
        private void button4_Click(object sender, EventArgs e)
        {
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『工站代號』...");
                return;     
            }
            if (MessageBox.Show(this, "確定要刪除『工站代號』為:" + this.txt1.Text.Trim() + "的資料嗎?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Per_Ass_Inf "
                    + "where Ws_Code='" + this.txt1.Text.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成,請重新查詢");
                button5_Click(sender, e);//清除
                button1_Click(sender, e);//查詢
            }
        }
        //修改
        private void button3_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //派工勾選
            string isYorN = (this.ckb1.Checked == true)?"Y":"N";

            //update
            SqlStr = "update Per_Ass_Inf set "
            + "Ws_Code = '" + txt1.Text.Trim() + "',"
            + "Ws_Name = '" + txt4.Text.Trim() + "',"
            + "Dis_Staff_Code = '" + txt2.Text.Trim() + "',"
            + "Staff_Name = '" + txt3.Text.Trim() + "',"
            + "Fun_need_level = '" + txt5.Text.Trim() + "',"        
            + "Time = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"
            + "Emp_Code = '" + txt7.Text.Trim() + "',"
            + "Fun_Qua_Emp_Name = '" + txt8.Text.Trim() + "',"
            //+ "Send_Workers_to_Check = '" + txt3.Text.Trim() + "'"
            + "Send_Workers_to_Check = '" + isYorN.Trim() + "'"
            + "where Ws_Code = '" + this.label_No.Text + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button1_Click(sender, e);//查詢
        }
        //結束
        private void Form21_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }
    }
}
