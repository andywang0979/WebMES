using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CPCAIModule
{
    public partial class FormPop : Form
    {
        public FormPop()
        {
            InitializeComponent();            
        }        

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = true;
            this.textBox4.ReadOnly = false;
            this.textBox5.ReadOnly = false;
            this.textBox6.ReadOnly = false;
            this.textBox7.ReadOnly = false;
            this.textBox8.ReadOnly = false;
            this.textBox9.ReadOnly = false;
            this.button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string SqlStr = "Update [LessonList] set "
            + "LName='" + this.textBox6.Text.Trim() + "',"
            + "LStar='" + this.textBox4.Text.Trim() + "',"
            + "LEnd='" + this.textBox5.Text.Trim() + "',"
            + "Teacher='" + this.textBox7.Text.Trim() + "',"
            + "Hours='" + this.textBox8.Text.Trim() + "',"
            + "ClassRoom='" + this.textBox9.Text.Trim() + "'"
            + " where LessonNo='" + textBox1.Text.Trim() + "' and SDay='" + this.label1.Text.Trim() + "'"
            + " and LName='" + this.label2.Text.Trim() + "'";
            Class1.Execute_SQL(SqlStr);
            MessageBox.Show("資料修改成功，返回後請重新查詢...");
            this.button2.Enabled = false;
            this.textBox4.ReadOnly = true;
            this.textBox5.ReadOnly = true;
            this.textBox6.ReadOnly = true;
            this.textBox7.ReadOnly = true;
            this.textBox8.ReadOnly = true;
            this.textBox9.ReadOnly = true;
            this.button1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string SNo = this.textBox11.Text.Trim();
            string SName = this.textBox10.Text.Trim();
            string LNo = this.textBox1.Text.Trim();
            string SNote = Class1.GetValue("LessonKind", "LessonDetail", "LessonNo = '" + LNo + "'");
            //抓期別            
            //string SqlStr = "Select '" + SNo + "' as S1,'" + SName + "' as S2,'" + SNote + "' as Note1,"
            //+ "* from [examerp].[LessonList] where LessonNo = '" + LNo + "'"
            //+ " and SDay = '" + textBox2.Text.Trim() + "'";
            //Fm6.report_dt = Class1.GetDataTable(SqlStr);
            //if (Fm6.report_dt.Rows.Count > 0)
            //{
            //    Fm6 fm = new Fm6();
            //    Fm6.report.Load("CrystalReport62.rpt");
            //    Fm6.report.SetDataSource(Fm6.report_dt);
            //    fm.ShowDialog();
            //}
            //else
            //    MessageBox.Show("查無資料...");
        }

        private void textBox10_Enter(object sender, EventArgs e)
        {
            string LNo = this.textBox1.Text.Trim();
            string SNo = LNo + textBox11.Text.PadLeft(3, '0');
            string SName = Class1.GetValue("Name", "Student", "LessonNo = '" + LNo + "' and DiplomaNo = '" + SNo + "'");
            textBox10.Text = SName.Trim();
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                this.textBox10.Focus();
        }

        private void textBox11_Enter(object sender, EventArgs e)
        {
            textBox11.Text = "";
        }
    }
}
