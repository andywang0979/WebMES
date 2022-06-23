using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace NightMarketTicket
{
    public partial class Fm7 : Form
    {
        string SqlStr = "";

        public Fm7()
        {
            InitializeComponent();
        }

        private void Fm7_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            Fm2 fm = new Fm2();
            fm.Show();
        }

        private void Fm7_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            splitContainer1.SplitterDistance = splitContainer1.Height - 400;
            splitContainer2.SplitterDistance = splitContainer2.Height - 100;
            //下拉選項
            Class1.DropDownList_B("City", "HotelTicket", comboBox1, "where HotelName<>''");
            SqlStr = "Select HotelNo,HotelName,City,Addr,RoomMax,TicketMax,TStar,TEnd,"
            + "author,convert(varchar,updatetime,120) as updatetime from"
            + " HotelTicket where 1=1 order by HotelNo";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            if (dt.Rows.Count >= 1)
            {
                this.label10.Text = "符合查詢共 " + dt.Rows.Count.ToString() + " 筆";
                dgvDetail2.DataSource = dt;
            }
            else
            {
                this.label10.Text = "符合查詢共 0 筆";
                MessageBox.Show("查無相關資料...");
            }
            dgvDetail2.Visible = true;
            dgvDetail2.Dock = System.Windows.Forms.DockStyle.Fill;
            //清textBox
            button7_Click(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //匯入前檢查資料是否存在
            SqlStr = "Select * from HotelTicket";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            if (dt.Rows.Count >= 1)
            {
                if (MessageBox.Show(this, "確定要刪除該消費劵的旅宿業者全部資料然後重新匯入嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    SqlStr = "Delete from"
                    + " HotelTicket"
                    + " where 1=1";
                    Class1.Execute_SQL(SqlStr);
                }
                else
                    return;
            }

            openFileDialog1.Filter = "XLS/XLSX|*.XLS;*.XLSX";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //載入檔案
                System.Windows.Forms.Application.DoEvents();
                string filepath = openFileDialog1.FileName;
                Microsoft.Office.Interop.Excel.Application Ex = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook Wb = Ex.Workbooks.Open(filepath);
                Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)Wb.Sheets[1];
                //blInitial = true;
                Microsoft.Office.Interop.Excel.Range xRange = sheet.UsedRange;
                int k = 0;
                try
                {
                    object[,] valueArray = (object[,])xRange.get_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault);

                    progressBar1.Minimum = 1;
                    progressBar1.Maximum = Convert.ToInt16(textBox2.Text.Trim()) - (Convert.ToInt16(textBox3.Text.Trim()) - 1);

                    if (valueArray.Length > 0)  //陣列資料總個數大於零
                    {
                        //int y = valueArray.GetLength(0);  //取得維度1的長度，即列數
                        //int x = valueArray.GetLength(1);  //取得維度2的長度，即欄數
                        int y = Convert.ToInt16(textBox2.Text.Trim());
                        int x = Convert.ToInt16(textBox3.Text.Trim());
                        //年度-YR，計畫名稱-PNo
                        for (int arrj = x; arrj <= y; arrj++)
                        {
                            #region 抓資料匯入
                            System.Windows.Forms.Application.DoEvents();

                            //總編號
                            string v1 = "";
                            try { v1 = valueArray.GetValue(arrj, 1).ToString().Trim(); }
                            catch 
                            {                                
                                continue;
                            }
                            v1 = Convert.ToInt32(v1).ToString("000000");

                            //縣市
                            string v2 = "";
                            try { v2 = valueArray.GetValue(arrj, 3).ToString().Trim(); }
                            catch { MessageBox.Show("此筆(總編號=" + v1 + ")【縣市】異常不匯入！"); }

                            //業者名稱
                            string v3 = "";
                            try { v3 = valueArray.GetValue(arrj, 4).ToString().Trim(); }
                            catch { MessageBox.Show("此筆(總編號=" + v1 + ")【業者名稱】異常不匯入！"); }

                            //地址
                            string v4 = "";
                            try { v4 = valueArray.GetValue(arrj, 5).ToString().Trim(); }
                            catch { MessageBox.Show("此筆(總編號=" + v1 + ")【地址】異常不匯入！"); }

                            //房間數
                            string v5 = "";
                            try { v5 = valueArray.GetValue(arrj, 6).ToString().Trim(); }
                            catch { MessageBox.Show("此筆(總編號=" + v1 + ")【房間數】異常不匯入！"); }

                            //張數
                            string v6 = "";
                            try { v6 = valueArray.GetValue(arrj, 7).ToString().Trim(); }
                            catch { MessageBox.Show("此筆(總編號=" + v1 + ")【張數】異常不匯入！"); }

                            //起號
                            string v7 = "";
                            try { v7 = valueArray.GetValue(arrj, 8).ToString().Trim(); }
                            catch { MessageBox.Show("此筆(總編號=" + v1 + ")【起號】異常不匯入！"); }

                            //迄號
                            string v8 = "";
                            try { v8 = valueArray.GetValue(arrj, 10).ToString().Trim(); }
                            catch { MessageBox.Show("此筆(總編號=" + v1 + ")【迄號】異常不匯入！"); }

                            if (v1 == "001451") v3 = "官邸飯店MONO TEL";
                            if (v1 == "004188") v3 = "Cest La Vie民宿";
                            //if (v1 == "001450") MessageBox.Show(SqlStr);

                            //Insert
                            SqlStr = "Insert into HotelTicket values ("
                            + "'" + v1 + "',"
                            + "'" + v3 + "',"
                            + "'" + v2 + "',"
                            + "'" + v4 + "',"
                            + "" + v5 + ","
                            + "" + v6 + ","
                            + "" + v7 + ","
                            + "" + v8 + ",'','','',"
                            + "'" + mainfm.id.Trim() + "',"
                            + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
                            + ")";                            
                            
                            Class1.Execute_SQL(SqlStr);
                            //進度bar
                            k++;
                            progressBar1.Value = k;
                            #endregion
                        }
                    }
                }
                catch { MessageBox.Show("開啟EXCEL檔有誤！"); }
                //關閉活頁簿
                Wb.Close(true, Type.Missing, Type.Missing);
                //關閉Excel
                Ex.Quit();
                //釋放Excel資源 
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Ex);
                Wb = null;
                sheet = null;
                xRange = null;
                Ex = null;
                GC.Collect();
            }
            MessageBox.Show("檔案匯入完畢!!");
            SqlStr = "Select HotelNo,HotelName,City,Addr,RoomMax,TicketMax,TStar,TEnd,author,updatetime from"
                + " HotelTicket"
                //+ " where LessonNo='" + this.txtLessonNo1.Text.Trim() + "'"
                + " order by HotelNo";
            dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;
            dgvDetail2.Visible = true;
            dgvDetail2.Dock = System.Windows.Forms.DockStyle.Fill;
            progressBar1.Value = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlStr = "Select HotelNo,HotelName,City,Addr,RoomMax,TicketMax,TStar,TEnd,author,updatetime from"
            + " HotelTicket where 1=1";
            if (comboBox1.SelectedValue.ToString().Trim() != "")
            {
                SqlStr = SqlStr + " and City = '" + comboBox1.SelectedValue.ToString().Trim() + "'";
            }
            if (this.textBox1.Text.Trim() != "")
            {
                SqlStr = SqlStr + " and HotelName like '%" + textBox1.Text.Trim() + "%'";
            }
            SqlStr = SqlStr + " order by HotelNo";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;
            if (dt.Rows.Count >= 1)
                this.label10.Text = "符合查詢共 "+dt.Rows.Count.ToString() + " 筆";
            else
            {
                this.label10.Text = "符合查詢共 0 筆";
                MessageBox.Show("查無相關資料...");
            }
        }

        private void dgvDetail2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            //業者代碼
            this.textBox11.Text = dgvDetail2.Rows[e.RowIndex].Cells["HotelNo"].Value.ToString().Trim();
            //名稱
            this.textBox4.Text = dgvDetail2.Rows[e.RowIndex].Cells["HotelName"].Value.ToString().Trim();
            //住址
            this.textBox6.Text = dgvDetail2.Rows[e.RowIndex].Cells["Addr"].Value.ToString().Trim();
            //縣市
            this.textBox5.Text = dgvDetail2.Rows[e.RowIndex].Cells["City"].Value.ToString().Trim();
            //房間數
            this.textBox7.Text = dgvDetail2.Rows[e.RowIndex].Cells["RoomMax"].Value.ToString().Trim();
            //發卷數
            this.textBox8.Text = dgvDetail2.Rows[e.RowIndex].Cells["TicketMax"].Value.ToString().Trim();
            //起號
            this.textBox9.Text = dgvDetail2.Rows[e.RowIndex].Cells["TStar"].Value.ToString().Trim();
            //迄號
            this.textBox10.Text = dgvDetail2.Rows[e.RowIndex].Cells["TEnd"].Value.ToString().Trim();
            //異動人
            this.textBox12.Text = dgvDetail2.Rows[e.RowIndex].Cells["author"].Value.ToString().Trim();
            //異動時間
            this.textBox13.Text = dgvDetail2.Rows[e.RowIndex].Cells["updatetime"].Value.ToString().Trim();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox11.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入旅宿業者代碼...");
                return;
            }
            if (MessageBox.Show(this, "確定要刪除該旅宿業者資料嗎？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                SqlStr = "Delete from"
                + " HotelTicket"
                + " where HotelNo='" + this.textBox11.Text.Trim() + "'";
                Class1.Execute_SQL(SqlStr);
                MessageBox.Show("該筆資料刪除完成...");
            }
            else
                return;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox11.Text.Trim() == "" || textBox4.Text.Trim() == "" || textBox6.Text.Trim() == "" || textBox5.Text.Trim() == "" || textBox7.Text.Trim() == "" || textBox8.Text.Trim() == "" || textBox9.Text.Trim() == "" || textBox10.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入旅宿業者相關資料...");
                return;
            }
            if (textBox5.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入縣市資料...");
                return;
            }
            if (textBox7.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入房間數資料...");
                return;
            }
            if (textBox8.Text.Trim() == "" || textBox9.Text.Trim() == "" || textBox10.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入卷數等相關資料...");
                return;
            }
            //Insert
            SqlStr = "Insert into HotelTicket values ("
            + "'" + textBox11.Text.Trim() + "',"
            + "'" + textBox4.Text.Trim() + "',"
            + "'" + textBox6.Text.Trim() + "',"
            + "'" + textBox5.Text.Trim() + "',"
            + "" + textBox7.Text.Trim() + ","
            + "" + textBox8.Text.Trim() + ","
            + "" + textBox9.Text.Trim() + ","
            + "" + textBox10.Text.Trim() + ",'','','',"
            + "'" + mainfm.id.Trim() + "',"
            + "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + ")";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("新增資料完成...");            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlStr = "Select HotelNo,HotelName,City,Addr,RoomMax,TicketMax,TStar,TEnd,"
            + "author,convert(varchar,updatetime,120) as updatetime from"
            + " HotelTicket where 1=1";
            if (textBox11.Text.Trim() != "")
            {
                SqlStr = SqlStr + " and HotelNo='" + this.textBox11.Text.Trim() + "'";                
            }
            if (textBox4.Text.Trim() != "")
            {
                SqlStr = SqlStr + " and HotelName like '%" + this.textBox4.Text.Trim() + "%'";
            }
            if (textBox6.Text.Trim() != "")
            {
                SqlStr = SqlStr + " and Addr like '%" + this.textBox6.Text.Trim() + "%'";
            }
            if (textBox5.Text.Trim() != "")
            {
                SqlStr = SqlStr + " and City like '%" + this.textBox5.Text.Trim() + "%'";
            }
            SqlStr = SqlStr + " order by HotelNo";
            System.Data.DataTable dt = Class1.GetDataTable(SqlStr);
            dgvDetail2.DataSource = dt;

            if (dt.Rows.Count >= 1)
                this.label10.Text = "符合查詢共 " + dt.Rows.Count.ToString() + " 筆";
            else
            {
                this.label10.Text = "符合查詢共 0 筆";
                MessageBox.Show("查無相關資料...");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //清textBox
            this.textBox4.Text = ""; this.textBox5.Text = ""; this.textBox6.Text = ""; this.textBox7.Text = "";
            this.textBox8.Text = ""; this.textBox9.Text = ""; this.textBox10.Text = ""; this.textBox11.Text = "";
            this.textBox12.Text = ""; this.textBox13.Text = "";
            this.textBox11.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox11.Text.Trim() == "" || textBox4.Text.Trim() == "" || textBox6.Text.Trim() == "" || textBox5.Text.Trim() == "" || textBox7.Text.Trim() == "" || textBox8.Text.Trim() == "" || textBox9.Text.Trim() == "" || textBox10.Text.Trim() == "")
            {
                MessageBox.Show("請先輸入旅宿業者相關資料...");
                return;
            }
            //update
            SqlStr = "update HotelTicket set"
            + " HotelName = '" + textBox4.Text.Trim() + "',"
            + " Addr = '" + textBox6.Text.Trim() + "',"
            + " City = '" + textBox5.Text.Trim() + "',"
            + " RoomMax = " + textBox7.Text.Trim() + ","
            + " TicketMax = " + textBox8.Text.Trim() + ","
            + " TStar = " + textBox9.Text.Trim() + ","
            + " TEnd = " + textBox10.Text.Trim() + ","
            + " author = '" + mainfm.id.Trim() + "',"
            + " updatetime = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'"
            + " where HotelNo = '" + textBox11.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("修改資料完成...");
            button3_Click(sender, e);
        }
    }
}
