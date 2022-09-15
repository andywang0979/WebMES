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
        8000

  每8秒在 CustomCallback事件重新產生SessionTable 
        protected void MainDBGrid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            Query_Publ.ASPxSetupQuery("PAJA0", MainDBGrid, ViewState["WhereSetStr"].ToString(), ViewState["FUNCPARAMS"].ToString(), "", ViewState["PanelGridCaption"].ToString(), "", "");
        }


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
    public partial class WoODueList : DevSFQuryGrid
    {
        protected override void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //cbxBMAKDNO.Value = Session["ActDeptID"] == null ? Session["CurDeptID"] : Session["ActDeptID"];
                //FillIndexCombo(cbxBMAKDNO.Value.ToString());
                //cbxBMANO.Value = Session["CurUserID"];
                //本月的第一天及最後一天
                DateTime FirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime LastDay = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1);
                tbxBSTPBEGDT.Text = FirstDay.ToString("yyyy/MM/dd");
                tbxBEDPBEGDT.Text = LastDay.ToString("yyyy/MM/dd");

                //tbxBSTPBEGDT.Text = System.DateTime.Today.AddYears(-1).ToString("yyyy/MM/dd");
                //tbxBEDPBEGDT.Text = System.DateTime.Today.ToString("yyyy/MM/dd");

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
            //tbxBSTPBEGDT.Validate();
            if (tbxBSTPBEGDT.Text == "")
            {
                //AjaxHelper.showAlert((Control)sender, "*為必填欄位。");
                return;
            }
            pclSearchPanel.ShowOnPageLoad = false;
            string PanelGridCaption = "逾交製令清單 : "+DateTime.Today.ToString("yyyy/MM/dd")+"以前尚未完工";
            string WhereSetStr = "Wo.FINDT IS NULL AND Wo.PFINDT<'"+ DateTime.Today.ToString("yyyy/MM/dd") +"'";
            string FUNCPARAMS = "";
            /*
            if (tbxBSTPBEGDT.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Wo.PBEGDT>='" + tbxBSTPBEGDT.Text + " 00:00:00'";
                else
                    WhereSetStr += " AND Wo.PBEGDT>='" + tbxBSTPBEGDT.Text + " 00:00:00'";
                PanelGridCaption += tbxBSTPBEGDT.Text.ToString();
            }
            if (tbxBEDPBEGDT.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Wo.PBEGDT<='" + tbxBEDPBEGDT.Text + " 23:59:59'";
                else
                    WhereSetStr += " AND Wo.PBEGDT<='" + tbxBEDPBEGDT.Text + " 23:59:59'";
                PanelGridCaption += " ~ " + tbxBEDPBEGDT.Text.ToString();
                FUNCPARAMS = tbxBEDPBEGDT.Text;
            }
            if (cbxBMANO.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Wo.PMANO='" + cbxBMANO.Value.ToString().Trim() + "' ";
                else
                    WhereSetStr += " AND Wo.PMANO='" + cbxBMANO.Value.ToString().Trim() + "' ";
                PanelGridCaption += tbxBSTPBEGDT.Text.ToString();
            }
            ViewState["WhereSetStr"] = WhereSetStr;
            ViewState["FUNCPARAMS"] = FUNCPARAMS;
            ViewState["PanelGridCaption"] = PanelGridCaption;
            */
            Query_Publ.ASPxSetupQuery("PKD00", MainDBGrid, WhereSetStr, FUNCPARAMS, "", PanelGridCaption, "", "");
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


        protected void FillIndexCombo(string CurDPTNO)
        {
            if (string.IsNullOrEmpty(CurDPTNO))
                return;
            Session["DPTNO"] = CurDPTNO;
            cbxBMANO.DataBind();
        }

        protected void cbxBMANO_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            FillIndexCombo(e.Parameter);
        }

        protected void MainDBGrid_CustomUnboundColumnData(object sender, DevExpress.Web.ASPxGridViewColumnDataEventArgs e)
        {
            /*
            if (e.Column.FieldName.Substring(0,7) == "UDUTYKD")
            {
                if ((e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYKD" + e.Column.FieldName.Substring(7, 2)).ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME" + e.Column.FieldName.Substring(7, 2)).ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME" + e.Column.FieldName.Substring(7, 2)).ToString() != ""))
                //if ((e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYSTTM").ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM").ToString() != ""))
                {
                    //加入實際差勤時間
                    //e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM") != null 當物件null 會跳出
                    DateTime CurDUTYSTTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME" + e.Column.FieldName.Substring(7, 2));
                    DateTime CurDUTYEDTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME" + e.Column.FieldName.Substring(7, 2));
                    //DateTime CurDUTYSTTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME" + e.Column.FieldName.Substring(7, 2));
                    //DateTime CurDUTYEDTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME" + e.Column.FieldName.Substring(7, 2));
                    e.Value += CurDUTYSTTM.ToString("HH:mm") + "~" + CurDUTYEDTM.ToString("HH:mm");
                }
            }
            */
            /*
            else if (e.Column.FieldName.Substring(0, 9) == "UAGENDAKD")
            {
                if ((e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAKD" + e.Column.FieldName.Substring(9, 2)).ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "ATTENDDUTYSEC" + e.Column.FieldName.Substring(9, 2)) != DBNull.Value) && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME" + e.Column.FieldName.Substring(9, 2)).ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME" + e.Column.FieldName.Substring(9, 2)).ToString() != ""))
                {
                    //加入實際差假時間
                    //e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM") != null 當物件null 會跳出
                    DateTime CurDUTYSTTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME" + e.Column.FieldName.Substring(9, 2));
                    DateTime CurDUTYEDTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME" + e.Column.FieldName.Substring(9, 2));
                    //e.Value += CurDUTYSTTM.ToString("HH:mm") + "~" + CurDUTYEDTM.ToString("HH:mm");
                    double CurAttendSpanSEC = Convert.ToDouble(e.GetListSourceFieldValue(e.ListSourceRowIndex, "ATTENDDUTYSEC" + e.Column.FieldName.Substring(9, 2)));
                    TimeSpan AttendSpan = TimeSpan.FromSeconds(CurAttendSpanSEC);
                    e.Value = AttendSpan.ToString();
                }
            }
            */ 
        }

        protected void cbxBMANO_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        protected void cbxBMANO_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            ASPxComboBox comboBox = (ASPxComboBox)source;
            sdsMaPop.SelectCommand =
                   @"SELECT [MANO], [MADESC], [MASPEC] FROM (select [MANO], [MADESC], [MASPEC], row_number()over(order by t.[MANO]) as [rn] from [Ma] as t where (([MANO] + ' ' + [MADESC] + ' ' + [MASPEC]) LIKE @filter)) as st where st.[rn] between @startIndex and @endIndex";

            sdsMaPop.SelectParameters.Clear();
            sdsMaPop.SelectParameters.Add("filter", TypeCode.String, string.Format("%{0}%", e.Filter));
            sdsMaPop.SelectParameters.Add("startIndex", TypeCode.Int64, (e.BeginIndex + 1).ToString());
            sdsMaPop.SelectParameters.Add("endIndex", TypeCode.Int64, (e.EndIndex + 1).ToString());
            comboBox.DataSourceID = null;
            comboBox.DataSource = sdsMaPop;
            //comboBox.DataBind();
            comboBox.DataBindItems();
        }

        protected void cbxBMANO_ItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
        {
            long value = 0;
            if (e.Value == null || !Int64.TryParse(e.Value.ToString(), out value))
                return;
            ASPxComboBox comboBox = (ASPxComboBox)source;
            sdsMaPop.SelectCommand = @"SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC FROM Ma WHERE (MANO = @MANO) ORDER BY MANO";

            sdsMaPop.SelectParameters.Clear();
            sdsMaPop.SelectParameters.Add("MANO", TypeCode.String, e.Value.ToString());
            comboBox.DataSourceID = null;
            comboBox.DataSource = sdsMaPop;
            //comboBox.DataBind();
            comboBox.DataBindItems();
        }

        protected void MainDBGrid_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
        {
            //if (e.Column.FieldName == "WONO")
            //    //製令單號顯示後8碼
            //    e.DisplayText = "..." + e.Value.ToString().Substring(e.Value.ToString().Length - 8);
        }

        protected void MainDBGrid_DataBinding(object sender, EventArgs e)
        {
            MainDBGrid.DataSource = DBUtility.GetSessionTable("MainDBGrid");
        }

        protected void cbkSearchPanel_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter == "edBDateRangeSel")
            {
                DBUtility.DateRangeSet(edBDateRangeSel.SelectedIndex, System.DateTime.Today, tbxBSTPBEGDT, tbxBEDPBEGDT);
            }
        }

        protected void MainDBGrid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            Query_Publ.ASPxSetupQuery("PKD00", MainDBGrid, ViewState["WhereSetStr"].ToString(), ViewState["FUNCPARAMS"].ToString(), "", ViewState["PanelGridCaption"].ToString(), "", "");
        }
    }

}