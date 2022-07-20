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
    public partial class Form19 : Form
    {
        string SqlStr = "";

        public Form19()
        {
            InitializeComponent();
        }
       
        /// <summary>定義每個欄位寬度</summary>
        /// <param name="column_num">總共多少欄位</param>
        /// <param name="column_width">欄寬 </param>
        private void Columns_for(int column_num, int[] column_width) 
        {            
            for (int i = 0; i < column_num; i++)
            {
                //如果是字串
                if (dgvDetail2.Columns[i].ValueType == typeof(string))
                {
                    dgvDetail2.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else 
                {
                    dgvDetail2.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                //定義每個欄位的寬度
                dgvDetail2.Columns[i].Width = column_width[i];
            }
        }
   
        /// <summary>查詢函式</summary>
        /// <param name="where_str">查詢字串</param>      
        private void Form_Query(string where_str)
        {
            SqlStr = "Select A.Ws_Code as '工站代號',"
                + "A.Ws_Name as '工站名稱',"
                //+ "A.Filer_Id as '建檔者代號',"
                //+ "A.Filer_Name as '建檔者名稱',"
                + "A.Fun_Need_Level '職能需求等級',"
            
                + "Emp_Code as '員工代號',"
                + "Fun_Qua_Emp_Name '職能資格員工姓名',"
                + "Fun_Level '職能等級',"
                + "A.SNo as '維護者代號',"
                + "convert(varchar,A.SDate,120) as '維護者時間'"
                + " from Ws_Fun_Req A"
                +" where 1=1";

            //如果查詢字串是空值
            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by convert(varchar,A.SDate,120) desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            int column_num = 8;//總共0-7共8個欄位
            int[] column_width = { 90, 110, 120, 120, 125, 90, 100, 100};//欄寬值

            Columns_for(column_num, column_width);

            if (dt.Rows.Count >= 1) 
            {
                this.label8.Text = "符合查詢共" + dt.Rows.Count.ToString() + "筆";
            }
            else
            {
                this.label8.Text = "符合查詢共0筆";
                MessageBox.Show("查無相關資料...");
            }
        }
        //速查
        private void button6_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.textBox6.Text.Trim() != "")
            {
                where_A = "and A.Ws_Code like '%" + textBox6.Text.Trim() + "%'";
            }

            Form_Query(where_A);
        } 
       
        //初始化
        private void Form19_load(object sender, EventArgs e)
        {
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 12 / 25;
            //上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 10 / 37;
            //功能鍵高，比例法，離上方往下182
            //button
            //Class1.DropDownList_B("goods_MachineNo", "goods_Basic", CB1, "where goods_MachineNo<>''");
            //Class1.DropDownList_C("", "", "", "", CB1, "");
            //this.CB1.SelectedIndex = 0;
            //反灰工站編號欄位
            this.txt1.ReadOnly = true;
            //反灰工站編號欄位
            this.txt1.ReadOnly = true;
            button6_Click(sender,e);
        }

        //點擊資料
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e) 
        {
            //如果沒有欄位資料及中斷
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //工站代號
            this.txt1.Text = dgvDetail2.Rows[e.RowIndex].Cells["工站代號"].Value.ToString().Trim();
            this.label_No.Text = this.txt1.Text;
            //工站名稱
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["工站名稱"].Value.ToString().Trim();
            //職能需求等級//下拉選項
            //this.txt3.Text = dgvDetail2.Rows[e.RowIndex].Cells["職能需求等級"].Value.ToString().Trim();
            string kind = dgvDetail2.Rows[e.RowIndex].Cells["職能需求等級"].Value.ToString().Trim();

            string[,] arr_kind = {{"1","新手"},{"2","熟手"},{"3","超熟手"}};

            Class1.cbo_choose(arr_kind, kind, CB1);

            //建檔時間.
            //this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["建檔時間"].Value.ToString().Trim();
            //職能等級
            this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["職能等級"].Value.ToString().Trim();
            //員工代號
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["員工代號"].Value.ToString().Trim();
            //職能資格員工姓名
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["職能資格員工姓名"].Value.ToString().Trim();     
            //修改者代號
            this.txt5.Text = dgvDetail2.Rows[e.RowIndex].Cells["修改者代號"].Value.ToString().Trim();
            //修改者時間
            this.txt6.Text = dgvDetail2.Rows[e.RowIndex].Cells["修改者時間"].Value.ToString().Trim();
        }
        //查詢
        private void button1_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "") //工站代號
                where_A = "and A.Ws_Code like '%" + txt1.Text.Trim() + "%'";

            Form_Query(where_A);
        }
        //清除
        private void button5_Click(object sender, EventArgs e)
        {
            this.txt1.Text = ""; this.txt4.Text = "";
            //this.txt2.Text = "";
            this.txt5.Text = "";
            //this.txt3.Text = "";
            this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = "";
            this.txt9.Text = "";
            this.label_No.Text = "";
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工站代號』資料...");
                return false;
            }
            if (txt4.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『工站名稱』資料...");
                return false;
            }        
            if (txt7.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『員工代號』資料...");
                return false;
            }
            //if (CB1.SelectedItem.ToString().Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『職能需求等級』資料...");
            //    return false;
            //}
            if (txt9.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『職能等級』資料...");
                return false;
            }
            if (txt8.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『職能資格員工姓名』資料...");
                return false;
            }         
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『維護者代號』資料...");
                return false;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『維護者時間』資料...");
                return false;
            }
            //if (CB1.SelectedItem.ToString().Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『製品分類』資料...");
            //    return false;
            //}
            //if (CB2.SelectedItem.ToString().Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『發料方式』資料...");
            //    return false;
            //}
            //if (CB3.SelectedItem.ToString().Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『移轉方式』資料...");
            //    return false;
            //}
            //if (CB4.SelectedItem.ToString().Trim() == "")
            //{
            //    MessageBox.Show("請先輸入『檢驗方式』資料...");
            //    return false;
            //}
            return isOK;           
        }
        //新增
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("Ws_Fun_Req"," Ws_Code='"+txt1.Text.Trim() +"'")>0)
            {
                MessageBox.Show("請注意『工站代號：" + txt1.Text.Trim() + "』資料，已存在...");
                return;
            }
            //Insert
            SqlStr = "Insert into[Ws_Fun_Req]"
            + "(Ws_Code, Ws_Name,Filer_Id,Filer_Name,Fun_Need_Level,SDate,Emp_Code,Fun_Qua_Emp_Name,Fun_Level)"
            + "values("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt4.Text.Trim() + "',"
            //+ "'" + txt2.Text.Trim() + "',"          
            //+ "'" + txt3.Text.Trim() + "',"
            //+ "'" + txt6.Text.Trim() + "',"
            //+ "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"
            + "'" + txt7.Text.Trim() + "',"
            //+ "'" + CB1.SelectedItem.ToString().Substring(0, 1) + "',"//發料方式
            + "'" + txt8.Text.Trim() + "',"
            + "'" + txt9.Text.Trim() + "',"
            + "'" + txt5.Text.Trim() + "',"
             + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',"
            //+ "'" + Loginfm.id.Trim() + "',"
            //+ "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            +")";
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
                MessageBox.Show("請先輸入欲刪除之『工站代號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『工站代號』為:" + this.txt1.Text.Trim() + "的資料嗎?", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)              
            {
                SqlStr = "Delete from Ws_Fun_Req"
                    + " where Ws_Code='" + this.txt1.Text.Trim()+"'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成，請重新查詢...");
                button5_Click(sender, e);//清除
                //button1_Click(sender, e);//查詢
            }
        }
        //修改
        private void button3_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //update
            SqlStr = "update [Ws_Fun_Req] set "
                + "Ws_Code = '" + txt1.Text.Trim() + "',"
                + "Ws_Name = '" + txt4.Text.Trim() + "',"
                //+ "Filer_Id = '" + txt2.Text.Trim() + "',"
                //+ "Filer_Name = '" + txt5.Text.Trim() + "',"
                //+ "Fun_Need_Level = '" + txt3.Text.Trim() + "',"
                + "SDate = '" + txt6.Text.Trim() + "',"
                + "Emp_Code = '" + txt7.Text.Trim() + "',"
                + "Fun_Qua_Emp_Name = '" + txt8.Text.Trim() + "',"
                + "Fun_Level = '" + txt9.Text.Trim() + "'"
                + " where Ws_Code = '" + this.label_No.Text + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button5_Click(sender, e);//清除
            button1_Click(sender, e);//查詢
        }

        //結束
        private void Form19_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
            //Application.Exit(); //加这句
        }

        private void txt11_TextChanged(object sender, EventArgs e)
        {

        }

   
    }
}
