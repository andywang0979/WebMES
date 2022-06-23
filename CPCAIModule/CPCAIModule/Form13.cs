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
    public partial class Form13 : Form
    {
        string SqlStr = "";
        public static string msg = "";
        //共用變數
        public Form13()
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
            SqlStr = "Select A.EmpNo as '員工編號',"
            + "B.Name as '姓名',"
            + "A.Item as '項次',"
            + "B.Department as '部門',"
            + "substring(convert(varchar,A.Time1,121),1,16) as '上線時間',"//上線時間
            + "substring(convert(varchar,A.Time2,121),1,16) as '下線時間',"//下線時間
            + "A.Time1ToTime2 as '小計(分)',"//小計(分)
            + "substring(convert(varchar,A.OverTime1,121),1,16) as '上線時間(加班)',"//上線時間
            + "substring(convert(varchar,A.OverTime2,121),1,16) as '下線時間(加班)',"//下線時間
            + "A.OverTime1ToTime2 as '小計[分]',"//小計(分)
            //+ "case when Convert(varchar,A.Expect_Time,111)='1900/01/01'"
            //+ " then '---' else substring(convert(varchar,A.Expect_Time,121),1,16) end as '預計備齊時間',"
            + "A.IsStop as '是否有除外工時',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from Employee_Work A left join Employee B on A.EmpNo = B.EmpNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by A.EmpNo,A.Item Asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 13;//總共0-13共14個欄位
            int[] column_width = { 80, 100, 60, 120, 160, 160, 80, 160, 160, 80, 120, 80,160 };//欄寬值
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
                where_A = " and A.EmpNo ='" + this.comboBox1.SelectedValue.ToString().Trim() + "'";

            if (this.textBox1.Text.Trim() != "")
                where_A = where_A + " and B.Name like '%" + textBox1.Text.Trim() + "%' or B.Department='" + textBox1.Text.Trim() + "'";

            Form_Query(where_A);
        }
        //初始化
        private void Form13_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 45 / 100;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 35 / 100;//功能鍵高，比例法，離上方往下182
            //上面中間切一刀
            this.splitContainer3.SplitterDistance = splitContainer1.Width * 51 / 100;//功能鍵高，比例法，離左方往下182
            //上面右邊切一刀
            this.splitContainer4.SplitterDistance = 680;
            //上面上邊切一刀
            this.splitContainer5.SplitterDistance = 10;
            //上面下邊切一刀
            this.splitContainer6.SplitterDistance = splitContainer1.Height * 20 / 100;
            //下拉選項
            Class1.DropDownList_B("EmpNo", "Employee", comboBox1, "where Name<>''");
            Class1.DropDownList_A("SNo", "ParaData1", "Para", CB1, "where Kind='B'");
            this.comboBox1.SelectedIndex = 0;           
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //物料代碼
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["員工編號"].Value.ToString().Trim();
            this.label_No1.Text = this.txt1.Text;
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["項次"].Value.ToString().Trim();
            this.label_No2.Text = this.txt2.Text;
            //日期時間
            if (dgvDetail2.Rows[e.RowIndex].Cells["上線時間"].Value.ToString().Trim() != "")
            {
                //this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                this.dateTimePicker1.Text = dgvDetail2.Rows[e.RowIndex].Cells["上線時間"].Value.ToString().Trim();
                //this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
                if (dgvDetail2.Rows[e.RowIndex].Cells["上線時間"].Value == DBNull.Value)
                    this.txt8.Text = "";
                else
                    this.txt8.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["上線時間"].Value).ToString("HH:mm");
            }
            if (dgvDetail2.Rows[e.RowIndex].Cells["下線時間"].Value.ToString().Trim() != "")
            {
                //this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                this.dateTimePicker2.Text = dgvDetail2.Rows[e.RowIndex].Cells["下線時間"].Value.ToString().Trim();
                //this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
                if (dgvDetail2.Rows[e.RowIndex].Cells["下線時間"].Value == DBNull.Value)
                    this.txt10.Text = "";
                else
                    this.txt10.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["下線時間"].Value).ToString("HH:mm");
            }
            this.txt11.Text = dgvDetail2.Rows[e.RowIndex].Cells["小計(分)"].Value.ToString().Trim();
            //日期時間
            if (dgvDetail2.Rows[e.RowIndex].Cells["上線時間(加班)"].Value.ToString().Trim() != "")
            {
                //this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                this.dateTimePicker5.Text = dgvDetail2.Rows[e.RowIndex].Cells["上線時間(加班)"].Value.ToString().Trim();
                //this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
                if (dgvDetail2.Rows[e.RowIndex].Cells["上線時間(加班)"].Value == DBNull.Value)
                    this.txt3.Text = "";
                else
                    this.txt3.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["上線時間(加班)"].Value).ToString("HH:mm");
            }
            if (dgvDetail2.Rows[e.RowIndex].Cells["下線時間(加班)"].Value.ToString().Trim() != "")
            {
                //this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                this.dateTimePicker6.Text = dgvDetail2.Rows[e.RowIndex].Cells["下線時間(加班)"].Value.ToString().Trim();
                //this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
                if (dgvDetail2.Rows[e.RowIndex].Cells["下線時間(加班)"].Value == DBNull.Value)
                    this.txt4.Text = "";
                else
                    this.txt4.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["下線時間(加班)"].Value).ToString("HH:mm");
            }
            this.txt15.Text = dgvDetail2.Rows[e.RowIndex].Cells["小計[分]"].Value.ToString().Trim();
            this.checkBox1.Checked = (dgvDetail2.Rows[e.RowIndex].Cells["是否有除外工時"].Value.ToString().Trim() == "Y") ? true : false;
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
            Form_Query_Detail();
        }
        //查詢
        private void button3_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "")//料號
                where_A = " and A.EmpNo like '%" + txt1.Text.Trim() + "%'";

            if (this.txt5.Text.Trim() != "")//名稱
                where_A = " and B.Name like '%" + txt5.Text.Trim() + "%'";

            if (this.txt6.Text.Trim() != "")//規格
                where_A = where_A + " and B.Department like '%" + txt6.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button7_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt2.Text = ""; this.txt3.Text = ""; this.txt4.Text = "";
            this.txt5.Text = ""; this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = "00:00";
            this.txt9.Text = ""; this.txt10.Text = "00:00"; this.txt11.Text = "";
            this.comboBox1.SelectedIndex = 0; this.textBox1.Text = ""; //this.txt14.BackColor = Color.Yellow;
            this.CB1.SelectedIndex = 0; //this.dateTimePicker1.Enabled = false; this.dateTimePicker2.Enabled = false;
            this.label_No1.Text = ""; this.label_No2.Text = "";
            this.dateTimePicker1.Value = DateTime.Now; this.dateTimePicker2.Value = DateTime.Now;
            this.dateTimePicker5.Value = DateTime.Now; this.dateTimePicker6.Value = DateTime.Now;
            this.checkBox1.Checked = false; this.txt15.Text = "";
            this.txt3.Text = "00:00"; this.txt4.Text = "00:00";
            this.txt13.Text = "00:00"; this.txt14.Text = "00:00";
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
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『項次』資料...");
                return false;
            }
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『姓名』資料...");
                return false;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『部門』資料...");
                return false;
            }
            //檢查時間是否異常
            string strtime1 = (txt8.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt8.Text.Trim() + ":00";
            string strtime2 = (txt10.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " " + txt10.Text.Trim() + ":00";
            TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            if (Convert.ToDateTime(strtime2) <= Convert.ToDateTime(strtime1))
            {
                MessageBox.Show("日期時間輸入異常，請檢查...");
                return false;
            }
            string overstrtime1 = (txt3.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker5.Text).ToString("yyyy-MM-dd") + " " + txt3.Text.Trim() + ":00";
            string overstrtime2 = (txt4.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker6.Text).ToString("yyyy-MM-dd") + " " + txt4.Text.Trim() + ":00";
            TimeSpan overts = Convert.ToDateTime(overstrtime2) - Convert.ToDateTime(overstrtime1);
            if (Convert.ToDateTime(overstrtime2) <= Convert.ToDateTime(overstrtime1))
            {
                MessageBox.Show("日期時間輸入異常，請檢查...");
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
            if (Class1.GetRowCount("Employee_Work", "EmpNo='" + txt1.Text.Trim() + "' and Item='" + txt2.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『員工編號/項次：" + txt1.Text.Trim() + "/" + txt2.Text.Trim() + "』資料，已存在...");
                return;
            }
            string strtime1 = (txt8.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt8.Text.Trim() + ":00";
            string strtime2 = (txt10.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " " + txt10.Text.Trim() + ":00";
            TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            string overstrtime1 = (txt3.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker5.Text).ToString("yyyy-MM-dd") + " " + txt3.Text.Trim() + ":00";
            string overstrtime2 = (txt4.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker6.Text).ToString("yyyy-MM-dd") + " " + txt4.Text.Trim() + ":00";
            TimeSpan overts = Convert.ToDateTime(overstrtime2) - Convert.ToDateTime(overstrtime1);
            string YN = (this.checkBox1.Checked == true) ? "Y" : "N";
            //Insert
            SqlStr = "Insert into [Employee_Work]"
            + " (EmpNo, Item, Time1, Time2, Time1ToTime2, OverTime1, OverTime2, OverTime1ToTime2, IsStop, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt2.Text.Trim() + "',"
            + "'" + strtime1.Trim() + "',"//上線時間
            + "'" + strtime2.Trim() + "',"//下線時間
            + "'" + ts.TotalMinutes.ToString() + "',"//小計(分)
            + "'" + overstrtime1.Trim() + "',"//上線時間
            + "'" + overstrtime2.Trim() + "',"//下線時間
            + "'" + overts.TotalMinutes.ToString() + "',"//小計(分)
            //+ "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"//Expect_Arrive
            + "'" + YN.Trim() + "',"
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
                MessageBox.Show("請先輸入欲刪除之『員工編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『員工編號/項次』為:" + this.txt1.Text.Trim() + "/" + this.txt2.Text.Trim() + "的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Employee_Work"
                + " where EmpNo='" + this.txt1.Text.Trim() + "' and Item='" + this.txt2.Text.Trim() + "'";
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
            string strtime1 = (txt8.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt8.Text.Trim() + ":00";
            string strtime2 = (txt10.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " " + txt10.Text.Trim() + ":00";
            TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            string overstrtime1 = (txt3.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker5.Text).ToString("yyyy-MM-dd") + " " + txt3.Text.Trim() + ":00";
            string overstrtime2 = (txt4.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker6.Text).ToString("yyyy-MM-dd") + " " + txt4.Text.Trim() + ":00";
            TimeSpan overts = Convert.ToDateTime(overstrtime2) - Convert.ToDateTime(overstrtime1);
            string YN = (this.checkBox1.Checked == true) ? "Y" : "N";
            //update
            SqlStr = "update [Employee_Work] set "
            + "EmpNo = '" + txt1.Text.Trim() + "',"
            //+ "I_Name = '" + txt5.Text.Trim() + "',"
            //+ "Brand = '" + txt2.Text.Trim() + "',"
            //+ "I_Spec = '" + txt6.Text.Trim() + "',"
            + "Item = '" + txt2.Text.Trim() + "',"
            + "Time1 = '" + strtime1 + "',"
            + "Time2 = '" + strtime2 + "',"
            + "Time1ToTime2 = '" + ts.TotalMinutes.ToString() + "',"
            + "overtsTime1 = '" + overstrtime1 + "',"
            + "overtsTime2 = '" + overstrtime2 + "',"
            + "overtsTime1ToTime2 = '" + overts.TotalMinutes.ToString() + "',"
            + "IsStop = '" + YN.Trim() + "',"
            + "ENo = '" + Loginfm.id.Trim() + "',"
            + "SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where EmpNo='" + this.label_No1.Text.Trim() + "' and Item='" + this.label_No2.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form13_FormClosed(object sender, FormClosedEventArgs e)
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
            InputBox_Emp input = new InputBox_Emp();
            DialogResult dr = input.ShowDialog();
            if (dr == DialogResult.OK)
                this.txt1.Text = input.GetMsg();
            else
                this.txt1.Text = msg.Trim();                
        }

        private void txt1_TextChanged(object sender, EventArgs e)
        {
            string dataA = this.txt1.Text.Trim();
            this.txt5.Text = Class1.GetValue("Name", "Employee", "EmpNo='" + dataA + "'");
            //this.txt2.Text = Class1.GetValue("Brand", "Instrument", "I_No='" + dataA + "'");
            this.txt6.Text = Class1.GetValue("Department", "Employee", "EmpNo='" + dataA + "'");
        }

        private void txt11_Enter(object sender, EventArgs e)
        {
            if (txt8.Text.Trim() != "" && txt10.Text.Trim() != "")
            {
                string strtime1 = (txt8.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt8.Text.Trim() + ":00";
                string strtime2 = (txt10.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " " + txt10.Text.Trim() + ":00";
                TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
                this.txt11.Text = ts.TotalMinutes.ToString();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (this.txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『員工編號』的資料，再取號...");
                return;
            }
            //取號
            string item_No = Class1.GetValue("count(*)", "Employee_Work", "EmpNo = '" + this.txt1.Text + "'");
            item_No = (Convert.ToInt16(item_No) + 1).ToString();
            this.txt2.Text = item_No.Trim();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『員工編號』資料...");
                return;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『項次』資料...");
                return;
            }
            string para1 = Class1.GetValue("ParaData1", "Para", "SNo='" + CB1.SelectedValue.ToString() + "' and Kind='B'");
            string strtime1 = (txt13.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker3.Text).ToString("yyyy-MM-dd") + " " + txt13.Text.Trim() + ":00";
            string strtime2 = (txt14.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker4.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00";
            TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            if (Convert.ToDateTime(strtime2) <= Convert.ToDateTime(strtime1))
            {
                MessageBox.Show("日期時間輸入異常，請檢查...");
                return;
            }
            //Insert
            SqlStr = "Insert into [Employee_Work_Detail]"
            + " (EmpNo, Item, SNo, ParaData1, Time1, Time2, Time1ToTime2, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt2.Text.Trim() + "',"
            + "'" + CB1.SelectedValue.ToString().Trim() + "',"
            + "'" + para1.Trim() + "',"
            + "'" + strtime1.Trim() + "',"//起時間
            + "'" + strtime2.Trim() + "',"//迄時間
            + "'" + ts.TotalMinutes.ToString() + "',"//小計(分)          
            + "'" + Loginfm.id.Trim() + "',"
            + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + ")";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("新增資料完成...");
            //重新查詢
            Form_Query_Detail();
        }
        private void Form_Query_Detail()
        {
            SqlStr = "Select "
                + "A.SNo as '除外工時代碼',"
                + "A.ParaData1 as '除外工時參數',"
                + "substring(convert(varchar,A.Time1,121),1,16) as '發生時間(起)',"
                + "substring(convert(varchar,A.Time2,121),1,16) as '發生時間(迄)',"
                + "A.Time1ToTime2 as '小計(分)'"
                + " from Employee_Work_Detail A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
                + " where A.EmpNo = '" + txt1.Text.Trim() + "' and A.Item = '" + txt2.Text.Trim() + "'";// and A.Work_No = '" + CB2.SelectedValue.ToString() + "'";
            SqlStr = SqlStr + " order by Time1 asc";
            System.Data.DataTable  dt = Class1.GetDataTable(SqlStr);
            dgv2.DataSource = dt;
            dgv2.Columns[0].Width = 100; 
            dgv2.Columns[1].Width = 100; 
            dgv2.Columns[2].Width = 120; 
            dgv2.Columns[3].Width = 120;
            dgv2.Columns[4].Width = 80;
            CB1.SelectedIndex = 0;
            this.dateTimePicker3.Value = DateTime.Now;
            this.txt13.Text = "00:00";
            this.dateTimePicker4.Value = DateTime.Now;
            this.txt14.Text = "00:00";
        }

        private void dgv2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //物料代碼
            this.CB1.SelectedValue = dgv2.Rows[e.RowIndex].Cells["除外工時代碼"].Value.ToString().Trim();
            //日期時間
            if (dgv2.Rows[e.RowIndex].Cells["發生時間(起)"].Value.ToString().Trim() != "")
            {
                label4.Text = dgv2.Rows[e.RowIndex].Cells["除外工時代碼"].Value.ToString().Trim();//key
                label5.Text = dgv2.Rows[e.RowIndex].Cells["發生時間(起)"].Value.ToString().Trim();//key
                this.dateTimePicker3.Text = dgv2.Rows[e.RowIndex].Cells["發生時間(起)"].Value.ToString().Trim();
                if (dgv2.Rows[e.RowIndex].Cells["發生時間(起)"].Value == DBNull.Value)
                    this.txt13.Text = "";
                else
                    this.txt13.Text = Convert.ToDateTime(dgv2.Rows[e.RowIndex].Cells["發生時間(起)"].Value).ToString("HH:mm");
            }
            if (dgv2.Rows[e.RowIndex].Cells["發生時間(迄)"].Value.ToString().Trim() != "")
            {
                this.dateTimePicker3.Text = dgv2.Rows[e.RowIndex].Cells["發生時間(迄)"].Value.ToString().Trim();
                if (dgv2.Rows[e.RowIndex].Cells["發生時間(迄)"].Value == DBNull.Value)
                    this.txt14.Text = "";
                else
                    this.txt14.Text = Convert.ToDateTime(dgv2.Rows[e.RowIndex].Cells["發生時間(迄)"].Value).ToString("HH:mm");
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『員工編號』資料...");
                return;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『項次』資料...");
                return;
            }
            if (CB1.SelectedIndex == 0)
            {
                MessageBox.Show("請先選擇『除外工時代碼』之資料...");
                return;
            }
            string para1 = Class1.GetValue("ParaData1", "Para", "SNo='" + CB1.SelectedValue.ToString() + "' and Kind='B'");
            string strtime1 = (txt13.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker3.Text).ToString("yyyy-MM-dd") + " " + txt13.Text.Trim() + ":00";
            string strtime2 = (txt14.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker4.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00";
            TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            if (Convert.ToDateTime(strtime2) <= Convert.ToDateTime(strtime1))
            {
                MessageBox.Show("日期時間輸入異常，請檢查...");
                return;
            }
            //update
            SqlStr = "update [Employee_Work_Detail] set "
            + "SNo = '" + CB1.SelectedValue.ToString() + "',"
            + "ParaData1 = '" + para1 + "',"
            + "Time1 = '" + strtime1 + "',"
            + "Time2 = '" + strtime2 + "',"
            + "Time1ToTime2 = '" + ts.TotalMinutes.ToString() + "',"
            + "ENo = '" + Loginfm.id.Trim() + "',"
            + "SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where EmpNo='" + this.label_No1.Text.Trim() + "' and Item='" + this.label_No2.Text.Trim() + "'"
            + " and SNo='" + this.label4.Text.Trim() + "' and Time1='" + this.label5.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            //重新查詢
            Form_Query_Detail();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『員工編號』資料...");
                return;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『項次』資料...");
                return;
            }
            if (CB1.SelectedIndex == 0)
            {
                MessageBox.Show("請先選擇『除外工時代碼』之資料...");
                return;
            }
            string para1 = Class1.GetValue("ParaData1", "Para", "SNo='" + CB1.SelectedValue.ToString() + "' and Kind='B'");
            string strtime1 = (txt13.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker3.Text).ToString("yyyy-MM-dd") + " " + txt13.Text.Trim() + ":00";
            if (MessageBox.Show(this, "確定要刪除『員工編號/項次/除外工時代碼』為：『" + this.txt1.Text.Trim() + "/" + this.txt2.Text.Trim() + "/" + this.CB1.SelectedValue + "』的明細資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Employee_Work_Detail"
                + " where EmpNo='" + this.label_No1.Text.Trim() + "' and Item='" + this.label_No2.Text.Trim() + "'"
                + " and SNo='" + this.CB1.SelectedValue.ToString() + "' and Time1='" + strtime1.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成，請重新查詢...");
                //重新查詢
                Form_Query_Detail();
            }
        }

        private void txt15_Enter(object sender, EventArgs e)
        {
            if (txt3.Text.Trim() != "" && txt4.Text.Trim() != "")
            {
                string strtime1 = (txt3.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker5.Text).ToString("yyyy-MM-dd") + " " + txt3.Text.Trim() + ":00";
                string strtime2 = (txt4.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker6.Text).ToString("yyyy-MM-dd") + " " + txt4.Text.Trim() + ":00";
                TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
                this.txt15.Text = ts.TotalMinutes.ToString();
            }
        }

        private void txt8_TextChanged(object sender, EventArgs e)
        {
            txt8.BackColor = (txt8.Text.Trim() == "00:00") ? Color.Yellow : Color.White;
        }

        private void txt10_TextChanged(object sender, EventArgs e)
        {
            txt10.BackColor = (txt10.Text.Trim() == "00:00") ? Color.Yellow : Color.White;
        }

        private void txt13_TextChanged(object sender, EventArgs e)
        {
            txt13.BackColor = (txt13.Text.Trim() == "00:00") ? Color.Moccasin : Color.White;
        }

        private void txt14_TextChanged(object sender, EventArgs e)
        {
            txt14.BackColor = (txt14.Text.Trim() == "00:00") ? Color.Moccasin : Color.White;
        }

        private void txt3_TextChanged(object sender, EventArgs e)
        {
            txt3.BackColor = (txt3.Text.Trim() == "00:00") ? Color.PaleGreen : Color.White;
        }

        private void txt4_TextChanged(object sender, EventArgs e)
        {
            txt4.BackColor = (txt4.Text.Trim() == "00:00") ? Color.PaleGreen : Color.White;
        }
    }
}
