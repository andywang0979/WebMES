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
    public partial class Form2 : Form
    {
        string SqlStr = "";
        //共用變數
        public Form2()
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
            SqlStr = "Select A.Item_order as '欄位排序',"
            + "A.Item_father as '上階項次',"
            + "A.Item_son as '現階項次',"
            + "A.goods_Name as '製品名稱',"
            + "A.goods_No as '製品編號',"
            + "A.Standard_timeSec as '標準工時(秒)',"
            + "A.Standard_Needmen as '標準作業人數',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from Standard_Worktime A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by Item_order";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 9;//總共0-13共14個欄位
            int[] column_width = { 80, 80, 80, 250, 150, 80, 80, 80, 160 };//欄寬值
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
                where_A = " and A.Item_father ='" + this.comboBox1.SelectedValue.ToString().Trim() + "'";

            if (this.textBox1.Text.Trim() != "")
                where_A = where_A + " and (A.goods_No like '%" + textBox1.Text.Trim() + "%' or A.goods_Name like '%" + textBox1.Text.Trim() + "%')";

            Form_Query(where_A);
        }
        //初始化
        private void Form2_Load(object sender, EventArgs e)
        {
            //this.splitContainer1.SplitterDistance = splitContainer1.Height - 520;//上方高，固定法
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 7 / 20;//上方高，比例法
            //下拉選項
            Class1.DropDownList_B("Item_son", "Standard_Worktime", comboBox1, "where Item_father=''");
            this.comboBox1.SelectedIndex = 0;
            button2_Click(sender, e);
        }
        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            this.label_No1.Text = dgvDetail2.Rows[e.RowIndex].Cells["欄位排序"].Value.ToString().Trim();
            //欄位
            string[] columnABC = new string[] { "上階項次", "現階項次", "製品名稱", "製品編號", "標準工時(秒)", "標準作業人數", "欄位排序", "異動人員", "異動時間" };//欄寬值
            //上階項次
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells[columnABC[0]].Value.ToString().Trim();
            //現階項次
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells[columnABC[1]].Value.ToString().Trim();
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells[columnABC[2]].Value.ToString().Trim();
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells[columnABC[3]].Value.ToString().Trim();
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells[columnABC[4]].Value.ToString().Trim();
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells[columnABC[5]].Value.ToString().Trim();
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells[columnABC[6]].Value.ToString().Trim();
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
            if (this.txt6.Text.Trim() != "")//料號
                where_A = " and A.goods_No like '%" + txt6.Text.Trim() + "%'";

            if (this.txt5.Text.Trim() != "")//名稱
                where_A = " and A.goods_Name = '" + txt5.Text.Trim() + "'";

            //if (this.txt6.Text.Trim() != "")//規格
            //    where_A = where_A + " and A.goods_Spec like '%" + txt6.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button7_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt2.Text = ""; this.txt3.Text = ""; this.txt4.Text = "";
            this.txt5.Text = ""; this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = "";
            this.txt9.Text = ""; this.txt10.Text = ""; 
            //this.comboBox1.SelectedIndex = 0; this.textBox1.Text = "";
            //this.CB1.SelectedIndex = 0;
            this.label_No1.Text = ""; this.label_No2.Text = "";
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            //if (txt1.Text.Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『物料編號』資料...");
            //    return false;
            //}            
            if (txt8.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『欄位排序』資料...");
                return false;
            }
            if (txt3.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『產品機型』資料...");
                return false;
            }
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『物料名稱』資料...");
                return false;
            }
            //if (txt6.Text.Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『物料規格』資料...");
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
            if (Class1.GetRowCount("Standard_Worktime", "Item_father='" + txt1.Text.Trim() + "' and Item_son='" + txt2.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『項次：" + txt1.Text.Trim() + "-" + txt2.Text.Trim() + "』資料，已存在...");
                return;
            }
            //Insert
            SqlStr = "Insert into [Standard_Worktime]"
            + " (Item_order, Item_father, Item_son, goods_Name, goods_No, Standard_timeSec, Standard_Needmen, ENo, SDate) "
            + "values ("
            + "'" + txt8.Text.Trim() + "',"
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt2.Text.Trim() + "',"
            + "'" + txt5.Text.Trim() + "',"
            + "'" + txt6.Text.Trim() + "',"
            + "'" + txt3.Text.Trim() + "',"
            + "'" + txt4.Text.Trim() + "',"
            //+ "'" + CB1.SelectedItem.ToString().Substring(0,1) + "',"
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
            if (txt1.Text.Trim() == "" || txt2.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入欲刪除之『上下階編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『階層編號』為:(" + this.txt1.Text.Trim() + "-" + this.txt2.Text.Trim() + ":" + this.txt5.Text.Trim() + ")的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from Standard_Worktime"
                + " where Item_order=" + this.txt8.Text.Trim() + "";
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
            SqlStr = "update [Standard_Worktime] set "
            + "Item_father = '" + txt1.Text.Trim() + "',"
            + "Item_son = '" + txt2.Text.Trim() + "',"
            + "Standard_timeSec = " + txt3.Text.Trim() + ","
            + "Standard_Needmen = '" + txt4.Text.Trim() + "',"
            + "goods_Name = '" + txt5.Text.Trim() + "',"
            + "goods_No = '" + txt6.Text.Trim() + "',"
                //+ "OutMaterial_Type = '" + CB1.SelectedItem.ToString().Trim().Substring(0,1) + "',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where Item_order = '" + this.label_No1.Text + "'";            
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }
    }
}
