using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web; //MenuItemEventArgs使用
using DevExpress.Export; //ExportType使用
using DevExpress.XtraPrinting; //XlsxExportOptionsEx、XlsExportOptionsEx、CsvExportOptionsEx使用

namespace WebMES.Admin
{
    //public partial class TW004 : System.Web.UI.Page　//新建立檔案時繼承自System.Web.UI.Page
    public partial class TW004 : DevSFEditGrid //改繼承自DevSFEditGrid
    {
        /*
        //由System.Web.UI.Page改繼承自DevSFEditGrid後
        //需增加 protected override void Page_Init 和 protected override void btnGoFilter_Click
        //並將新建立檔案時自動產生的protected void Page_Load取消
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        */
        
        protected override void Page_Init(object sender, EventArgs e)
        {
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

        }

        protected void MainDBGrid_CustomColumnDisplayText(object sender, DevExpress.Web.ASPxGridViewColumnDisplayTextEventArgs e)
        {
            //if (e.Column.FieldName != "WEGTPCNT")
            //    return;
            //e.DisplayText = e.Value+"%";

        }

        //查詢視窗使用
        protected void cbkSearchPanel_Callback(object sender, CallbackEventArgsBase e)
        {
            /*
            //設定查詢視窗內欄位預設值
            if (e.Parameter == "edBMFeeYR")
            {
                tbxBSTACPTDT.Text = edBMFeeYR.Value + "/03/20";
                tbxBEDACPTDT.Text = (Convert.ToUInt16(edBMFeeYR.Value) + 1).ToString() + "/03/19";
            }
            */
        }

        protected override void btnGoFilter_Click(object sender, EventArgs e)
        //protected void btnGoFilter_Click(object sender, EventArgs e)
        {
            //將進階查詢設定傳入
            pclSearchPanel.ShowOnPageLoad = false; //關閉顯示
            string WhereSetStr = "";
            if (edFactNo.Value != null)
            {
                WhereSetStr += " Fact.FactNO ='" + edFactNo.Value.ToString() + "' ";
            }

            if (sdsMachine.SelectCommand.IndexOf("ORDER BY") >= 0)
                sdsMachine.SelectCommand = sdsMachine.SelectCommand.Substring(0, sdsMachine.SelectCommand.IndexOf("ORDER BY") - 1);
            if (sdsMachine.SelectCommand.IndexOf("WHERE ") >= 0)
                sdsMachine.SelectCommand = sdsMachine.SelectCommand.Substring(0, sdsMachine.SelectCommand.IndexOf("WHERE "));
            if (WhereSetStr != "")
                sdsMachine.SelectCommand += " WHERE " + WhereSetStr;
        }

        protected void SelectManuLine_Click(object sender, EventArgs e)
        {
        }
    }
}