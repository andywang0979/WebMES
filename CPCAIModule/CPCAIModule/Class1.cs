using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace CPCAIModule
{
    public static class Class1
    {
        public static string conn = string.Empty;

        public static DataTable GetDataTable(string sqlCommand)
        {
            //string connectionString = @"Data Source=.;Initial Catalog=CPCAIDB;User ID=sa;pwd=abc";
            string connectionString = @"Data Source=DB-3A120;Initial Catalog=CPCAIDB;User ID=sa;pwd=whale";

            SqlConnection Connection = new SqlConnection(connectionString);            
            conn = Connection.DataSource.ToString();
            SqlCommand command = new SqlCommand(sqlCommand, Connection);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command; 

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            adapter.Fill(table);
            return table;
        }

        public static DataTable GetDataTableA(string sqlCommand)
        {
            //string connectionString = Loginfm.connstring;
            //台中CPC
            //string connectionString = "Initial Catalog=cpc;Data Source=10.3.1.228;" +
            //"User ID=etestuser;Password=TccpcPao;";
            //台南CPC
            //string connectionString = "Initial Catalog=tncpc;Data Source=10.5.1.250;" +
            //"User ID=tnerp;Password=15326;";
            //高雄CPC
            //string connectionString = "Initial Catalog=ExamService;Data Source=10.4.1.5;" +
            //"User ID=sa;Password=1287;";

            string connectionString = @"Data Source=.;Initial Catalog=CPCAIDB;User ID=sa;pwd=abc";

            SqlConnection Connection = new SqlConnection(connectionString);

            SqlCommand command = new SqlCommand(sqlCommand, Connection);
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command; 

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            adapter.Fill(table);
            return table;
        }

        public static DataTable GetDataTable_OLEDB(string sqlCommand)
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ExamService.mdb;";
            //DB路徑為exe執行檔路徑
            //DB開啟後:工具→保全→使用者及群組的權限→選擇 table 勾選權限
                        
            OleDbConnection Connection = new OleDbConnection(connectionString);

            OleDbCommand command = new OleDbCommand(sqlCommand, Connection);
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            adapter.SelectCommand = command;

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            adapter.Fill(table);
            return table;
        }

        public static void Execute_SQL(string strSQL)
        {
            //string connectionString = Loginfm.connstring;
            //台中CPC
            //string connectionString = "Initial Catalog=ExamService;Data Source=10.3.1.228;" +
            //"User ID=cpcerp;Password=3a03;";
            //hank-pc
            //string connectionString = "Initial Catalog=CPCAIDB;Data Source=Barry;" +
            //"User ID=sa;Password=whale;";
            //NoteBook-pc
            //string connectionString = "Initial Catalog=CPCAIDB;Data Source=NB-3A120\\SQLEXPRESS;" +
            //"User ID=sa;Password=whale;";
            //高雄CPC            
            //string connectionString = "Initial Catalog=ExamService;Data Source=10.4.1.5;" +
            //"User ID=sa;Password=1287;";             
            //台南CPC
            //string connectionString = "Initial Catalog=ExamService;Data Source=10.5.1.250;" +
            //"User ID=tnerp;Password=15326;";

            //string connectionString = @"Data Source=.;Initial Catalog=CPCAIDB;User ID=sa;pwd=abc";
            string connectionString = @"Data Source=DB-3A120;Initial Catalog=CPCAIDB;User ID=sa;pwd=whale";
            SqlConnection objConn = new SqlConnection(connectionString);
            try { objConn.Open(); }
            catch { MessageBox.Show("『警告』連線失敗..."); return; }
            SqlCommand objCmd = new SqlCommand(strSQL, objConn);
            objCmd.ExecuteNonQuery();
            objConn.Close();
        }

        public static void Execute_SQL_OLEDB(string strSQL)
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ExamService.mdb;";
            //DB路徑為exe執行檔路徑
            //DB開啟後:工具→保全→使用者及群組的權限→選擇 table 勾選權限

            OleDbConnection Connection = new OleDbConnection(connectionString);
            OleDbCommand command = new OleDbCommand(strSQL, Connection);
            Connection.Open();
            command.ExecuteNonQuery();
            Connection.Close();
        }

        public static string[] GetYearTime()
        {
            string[] YearTime = new string[4];
            string SqlStr = "SELECT YY,Sequence,VNo,TNo from ExamSequence"
            + " where WorkType='Y'"
            + " and YY<>''"
            + " and Sequence<>''";
            DataTable dt = GetDataTable(SqlStr);
            if (GetDataTable(SqlStr).Rows.Count >= 1)
            {
                YearTime[0] = dt.Rows[0][0].ToString();
                YearTime[1] = dt.Rows[0][1].ToString();
                YearTime[2] = dt.Rows[0][2].ToString();
                YearTime[3] = dt.Rows[0][3].ToString();
            }
            return YearTime;
        }

        public static bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string[] GetCityTown(string sNo)
        {
            string[] CityTown = new string[2];
            string SqlStr = "select City,Town from NightMarket where NightMarketNo = '" + sNo + "'";
            DataTable dt = GetDataTable(SqlStr);
            if (dt.Rows[0][0].ToString().Trim() != "")
            {
                CityTown[0] = dt.Rows[0][0].ToString();
                CityTown[1] = dt.Rows[0][1].ToString();
            }
            return CityTown;
        }

        public static string GetMax(string table, string column)
        {
            string Vid = "";
            string SqlStr = "select max(" + column + ") from " + table + "";
            DataTable dt = GetDataTable(SqlStr);
            if (dt.Rows[0][0].ToString().Trim() != "")
            {
                Vid = dt.Rows[0][0].ToString().Trim();
                Vid = Convert.ToString(Convert.ToInt32(Vid) + 1);
            }
            else
            {
                Vid = "1";
            }
            return Vid;
        }

        public static string GetID(string sName)
        {
            string Vid = "";
            string SqlStr = "select IDNO from TeacherBasic where Name = N'" + sName + "'";
            DataTable dt = GetDataTable(SqlStr);
            if (dt.Rows[0][0].ToString().Trim() != "")
            {
                Vid = dt.Rows[0][0].ToString().Trim();                
            }
            return Vid;
        }

        public static string GetValue(string column, string table, string wherestr)
        {
            string Vid = "";
            string SqlStr = "select " + column + " from " + table + " where " + wherestr + "";
            DataTable dt = GetDataTable(SqlStr);
            if (dt.Rows.Count==0) return "";
            if (dt.Rows[0][0].ToString().Trim() != "")
            {
                Vid = dt.Rows[0][0].ToString().Trim();
            }
            return Vid;
        }

        /// <summary>取得行列數</summary>
        /// <param name="table">資料表</param>  
        /// <param name="wherestr">查詢字串</param>  
        public static int GetRowCount(string table, string wherestr)
        {
            string Vid = "0";
            string SqlStr = "select Count(*) from " + table + " where " + wherestr + "";
            DataTable dt = GetDataTable(SqlStr);
            if (dt.Rows[0][0].ToString().Trim() != "")
            {
                Vid = dt.Rows[0][0].ToString().Trim();
            }
            return Convert.ToInt32(Vid);
        }

        public static string GetNightMarketName(string sNo)
        {
            string Vid = "";
            string SqlStr = "select NightMarketName from NightMarket where NightMarketNo = '" + sNo + "'";
            DataTable dt = GetDataTable(SqlStr);
            if (dt.Rows[0][0].ToString().Trim() != "")
            {
                Vid = dt.Rows[0][0].ToString().Trim();
            }
            return Vid;
        }

        public static string GetMax1(string table, string column)
        {
            string Vid = "";
            string SqlStr = "select max(Convert(int," + column + ")) from " + table + "";
            DataTable dt = GetDataTable(SqlStr);
            if (dt.Rows[0][0].ToString().Trim() != "")
            {
                Vid = dt.Rows[0][0].ToString().Trim();
                Vid = Convert.ToString(Convert.ToInt32(Vid) + 1);
            }
            else
            {
                Vid = "1";
            }
            return Vid;
        }

        public static string ChangeText(string str)
        {
            string S = "元整";
            int j = 0;
            for (int i = str.Length; i > 0; i--)
            {
                if (str.Substring(i - 1, 1) == "0")
                {
                    if (j==4)
                        S = "萬" + S;
                    else
                        S = "" + S;
                }
                else
                {
                    switch (j)
                    {
                        case 1:
                            S = "拾" + S;
                            break;
                        case 2:
                            S = "佰" + S;
                            break;
                        case 3:
                            S = "仟" + S;
                            break;
                        case 4:
                            S = "萬" + S;
                            break;
                        case 5:
                            S = "拾" + S;
                            break;
                        case 6:
                            S = "佰" + S;
                            break;
                    }
                }                

                j++;
                
                //if (str.Substring(i - 1, 1) == "0")
                //    S = "零" + S;
                if (str.Substring(i - 1, 1) == "1")
                    S = "壹" + S;
                else if (str.Substring(i - 1, 1) == "2")
                    S = "貳" + S;
                else if (str.Substring(i - 1, 1) == "3")
                    S = "參" + S;
                else if (str.Substring(i - 1, 1) == "4")
                    S = "肆" + S;
                else if (str.Substring(i - 1, 1) == "5")
                    S = "伍" + S;
                else if (str.Substring(i - 1, 1) == "6")
                    S = "陸" + S;
                else if (str.Substring(i - 1, 1) == "7")
                    S = "柒" + S;
                else if (str.Substring(i - 1, 1) == "8")
                    S = "捌" + S;
                else if (str.Substring(i - 1, 1) == "9")
                    S = "玖" + S;                
            }
            return S;
        }

        public static void cbo_choose(string [,] arr, string type, ComboBox cboname)
        {
            for (int i = 0; i <= arr.GetLength(0) - 1; i++)
            {
                if (type == arr[i, 1])
                {
                    cboname.SelectedIndex = i;
                    break;
                }
            }
        }

        public static void DropDownList(string column1, string column2, string table, ComboBox cboname)
        {            
            string SqlStr = "Select " + column1 + "=''," + column2 + "='' from " + table + " Union"
            //+ " Select Rtrim(" + column1 + "),Rtrim(" + column1 + ") +' '+ Rtrim(" + column2 + ") from " + table
            + " Select Rtrim(" + column1 + "),Rtrim(" + column2 + ") from " + table
            + " group by " + column1 + "," + column2 + " order by " + column1;
            DataTable combodt = GetDataTable(SqlStr);            
            cboname.DataSource = combodt;
            cboname.ValueMember = column1;
            cboname.DisplayMember = column2;           
        }
        //適用2個欄位
        public static void DropDownList_A(string column1, string column2, string table, ComboBox cboname, string whereStr)
        {
            string SqlStr = "Select " + column1 + "=''," + column2 + "='' from " + table + " Union"
            + " Select Rtrim(" + column1 + "),Rtrim(" + column1 + ") +' '+ Rtrim(" + column2 + ") from " + table
            + " " + whereStr
            + " group by " + column1 + "," + column2 + " order by " + column1;
            DataTable combodt = GetDataTable(SqlStr);
            cboname.DataSource = combodt;
            cboname.ValueMember = column1;
            cboname.DisplayMember = column2;
        }
        //適用單一欄位
        public static void DropDownList_B(string column1, string table, ComboBox cboname, string whereStr)
        {
            string SqlStr = "Select " + column1 + "='' from " + table + " Union"
            + " Select Rtrim(" + column1 + ") from " + table
            + " " + whereStr
            + " group by " + column1 + " order by " + column1;
            DataTable combodt = GetDataTable(SqlStr);
            cboname.DataSource = combodt;
            cboname.ValueMember = column1;
            cboname.DisplayMember = column1;
        }

        ///// <summary>適用兩張資料表合併兩個欄位</summary>
        ///// <param name="table_A">資料表A</param>
        ///// <param name="table_B">資料表B</param>
        ///// <param name="cbonme">下拉欄位</param>
        ///// <param name="where_str">查詢字串</param>
        //public static void DropDownList_C(string column1, string column2, string table_A, string table_B, ComboBox cboname, string whereStr) 
        //{
        //    string SqlStr = "Select " + column1 + "=''," + column2 + "='' from " + table_A + " Union"
        //    + " Select Rtrim(" + column1 + "),Rtrim(" + column1 + ") +' '+ Rtrim(" + column2 + ") from " + table_B
        //    + " " + whereStr
        //    + " group by " + column1 + "," + column2 + " order by " + column1;
        //    DataTable combodt = GetDataTable(SqlStr);
        //    cboname.DataSource = combodt;
        //    cboname.ValueMember = column1;
        //    cboname.DisplayMember = column2;
        //}

        
        public static string frontText(string str)
        {
            string S = "";
            for (int i = 0; i <= str.Length; i++)
            {
                if (str.Substring(i, 1) == ")")
                    break;
                else
                {
                    S = S + str.Substring(i, 1);
                }
            }
            return S;
        }

        public static string behindText(string str)
        {
            string S = "";
            for (int i = str.Length; i > 0; i--)
            {
                if (str.Substring(i - 1, 1) == "(")
                    break;
                else
                {
                    S = str.Substring(i - 1, 1) + S;
                }
            }
            return S;
        }

        public static string InsertText(string str)
        {
            string S = ""; int j = 1;
            for (int i = str.Length; i > 0; i--)
            {
                if (j % 3 == 0)
                {
                    S = "," + str.Substring(i - 1, 1) + S;
                }
                else
                {
                    S = str.Substring(i - 1, 1) + S;
                }
                j++;
            }
            if (S.Substring(0, 1) == ",") S = S.Substring(1);
            return S;
        }

        public static string InsertText1(string str)
        {
            string S = ""; int j = 1;
            for (int i = str.Length; i > 0; i--)
            {
                if (j % 1 == 0)
                {
                    S = "   " + str.Substring(i - 1, 1) + S;
                }
                else
                {
                    S = str.Substring(i - 1, 1) + S;
                }
                j++;
            }
            if (S.Substring(0, 1) == "   ") S = S.Substring(1);
            return S;
        }

        public static string ToFullTaiwanDate(DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            return string.Format("民國 {0} 年 {1} 月 {2} 日",
                taiwanCalendar.GetYear(datetime),datetime.Month,datetime.Day);
        }

        public static string ToSimpleTaiwanDate(DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            return string.Format("{0}/{1}/{2}",
                taiwanCalendar.GetYear(datetime), datetime.Month, datetime.Day);
        }

        public static string ToShortTaiwanDate(DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            return string.Format("{0}{1}{2}",
                taiwanCalendar.GetYear(datetime).ToString("000"), datetime.Month.ToString("00"), datetime.Day.ToString("00"));
        }

        public static string ToLongSimpleTaiwanDate(DateTime datetime)
        {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            return string.Format("{0}年{1}月{2}日",
                taiwanCalendar.GetYear(datetime), datetime.Month, datetime.Day);
        }

        public static string LongTaiwanDateToWestDate(string str)
        {
            //民國轉西元
            return Convert.ToString(Convert.ToInt32(str.Substring(0, 2)) + 1911) + "/" + str.Substring(3, 2) + "/" + str.Substring(6, 2);            
        }

        public static string LongWestDateDateToTaiwan(string str)
        {
            //西元轉民國
            return Convert.ToString(Convert.ToInt32(str.Substring(0, 4)) - 1911) + "/" + str.Substring(5, 2) + "/" + str.Substring(8, 2);
        }

        public static void chkLBData(CheckedListBox chkLB, string str)
        {
            bool f = false;
            if (chkLB.Items.Count == 0)
                chkLB.Items.Add(str, false);
            else
            {
                for (int i = 0; i < chkLB.Items.Count; i++)
                {
                    if (chkLB.Items[i].ToString().Trim() == str)
                    {
                        f = true;
                        break;
                    }                    
                }
                if (f == false) chkLB.Items.Add(str, false);
            }
        }
    }
}
