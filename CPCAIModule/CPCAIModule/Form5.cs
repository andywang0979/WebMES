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
    public partial class Form5 : Form
    {
        string SqlStr = "";
        //共用變數
        public Form5()
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
            dgvDetail2.Columns[4].Frozen = true;//固定欄位
        }
        //查詢函式
        private void Form_Query(string where_str)
        {
            //PARSENAME('$'+ Convert(varchar,Convert(money,A.Pay),1),2) as '產品單價'//轉貨幣符號
            //+ "REPLACE(Convert(varchar,A.Price),'.00','') as '產品單價',"//取代
            SqlStr = "Select top 100 A.Order_No as '銷售訂單編號',"
            + "A.Backup_No as '備料單號',"
            + "A.Item as '項次',"
            + "A.goods_name as '製品名稱',"
            + "A.goods_No as '製品編號',"
            + "A.goods_Spec as '製品規格',"
            + "A.Qty as '需求量',"
            + "A.Now_Qty as '現有數量',"
            + "A.Need_Qty as '欠料數量',"
            + "A.Unit as '單位',"
            + "case when A.Output_Type = '1' then '一次發料' when A.Output_Type = '2' then '分批發料' when A.Output_Type = '3' then '現場領料' else '' end as '發料方式',"
            + "A.Output_Frequency as '發料頻率',"
            + "A.Output_QtyOneTime as '每次發料數量',"
            + "A.WorkSelf_Or_Buy as '內製或外購',"
            + "case when Convert(varchar,A.Expect_Arrive,111)='1900/01/01'"
            + " then '---' else substring(convert(varchar,A.Expect_Arrive,121),1,16) end as '預計到料時間',"
            + "A.Note as '備註',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from Backup_Hub A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by Order_No,Backup_No,Item asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 17;//總共0-16共17個欄位
            int[] column_width = { 110, 80, 40, 200, 100, 300, 70, 80, 80, 60, 80, 80, 110, 90, 110, 250, 80, 160 };//欄寬值
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
        private void Form5_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 13 / 25;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 15 / 36;//功能鍵高，比例法，離上方往下182
            //下拉選項            
            Class1.DropDownList_A("Order_No", "goods_Name", "Cus_Order", CB1, "where IsClose='N'");
            Class1.DropDownList_B("goods_No", "Backup_Hub", comboBox1, "where goods_Name<>''");
            if (comboBox1.Items.Count>0) this.comboBox1.SelectedIndex = 0;
            this.CB1.SelectedIndex = 0;
            button2_Click(sender, e);//查詢
            button7_Click(sender, e);//清除            
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //物料代碼
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["銷售訂單編號"].Value.ToString().Trim();
            this.label_No.Text = this.txt1.Text;
            //物料名稱
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["備料單號"].Value.ToString().Trim();
            this.label3.Text = this.txt2.Text;
            //項次
            this.txt11.Text = dgvDetail2.Rows[e.RowIndex].Cells["項次"].Value.ToString().Trim();
            this.label4.Text = dgvDetail2.Rows[e.RowIndex].Cells["項次"].Value.ToString().Trim();//key
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品編號"].Value.ToString().Trim();
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品名稱"].Value.ToString().Trim();
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品規格"].Value.ToString().Trim();
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["需求量"].Value.ToString().Trim();
            this.txt10.Text = dgvDetail2.Rows[e.RowIndex].Cells["單位"].Value.ToString().Trim();
            //現有數量
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["現有數量"].Value.ToString().Trim();
            this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["欠料數量"].Value.ToString().Trim();
            //發料頻率
            this.txt12.Text = dgvDetail2.Rows[e.RowIndex].Cells["發料頻率"].Value.ToString().Trim();
            if (txt12.Text.Trim() == "") txt12.Text = "1次/1hr";
            //發料方式//下拉選項
            string type = dgvDetail2.Rows[e.RowIndex].Cells["發料方式"].Value.ToString().Trim();
            if (type.Trim() != "")
            {
                if (type.Trim() == "一次發料")
                    this.CB3.SelectedIndex = 1;
                else if (type.Trim() == "分批發料")
                    this.CB3.SelectedIndex = 2;
                else if (type.Trim() == "現場領料")
                    this.CB3.SelectedIndex = 3;
                else
                    this.CB3.SelectedIndex = 0;
            }
            else
            {
                this.CB3.SelectedIndex = 0;
                this.txt12.Text = "";
            }
            this.txt13.Text = dgvDetail2.Rows[e.RowIndex].Cells["每次發料數量"].Value.ToString().Trim();
            this.CB4.SelectedItem = dgvDetail2.Rows[e.RowIndex].Cells["內製或外購"].Value.ToString().Trim();
            //日期時間
            if (dgvDetail2.Rows[e.RowIndex].Cells["預計到料時間"].Value.ToString().Trim() == "---")
            {
                this.dateTimePicker1.Enabled = false;
                this.txt14.Enabled = false;
                this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); 
                this.txt14.Text = "00:00";
            }
            else
            {
                this.dateTimePicker1.Text = dgvDetail2.Rows[e.RowIndex].Cells["預計到料時間"].Value.ToString().Trim();
                if (dgvDetail2.Rows[e.RowIndex].Cells["預計到料時間"].Value == DBNull.Value)
                    this.txt14.Text = "";
                else
                    this.txt14.Text = Convert.ToDateTime(dgvDetail2.Rows[e.RowIndex].Cells["預計到料時間"].Value).ToString("HH:mm");
            }
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["備註"].Value.ToString().Trim();
        }
        //查詢
        private void button3_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "")//訂單編號
                where_A = " and A.Order_No like '%" + txt1.Text.Trim() + "%'";

            if (this.txt2.Text.Trim() != "")//客戶名稱
                where_A = " and A.Backup_No like '%" + txt2.Text.Trim() + "%'";

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
            this.txt12.Text = ""; this.txt13.Text = ""; this.txt14.Text = "10:30";
            if (comboBox1.Items.Count > 0) this.comboBox1.SelectedIndex = 0;
            this.textBox1.Text = "";
            dateTimePicker1.Text = "";
            this.CB1.SelectedIndex = 0;
            this.CB2.SelectedIndex = 0;
            this.CB3.SelectedIndex = 0;
            this.CB4.SelectedIndex = 0;
            this.label_No.Text = "";
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
                MessageBox.Show("請先輸入『備料單號』資料...");
                return false;
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
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製品規格』資料...");
                return false;
            }
            if (txt7.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『單位用量』資料...");
                return false;
            }
            if (txt12.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『發料頻率』資料...");
                return false;
            }
            if (txt13.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『每次發料數量』資料...");
                return false;
            }
            if (txt14.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『時間』資料，例如:14:30");
                return false;
            }
            //下拉選項
            if (CB3.SelectedIndex <= 0)
            {
                MessageBox.Show("請先選擇『發料方式』資料...");
                return false;
            }
            if (CB4.SelectedIndex <= 0)
            {
                MessageBox.Show("請先選擇『內製/外購』資料...");
                return false;
            }
            //判斷是否為數字
            int n;
            if (Int32.TryParse(txt7.Text.Trim(), out n) == false)
            {
                MessageBox.Show("單位用量請輸入『數字』資料...");
                return false;
            }
            //判斷是否時間
            string timepart = "";
            if (txt14.Text.Trim() != "")
                timepart = txt14.Text.Trim() + ":00";
            if (Class1.IsDate(this.dateTimePicker1.Value.ToString("yyyy/MM/dd") + " " + timepart ) == false)
            {
                MessageBox.Show("請先輸入『正確時間格式』資料，例如:2020/10/10 14:30");
                return false;
            }
            
            if (this.dateTimePicker1.Value < DateTime.Now)
            {
                if (MessageBox.Show(this, "請先確認『預計到料日期』為【過去日期】嗎？若為現有庫存，請選擇今日日期或之前日期", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    MessageBox.Show("請更改選擇及輸入『預計到料日期』資料...");
                    return false;
                }                
            }
            if (CB3.SelectedItem.ToString().Trim() == "")
            {
                MessageBox.Show("請先選擇『發料方式』...");
                return false;
            }
            return isOK;
        }
        //新增
        private void button4_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("Backup_List", "Order_No='" + txt1.Text.Trim() + "' and Backup_No='" + txt2.Text.Trim() + "' and goods_No='" + txt3.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意此筆『銷售訂單編號/備料單號/製品編號：" + txt1.Text.Trim() + "/" + txt2.Text.Trim() + "/" + txt3.Text.Trim() + "』資料，已存在...");
                return;
            }
            //Insert
            SqlStr = "Insert into [Backup_List]"
            + " (Order_No, Backup_No, goods_No, goods_Name, goods_Spec, Qty, Unit, Note, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt2.Text.Trim() + "',"
            + "'" + txt3.Text.Trim() + "',"
            + "'" + txt5.Text.Trim() + "',"
            + "'" + txt6.Text.Trim() + "',"
            + "" + txt7.Text.Trim() + ","
            + "'" + txt9.Text.Trim() + "',"
            + "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd HH:mm:ss") + "',"
            + "'" + txt4.Text.Trim() + "',"
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
            if (Class1.GetRowCount("Backup_List", "Order_No='" + txt1.Text.Trim() + "' and Backup_No='" + txt2.Text.Trim() + "'") == 0)
            {
                MessageBox.Show("請注意此筆『銷售訂單編號/備料單號：" + txt1.Text.Trim() + "/" + txt2.Text.Trim() + "』資料，不存在...");
                return;
            }
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『銷售訂單編號』...");
                return;
            }
            if (txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『備料單號』...");
                return;
            }            
            if (MessageBox.Show(this, "確定要刪除『銷售訂單編號/備料單號：" + txt1.Text.Trim() + "/" + txt2.Text.Trim() + "』的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Backup_Hub"
                + " where Order_No='" + txt1.Text.Trim() + "' and Backup_No='" + txt2.Text.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成，請重新查詢...");
                button3_Click(sender, e);//查詢
            }
        }
        //修改
        private void button5_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            string arrivetime = "";
            if (dateTimePicker1.Enabled == false)
                arrivetime = null;
            else
                arrivetime = Convert.ToDateTime(dateTimePicker1.Value).ToString("yyyy-MM-dd") + " " + txt14.Text.Trim() + ":00";
            //update
            SqlStr = "update [Backup_Hub] set "
            + "Order_No = '" + txt1.Text.Trim() + "',"
            + "Backup_No = '" + txt2.Text.Trim() + "',"
            + "Item = '" + txt11.Text.Trim() + "',"
            + "goods_No = '" + txt3.Text.Trim() + "',"
            + "goods_Name = '" + txt5.Text.Trim() + "',"
            + "goods_Spec = '" + txt6.Text.Trim() + "',"
                //數字
            + "Qty = " + txt7.Text.Trim() + ","
            + "Now_Qty = " + txt8.Text.Trim() + ","
            + "Need_Qty = " + txt9.Text.Trim() + ","
            + "Output_QtyOneTime = " + txt13.Text.Trim() + ","
            + "Unit = '" + txt10.Text.Trim() + "',"
                //下拉選項
            + "Output_Type = '" + CB3.SelectedItem.ToString().Substring(0, 1) + "',"
            + "WorkSelf_Or_Buy = '" + CB4.SelectedItem.ToString().Trim() + "',"
            + "Output_Frequency = '" + txt12.Text.Trim() + "',"
            + "Note = '" + txt4.Text.Trim() + "',"
                //日期時間
            + "Expect_Arrive = '" + arrivetime + "',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where Order_No='" + this.label_No.Text.Trim() + "' and Backup_No='" + this.label3.Text.Trim() + "'"
            + " and Item = '" + this.label4.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form5_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }

        private void CB1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txt1.Text = CB1.SelectedValue.ToString();//訂單號
            //下拉選項            
            Class1.DropDownList_B("Backup_No", "Backup_List", CB2, "where Order_No='" + txt1.Text.Trim() + "'");
            //由訂單號抓製品
            this.txt3.Text = Class1.GetValue("goods_No", "goods_Basic", "goods_No='" + CB1.SelectedValue.ToString() + "'");
            this.txt5.Text = Class1.GetValue("goods_Name", "goods_Basic", "goods_No='" + CB1.SelectedValue.ToString() + "'");
            this.txt6.Text = Class1.GetValue("goods_Spec", "goods_Basic", "goods_No='" + CB1.SelectedValue.ToString() + "'");
            //抓訂單明細
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //取號
            string cus = Class1.GetValue("count(*)", "Cus_Order", "SUBSTRING(Order_No,1,8) = '" + DateTime.Now.ToString("yyyyMMdd") +"'");
            cus = (Convert.ToInt16(cus) + 1).ToString();
            this.txt1.Text = DateTime.Now.ToString("yyyyMMdd") + cus.PadLeft(3, '0');
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
            this.txt2.Text = CB2.SelectedValue.ToString();//備料單號
            if (this.CB1.SelectedIndex > 0 && this.CB2.SelectedIndex > 0)
            {
                this.button8.Enabled = true;//顯示轉備料清單
                SqlStr = "Select A.Order_No as '銷售訂單編號',"
                + "A.Backup_No as '備料單號',"
                + "A.goods_No as '製品編號',"
                + "A.goods_Name as '製品名稱',"
                + "A.goods_Spec as '製品規格',"
                + "A.Qty as '單位用量',"
                + "A.Unit as '單位'"                
                + " from Backup_List A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
                + " where A.Order_No = '" + CB1.SelectedValue.ToString() + "' and A.Backup_No = '" + CB2.SelectedValue.ToString() + "'";

                SqlStr = SqlStr + " order by SDate asc";
                System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
                dgv2.DataSource = dt;

                dgv2.Columns[0].Width = 110;
                dgv2.Columns[1].Width = 80;
                dgv2.Columns[2].Width = 80;
                dgv2.Columns[5].Width = 80;
                dgv2.Columns[6].Width = 40;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //抓訂單用量
            int order_qty = Convert.ToInt32(Class1.GetValue("Order_num", "Cus_Order", "Order_No='" + CB1.SelectedValue.ToString() + "'"));
            SqlStr = "Select A.* from Backup_List A"
            + " where A.Order_No = '" + CB1.SelectedValue.ToString() + "' and A.Backup_No = '" + CB2.SelectedValue.ToString() + "'"
            + " order by SDate asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            string SqlStrA = ""; int i = 1;
            foreach (DataRow row in dt.Rows)//迴圈
            {
                string orderno = row["Order_No"].ToString().Trim();
                string backupno = row["Backup_No"].ToString().Trim();
                string goodsno = row["goods_No"].ToString().Trim();
                int unit_qty = Convert.ToInt32(row["Qty"]);//單位用量                
                //檢查重複
                if (Class1.GetRowCount("Backup_Hub", "Order_No='" + orderno + "' and Backup_No='" + backupno + "' and goods_No='" + goodsno + "'") > 0)
                {
                    MessageBox.Show("請注意此筆『銷售訂單編號/備料單號/製品編號：" + orderno + "/" + backupno + "/" + goodsno + "』資料，已存在。轉入失敗...");
                    return;
                }
                //組合新增SQL
                SqlStrA = SqlStrA + "Insert into [Backup_Hub] (Order_No,Backup_No,Item,goods_No,goods_Name,goods_Spec,Qty,Unit,ENo,SDate)"
                + " values ("
                + "'" + orderno + "',"
                + "'" + backupno + "',"
                + "'" + i.ToString() + "',"
                + "'" + goodsno + "',"
                + "'" + row["goods_Name"].ToString().Trim() + "',"
                + "'" + row["goods_Spec"].ToString().Trim() + "',"
                + "" + order_qty * unit_qty + ","//需求量
                + "'" + row["Unit"].ToString().Trim() + "',"//單位
                + "'" + Loginfm.id.Trim() + "',"
                + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
                + ");";
                i++;
            }
            //新增
            Class1.Execute_SQL(SqlStrA);
            MessageBox.Show("新增資料完成...");
            button3_Click(sender, e);//查詢
        }

        private void txt8_Leave(object sender, EventArgs e)
        {
            int n;
            if (Int32.TryParse(txt8.Text.Trim(), out n) == false)
            {
                MessageBox.Show("現有數量請輸入『數字』資料...");
                return;
            }
            if (txt7.Text.Trim() != "" && txt8.Text.Trim() != "")
            {
                txt9.Text = (Convert.ToInt32(txt7.Text.Trim()) - Convert.ToInt32(txt8.Text.Trim())).ToString();
            }
            txt9.Focus();
        }

        private void txt13_Leave(object sender, EventArgs e)
        {
            //數字檢查
            int n;
            if (Int32.TryParse(txt13.Text.Trim(), out n) == false)
            {
                MessageBox.Show("每次發料數量請輸入『數字』資料...");
                return;
            }
        }

        private void txt9_Leave(object sender, EventArgs e)
        {
            //數字檢查
            int n;
            if (Int32.TryParse(txt9.Text.Trim(), out n) == false)
            {
                MessageBox.Show("欠料數量請輸入『數字』資料...");
                return;
            }
        }

        private void CB3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CB3.SelectedIndex == 3)
            {
                this.dateTimePicker1.Enabled = false;//不用日期的時候
                this.dateTimePicker1.Value = Convert.ToDateTime("1900/01/01"); this.txt14.Text = "00:00";
                this.txt14.Enabled = false;
            }
            else
            {
                this.dateTimePicker1.Enabled = true;
                this.txt14.Enabled = true;
            }
            this.txt12.Focus();
        }

        private void txt8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                txt9.Focus();
        }

        private void txt9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                CB4.Focus();
        }

        private void CB4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                txt13.Focus();
        }

        private void txt12_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                txt8.Focus();
        }

        private void txt4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                button5.Focus();
        }

        private void txt14_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                txt4.Focus();
        }

        private void txt13_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (dateTimePicker1.Enabled == true)
                    this.dateTimePicker1.Focus();
                else
                    txt4.Focus();
            }
        }

        private void dateTimePicker1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                txt14.Focus();
        }
    }
}
