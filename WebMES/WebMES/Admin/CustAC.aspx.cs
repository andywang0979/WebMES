/*
 *
 * Client端 :
 *   NavgInsert Button
 *      MainTabPage.GetTabByName('TabPageView'));cbkfloCus.PerformCallback('NavgInsert')
 *      
 *   NavgEdit Button
 *      MainTabPage.GetTabByName('TabPageView'));cbkfloCus.PerformCallback('NavgEdit')
 *  
 * cbkfloCus包含 :floCus(ASPxFormLayout) + CusSalDBGrid(ASPxGridView)
   cbkfloCus_Callback
            if (e.Parameter == "NavgInsert")
            {
                //新增
                   :
            else if (e.Parameter == "NavgEdit")
            {
                //更改
                   :

SELECT Cus.CUSNO, Cus.CUSCM, Cus.CUSNM, Cus.LOGONID, Cus.CUSKDNO, Cuskd.CUSKDNM, Cus.BIRDT, Cus.IDNO, 
    Cus.SEXNO, Cus.REGNO, Cus.DPTNO, Cus.PRSNNO, Cus.BOSSNM, Cus.BOSSTIT, Cus.BOSSMOVTEL, 
    Cus.CRPADDR, Cus.CRPZIPCD, Cus.CRPTELNO, Cus.CRPFAXNO, Cus.CRPEMAILBOX, Cus.CRPIPADDR, 
    Cus.CNTMAN, Cus.CNTTIT, Cus.CNTADDR, Cus.CNTZIPCD, Cus.CNTMOVTEL, Cus.CNTTELNO, Cus.CNTFAXNO, 
    Cus.CNTEMAILBOX, Cus.DLYMAN, Cus.DLYTIT, Cus.DLYADDR, Cus.DLYZIPCD, Cus.DLYMOVTEL, Cus.DLYTELNO, 
    Cus.DLYEMAILBOX, Cus.INVCMAN, Cus.INVCTIT, Cus.INVCADDR, Cus.INVCZIPCD, Cus.INVCMOVTEL, 
    Cus.INVCTELNO, Cus.INVCEMAILBOX, Cus.AMTACNTNO, Cus.CHKACNTNO, Cus.BANKNO, Cus.SAVENO, 
    Cus.TAXRATE, Cus.TAXMDNO, Cus.PAYTERMNO, Cus.INVCMD, Cus.ENABLED, Cus.REMARK, Cus.SELFPHOTO, 
    Cus.DUEAMT, Cus.PAYAMT, Cus.MAXCREDIT, Cus.ORDCREDIT, Cus.USECREDIT, Cus.ISHOPAMT, Cus.CSHOPAMT, 
    Cus.HAIRTXTRID, Cus.HAIRAMNTID, Cus.HAIRWCLRID, Cus.HAIRCOLRID, Cus.HAIRSTYLID, Cus.HAIRSTYLDESC, 
    Cus.PASSNO, Cus.ISLOGIN, Cus.LLOGINDT, Cus.PASSMDNO, Cus.PASSVALDDAY, Cus.LPASSCHGDT, Cus.LOGID, 
    Cus.LOGDT
FROM  Cus LEFT OUTER JOIN Cuskd ON Cus.CUSKDNO = Cuskd.CUSKDNO

SELECT Cus.CUSNO, Cus.CUSCM, Cus.CUSNM, Cus.CUSKDNO, Cuskd.CUSKDNM, Cus.BIRDT, Cus.IDNO, 
    Cus.SEXNO, Cus.DPTNO, Cus.PRSNNO,
    Cus.CNTADDR, Cus.CNTZIPCD, Cus.CNTMOVTEL, Cus.CNTTELNO, Cus.CNTFAXNO, Cus.CNTEMAILBOX,
    Cus.DLYMAN, Cus.DLYTIT, Cus.DLYADDR, Cus.DLYZIPCD, Cus.DLYMOVTEL, Cus.DLYTELNO, 
    Cus.DLYEMAILBOX 
FROM  Cus LEFT OUTER JOIN Cuskd ON Cus.CUSKDNO = Cuskd.CUSKDNO

 * 
 * 
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
//
using DevExpress.Web;
//Export 使用
using DevExpress.Export;
using DevExpress.XtraPrinting;
using System.Reflection;

namespace WebMES.Admin
{
    public partial class CustAC : DevSFEditTPagGrid
    {
        protected override void Page_Init(object sender, EventArgs e)
        {

           AEDContentUrl = "CustACPage.aspx";
            strEditKey = "CUSNO";
            //strInsertContentUrl = "'CustACPage.aspx?EditMode=1'";
            //strEditContentUrl = "'CustACPage.aspx?EditMode=2&CUSNO=' + values";
            //strDeleteContentUrl = "'CustACPage.aspx?EditMode=3&CUSNO=' + values";
            if (!IsPostBack)
            {
                cbxBDPTNO.Value = Session["ActDeptID"] == null ? Session["CurDeptID"] : Session["ActDeptID"];
                cbxBPRSNNOSet(cbxBDPTNO.Value.ToString());
            }
        }

        /*
        protected override void Page_Load(object sender, EventArgs e)
        {
            //檢查是否已登入
            //if (!DBUtility.Logoned(false))
            //{
            //    Response.Redirect("~/PrsnLogin.aspx");
            //}

            if (!IsPostBack)
            {
                //hfdBACNTYR.Value = Session["ACNTYR"].ToString();
                //tbxBSTSHFTDT.Text = hfdBACNTYR.Value;
                btnGoFilter_Click(sender, e);
            }
            //根據權限動態設定該功能顯示否
            //NavgInsert.Visible需在DataBound事件處理
            //NavgEdit.Visible, NavgDelete 需在Page_Load事件處理
            string FormID = this.Form.ID;
            string ParentHeadStr;
            ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
            //DBUtility.CmdbtnSetRight(ParentHeadStr, (MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn), 1);
            //Page_Load事件無法設定 HeaderTemplate的 NavgInsert.Visible, 須轉到 NavgInsert_Load 事件設定
            //ASPxButton btnInsert = (MainDBGrid.FindHeaderTemplateControl(MainDBGrid.Columns[0], "NavgInsert") as ASPxButton);
            //btnInsert.Visible = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "A").PadRight(5, '0'), false);
            //根據權限動態設定編輯功能顯示否
            if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "B").PadRight(5, '0'), false))
                ((MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.BrowsableRow;
            else
                ((MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.Invisible;
            //根據權限動態設定刪除功能顯示否
            if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "C").PadRight(5, '0'), false))
                ((MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.BrowsableRow;
            else
                ((MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.Invisible;
        }
        */

        protected override void btnGoFilter_Click(object sender, EventArgs e)
        {
            pclSearchPanel.ShowOnPageLoad = false;
            string WhereSetStr = "";
            if (cbxBDPTNO.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Cus.DPTNO='" + cbxBDPTNO.Value.ToString() + "'";
                else
                    WhereSetStr += " AND Cus.DPTNO='" + cbxBDPTNO.Value.ToString() + "'";
            }
            if (cbxBPRSNNO.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Cus.PRSNNO='" + cbxBPRSNNO.Value.ToString() + "'";
                else
                    WhereSetStr += " AND Cus.PRSNNO='" + cbxBPRSNNO.Value.ToString() + "'";
                //MainDBGrid.Caption = "<div style='float: left;'>" + hfdBACNTYR.Value + "年" + "</div><div style='float: center;'>" + MainDBGrid.Caption + "</div>";
            }
            if (sdsCus.SelectCommand.IndexOf("WHERE ") >= 0)
                sdsCus.SelectCommand = sdsCus.SelectCommand.Substring(0, sdsCus.SelectCommand.IndexOf("WHERE "));
            if (WhereSetStr != "")
                sdsCus.SelectCommand += " WHERE " + WhereSetStr;
            sdsCus.SelectCommand += " ORDER BY Cus.CUSNO";

            sdsCus.SelectCommandType = SqlDataSourceCommandType.Text;
            //MainDBGrid.Caption = "關鍵指標設定";
            //MainDBGrid.Caption = "<div style='float: left;'>" + hfdBACNTYR.Value + "年" + "</div><div style='float: center;'>" + MainDBGrid.Caption + "</div>";
            /*
            try
            {  //==== 以下程式，只放「執行期間」的指令！=================
                // ---- 不用寫Conn.Open(); ，DataAdapter會自動開啟
                //myAdapter.Fill(ds, "test");    //---- 這時候執行SQL指令。取出資料，放進 DataSet。


                //***********************************
                //*** .Fill()方法之後，資料庫連線就中斷囉！
                //Response.Write("<hr />資料庫連線 Conn.State ---- " + Conn.State.ToString() + "<hr />");
                //***********************************

                //----(3). 自由發揮。由 GridView來呈現資料。----
                 
                string cmd = "SELECT TargNodes.ACNTYR, TargNodes.NODEID, TargNodes.NODENM, TargNodes.NODEDESC, TargNodes.WEGTPCNT "
                     + "FROM TargNodes "
                     + "WHERE TargNodes.ACNTYR=" + tbxBSTSHFTDT.Text ;
                //dtbTargNodes = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdPrsnHairs);
                //
                MainDBGrid.DataSourceID = null;
                MainDBGrid.DataSource = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WebBSCConnectionString"].ConnectionString, cmd);
                //lvwPrsn.DataSource = ds;
                //----標準寫法 GridView1.DataSource = ds.Tables["test"].DefaultView; ----
                MainDBGrid.DataBind();
            }
            catch (Exception ex)
            {
                //-- http://www.dotblogs.com.tw/billchung/archive/2009/03/31/7779.aspx
                Response.Write("<hr /> Exception Error Message----  " + ex.ToString());
            }
            finally
            {
                //----(4). 釋放資源、關閉連結資料庫----
                //---- 不用寫，DataAdapter會自動關閉
                //if (Conn.State == ConnectionState.Open)  {
                //  Conn.Close();
                //  Conn.Dispose();
                // }
            }
            */
        }

        protected void SubDBGrid_BeforePerformDataSelect(object sender, EventArgs e)
        {
            string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            Session["CUSNO"] = MasterKeyValue.Split('|')[0].Trim();
            //根據權限代碼動態設定增刪修按鍵的顯示否
            int index = MainDBGrid.FindVisibleIndexByKeyValue(MasterKeyValue);
            string FormID = this.Form.ID;
            string ParentHeadStr;
            ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
            DBUtility.CmdbtnSetRight(ParentHeadStr, ((MainDBGrid.FindDetailRowTemplateControl(index, "SubDBGrid") as ASPxGridView).Columns[0] as DevExpress.Web.GridViewCommandColumn), 2);
        }

        protected void cbxBPRSNNO_Callback(object sender, CallbackEventArgsBase e)
        {
            cbxBPRSNNOSet(e.Parameter);
        }

        protected void cbxBPRSNNOSet(string CurDPTNO)
        {
            if (string.IsNullOrEmpty(CurDPTNO))
                return;
            Session["DPTNO"] = CurDPTNO;
            cbxBPRSNNO.DataBind();
        }

        protected void sdsCSal_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //e.Command.Parameters["@ACNTYR"].Value = hfdBACNTYR.Value;

        }

        protected void sdsCSald_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            e.Command.Parameters["@SALNO"].Value = Session["SALNO"].ToString();

        }

        protected void MainDBGrid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            //e.NewValues["NODEID"] = "66";
        }

        protected override void LoadViewState(Object savedState)
        {
            if (savedState != null)
            {
                object[] mState = (object[])savedState;
                if (mState[0] != null)
                    base.LoadViewState(mState[0]);
                if (mState[1] != null)
                    sdsCus.SelectCommand = (string)mState[1];
            }
        }

        //SqlDataSource.SaveViewState 方法
        protected override Object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] mState = new object[2];
            mState[0] = baseState;
            mState[1] = sdsCus.SelectCommand;
            return mState;
        }

        protected void floCSal_DataBound(object sender, EventArgs e)
        {
            //Session["SALNO"] = floCCus.GetNestedControlValueByFieldName("SALNO");
        }

        protected void cbkfloCus_Callback(object sender, CallbackEventArgsBase e)
        {
            /*
            if (e.Parameter == "NavgInsert")
            {
                //新增
                //sdsCus.SelectParameters["CUSNO"].DefaultValue = null;
                //抓編碼最大值
                string CurCUSNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Cus", "CUSNO", "", true, "yyyymmdd####", DateTime.Today, Session["ActDeptBID"].ToString());
                //設定控制項初始值
                //floCus.FindItemByFieldName("CUSNO").
                edSHFTDT.Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                //設定欄位初始值
                sdsCus.InsertParameters["CUSNO"].DefaultValue = CurCUSNO;
                sdsCus.InsertParameters["LOGDT"].DefaultValue = System.DateTime.Today.ToString("yyyy/MM/dd");
                //sdsCus.InsertParameters["SHFTDT"].DefaultValue = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                sdsCus.InsertParameters["DPTNO"].DefaultValue = Session["ActDeptID"].ToString();
                if (Session["ActDeptID"].ToString() == Session["ActDeptID"].ToString())
                    sdsCus.InsertParameters["LOGID"].DefaultValue = Session["CurUserID"].ToString();
                sdsCus.Insert();
                Session["CUSNO"] = CurCUSNO;
                if (sdsCus.SelectCommand.IndexOf("WHERE ") >= 0)
                    sdsCus.SelectCommand = sdsCus.SelectCommand.Substring(0, sdsCus.SelectCommand.IndexOf("WHERE "));
                sdsCus.SelectCommand += " WHERE " + "CUSNO='" + CurCUSNO + "'";
                sdsCus.SelectCommand += " ORDER BY Cus.CUSNO";
                sdsCus.SelectCommandType = SqlDataSourceCommandType.Text;
                floCus.DataBind();
            }
            else if (e.Parameter == "NavgEdit")
            {
                //更改
                object ojtCUSNO = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "CUSNO");
                DataTable dtbCus = ((DataView)sdsCus.Select(new DataSourceSelectArguments())).ToTable();
                DataRow[] dtbCusRows = dtbCus.Select("CUSNO='" + ojtCUSNO.ToString() + "'");
                //DataTable dtbCus = (dvwCus.Select("CUSNO='" + ojtCUSNO.ToString() + "'").ToTable();
                //DataTable dtbCus = ((DataView)sdsCus.Select("CUSNO='" + ojtCUSNO.ToString() + "'")).ToTable();
                floCus.DataSourceID = null;
                floCus.DataSource = dtbCusRows;
                floCus.DataBind();
                if (sdsCus.SelectCommand.IndexOf("WHERE ") >= 0)
                    sdsCus.SelectCommand = sdsCus.SelectCommand.Substring(0, sdsCus.SelectCommand.IndexOf("WHERE "));
                sdsCus.SelectCommand += " WHERE " + "CUSNO='" + ojtCUSNO.ToString() + "'";
                sdsCus.SelectCommand += " ORDER BY Cus.CUSNO";
                sdsCus.SelectCommandType = SqlDataSourceCommandType.Text;
                floCus.DataBind();
            }
            else if (e.Parameter == "NavgView")
            {
                //調閱
                sptCusView.GetPaneByName("sppCusView").ContentUrl = "'CustACPage.aspx?CUSNO=06824'";
                //sptCusView.Panes[sptCusView.Panes.IndexOfName("sppCusView")].ContentUrl = "'CustACPage.aspx?CUSNO=06824'";
                //sptCusView.Panes[0].ContentUrl = "'CustACPage.aspx?CUSNO=06824'";
                object ojtCUSNO = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "CUSNO");
                if (ojtCUSNO != null)
                {
                    //ojtCUSNO = null --> 畫面沒有任何紀錄
                    if (sdsCus.SelectCommand.IndexOf("WHERE ") >= 0)
                        sdsCus.SelectCommand = sdsCus.SelectCommand.Substring(0, sdsCus.SelectCommand.IndexOf("WHERE "));
                    sdsCus.SelectCommand += " WHERE " + "CUSNO='" + ojtCUSNO.ToString() + "'";
                    sdsCus.SelectCommand += " ORDER BY Cus.CUSNO";
                    sdsCus.SelectCommandType = SqlDataSourceCommandType.Text;
                    floCus.DataBind();
                }
            }
            else
            {
                //edPPMANO
                string CurMANO = e.Parameter;
                ASPxTextBox edMADESC = (ASPxTextBox)floCus.FindNestedControlByFieldName("MADESC");
                ASPxTextBox edMASPEC = (ASPxTextBox)floCus.FindNestedControlByFieldName("MASPEC");
                string cmd = "SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB, "
                         + "Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST, "
                         + "Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO, Ma.ISINV "
                         + "FROM Ma "
                         + "WHERE Ma.MANO='" + CurMANO + "' ";
                DataTable MaDataTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                if (MaDataTable.Rows.Count > 0)
                {
                    edMADESC.Text = MaDataTable.Rows[0]["MADESC"].ToString();
                    edMASPEC.Text = MaDataTable.Rows[0]["MASPEC"].ToString();
                }
            }
             */ 

        }

        protected void MainDBGridMenu_ItemClick(object source, MenuItemEventArgs e)
        {
            var CurMenuItemName = e.Item.Name;
            //Export 只能在 Server 端或PostBack執行
            if (CurMenuItemName == "NavgPrintToPdf")
            {
                //Pdf務必設定Unicode, 否則無法正常顯示中文
                //MainDBGridExporter.Styles.Header.Font.Name = "Arial Unicode MS";
                //MainDBGridExporter.Styles.Cell.Font.Name = "Arial Unicode MS";
                MainDBGridExporter.WritePdfToResponse();
            }
            else if (CurMenuItemName == "NavgPrintToXls")
            {
                MainDBGridExporter.WriteXlsToResponse(new XlsExportOptionsEx { ExportType = ExportType.WYSIWYG });
            }
            else if (CurMenuItemName == "NavgPrintToXlsx")
            {
                MainDBGridExporter.WriteXlsxToResponse(new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
            }
            else if (CurMenuItemName == "NavgPrintToRtf")
            {
                MainDBGridExporter.WriteRtfToResponse();
            }
            else if (CurMenuItemName == "NavgPrintToCsv")
            {
                MainDBGridExporter.WriteCsvToResponse(new CsvExportOptionsEx() { ExportType = ExportType.WYSIWYG });
            }
        }

        protected void sdsCus_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //sdsCus.InsertParameters["CUSNO"].DefaultValue = Session["CUSNO"].ToString();
            //sdsCus.InsertParameters["SHFTDT"].DefaultValue = edSHFTDT.Value.ToString();
            //sdsCus.InsertParameters["CUSNO"].DefaultValue = tbxCUSNO.Value.ToString();
            //sdsCus.InsertParameters["CARDNO"].DefaultValue = tbxCARDNO.Value.ToString();
        }

        protected void sdsCusSal_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            /*
            string CurCUSNO = e.Command.Parameters["@CUSNO"].Value.ToString();
            string CurSHFTITNO = e.Command.Parameters["@SHFTITNO"].Value.ToString();
            DateTime CurSHFTDT = (DateTime)e.Command.Parameters["@SHFTDT"].Value;
            //string CurSALSTNO = e.Command.Parameters["@SALSTNO"].Value.ToString();
            string CurMANO = e.Command.Parameters["@MANO"].Value.ToString();
            double CurSHFTQTY = Convert.ToDouble(e.Command.Parameters["@SHFTQTY"].Value);
            double CurSHFTCST = Convert.ToDouble(e.Command.Parameters["@SALCST"].Value);
            string CurSTORID = e.Command.Parameters["@STORID"].Value.ToString();
            object OldISINV = Session["ISINV"];
            if ((OldISINV != null) && (Convert.ToBoolean(OldISINV)))
            //object OldISINV = CusSalDBGrid.GetRowValues(CusSalDBGrid.FocusedRowIndex, "ISINV");
            //if ((OldISINV != null) && (Convert.ToBoolean(CusSalDBGrid.GetRowValues(CusSalDBGrid.FocusedRowIndex, "ISINV"))))
            {
                //同步庫存
                CSal_Publ.SyncMaInv(CurMANO, "", CurSHFTQTY, CurSHFTCST, "2", CurSTORID, CSal_Publ.GetWorkYYMM(CurCUSNO, CurSHFTDT, "2"));
            }
             */
        }

        protected void sdsCusSal_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            //double CurSHFTQTY = Convert.ToDouble(e.Command.Parameters["@SHFTQTY"].Value);
            //double OldSHFTQTY = Convert.ToDouble(e.Command.Parameters["@original_SHFTQTY"].Value);
        }

        protected void sdsCusSal_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            /*
            if (Convert.ToBoolean(e.Command.Parameters["@original_ISINV"].Value))
            {
                //同步庫存
                //original_SHFTQTY 在 btnSave_Click 事件 CusSalDBGrid.GetRowValues(CusSalDBGrid.FocusedRowIndex, "SHFTQTY") 設定
                if (e.Command.Parameters["@SHFTQTY"].Value != e.Command.Parameters["@original_SHFTQTY"].Value)
                {
                    double CurSHFTQTY = Convert.ToDouble(e.Command.Parameters["@SHFTQTY"].Value);
                    double OldSHFTQTY = Convert.ToDouble(e.Command.Parameters["@original_SHFTQTY"].Value);
                    string CurCUSNO = e.Command.Parameters["@CUSNO"].ToString();
                    DateTime CurSHFTDT = (DateTime)(e.Command.Parameters["@SHFTDT"].Value);
                    string CurMANO = e.Command.Parameters["@MANO"].ToString();
                    string CurSTORID = e.Command.Parameters["@STORID"].ToString();
                    double CurSHFTCST = Convert.ToDouble(e.Command.Parameters["@SALCST"]);
                    double OldSHFTCST = Convert.ToDouble(e.Command.Parameters["@original_SALCST"]);
                    CSal_Publ.SyncMaInv(CurMANO, "", -OldSHFTQTY, -OldSHFTCST, "2", CurSTORID, CSal_Publ.GetWorkYYMM(CurCUSNO, CurSHFTDT, "2"));
                    CSal_Publ.SyncMaInv(CurMANO, "", CurSHFTQTY, CurSHFTCST, "2", CurSTORID, CSal_Publ.GetWorkYYMM(CurCUSNO, CurSHFTDT, "2"));
                }
            }
             */
        }

        protected void edHPRSNNO_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void edMAKDNO_DataBound(object sender, EventArgs e)
        {
            ASPxComboBox CurMAKDNO = sender as ASPxComboBox;
            Session["MAKDNO"] = CurMAKDNO.Value;
            //ASPxFormLayout CurfltCusd = CusSalDBGrid.FindEditFormTemplateControl("fltCusd") as ASPxFormLayout;
            //Session["DPTNO"] = ((ASPxComboBox)CurfltCusd.FindControl("edMAKDNO")).Value;
        }

        protected void mMain_ItemClick(object source, MenuItemEventArgs e)
        {

        }

        protected void sdsCus_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //刪除製程 MaRoutProc
            object CurCUSNO = e.Command.Parameters["@CUSNO"].Value;
            string DeleteCmd = "DELETE FROM Cusd "
                 + "WHERE Cusd.CUSNO='" + CurCUSNO.ToString() + "' ";
            DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, DeleteCmd);
        }

        /*
         If you place the ASP.NET AJAX UpdatePanel into one of DevExpress containers, such as ASPxSplitter, ASPxPopupControl, ASPxPageControl, etc., 
          you can get the "Cannot unregister UpdatePanel with ID since it was not registered ..."
         call the following code to forcibly register the UpdatePanel in ScriptManager before it is unloaded:
         */
        protected void UpdatePanel_Unload(object sender, EventArgs e)
        {
            RegisterUpdatePanel((UpdatePanel)sender);
        }

        protected void RegisterUpdatePanel(UpdatePanel panel)
        {
            var sType = typeof(ScriptManager);
            var mInfo = sType.GetMethod("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel", BindingFlags.NonPublic | BindingFlags.Instance);
            if (mInfo != null)
                mInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { panel });
        }

        protected void MainDBGrid_DataBound(object sender, EventArgs e)
        {
            //根據權限動態設定該功能顯示否
            /*
            string FormID = this.Form.ID;
            string ParentHeadStr;
            ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
            //DBUtility.CmdbtnSetRight(ParentHeadStr, (MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn), 1);
            ASPxButton btnInsert = (MainDBGrid.FindHeaderTemplateControl(MainDBGrid.Columns[0], "NavgInsert") as ASPxButton);
            btnInsert.Visible = DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "A").PadRight(5, '0'), false);
            //(MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).HeaderTemplate
            //根據權限動態設定編輯功能顯示否
            if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "B").PadRight(5, '0'), false))
                ((MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.BrowsableRow;
            else
                ((MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgEdit"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.Invisible;
            //根據權限動態設定刪除功能顯示否
            if (DBUtility.IsOwnRight(String.Concat(ParentHeadStr, "C").PadRight(5, '0'), false))
                ((MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.BrowsableRow;
            else
                ((MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).CustomButtons["NavgDelete"] as GridViewCommandColumnCustomButton).Visibility = GridViewCustomButtonVisibility.Invisible;
             */ 
        }

        protected override void NavgInsert_Load(object sender, EventArgs e)
        {
            base.NavgInsert_Load(sender, e);
        }
    }
}