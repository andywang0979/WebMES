using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;

namespace CPCAIModule
{
    public partial class Form20 : Form
    {
        //共用變數
        string SqlStr = "";
        string msg = "";

        public Form20()
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
                if (dgvDetail2.Columns[i].ValueType == typeof(string)) //若是字串
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

        /// <summary>查詢函式</summary>
        /// <param name="where_str">查詢字串</param>      
        private void Form_Query(string where_str)
        {
            SqlStr = "Select A.Emp_Code as '員工編號',"
                + "A.Raters as '受評人員',"
                + "A.Ws_Code as '工站編號',"
                + "A.Ws_Name as '工站名稱',"
                + "A.Skill_Eva as '技能評比',"
                + "A.Ass_Code as '評核員編號',"
                + "A.Assessor as '評核員',"
                + "convert(varchar,A.Ass_Date,111) as '評核日期',"
                + "A.SNo as '建檔者編號',"
                + "convert(varchar, A.SDate, 120) as '建檔者時間',"
                + "A.UNo as '修改者編號',"
                + "substring(convert(varchar,A.UDate,121),1,16) as '修改者時間'"
                + "from Per_Fun_Data A"
                + " where 1=1";

            //如果查詢字串是空值
            if (where_str.Trim() != "")
            {
                SqlStr = SqlStr + where_str;
            }
            SqlStr = SqlStr + "order by convert(varchar,A.Ass_Date,120) desc";
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
                where_A = "and A.Emp_Code like'%" + textBox8.Text.Trim() + "%'";
            }
            if (this.textBox1.Text.Trim() != "")
            {
                where_A = where_A + "and A.Ws_Code like'%" + textBox1.Text.Trim() + "%'";
            }
            Form_Query(where_A);
        }
        //初始化
        private void Form20_Load(object sender, EventArgs e)
        {
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 10 / 25;
            //上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 8 / 37;
            //功能鍵高，比例法，離上方往下182
            //button2
            //Class1.DropDownList_B("goods_MachineNo", "goods_Basic", comboBox1, "where goods_MachineNo<>''");
            // this.comboBox1.SelectedIndex = 0;

            string LoginAccount = Loginfm.id;
            //登入者帳號指定給txt6欄位
            this.txt6.Text = LoginAccount;

            button6_Click(sender, e);
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //如果沒有欄位資料就中斷
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //員工代號
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["員工編號"].Value.ToString().Trim();

            this.label_No.Text = this.txt1.Text;

            //受評人員
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["受評人員"].Value.ToString().Trim();
            //工站編號
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["工站編號"].Value.ToString().Trim();
            //工站名稱
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["工站名稱"].Value.ToString().Trim();
            //技能評比 //下拉選單
            //this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["技能評比"].Value.ToString().Trim();
            string kind = dgvDetail2.Rows[e.RowIndex].Cells["技能評比"].Value.ToString().Trim();
            string[,] arr_kind = { { "1", "1" }, { "2", "2" }, { "3", "3" }, { "4", "4" }, { "5", "5" } };
            Class1.cbo_choose(arr_kind, kind, comboBox1);
            //評核員編號
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["評核員編號"].Value.ToString().Trim();
            //評核員
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["評核員"].Value.ToString().Trim();
            //評核日期
            this.dateTimePicker1.Text = dgvDetail2.Rows[e.RowIndex].Cells["評核日期"].Value.ToString().Trim();
            //建檔者編號
            this.txt10.Text = dgvDetail2.Rows[e.RowIndex].Cells["建檔者編號"].Value.ToString().Trim();
            //建檔者時間
            this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["建檔者時間"].Value.ToString().Trim();
            //修改者編號
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["修改者編號"].Value.ToString().Trim();
            //修改者時間
            //this.dateTimePicker3.Text = dgvDetail2.Rows[e.RowIndex].Cells["修改者時間"].Value.ToString().Trim();
            //this.txt8.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["修改者時間"].Value).ToString("HH:mm");

            this.dateTimePicker3.Text = dgvDetail2.Rows[e.RowIndex].Cells["修改者時間"].Value.ToString().Trim();
            if (dgvDetail2.Rows[e.RowIndex].Cells["修改者時間"].Value == DBNull.Value)
                this.txt8.Text = "";
            else
                this.txt8.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["修改者時間"].Value).ToString("HH:mm");
        }



        //查詢
        private void button1_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "") //員工編號
                where_A = "and A.Emp_Code like '%" + txt1.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button5_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt4.Text = ""; this.txt2.Text = "";
            this.txt3.Text = "";
            //this.txt8.Text = ""; 
            this.txt5.Text = "";
            this.txt6.Text = "";
            //this.txt7.Text = "";
            this.comboBox1.SelectedIndex = 0;
            this.label_No.Text = "";
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『員工編號』資料...");
                return false;
            }
            if (txt4.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『受評人員』資料...");
                return false;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工站代號』資料...");
                return false;
            }
            if (txt3.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工站名稱』資料...");
                return false;
            }
            if (comboBox1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『技能評比』資料...");
                return false;
            }
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『評核員代號』資料...");
                return false;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『評核員』資料...");
                return false;
            }
            //if (txt7.Text.Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『評核日期』資料...");
            //    return false;
            //}
            return isOK;
        }
        //新增
        private void button2_Click(object sender, EventArgs e)
        {
            //檢查欄位是否有填
            if (Form_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("Per_Fun_Data", "Emp_Code='" + txt1.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『員工代號：" + txt1.Text.Trim() + "』資料，已存在...");
                return;
            }
            //Insert
            SqlStr = "Insert into Per_Fun_Data"
                + "(Emp_Code, Raters, Ws_Code, Ws_Name, Skill_Eva, Ass_Code, Assessor, Ass_Date,SNo,SDate,UNo,UDate)"
                + "values("
                + "'" + txt1.Text.Trim() + "',"
                + "'" + txt4.Text.Trim() + "',"
                + "'" + txt2.Text.Trim() + "',"
                + "'" + txt3.Text.Trim() + "',"                
                + "'" + comboBox1.SelectedItem.ToString().Trim() + "'," //技能評比
                + "'" + txt5.Text.Trim() + "',"
                + "'" + txt6.Text.Trim() + "',"
                + "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd HH:mm:ss") + "',"
                + "'" + txt10.Text.Trim() + "',"
                + "'" + txt7.Text.Trim() + "',"
                + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"
                + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
                + ")";
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
                MessageBox.Show("請先輸入欲刪除之『員工代號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『員工代號』為:" + this.txt1.Text.Trim() + "的資料嗎?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Per_Fun_Data "
                    + "where Emp_Code='" + this.txt1.Text.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成，請重新查詢...");
                button5_Click(sender, e);//清除
                button1_Click(sender, e);//查詢 
            }
        }
        //修改
        private void button3_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //update
            SqlStr = "update Per_Fun_Data set "
            + "Emp_Code = '" + txt1.Text.Trim() + "',"
            + "Raters = '" + txt4.Text.Trim() + "',"
            + "Ws_Code = '" + txt2.Text.Trim() + "',"
            + "Ws_Name = '" + txt3.Text.Trim() + "',"
                //+ "Skill_Eva = '" + txt8.Text.Trim() + "',"
            + "Ass_Code = '" + txt5.Text.Trim() + "',"
            + "Assessor = '" + txt6.Text.Trim() + "',"
            + "Ass_Date = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'"
            + "where Emp_Code = '" + this.label_No.Text + "'";

            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            //button5_Click(sender, e);//清除
            button1_Click(sender, e);//查詢
        }
        //結束
        private void Form20_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }
        //員工編號: 查
        private void button7_Click(object sender, EventArgs e)
        {
            InputBox_Emp input = new InputBox_Emp();
            DialogResult dr = input.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.txt4.Text = input.GetMsg();
            }
            else
            {
                this.txt4.Text = msg.Trim();
            }
        }

        //private void txt8_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == 13)
        //        txt4.Focus();
        //}

    }
}