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
    public partial class Form8 : Form
    {
        string SqlStr = "";
        //共用變數
        public Form8()
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
            + "A.I_Name as '治具名稱',"
            + "A.Brand as '品牌',"
            + "A.I_Spec as '規格',"
            + "A.Precision as '精度',"
            + "A.P_Unit as '精度單位',"
            + "A.Life_time as '使用壽命時間',"
            + "A.Used_time as '已使用時間',"
            + "A.U_Unit as '時間單位',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from Instrument A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by A.I_Name Asc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 11;//總共0-13共14個欄位
            int[] column_width = { 120, 200, 120, 200, 80, 100, 100, 100, 80,80, 160 };//欄寬值
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
                where_A = where_A + " and A.I_Name like '%" + textBox1.Text.Trim() + "%' or A.I_Spec='" + textBox1.Text.Trim() + "'";

            Form_Query(where_A);
        }
        //初始化
        private void Form1_Load(object sender, EventArgs e)
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
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["治具名稱"].Value.ToString().Trim();
            //產品階層
            this.txt2.Text = dgvDetail2.Rows[e.RowIndex].Cells["品牌"].Value.ToString().Trim();
            //產品規格
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["規格"].Value.ToString().Trim();
            //產品機型
            this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["精度"].Value.ToString().Trim();
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["精度單位"].Value.ToString().Trim();
            //壽命時間
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["使用壽命時間"].Value.ToString().Trim();
            this.txt10.Text = dgvDetail2.Rows[e.RowIndex].Cells["已使用時間"].Value.ToString().Trim();
            this.txt11.Text = dgvDetail2.Rows[e.RowIndex].Cells["時間單位"].Value.ToString().Trim();
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
                where_A = " and A.I_No = '" + txt1.Text.Trim() + "'";

            if (this.txt5.Text.Trim() != "")//名稱
                where_A = " and A.I_Name like '%" + txt5.Text.Trim() + "%'";

            if (this.txt6.Text.Trim() != "")//規格
                where_A = where_A + " and A.I_Spec like '%" + txt6.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button7_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt2.Text = ""; this.txt3.Text = ""; this.txt4.Text = "";
            this.txt5.Text = ""; this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = "";
            this.txt9.Text = ""; this.txt10.Text = ""; this.txt11.Text = "";
            this.comboBox1.SelectedIndex = 0; this.textBox1.Text = "";
            this.CB1.SelectedIndex = 0;
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
            if (Class1.GetRowCount("Instrument", "I_No='" + txt1.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『治具編號：" + txt1.Text.Trim() + "』資料，已存在...");
                return;
            }
            string strPrec = (txt3.Text.Trim() == "") ? "null" : txt3.Text.Trim();
            string strLife = (txt8.Text.Trim() == "") ? "null" : txt8.Text.Trim();
            string strUsed = (txt10.Text.Trim() == "") ? "null" : txt10.Text.Trim();
            //Insert
            SqlStr = "Insert into [Instrument]"
            + " (I_No, I_Name, Brand, I_Spec, Precision, P_Unit, Life_time, Used_time, U_Unit, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt5.Text.Trim() + "',"
            + "'" + txt2.Text.Trim() + "',"
            + "'" + txt6.Text.Trim() + "',"
            + "" + strPrec + ","
            + "'" + txt4.Text.Trim() + "',"
            + "" + strLife + ","
            + "" + strUsed + ","
            + "'" + txt11.Text.Trim() + "',"
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
                SqlStr = "Delete from Instrument"
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
            string strPrec = (txt3.Text.Trim() == "") ? "null" : txt3.Text.Trim();
            string strPUnit = (txt4.Text.Trim() == "") ? "" : txt4.Text.Trim();
            //使用壽命
            string strLife = (txt8.Text.Trim() == "") ? "null" : txt8.Text.Trim();
            string strUsed = (txt10.Text.Trim() == "") ? "null" : txt10.Text.Trim();
            string strUUnit = (txt11.Text.Trim() == "") ? "" : txt11.Text.Trim();
            //update
            SqlStr = "update [Instrument] set "
            + "I_No = '" + txt1.Text.Trim() + "',"
            + "I_Name = '" + txt5.Text.Trim() + "',"
            + "Brand = '" + txt2.Text.Trim() + "',"
            + "I_Spec = '" + txt6.Text.Trim() + "',"
            + "Precision = " + strPrec + ","
            + "P_Unit = '" + strPUnit + "',"
            + "Life_time = " + strLife + ","
            + "Used_time = " + strUsed + ","
            + "U_Unit = '" + strUUnit + "',"
            //+ "OutMaterial_Type = '" + CB1.SelectedItem.ToString().Trim().Substring(0,1) + "',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where I_No = '" + this.label_No.Text + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form8_FormClosed(object sender, FormClosedEventArgs e)
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
    }
}
