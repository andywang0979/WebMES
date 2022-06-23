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
    public partial class Form10 : Form
    {
        string SqlStr = "";
        //共用變數
        public Form10()
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
            dgvDetail2.Columns[5].Frozen = true;//固定欄位
        }
        //查詢函式
        private void Form_Query(string where_str)
        {
            //PARSENAME('$'+ Convert(varchar,Convert(money,A.Pay),1),2) as '產品單價'//轉貨幣符號
            //+ "REPLACE(Convert(varchar,A.Price),'.00','') as '產品單價',"//取代
            SqlStr = "Select "
            + "A.Work_No as '工令單號',"
            + "A.Work_Qty as '工單數量',"
            + "A.goods_No as '製品編號',"
            + "A.goods_Name as '製品名稱',"
            + "case A.Turn_Type when '1' then '1:一次轉出' when '2' then '2:分批轉出' else '' end as '轉出方式',"
            //+ "A.TurnOneTime as '每次轉出數量',"
            + "case when A.TurnOneTime is NULL then '---' else A.TurnOneTime end as '每次轉出數量',"
            + "A.Frequency as '轉出頻率',"
            + "case A.QC_Type when '1' then '1:免檢' when '2' then '2:自主檢驗' when '3' then '3:IPQC' when '4' then '4:FQC' else '' end as '品檢方式',"
            + "A.Tnru_Next as '直轉/入庫',"
            + "A.Next_Work as '後製程',"
            //+ "substring(convert(varchar,A.Expect_Arrive,121),1,16) as '預計到料時間',"
            //+ "A.Note as '備註',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from MoveWork_Master A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by A.Work_No asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 12;//總共0-11共12個欄位
            int[] column_width = { 100, 100, 100, 200, 100, 100, 100, 100, 100, 100, 80, 160 };//欄寬值
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
                where_A = " and A.goods_No ='" + this.comboBox1.SelectedValue.ToString().Trim() + "'";

            if (this.textBox1.Text.Trim() != "")
                where_A = where_A + " and A.goods_Name like '%" + textBox1.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //初始化
        private void Form10_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 42 / 100;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 30 / 100;//功能鍵高，比例法，離上方往下182
            //下拉選項
            Class1.DropDownList_A("Order_No", "goods_Name", "Cus_Order", CB1, "where goods_Name<>''");
            Class1.DropDownList_B("goods_No", "goods_Basic", comboBox1, "where goods_Name<>''");
            //this.comboBox1.SelectedIndex = 0; this.CB1.SelectedIndex = 0;
            button2_Click(sender, e);//查詢
            button7_Click(sender, e);//清除            
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //訂單編號
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["工令單號"].Value.ToString().Trim();
            this.label_No.Text = this.txt1.Text;
            //工單數量
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["工單數量"].Value.ToString().Trim();
            this.label3.Text = this.txt2.Text;
            //製品名稱
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品編號"].Value.ToString().Trim();
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品名稱"].Value.ToString().Trim();
            //轉出方式//下拉選項
            string turn_type = dgvDetail2.Rows[e.RowIndex].Cells["轉出方式"].Value.ToString().Trim();
            if (turn_type.Trim() != "")
            {
                this.comboBox3.SelectedIndex = Convert.ToInt32(turn_type.Trim().Substring(0,1));
            }
            //每次轉出數量
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["每次轉出數量"].Value.ToString().Trim();
            //this.item.Text = this.txt7.Text;            
            //轉出頻率
            this.txt15.Text = dgvDetail2.Rows[e.RowIndex].Cells["轉出頻率"].Value.ToString().Trim();
            //品檢方式
            this.comboBox2.SelectedIndex = Convert.ToInt32(dgvDetail2.Rows[e.RowIndex].Cells["品檢方式"].Value.ToString().Substring(0, 1));
            //直轉/入庫
            this.txt16.Text = dgvDetail2.Rows[e.RowIndex].Cells["直轉/入庫"].Value.ToString().Trim();
            //後製程
            this.txt18.Text = dgvDetail2.Rows[e.RowIndex].Cells["後製程"].Value.ToString().Trim();
            //異動人員
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動人員"].Value.ToString().Trim();
            //異動時間
            this.txt10.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動時間"].Value.ToString().Trim();
            //this.dateTimePicker1.Text = dgvDetail2.Rows[e.RowIndex].Cells["發料時間"].Value.ToString().Trim();
            //預計到料時間
            //this.dateTimePicker2.Text = dgvDetail2.Rows[e.RowIndex].Cells["預計到料時間"].Value.ToString().Trim();
            //this.txt17.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["預計到料時間"].Value).ToString("HH:mm");
            //this.checkBox1.Checked = (dgvDetail2.Rows[e.RowIndex].Cells["是否拆單"].Value.ToString().Trim() == "Y") ? true : false;
            //單位
            //this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["單位"].Value.ToString().Trim();
            //下拉選單
            //comboBox2.SelectedItem = dgvDetail2.Rows[e.RowIndex].Cells["內製/外購"].Value.ToString().Trim();                                                
            //this.CB1.SelectedValue = (this.txt1.Text.Trim() != "") ? txt1.Text.Trim() : "";
            Form_Detail_Query();
        }
        //查詢
        private void button3_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "")//工令單號
                where_A = " and A.Work_No like '%" + txt1.Text.Trim() + "%'";

            if (this.txt2.Text.Trim() != "")//工令單號
                where_A = " and A.Work_Qty = " + txt2.Text.Trim() + "";

            if (this.txt5.Text.Trim() != "")//製品編號
                where_A = where_A + " and A.goods_No like '%" + txt3.Text.Trim() + "%'";

            if (this.txt6.Text.Trim() != "")//名稱
                where_A = where_A + " and A.goods_Name like '%" + txt5.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button7_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt2.Text = ""; this.txt3.Text = ""; this.txt4.Text = "";
            this.txt5.Text = ""; this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = "";
            this.txt9.Text = ""; this.txt10.Text = ""; this.txt11.Text = "";
            this.txt14.Text = "12:00"; this.txt17.Text = "12:00";
            this.txt14.BackColor = Color.White; this.txt17.BackColor = Color.White;
            this.txt12.Text = ""; this.txt15.Text = ""; this.txt16.Text = "";
            this.txt18.Text = "";
            this.comboBox1.SelectedIndex = 0; this.textBox1.Text = "";
            this.comboBox2.SelectedIndex = 0; this.comboBox3.SelectedIndex = 0;
            this.CB1.SelectedIndex = 0; this.CB2.SelectedIndex = 0;
            //日期元件
            this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
            this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
            this.checkBox1.Checked = false;
            this.label_No.Text = ""; this.label3.Text = ""; this.label4.Text = ""; this.item.Text = "";//key
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工令單號』資料...");
                return false;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工單數量』資料...");
                return false;
            }
            int m;
            if (txt2.Text.Trim() != "")
            {
                if (Int32.TryParse(txt2.Text.Trim(), out m) == false)
                {
                    MessageBox.Show("工單數量請輸入『數字』資料...");
                    return false;
                }
            }
            if (txt3.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製品編號』資料...");
                return false;
            }
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製品名稱』資料...");
                return false;
            }
            if (this.comboBox3.SelectedItem.ToString().Trim() == "")//轉出方式
            {
                MessageBox.Show("請先選擇『轉出方式』資料...");
                return false;
            }
            //判斷是否為數字
            int n;
            if (txt7.Text.Trim() != "" && txt7.Text.Trim() != "---")
            {
                if (Int32.TryParse(txt7.Text.Trim(), out n) == false)
                {
                    MessageBox.Show("每次轉出數量請輸入『數字』資料...");
                    return false;
                }
            }
            if (this.comboBox2.SelectedItem.ToString().Trim() == "")//品檢方式
            {
                MessageBox.Show("請先選擇『品檢方式』資料...");
                return false;
            }
            if (txt16.Text.Trim() == "")//直轉/入庫
            {
                MessageBox.Show("請先輸入『直轉/入庫』資料...");
                return false;
            }                        
            //判斷日期
            //if (this.dateTimePicker1.Value.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
            //{
            //    if (MessageBox.Show(this, "請先確認『發料時間』為【今日】嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.No)
            //    {
            //        MessageBox.Show("請先選擇『預計出貨日期』資料...");
            //        return false;
            //    }
            //}
            //if (this.dateTimePicker2.Value.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
            //{
            //    if (MessageBox.Show(this, "請先確認『預計到料時間』為【今日】嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.No)
            //    {
            //        MessageBox.Show("請先選擇『預計出貨日期』資料...");
            //        return false;
            //    }
            //}
            //if (this.comboBox2.SelectedItem.ToString().Trim() == "")
            //{
            //    MessageBox.Show("請先選擇『內製/外購』...");
            //    return false;
            //}
            //if (this.comboBox3.SelectedItem.ToString().Trim() == "")
            //{
            //    MessageBox.Show("請先選擇『直轉/入庫』...");
            //    return false;
            //}
            return isOK;
        }
        //新增
        private void button4_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //檢查是否重複
            //if (Class1.GetRowCount("MoveWork_Master", "Order_No='" + txt1.Text.Trim() + "' and Work_No='" + txt2.Text.Trim() + "' and Issue_No='" + txt12.Text.Trim() + "' and Item='" + txt11.Text.Trim() + "'") > 0)
            //{
            //    MessageBox.Show("請注意此筆『銷售訂單編號/發料單號/工令單號/項次編號：" + txt1.Text.Trim() + "/" + txt12.Text.Trim() + "/" + txt2.Text.Trim() + "/" + txt11.Text.Trim() + "』資料，已存在...");
            //    return;
            //}
            if (Class1.GetRowCount("MoveWork_Master", "Work_No='" + txt1.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意此筆『工令單號：" + txt1.Text.Trim() + "』資料，已存在...");
                return;
            }
            //string isYorN = (this.checkBox1.Checked == true) ? "Y" : "N";//是否拆單
            //Insert
            SqlStr = "Insert into [MoveWork_Master]"
            + " (Work_No, goods_No, goods_Name, Work_Qty, Turn_Type, TurnOneTime, Frequency, QC_Type, Tnru_Next, Next_Work, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt3.Text.Trim() + "',"
            + "'" + txt5.Text.Trim() + "',"
            + "" + txt2.Text.Trim() + ","
            + "'" + this.comboBox3.SelectedItem.ToString().Substring(0, 1) + "',"//Turn_Type
            + "'" + txt7.Text.Trim() + "',"
            + "'" + txt15.Text.Trim() + "',"
            + "'" + this.comboBox2.SelectedItem.ToString().Substring(0, 1) + "',"//QC_Type
            //+ "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"//IssueTime
            + "'" + txt16.Text.Trim() + "',"//Tnru_Next            
            + "'" + txt18.Text.Trim() + "',"//Next_Work
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
                MessageBox.Show("請先輸入欲刪除之『工令單號』...");
                return;
            } 
            if (Class1.GetRowCount("MoveWork_Master", "Work_No='" + txt1.Text.Trim() + "'") == 0)
            {
                MessageBox.Show("請注意此筆『工令單號：" + txt1.Text.Trim() + "』資料，不存在，刪除失敗...");
                return;
            }                       
            //if (txt11.Text.Trim() == "")
            //{
            //    MessageBox.Show("請先輸入欲刪除之『項次編號』...");
            //    return;
            //}
            if (MessageBox.Show(this, "確定要刪除『工令單號：" + txt1.Text.Trim() + "』的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from MoveWork_Master"
                + " where Work_No='" + txt1.Text.Trim() + "'";
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
            //string isYorN = (this.checkBox1.Checked == true) ? "Y" : "N";//是否拆單
            //每次轉出數量
            string turn1time = (this.txt7.Text.Trim() == "") ? "NULL" : this.txt7.Text.Trim();
            //update
            SqlStr = "update [MoveWork_Master] set "
            + "Work_No = '" + txt1.Text.Trim() + "',"
            + "Work_Qty = " + txt2.Text.Trim() + ","
            + "goods_No = '" + txt3.Text.Trim() + "',"
            + "goods_Name = '" + txt5.Text.Trim() + "',"
            + "Turn_Type = '" + this.comboBox3.SelectedItem.ToString().Substring(0, 1) + "',"//轉出方式
            + "TurnOneTime = '" + turn1time + "',"//每次轉出數量
            + "Frequency = '" + txt15.Text.Trim() + "',"
            + "QC_Type = '" + this.comboBox2.SelectedItem.ToString().Substring(0, 1) + "',"//QC方式
            + "Tnru_Next = '" + txt16.Text.Trim() + "',"
            + "Next_Work = '" + txt18.Text.Trim() + "',"
            //時間
            //+ "Expect_Arrive = '" + Convert.ToDateTime(dateTimePicker2.Value).ToString("yyyy-MM-dd") + " " + txt17.Text.Trim() + ":00',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where Work_No='" + this.label_No.Text.Trim() + "'";//and Work_No='" + this.label3.Text.Trim() + "' and Issue_No='" + this.label4.Text.Trim() + "' and Item='" + this.item.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form10_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }

        private void CB1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txt1.Text = CB1.SelectedValue.ToString();
            //訂單抓工單
            Class1.DropDownList_A("Work_No", "goods_Name", "ProductWork", CB2, "where Order_No = '" + CB1.SelectedValue.ToString() + "'");
            //抓訂單明細show左下角
            SqlStr = "Select A.Order_No as '銷售訂單編號',"
            + "A.Cus_Name as '客戶名稱',"
            + "A.Cus_OrderNo as '客戶訂單編號',"
            + "A.goods_Name as '製品名稱',"
            + "A.goods_No as '製品編號',"
            + "A.goods_Spec as '製品規格',"
            + "A.Order_num as '訂單數量',"
            + "convert(varchar,A.Ship_Date,111) as '預計出貨日期',"
            + "A.Note as '備註',"
            + "A.IsClose as '完工狀態',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from Cus_Order A"
            + " where A.Order_No = '" + CB1.SelectedValue.ToString() + "'";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgv3.DataSource = dt;
            //抓工單明細檔
            SqlStr = "Select A.Work_No as '工單單號',"
                + "A.Item as '項次',"
                + "A.Move_No as '移轉單號',"
                + "A.Move_Time as '轉出時間',"
                + "A.Move_Qty as '轉出數量',"
                + "A.Status as '狀態'"
                + " from MoveWork_Detail A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
                + " where A.Work_No = '" + this.txt1.Text.Trim() + "'";// and A.Work_No = '" + CB2.SelectedValue.ToString() + "'";
            SqlStr = SqlStr + " order by A.Item asc";
            dt = Class1.GetDataTable(SqlStr);
            dgv2.DataSource = dt;
            dgv2.Columns[0].Width = 100; dgv2.Columns[1].Width = 60; dgv2.Columns[2].Width = 100; dgv2.Columns[3].Width = 160;
            dgv2.Columns[4].Width = 80; dgv2.Columns[5].Width = 80;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.txt12.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『發料單號』的資料，再取號...");
                return;
            }
            //取號
            string item_No = Class1.GetValue("count(*)", "Issue", "Issue_No = '" + this.txt12.Text + "'");
            item_No = (Convert.ToInt16(item_No) + 1).ToString();
            this.txt11.Text = item_No.Trim();
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

        private void CB2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txt2.Text = CB2.SelectedValue.ToString();
            
            this.txt13.Text = Class1.GetValue("Work_Qty", "ProductWork", "Work_No='" + CB2.SelectedValue.ToString() + "'");
            this.txt3.Text = Class1.GetValue("goods_No", "goods_Basic", "goods_No='" + CB2.SelectedValue.ToString() + "'");
            this.txt5.Text = Class1.GetValue("goods_Name", "goods_Basic", "goods_No='" + CB2.SelectedValue.ToString() + "'");
            this.txt6.Text = Class1.GetValue("goods_Spec", "goods_Basic", "goods_No='" + CB2.SelectedValue.ToString() + "'");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.txt1.Text = (CB1.SelectedValue.ToString().Trim() != "") ? CB1.SelectedValue.ToString().Trim() : "";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.txt9.Text = Class1.GetValue("Unit", "ProductWork", "Order_No='" + CB1.SelectedValue.ToString() + "' and Work_No='" + CB2.SelectedValue.ToString() + "'");
            //this.txt3.Text = (CB2.SelectedValue.ToString().Trim() != "") ? CB2.SelectedValue.ToString().Trim() : "";
            this.txt3.Text = Class1.GetValue("goods_No", "ProductWork", "Order_No='" + CB1.SelectedValue.ToString() + "' and Work_No='" + CB2.SelectedValue.ToString() + "'");
            this.txt5.Text = Class1.GetValue("goods_name", "ProductWork", "Order_No='" + CB1.SelectedValue.ToString() + "' and Work_No='" + CB2.SelectedValue.ToString() + "'");
            this.txt6.Text = Class1.GetValue("goods_Spec", "ProductWork", "Order_No='" + CB1.SelectedValue.ToString() + "' and Work_No='" + CB2.SelectedValue.ToString() + "'");
            this.txt7.Text = Class1.GetValue("Work_Qty", "ProductWork", "Order_No='" + CB1.SelectedValue.ToString() + "' and Work_No='" + CB2.SelectedValue.ToString() + "'");
            this.txt7.Focus();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string ss = "M" + DateTime.Now.ToString("yyyyMMdd");
            //取號
            string msNo = Class1.GetValue("count(*)", "Issue", "Issue_No like '" + ss + "'");
            msNo = "M" + DateTime.Now.ToString("yyyyMMdd") + (Convert.ToInt16(msNo) + 1).ToString("0000");
            this.txt12.Text = msNo.Trim();
        }

        private void txt16_Enter(object sender, EventArgs e)
        {
            if (txt7.Text.Trim() != "" && txt15.Text.Trim() != "")
            {
                this.txt16.Text = (Convert.ToInt16(txt7.Text) - Convert.ToInt16(txt15.Text)).ToString();
                this.txt14.Text = "12:00"; this.txt17.Text = "12:00";
                this.txt18.Text = "無";
                this.txt14.BackColor = Color.Yellow;
                this.txt17.BackColor = Color.Yellow;
            }
        }

        private void txt7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                txt15.Focus();
        }

        private void txt15_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                txt16.Focus();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //取號
            string item = Class1.GetValue("count(*)", "MoveWork_Detail", "Work_No = '" + txt1.Text.Trim() + "'");
            item = (Convert.ToInt16(item) + 1).ToString();
            this.txt11.Text = item;
            this.txt6.Text = txt1.Text.Trim() + "-" + item.PadLeft(3, '0');
            this.dateTimePicker1.Enabled = true; this.txt14.Enabled = true;
            this.dateTimePicker1.Value = DateTime.Now;
            this.txt14.Text = DateTime.Now.ToString("HH:mm");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (txt11.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工令單號』資料...");
                return;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『移轉單號』資料...");
                return;
            }
            if (txt9.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『狀態』資料...");
                return;
            }
            int m;
            if (txt13.Text.Trim() != "")
            {
                if (Int32.TryParse(txt13.Text.Trim(), out m) == false)
                {
                    MessageBox.Show("移轉數量請輸入『數字』資料...");
                    return;
                }
            }
            //Insert
            SqlStr = "Insert into [MoveWork_Detail]"
            + " (Work_No, Item, Move_No, Move_Time, Move_Qty, Status, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt11.Text.Trim() + "',"
            + "'" + txt6.Text.Trim() + "',"
            + "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"//Move_Time
            + "" + txt13.Text.Trim() + ","//Move_Qty            
            + "'" + txt9.Text.Trim() + "',"//Status
            + "'" + Loginfm.id.Trim() + "',"
            + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + ")";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("新增資料完成...");
            //Query
            Form_Detail_Query();
        }
        private void Form_Detail_Query()
        {
            SqlStr = "Select "
            + "A.Work_No as '工令單號',"
            + "A.Item as '項次',"
            + "A.Move_No as '移轉單號',"
            + "substring(convert(varchar,A.Move_Time,121),1,16) as '轉出時間',"
            + "A.Move_Qty as '轉出數量',"
            + "A.Status as '狀態',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from MoveWork_Detail A"
            + " where A.Work_No = '" + label_No.Text.Trim() + "'";
            SqlStr = SqlStr + " order by A.Item asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgv2.DataSource = dt;
            dgv2.Columns[0].Width = 80; dgv2.Columns[1].Width = 60; dgv2.Columns[2].Width = 100; dgv2.Columns[3].Width = 160;
            dgv2.Columns[4].Width = 80; dgv2.Columns[5].Width = 60; dgv2.Columns[6].Width = 80; dgv2.Columns[7].Width = 120;
        }

        private void dgv2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //工令單號
            //this.txt1.Text = dgv2.Rows[e.RowIndex].Cells["工令單號"].Value.ToString().Trim();
            //this.label_No.Text = this.txt1.Text;
            this.txt11.Text = dgv2.Rows[e.RowIndex].Cells["項次"].Value.ToString().Trim();
            this.item.Text = txt11.Text.Trim();
            this.txt6.Text = dgv2.Rows[e.RowIndex].Cells["移轉單號"].Value.ToString().Trim();
            //日期時間
            if (dgv2.Rows[e.RowIndex].Cells["轉出時間"].Value.ToString().Trim() == "---")
            {
                this.dateTimePicker1.Enabled = false; this.txt14.Enabled = false;
                this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
            }
            else
            {
                this.dateTimePicker1.Enabled = true; this.txt14.Enabled = true;
                this.dateTimePicker1.Text = dgv2.Rows[e.RowIndex].Cells["轉出時間"].Value.ToString().Trim();
                if (dgv2.Rows[e.RowIndex].Cells["轉出時間"].Value == DBNull.Value)
                    this.txt14.Text = "";
                else
                    this.txt14.Text = Convert.ToDateTime(dgv2.Rows[e.RowIndex].Cells["轉出時間"].Value).ToString("HH:mm");
            }
            this.txt13.Text = dgv2.Rows[e.RowIndex].Cells["轉出數量"].Value.ToString().Trim();
            //狀態
            this.txt9.Text = dgv2.Rows[e.RowIndex].Cells["狀態"].Value.ToString().Trim();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (txt11.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工令單號』資料...");
                return;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『移轉單號』資料...");
                return;
            }
            if (txt9.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『狀態』資料...");
                return;
            }
            int m;
            if (txt13.Text.Trim() != "")
            {
                if (Int32.TryParse(txt13.Text.Trim(), out m) == false)
                {
                    MessageBox.Show("移轉數量請輸入『數字』資料...");
                    return;
                }
            }
            //update
            SqlStr = "update [MoveWork_Detail] set "
            + "Move_No = '" + txt6.Text.Trim() + "',"
            + "Move_Time = '" + Convert.ToDateTime(dateTimePicker1.Value).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"
            + "Move_Qty = " + txt13.Text.Trim() + ","
            + "Status = '" + txt9.Text.Trim() + "',"
            //時間
            //+ "Expect_Arrive = '" + Convert.ToDateTime(dateTimePicker2.Value).ToString("yyyy-MM-dd") + " " + txt17.Text.Trim() + ":00',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where Work_No='" + this.label_No.Text.Trim() + "' and Item='" + this.item.Text.Trim() + "'"; //and Work_No='" + this.label3.Text.Trim() + "' and Issue_No='" + this.label4.Text.Trim() + "' and Item='" + this.item.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            Form_Detail_Query();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『工令單號』的資料...");
                return;
            }
            if (txt11.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『項次』的資料...");
                return;
            }
            if (Class1.GetRowCount("MoveWork_Detail", "Work_No='" + txt1.Text.Trim() + "' and Item='" + txt11.Text.Trim() + "'") == 0)
            {
                MessageBox.Show("請注意此筆『工令單號/項次：" + txt1.Text.Trim() + "/" + txt11.Text.Trim() + "』資料，不存在，刪除失敗...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『工令單號/項次：" + txt1.Text.Trim() + "/" + txt11.Text.Trim() + "』的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from MoveWork_Detail"
                + " where Work_No='" + txt1.Text.Trim() + "'"
                + " and item = '" + txt11.Text.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成，請重新查詢...");
                Form_Detail_Query();
            }
        }
    }
}
