/*


MainDBGrid點選 即可打開DetailRow需作下列兩個設定 :
  Client端
    DetailRowExpanding事件 
      MainDBGrid.SetFocusedRowIndex(e.visibleIndex);
    RowFocusing事件
      MainDBGrid.ExpandDetailRow(e.visibleIndex);

  Server端
    MainDBGrid_DataBound事件
        if (MainDBGrid.VisibleRowCount > 0)
            MainDBGrid.DetailRows.ExpandRow(0);


SELECT ManuMach.MPROCID, ManuMach.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, 
 Machine.MLINEID, ManuLine.MLINENM
FROM  ManuMach LEFT JOIN Machine ON ManuMach.MACHINID = Machine.MACHINID
 LEFT JOIN ManuLine ON (Machine.MLINEID = ManuLine.MLINEID)
WHERE ManuMach.MPROCID=@MPROCID
ORDER BY ManuMach.MACHINID


SELECT Machine.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, 
 Machine.MLINEID, ManuLine.MLINENM, Fact.FACTNM
FROM Machine LEFT JOIN ManuLine ON (Machine.MLINEID = ManuLine.MLINEID)
 LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO) 
ORDER BY Machine.MACHINID

 *  
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//AjaxHelper
using CPC.Web.UI;
//SqlHelper
using CPC.Utility.SQL;

using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
//
using DevExpress.Web;
//using DevExpress.Web.ASPxEditors;
//using DevExpress.Web.ASPxFormLayout;
//ASPxScheduler 使用
using DevExpress.Web.ASPxScheduler;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler;
//
using DevExpress.Web.ASPxTreeList;
//Export 使用
using DevExpress.Export;
using DevExpress.XtraPrinting;

namespace WebMES.Admin
{
    public partial class ManuMach : DevDFEditGrid
    {
        protected override void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //預設 1-技服
                //Session["MACDNO"] = Session["SERVMACDNO"];
                //cbxBMACDNO.Value = Session["MACDNO"];
                //FillcbxBMPROCIDCombo(cbxBMACDNO.Value.ToString());
            }
        }
        /*
        protected void Page_Load(object sender, EventArgs e)
        {
            //根據權限動態設定該功能顯示否
            string FormID = this.Form.ID;
            string ParentHeadStr;
            ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
            DBUtility.CmdbtnSetRight(ParentHeadStr, (MainDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn), 1);
            //根據權限代碼動態設定增刪修按鍵的顯示否
            if (!IsPostBack)
            {
                //sdsDpts.SelectCommand設定完成後使用SaveViewState()
                //在callback觸發Page_Load事件時, 再使用LoadViewState()取回sdsDpts.SelectCommand原設定
                btnGoFilter_Click(sender, e);
            }
        }
        */

        protected virtual void Page_PreRenderComplete(object sender, EventArgs e)
        {
            /*
            if (MainDBGrid.VisibleRowCount > 0)
            {
                //ASPxGridView SubDBGrid = (ASPxGridView)MainDBGrid.FindDetailRowTemplateControl(0, "SubDBGrid");
                //InitGridSetting(SubDBGrid, 2);
                MainDBGrid.DetailRows.ExpandRow(0);
            }
            */
        }

        /*
        protected override void PageLoadOnce(object sender, System.EventArgs e)
        {
            base.PageLoadOnce(sender, e);
            //if (MainDBGrid.VisibleRowCount > 0)
                MainDBGrid.DetailRows.ExpandRow(0);
        }
        */
        protected override void btnGoFilter_Click(object sender, EventArgs e)
        {
            pclSearchPanel.ShowOnPageLoad = false;
            string WhereSetStr = "";
            if (cbxBMPROCID.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " ManuProc.MPROCID='" + cbxBMPROCID.Value.ToString() + "'";
                else
                    WhereSetStr += " AND ManuProc.MPROCID='" + cbxBMPROCID.Value.ToString() + "'";
            }
            if (sdsManuProc.SelectCommand.IndexOf("ORDER BY") >= 0)
                sdsManuProc.SelectCommand = sdsManuProc.SelectCommand.Substring(0, sdsManuProc.SelectCommand.IndexOf("ORDER BY") - 1);
            if (sdsManuProc.SelectCommand.IndexOf("WHERE ") >= 0)
                sdsManuProc.SelectCommand = sdsManuProc.SelectCommand.Substring(0, sdsManuProc.SelectCommand.IndexOf("WHERE "));
            if (WhereSetStr != "")
                sdsManuProc.SelectCommand += " WHERE " + WhereSetStr;
            sdsManuProc.SelectCommand += " ORDER BY ManuProc.MPROCID";
            sdsManuProc.SelectCommandType = SqlDataSourceCommandType.Text;
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
                //MainDBGrid搭配DataView, 必須自行撰寫新增更改刪除, InLine編輯模式無法直接使用SqlDataSource設定的新增更改刪除邏輯 
                string cmd = "SELECT TargNodes.ACNTYR, TargNodes.NODEID, TargNodes.NODENM, TargNodes.NODEDESC, TargNodes.WEGTPCNT "
                     + "FROM TargNodes "
                     + "WHERE TargNodes.ACNTYR=" + tbxBACNTYR.Text ;
                //dtbTargNodes = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdPrsnHairs);
                //
                MainDBGrid.DataSourceID = null;
                MainDBGrid.DataSource = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WebBSCConnectionString"].ConnectionString, cmd);
                //lvwPrsn.DataSource = ds;
                //----標準寫法 GridView1.DataSource = ds.Tables["test"].DefaultView; ----
                MainDBGrid.DataBind();
                MainDBGrid.KeyFieldName = "ACNTYR;NODEID";
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

        protected void sdsManuProc_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //if ((e.Command.Parameters["@AGENDASTTIME"] != null) && (e.Command.Parameters["@AGENDAEDTIME"] != null))
            //    e.Command.Parameters["@ATTENDTS"].Value = (DateTime)e.Command.Parameters["@AGENDAEDTIME"].Value - (DateTime)e.Command.Parameters["@AGENDASTTIME"].Value;
            //if (e.Command.Parameters["@AGENDAKDNO"].Value != "28")
            //    e.Command.Parameters["@DUTYKDNO"].Value = e.Command.Parameters["@AGENDAKDNO"].Value;
        }


        protected override void LoadViewState(Object savedState)
        {
            if (savedState != null)
            {
                object[] mState = (object[])savedState;
                if (mState[0] != null)
                    base.LoadViewState(mState[0]);
                if (mState[1] != null)
                    sdsManuProc.SelectCommand = (string)mState[1];
                if (mState[2] != null)
                    sdsManuMach.SelectCommand = (string)mState[2];
            }
        }

        //SqlDataSource.SaveViewState 方法
        protected override Object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] mState = new object[3];
            mState[0] = baseState;
            mState[1] = sdsManuProc.SelectCommand;
            mState[2] = sdsManuMach.SelectCommand;
            return mState;
        }

        protected void MainDBGridMenu_ItemClick(object source, MenuItemEventArgs e)
        {
            var CurMenuItemName = e.Item.Name;
            //Export 只能在 Server 端或PostBack執行
            if ((CurMenuItemName == "NavgPrintToPdf") || (CurMenuItemName == "NavgPrint"))
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
            /*
            else if (CurMenuItemName == "NavgDutyAssign")
            {
                DutyWorkAssign(cbxBDPTNO.Value.ToString(), Convert.ToDateTime(tbxBSTAGENDADT.Text + " 00:00:00"), Convert.ToDateTime(tbxBEDAGENDADT.Text + " 23:59:59"));
            }
            */
        }

        protected void sdsManuProc_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            //if (e.Command.Parameters["@AGENDAKDNO"].Value.ToString() != "28")
            Lead_Publ.CalcOnDutySEC(Convert.ToInt32(e.Command.Parameters["@AGENDAID"].Value));
        }

        protected void sdsManuProc_Updating(object sender, SqlDataSourceCommandEventArgs e)
        {
            //if ((e.Command.Parameters["@AGENDASTTIME"] != null) &&  (e.Command.Parameters["@AGENDAEDTIME"] != null))
            //    e.Command.Parameters["@ATTENDTS"].Value = (DateTime)e.Command.Parameters["@AGENDAEDTIME"].Value - (DateTime)e.Command.Parameters["@AGENDASTTIME"].Value;

        }

        protected void sdsManuProc_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //if (e.Command.Parameters["@AGENDAKDNO"].Value.ToString() != "28")
            //{
            //string CurAGENDAID = e.Command.Parameters["@NEWAGENDAID"].Value.ToString();
            Lead_Publ.CalcOnDutySEC(Convert.ToInt32(e.Command.Parameters["@NEWAGENDAID"].Value));
            //}
        }

        protected void sdsManuProc_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            string CurAGENDAKDNO = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "AGENDAKDNO").ToString();
            if (CurAGENDAKDNO == "28")
            {
                if ((MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "AGENDASTTIME") != DBNull.Value) && (MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "AGENDAEDTIME") != DBNull.Value))
                {
                    string CurAGENDASTTIME = ((DateTime)MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "AGENDASTTIME")).ToString("yyyy/MM/dd HH:mm");
                    string CurAGENDAEDTIME = ((DateTime)MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "AGENDAEDTIME")).ToString("yyyy/MM/dd HH:mm");
                    string CurDPTNO = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "DPTNO").ToString();
                    string CurPRSNNO = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "PRSNNO").ToString();
                    string cmdAttendDuty = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.DUTYSTTM, "
                                         + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO, Agenda.AGENDAKDNO "
                                         + "FROM Agenda "
                                         + "WHERE  Agenda.AGENDASTTIME<='" + CurAGENDASTTIME + "' "
                                         + "AND  Agenda.AGENDAEDTIME>='" + CurAGENDAEDTIME + "' "
                                         + "AND Agenda.DPTNO='" + CurDPTNO + "' AND Agenda.PRSNNO='" + CurPRSNNO + "' "
                                         + "AND  Agenda.AGENDAKDNO<>'28' ";
                    DataTable dtbAttendDuty = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendDuty);
                    if (dtbAttendDuty.Rows.Count > 0)
                        Lead_Publ.CalcOnDutySEC(Convert.ToInt32(dtbAttendDuty.Rows[0]["AGENDAID"].ToString()));
                }
            }
            else
                Lead_Publ.CalcOnDutySEC(Convert.ToInt32(e.Command.Parameters["@AGENDAID"].Value));
        }

        protected void MainDBGrid_DetailRowExpandedChanged(object sender, ASPxGridViewDetailRowEventArgs e)
        {
            /*
            DataRow MainRow = (sender as ASPxGridView).GetDataRow(e.VisibleIndex);
            string CurMaList = DBUtility.GetMaBom(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, MainRow["MPROCID"].ToString());
            string WhereSetStr = "";
            if (CurMaList.IndexOf(",") >= 0)
                WhereSetStr = "MaBom.MPROCID IN (" + CurMaList + ")";
            else
                WhereSetStr = "MaBom.MPROCID = '" + CurMaList + "' ";

            if (sdsManuMach.SelectCommand.IndexOf("ORDER BY") >= 0)
                sdsManuMach.SelectCommand = sdsManuMach.SelectCommand.Substring(0, sdsManuMach.SelectCommand.IndexOf("ORDER BY") - 1);
            if (sdsManuMach.SelectCommand.IndexOf("WHERE ") >= 0)
                sdsManuMach.SelectCommand = sdsManuMach.SelectCommand.Substring(0, sdsManuMach.SelectCommand.IndexOf("WHERE "));
            if (WhereSetStr != "")
                sdsManuMach.SelectCommand += " WHERE " + WhereSetStr;
            sdsManuMach.SelectCommand += " ORDER BY MaBom.MPROCID,MaBom.SUBMPROCID";
            sdsManuMach.SelectCommandType = SqlDataSourceCommandType.Text;

            //Session["MPROCID"] = DBUtility.GetMaBom(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString,MainRow["MPROCID"].ToString());
            //Session["MPROCID"] = MainRow["MPROCID"];
            */
        }

        protected void SubDBGrid_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            //頁面編輯未顯示MPROCID, 下列設定無效, 必須在SubDBGrid_RowInserting事件設定
            e.NewValues["MPROCID"] = MasterKeyValue.Split('|')[0].Trim();
            //e.NewValues["MPROCID"] = Session["MPROCID"];
        }

        protected override void SubDBGrid_BeforePerformDataSelect(object sender, EventArgs e)
        {
            base.SubDBGrid_BeforePerformDataSelect(sender, e);
            Session["MPROCID"] = (sender as ASPxGridView).GetMasterRowFieldValues("MPROCID");

            //string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            //Session["MPROCID"] = MasterKeyValue.Split('|')[0].Trim();

            /*
            //根據權限代碼動態設定增刪修按鍵的顯示否
            int index = MainDBGrid.FindVisibleIndexByKeyValue(MasterKeyValue);
            string FormID = this.Form.ID;
            string ParentHeadStr;
            ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
            DBUtility.CmdbtnSetRight(ParentHeadStr, ((MainDBGrid.FindDetailRowTemplateControl(index, "SubDBGrid") as ASPxGridView).Columns[0] as DevExpress.Web.GridViewCommandColumn), 2);
            */
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ASPxGridView SubDBGrid = (ASPxGridView)MainDBGrid.FindDetailRowTemplateControl(MainDBGrid.FocusedRowIndex, "SubDBGrid");
            ASPxCallbackPanel cbkPanelMPROCID = SubDBGrid.FindEditFormTemplateControl("cbkPanelMPROCID") as ASPxCallbackPanel;
            ASPxFormLayout fltMaShftd = cbkPanelMPROCID.FindControl("fltMaShftd") as ASPxFormLayout;
            if (SubDBGrid.IsNewRowEditing)
            {
                //新增
                DBUtility.FormLayoutMapToData(fltMaShftd, sdsManuMach, true);
                sdsManuMach.InsertParameters["MPROCID"].DefaultValue = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "MPROCID").ToString();

                sdsManuMach.Insert(); //Uncomment this line to allow updating.
            }
            else
            {
                //更改
                //ASPxFormLayout fltMaShftd = (SubDBGrid.FindEditFormTemplateControl("fltMaShftd") as ASPxFormLayout);
                DBUtility.FormLayoutMapToData(fltMaShftd, sdsManuMach, false);
                //ASPxFormLayout內無SHFTNO的對應元件, 須以下列取得SHFTNO值才能執行更新
                sdsManuMach.UpdateParameters["MPROCID"].DefaultValue = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "MPROCID").ToString();
                //SubDBGrid.FocusedRowIndex == -1 --> 無法取得MPROCID的值
                //sdsManuMach.UpdateParameters["MPROCID"].DefaultValue = SubDBGrid.GetRowValues(SubDBGrid.FocusedRowIndex, "MPROCID").ToString();

                sdsManuMach.Update(); //Uncomment this line to allow updating.
            }
            SubDBGrid.CancelEdit();
            SubDBGrid.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ASPxGridView SubDBGrid = (ASPxGridView)MainDBGrid.FindDetailRowTemplateControl(MainDBGrid.FocusedRowIndex, "SubDBGrid");
            SubDBGrid.CancelEdit();
        }

        protected void cbkPanelMPROCID_Callback(object sender, CallbackEventArgsBase e)
        {
            if ((e.Parameter.Length > 9) && (e.Parameter.Substring(0, 9) == "edMPROCID"))
            {
                /*
                Session["MPROCID"] = e.Parameter.Split('|')[1].Trim();
                //
                ASPxGridView SubDBGrid = (ASPxGridView)MainDBGrid.FindDetailRowTemplateControl(MainDBGrid.FocusedRowIndex, "SubDBGrid");
                ASPxCallbackPanel cbkPanelMPROCID = SubDBGrid.FindEditFormTemplateControl("cbkPanelMPROCID") as ASPxCallbackPanel;
                ASPxFormLayout fltMaShftd = cbkPanelMPROCID.FindControl("fltMaShftd") as ASPxFormLayout;
                //ASPxCallbackPanel cbkPanelMPROCID = SubDBGrid.FindEditFormTemplateControl("cbkPanelMPROCID") as ASPxCallbackPanel;
                //ASPxFormLayout fltMaShftd = cbkPanelMPROCID.FindControl("fltMaShftd") as ASPxFormLayout;
                //edMPROCID若有edMPROCID_DataBound事件務必拿掉, 否則會產生 
                //  System.InvalidOperationException: 'Databinding methods such as Eval(), XPath(), and Bind() can only be used in the context of a databound control
                ASPxComboBox edMPROCID = ((ASPxComboBox)fltMaShftd.FindControl("edMPROCID"));
                edMPROCID.DataBindItems();
                */
            }
            else
            {
                string CurMACHINID = e.Parameter;
                string cmd = "SELECT Machine.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, Machine.MLINEID, ManuLine.MLINENM "
                         + "FROM Machine LEFT JOIN ManuLine ON(Machine.MLINEID = ManuLine.MLINEID) "
                         + "WHERE Machine.MACHINID='" + CurMACHINID + "' ";
                DataTable MaDataTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                if (MaDataTable.Rows.Count == 0)
                {
                    return;
                }
                else
                {
                    //DetailRow(使用ASPxCallbackPanel 包裹 ASPxGridView)
                    ASPxCallbackPanel cbkPanelMPROCID = (ASPxCallbackPanel)MainDBGrid.FindDetailRowTemplateControl(MainDBGrid.FocusedRowIndex, "cbkPanelMPROCID");
                    ASPxGridView SubDBGrid = cbkPanelMPROCID.FindControl("SubDBGrid") as ASPxGridView;
                    cbkPanelMPROCID.JSProperties["cpISedMACHINID"] = true;
                    cbkPanelMPROCID.JSProperties["cpMACHINNM"] = MaDataTable.Rows[0]["MACHINNM"].ToString();
                    cbkPanelMPROCID.JSProperties["cpCAPACITY"] = MaDataTable.Rows[0]["CAPACITY"].ToString();

                    //SubDBGrid使用EditForm
                    //ASPxFormLayout fltMaShftd = SubDBGrid.FindEditFormTemplateControl("fltMaShftd") as ASPxFormLayout;
                    //ASPxTextBox edMADESC = ((ASPxTextBox)fltMaShftd.FindControl("edMADESC"));
                    //ASPxTextBox edMASPEC = ((ASPxTextBox)fltMaShftd.FindControl("edMASPEC"));
                    //ASPxCheckBox edISINV = ((ASPxCheckBox)fltMaShftd.FindControl("edISINV"));

                    //edMADESC.Text = MaDataTable.Rows[0]["MADESC"].ToString();
                    //edMASPEC.Text = MaDataTable.Rows[0]["MASPEC"].ToString();
                    //edISINV.Value = (bool)MaDataTable.Rows[0]["ISINV"];
                    //設定 UNITCOST 計算成本
                    //((DataRow)EditingRowMaShftd).ISINV = (bool)MaDataTable.Rows[0]["ISINV"];
                }
            }
        }

        protected void MainDBGrid_DataBound(object sender, EventArgs e)
        {
            if (MainDBGrid.VisibleRowCount > 0)
                MainDBGrid.DetailRows.ExpandRow(0);
        }

        protected void SubDBGrid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            e.NewValues["MPROCID"] = MasterKeyValue;
        }
    }
}