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
    public partial class Form7 : Form
    {
        string SqlStr = "";
        //共用變數
        public Form7()
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
            SqlStr = "Select A.Order_No as '銷售訂單編號',"
            + "A.Work_No as '工令單號',"
            + "A.Issue_No as '發料單號',"
            + "A.Item as '項次',"
            + "A.goods_No as '製品/半成品/物料編號',"
            + "A.goods_Name as '製品/半成品/物料名稱',"
            + "A.goods_Spec as '規格',"
            + "A.Work_Qty as '工單數量',"
            + "substring(convert(varchar,A.IssueTime,121),1,16) as '發料時間',"
            + "A.TurnNext as '直轉/入庫',"
            + "A.Next_Work as '下工程',"
            + "A.Need_Qty as '需求量',"
            + "A.Issue_Qty as '發料數量',"
            + "A.Lack_Qty as '欠料數量',"
            + "A.Unit as '單位',"
            + "A.WorkSelf_Or_Buy as '內製/外購',"
            + "substring(convert(varchar,A.Expect_Arrive,121),1,16) as '預計到料時間',"
            + "A.Note as '備註',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from Issue A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by A.Order_No,A.Work_No,A.Issue_No,A.Item asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 20;//總共0-9共10個欄位
            int[] column_width = { 110, 100, 120, 60, 150, 200, 200, 80, 100, 100, 80, 80, 80, 80, 60, 100, 120, 250, 80, 160 };//欄寬值
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
        private void Form7_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 56 / 100;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 45 / 100;//功能鍵高，比例法，離上方往下182
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
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["銷售訂單編號"].Value.ToString().Trim();
            this.label_No.Text = this.txt1.Text;
            //工令單號
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["工令單號"].Value.ToString().Trim();
            this.label3.Text = this.txt2.Text;
            //發料單號
            this.txt12.Text = dgvDetail2.Rows[e.RowIndex].Cells["發料單號"].Value.ToString().Trim();
            this.label4.Text = this.txt12.Text;
            //項次
            this.txt11.Text = dgvDetail2.Rows[e.RowIndex].Cells["項次"].Value.ToString().Trim();
            this.item.Text = this.txt11.Text;            
            //製品名稱
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品/半成品/物料編號"].Value.ToString().Trim();
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品/半成品/物料名稱"].Value.ToString().Trim();
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["規格"].Value.ToString().Trim();
            //工單數量
            this.txt13.Text = dgvDetail2.Rows[e.RowIndex].Cells["工單數量"].Value.ToString().Trim();
            //需求量
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["需求量"].Value.ToString().Trim();
            this.txt15.Text = dgvDetail2.Rows[e.RowIndex].Cells["發料數量"].Value.ToString().Trim();
            this.txt16.Text = dgvDetail2.Rows[e.RowIndex].Cells["欠料數量"].Value.ToString().Trim();
            //發料時間
            this.dateTimePicker1.Text = dgvDetail2.Rows[e.RowIndex].Cells["發料時間"].Value.ToString().Trim();
            this.txt14.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["發料時間"].Value).ToString("HH:mm");
            //預計到料時間
            this.dateTimePicker2.Text = dgvDetail2.Rows[e.RowIndex].Cells["預計到料時間"].Value.ToString().Trim();
            this.txt17.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["預計到料時間"].Value).ToString("HH:mm");
            //this.checkBox1.Checked = (dgvDetail2.Rows[e.RowIndex].Cells["是否拆單"].Value.ToString().Trim() == "Y") ? true : false;
            //單位
            this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["單位"].Value.ToString().Trim();
            //下拉選單
            comboBox2.SelectedItem = dgvDetail2.Rows[e.RowIndex].Cells["內製/外購"].Value.ToString().Trim();
            comboBox3.SelectedItem = dgvDetail2.Rows[e.RowIndex].Cells["直轉/入庫"].Value.ToString().Trim();
            //下工程
            this.txt18.Text = dgvDetail2.Rows[e.RowIndex].Cells["下工程"].Value.ToString().Trim();
            //備註
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["備註"].Value.ToString().Trim();
            //異動人員
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動人員"].Value.ToString().Trim();
            //異動時間
            this.txt10.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動時間"].Value.ToString().Trim();
            //發料方式//下拉選項
            //string goodsNo = dgvDetail2.Rows[e.RowIndex].Cells["製品編號"].Value.ToString().Trim();
            //if (goodsNo.Trim() != "")
            //{
            //    this.CB2.SelectedValue = goodsNo.Trim();
            //}
            //this.CB1.SelectedValue = (this.txt1.Text.Trim() != "") ? txt1.Text.Trim() : "";
        }
        //查詢
        private void button3_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "")//訂單編號
                where_A = " and A.Order_No like '%" + txt1.Text.Trim() + "%'";

            if (this.txt2.Text.Trim() != "")//工令單號
                where_A = " and A.Work_No like '%" + txt2.Text.Trim() + "%'";

            if (this.txt5.Text.Trim() != "")//製品名稱
                where_A = where_A + " and A.goods_Name like '%" + txt5.Text.Trim() + "%'";

            if (this.txt6.Text.Trim() != "")//規格
                where_A = where_A + " and A.goods_Spec like '%" + txt6.Text.Trim() + "%'";

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
            //dateTimePicker1.Text = "";
            this.checkBox1.Checked = false;
            this.label_No.Text = ""; this.label3.Text = ""; this.label4.Text = ""; this.item.Text = "";//key
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『銷售訂單編號』資料...");
                return false;
            }            
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工令單號』資料...");
                return false;
            }
            if (txt12.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『發料單號』資料...");
                return false;
            }
            if (txt11.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『項次編號』資料...");
                return false;
            }
            if (txt3.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製品/半成品/物料編號』資料...");
                return false;
            }
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製品/半成品/物料名稱』資料...");
                return false;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製品規格』資料...");
                return false;
            }
            if (txt7.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『需求量』資料...");
                return false;
            }
            if (txt13.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工單數量』資料...");
                return false;
            }
            if (txt15.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『發料數量』資料...");
                return false;
            }
            if (txt16.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『欠料數量』資料...");
                return false;
            }
            //判斷是否為數字
            int n;
            if (Int32.TryParse(txt7.Text.Trim(), out n) == false)
            {
                MessageBox.Show("需求量請輸入『數字』資料...");
                return false;
            }
            if (Int32.TryParse(txt13.Text.Trim(), out n) == false)
            {
                MessageBox.Show("工單數量請輸入『數字』資料...");
                return false;
            }
            if (Int32.TryParse(txt15.Text.Trim(), out n) == false)
            {
                MessageBox.Show("發料數量請輸入『數字』資料...");
                return false;
            }
            if (Int32.TryParse(txt16.Text.Trim(), out n) == false)
            {
                MessageBox.Show("欠料數量請輸入『數字』資料...");
                return false;
            }
            //判斷日期
            if (this.dateTimePicker1.Value.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
            {
                if (MessageBox.Show(this, "請先確認『發料時間』為【今日】嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    MessageBox.Show("請先選擇『預計出貨日期』資料...");
                    return false;
                }
            }
            if (this.dateTimePicker2.Value.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
            {
                if (MessageBox.Show(this, "請先確認『預計到料時間』為【今日】嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    MessageBox.Show("請先選擇『預計出貨日期』資料...");
                    return false;
                }
            }
            if (this.comboBox2.SelectedItem.ToString().Trim() == "")
            {
                MessageBox.Show("請先選擇『內製/外購』...");
                return false;
            }
            if (this.comboBox3.SelectedItem.ToString().Trim() == "")
            {
                MessageBox.Show("請先選擇『直轉/入庫』...");
                return false;
            }
            if (txt9.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『單位』資料...");
                return false;
            }
            return isOK;
        }
        //新增
        private void button4_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("Issue", "Order_No='" + txt1.Text.Trim() + "' and Work_No='" + txt2.Text.Trim() + "' and Issue_No='" + txt12.Text.Trim() + "' and Item='" + txt11.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意此筆『銷售訂單編號/發料單號/工令單號/項次編號：" + txt1.Text.Trim() + "/" + txt12.Text.Trim() + "/" + txt2.Text.Trim() + "/" + txt11.Text.Trim() + "』資料，已存在...");
                return;
            }
            //string isYorN = (this.checkBox1.Checked == true) ? "Y" : "N";//是否拆單
            //Insert
            SqlStr = "Insert into [Issue]"
            + " (Order_No, Issue_No, Work_No, Item, goods_No, goods_Name, goods_Spec, IssueTime, Work_Qty, TurnNext, Next_Work, Need_Qty, Issue_Qty, Lack_Qty, Unit, WorkSelf_Or_Buy, Expect_Arrive, Note, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt12.Text.Trim() + "',"
            + "'" + txt2.Text.Trim() + "',"
            + "'" + txt11.Text.Trim() + "',"
            + "'" + txt3.Text.Trim() + "',"
            + "'" + txt5.Text.Trim() + "',"
            + "'" + txt6.Text.Trim() + "',"
            + "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"//IssueTime
            + "" + txt13.Text.Trim() + ","//Work_Qty
            + "'" + this.comboBox3.SelectedItem.ToString() + "',"//TurnNext
            + "'" + txt18.Text.Trim() + "',"//Next_Work
            + "" + txt7.Text.Trim() + ","//Need_Qty
            + "" + txt15.Text.Trim() + ","//Issue_Qty
            + "" + txt16.Text.Trim() + ","//Lack_Qty
            + "'" + txt9.Text.Trim() + "',"//Unit
            + "'" + this.comboBox2.SelectedItem.ToString() + "',"//WorkSelf_Or_Buy
            + "'" + Convert.ToDateTime(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " " + txt17.Text.Trim() + ":00',"//Expect_Arrive
            + "'" + txt4.Text.Trim() + "',"//Note
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
            if (Class1.GetRowCount("Issue", "Order_No='" + txt1.Text.Trim() + "' and Work_No='" + txt2.Text.Trim() + "' and Issue_No='" + txt12.Text.Trim() + "' and Item='" + txt11.Text.Trim() + "'") == 0)
            {
                MessageBox.Show("請注意此筆『銷售訂單編號/工令單號/發料單號/項次編號：" + txt1.Text.Trim() + "/" + txt2.Text.Trim() + "/" + txt12.Text.Trim() + "/" + txt11.Text.Trim() + "』資料，不存在...");
                return;
            }
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『銷售訂單編號』...");
                return;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『工令單號』...");
                return;
            }
            if (txt12.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『發料單號』...");
                return;
            }
            if (txt11.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『項次編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『銷售訂單編號/工令單號/發料單號/項次編號：" + txt1.Text.Trim() + "/" + txt2.Text.Trim() + "/" + txt12.Text.Trim() + "/" + txt11.Text.Trim() + "』的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Issue"
                + " where Order_No='" + txt1.Text.Trim() + "' and Work_No='" + txt2.Text.Trim() + "' and Issue_No='" + txt12.Text.Trim() + "' and Item='" + txt11.Text.Trim() + "'";
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
            //update
            SqlStr = "update [Issue] set "
            + "Order_No = '" + txt1.Text.Trim() + "',"
            + "Work_No = '" + txt2.Text.Trim() + "',"
            + "Issue_No = '" + txt12.Text.Trim() + "',"
            + "Item = '" + txt11.Text.Trim() + "',"
            + "goods_No = '" + txt3.Text.Trim() + "',"
            + "goods_Name = '" + txt5.Text.Trim() + "',"
            + "goods_Spec = '" + txt6.Text.Trim() + "',"
            + "Work_Qty = " + txt13.Text.Trim() + ","
            + "Unit = '" + txt9.Text.Trim() + "',"
            + "Need_Qty = " + txt7.Text.Trim() + ","
            + "Issue_Qty = " + txt15.Text.Trim() + ","
            + "Lack_Qty = " + txt16.Text.Trim() + ","
            //下拉選單
            + "TurnNext = '" + this.comboBox3.SelectedItem.ToString() + "',"
            + "WorkSelf_Or_Buy = '" + this.comboBox2.SelectedItem.ToString() + "',"
            + "Next_Work = '" + txt18.Text.Trim() + "',"
            + "Note = '" + txt4.Text.Trim() + "',"
            //兩個時間
            + "IssueTime = '" + Convert.ToDateTime(dateTimePicker1.Value).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00',"
            + "Expect_Arrive = '" + Convert.ToDateTime(dateTimePicker2.Value).ToString("yyyy-MM-dd") + " " + txt17.Text.Trim() + ":00',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where Order_No='" + this.label_No.Text.Trim() + "' and Work_No='" + this.label3.Text.Trim() + "' and Issue_No='" + this.label4.Text.Trim() + "' and Item='" + this.item.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form7_FormClosed(object sender, FormClosedEventArgs e)
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
            SqlStr = "Select A.Order_No as '銷售訂單編號',"
                + "A.Work_No as '工單單號',"
                + "A.Item as '項次',"
                + "A.goods_No as '製品/半製品編號',"
                + "A.goods_Name as '製品/半製品名稱',"
                + "A.goods_Spec as '製品/半製品規格',"
                + "A.Work_Qty as '生產量',"
                + "A.Unit as '單位',"
                + "A.IsApart as '是否拆單'"
                + " from ProductWork A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
                + " where A.Order_No = '" + CB1.SelectedValue.ToString() + "'";// and A.Work_No = '" + CB2.SelectedValue.ToString() + "'";
            SqlStr = SqlStr + " order by Item asc";
            dt = Class1.GetDataTable(SqlStr);
            dgv2.DataSource = dt;
            dgv2.Columns[2].Width = 50; dgv2.Columns[6].Width = 60; dgv2.Columns[7].Width = 50; dgv2.Columns[8].Width = 60;
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
    }
}
