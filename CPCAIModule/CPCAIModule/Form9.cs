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
    public partial class Form9 : Form
    {
        string SqlStr = "";
        //共用變數
        public Form9()
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
            SqlStr = "Select A.I_No as '治具編號',"
            + "B.I_Name as '治具名稱',"
            + "B.Brand as '品牌',"
            + "B.I_Spec as '規格',"
            + "A.Line_No as '線別',"
            + "A.Area as '作業區域',"
            + "A.Need_Qty as '使用數量',"
            + "A.Now_Qty as '現有數量',"
            + "A.Lack_Qty as '欠料數量',"
            + "case when Convert(varchar,A.Expect_Time,111)='1900/01/01'"
            + " then '---' else substring(convert(varchar,A.Expect_Time,121),1,16) end as '預計備齊時間',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from Instrument_Used A left join Instrument B on A.I_No = B.I_No"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by A.I_No Asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 12;//總共0-13共14個欄位
            int[] column_width = { 120, 200, 120, 200, 80, 80, 80, 80, 80, 160, 80,160 };//欄寬值
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
                where_A = " and A.I_No ='" + this.comboBox1.SelectedValue.ToString().Trim() + "'";

            if (this.textBox1.Text.Trim() != "")
                where_A = where_A + " and B.I_Name like '%" + textBox1.Text.Trim() + "%' or B.I_Spec='" + textBox1.Text.Trim() + "'";

            Form_Query(where_A);
        }
        //初始化
        private void Form9_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 8 / 25;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 8 / 37;//功能鍵高，比例法，離上方往下182
            //下拉選項
            Class1.DropDownList_B("I_No", "Instrument", comboBox1, "where I_Name<>''");
            this.comboBox1.SelectedIndex = 0;           
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //物料代碼
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["治具編號"].Value.ToString().Trim();
            this.label_No.Text = this.txt1.Text;
            //物料名稱
            //this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["治具名稱"].Value.ToString().Trim();
            //產品階層
            //this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["品牌"].Value.ToString().Trim();
            //產品規格
            //this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["規格"].Value.ToString().Trim();
            //使用數量
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["線別"].Value.ToString().Trim();
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["作業區域"].Value.ToString().Trim();
            //使用數量
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["使用數量"].Value.ToString().Trim();
            this.txt10.Text = dgvDetail2.Rows[e.RowIndex].Cells["現有數量"].Value.ToString().Trim();
            this.txt11.Text = dgvDetail2.Rows[e.RowIndex].Cells["欠料數量"].Value.ToString().Trim();
            //日期時間
            if (dgvDetail2.Rows[e.RowIndex].Cells["預計備齊時間"].Value.ToString().Trim() == "---")
            {
                this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
            }
            else
            {
                this.dateTimePicker1.Enabled = true; this.txt14.Enabled = true;
                this.dateTimePicker1.Text = dgvDetail2.Rows[e.RowIndex].Cells["預計備齊時間"].Value.ToString().Trim();
                if (dgvDetail2.Rows[e.RowIndex].Cells["預計備齊時間"].Value == DBNull.Value)
                    this.txt14.Text = "";
                else
                    this.txt14.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["預計備齊時間"].Value).ToString("HH:mm");
            }
            //異動人員
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動人員"].Value.ToString().Trim();
            //異動時間
            this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動時間"].Value.ToString().Trim();
            //發料方式//下拉選項
            //string type = dgvDetail2.Rows[e.RowIndex].Cells["發料方式"].Value.ToString().Trim();
            //if (type.Trim() != "")
            //{
            //    this.CB1.SelectedIndex = Convert.ToInt16(type.Trim());
            //}
        }
        //查詢
        private void button3_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "")//料號
                where_A = " and A.I_No like '%" + txt1.Text.Trim() + "%'";

            if (this.txt5.Text.Trim() != "")//名稱
                where_A = " and B.I_Name like '%" + txt5.Text.Trim() + "%'";

            if (this.txt6.Text.Trim() != "")//規格
                where_A = where_A + " and B.I_Spec like '%" + txt6.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button7_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt2.Text = ""; this.txt3.Text = ""; this.txt4.Text = "";
            this.txt5.Text = ""; this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = "";
            this.txt9.Text = ""; this.txt10.Text = ""; this.txt11.Text = ""; this.txt14.Text = "00:00";
            this.comboBox1.SelectedIndex = 0; this.textBox1.Text = ""; //this.txt14.BackColor = Color.Yellow;
            this.CB1.SelectedIndex = 0; this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
            this.label_No.Text = "";
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『治具編號』資料...");
                return false;
            }            
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『品牌』資料...");
                return false;
            }
            //if (txt3.Text.Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『精度』資料...");
            //    return false;
            //}
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『治具名稱』資料...");
                return false;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『治具規格』資料...");
                return false;
            }
            //if (CB1.SelectedItem.ToString().Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『發料方式』資料...");
            //    return false;
            //}
            return isOK;
        }
        //新增
        private void button4_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("Instrument_Used", "I_No='" + txt1.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『治具編號：" + txt1.Text.Trim() + "』資料，已存在...");
                return;
            }
            string strline = (txt3.Text.Trim() == "") ? "null" : txt3.Text.Trim();
            string strused = (txt8.Text.Trim() == "") ? "null" : txt8.Text.Trim();
            string strnow = (txt10.Text.Trim() == "") ? "null" : txt10.Text.Trim();
            //Insert
            SqlStr = "Insert into [Instrument_Used]"
            + " (I_No, Line_No, Area, Need_Qty, Now_Qty, Lack_Qty, Expect_Time, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + strline.Trim() + "',"
            + "'" + txt4.Text.Trim() + "',"
            + "'" + strused.Trim() + "',"//使用數量
            + "'" + strnow.Trim() + "',"//現有數量
            + "'" + txt11.Text.Trim() + "',"//欠料數量           
            + "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"//Expect_Arrive
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
                MessageBox.Show("請先輸入欲刪除之『物料編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『治具編號』為:" + this.txt1.Text.Trim() + "的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Instrument_Used"
                + " where I_No='" + this.txt1.Text.Trim() + "'";
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
            string strPrec = (txt3.Text.Trim() == "") ? "null" : txt3.Text.Trim();//精度
            //update
            SqlStr = "update [Instrument_Used] set "
            + "I_No = '" + txt1.Text.Trim() + "',"
            //+ "I_Name = '" + txt5.Text.Trim() + "',"
            //+ "Brand = '" + txt2.Text.Trim() + "',"
            //+ "I_Spec = '" + txt6.Text.Trim() + "',"
            + "Line_No = '" + txt3.Text.Trim() + "',"
            + "Area = '" + txt4.Text.Trim() + "',"
            + "Need_Qty = " + txt8.Text.Trim() + ","
            + "Now_Qty = " + txt10.Text.Trim() + ","
            + "Lack_Qty = " + txt11.Text.Trim() + ","
            + "Expect_Time = '" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"
            + "ENo = '" + Loginfm.id.Trim() + "',"
            + "SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where I_No = '" + this.label_No.Text + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form9_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                button2.Focus();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                button2.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InputBox input = new InputBox();
            DialogResult dr = input.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string dataA = input.GetMsg();
                this.txt1.Text = dataA.Trim();
            }
        }

        private void txt1_TextChanged(object sender, EventArgs e)
        {
            string dataA = this.txt1.Text.Trim();
            this.txt5.Text = Class1.GetValue("I_Name", "Instrument", "I_No='" + dataA + "'");
            this.txt2.Text = Class1.GetValue("Brand", "Instrument", "I_No='" + dataA + "'");
            this.txt6.Text = Class1.GetValue("I_Spec", "Instrument", "I_No='" + dataA + "'");
        }

        private void txt11_Enter(object sender, EventArgs e)
        {
            if (txt8.Text.Trim() != "" && txt10.Text.Trim() != "")
            {
                this.txt11.Text = (Convert.ToInt16(txt8.Text) - Convert.ToInt16(txt10.Text)).ToString();
                if (Convert.ToInt32(txt11.Text) > 0)
                {
                    this.txt14.Text = "12:00";
                    this.dateTimePicker1.Enabled = true; this.txt14.Enabled = true;
                    this.txt14.BackColor = Color.Yellow;
                }
                else
                {
                    this.txt14.Text = "00:00";
                    this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01");
                    this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                    this.txt14.BackColor = Color.White;
                }
            }
        }
    }
}
