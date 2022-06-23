using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace CPCAIModule
{
    public partial class Form14 : Form
    {
        string SqlStr = "";
        public static string msg = "";
        //public string[] msg = new string[3];
        //共用變數
        public Form14()
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
            //+ "substring(convert(varchar,A.Time1,121),1,16) as '開機時間',"//開機時間
            //+ "case when Convert(varchar,A.Expect_Time,111)='1900/01/01'"
            //+ " then '---' else substring(convert(varchar,A.Expect_Time,121),1,16) end as '預計備齊時間',"
            //+ "A.IsStop as '除外工時',"
            SqlStr = "Select top 300 A.Line_No as '線別編號',"
            + "A.Work_No as '工單編號',"
            + "A.Num as '數量序號',"
            + "B.goods_No as '物料編號',"
            + "B.goods_name as '物料名稱',"
            //+ "B.goods_Spec as '物料規格',"           
            + "A.OK as 'OK',"
            + "A.NG as 'NG',"
            + "A.NG_Num as 'NG代號：0無 / 1電壓 / 2電流 / 4溫度',"
            + "A.ENo as '檢驗人員',"
            + "convert(varchar,A.SDate,120) as '檢驗時間'"
            + " from QC_Data A left join ProductWork B on A.Work_No=B.Work_No"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by A.SDate desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 10;//總共0-13共14個欄位
            int[] column_width = { 100, 100, 100, 100, 160, 100, 100, 220, 80, 160 };//欄寬值
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
        private void Form14_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 75 / 100;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 62 / 100;//功能鍵高，比例法，離上方往下182
            //上面中間切一刀
            this.splitContainer3.SplitterDistance = splitContainer1.Width * 55 / 100;//功能鍵高，比例法，離左方往下182
            //上面右邊切一刀
            this.splitContainer4.SplitterDistance = 680;
            //上面上邊切一刀
            this.splitContainer5.SplitterDistance = 50;
            //上面下邊切一刀
            this.splitContainer6.SplitterDistance = splitContainer1.Height * 42 / 100;//藍色高
            //下拉選項
            Class1.DropDownList_B("I_No", "Instrument", comboBox1, "where I_Name<>''");
            Class1.DropDownList_A("SNo", "ParaData1", "Para", CB1, "where Kind='A'");
            this.comboBox1.SelectedIndex = 0;           
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
            //抓DB上下限值
            label5.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '1'") + "<=電壓<=" + Class1.GetValue("Up_limit_1", "QC_Item", "Num = '1'");
            label11.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '2'") + "<=電流<=" + Class1.GetValue("Up_limit_1", "QC_Item", "Num = '2'");
            label12.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '4'") + "<=溫度<=" + Class1.GetValue("Up_limit_1", "QC_Item", "Num = '4'");
            txt4.Focus();
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //物料代碼
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["設備編號"].Value.ToString().Trim();
            this.label_No1.Text = this.txt1.Text;
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["項次"].Value.ToString().Trim();
            this.label_No2.Text = this.txt2.Text;
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["線別"].Value.ToString().Trim();
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["作業區域"].Value.ToString().Trim();
            //日期時間
            if (dgvDetail2.Rows[e.RowIndex].Cells["開機時間"].Value.ToString().Trim() != "")
            {
                //this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                this.dateTimePicker1.Text = dgvDetail2.Rows[e.RowIndex].Cells["開機時間"].Value.ToString().Trim();
                //this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
                if (dgvDetail2.Rows[e.RowIndex].Cells["開機時間"].Value == DBNull.Value)
                    this.txt8.Text = "";
                else
                    this.txt8.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["開機時間"].Value).ToString("HH:mm");
            }
            if (dgvDetail2.Rows[e.RowIndex].Cells["停機時間"].Value.ToString().Trim() != "")
            {
                //this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                this.dateTimePicker2.Text = dgvDetail2.Rows[e.RowIndex].Cells["停機時間"].Value.ToString().Trim();
                //this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
                if (dgvDetail2.Rows[e.RowIndex].Cells["停機時間"].Value == DBNull.Value)
                    this.txt10.Text = "";
                else
                    this.txt10.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["停機時間"].Value).ToString("HH:mm");
            }
            this.txt11.Text = dgvDetail2.Rows[e.RowIndex].Cells["小計(分)"].Value.ToString().Trim();
            this.checkBox1.Checked = (dgvDetail2.Rows[e.RowIndex].Cells["除外工時"].Value.ToString().Trim() == "Y") ? true : false;
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
            this.txt9.Text = ""; this.txt10.Text = ""; this.txt11.Text = ""; this.txt12.Text = "";
            this.txt15.Text = ""; this.txt16.Text = ""; this.txt17.Text = "L01";
            this.comboBox1.SelectedIndex = 0; this.textBox1.Text = ""; //this.txt14.BackColor = Color.Yellow;
            this.CB1.SelectedIndex = 0; //this.dateTimePicker1.Enabled = false; this.dateTimePicker2.Enabled = false;
            this.label_No1.Text = ""; this.label_No2.Text = "";
            this.dateTimePicker1.Value = DateTime.Now; this.dateTimePicker2.Value = DateTime.Now;
            this.txt8.Text = DateTime.Now.ToString("HH:mm");
            this.checkBox1.Checked = false;
            this.txt13.Text = "00:00"; this.txt14.Text = "00:00";
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『物料編號』資料...");
                return false;
            }            
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『顏色』資料...");
                return false;
            }
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『物料名稱』資料...");
                return false;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『物料規格』資料...");
                return false;
            }
            //檢查時間是否異常
            //string strtime1 = (txt8.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt8.Text.Trim() + ":00";
            //string strtime2 = (txt10.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " " + txt10.Text.Trim() + ":00";
            //TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            //if (Convert.ToDateTime(strtime2) <= Convert.ToDateTime(strtime1))
            //{
            //    MessageBox.Show("日期時間輸入異常，請檢查...");
            //    return false;
            //}
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
            string strtime1 = Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt8.Text.Trim();
            //if (Class1.GetRowCount("B02A1", "QCDateTime='" + strtime1.Trim() + "'") > 0)
            //{
            //    MessageBox.Show("請注意『工單編號/物料編號/檢驗時間：" + txt3.Text.Trim() + "/" + txt1.Text.Trim() + "/" + strtime1.Trim() + "』資料，已存在...");
            //    return;
            //}
            //string strarea = (txt4.Text.Trim() == "") ? "null" : txt4.Text.Trim();
            //string strtime1 = (txt8.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt8.Text.Trim() + ":00";
            //string strtime2 = (txt10.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " " + txt10.Text.Trim() + ":00";
            //TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            //判斷是否NG=>1電壓、2電流、4溫度、3顏色不用判斷
            int isOK = 0; int isNG = 0; string ngnum = "";            
            if (txt1E.Text.Trim() == "OK" && txt2E.Text.Trim() == "OK" && txt3E.Text.Trim() == "OK")
            {
                isOK = 1; isNG = 0; ngnum = txt16.Text;
            }
            else
            {
                isOK = 0; isNG = 1; ngnum = txt16.Text;
            }
            #region 閃燈
            if (isOK == 1 && isNG == 0)
            {
                ngnum = "0";
                for (int i = 0; i < 3; i++)
                {
                    this.txt15.BackColor = Color.Green;
                    this.Refresh();
                    Thread.Sleep(100);
                    this.txt15.BackColor = Color.White;
                    this.Refresh();
                    Thread.Sleep(100);
                }
            }
            else
            {
                if (ngnum == "1")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        this.txt16.BackColor = Color.Red;
                        this.Refresh();
                        Thread.Sleep(100);
                        this.txt16.BackColor = Color.White;
                        this.Refresh();
                        Thread.Sleep(100);
                    }
                }
                else if (ngnum == "2")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        this.txt16.BackColor = Color.Gold;
                        this.Refresh();
                        Thread.Sleep(100);
                        this.txt16.BackColor = Color.White;
                        this.Refresh();
                        Thread.Sleep(100);
                    }
                }
                else if (ngnum == "4")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        this.txt16.BackColor = Color.Pink;
                        this.Refresh();
                        Thread.Sleep(100);
                        this.txt16.BackColor = Color.White;
                        this.Refresh();
                        Thread.Sleep(100);
                    }
                }
            }
            #endregion
            //取號//抓目前工單數量
            string cus = Class1.GetValue("count(*)", "QC_Data", "Line_No = '" + txt17.Text.Trim() + "' and Work_No = '" + txt3.Text + "'");
            cus = (Convert.ToInt16(cus) + 1).ToString();
            //Insert
            SqlStr = "Insert into [QC_Data]"
            + " (Line_No, Work_No, Num, OK, NG, NG_Num, ENo, SDate) "
            + "values ("
            + "'" + txt17.Text.Trim() + "',"//線別
            + "'" + txt3.Text.Trim() + "',"//工單號
            + "" + cus.Trim() + ","//第幾個
            + "" + isOK + ","
            + "" + isNG + ","
            + "'" + ngnum + "',"
            + "'" + txt4.Text.Trim() + "',"
            + "'" + strtime1.Trim() + "'"//檢驗時間
            + ")";
            Class1.Execute_SQL(SqlStr);
            //改燈號
            int colorNo=3;//綠燈
            if (isOK == 1) 
                colorNo = 3;
            else
                colorNo = 1;
            SqlStr = "update ColorLight set Color=" + colorNo + "";
            Class1.Execute_SQL(SqlStr);
            //Line NG=1
            int lineNo = 0;//綠燈
            if (isOK == 1)
                lineNo = 0;
            else
                lineNo = 1;
            SqlStr = "update LineMessage set Message=" + lineNo + "";
            Class1.Execute_SQL(SqlStr);
            //MessageBox.Show("新增資料完成...");
            //button7_Click(sender, e);//清除
            //button3_Click(sender, e);//查詢
            Form_Query("");
        }
        //刪除
        private void button6_Click(object sender, EventArgs e)
        {
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『設備編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『設備編號/項次』為:" + this.txt1.Text.Trim() + "/" + this.txt2.Text.Trim() + "的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Equipment_Work"
                + " where I_No='" + this.txt1.Text.Trim() + "' and Item='" + this.txt2.Text.Trim() + "'";
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
            string strline = (txt3.Text.Trim() == "") ? "null" : txt3.Text.Trim();
            string strarea = (txt4.Text.Trim() == "") ? "null" : txt4.Text.Trim();
            string strtime1 = (txt8.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt8.Text.Trim() + ":00";
            string strtime2 = (txt10.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " " + txt10.Text.Trim() + ":00";
            TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            string YN = (this.checkBox1.Checked == true) ? "Y" : "N";
            //update
            SqlStr = "update [Equipment_Work] set "
            + "I_No = '" + txt1.Text.Trim() + "',"
                //+ "I_Name = '" + txt5.Text.Trim() + "',"
                //+ "Brand = '" + txt2.Text.Trim() + "',"
                //+ "I_Spec = '" + txt6.Text.Trim() + "',"
            + "Item = '" + txt2.Text.Trim() + "',"
            + "Line_No = '" + strline.Trim() + "',"
            + "Area = '" + strarea.Trim() + "',"
            + "Time1 = '" + strtime1 + "',"
            + "Time2 = '" + strtime2 + "',"
            + "Time1ToTime2 = '" + ts.TotalMinutes.ToString() + "',"
            + "IsStop = '" + YN.Trim() + "',"
            + "ENo = '" + Loginfm.id.Trim() + "',"
            + "SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where I_No='" + this.label_No1.Text.Trim() + "' and Item='" + this.label_No2.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form14_FormClosed(object sender, FormClosedEventArgs e)
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
                this.txt1.Text = input.GetMsg();
            else
                this.txt1.Text = msg.Trim();
            if (txt1.Text.Trim() != "")
            {
                this.txt5.Text = Class1.GetValue("goods_Name", "goods_Basic", "goods_No='" + txt1.Text.ToString() + "'");
                this.txt6.Text = Class1.GetValue("goods_Spec", "goods_Basic", "goods_No='" + txt1.Text.ToString() + "'");
            }
            txt2.Focus();
        }

        private void txt1_TextChanged(object sender, EventArgs e)
        {
            string dataA = this.txt1.Text.Trim();
            this.txt5.Text = Class1.GetValue("I_Name", "Instrument", "I_No='" + dataA + "'");
            //this.txt2.Text = Class1.GetValue("Brand", "Instrument", "I_No='" + dataA + "'");
            this.txt6.Text = Class1.GetValue("I_Spec", "Instrument", "I_No='" + dataA + "'");
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
                MessageBox.Show("請先輸入『設備編號』的資料，再取號...");
                return;
            }
            //取號
            string item_No = Class1.GetValue("count(*)", "Equipment_Work", "I_No = '" + this.txt1.Text + "'");
            item_No = (Convert.ToInt16(item_No) + 1).ToString();
            this.txt2.Text = item_No.Trim();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『設備編號』資料...");
                return;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『項次』資料...");
                return;
            }
            string para1 = Class1.GetValue("ParaData1", "Para", "SNo='" + CB1.SelectedValue.ToString() + "' and Kind='A'");
            string strtime1 = (txt13.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker3.Text).ToString("yyyy-MM-dd") + " " + txt13.Text.Trim() + ":00";
            string strtime2 = (txt14.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker4.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00";
            TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            if (Convert.ToDateTime(strtime2) <= Convert.ToDateTime(strtime1))
            {
                MessageBox.Show("日期時間輸入異常，請檢查...");
                return;
            }
            //Insert
            SqlStr = "Insert into [Equipment_Work_Detail]"
            + " (I_No, Item, SNo, ParaData1, Time1, Time2, Time1ToTime2, ENo, SDate) "
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
                + " from Equipment_Work_Detail A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
                + " where A.I_No = '" + txt1.Text.Trim() + "' and A.Item = '" + txt2.Text.Trim() + "'";// and A.Work_No = '" + CB2.SelectedValue.ToString() + "'";
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
                MessageBox.Show("請先輸入『設備編號』資料...");
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
            string para1 = Class1.GetValue("ParaData1", "Para", "SNo='" + CB1.SelectedValue.ToString() + "' and Kind='A'");
            string strtime1 = (txt13.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker3.Text).ToString("yyyy-MM-dd") + " " + txt13.Text.Trim() + ":00";
            string strtime2 = (txt14.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker4.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00";
            TimeSpan ts = Convert.ToDateTime(strtime2) - Convert.ToDateTime(strtime1);
            if (Convert.ToDateTime(strtime2) <= Convert.ToDateTime(strtime1))
            {
                MessageBox.Show("日期時間輸入異常，請檢查...");
                return;
            }
            //update
            SqlStr = "update [Equipment_Work_Detail] set "
            + "SNo = '" + CB1.SelectedValue.ToString() + "',"
            + "ParaData1 = '" + para1 + "',"
            + "Time1 = '" + strtime1 + "',"
            + "Time2 = '" + strtime2 + "',"
            + "Time1ToTime2 = '" + ts.TotalMinutes.ToString() + "',"
            + "ENo = '" + Loginfm.id.Trim() + "',"
            + "SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where I_No='" + this.label_No1.Text.Trim() + "' and Item='" + this.label_No2.Text.Trim() + "'"
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
                MessageBox.Show("請先輸入『設備編號』資料...");
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
            string para1 = Class1.GetValue("ParaData1", "Para", "SNo='" + CB1.SelectedValue.ToString() + "' and Kind='A'");
            string strtime1 = (txt13.Text.Trim() == "") ? "null" : Convert.ToDateTime(this.dateTimePicker3.Text).ToString("yyyy-MM-dd") + " " + txt13.Text.Trim() + ":00";
            if (MessageBox.Show(this, "確定要刪除『設備編號/項次/除外工時代碼』為：『" + this.txt1.Text.Trim() + "/" + this.txt2.Text.Trim() + "/" + this.CB1.SelectedValue + "』的明細資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Equipment_Work_Detail"
                + " where I_No='" + this.label_No1.Text.Trim() + "' and Item='" + this.label_No2.Text.Trim() + "'"
                + " and SNo='" + this.CB1.SelectedValue.ToString() + "' and Time1='" + strtime1.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成，請重新查詢...");
                //重新查詢
                Form_Query_Detail();
            }
        }

        private void txt8_TextChanged(object sender, EventArgs e)
        {
            //txt8.BackColor = (txt8.Text.Trim() == "00:00") ? Color.Yellow : Color.White;
        }
        
        private void txt10_TextChanged(object sender, EventArgs e)
        {
            //txt10.BackColor = (txt10.Text.Trim() == "00:00") ? Color.Yellow : Color.White;
        }

        private void txt13_TextChanged(object sender, EventArgs e)
        {
            txt13.BackColor = (txt13.Text.Trim() == "00:00") ? Color.Moccasin : Color.White;
        }

        private void txt14_TextChanged(object sender, EventArgs e)
        {
            txt14.BackColor = (txt14.Text.Trim() == "00:00") ? Color.Moccasin : Color.White;
        }

        private void txt4_TextChanged(object sender, EventArgs e)
        {
            //抓姓名
            string sname = Class1.GetValue("Name", "Employee", "EmpNo='" + txt4.Text.Trim() + "'");
            if (sname != "")
            {
                button10.Text = sname;
                button10.BackColor = Color.Yellow;
                txt3.Focus();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            InputBox2 input = new InputBox2();
            DialogResult dr = input.ShowDialog();
            if (dr == DialogResult.OK)
                this.txt3.Text = input.GetMsg();
            else
                this.txt3.Text = msg.Trim();
            txt1.Focus();
        }        

        private void txt1_Enter(object sender, EventArgs e)
        {
            string sno = this.txt3.Text.Trim();
            if (sno.Trim() != "")
            {
                string mno = Class1.GetValue("goods_No", "ProductWork", "Work_No='" + sno + "'");
                this.txt1.Text = mno.Trim();
                this.txt5.Text = Class1.GetValue("goods_name", "goods_Basic", "goods_No='" + mno + "'");
                this.txt6.Text = Class1.GetValue("goods_Spec", "goods_Basic", "goods_No='" + mno + "'");
                txt15.Focus();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.txt8.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //*
            //每5秒亂數產生1個
            string[] colorArray = new string[10]{"水漾藍","天空藍","乳白","純萃白","橘黃","深灰","淺灰","藏青","楊柳綠","洋紅"};
            Random rnd = new Random();  //產生亂數初始值
            int i = rnd.Next(0, 10);
            //顏色
            this.txt2.Text = colorArray[i].ToString().Trim();
            //電壓
            this.txt11.Text = (rnd.Next(90, 118)).ToString();
            //溫度
            this.txt10.Text = (rnd.Next(38, 95)).ToString();
            //電流
            this.txt12.Text = (Convert.ToDecimal(rnd.Next(270, 620))/100).ToString();
            //重複檢查
            string sdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (Class1.GetRowCount("QC_Temp", "SDateTime='" + sdatetime + "'") == 0)
            {
                //Insert
                SqlStr = "Insert into [QC_Temp]"
                + " (SDateTime, TR, color, Vote, EC) "
                + "values ("
                + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"
                + "" + txt10.Text.Trim() + ","
                + "'" + txt2.Text.Trim() + "',"
                + "" + txt11.Text.Trim() + ","
                + "" + txt12.Text.Trim() + ""
                    //+ "'" + strtime1.Trim() + "',"//檢驗時間
                    //+ "'" + ts.TotalMinutes.ToString() + "',"//小計(分)          
                    //+ "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"//Expect_Arrive
                + ")";
                Class1.Execute_SQL(SqlStr);
            }
            //*/
            #region 抓 DB show 在畫面
            SqlStr = "Select top 300 "
            + "convert(varchar,A.SDateTime,120) as '檢驗時間',"
            + "A.Color as '顏色',"
            + "A.TR as '溫度',"
            + "A.Vote as '電壓',"
            + "A.EC as '電流'"
            + " from QC_Temp A"
            + " where 1=1 order by A.SDateTime desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            this.dgv2.DataSource = dt;
            dgv2.Columns[0].Width = 160;
            label4.Text = "目前筆數：" + dt.Rows.Count.ToString();
            /*
            SqlStr = "Select top 1 A.Work_No as '工單編號',"
            + "A.goods_No as '物料編號',"
            + "B.goods_name as '物料名稱',"
            + "B.goods_Spec as '物料規格',"
            + "convert(varchar,A.QCDateTime,120) as '檢驗時間',"
            + "A.Color as '顏色',"
            + "A.Temperature as '溫度',"
            + "A.Voltage as '電壓',"
            + "A.ECurrent as '電流',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from B02A1 A left join goods_Basic B on A.goods_No=B.goods_No"
            + " where 1=1 order by A.QCDateTime desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            //姓名
            this.txt4.Text = dt.Rows[0]["異動人員"].ToString();
            this.txt3.Text = dt.Rows[0]["工單編號"].ToString();
            this.txt1.Text = dt.Rows[0]["物料編號"].ToString();
            this.txt5.Text = Class1.GetValue("goods_name", "goods_Basic", "goods_No='" + txt1.Text.Trim() + "'");
            this.txt6.Text = Class1.GetValue("goods_Spec", "goods_Basic", "goods_No='" + txt1.Text.Trim() + "'");
            //時間
            this.dateTimePicker1.Text = dt.Rows[0]["檢驗時間"].ToString().Trim();
            this.txt8.Text = Convert.ToDateTime(dt.Rows[0]["檢驗時間"]).ToString("HH:mm:ss");
            //顏色
            this.txt2.Text = dt.Rows[0]["顏色"].ToString();
            //電壓
            this.txt11.Text = dt.Rows[0]["電壓"].ToString();
            //溫度
            this.txt10.Text = dt.Rows[0]["溫度"].ToString();
            //電流
            this.txt12.Text = dt.Rows[0]["電流"].ToString();
            */
            #endregion
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            Form_Query("");//設定筆數超過幾筆就砍掉 最早資料
            int Num = Class1.GetRowCount("QC_Temp", "TR<>'' and color<>'' and Vote>0 and EC>0");
            if (Num > 200)//>200砍10
            {
                SqlStr = "Delete from QC_Temp where SDateTime in (select top 20 SDateTime from QC_Temp order by SDateTime asc)";
                Class1.Execute_SQL(SqlStr);
            }
            label4.Text = "目前筆數：" + Num.ToString();
        }

        private void txt3_TextChanged(object sender, EventArgs e)
        {
            string dataA = this.txt3.Text.Trim();
            this.txt1.Text = Class1.GetValue("goods_No", "ProductWork", "Work_No='" + dataA + "'");
            this.txt5.Text = Class1.GetValue("goods_Name", "ProductWork", "Work_No='" + dataA + "'");
            this.txt6.Text = Class1.GetValue("goods_Spec", "ProductWork", "Work_No='" + dataA + "'");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                timer_Random.Enabled = true;
                checkBox2.Checked = false;
            }
            else
            {
                timer_Random.Enabled = false;
                checkBox2.Checked = false;
            }
        }

        private void timer_showDB_Tick(object sender, EventArgs e)
        {
            SqlStr = "Select top 300 "
            + "convert(varchar,A.SDateTime,120) as '檢驗時間',"
            + "A.Color as '顏色',"
            + "A.TR as '溫度',"
            + "A.Vote as '電壓',"
            + "A.EC as '電流'"
            + " from QC_Temp A"
            + " where 1=1 order by A.SDateTime desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            this.dgv2.DataSource = dt;
            //抓到畫面
            //this.txt2.Text = dt.Rows[0]["顏色"].ToString();
            //this.txt11.Text = dt.Rows[0]["電壓"].ToString();
            //this.txt10.Text = dt.Rows[0]["溫度"].ToString();
            //this.txt12.Text = dt.Rows[0]["電流"].ToString();
            dgv2.Columns[0].Width = 160;
            label4.Text = "目前筆數：" + dt.Rows.Count.ToString();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                timer_showDB.Enabled = true;
                timer_Random.Enabled = false;
                checkBox1.Checked = false;
            }
            else
            {
                timer_showDB.Enabled = false;
                timer_Random.Enabled = false;
                checkBox1.Checked = false;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //抓DB上下限值
            label5.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '1'") + "<=電壓<=" + Class1.GetValue("Up_limit_1", "QC_Item", "Num = '1'");
            label11.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '2'") + "<=電流<=" + Class1.GetValue("Up_limit_1", "QC_Item", "Num = '2'");
            label12.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '4'") + "<=溫度<=" + Class1.GetValue("Up_limit_1", "QC_Item", "Num = '4'");
            //第1段
            SqlStr = "Select top 1 "
            + "convert(varchar,A.SDateTime,120) as '檢驗時間',"
            + "A.Color as '顏色',"
            + "A.TR as '溫度',"
            + "A.Vote as '電壓',"
            + "A.EC as '電流'"
            + " from QC_Temp A"
            + " where 1=1 order by A.SDateTime desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            //抓到畫面
            this.txt2.Text = dt.Rows[0]["顏色"].ToString();
            this.txt11.Text = dt.Rows[0]["電壓"].ToString();
            this.txt10.Text = dt.Rows[0]["溫度"].ToString();
            this.txt12.Text = dt.Rows[0]["電流"].ToString();
            #region 第1段
            //電壓NG-OK
            string Rvalue = "OK";
            decimal Vote_down = Convert.ToDecimal(Class1.GetValue("down_limit_1", "QC_Item", "Num = '1'"));
            decimal Vote_up = Convert.ToDecimal(Class1.GetValue("up_limit_1", "QC_Item", "Num = '1'"));
            if (Convert.ToDecimal(txt11.Text) < Vote_down || Convert.ToDecimal(txt11.Text) > Vote_up)
            {
                Rvalue = "NG"; txt1E.Text = "NG"; txt16.Text = "1";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return;
            }
            else
            { Rvalue = "OK"; txt1E.Text = "OK"; }
            //電流NG-OK
            decimal EC_down = Convert.ToDecimal(Class1.GetValue("down_limit_1", "QC_Item", "Num = '2'"));
            decimal EC_up = Convert.ToDecimal(Class1.GetValue("up_limit_1", "QC_Item", "Num = '2'"));
            if (Convert.ToDecimal(txt12.Text) < EC_down || Convert.ToDecimal(txt12.Text) > EC_up)
            {
                Rvalue = "NG"; txt1E.Text = "NG"; txt16.Text = "2";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return;             
            }
            else
            { Rvalue = "OK"; txt1E.Text = "OK"; }
            //溫度NG-OK
            decimal TR_down = Convert.ToDecimal(Class1.GetValue("down_limit_1", "QC_Item", "Num = '4'"));
            decimal TR_up = Convert.ToDecimal(Class1.GetValue("up_limit_1", "QC_Item", "Num = '4'"));
            if (Convert.ToDecimal(txt10.Text) < TR_down || Convert.ToDecimal(txt10.Text) > TR_up)
            {
                Rvalue = "NG"; txt1E.Text = "NG"; txt16.Text = "4";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return; 
            }
            else
            { Rvalue = "OK"; txt1E.Text = "OK"; }
            //判斷
            if (Rvalue == "OK")
            {
                this.txt15.BackColor = Color.Green;
                this.txt16.BackColor = Color.White; 
                txt16.Text = "0";
            }
            #endregion
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //抓DB上下限值
            label5.Text = Class1.GetValue("down_limit_2", "QC_Item", "Num = '1'") + "<=電壓<=" + Class1.GetValue("Up_limit_2", "QC_Item", "Num = '1'");
            label11.Text = Class1.GetValue("down_limit_2", "QC_Item", "Num = '2'") + "<=電流<=" + Class1.GetValue("Up_limit_2", "QC_Item", "Num = '2'");
            label12.Text = Class1.GetValue("down_limit_2", "QC_Item", "Num = '4'") + "<=溫度<=" + Class1.GetValue("Up_limit_2", "QC_Item", "Num = '4'");
            //第2段
            SqlStr = "Select top 1 "
            + "convert(varchar,A.SDateTime,120) as '檢驗時間',"
            + "A.Color as '顏色',"
            + "A.TR as '溫度',"
            + "A.Vote as '電壓',"
            + "A.EC as '電流'"
            + " from QC_Temp A"
            + " where 1=1 order by A.SDateTime desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            //抓到畫面
            this.txt2A.Text = dt.Rows[0]["顏色"].ToString();
            this.txt2B.Text = dt.Rows[0]["電壓"].ToString();
            this.txt2C.Text = dt.Rows[0]["溫度"].ToString();
            this.txt2D.Text = dt.Rows[0]["電流"].ToString();
            #region 第2段
            //電壓NG-OK
            string Rvalue = "OK";
            decimal Vote_down = Convert.ToDecimal(Class1.GetValue("down_limit_2", "QC_Item", "Num = '1'"));
            decimal Vote_up = Convert.ToDecimal(Class1.GetValue("up_limit_2", "QC_Item", "Num = '1'"));
            if (Convert.ToDecimal(txt2B.Text) < Vote_down || Convert.ToDecimal(txt2B.Text) > Vote_up)
            {
                Rvalue = "NG"; txt2E.Text = "NG"; txt16.Text = "1";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return;
            }
            else
            { Rvalue = "OK"; txt2E.Text = "OK"; }
            //電流NG-OK
            decimal EC_down = Convert.ToDecimal(Class1.GetValue("down_limit_2", "QC_Item", "Num = '2'"));
            decimal EC_up = Convert.ToDecimal(Class1.GetValue("up_limit_2", "QC_Item", "Num = '2'"));
            if (Convert.ToDecimal(txt2D.Text) < EC_down || Convert.ToDecimal(txt2D.Text) > EC_up)
            {
                Rvalue = "NG"; txt2E.Text = "NG"; txt16.Text = "2";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return;
            }
            else
            { Rvalue = "OK"; txt2E.Text = "OK"; }
            //溫度NG-OK
            decimal TR_down = Convert.ToDecimal(Class1.GetValue("down_limit_2", "QC_Item", "Num = '4'"));
            decimal TR_up = Convert.ToDecimal(Class1.GetValue("up_limit_2", "QC_Item", "Num = '4'"));
            if (Convert.ToDecimal(txt2C.Text) < TR_down || Convert.ToDecimal(txt2C.Text) > TR_up)
            {
                Rvalue = "NG"; txt2E.Text = "NG"; txt16.Text = "4";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return;
            }
            else
            { Rvalue = "OK"; txt2E.Text = "OK"; }
            //判斷
            if (Rvalue == "OK")
            {
                this.txt15.BackColor = Color.Green;
                this.txt16.BackColor = Color.White;
                txt16.Text = "0";
            }
            #endregion
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //抓DB上下限值
            label5.Text = Class1.GetValue("down_limit_3", "QC_Item", "Num = '1'") + "<=電壓<=" + Class1.GetValue("Up_limit_3", "QC_Item", "Num = '1'");
            label11.Text = Class1.GetValue("down_limit_3", "QC_Item", "Num = '2'") + "<=電流<=" + Class1.GetValue("Up_limit_3", "QC_Item", "Num = '2'");
            label12.Text = Class1.GetValue("down_limit_3", "QC_Item", "Num = '4'") + "<=溫度<=" + Class1.GetValue("Up_limit_3", "QC_Item", "Num = '4'");
            //第3段
            SqlStr = "Select top 1 "
            + "convert(varchar,A.SDateTime,120) as '檢驗時間',"
            + "A.Color as '顏色',"
            + "A.TR as '溫度',"
            + "A.Vote as '電壓',"
            + "A.EC as '電流'"
            + " from QC_Temp A"
            + " where 1=1 order by A.SDateTime desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            //抓到畫面
            this.txt3A.Text = dt.Rows[0]["顏色"].ToString();
            this.txt3B.Text = dt.Rows[0]["電壓"].ToString();
            this.txt3C.Text = dt.Rows[0]["溫度"].ToString();
            this.txt3D.Text = dt.Rows[0]["電流"].ToString();
            #region 第3段
            //電壓NG-OK
            string Rvalue = "OK";
            decimal Vote_down = Convert.ToDecimal(Class1.GetValue("down_limit_3", "QC_Item", "Num = '1'"));
            decimal Vote_up = Convert.ToDecimal(Class1.GetValue("up_limit_3", "QC_Item", "Num = '1'"));
            if (Convert.ToDecimal(txt3B.Text) < Vote_down || Convert.ToDecimal(txt3B.Text) > Vote_up)
            {
                Rvalue = "NG"; txt3E.Text = "NG"; txt16.Text = "1";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return;
            }
            else
            { Rvalue = "OK"; txt3E.Text = "OK"; }
            //電流NG-OK
            decimal EC_down = Convert.ToDecimal(Class1.GetValue("down_limit_3", "QC_Item", "Num = '2'"));
            decimal EC_up = Convert.ToDecimal(Class1.GetValue("up_limit_3", "QC_Item", "Num = '2'"));
            if (Convert.ToDecimal(txt3D.Text) < EC_down || Convert.ToDecimal(txt3D.Text) > EC_up)
            {
                Rvalue = "NG"; txt3E.Text = "NG"; txt16.Text = "2";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return;
            }
            else
            { Rvalue = "OK"; txt3E.Text = "OK"; }
            //溫度NG-OK
            decimal TR_down = Convert.ToDecimal(Class1.GetValue("down_limit_3", "QC_Item", "Num = '4'"));
            decimal TR_up = Convert.ToDecimal(Class1.GetValue("up_limit_3", "QC_Item", "Num = '4'"));
            if (Convert.ToDecimal(txt3C.Text) < TR_down || Convert.ToDecimal(txt3C.Text) > TR_up)
            {
                Rvalue = "NG"; txt3E.Text = "NG"; txt16.Text = "4";
                this.txt15.BackColor = Color.White;
                this.txt16.BackColor = Color.Red;
                return;
            }
            else
            { Rvalue = "OK"; txt3E.Text = "OK"; }
            //判斷
            if (Rvalue == "OK")
            {
                this.txt15.BackColor = Color.Green;
                this.txt16.BackColor = Color.White;
                txt16.Text = "0";
            }
            #endregion
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                //取電壓值第一段
                txt_x1.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '1'");
                txt_x2.Text = Class1.GetValue("up_limit_1", "QC_Item", "Num = '1'");
                //取電壓值第二段
                txt_y1.Text = Class1.GetValue("down_limit_2", "QC_Item", "Num = '1'");
                txt_y2.Text = Class1.GetValue("up_limit_2", "QC_Item", "Num = '1'");
                //取電壓值第三段
                txt_z1.Text = Class1.GetValue("down_limit_3", "QC_Item", "Num = '1'");
                txt_z2.Text = Class1.GetValue("up_limit_3", "QC_Item", "Num = '1'");               
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                //取電流值第一段
                txt_x1.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '2'");
                txt_x2.Text = Class1.GetValue("up_limit_1", "QC_Item", "Num = '2'");
                //取電流值第二段
                txt_y1.Text = Class1.GetValue("down_limit_2", "QC_Item", "Num = '2'");
                txt_y2.Text = Class1.GetValue("up_limit_2", "QC_Item", "Num = '2'");
                //取電流值第三段
                txt_z1.Text = Class1.GetValue("down_limit_3", "QC_Item", "Num = '2'");
                txt_z2.Text = Class1.GetValue("up_limit_3", "QC_Item", "Num = '2'");
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton4.Checked = false;
                //取顏色值第一段
                txt_x1.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '3'");
                txt_x2.Text = Class1.GetValue("up_limit_1", "QC_Item", "Num = '3'");
                //取顏色值第二段
                txt_y1.Text = Class1.GetValue("down_limit_2", "QC_Item", "Num = '3'");
                txt_y2.Text = Class1.GetValue("up_limit_2", "QC_Item", "Num = '3'");
                //取顏色值第三段
                txt_z1.Text = Class1.GetValue("down_limit_3", "QC_Item", "Num = '3'");
                txt_z2.Text = Class1.GetValue("up_limit_3", "QC_Item", "Num = '3'");
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked == true)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                //取溫度值第一段
                txt_x1.Text = Class1.GetValue("down_limit_1", "QC_Item", "Num = '4'");
                txt_x2.Text = Class1.GetValue("up_limit_1", "QC_Item", "Num = '4'");
                //取溫度值第二段
                txt_y1.Text = Class1.GetValue("down_limit_2", "QC_Item", "Num = '4'");
                txt_y2.Text = Class1.GetValue("up_limit_2", "QC_Item", "Num = '4'");
                //取溫度值第三段
                txt_z1.Text = Class1.GetValue("down_limit_3", "QC_Item", "Num = '4'");
                txt_z2.Text = Class1.GetValue("up_limit_3", "QC_Item", "Num = '4'");
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false && radioButton2.Checked == false && radioButton3.Checked == false && radioButton4.Checked == false)
            {
                MessageBox.Show("請先選擇左方選項...");
                return;
            }
            //更新電壓值第一段
            if (radioButton1.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_1 = '" + txt_x1.Text.Trim() + "',"
                + "up_limit_1 = '" + txt_x2.Text.Trim() + "'"
                + " where Num='1'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新電流值第一段
            if (radioButton2.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_1 = '" + txt_x1.Text.Trim() + "',"
                + "up_limit_1 = '" + txt_x2.Text.Trim() + "'"
                + " where Num='2'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新顏色值第一段
            if (radioButton3.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_1 = '" + txt_x1.Text.Trim() + "',"
                + "up_limit_1 = '" + txt_x2.Text.Trim() + "'"
                + " where Num='3'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新溫度值第一段
            if (radioButton4.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_1 = '" + txt_x1.Text.Trim() + "',"
                + "up_limit_1 = '" + txt_x2.Text.Trim() + "'"
                + " where Num='4'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false && radioButton2.Checked == false && radioButton3.Checked == false && radioButton4.Checked == false)
            {
                MessageBox.Show("請先選擇左方選項...");
                return;
            }
            //更新電壓值第二段
            if (radioButton1.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_2 = '" + txt_y1.Text.Trim() + "',"
                + "up_limit_2 = '" + txt_y2.Text.Trim() + "'"
                + " where Num='1'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新電流值第二段
            if (radioButton2.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_2 = '" + txt_y1.Text.Trim() + "',"
                + "up_limit_2 = '" + txt_y2.Text.Trim() + "'"
                + " where Num='2'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新顏色值第二段
            if (radioButton3.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_2 = '" + txt_y1.Text.Trim() + "',"
                + "up_limit_2 = '" + txt_y2.Text.Trim() + "'"
                + " where Num='3'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新溫度值第二段
            if (radioButton4.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_2 = '" + txt_y1.Text.Trim() + "',"
                + "up_limit_2 = '" + txt_y2.Text.Trim() + "'"
                + " where Num='4'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false && radioButton2.Checked == false && radioButton3.Checked == false && radioButton4.Checked == false)
            {
                MessageBox.Show("請先選擇左方選項...");
                return;
            }
            //更新電壓值第三段
            if (radioButton1.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_3 = '" + txt_z1.Text.Trim() + "',"
                + "up_limit_3 = '" + txt_z2.Text.Trim() + "'"
                + " where Num='1'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新電流值第三段
            if (radioButton2.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_3 = '" + txt_z1.Text.Trim() + "',"
                + "up_limit_3 = '" + txt_z2.Text.Trim() + "'"
                + " where Num='2'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新顏色值第三段
            if (radioButton3.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_3 = '" + txt_z1.Text.Trim() + "',"
                + "up_limit_3 = '" + txt_z2.Text.Trim() + "'"
                + " where Num='3'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
            //更新溫度值第三段
            if (radioButton4.Checked == true)
            {
                SqlStr = "update [QC_Item] set "
                + "down_limit_3 = '" + txt_z1.Text.Trim() + "',"
                + "up_limit_3 = '" + txt_z2.Text.Trim() + "'"
                + " where Num='4'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("修改資料完成...");
            }
        }
    }
}
