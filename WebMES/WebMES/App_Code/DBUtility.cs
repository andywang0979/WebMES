using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//
using System.Web.UI;
using System.Web.UI.WebControls;
//
using System.Data;
using System.Data.SqlClient;
//
using System.Data.OleDb;
//
using System.IO;
//
using System.Net;
// ScriptManager  使用
using System.Web.Configuration;
//SqlHelper
using CPC.Utility.SQL;
//
using DevExpress.Web;


//在App_Code寫程式
//Session()要換成HttpContext.Current.Session()
//Response.Write()要換成System.Web.HttpContext.Current.Response.Write()
//Request系列也要換成HttpContext.Current.Request


/// <summary>
/// DBUtility 的摘要描述
/// </summary>
public class DBUtility
{
    //public DBUtility()
    //{

    //lvwPrsn.DataSource = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
    //跨類別變數
    static string CurDptList = "";
    static DataTable dtOrganAll = new DataTable();
    static string CurMaList = "";
    static DataTable dtMaBomAll = new DataTable();
    static DataTable dtMaRoutBomAll = new DataTable();

    //ExecuteNonQuery用來執行INSERT、UPDATE、DELETE和其他沒有返回值得SQL命令。
    // 例如：CREATE DATABASE 和 CREATE TABLE 命令
    public static int ExecuteNonQuery(string CurConnectionString, string strSQL, int CmdTimeout = 90)
    {
        using (SqlConnection conn = new SqlConnection(CurConnectionString))
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSQL;
            cmd.CommandTimeout = CmdTimeout;
            int result = cmd.ExecuteNonQuery();
            //cmd.Parameters.Clear();
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return result;
        }
    }

    //ExecuteScalar 執行一個SQL命令返回結果集的第一列的第一行。
    // 它經常用來執行SQL的COUNT、AVG、MIN、MAX 和 SUM 函數，這些函數都是返回單行單列的結果集。
    public static object ExecuteScalar(string CurConnectionString, string strSQL)
    {
        using (SqlConnection conn = new SqlConnection(CurConnectionString))
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSQL;

            object result = cmd.ExecuteScalar();

            cmd.Parameters.Clear();
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return result;
        }
    }

    public static bool IsDataExist(string CurConnectionString, string SelectStr)
    {
        using (SqlConnection cn = new SqlConnection())
        {
            cn.ConnectionString = CurConnectionString;
            cn.Open();

            SqlCommand cmd = new SqlCommand(SelectStr, cn);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                dr.Close();
                return true;
            }
            else
            {
                dr.Close();
                return false;
            }
        }


        /*
        SqlConnection Conn = new SqlConnection(CurConnectionString);
        DataTable myDataTable = new DataTable();
        SqlDataAdapter myAdapter = new SqlDataAdapter(SelectStr, Conn);
        DataSet ds = new DataSet();
        myAdapter.Fill(ds);    //---- 這時候執行SQL指令。取出資料，放進 DataSet。
        myDataTable = ds.Tables[0];
        if (Conn.State == ConnectionState.Open) Conn.Close();
        if (myDataTable.Rows.Count == 0)
            return false;
        else
            return true;
        */
    }



    public static string GetFormatPreStr(DateTime CurDate, string FormatStr)
    {
        int Preyr = CurDate.Year;
        int Premn = CurDate.Month;
        int Predy = CurDate.Day;
        string HeadStr = "";
        if (FormatStr.IndexOf("yyyy") >= 0)
            HeadStr = Preyr.ToString();
        else if (FormatStr.IndexOf("yy") >= 0)
            HeadStr = Preyr.ToString();
        else if (FormatStr.IndexOf("eee") >= 0)
        {
            HeadStr = (Preyr - 1911).ToString();
            HeadStr.PadLeft(3, '0');
            // HeadStr = (Preyr - 1911).ToString("000");
        }
        else if (FormatStr.IndexOf("ee") >= 0)
        {
            HeadStr = (Preyr - 1911).ToString();
            HeadStr.PadLeft(2, '0');
            // HeadStr = (Preyr - 1911).ToString("00");
        }
        if (FormatStr.IndexOf("mm") >= 0)
            HeadStr = HeadStr + Premn.ToString().PadLeft(2, '0');
        if (FormatStr.IndexOf("dd") >= 0)
            HeadStr = HeadStr + Predy.ToString().PadLeft(2, '0');
        return HeadStr;
    }


    public static string RAdoGetFormatCode(string CurConnectionString, string GLTableName, string GLFieldName, string GLWhereCond, Boolean IsIncrement, string GLFormatStr, DateTime FReldate, string HeadFixStr)
    //function RAdoGetFormatCode(GLRemoteServer:TAdoConnection;GLTableName,GLFieldName,GLWhereCond:string;IsIncrement:Boolean;GLFormatStr:string;FReldate:TDate;HeadFixStr:string=''):Variant;
    {
        DataTable GLQuery = new DataTable();
        string HeadStr = "";
        string CmdText = "";
        if (FReldate == null)
        {
            if (GLFormatStr.IndexOf("#") > 0)
                HeadStr = GLFormatStr.Substring(1, GLFormatStr.IndexOf("#") - 1);
        }
        else if ((!string.IsNullOrEmpty(GLFormatStr)) && (!string.IsNullOrEmpty(FReldate.ToString())))
            HeadStr = GetFormatPreStr(FReldate, GLFormatStr);
        //HeadStr = String.Format("0:" + FReldate.ToString().Replace("/", ""), GLFormatStr);
        HeadStr = HeadFixStr + HeadStr;
        CmdText = "Select Max(" + GLFieldName + ") as LastFieldValue From " + GLTableName;
        if (GLFieldName == "*")
            //使用 COUNT(*) --> LastSrlNumStr 代表合乎條件的紀錄筆數
            CmdText = "Select COUNT(" + GLFieldName + ") as LastFieldValue From " + GLTableName;
        if ((!string.IsNullOrEmpty(GLWhereCond)) && (GLWhereCond.Length > 0))
            CmdText = CmdText + " Where " + GLWhereCond;
        if ((HeadStr.Length > 0) && (GLFieldName != "*"))
        {
            if (CmdText.IndexOf(" Where ") >= 0)
                CmdText = CmdText + " And SubString(" + GLFieldName + ",1," + HeadStr.Length.ToString() + ")='" + HeadStr + "'";
            else
                CmdText = CmdText + " Where SubString(" + GLFieldName + ",1," + HeadStr.Length.ToString() + ")='" + HeadStr + "'";
        }
        GLQuery = DBUtility.RefetchDataTable(CurConnectionString, CmdText);
        if (IsIncrement)
        {
            switch (Type.GetTypeCode(GLQuery.Columns["LastFieldValue"].DataType))
            //switch (Type.GetTypeCode(((DataColumn)GLQuery.Rows[0]["LastFieldValue"]).DataType))
            {
                case TypeCode.String:
                    // Handle String
                    if (GLQuery.Rows[0]["LastFieldValue"].ToString() == "")
                    {
                        if ((GLWhereCond.Length > 0) && (GLWhereCond.IndexOf(GLFieldName + "=") >= 0))
                        //判斷GLFieldName欄位名稱是否存在於GLWhereCond條件字串中
                        {
                            string GLFldValStr = GLWhereCond.Substring(GLWhereCond.IndexOf(GLFieldName + "=") + GLFieldName.Length + 2);
                            GLFldValStr = GLFldValStr.Substring(0, GLFldValStr.IndexOf("'"));
                            //GLFldValStr = GLFldValStr.Substring(1, GLFldValStr.IndexOf("'") - 1);
                            return (HeadStr + GLFldValStr).PadRight(GLFormatStr.Length + HeadFixStr.Length - 1, '0') + "1";
                            //return HeadStr.PadRight(GLFormatStr.Length - (GLFormatStr.IndexOf("#") - 1) - 1, '0') + "1";
                            //return HeadStr+Copy(GLFldValStr+StringOfChar('0',Length(GLFormatStr)-Length(GLFldValStr)-Length(HeadStr)-1)+'1',1,254);
                        }
                        else
                            return HeadStr.PadRight(GLFormatStr.Length + HeadFixStr.Length - 1, '0') + "1";
                        //return HeadStr.PadRight(GLFormatStr.Length - (GLFormatStr.IndexOf("#") - 1) - 1, '0') + "1";
                        //return HeadStr+StringOfChar(  '0',Length(GLFormatStr)-(Pos('#',GLFormatStr)-1)-1  )+'1';
                    }
                    else
                    {
                        string LastSrlNumStr = GLQuery.Rows[0]["LastFieldValue"].ToString().Substring(HeadStr.Length, GLQuery.Rows[0]["LastFieldValue"].ToString().Length - HeadStr.Length);
                        int LastSrlNum = Convert.ToInt32(LastSrlNumStr) + 1;
                        string NewSrlNumStr = LastSrlNum.ToString().PadLeft(LastSrlNumStr.Length, '0');
                        return HeadStr + NewSrlNumStr;
                        //return HeadStr+StrInc(Copy(GLQuery.Rows[0]["LastFieldValue"].AsString,Length(HeadStr)+1,Length(GLQuery.FieldByName('LastFieldValue').AsString)-Length(Headstr)),1);
                    }
                //break;
                default:
                    //非字串
                    if (GLFieldName == "*")
                    {
                        //使用 COUNT(*) --> LastSrlNumStr 代表紀錄筆數
                        string LastSrlNumStr = GLQuery.Rows[0]["LastFieldValue"].ToString();
                        int NewSrlNum = Convert.ToInt32(LastSrlNumStr) + 1;
                        //int SrlNumStrLen = GLFormatStr.LastIndexOf("#") - GLFormatStr.IndexOf("#");
                        //string NewSrlNumStr = LastSrlNum.ToString().PadLeft(LastSrlNumStr.Length, '0');
                        return HeadStr.PadRight(GLFormatStr.Length - NewSrlNum.ToString().Length, '0') + NewSrlNum.ToString();
                        //return HeadStr.PadRight(SrlNumStrLen - NewSrlNum.ToString().Length, '0') + NewSrlNum.ToString();
                    }
                    else
                        return (Convert.ToInt16(GLQuery.Rows[0]["LastFieldValue"].ToString()) + 1).ToString().PadLeft(GLFormatStr.Length, '0');
                    //break;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(GLQuery.Rows[0]["LastFieldValue"].ToString()))
            {
                switch (Type.GetTypeCode(GLQuery.Columns["LastFieldValue"].DataType))
                //switch (Type.GetTypeCode(((DataColumn)GLQuery.Rows[0]["LastFieldValue"]).DataType))
                {
                    case TypeCode.String:
                        // Handle String
                        return "";
                    //break;
                    default:
                        return null;
                        //break;
                }
            }
            else
                return GLQuery.Rows[0]["LastFieldValue"].ToString();
            //Result:=GLQuery.FieldByName('LastFieldValue').Value;
        }
    }


    public static object RefetchFieldValueOfTable(string CurConnectionString, string SelectStr, string FieldName)
    {
        SqlConnection Conn = new SqlConnection(CurConnectionString);
        DataTable myDataTable = new DataTable();
        SqlDataAdapter myAdapter = new SqlDataAdapter(SelectStr, Conn);

        DataSet ds = new DataSet();

        myAdapter.Fill(ds);    //---- 這時候執行SQL指令。取出資料，放進 DataSet。
        myDataTable = ds.Tables[0];
        if (Conn.State == ConnectionState.Open) Conn.Close();
        return myDataTable.Rows[0][FieldName];
    }

    public static DataTable RefetchDataTable(string CurConnectionString, string SelectStr, int CmdTimeout = 30)
    {
        SqlConnection Conn = new SqlConnection(CurConnectionString);
        DataTable myDataTable = new DataTable();
        SqlDataAdapter myAdapter = new SqlDataAdapter(SelectStr, Conn);
        myAdapter.SelectCommand.CommandTimeout = CmdTimeout;
        DataSet ds = new DataSet();

        myAdapter.Fill(ds);    //---- 這時候執行SQL指令。取出資料，放進 DataSet。
        myDataTable = ds.Tables[0];
        if (Conn.State == ConnectionState.Open) Conn.Close();
        return myDataTable;
    }

    public static SqlConnection OpenSqlConn(string Server, string Database, string dbuid, string dbpwd)
    {
        string cnstr = string.Format("server={0};database={1};uid={2};pwd={3};Connect Timeout = 180", Server, Database, dbuid, dbpwd);
        SqlConnection icn = new SqlConnection();
        icn.ConnectionString = cnstr;
        if (icn.State == ConnectionState.Open) icn.Close();
        icn.Open();
        return icn;
    }


    //string sql = "select * from TableName";
    //DataTable myDataTable = GetSqlDataTable("ServerName", "DataBaseName", "UserName", "PassWord", sql);

    public static DataTable GetSqlDataTable(string Server, string Database, string dbuid, string dbpwd, string SqlString)
    {
        DataTable myDataTable = new DataTable();
        SqlConnection icn = null;
        icn = OpenSqlConn(Server, Database, dbuid, dbpwd);
        SqlCommand isc = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter(isc);
        isc.Connection = icn;
        isc.CommandText = SqlString;
        isc.CommandTimeout = 600;
        DataSet ds = new DataSet();
        ds.Clear();
        da.Fill(ds);
        myDataTable = ds.Tables[0];
        if (icn.State == ConnectionState.Open) icn.Close();
        return myDataTable;
    }

    public static DataTable GetOleDataTable(string FileName, string SheetName)
    {
        //選擇Excel版本
        //Excel 12.0 針對Excel 2010、2007版本(OLEDB.12.0)
        //Excel 8.0 針對Excel 97-2003版本(OLEDB.4.0)
        //Excel 5.0 針對Excel 97(OLEDB.4.0)
        //"Extended Properties='Excel 8.0;" 

        //開頭是否為資料
        //若指定值為 HDR=Yes，代表 Excel 檔中的工作表第一列是欄位名稱，oleDB直接從第二列讀取
        //若指定值為 HDR=No，代表 Excel 檔中的工作表第一列就是資料了，沒有欄位名稱，oleDB直接從第一列讀取

        //IMEX=0 為「匯出模式」，能對檔案進行寫入的動作。
        //IMEX=1 為「匯入模式」，能對檔案進行讀取的動作。
        //IMEX=2 為「連結模式」，能對檔案進行讀取與寫入的動作。

        string ImportConnectionString = "";
        if (FileName.IndexOf(".xlsx") >= 0)
        {
            //office2007後的Excel版本用這個連接，若連接不上，請找AccessDatabaseEngine去下載引擎
            ImportConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0; "
                                    + "Data Source= " + FileName + ";"
                                    + "Extended Properties=Excel 12.0 Xml;HDR=YES; ";
            //Microsoft.ACE.OLEDB.12.0;Data Source=c:\test.xlsx;Extended Properties=""Excel 12.0 Xml;HDR=YES"";
        }
        else
        {
            //office2003前的Excel版本用這個連接
            ImportConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0; "
                                    + "Data Source= " + FileName + ";"
                                    + "Mode=ReadWrite|Share Deny None; "
                                    + "Extended Properties=IMEX=1;Excel 8.0;Persist Security Info=False ";
            //+"Extended Properties=IMEX=1;Excel 8.0;Persist Security Info=False' ";
        }
        OleDbConnection connection = new OleDbConnection(ImportConnectionString);
        //OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FileName + ";Extended Properties=Excel 8.0;");
        connection.Open();
        string query = "select * from [" + SheetName + "$]";
        OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
        DataSet ds = new DataSet();
        adapter.Fill(ds);
        return ds.Tables[0];
    }

    public DataTable TextFileToDataTable(string File, string TableName, string delimiter)
    {
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        StreamReader s = new StreamReader(File, System.Text.Encoding.Default);
        //string ss = s.ReadLine();//skip the first line
        string[] columns = s.ReadLine().Split(delimiter.ToCharArray());
        ds.Tables.Add(TableName);
        foreach (string col in columns)
        {
            bool added = false;
            string next = "";
            int i = 0;
            while (!added)
            {
                string columnname = col + next;
                columnname = columnname.Replace("#", "");
                columnname = columnname.Replace("'", "");
                columnname = columnname.Replace("&", "");

                if (!ds.Tables[TableName].Columns.Contains(columnname))
                {
                    ds.Tables[TableName].Columns.Add(columnname.ToUpper());
                    added = true;
                }
                else
                {
                    i++;
                    next = "_" + i.ToString();
                }
            }
        }

        string AllData = s.ReadToEnd();
        string[] rows = AllData.Split("\n".ToCharArray());

        foreach (string r in rows)
        {
            string[] items = r.Split(delimiter.ToCharArray());
            ds.Tables[TableName].Rows.Add(items);
        }

        s.Close();

        dt = ds.Tables[0];

        return dt;
    }


    public static bool Logoned(Boolean ShowMessage)
    {
        if (HttpContext.Current.Session["CurUserID"] == null || HttpContext.Current.Session["CurUserID"].ToString() == "")
            //if (string.IsNullOrEmpty(HttpContext.Current.Session["CurUserID"]))
            //if (ShowMessage)
            //    MessageBox.Show(System.Web.UI.Page, "尚未登入!!");
            return false;
        else
        {
            return true;
        }

    }

    public static bool CusLogoned(Boolean ShowMessage)
    {
        if (HttpContext.Current.Session["CurUserID"] == null || HttpContext.Current.Session["CurUserID"].ToString() == "")
            //if (string.IsNullOrEmpty(HttpContext.Current.Session["CurUserID"]))
            //if (ShowMessage)
            //    MessageBox.Show(System.Web.UI.Page, "尚未登入!!");
            return false;
        else
        {
            return true;
        }

    }



    public static bool UserLogoned(Boolean ShowMessage)
    {
        if (HttpContext.Current.Session["BlogUserID"] == null || HttpContext.Current.Session["BlogUserID"].ToString() == "")
            //if (string.IsNullOrEmpty(HttpContext.Current.Session["CurUserID"]))
            //if (ShowMessage)
            //    MessageBox.Show(System.Web.UI.Page, "尚未登入!!");
            return false;
        else
        {
            return true;
        }

    }


    //判斷使用者是否擁有該權限識別碼的權限
    public static bool IsOwnRight(string PGID, Boolean MSGON)
    {

        string MPGID;
        string NZeroStr;
        string UserRight;
        UserRight = (string)HttpContext.Current.Session["UserRight"];
        int NZeroLen;
        if (UserRight.IndexOf("00000") >= 0)
            return true;
        else if (UserRight.IndexOf(PGID) >= 0)
            return true;
        //else if ((PGID.IndexOf("0") >= 0) && (UserRight.IndexOf("," + PGID.Substring(0, PGID.IndexOf("0"))) > 0))
        //    return true;
        //else if ((PGID.IndexOf("0") >= 0) && (UserRight == PGID.Substring(0, PGID.IndexOf("0"))))
        //    return true;
        else
        {
            Boolean FinCheck = false;
            MPGID = PGID;
            //NZeroLen = MPGID.IndexOf("0") - 1;
            //if (NZeroLen == -1)
            //    NZeroLen = MPGID.Length;
            do
            {
                NZeroLen = MPGID.IndexOf("0") - 1;
                if (NZeroLen == -1)
                    NZeroLen = MPGID.Length - 1;
                NZeroStr = MPGID.Substring(0, NZeroLen);
                MPGID = NZeroStr.PadRight(5, '0');
                if (UserRight.IndexOf(MPGID) >= 0)
                {
                    FinCheck = true;
                    break;
                }
            }
            while (NZeroLen > 0);
            /*
            while (NZeroLen > 0)
            {
                NZeroLen = MPGID.IndexOf("0") - 1;
                if (NZeroLen == -1)
                    NZeroLen = MPGID.Length;
                NZeroStr = MPGID.Substring(0, NZeroLen);
                MPGID = NZeroStr.PadRight(5, '0');
                if (UserRight.IndexOf(MPGID) >= 0)
                {
                    return true;
                    break;
                }
            }  //while (NZeroLen > 0)
             */
            //if (MSGON)
            //    MessageBox.Show(System.Web.UI.Page, "使用者無此作業權限 !!");
            return FinCheck;
        }
    }


    //判斷使用者是否擁有該權限識別碼的權限
    public static bool IsGotRight(string PGID)
    {
        //判斷使用者是否擁有該權限識別碼的權限
        //PGID    =>作業權限代號
        //MSGON   =>.T.(營幕顯示錯誤訊息),.F.((營幕不顯示錯誤訊息)
        //Session.UserRight=>變數內容儲存使用者在整個系統的權限代碼(EX. USERIGHT="A0000,B0000,C1000,D3100")
        string MPGID;
        string NZeroStr;
        string UserRight;
        UserRight = (string)HttpContext.Current.Session["UserRight"];
        int NZeroLen;
        if (UserRight.IndexOf(PGID) >= 0)
            return true;
        /*
        else if ((PGID.IndexOf("0") > 0) && (UserRight.IndexOf("," + PGID.Substring(0, PGID.IndexOf("0"))) >= 0))
            //Error? PGID="00000" 不論 Session.UserRight 內容 皆return true
            //else if (Pos('0',PGID)>0) and (Pos(','+Copy(PGID,1,Pos('0',PGID)-1),Session.UserRight)>0) then
            return true;
        else if ((PGID.IndexOf("0") > 0) && (UserRight == PGID.Substring(0, PGID.IndexOf("0"))))
            //else if  (Pos('0',PGID)>0) and (Session.UserRight=Copy(PGID,1,Pos('0',PGID)-1)) then
            return true;
        */
        else
        {
            Boolean FinCheck = false;
            MPGID = PGID;
            do
            {
                NZeroLen = MPGID.IndexOf("0") - 1;
                if (NZeroLen < 0)
                    NZeroLen = MPGID.Length;
                NZeroStr = MPGID.Substring(0, NZeroLen);
                MPGID = NZeroStr.PadRight(5, '0');
                if (UserRight.IndexOf(MPGID) >= 0)
                {
                    FinCheck = true;
                    break;
                }
                if (MPGID == "00000")
                {
                    FinCheck = false;
                    break;
                }

            } while (true);

            return FinCheck;
        }

    }

    //判斷使用者是否擁有該權限識別碼的權限
    public static bool IsOwnChild(string PGID)
    {
        //EX. USERIGHT="A0000,B0000,C1000,D3100"
        string MPGID;
        string NZeroStr;
        string UserRight;
        int RightItmCnt;
        string MChildID;
        int NZeroLen;
        UserRight = (string)HttpContext.Current.Session["UserRight"];
        if (UserRight.IndexOf("00000") >= 0)
            return true;
        else if ((UserRight.IndexOf(PGID) >= 0) || (UserRight.IndexOf("," + PGID.Substring(0, PGID.IndexOf("0"))) >= 0))
            return true;
        else
        {
            Boolean FinCheck = false;
            MPGID = PGID;
            NZeroLen = MPGID.IndexOf("0");
            NZeroStr = MPGID.Substring(0, NZeroLen);
            RightItmCnt = (UserRight.Length + 1) / 6;
            for (int i = 0; i < RightItmCnt; i++)
            {
                MChildID = UserRight.Substring(i * 6, 5);
                if (MChildID.Substring(0, NZeroLen) == NZeroStr)
                {
                    FinCheck = true;
                    break;
                }

            }
            return FinCheck;
        }
    }

    public static void CmdbtnSetRight(string ParentHeadStr, DevExpress.Web.GridViewCommandColumn CurCommandColumn, int GridPtr, DevExpress.Web.MenuItem QuryMenuItem = null, DevExpress.Web.MenuItem PrntMenuItem = null)
    {
        //根據權限代碼動態設定增刪修按鍵的顯示否
        //string FormID = this.Form.ID;
        //string ParentHeadStr;
        //ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
        switch (GridPtr)
        {
            case 1:
                if (CurCommandColumn != null)
                {
                    //CurCommandColumn == null  --> 查詢列印畫面無須設定增刪修功能
                    if ((CurCommandColumn.Grid.SettingsDataSecurity.AllowInsert) && (CurCommandColumn.ShowNewButtonInHeader))
                        CurCommandColumn.ShowNewButtonInHeader = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "A").PadRight(5, '0'), false);
                    if ((CurCommandColumn.Grid.SettingsDataSecurity.AllowEdit) && (CurCommandColumn.ShowEditButton))
                        CurCommandColumn.ShowEditButton = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "B").PadRight(5, '0'), false);
                    if ((CurCommandColumn.Grid.SettingsDataSecurity.AllowDelete) && (CurCommandColumn.ShowDeleteButton))
                        CurCommandColumn.ShowDeleteButton = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "C").PadRight(5, '0'), false);
                }
                //根據權限動態設定查詢功能顯示否
                if (QuryMenuItem != null)
                    QuryMenuItem.Visible = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "D").PadRight(5, '0'), false);
                //根據權限動態設定列印功能顯示否
                if (PrntMenuItem != null)
                    PrntMenuItem.Visible = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "E").PadRight(5, '0'), false);
                break;
            case 2:
                if ((CurCommandColumn.Grid.SettingsDataSecurity.AllowInsert) && (CurCommandColumn.ShowNewButtonInHeader))
                    CurCommandColumn.ShowNewButtonInHeader = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "F").PadRight(5, '0'), false);
                if ((CurCommandColumn.Grid.SettingsDataSecurity.AllowEdit) && (CurCommandColumn.ShowEditButton))
                    CurCommandColumn.ShowEditButton = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "G").PadRight(5, '0'), false);
                if ((CurCommandColumn.Grid.SettingsDataSecurity.AllowDelete) && (CurCommandColumn.ShowDeleteButton))
                    CurCommandColumn.ShowDeleteButton = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "H").PadRight(5, '0'), false);
                break;
            default:
                break;
        }  //case I of
    }

    public static string EnPcode(string CurPassID)
    {
        //char[] MyCharrAray = new char[10];
        //string CurrentString;

        string TranPassID = "";
        char CurPassChr;
        /*
        for (int i = 0; i <= CurPassID.Length - 1; i++)
        {       // This is where the "magic" happens – I found this 
            // method while looking for an answer for you J

            CurPassID.CopyTo(i, MyCharrAray, i, 1);

        }

        for (int i = 0; i < CurPassID.Length; i++)
        {        // This is a printing command – you can simply change 
            // it into setting the ASCII values into an int[]Aray.

            Console.WriteLine(MyCharrAray[i] + "\t" + "Ascii code is: " + (int)MyCharrAray[i]);

        }
        */
        for (int i = 1; i <= CurPassID.Length; i++)
        {
            CurPassChr = Convert.ToChar(CurPassID[i - 1]);
            //CurPassChr = Convert.ToChar(CurPassID.Substring(i - 1, 1));
            switch (i)
            {
                case 1:
                    if ((Ord(CurPassChr) - 03) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 04);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 03);
                    break;
                case 2:
                    if ((Ord(CurPassChr) - 12) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 13);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 12);
                    break;
                case 3:
                    if ((Ord(CurPassChr) - 16) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 17);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 16);
                    break;
                case 4:
                    if ((Ord(CurPassChr) - 05) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 06);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 05);
                    break;
                case 5:
                    if ((Ord(CurPassChr) - 07) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 08);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 07);
                    break;
                case 6:
                    if ((Ord(CurPassChr) - 17) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 18);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 17);
                    break;
                case 7:
                    if ((Ord(CurPassChr) - 09) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 10);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 09);
                    break;
                case 8:
                    if ((Ord(CurPassChr) - 06) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 07);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 06);
                    break;
                case 9:
                    if ((Ord(CurPassChr) - 04) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 05);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 04);
                    break;
                case 10:
                    if ((Ord(CurPassChr) - 11) == 32)
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 12);
                    else
                        TranPassID = TranPassID + Chr(Ord(CurPassChr) - 11);
                    break;
            }  //case I of
        }
        return TranPassID;
    }

    public static char Chr(int Num)
    {
        char C = Convert.ToChar(Num);
        return C;
    }


    public static int Ord(char C)
    {
        int N = Convert.ToInt32(C);
        return N;
    }

    /* 欄位的儲存數值對應成CheckBoxList的選項, 傳回 "選項1+選項3+.."的字串 , 例如 "個性+優雅", 可複選
     * CheckBoxList的選項 : 
            <dx:ListEditItem Text="個性" Value="1" />
            <dx:ListEditItem Text="前衛" Value="2" />
            <dx:ListEditItem Text="優雅" Value="4" />
            <dx:ListEditItem Text="時尚" Value="8" />
            <dx:ListEditItem Text="可愛" Value="16" />
            <dx:ListEditItem Text="浪漫" Value="32" />
     * 欄位的儲存數值 : 5 --> 1,4 對應成 個性+優雅, 
     * 欄位的儲存數值拆解成2昇冪, 對應ListEditItem的Value
     * 範例:
         MlblHAIRSTYLID.Text = DBUtility.PowerToNameList(CheckBoxList_HairStyl, Convert.ToInt16(MlblHAIRSTYLID.Text));
     */
    public static string PowerToNameList(CheckBoxList CurCheckBoxList, int FieldStorValue)
    {
        string NameList = "";
        int binVal = 1;
        for (int Index = 0; Index < CurCheckBoxList.Items.Count; Index++)
        // for (int Index = 0; Index < CheckBoxList_HairCurl.Items.Count; Index++)
        {
            //二進位運算(and) FieldStorValue & binVal
            int SelectValInList = FieldStorValue & binVal;
            if (SelectValInList == binVal)
            {
                NameList += (NameList == "" ? CurCheckBoxList.Items[Index].Text : "+" + CurCheckBoxList.Items[Index].Text);
            }
            binVal = binVal * 2;
        }
        return NameList;
    }

    public static string AspxPowerToNameList(ASPxCheckBoxList CurCheckBoxList, int FieldStorValue)
    {
        string NameList = "";
        int binVal = 1;
        for (int Index = 0; Index < CurCheckBoxList.Items.Count; Index++)
        // for (int Index = 0; Index < CheckBoxList_HairCurl.Items.Count; Index++)
        {
            //二進位運算(and) FieldStorValue & binVal
            int SelectValInList = FieldStorValue & binVal;
            if (SelectValInList == binVal)
            {
                NameList += (NameList == "" ? CurCheckBoxList.Items[Index].Text : "+" + CurCheckBoxList.Items[Index].Text);
            }
            binVal = binVal * 2;
        }
        return NameList;
    }


    /* 將CheckBoxList已勾選項目對應的2昇冪轉換為SQL-SELECT 的條件字串
     * CheckBoxList的選項 : 
            <dx:ListEditItem Text="個性" Value="1" />
            <dx:ListEditItem Text="前衛" Value="2" />
            <dx:ListEditItem Text="優雅" Value="4" />
            <dx:ListEditItem Text="時尚" Value="8" />
            <dx:ListEditItem Text="可愛" Value="16" />
            <dx:ListEditItem Text="浪漫" Value="32" />
     * 
          0,1  ,2      ,3  ,4 ,5 ,6 ,7  ,8
          1,2  ,4      ,8  ,16,32,64,124,248
          1,2,3,4,5,6,7,8,9,16,32,64,124,248

     * 範例:
        if (CheckBoxList_HairStyl.Items[Index].Selected)
        {
            if (WhereSetStr == "")
            {
                WhereSetStr += " HairGallery.HAIRSTYLID IN " + PowerToInList(MaxItemInStyl, Index);
            }
            else
            {
                WhereSetStr += " AND HairGallery.HAIRSTYLID IN " + PowerToInList(MaxItemInStyl, Index);
            }
        }
     */
    public static string PowerToInList(int MaxItemCount, int SelectPower)
    {
        //MaxItemCount:該項目所有選項數目
        //風格 : 6個選項;
        //捲度 : 2個選項;
        //長度 : 4個選項
        //MaxValue : 所有選項數目Pow(i)累加總和
        int binVal = 1;
        int MaxValue = 0;
        for (int i = 0; i < MaxItemCount; i++)
        {
            MaxValue += binVal;
            binVal = binVal * 2;
        }
        int SelectValue = Convert.ToInt16(Math.Pow(2, SelectPower));
        string strRet = "";
        for (int i = SelectValue; i <= MaxValue; i++)
        {
            //二進位運算(and) SelectValue & i
            int SelectValInList = SelectValue & i;
            if (SelectValInList == SelectValue)
            {
                strRet += "," + i.ToString();
            }
        }
        return "(" + strRet.Substring(1) + ")";
    }

    /* 將欄位的儲存數值拆解成2昇冪對應ListEditItem的Value, 並將CheckBoxList的對應選項打勾
     * CheckBoxList的選項 : 
            <dx:ListEditItem Text="個性" Value="1" />
            <dx:ListEditItem Text="前衛" Value="2" />
            <dx:ListEditItem Text="優雅" Value="4" />
            <dx:ListEditItem Text="時尚" Value="8" />
            <dx:ListEditItem Text="可愛" Value="16" />
            <dx:ListEditItem Text="浪漫" Value="32" />
     * 欄位的儲存數值 : 5 --> 1,4 CheckBoxList的 個性 & 優雅選項打勾, 
     * 範例:
            ASPxCheckBoxList CurHAIRSTYLID = sender as ASPxCheckBoxList;
            //載入風格資料到CurHAIRSTYLID
            DataTable dtbHairStyl = ((DataView)sdsHairStyl.Select(new DataSourceSelectArguments())).ToTable();
            foreach (DataRow rowHairStyl in dtbHairStyl.Rows)
            {
                ListEditItem leiHairStyl = new ListEditItem();
                leiHairStyl.Text = rowHairStyl["HAIRSTYL"].ToString();
                leiHairStyl.Value = rowHairStyl["HAIRSTYLID"].ToString();
                CurHAIRSTYLID.Items.Add(leiHairStyl);
            }
            object obtHAIRSTYLID = cvwHairGaly.GetCardValues(cvwHairGaly.FocusedCardIndex, "HAIRSTYLID");
            if (obtHAIRSTYLID != DBNull.Value)
                FieldMapToCheckBoxList(CurHAIRSTYLID, Convert.ToInt16(obtHAIRSTYLID.ToString()));
     */
    public static void FieldMapToCheckBoxList(ASPxCheckBoxList CurCheckBoxList, int FieldStorValue)
    {
        int binVal = 1;
        for (int Index = 0; Index < CurCheckBoxList.Items.Count; Index++)
        // for (int Index = 0; Index < CheckBoxList_HairCurl.Items.Count; Index++)
        {
            //二進位運算(and) FieldStorValue & binVal
            int SelectValInList = FieldStorValue & binVal;
            if (SelectValInList == binVal)
            {
                CurCheckBoxList.Items[Index].Selected = true;
            }
            binVal = binVal * 2;
        }
    }

    //將ASPxCheckBoxList的所有打勾選項, 以2昇冪加總後傳回(傳回值 = 對應欄位的儲存值) 
    public static int CheckBoxListMapToField(ASPxCheckBoxList CurCheckBoxList)
    {
        int FieldValue = 0;
        for (int Index = 0; Index < CurCheckBoxList.Items.Count; Index++)
        {
            if (CurCheckBoxList.Items[Index].Selected)
            {
                int SelectItemValue = Convert.ToInt16(Math.Pow(2, Index));
                FieldValue += SelectItemValue;
            }
        }
        return FieldValue;
    }

    public static void FieldMapToCheckBoxListAsp(CheckBoxList CurCheckBoxList, int FieldStorValue)
    {
        int binVal = 1;
        for (int Index = 0; Index < CurCheckBoxList.Items.Count; Index++)
        // for (int Index = 0; Index < CheckBoxList_HairCurl.Items.Count; Index++)
        {
            //二進位運算(and) FieldStorValue & binVal
            int SelectValInList = FieldStorValue & binVal;
            if (SelectValInList == binVal)
            {
                CurCheckBoxList.Items[Index].Selected = true;
            }
            binVal = binVal * 2;
        }
    }

    public static DateTime FirstDayOfWeek(DateTime ThisDate)
    {
        int day = (int)ThisDate.DayOfWeek;
        return ThisDate.AddDays((-1) * (day == 0 ? 6 : day - 1));
    }

    public static DateTime LastDayOfWeek(DateTime ThisDate)
    {
        int day = (int)ThisDate.DayOfWeek;
        return ThisDate.AddDays((1) * (day == 0 ? day : 7 - day));
    }

    public static DateTime FirstDayOfMonth(DateTime ThisDate)
    {
        //DateTime FirstDay = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
        return new DateTime(ThisDate.Year, ThisDate.Month, 1);
    }

    public static DateTime LastDayOfMonth(DateTime ThisDate)
    {
        //DateTime LastDay = new DateTime(DateTime.Now.AddMonths(1).Year,DateTime.Now.AddMonths(1).Month,1).AddDays(-1);
        return new DateTime(ThisDate.AddMonths(1).Year, ThisDate.AddMonths(1).Month, 1).AddDays(-1);
    }
    public static DateTime FirstDayOfYear(DateTime ThisDate)
    {
        return new DateTime(ThisDate.Year, 1, 1); ;
    }

    public static DateTime LastDayOfYear(DateTime ThisDate)
    {
        return new DateTime(ThisDate.Year, 12, 31); ;
    }

    public static string YMDSpan(string CurINVYYMM, int SpanMode, int SpanCnt)
    {
        //SpanMode=1=>以年為單位
        //SpanMode=2=>以月為單位
        //SpanMode=3=>以日為單位
        DateTime ThisDate = Convert.ToDateTime(CurINVYYMM.Substring(0, 4) + "/" + CurINVYYMM.Substring(4, 2) + "/01");
        if (SpanMode == 1)
            return ThisDate.AddYears(SpanCnt).Year.ToString().PadLeft(4, '0') + ThisDate.AddYears(SpanCnt).Month.ToString().PadLeft(2, '0');
        else if (SpanMode == 2)
            return ThisDate.AddMonths(SpanCnt).Year.ToString().PadLeft(4, '0') + ThisDate.AddMonths(SpanCnt).Month.ToString().PadLeft(2, '0');
        else if (SpanMode == 3)
            return ThisDate.AddDays(SpanCnt).Year.ToString().PadLeft(4, '0') + ThisDate.AddDays(SpanCnt).Month.ToString().PadLeft(2, '0');
        else
            return ThisDate.AddMonths(SpanCnt).Year.ToString().PadLeft(4, '0') + ThisDate.AddMonths(SpanCnt).Month.ToString().PadLeft(2, '0');

    }


    public static int DaysBetween(DateTime EndDate, DateTime BeginDate)
    {
        //取得兩個日期之間的「天數」（不足一天者採「無條件刪去法」）
        return new TimeSpan(EndDate.Ticks - BeginDate.Ticks).Days + 1;
        /* 
        DateTime dt1 = Convert.DateTime("2007-8-1");
        DateTime dt2 = Convert.DateTime("2007-8-15");
        TimeSpan span = dt2.Subtract(dt1);
        int dayDiff = span.Days + 1;
        */
    }

    public static DateTime MinOfDateTime(DateTime ThisDate1, DateTime ThisDate2)
    {
        if (DateTime.Compare(ThisDate1, ThisDate2) > 0)
            return ThisDate2;
        else
            return ThisDate1;
    }

    public static string GetDptBom(string CurConnectionString, string DPTNO)
    {
        /*
         跨類別變數-->宣告在本頁頂端
           CurDptList 
           dtOrganAll 
        */
        //static string CurDptList = "";   
        CurDptList = "'" + DPTNO + "'";
        string cmd = "SELECT Organ.DPTNO, Dpt.DPTNM, Dpt.ISCORP, Organ.SUBDPTNO, SubDpt.DPTNM SUBDPTNM, SubDpt.ISCORP SUBISCORP "
             + "FROM Organ LEFT JOIN Dpt ON (Organ.DPTNO=Dpt.DPTNO) "
             + "LEFT JOIN Dpt SubDpt ON (Organ.SUBDPTNO=SubDpt.DPTNO) "
             + "ORDER BY Organ.DPTNO,Organ.SUBDPTNO ";
        //DataTable dtOrganAll = new DataTable();
        dtOrganAll = RefetchDataTable(CurConnectionString, cmd);
        DataView dvOrganAll = new DataView(dtOrganAll);
        DptBomExpand(dvOrganAll, DPTNO);
        if (CurDptList.IndexOf(",") < 0)
            CurDptList = DPTNO;
        return CurDptList;
    }

    public static void DptBomExpand(DataView dvOrganForDptno, string DPTNO)
    {

        //DataView dvOrganForDptno = dvOrganAll;
        dvOrganForDptno.RowFilter = "DPTNO='" + DPTNO + "'"; //篩選對應節點的所有子節點
        foreach (DataRowView drv in dvOrganForDptno) //遍歷填充節點的所有子節點，如果傳入的節點node為葉子節點，遍歷要退出，不再進行遞歸
        {
            if (CurDptList == "")
                CurDptList = "'" + drv["SUBDPTNO"].ToString() + "'";
            else
                CurDptList = CurDptList + ',' + "'" + drv["SUBDPTNO"].ToString() + "'";
            DataView dvOrganAll = new DataView(dtOrganAll);
            dvOrganAll.Sort = "DPTNO";
            if (dvOrganAll.Find(drv["SUBDPTNO"].ToString()) >= 0)
                DptBomExpand(dvOrganAll, drv["SUBDPTNO"].ToString());

        }
    }

    public static string GetMaBom(string CurConnectionString, string MANO)
    {
        /*
         跨類別變數-->宣告在本頁頂端
           CurMaList 
           dtMaBomAll 
        */
        //static string CurMaList = "";   
        CurMaList = "'" + MANO + "'";
        string cmd = "SELECT MaBom.MANO, MaBom.SUBMANO, SubMa.MADESC SUBMADESC "
             + "FROM MaBom LEFT JOIN Ma ON (MaBom.MANO=Ma.MANO) "
             + "LEFT JOIN Ma SubMa ON (MaBom.SUBMANO=SubMa.MANO) "
             + "ORDER BY MaBom.MANO,MaBom.SUBMANO ";
        //DataTable dtMaBomAll = new DataTable();
        dtMaBomAll = RefetchDataTable(CurConnectionString, cmd);
        DataView dvMaBomAll = new DataView(dtMaBomAll);
        MaBomExpand(dvMaBomAll, MANO);
        if (CurMaList.IndexOf(",") < 0)
            CurMaList = MANO;
        return CurMaList;
    }

    public static void MaBomExpand(DataView dvMaBomForMANO, string MANO)
    {

        //DataView dvMaBomForMANO = dvMaBomAll;
        dvMaBomForMANO.RowFilter = "MANO='" + MANO + "'"; //篩選對應節點的所有子節點
        foreach (DataRowView drv in dvMaBomForMANO) //遍歷填充節點的所有子節點，如果傳入的節點node為葉子節點，遍歷要退出，不再進行遞歸
        {
            if (CurMaList == "")
                CurMaList = "'" + drv["SUBMANO"].ToString() + "'";
            else
                CurMaList = CurMaList + ',' + "'" + drv["SUBMANO"].ToString() + "'";
            DataView dvMaBomAll = new DataView(dtMaBomAll);
            dvMaBomAll.Sort = "MANO";
            if (dvMaBomAll.Find(drv["SUBMANO"].ToString()) >= 0)
                MaBomExpand(dvMaBomAll, drv["SUBMANO"].ToString());

        }
    }

    public static string GetMaRoutBom(string CurConnectionString, string MANO, string ROUTID)
    {
        //static string CurMaList = "";   
        CurMaList = "'" + MANO + "'";
        string cmd = "SELECT MaRoutBom.MANO, MaRoutBom.MROUTID, MaRoutBom.SUBMANO, SubMa.MADESC SUBMADESC "
             + "FROM MaRoutBom LEFT JOIN Ma ON (MaRoutBom.MANO=Ma.MANO) "
             + "LEFT JOIN Ma SubMa ON (MaRoutBom.SUBMANO=SubMa.MANO) "
             //+ "WHERE MaRoutBom.MANO='" + MANO + "' AND MaRoutBom.MROUTID='" + ROUTID + "' "
             + "WHERE MaRoutBom.MROUTID='" + ROUTID + "' "
             + "ORDER BY MaRoutBom.MANO,MaRoutBom.MROUTID,MaRoutBom.SUBMANO ";
        //DataTable dtMaRoutBomAll = new DataTable();
        dtMaRoutBomAll = RefetchDataTable(CurConnectionString, cmd);
        DataView dvMaRoutBomAll = new DataView(dtMaRoutBomAll);
        MaRoutBomExpand(dvMaRoutBomAll, MANO, ROUTID);
        if (CurMaList.IndexOf(",") < 0)
            CurMaList = MANO;
        return CurMaList;
    }

    public static void MaRoutBomExpand(DataView dvMaRoutBomForMANO, string MANO, string ROUTID)
    {

        //DataView dvMaRoutBomForMANO = dvMaRoutBomAll;
        dvMaRoutBomForMANO.RowFilter = "MANO='" + MANO + "' AND MROUTID='" + ROUTID + "'"; //篩選對應節點的所有子節點
        foreach (DataRowView drv in dvMaRoutBomForMANO) //遍歷填充節點的所有子節點，如果傳入的節點node為葉子節點，遍歷要退出，不再進行遞歸
        {
            if (CurMaList == "")
                CurMaList = "'" + drv["SUBMANO"].ToString() + "'";
            else
                CurMaList = CurMaList + ',' + "'" + drv["SUBMANO"].ToString() + "'";
            DataView dvMaRoutBomAll = new DataView(dtMaRoutBomAll);
            dvMaRoutBomAll.Sort = "MANO";
            if (dvMaRoutBomAll.Find(drv["SUBMANO"].ToString()) >= 0)
                MaRoutBomExpand(dvMaRoutBomAll, drv["SUBMANO"].ToString(), ROUTID);
        }
    }


    /*
    public static string GetMaRoutBom(string CurConnectionString, string MANO, string ROUTID)
    {
        //static string CurMaList = "";   
        CurMaList = "'" + MANO + ROUTID + "'";
        string cmd = "SELECT MaRoutBom.MANO, MaRoutBom.SUBMANO, SubMa.MADESC SUBMADESC "
             + "FROM MaRoutBom LEFT JOIN Ma ON (MaRoutBom.MANO=Ma.MANO) "
             + "LEFT JOIN Ma SubMa ON (MaRoutBom.SUBMANO=SubMa.MANO) "
             + "ORDER BY MaRoutBom.MANO,MaRoutBom.MROUTID,MaRoutBom.SUBMANO ";
        //DataTable dtMaRoutBomAll = new DataTable();
        dtMaRoutBomAll = DBUtility.RefetchDataTable(CurConnectionString, cmd);
        DataView dvMaRoutBomAll = new DataView(dtMaRoutBomAll);
        MaRoutBomExpand(dvMaRoutBomAll, MANO, ROUTID);
        if (CurMaList.IndexOf(",") < 0)
            CurMaList = MANO + ROUTID;
        return CurMaList;
    }

    public static void MaRoutBomExpand(DataView dvMaRoutBomForMANO, string MANO, string ROUTID)
    {

        //DataView dvMaRoutBomForMANO = dvMaRoutBomAll;
        dvMaRoutBomForMANO.RowFilter = "MANO='" + MANO + "'AND MROUTID='" + ROUTID + "'"; //篩選對應節點的所有子節點
        foreach (DataRowView drv in dvMaRoutBomForMANO) //遍歷填充節點的所有子節點，如果傳入的節點node為葉子節點，遍歷要退出，不再進行遞歸
        {
            if (CurMaList == "")
                CurMaList = "'" + drv["SUBMANO"].ToString() + "'";
            else
                CurMaList = CurMaList + ',' + "'" + drv["SUBMANO"].ToString() + "'";
            DataView dvMaRoutBomAll = new DataView(dtMaRoutBomAll);
            dvMaRoutBomAll.Sort = "MANO";
            if (dvMaRoutBomAll.Find(drv["SUBMANO"].ToString()) >= 0)
                MaRoutBomExpand(dvMaRoutBomAll, drv["SUBMANO"].ToString());

        }
    }
     */

    public static string ObtainResCaption(string ResourcesFile, string FieldName)
    {
        try
        {
            return HttpContext.GetGlobalResourceObject(ResourcesFile, FieldName).ToString();
            /*
            ResourceManager RM = new ResourceManager("OrganMain", GetType().Assembly);
            //ResourceManager RM = new ResourceManager(ResourcesFile + ".zh-CN.resx", GetType().Assembly);
            return RM.GetString(FieldName);
            */
            //return "<%$ Resources:" + ResourcesFile + ", " + FieldName + " %>";
        }
        catch
        {
            return "?";
        }

    }

    public static DataTable GetSessionTable(string CurTableName)
    {
        //You can store a DataTable in the session state
        DataTable table = HttpContext.Current.Session[CurTableName] as DataTable;
        if (table == null)
        {
            /*
            table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("data", typeof(String));
            for (int n = 0; n < 100; n++)
            {
                table.Rows.Add(n, "row" + n.ToString());
            }
            Session["Table"] = table;
            */
        }
        return table;
    }

    public static string DateSpan(DateTime DateTime1, DateTime DateTime2)
    {
        string dateDiff = null;
        TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
        TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
        TimeSpan ts = ts1.Subtract(ts2).Duration();
        dateDiff = ts.Days.ToString() + "." + ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
        //dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小時" + ts.Minutes.ToString() + "分鐘" + ts.Seconds.ToString() + "秒";
        return dateDiff;
    }

    //整數轉成十六進制字串
    public static string IntToHexString(int value)
    {
        return "0x" + String.Format("{0:X}", value);
    }

    //十六進制字串轉成整數
    public static int HexStringToInt(string value)
    {
        if (value.ToUpper().StartsWith("0X"))
            value = value.Substring(2);
        return Int32.Parse(value, System.Globalization.NumberStyles.HexNumber);
    }

    //把ASPxFormLayout的輸入項對應到指定的SqlDataSource, 再進行SqlDataSource.Insert()或SqlDataSource.Update()
    public static void FormLayoutMapToData(ASPxFormLayout CurFormLayout, SqlDataSource CurDataSource, bool ISInsert)
    {
        foreach (var Curitem in CurFormLayout.Items)
        {
            if (Curitem is TabbedLayoutGroup)
            {
                //TabbedLayoutGroup
                if ((Curitem as TabbedLayoutGroup).Items.Count > 0)
                {
                    foreach (LayoutGroupBase ltgitem in (Curitem as TabbedLayoutGroup).Items)
                    {
                        if (ltgitem is LayoutGroupBase)
                        {
                            if ((ltgitem as LayoutGroup).Items.Count > 0)
                            {
                                foreach (LayoutItemBase item in (ltgitem as LayoutGroup).Items)
                                {
                                    LayoutItemMapToData(item, CurDataSource, ISInsert);
                                }
                            }

                        }
                        else
                        {
                            if (Curitem is LayoutItem)
                                LayoutItemMapToData((LayoutItem)Curitem, CurDataSource, ISInsert);
                        }
                    }
                }
            }
            else if (Curitem is LayoutGroupBase)
            {
                //TabbedLayoutGroup
                if ((Curitem as LayoutGroup).Items.Count > 0)
                {
                    foreach (LayoutItemBase item in (Curitem as LayoutGroup).Items)
                    {
                        LayoutItemMapToData(item, CurDataSource, ISInsert);
                    }
                }
            }
            else
            {
                if (Curitem is LayoutItem)
                    LayoutItemMapToData((LayoutItem)Curitem, CurDataSource, ISInsert);
            }
        }
    }

    public static void LayoutItemMapToData(LayoutItemBase item, SqlDataSource CurDataSource, bool ISInsert)
    {
        if (item is DevExpress.Web.EmptyLayoutItem)
            return;
        LayoutItem layoutItem = item as LayoutItem;
        if (layoutItem != null)
        {
            ASPxEditBase editBase = layoutItem.GetNestedControl() as ASPxEditBase;
            if ((editBase != null) && (editBase.Value != null))
            {
                if (ISInsert)
                {
                    //新增
                    if (CurDataSource.InsertParameters[layoutItem.FieldName] != null)
                    {
                        CurDataSource.InsertParameters[layoutItem.FieldName].DefaultValue = editBase.Value.ToString();
                    }
                }
                else
                {
                    //更改
                    if (CurDataSource.UpdateParameters[layoutItem.FieldName] != null)
                    {
                        CurDataSource.UpdateParameters[layoutItem.FieldName].DefaultValue = editBase.Value.ToString();
                    }
                }
            }
        }
    }

    //清空ASPxFormLayout所有的輸入項
    public static void FormLayoutClearData(ASPxFormLayout CurFormLayout)
    {
        foreach (var Curitem in CurFormLayout.Items)
        {
            if (Curitem is LayoutGroupBase)
            {
                if ((Curitem as LayoutGroup).Items.Count > 0)
                {
                    foreach (LayoutItemBase item in (Curitem as LayoutGroup).Items)
                    {
                        LayoutItemClearData((LayoutItem)Curitem);
                    }
                }
            }
            else
            {
                if (Curitem is LayoutItem)
                    LayoutItemClearData((LayoutItem)Curitem);
            }
        }
    }

    private static void LayoutItemClearData(LayoutItemBase item)
    {
        var layoutItem = item as LayoutItem;
        if (layoutItem != null)
        {
            var editBase = layoutItem.GetNestedControl() as ASPxEditBase;
            if (editBase != null)
            {
                editBase.Value = string.Empty;
            }
        }
    }

    //設定Grid中文訊息、主題、字型、權限...
    public static void InitGridSetting(ASPxGridView CurDBGrid, int GridPtr = 1, string FormID = "")
    {
        if (GridPtr == 1)
        {
            //GridPtr == 1 --> 主檔 MainDBGrid
            //訊息設定為中文顯示 
            CurDBGrid.SettingsText.CommandNew = "新增";
            CurDBGrid.SettingsText.CommandEdit = "編輯";
            CurDBGrid.SettingsText.CommandDelete = "刪除";
            CurDBGrid.SettingsText.CommandUpdate = "存入";
            CurDBGrid.SettingsText.CommandCancel = "取消";
            CurDBGrid.SettingsText.ConfirmDelete = "確定刪除此筆資料 ?";
            CurDBGrid.SettingsText.EmptyDataRow = "無符合資料顯示";
            CurDBGrid.SettingsLoadingPanel.Text = "資料載入中...";
            //Theme 設定
            CurDBGrid.Theme = "Aqua";
            //預先指定 CurDBGrid.EditForm的背景顏色
            CurDBGrid.Styles.EditForm.BackColor = System.Drawing.Color.LightBlue;
            //CurDBGrid.Styles.EditForm.BackColor = System.Drawing.Color.Khaki;
            //字型大小
            CurDBGrid.Font.Size = FontUnit.Small;
            //隱藏格子的水平線
            CurDBGrid.Settings.GridLines = GridLines.None;
            //HorizontalScrollBarMode="Auto" --> 導致AdaptivityMode = off 
            //Column Width可調整--> 導致AdaptivityMode = off
            //CurDBGrid.SettingsResizing.ColumnResizeMode = ColumnResizeMode.Control;
            //CurDBGrid - 增刪修按鈕設定
            // 20*20
            CurDBGrid.SettingsCommandButton.NewButton.Image.Url = "~/Images/Navigator/Grid_Add.png";
            CurDBGrid.SettingsCommandButton.EditButton.Image.Url = "~/Images/Navigator/Grid_Edit.png";
            CurDBGrid.SettingsCommandButton.DeleteButton.Image.Url = "~/Images/Navigator/Grid_Delete.png";
            CurDBGrid.SettingsCommandButton.UpdateButton.Image.Url = "~/Images/Navigator/Grid_Post.png";
            CurDBGrid.SettingsCommandButton.CancelButton.Image.Url = "~/Images/Navigator/Grid_Cancel.png";
            // 24*24
            //CurDBGrid.SettingsCommandButton.EditButton.Image.Url = "~/Images/Navigator/Grid_Edit-24.png";
            //CurDBGrid.SettingsCommandButton.DeleteButton.Image.Url = "~/Images/Navigator/Grid_Trash-26.png";
            //CurDBGrid.SettingsCommandButton.UpdateButton.Image.Url = "~/Images/Navigator/Grid_Post.png";
            //CurDBGrid.SettingsCommandButton.CancelButton.Image.Url = "~/Images/Navigator/Grid_Cancel.png";

            //CurDBGrid - 自訂按鈕(編輯+刪除)設定
            //Page_Load事件無法設定 HeaderTemplate的 NavgInsert..Image.Url, 須轉到 Page_PreRenderComplete 事件設定
            //ASPxButton btnInsert = (CurDBGrid.FindHeaderTemplateControl(CurDBGrid.Columns[0], "NavgInsert") as ASPxButton);
            //btnInsert.Image.Url = "~/Images/Navigator/Grid_Delete.png";
            if ((CurDBGrid.Columns.Count > 0) && ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn) != null))
            {
                if ((!(CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowEditButton) && (CurDBGrid.SettingsDataSecurity.AllowEdit))
                {
                    //自訂編輯按鈕
                    if ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] == null)
                    {
                        (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons.Add(new GridViewCommandColumnCustomButton()
                        {
                            ID = "NavgEdit",
                            Text = " ",
                            Visibility = GridViewCustomButtonVisibility.BrowsableRow
                        });
                        ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] as GridViewCommandColumnCustomButton).Image.ToolTip = "編輯";
                        ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] as GridViewCommandColumnCustomButton).Image.Url = "~/Images/Navigator/Grid_Edit.png";
                    }
                }
                if ((!(CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowDeleteButton) && (CurDBGrid.SettingsDataSecurity.AllowDelete))
                {
                    //自訂刪除按鈕
                    if ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] == null)
                    {
                        (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons.Add(new GridViewCommandColumnCustomButton()
                        {
                            ID = "NavgDelete",
                            Text = " ",
                            Visibility = GridViewCustomButtonVisibility.BrowsableRow
                        });
                        ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] as GridViewCommandColumnCustomButton).Image.ToolTip = "刪除";
                        ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] as GridViewCommandColumnCustomButton).Image.Url = "~/Images/Navigator/Grid_Delete.png";
                    }
                }
            }
            CurDBGrid.ClientSideEvents.CustomButtonClick = "function(s, e) {"
                                                        + "if (e.buttonID == 'NavgInsert'){"
                                                        + "    OnNavgInsertClick(s, e);"
                                                        + "}"
                                                        + "if (e.buttonID == 'NavgEdit'){"
                                                        + "    OnNavgEditClick(s, e);"
                                                        + "}"
                                                        + "if (e.buttonID == 'NavgDelete'){"
                                                        + "    OnNavgDeleteClick(s, e);"
                                                        + "}"
                                                        + "}";
            //根據權限動態設定該功能顯示否
            //NavgInsert.Visible需轉到 Page_PreRenderComplete 事件設定
            //NavgEdit.Visible, NavgDelete 需在Page_Load事件處理
            //string FormID = this.Form.ID;
            if (FormID != "")
            {
                string ParentHeadStr;
                ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
                //Page_Load事件無法設定 HeaderTemplate的 NavgInsert.Visible, 須轉到 NavgInsert_Load 事件設定
                //ASPxButton NavgInsert = (CurDBGrid.FindHeaderTemplateControl(CurDBGrid.Columns[0], "NavgInsert") as ASPxButton);
                //NavgInsert.Load += new EventHandler(this.NavgInsert_Load);
                //NavgInsert.Load += new System.Web.UI.ImageClickEventHandler(this.NavgInsert_Load);
                //根據權限動態設定新增功能顯示否
                //Page_Load事件無法設定 HeaderTemplate的 NavgInsert.Visible, 須轉到 NavgInsert的NavgInsert_Load 事件設定
                if ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowNewButtonInHeader)
                {
                    //內訂
                    if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "A").PadRight(5, '0'), false))
                        (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowNewButtonInHeader = true;
                    else
                        (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowNewButtonInHeader = false;
                }
                //根據權限動態設定編輯功能顯示否
                if ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowEditButton)
                {
                    //內訂
                    if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "B").PadRight(5, '0'), false))
                        (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowEditButton = true;
                    else
                        (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowEditButton = false;
                }
                else if ((CurDBGrid.Columns.Count > 0) && ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn) != null) && ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] != null))
                {
                    //自訂
                    if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "B").PadRight(5, '0'), false))
                        ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.BrowsableRow;
                    else
                        ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.Invisible;
                }
                //根據權限動態設定刪除功能顯示否
                if ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowDeleteButton)
                {
                    //內訂
                    if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "C").PadRight(5, '0'), false))
                        (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowDeleteButton = true;
                    else
                        (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowDeleteButton = false;
                }
                else if (((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn) != null) && ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] != null))
                {
                    //自訂
                    if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "C").PadRight(5, '0'), false))
                        ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.BrowsableRow;
                    else
                        ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.Invisible;
                }
                //根據權限動態設定查詢列印功能顯示否
                //ASPxMenu MainDBGridMenu = (ASPxMenu)Page.FindControl("MainDBGridMenu");
                //DBUtility.CmdbtnSetRight(ParentHeadStr, null, 1, (MainDBGridMenu.Items.FindByName("NavgFilter") as DevExpress.Web.MenuItem), (MainDBGridMenu.Items.FindByName("NavgPrint") as DevExpress.Web.MenuItem));
            }
        }
        else
        {
            //GridPtr == 2 --> 副檔 SubDBGrid
            CurDBGrid.SettingsText.CommandNew = "新增";
            CurDBGrid.SettingsText.CommandEdit = "編輯";
            CurDBGrid.SettingsText.CommandDelete = "刪除";
            CurDBGrid.SettingsText.CommandUpdate = "存入";
            CurDBGrid.SettingsText.CommandCancel = "取消";
            CurDBGrid.SettingsText.ConfirmDelete = "確定刪除此筆資料 ?";
            CurDBGrid.SettingsText.EmptyDataRow = "無符合資料顯示";
            CurDBGrid.SettingsLoadingPanel.Text = "資料載入中...";
            CurDBGrid.Theme = "Office2010Black";
            //預先指定 CurDBGrid.EditForm的背景顏色
            CurDBGrid.Styles.EditForm.BackColor = System.Drawing.Color.Khaki;
            //CurDBGrid.Styles.EditForm.BackColor = System.Drawing.Color.LightBlue;
            //隱藏格子的水平線
            CurDBGrid.Settings.GridLines = GridLines.None;
            //字型大小
            CurDBGrid.Font.Size = FontUnit.Small;
            //Column Width可調整
            //CurDBGrid.SettingsResizing.ColumnResizeMode = ColumnResizeMode.Control;
            //CurDBGrid - 增刪修按鈕設定
            // 20*20
            CurDBGrid.SettingsCommandButton.NewButton.Image.Url = "~/Images/Navigator/Grid_Add.png";
            CurDBGrid.SettingsCommandButton.EditButton.Image.Url = "~/Images/Navigator/Grid_Edit.png";
            CurDBGrid.SettingsCommandButton.DeleteButton.Image.Url = "~/Images/Navigator/Grid_Delete.png";
            CurDBGrid.SettingsCommandButton.UpdateButton.Image.Url = "~/Images/Navigator/Grid_Post.png";
            CurDBGrid.SettingsCommandButton.CancelButton.Image.Url = "~/Images/Navigator/Grid_Cancel.png";
            // 24*24
            //CurDBGrid.SettingsCommandButton.EditButton.Image.Url = "~/Images/Navigator/Grid_Edit-24.png";
            //CurDBGrid.SettingsCommandButton.DeleteButton.Image.Url = "~/Images/Navigator/Grid_Delete.png";
            //CurDBGrid.SettingsCommandButton.UpdateButton.Image.Url = "~/Images/Navigator/Grid_Post.png";
            //CurDBGrid.SettingsCommandButton.CancelButton.Image.Url = "~/Images/Navigator/Grid_Cancel.png";

            if ((CurDBGrid.Columns.Count > 0) && ((CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn) != null))
            {
                //根據權限代碼動態設定增刪修按鍵的顯示否
                //string FormID = this.Form.ID;
                if (FormID != "")
                {
                    string ParentHeadStr;
                    if (FormID.IndexOf("0") >= 0)
                    {
                        ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
                        DBUtility.CmdbtnSetRight(ParentHeadStr, (CurDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn), 2);
                    }
                }
            }
        }
    }

    //ASPxTreeList使用
    public static void FormLayoutInsertToValues(ASPxFormLayout CurFormLayout, DevExpress.Web.Data.ASPxDataInsertingEventArgs e, bool ISInsert)
    {
        foreach (var Curitem in CurFormLayout.Items)
        {
            if (Curitem is LayoutGroupBase)
            {
                if ((Curitem as LayoutGroup).Items.Count > 0)
                {
                    foreach (LayoutItemBase item in (Curitem as LayoutGroup).Items)
                    {
                        LayoutItemInsertToValue(item, e, ISInsert);
                    }
                }
            }
            else
            {
                if (Curitem is LayoutItem)
                    LayoutItemInsertToValue((LayoutItem)Curitem, e, ISInsert);
            }
        }
    }

    public static void LayoutItemInsertToValue(LayoutItemBase item, DevExpress.Web.Data.ASPxDataInsertingEventArgs e, bool ISInsert)
    {
        if (item is DevExpress.Web.EmptyLayoutItem)
            return;
        LayoutItem layoutItem = item as LayoutItem;
        if (layoutItem != null)
        {
            ASPxEditBase editBase = layoutItem.GetNestedControl() as ASPxEditBase;
            if ((editBase != null) && (editBase.Value != null))
            {
                if (ISInsert)
                {
                    //新增
                    if (layoutItem.FieldName != null)
                        e.NewValues[layoutItem.FieldName] = editBase.Value.ToString();
                }
                else
                {
                    //更改
                    if (layoutItem.FieldName != null)
                        e.NewValues[layoutItem.FieldName] = editBase.Value.ToString();
                }
            }
        }
    }

    //ASPxTreeList使用
    public static void FormLayoutUpdateToValues(ASPxFormLayout CurFormLayout, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e, bool ISInsert)
    {
        foreach (var Curitem in CurFormLayout.Items)
        {
            if (Curitem is LayoutGroupBase)
            {
                if ((Curitem as LayoutGroup).Items.Count > 0)
                {
                    foreach (LayoutItemBase item in (Curitem as LayoutGroup).Items)
                    {
                        LayoutItemUpdateToValue(item, e, ISInsert);
                    }
                }
            }
            else
            {
                if (Curitem is LayoutItem)
                    LayoutItemUpdateToValue((LayoutItem)Curitem, e, ISInsert);
            }
        }
    }

    public static void LayoutItemUpdateToValue(LayoutItemBase item, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e, bool ISInsert)
    {
        if (item is DevExpress.Web.EmptyLayoutItem)
            return;
        LayoutItem layoutItem = item as LayoutItem;
        if (layoutItem != null)
        {
            ASPxEditBase editBase = layoutItem.GetNestedControl() as ASPxEditBase;
            if ((editBase != null) && (editBase.Value != null))
            {
                if (ISInsert)
                {
                    //新增
                    if (layoutItem.FieldName != null)
                        e.NewValues[layoutItem.FieldName] = editBase.Value.ToString();
                }
                else
                {
                    //更改
                    if (layoutItem.FieldName != null)
                        e.NewValues[layoutItem.FieldName] = editBase.Value.ToString();
                }
            }
        }
    }


    public static bool URLExists(string url)
    {
        bool result = true;
        WebRequest webRequest = WebRequest.Create(url);
        webRequest.Timeout = 1200; // miliseconds
        webRequest.Method = "HEAD";
        try
        {
            webRequest.GetResponse();
        }
        catch
        {
            result = false;
        }
        return result;
    }

    /// <summary>產生亂數字串</summary>
    /// <param name="Number">字元數</param>
    /// <returns></returns>
    public static string CreateRandomCode(int Number)
    {
        string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
        string[] allCharArray = allChar.Split(',');
        string randomCode = "";
        int temp = -1;

        Random rand = new Random();
        for (int i = 0; i < Number; i++)
        {
            if (temp != -1)
            {
                rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
            }
            int t = rand.Next(36);
            if (temp != -1 && temp == t)
            {
                return CreateRandomCode(Number);
            }
            temp = t;
            randomCode += allCharArray[t];
        }
        return randomCode;
    }

    public static string GetCCARDSSLID(string CurConnectionString)
    {
        string cmd = "SELECT Sysvar.CCARDSSLID, Sysvar.LCCARDSSLIDDT "
             + "FROM Sysvar ";
        DataTable dtbSysvar = DBUtility.RefetchDataTable(CurConnectionString, cmd);
        if (((DateTime)dtbSysvar.Rows[0]["LCCARDSSLIDDT"]).Date >= System.DateTime.Today)
            return dtbSysvar.Rows[0]["CCARDSSLID"].ToString();
        else
            return null;
    }

    public static bool SetCCARDSSLID(string CurConnectionString)
    {
        //更新
        string UpdateCmd = "UPDATE Sysvar "
             + "SET "
             + "CCARDSSLID= '" + CreateRandomCode(6) + "', "
             + "LCCARDSSLIDDT= '" + System.DateTime.Today.ToString("yyyy/MM/dd") + "' ";
        return (SqlHelper.ExecuteNonQuery(CurConnectionString, CommandType.Text, UpdateCmd) == 1);
        //DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings[CurConnectionString].ConnectionString, UpdateCmd);
    }

    /// <summary>
    /// 最简单的方式由SQL计算
    /// </summary>
    /// <param name="expression">表达式</param>
    /// <returns></returns>
    public static float CalcBySQL(string expression, string CurConnectionString)
    {
        string SQL = "SELECT " + expression + " AS RESULT_VALUE";
        SqlConnection conn = new SqlConnection(CurConnectionString);
        SqlCommand cmd = new SqlCommand(SQL, conn);
        object o = cmd.ExecuteScalar(); //执行SQL.
        return float.Parse(o.ToString());
    }

    /// <summary>
    /// 由DataTable计算公式
    /// </summary>
    /// <param name="expression">表达式</param>
    public static float CalcByDataTable(string expression)
    {
        object result = new DataTable().Compute(expression, "");
        return float.Parse(result + "");
    }

    public static string GetUpperDptLeader(string CurConnectionString, string CurPRSNNO)
    {
        string cmdDptLeader = "SELECT Prsn.PRSNNO, Prsn.PRSNNM, Prsn.DPTNO, Dpt.DPTHANDLER "
                    + "FROM Prsn LEFT JOIN Dpt ON (Prsn.DPTNO=Dpt.DPTNO) "
                    + "WHERE Prsn.PRSNNO= '" + CurPRSNNO + "' ";
        DataTable dtbDptLeader = DBUtility.RefetchDataTable(CurConnectionString, cmdDptLeader);
        if (dtbDptLeader.Rows.Count == 0)
            //尋找父部門的主管
            return FindUpperDptLeader(CurConnectionString, dtbDptLeader.Rows[0]["DPTNO"].ToString(), CurPRSNNO);
        else
            //目前部門的主管
            return dtbDptLeader.Rows[0]["DPTHANDLER"].ToString();
    }

    public static string FindUpperDptLeader(string CurConnectionString, string CurDPTNO, string CurPRSNNO)
    {
        string cmdDptLeader = "SELECT Organ.DPTNO, Dpt.DPTNM, Dpt.ISCORP, Dpt.DPTLEADER, Organ.SUBDPTNO, SubDpt.DPTNM SUBDPTNM, SubDpt.ISCORP SUBISCORP "
                    + "FROM Organ LEFT JOIN Dpt ON(Organ.DPTNO= Dpt.DPTNO) "
                    + " LEFT JOIN Dpt SubDpt ON(Organ.SUBDPTNO = SubDpt.DPTNO) "
                    + "WHERE Organ.SUBDPTNO= '" + CurDPTNO + "' "
                    + "ORDER BY Organ.DPTNO,Organ.SUBDPTNO ";
        DataTable dtbDptLeader = DBUtility.RefetchDataTable(CurConnectionString, cmdDptLeader);
        if (dtbDptLeader.Rows.Count == 0)
            //尋找父部門的主管
            return FindUpperDptLeader(CurConnectionString, dtbDptLeader.Rows[0]["DPTNO"].ToString(), CurPRSNNO);
        else
            //目前部門的主管
            return dtbDptLeader.Rows[0]["DPTHANDLER"].ToString();
    }

    public static bool IsAllowIP(string CurConnectionString, string CurIP)
    {
        string cmdAllowIP = "SELECT IPADDR "
                    + "FROM AllowIP  "
                    + "WHERE IPADDR= '" + CurIP + "' ";
        return IsDataExist(CurConnectionString, cmdAllowIP);
    }


    /// 取得正確的Client端IP
    /// </summary>
    /// <returns></returns>
    public static string GetClientIP()
    {
        //判所client端是否有設定代理伺服器
        if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] == null)
            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
        else
            return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
    }

    private static string RetrieveIP(HttpRequest request)
    {
        string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (ip == null || ip.Trim() == string.Empty)
        {
            ip = request.ServerVariables["REMOTE_ADDR"];
        }
        return ip;
    }

    //設定按鈕Client端防止按鈕重複送出&重複點擊
    public static void ButtonPreventMultipleClicks(DevExpress.Web.ASPxButton button, String valgroupname, bool ShowWaitMesg = false)
    {
        //if (ShowWaitMesg) { s.SetEnabled(false); lplProcessing.Show(); { 1} } else { s.SetEnabled(false); lplProcessing.Show(); { 1} }
        //button.ClientSideEvents.Click = string.Format("function(s, e) {{ if(!ASPxClientEdit.ValidateGroup('{0}')) {{e.processOnServer=false;}} else {{if (ShowWaitMesg) {{ s.SetEnabled(false); lplProcessing.Show(); {1} }} else {{ s.SetEnabled(false); {1}} }} }}", valgroupname, button.Page.ClientScript.GetPostBackEventReference(button, String.Empty).ToString());
        //button.ClientSideEvents.Click = string.Format("function(s, e) {{ if(!ASPxClientEdit.ValidateGroup('{0}')) {{e.processOnServer=false;}} else {{ if (ShowWaitMesg) { s.SetEnabled(false); lplProcessing.Show(); {1} } else { s.SetEnabled(false); {1}  }}", valgroupname, button.Page.ClientScript.GetPostBackEventReference(button, String.Empty).ToString());
        if (ShowWaitMesg)
            button.ClientSideEvents.Click = string.Format("function(s, e) {{ if(!ASPxClientEdit.ValidateGroup('{0}')) {{e.processOnServer=false;}} else {{s.SetEnabled(false);lplProcessing.Show(); {1}}} }}", valgroupname, button.Page.ClientScript.GetPostBackEventReference(button, String.Empty).ToString());
        else
            button.ClientSideEvents.Click = string.Format("function(s, e) {{ if(!ASPxClientEdit.ValidateGroup('{0}')) {{e.processOnServer=false;}} else {{s.SetEnabled(false); {1}}} }}", valgroupname, button.Page.ClientScript.GetPostBackEventReference(button, String.Empty).ToString());
    }

    /// <summary>
    /// 最简单的方式由SQL计算
    /// </summary>
    /// <param name="expression">表达式</param>
    /// <returns></returns>
    public static void DateRangeSet(int DatesSelectIndex, DateTime CurDate, ASPxDateEdit STSender, ASPxDateEdit EDSender)
    {
        switch (DatesSelectIndex)
        {
            case 0:
                //今天
                STSender.Value = System.DateTime.Today;
                EDSender.Value = System.DateTime.Today;
                break;
            case 1:
                //昨天
                STSender.Value = System.DateTime.Today.AddDays(-1);
                EDSender.Value = System.DateTime.Today.AddDays(-1);
                break;
            case 2:
                //明天
                STSender.Value = System.DateTime.Today.AddDays(+1);
                EDSender.Value = System.DateTime.Today.AddDays(+1);
                break;
            case 3:
                //本週
                STSender.Value = FirstDayOfWeek(System.DateTime.Today);
                EDSender.Value = LastDayOfWeek(System.DateTime.Today);
                break;
            case 4:
                //上週
                STSender.Value = FirstDayOfWeek(System.DateTime.Today).AddDays(-7);
                EDSender.Value = LastDayOfWeek(System.DateTime.Today).AddDays(-7);
                break;
            case 5:
                //下週
                STSender.Value = FirstDayOfWeek(System.DateTime.Today).AddDays(+7);
                EDSender.Value = LastDayOfWeek(System.DateTime.Today).AddDays(+7);
                break;
            case 6:
                //本月
                STSender.Value = FirstDayOfMonth(System.DateTime.Today);
                EDSender.Value = LastDayOfMonth(System.DateTime.Today);
                break;
            case 7:
                //上月
                STSender.Value = FirstDayOfMonth(System.DateTime.Today.AddMonths(-1));
                EDSender.Value = LastDayOfMonth(System.DateTime.Today.AddMonths(-1));
                break;
            case 8:
                //下月
                STSender.Value = FirstDayOfMonth(System.DateTime.Today.AddMonths(+1));
                EDSender.Value = LastDayOfMonth(System.DateTime.Today.AddMonths(+1));
                break;
            case 9:
                //今年
                STSender.Value = FirstDayOfYear(System.DateTime.Today);
                EDSender.Value = LastDayOfYear(System.DateTime.Today);
                break;
            case 10:
                //去年
                STSender.Value = FirstDayOfYear(System.DateTime.Today).AddYears(-1);
                EDSender.Value = LastDayOfYear(System.DateTime.Today).AddYears(-1);
                break;
            case 11:
                //明年
                STSender.Value = FirstDayOfYear(System.DateTime.Today).AddYears(+1);
                EDSender.Value = LastDayOfYear(System.DateTime.Today).AddYears(+1);
                break;
        }  //case I of

    }


    //
    // TODO: 在這裡新增建構函式邏輯
    //
    //}
}