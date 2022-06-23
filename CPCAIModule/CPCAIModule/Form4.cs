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
    public partial class Form4 : Form
    {
        string SqlStr = "";
        //共用變數
        public Form4()
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
            SqlStr = "Select A.Order_No as '銷售訂單編號',"
            + "A.Backup_No as '備料單號',"
            + "A.goods_No as '製品編號',"
            + "A.goods_Name as '製品名稱',"
            + "A.goods_Spec as '製品規格',"
            + "A.Qty as '單位用量',"
            + "A.Unit as '單位',"
            //+ "convert(varchar,A.Ship_Date,111) as '預計出貨日期',"
            + "A.Note as '備註',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from Backup_List A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by SDate asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 10;//總共0-9共10個欄位
            int[] column_width = { 110, 100, 120, 200, 200, 80, 80, 250, 80, 160 };//欄寬值
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
        private void Form4_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 8 / 24;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 8 / 35;//功能鍵高，比例法，離上方往下182
            //下拉選項            
            Class1.DropDownList_A("goods_No", "goods_Name", "goods_Basic", CB1, "where goods_Name<>''");
            Class1.DropDownList_B("goods_No", "goods_Basic", comboBox1, "where goods_Name<>''");
            this.comboBox1.SelectedIndex = 0; this.CB1.SelectedIndex = 0;
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
            //產品階層
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品編號"].Value.ToString().Trim();
            this.label4.Text = this.txt3.Text;
            //產品規格
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["備註"].Value.ToString().Trim();
            //製品名稱
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品名稱"].Value.ToString().Trim();
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["製品規格"].Value.ToString().Trim();
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["單位用量"].Value.ToString().Trim();
            //this.dateTimePicker1.Text = dgvDetail2.Rows[e.RowIndex].Cells["預計出貨日期"].Value.ToString().Trim();
            this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["單位"].Value.ToString().Trim();
            //異動人員
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動人員"].Value.ToString().Trim();
            //異動時間
            this.txt10.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動時間"].Value.ToString().Trim();
            //發料方式//下拉選項
            string goodsNo = dgvDetail2.Rows[e.RowIndex].Cells["製品編號"].Value.ToString().Trim();
            if (goodsNo.Trim() != "")
            {
                this.CB1.SelectedValue = goodsNo.Trim();
            }
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
            this.txt9.Text = ""; this.txt10.Text = ""; 
            this.comboBox1.SelectedIndex = 0; this.textBox1.Text = "";
            dateTimePicker1.Text = "";
            this.CB1.SelectedIndex = 0;
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
            //判斷是否為數字
            int n;
            if (Int32.TryParse(txt7.Text.Trim(), out n) == false)
            {
                MessageBox.Show("單位用量請輸入『數字』資料...");
                return false;
            }
            //if (this.dateTimePicker1.Value.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
            //{
            //    if (MessageBox.Show(this, "請先確認『預計出貨日期』為【今日】嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.No)
            //    { 
            //        MessageBox.Show("請先選擇『預計出貨日期』資料...");
            //        return false;
            //    }
                
            //}
            if (CB1.SelectedItem.ToString().Trim() == "")
            {
                MessageBox.Show("請先選擇『製品資料』...");
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
            //+ "'" + Convert.ToDateTime(this.dateTimePicker1.Text).ToString("yyyy-MM-dd HH:mm:ss") + "',"
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
            if (Class1.GetRowCount("Backup_List", "Order_No='" + txt1.Text.Trim() + "' and Backup_No='" + txt2.Text.Trim() + "' and goods_No='" + txt3.Text.Trim() + "'") == 0)
            {
                MessageBox.Show("請注意此筆『銷售訂單編號/備料單號/製品編號：" + txt1.Text.Trim() + "/" + txt2.Text.Trim() + "/" + txt3.Text.Trim() + "』資料，不存在...");
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
            if (txt3.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『製品編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『銷售訂單編號/備料單號/製品編號：" + txt1.Text.Trim() + "/" + txt2.Text.Trim() + "/" + txt3.Text.Trim() + "』的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Backup_List"
                + " where Order_No='" + txt1.Text.Trim() + "' and Backup_No='" + txt2.Text.Trim() + "' and goods_No='" + txt3.Text.Trim() + "'";
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
            SqlStr = "update [Backup_List] set "
            + "Order_No = '" + txt1.Text.Trim() + "',"
            + "Backup_No = '" + txt2.Text.Trim() + "',"
            + "goods_No = '" + txt3.Text.Trim() + "',"
            + "goods_Name = '" + txt5.Text.Trim() + "',"
            + "goods_Spec = '" + txt6.Text.Trim() + "',"
            + "Qty = " + txt7.Text.Trim() + ","
            + "Unit = '" + txt9.Text.Trim() + "',"
            + "Note = '" + txt4.Text.Trim() + "',"                      
            //+ "Ship_Date = '" + Convert.ToDateTime(dateTimePicker1.Value).ToString("yyyy-MM-dd HH:mm:ss") + "',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where Order_No='" + this.label_No.Text.Trim() + "' and Backup_No='" + this.label3.Text.Trim() + "' and goods_No='" + this.label4.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }

        private void CB1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txt3.Text = CB1.SelectedValue.ToString();
            this.txt5.Text = Class1.GetValue("goods_Name", "goods_Basic", "goods_No='" + CB1.SelectedValue.ToString() + "'");
            this.txt6.Text = Class1.GetValue("goods_Spec", "goods_Basic", "goods_No='" + CB1.SelectedValue.ToString() + "'");
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
    }
}
