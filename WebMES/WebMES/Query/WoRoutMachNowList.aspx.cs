/*
 * 由於MainDBGrid的的Table是動態產生, 每次 setTimeout(每8秒刷新畫面)或 GridView換頁時, 再次從Page_Load事件開始執行, MainDBGrid.DataSource=null , 最後頁面是空白數據
 *   因此務必在 MainDBGrid_DataBinding 事件設定 DataSource連結的Table:
        MainDBGrid.DataSource = GetSessionTable("MainDBGrid");
 *   在Query_Publ.ASPxSetupQuery當中的查詢 務必設定下列的HttpContext....　:
        DigiPanelTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, SelOrgStr + OrderByOrgStr);
        HttpContext.Current.Session[PanelGrid.ID] = DigiPanelTable;
 *
 * Client端設定 setTimeout, 每8秒刷新畫面--> 執行Grid的 PerformCallback()
        timeout = window.setTimeout(
        function () {
            //grid.Refresh();
            grid.PerformCallback();
        },
        15000

  每15秒在 CustomCallback事件重新產生SessionTable 
        protected void MainDBGrid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            Query_Publ.ASPxSetupQuery("PAJA0", MainDBGrid, ViewState["WhereSetStr"].ToString(), ViewState["FUNCPARAMS"].ToString(), "", ViewState["PanelGridCaption"].ToString(), "", "");
        }


 ASPxGridView
   使用 <SettingsBehavior AllowCellMerge="true" /> 讓上下row columns內容相同合併在一個Cell顯示(製令單號、產品代碼、品名...

SELECT WoRout.MACHINID, Machine.MACHINNM, WoRout.MPROCID, ManuProc.MPROCNM, WoRout.WONO,
 Wo.PMANO, Ma.MADESC
FROM WoRout LEFT JOIN ManuProc ON (WoRout.MPROCID = ManuProc.MPROCID)
 LEFT JOIN Machine ON (WoRout.MACHINID = Machine.MACHINID)
 LEFT JOIN Wo ON (WoRout.WONO = Wo.WONO)
 LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO)
WHERE WoRout.MACHINID IS NOT NULL AND WoRout.PROCSTDT <= 現在時間 AND WoRout.PROCEDDT >= 現在時間
UNION
SELECT Machine.MACHINID, Machine.MACHINNM, CAST(null AS varchar(10)) MPROCID, CAST(null AS varchar(10)) MPROCNM, CAST(null AS varchar(10)) WONO, 
 CAST(null AS varchar(10)) PMANO, CAST(null AS varchar(10)) MADESC
FROM Machine


檢查站台的最後一筆
SELECT WoRout.MACHINID, Machine.MACHINNM, WoRout.MPROCID, ManuProc.MPROCNM, WoRout.WONO, Wo.PMANO, Ma.MADESC, WoRout.PROCSTDT, WoRout.PROCEDDT FROM WoRout LEFT JOIN ManuProc ON (WoRout.MPROCID = ManuProc.MPROCID)  LEFT JOIN Machine ON (WoRout.MACHINID = Machine.MACHINID)  LEFT JOIN Wo ON (WoRout.WONO = Wo.WONO)  LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO) 
WHERE WoRout.MACHINID='GA01      ' 
ORDER BY WoRout.PROCSTDT DESC 

 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
//SqlHelper
using CPC.Utility.SQL;
//
using DevExpress.Web;
//Export 使用
using DevExpress.Export;
using DevExpress.XtraPrinting;
//
using System.Collections.Specialized;

namespace WebMES.Admin
{
    public partial class WoRoutMachNowList : DevSFQuryGrid
    {
        protected override void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //cbxBMACHINID.Value = Session["ActDeptID"] == null ? Session["CurDeptID"] : Session["ActDeptID"];
                //FillIndexCombo(cbxBMACHINID.Value.ToString());
                //cbxBMLINEID.Value = Session["CurUserID"];
                //本月的第一天及最後一天
                /*
                DateTime FirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime LastDay = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1);
                tbxBSTPROCSTDT.Text = FirstDay.ToString("yyyy/MM/dd");
                tbxBEDPROCSTDT.Text = LastDay.ToString("yyyy/MM/dd");
                */
            }
        }
        /*
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!DBUtility.IsOwnRight("FA000", false))
            {
                //未擁有組織設定的權限->只能維護隸屬部門在組織架構下的子公司以下所有子部門
                if (Session["LiveDutyDptList"] != null)
                {
                    string WhereSetMySubDpt = "";
                    string CurDptList = Session["LiveDutyDptList"].ToString();
                    if (CurDptList.IndexOf(",") >= 0)
                    {
                        //DataTable DigiPanelTable = new DataTable();
                        WhereSetMySubDpt = WhereSetMySubDpt + " AND Dpt.DPTNO IN (" + CurDptList + ")  ";
                    }
                    else
                    {
                        WhereSetMySubDpt = WhereSetMySubDpt + " AND Dpt.DPTNO='" + CurDptList + "' ";
                    }
                    sdsDpts.SelectCommand += WhereSetMySubDpt;
                    sdsDpts.SelectCommandType = SqlDataSourceCommandType.Text;
                }
            }
            //if (!IsPostBack)
            //{
                //搭配Client端設定 setTimeout, 每5秒刷新畫面, 需重新查詢
                btnGoFilter_Click(sender, e);
            //}
        }
        */
        protected override void btnGoFilter_Click(object sender, EventArgs e)
        {
            pclSearchPanel.ShowOnPageLoad = false;
            string PanelGridCaption = "";
            string WhereSetStr = "";
            string FUNCPARAMS = "";
            if (tbxBSTPROCSTDT.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " WoRout.PROCSTDT>='" + tbxBSTPROCSTDT.Text + " 00:00:00'";
                else
                    WhereSetStr += " AND WoRout.PROCSTDT>='" + tbxBSTPROCSTDT.Text + " 00:00:00'";
                PanelGridCaption += tbxBSTPROCSTDT.Text.ToString();
            }
            if (tbxBEDPROCSTDT.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " WoRout.PROCSTDT<='" + tbxBEDPROCSTDT.Text + " 23:59:59'";
                else
                    WhereSetStr += " AND WoRout.PROCSTDT<='" + tbxBEDPROCSTDT.Text + " 23:59:59'";
                PanelGridCaption += " ~ " + tbxBEDPROCSTDT.Text.ToString();
                //FUNCPARAMS = tbxBEDPROCSTDT.Text;
            }
            if (cbxBMLINEID.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Machine.MLINEID='" + cbxBMLINEID.Value.ToString().Trim() + "' ";
                else
                    WhereSetStr += " AND Machine.MLINEID='" + cbxBMLINEID.Value.ToString().Trim() + "' ";
                if (FUNCPARAMS == "")
                    FUNCPARAMS += " Machine.MLINEID='" + cbxBMLINEID.Value.ToString().Trim() + "' ";
                else
                    FUNCPARAMS += " AND Machine.MLINEID='" + cbxBMLINEID.Value.ToString().Trim() + "' ";
            }
            if (cbxBMACHINID.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Machine.MACHINID='" + cbxBMACHINID.Value.ToString().Trim() + "' ";
                else
                    WhereSetStr += " AND Machine.MACHINID='" + cbxBMACHINID.Value.ToString().Trim() + "' ";
                if (FUNCPARAMS == "")
                    FUNCPARAMS += " Machine.MACHINID='" + cbxBMACHINID.Value.ToString().Trim() + "' ";
                else
                    FUNCPARAMS += " AND Machine.MACHINID='" + cbxBMACHINID.Value.ToString().Trim() + "' ";
            }
            ViewState["WhereSetStr"] = WhereSetStr;
            ViewState["FUNCPARAMS"] = FUNCPARAMS;
            ViewState["PanelGridCaption"] = PanelGridCaption;
            Query_Publ.ASPxSetupQuery("PKC00", MainDBGrid, WhereSetStr, FUNCPARAMS, "", PanelGridCaption, "", "");
            //設定MainDBGridZone寬度 = MainDBGrid寬度, 避免水平捲軸
            //MainDBGridZone.Width = MainDBGrid.Width;
            //MainDBGridZone.Width += Unit.Parse("20px");
            //MainDBGridZone.Width = MainDBGrid.Width + Unit.Parse(MainDBGrid.Width + "20px");
        }

        protected void MainDBGridMenu_ItemClick(object source, DevExpress.Web.MenuItemEventArgs e)
        {
            var CurMenuItemName = e.Item.Name;
            if (CurMenuItemName.Substring(0, 9) == "NavgPrint")
            {
                //Export 只能在 Server 端或PostBack執行
                //Export 返回Server 端經過 Page_Init事件, MainDBGrid.DataSource=null
                //務必搭配事先已在Query_Publ設定   HttpContext.Current.Session[PanelGrid.ID] = DigiPanelTable;
                //再由DBUtility.GetSessionTable("MainDBGrid")取出
                MainDBGrid.DataSource = DBUtility.GetSessionTable("MainDBGrid");
            }
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

        }

        /*
            foreach (var args in e.UpdateValues)
            {
                //每一列row
                ProcessValues(args.NewValues);
                //int index  = MainDBGrid.FindVisibleIndexByKeyValue(args.Keys("id"));
                //string value = MainDBGrid.GetRowValues(index, "comments");

            }
            String siteId = MainDBGrid.GetRowValues(0, "fldSiteId").ToString();
        */
        protected void MainDBGrid_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
        {
            /*
            if (e.Column.FieldName == "WONO")
                //製令單號顯示後8碼
                e.DisplayText = "..." + e.Value.ToString().Substring(e.Value.ToString().Length - 8);
             */
        }

        protected void MainDBGrid_DataBinding(object sender, EventArgs e)
        {
            MainDBGrid.DataSource = DBUtility.GetSessionTable("MainDBGrid");
        }

        protected void cbkSearchPanel_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter == "edBDateRangeSel")
            {
                DBUtility.DateRangeSet(edBDateRangeSel.SelectedIndex, System.DateTime.Today, tbxBSTPROCSTDT, tbxBEDPROCSTDT);
            }
        }

        protected void MainDBGrid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            Query_Publ.ASPxSetupQuery("PKC00", MainDBGrid, ViewState["WhereSetStr"].ToString(), ViewState["FUNCPARAMS"].ToString(), "", ViewState["PanelGridCaption"].ToString(), "", "");
        }

        protected void cbxBMACHINID_Callback(object sender, CallbackEventArgsBase e)
        {
            FillcbxBMACHINIDCombo(e.Parameter);
        }

        protected void FillcbxBMACHINIDCombo(string CurMLINEID)
        {
            if (string.IsNullOrEmpty(CurMLINEID))
                return;
            Session["MLINEID"] = CurMLINEID;
            cbxBMACHINID.DataBind();
        }

    }

}