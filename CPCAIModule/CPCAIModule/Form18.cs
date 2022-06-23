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
    public partial class Form18 : Form
    {
        string SqlStr = "";
        public static string msg = "";
        //共用變數
        public Form18()
        {
            InitializeComponent();
        }
        //定義每個欄位寬度
        private void Columns_for(DataGridView dgv, int column_num, int[] column_width)
        {
            //column_num : 總共欄位幾個
            //C# 數字陣列 column_width
            for (int i = 0; i < column_num; i++)
            {
                if (dgv.Columns[i].ValueType == typeof(string))//若是字串
                {
                    dgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;  
                }
                else
                {
                    dgv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;              
                }
                dgv.Columns[i].Width = column_width[i];//定義每個欄位寬度
            }
        }
        //查詢函式
        private void Form_Query_M(string where_str)
        {
            //PARSENAME('$'+ Convert(varchar,Convert(money,A.Pay),1),2) as '產品單價'//轉貨幣符號
            //+ "REPLACE(Convert(varchar,A.Price),'.00','') as '產品單價',"//取代
            #region Mastr查詢
            SqlStr = "Select A.Journey_No as '途程編號',"
            + "A.Journey_Name as '途程名稱',"
            + "A.Note as '備註說明',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from JourneyM A"//left join Customer B on A.CusNo = B.CusNo"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1 ";

            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;

            SqlStr = SqlStr + " order by convert(varchar,A.SDate,120) desc";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail1.DataSource = dt;

            int column_num = 5;//總共0-13共14個欄位
            int[] column_width = { 80, 200, 250, 80, 160 };//欄寬值
            Columns_for(dgvDetail1 ,column_num, column_width);
            
            if (dt.Rows.Count >= 1)
                this.label10.Text = "符合查詢共 " + dt.Rows.Count.ToString() + " 筆";
            else
            {
                this.label10.Text = "符合查詢共 0 筆";
                MessageBox.Show("查無相關資料...");
            }
            #endregion                       
        }

        private void Form_Query_D(string where_str)
        {
            //PARSENAME('$'+ Convert(varchar,Convert(money,A.Pay),1),2) as '產品單價'//轉貨幣符號
            //+ "REPLACE(Convert(varchar,A.Price),'.00','') as '產品單價',"//取代            
            #region Detail查詢
            SqlStr = "Select A.Journey_odr as '作業順序',"
            + "A.MProcess_No as '製程編號',"
            + "B.MProcess_Name as '製程名稱',"
            + "A.Note as '明細備註',"
            + "A.ENo as '異動人員',"
            + "convert(varchar,A.SDate,120) as '異動時間'"
            + " from JourneyD A left join MProcess B on A.MProcess_No = B.MProcess_No"// left join Products C on A.ProductNo=C.ProductNo"
            + " where 1=1 ";
            if (where_str.Trim() != "")
                SqlStr = SqlStr + where_str;
            SqlStr = SqlStr + " and A.Journey_No = '" + this.txt1.Text.Trim() + "' order by Journey_odr";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;
            int column_num_D = 6;//總共0-13共14個欄位
            int[] column_width_D = { 80, 100, 200, 250, 80, 160 };//欄寬值
            Columns_for(dgvDetail2, column_num_D, column_width_D);
            #endregion
        }

        //速查
        private void button2_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.comboBox1.SelectedIndex > 0)
                where_A = " and A.Journey_No ='" + this.comboBox1.SelectedValue.ToString().Trim() + "'";

            if (this.textBox1.Text.Trim() != "")
                where_A = where_A + " and A.Journey_Name like '%" + textBox1.Text.Trim() + "%' or A.Note like '%" + textBox1.Text.Trim() + "%'";

            Form_Query_M(where_A);
            Form_Query_D("");
        }
        //初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            this.splitContainer3.SplitterDistance = splitContainer1.Height - 100;//左方寬，固定法
            //第1刀位置
            this.splitContainer1.SplitterDistance = splitContainer1.Height * 12 / 27;//上方高，比例法，離上方往下266
            //第2刀位置
            this.splitContainer2.SplitterDistance = splitContainer1.Height * 11 / 37;//功能鍵高，比例法，離上方往下182
            //下拉選項
            Class1.DropDownList_B("Journey_No", "JourneyM", comboBox1, "where Journey_No<>''");
            if (comboBox1.SelectedIndex > -1)//當DB沒半比時不執行下拉選項
                this.comboBox1.SelectedIndex = 0;
            button2_Click(sender, e);//速查
            button7_Click(sender, e);//清除
            button17_Click(sender, e);//明細清除
        }
        //點擊資料D
        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //物料代碼
            this.txt4.Text = dgvDetail2.Rows[e.RowIndex].Cells["作業順序"].Value.ToString().Trim();
            this.txt8.Text = dgvDetail2.Rows[e.RowIndex].Cells["製程編號"].Value.ToString().Trim();
            this.label_D1.Text = this.txt4.Text;
            this.label_D2.Text = this.txt8.Text;
            //物料名稱
            this.txt10.Text = dgvDetail2.Rows[e.RowIndex].Cells["製程名稱"].Value.ToString().Trim();
            //明細備註
            this.txt11.Text = dgvDetail2.Rows[e.RowIndex].Cells["明細備註"].Value.ToString().Trim();
            //異動人員
            this.txt7.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動人員"].Value.ToString().Trim();
            //異動時間
            this.txt9.Text = dgvDetail2.Rows[e.RowIndex].Cells["異動時間"].Value.ToString().Trim();           
            #region 下拉選項
            //發料方式//下拉選項
            //string kind1 = dgvDetail2.Rows[e.RowIndex].Cells["大分類"].Value.ToString().Trim();
            //string[,] arr_kind1 = { { "0", "" }, { "1", "原料" }, { "2", "物料" }, { "3", "耗材" } };
            //Class1.cbo_choose(arr_kind1, kind1, CB1);
            
            //string type = dgvDetail2.Rows[e.RowIndex].Cells["發料方式"].Value.ToString().Trim();
            //string[,] arr_Out = { { "0", "" }, { "1", "一次發料" }, { "2", "分批發料" }, { "3", "現場領料" } };
            //Class1.cbo_choose(arr_Out, type, CB2);

            //string kind2 = dgvDetail2.Rows[e.RowIndex].Cells["中分類"].Value.ToString().Trim();
            //string[,] arr_kind2 = { { "0", "" }, { "1", "油品" }, { "2", "零件" }, { "3", "棒材" }, { "4", "線材" } };
            //Class1.cbo_choose(arr_kind2, kind2, CB3);
            
            //type = dgvDetail2.Rows[e.RowIndex].Cells["檢驗方式"].Value.ToString().Trim();
            //string[,] arr_QC = { { "0", "" }, { "1", "免驗" }, { "2", "抽驗" }, { "3", "全檢" } };
            //Class1.cbo_choose(arr_QC, type, CB4);
            #endregion
        }
        //查詢M
        private void button3_Click(object sender, EventArgs e)
        {
            string where_A = "";
            if (this.txt1.Text.Trim() != "")//料號
                where_A = " and A.Journey_No like '%" + txt1.Text.Trim() + "%'";

            if (this.txt5.Text.Trim() != "")//名稱
                where_A = " and A.Journey_Name like '%" + txt5.Text.Trim() + "%'";

            if (this.txt6.Text.Trim() != "")//規格
                where_A = where_A + " and A.Note like '%" + txt6.Text.Trim() + "%'";

            Form_Query_M(where_A);
            Form_Query_D("");
        }
        //清除
        private void button7_Click(object sender, EventArgs e)
        {
            #region 清除
            this.txt1.Text = ""; this.txt2.Text = ""; this.txt3.Text = ""; this.txt4.Text = "";
            this.txt5.Text = ""; this.txt6.Text = ""; this.txt7.Text = ""; this.txt8.Text = "";
            this.txt9.Text = ""; 
            if (comboBox1.SelectedIndex>-1)//當DB沒半比時不執行下拉選項
                this.comboBox1.SelectedIndex = 0; 
            this.textBox1.Text = "";
            this.CB1.SelectedIndex = 0; this.CB2.SelectedIndex = 0; this.CB3.SelectedIndex = 0; this.CB4.SelectedIndex = 0;
            this.label_No.Text = "";
            #endregion
        }
        //檢查空欄位
        private bool Form_chk()
        {
            bool isOK = true;
            if (txt1.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『途程編號』資料...");
                return false;
            }            
            
            if (txt5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『途程名稱』資料...");
                return false;
            }
            if (txt6.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『備註說明』資料...");
                return false;
            }
            return isOK;
        }
        private bool Form_D_chk()
        {
            bool isOK = true;
            if (txt4.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『作業順序』資料...");
                return false;
            }
            if (txt8.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製程編號』資料...");
                return false;
            }
            if (txt10.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入『製程名稱』資料...");
                return false;
            }
            return isOK;
        }
        //新增
        private void button4_Click(object sender, EventArgs e)
        {
            if (Form_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("JourneyM", "Journey_No='" + txt1.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『途程編號：" + txt1.Text.Trim() + "』資料，已存在...");
                return;
            }
            //Insert
            SqlStr = "Insert into [JourneyM]"
            + " (Journey_No, Journey_Name, Note, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt5.Text.Trim() + "',"
            + "'" + txt6.Text.Trim() + "',"
            //+ "'" + CB3.SelectedItem.ToString().Trim() + "',"//發料方式
            //+ "'" + CB4.SelectedItem.ToString().Substring(0, 1) + "',"//檢驗方式
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
                MessageBox.Show("請先輸入欲刪除之『途程編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『途程編號』為:" + this.txt1.Text.Trim() + "的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from [JourneyM]"
                + " where Journey_No='" + this.txt1.Text.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                SqlStr = "Delete from [JourneyD]"
                + " where Journey_No='" + this.txt1.Text.Trim() + "'";
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
            SqlStr = "update [JourneyM] set "
            + "Journey_No = '" + txt1.Text.Trim() + "',"
            + "Journey_Name = '" + txt5.Text.Trim() + "',"
            + "Note = '" + txt6.Text.Trim() + "',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where Journey_No = '" + this.label_No.Text + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button7_Click(sender, e);//清除
            button3_Click(sender, e);//查詢
        }
        //結束
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            FmMenu fm = new FmMenu();
            fm.Show();
        }
        
        private void dgvDetail1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //物料代碼
            this.txt1.Text = dgvDetail1.Rows[e.RowIndex].Cells["途程編號"].Value.ToString().Trim();
            this.label_No.Text = this.txt1.Text;
            //物料名稱
            this.txt5.Text = dgvDetail1.Rows[e.RowIndex].Cells["途程名稱"].Value.ToString().Trim();
            //備註說明
            this.txt6.Text = dgvDetail1.Rows[e.RowIndex].Cells["備註說明"].Value.ToString().Trim();
            //異動人員
            this.txt7.Text = dgvDetail1.Rows[e.RowIndex].Cells["異動人員"].Value.ToString().Trim();
            //異動時間
            this.txt9.Text = dgvDetail1.Rows[e.RowIndex].Cells["異動時間"].Value.ToString().Trim();
        }

        private void MasterGetValue(int k)//按上下一筆時帶出資料
        {
            this.txt1.Text = dgvDetail1.Rows[k].Cells[0].Value.ToString();
            this.label_No.Text = this.txt1.Text;
            this.txt5.Text = dgvDetail1.Rows[k].Cells[1].Value.ToString();
            this.txt6.Text = dgvDetail1.Rows[k].Cells[2].Value.ToString();
            this.txt7.Text = dgvDetail1.Rows[k].Cells[3].Value.ToString();
            this.txt9.Text = dgvDetail1.Rows[k].Cells[4].Value.ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            int rowindex = dgvDetail1.SelectedCells[0].OwningRow.Index;
            if (rowindex <= 0) return;//首筆指標移動
            dgvDetail1.CurrentCell = dgvDetail1.Rows[rowindex - 1].Cells[0];
            dgvDetail1.Rows[rowindex - 1].Selected = true;
            MasterGetValue(rowindex - 1);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int rowindex = dgvDetail1.SelectedCells[0].OwningRow.Index;
            if (rowindex >= dgvDetail1.Rows.Count - 1) return;//末筆指標移動
            dgvDetail1.CurrentCell = dgvDetail1.Rows[rowindex + 1].Cells[0];
            dgvDetail1.Rows[rowindex + 1].Selected = true;
            MasterGetValue(rowindex + 1);
        }

        private void label_No_TextChanged(object sender, EventArgs e)
        {
            string keyno = label_No.Text.Trim();
            Form_Query_D("and A.Journey_No = '" + keyno + "'");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            #region 明細新增
            if (Form_D_chk() == false) return;
            //檢查是否重複
            if (Class1.GetRowCount("JourneyD", "Journey_No='" + txt4.Text.Trim() + "' and  Journey_odr='"+txt8.Text.Trim()+"'") > 0)
            {
                MessageBox.Show("請注意『途程編號:" + txt1.Text.Trim() + "/作業順序:" + txt8.Text.Trim() + "』資料，已存在...");
                return;
            }
            if (Class1.GetRowCount("JourneyD", "Journey_No='" + txt4.Text.Trim() + "' and  MProcess_No='" + txt10.Text.Trim() + "'") > 0)
            {
                MessageBox.Show("請注意『途程編號:" + txt1.Text.Trim() + "/製程編號:" + txt10.Text.Trim() + "』資料，已存在...");
                return;
            }
            //Insert
            SqlStr = "Insert into [JourneyD]"
            + " (Journey_No, Journey_odr, MProcess_No, Note, ENo, SDate) "
            + "values ("
            + "'" + txt1.Text.Trim() + "',"
            + "'" + txt4.Text.Trim() + "',"
            + "'" + txt8.Text.Trim() + "',"
            + "'" + txt11.Text.Trim() + "',"
            + "'" + Loginfm.id.Trim() + "',"
            + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + ")";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("新增資料完成...");
            #endregion
            //明細查詢
            Form_Query_D("and A.Journey_No = '" + label_No.Text.Trim() + "'");
            button17_Click(sender, e);//清除
        }        

        private void button15_Click(object sender, EventArgs e)
        {
            msg = "";//取消傳回
            InputBox_MProcess input = new InputBox_MProcess();
            DialogResult dr = input.ShowDialog();
            if (dr == DialogResult.OK)//確定傳回
                this.txt8.Text = input.GetMsg();
            else
                this.txt8.Text = msg;
            this.txt10.Text = Class1.GetValue("MProcess_Name", "MProcess", "MProcess_No='" + txt8.Text.Trim() + "'");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            #region 明細取號
            string cus = Class1.GetValue("count(*)", "JourneyD", "Journey_No = '" + this.txt1.Text + "'");
            cus = (Convert.ToInt16(cus) + 1).ToString();
            this.txt4.Text = cus.Trim();
            #endregion
        }

        private void button14_Click(object sender, EventArgs e)
        {
            #region 明細刪除
            if (label_D1.Text.Trim() == "")
            {
                MessageBox.Show("請先點選欲刪除之『作業順序』...");
                return;
            }
            if (label_D2.Text.Trim() == "")
            {
                MessageBox.Show("請先點選欲刪除之『製程編號』...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除『作業順序:" + txt4.Text.Trim() + "/製程編號:" + txt8.Text.Trim() + "』的資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from [JourneyD]"
                + " where Journey_No='" + label_No.Text.Trim() + "' and  Journey_odr='" + label_D1.Text.Trim() + "' and MProcess_No='" + label_D2.Text.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成，請重新查詢...");                
                Form_Query_D("and A.Journey_No = '" + label_No.Text.Trim() + "'");
                button17_Click(sender, e);//清除
            }
            #endregion
        }

        private void button17_Click(object sender, EventArgs e)//明細清除
        {
            this.txt4.Text = ""; this.txt8.Text = "";
            this.txt10.Text = ""; this.txt11.Text = "";
            this.comboBox1.SelectedIndex = 0;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            #region 明細修改
            if (Form_D_chk() == false) return;
            //update
            SqlStr = "update [JourneyD] set "
            + "Journey_odr = '" + txt4.Text.Trim() + "',"
            + "MProcess_No = '" + txt8.Text.Trim() + "',"
            + "Note = '" + txt11.Text.Trim() + "',"
            + " ENo = '" + Loginfm.id.Trim() + "',"
            + " SDate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where Journey_No = '" + this.label_No.Text + "' and Journey_odr='" + label_D1.Text.Trim() + "' and MProcess_No='" + label_D2.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            //明細查詢
            Form_Query_D("and A.Journey_No = '" + label_No.Text.Trim() + "'");
            button17_Click(sender, e);//清除
            #endregion
        }        
    }
}
