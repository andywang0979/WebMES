/*
SELECT Machine.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, 
 Machine.MLINEID, ManuLine.MLINENM, Fact.FACTNM
FROM Machine LEFT JOIN ManuLine ON (Machine.MLINEID = ManuLine.MLINEID)
 LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO) 
ORDER BY Machine.MACHINID
 *  
  
SELECT ManuLine.MLINEID, ManuLine.MLINENM, ManuLine.FACTNO, Fact.FACTNM
FROM ManuLine LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO)
ORDER BY ManuLine.MLINEID  
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

using DevExpress.Web;
//using DevExpress.Web.ASPxEditors;
//using DevExpress.Web.ASPxFormLayout;
//Export 使用
using DevExpress.Export;
using DevExpress.XtraPrinting;

namespace WebMES.Admin
{
    public partial class Machine2 : DevSFEditGrid
    {
        protected override void Page_Init(object sender, EventArgs e)
        {
            //hfdBACNTYR.Value = Session["ACNTYR"].ToString();
            //tbxBACNTYR.Text = hfdBACNTYR.Value;
        }

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        hfdBACNTYR.Value = Session["ACNTYR"].ToString();
        //        tbxBACNTYR.Text = hfdBACNTYR.Value;
        //        btnGoFilter_Click(sender, e);
        //    }
        //}

        protected override void btnGoFilter_Click(object sender, EventArgs e)
        {

        }

        /*
        public static Control GetPostBackControl(System.Web.UI.Page page)//取得是哪個控制項觸發 Postback
        {
            Control control = null;

            string ctrlname = page.Request.Params.Get("__EVENTTARGET");
            if (ctrlname != null && ctrlname != string.Empty)
            {
                control = page.FindControl(ctrlname);
            }
            else
            {
                foreach (string ctl in page.Request.Form)
                {
                    Control c = page.FindControl(ctl);
                    if (c is System.Web.UI.WebControls.Button)
                    {
                        control = c;
                        break;
                    }
                }
            }
            return control;
        }
        protected override void LoadViewState(Object savedState)
        {
            if (savedState != null)
            {
                object[] mState = (object[])savedState;
                if (mState[0] != null)
                    base.LoadViewState(mState[0]);
                if (mState[1] != null)
                    sdsPunchGear.SelectCommand = (string)mState[1];
            }
        }

        //SqlDataSource.SaveViewState 方法
         protected override Object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] mState = new object[2];
            mState[0] = baseState;
            mState[1] = sdsPunchGear.SelectCommand;
            return mState;
        }
        */

        protected void MainDBGrid_CustomColumnDisplayText(object sender, DevExpress.Web.ASPxGridViewColumnDisplayTextEventArgs e)
        {
            //if (e.Column.FieldName != "WEGTPCNT")
            //    return;
            //e.DisplayText = e.Value+"%";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            MainDBGrid.CancelEdit();
        }

        protected void MainDBGridMenu_ItemClick(object source, MenuItemEventArgs e)
        {
            var CurMenuItemName = e.Item.Name;
            //Export 只能在 Server 端或PostBack執行
            if((CurMenuItemName == "NavgPrintToPdf")||(CurMenuItemName == "NavgPrint"))
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
    }
}