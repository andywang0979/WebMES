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
//
using System.Globalization;
//
using DevExpress.Web;
//
using System.Drawing;

public class Query_Publ
{
    //跨類別變數
    static string CurDptList = "";
    static DataTable dtOrganAll = new DataTable();

    public static void ASPxSetupQuery(string OPNO, ASPxGridView PanelGrid, string WhereSetStr = "", string FUNCPARAMS = "", string OrderSetStr = "", string PanelGridCaption = "", string WhereSetStr1 = "", string WhereSetStr2 = "")
    {
        string SelOrgStr, CurSqlStr, GroupByOrgStr, OrderByOrgStr;
        int I, TColWidth;
        //For PSA00 -當日營業資訊
        //string MMACDNO, MMACDNM;
        //float MTMACDAMT, MTMACDCNT, MTTMACDAMT, MTTMACDCNT;
        int TGOALDAY, TRECHDAY;
        string GoalWhereSetStr;
        string CurDptList;
        //Session["ExtToOverTime"] = "0" 正常工作時間以後延續工作不判別起算加班
        //Session["ExtToOverTime"] = "1" 正常工作時間以後延續工作自動起算加班(不辨別卡鐘的 5: 加班 - 簽到 6: 加班 - 簽退)
        string ExtToOverTime = "1";
        //Session["ExtMinToOverTime"] = 50 正常工作時間以後延續50分鐘以上起算加班
        int ExtMinToOverTime = 50;

        if (OPNO == "PKC00")
        {
            /*
            //機台生產現況
SELECT WoRout.MACHINID, Machine.MACHINNM, WoRout.MPROCID, ManuProc.MPROCNM, WoRout.WONO,
 Wo.PMANO, Ma.MADESC, WoRout.PROCSTDT, WoRout.PROCEDDT
FROM WoRout LEFT JOIN ManuProc ON (WoRout.MPROCID = ManuProc.MPROCID)
 LEFT JOIN Machine ON (WoRout.MACHINID = Machine.MACHINID)
 LEFT JOIN Wo ON (WoRout.WONO = Wo.WONO)
 LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO)
WHERE WoRout.MACHINID IS NOT NULL AND WoRout.PROCSTDT <= 現在時間 AND WoRout.PROCEDDT IS NULL
UNION
SELECT Machine.MACHINID, Machine.MACHINNM, CAST(null AS varchar(10)) MPROCID, CAST(null AS varchar(10)) MPROCNM, CAST(null AS varchar(10)) WONO, 
 CAST(null AS varchar(10)) PMANO, CAST(null AS varchar(10)) MADESC
FROM Machine

            *   
            */
            //所有機台
            string cmdMachine = "SELECT Machine.MACHINID, Machine.MACHINNM, CAST(null AS varchar(10)) MPROCID, CAST('待機中' AS varchar(10)) MPROCNM, CAST(null AS varchar(10)) WONO, "
                              + "CAST(null AS varchar(10)) PMANO, CAST(null AS varchar(10)) MADESC, CAST(null AS datetime) PROCSTDT, CAST(null AS datetime) PROCEDDT "
                              + "FROM Machine ";
            if (FUNCPARAMS != "")
            {
                cmdMachine += "WHERE " + FUNCPARAMS + " ";
            }
            OrderByOrgStr = "ORDER BY Machine.MACHINID";
            DataTable dtbMachine = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdMachine);
            for (int i = 0; i < dtbMachine.Rows.Count; i++)
            {
                //查詢每台機台是否已有製令製程在跑
                // 2022/07/06修正如下(避免之前殘留未報完工的工單造成抓錯)
                SelOrgStr = "SELECT TOP 1 WoRout.MACHINID, Machine.MACHINNM, WoRout.MPROCID, ManuProc.MPROCNM, WoRout.WONO, "
                   + "Wo.PMANO, Ma.MADESC, WoRout.PROCSTDT, WoRout.PROCEDDT "
                   + "FROM WoRout LEFT JOIN ManuProc ON (WoRout.MPROCID = ManuProc.MPROCID) "
                   + " LEFT JOIN Machine ON (WoRout.MACHINID = Machine.MACHINID) "
                   + " LEFT JOIN Wo ON (WoRout.WONO = Wo.WONO) "
                   + " LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO) "
                   + "WHERE WoRout.MACHINID='" + dtbMachine.Rows[i]["MACHINID"].ToString() + "' ";
                SelOrgStr = SelOrgStr + "ORDER BY WoRout.PROCSTDT DESC ";
                DataTable dtbWoRoutMach = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, SelOrgStr);
                if (dtbWoRoutMach.Rows.Count > 0)
                {
                    if (dtbWoRoutMach.Rows[0]["PROCEDDT"] == DBNull.Value)
                    {
                        dtbMachine.Rows[i]["MPROCID"] = dtbWoRoutMach.Rows[0]["MPROCID"].ToString();
                        dtbMachine.Rows[i]["MPROCNM"] = dtbWoRoutMach.Rows[0]["MPROCNM"].ToString();
                        dtbMachine.Rows[i]["WONO"] = dtbWoRoutMach.Rows[0]["WONO"].ToString();
                        dtbMachine.Rows[i]["PMANO"] = dtbWoRoutMach.Rows[0]["PMANO"].ToString();
                        dtbMachine.Rows[i]["MADESC"] = dtbWoRoutMach.Rows[0]["MADESC"].ToString();
                        dtbMachine.Rows[i]["PROCSTDT"] = dtbWoRoutMach.Rows[0]["PROCSTDT"].ToString();
                        //dtbMachine.Rows[i]["PROCEDDT"] = dtbWoRoutMach.Rows[0]["PROCEDDT"].ToString();
                    }
                }
                /* 2022/07/06放棄
                 * 若   工單1 11:00~ 結束, 但未報完工
                 * 續做 工單2 14:00~15:00  有報完工
                 * 下列會抓到 工單1, 實際已完成 工單2 
                SelOrgStr = "SELECT WoRout.MACHINID, Machine.MACHINNM, WoRout.MPROCID, ManuProc.MPROCNM, WoRout.WONO, "
                   + "Wo.PMANO, Ma.MADESC, WoRout.PROCSTDT, WoRout.PROCEDDT "
                   + "FROM WoRout LEFT JOIN ManuProc ON (WoRout.MPROCID = ManuProc.MPROCID) "
                   + " LEFT JOIN Machine ON (WoRout.MACHINID = Machine.MACHINID) "
                   + " LEFT JOIN Wo ON (WoRout.WONO = Wo.WONO) "
                   + " LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO) "
                   + "WHERE WoRout.MACHINID='" + dtbMachine.Rows[i]["MACHINID"].ToString() + "' AND WoRout.PROCSTDT <='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "' AND WoRout.PROCEDDT IS NULL ";
                SelOrgStr = SelOrgStr + "ORDER BY WoRout.PROCSTDT DESC ";
                DataTable dtbWoRoutMach = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, SelOrgStr);
                if (dtbWoRoutMach.Rows.Count > 0)
                {
                    dtbMachine.Rows[i]["MPROCID"] = dtbWoRoutMach.Rows[0]["MPROCID"].ToString();
                    dtbMachine.Rows[i]["MPROCNM"] = dtbWoRoutMach.Rows[0]["MPROCNM"].ToString();
                    dtbMachine.Rows[i]["WONO"] = dtbWoRoutMach.Rows[0]["WONO"].ToString();
                    dtbMachine.Rows[i]["PMANO"] = dtbWoRoutMach.Rows[0]["PMANO"].ToString();
                    dtbMachine.Rows[i]["MADESC"] = dtbWoRoutMach.Rows[0]["MADESC"].ToString();
                    dtbMachine.Rows[i]["PROCSTDT"] = dtbWoRoutMach.Rows[0]["PROCSTDT"].ToString();
                    //dtbMachine.Rows[i]["PROCEDDT"] = dtbWoRoutMach.Rows[0]["PROCEDDT"].ToString();
                }
                */
            }  //for (int i = 0; i < dtbMachine.Rows.Count; i++)


            /*
            SelOrgStr = "SELECT WoRout.MACHINID, Machine.MACHINNM, WoRout.MPROCID, ManuProc.MPROCNM, WoRout.WONO, "
                       + "Wo.PMANO, Ma.MADESC, WoRout.PROCSTDT, WoRout.PROCEDDT "
                       + "FROM WoRout LEFT JOIN ManuProc ON (WoRout.MPROCID = ManuProc.MPROCID) "
                       + " LEFT JOIN Machine ON (WoRout.MACHINID = Machine.MACHINID) "
                       + " LEFT JOIN Wo ON (WoRout.WONO = Wo.WONO) "
                       + " LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO) "
                       + (WhereSetStr == "" ? "WHERE WoRout.MACHINID IS NOT NULL AND WoRout.PROCSTDT <='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "' AND WoRout.PROCEDDT IS NULL "
                         : "WHERE " + WhereSetStr + " AND WoRout.MACHINID IS NOT NULL AND WoRout.PROCSTDT <='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "' AND WoRout.PROCEDDT IS NULL ")
                       //+ "WHERE WoRout.MACHINID IS NOT NULL AND WoRout.PROCSTDT <='" +DateTime.Now.ToString("yyyy/MM/dd HH:mm")+ "' AND WoRout.PROCEDDT >='" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "' "
                       + " UNION "
                       + "SELECT Machine.MACHINID, Machine.MACHINNM, CAST(null AS varchar(10)) MPROCID, CAST('待機中' AS varchar(10)) MPROCNM, CAST(null AS varchar(10)) WONO,  "
                       + "CAST(null AS varchar(10)) PMANO, CAST(null AS varchar(10)) MADESC, CAST(null AS datetime) PROCSTDT, CAST(null AS datetime) PROCEDDT "
                       + "FROM Machine ";
            //DataTable DigiPanelTable = new DataTable();
            //if (WhereSetStr != "")
            //    SelOrgStr = SelOrgStr + "WHERE " + WhereSetStr;
            SelOrgStr = SelOrgStr + "ORDER BY WoRout.MACHINID ";
            DataTable dtbWoRoutMach = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, SelOrgStr);
            */
            if (PanelGridCaption == "")
                PanelGrid.Caption = "機台生產現況 :" + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            else
                PanelGrid.Caption = "機台生產現況 :" + PanelGridCaption;
            DataTable DigiPanelTable = dtbMachine;
            //DataTable DigiPanelTable = dtbWoRoutMach;
            //DataTable DigiPanelTable = new DataTable();
            //DigiPanelTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, SelOrgStr + GroupByOrgStr + OrderByOrgStr);
            HttpContext.Current.Session[PanelGrid.ID] = DigiPanelTable;

            //動態建立ASPxGridView
            //DevExpress.Web.ASPxGridView ASPxPanelGrid = new DevExpress.Web.ASPxGridView();
            //清除上一次查詢的Columns設定
            PanelGrid.Columns.Clear();
            //動態設定每一欄型態、欄名與格式化，並將DataTable的資料自動對應每一欄中 
            foreach (DataColumn c in DigiPanelTable.Columns)
            {
                if (c.ColumnName == "WONO" || c.ColumnName == "PMANO" || c.ColumnName == "MADESC" ||
                    c.ColumnName == "PBEGDT" || c.ColumnName == "WOQTY" || c.ColumnName == "MPRODSTAT" ||
                    c.ColumnName == "MACHINID" || c.ColumnName == "MACHINNM" || c.ColumnName == "MPROCID" ||
                    c.ColumnName == "MPROCNM" ||
                    c.ColumnName == "PROCSTDT" || c.ColumnName == "PROCEDDT"
                   )
                {
                    DevExpress.Web.GridViewDataTextColumn bf = new DevExpress.Web.GridViewDataTextColumn();
                    bf.FieldName = c.ColumnName;
                    switch (c.ColumnName)
                    {
                        case "WONO":
                            bf.Caption = "製令單號";
                            bf.Width = Unit.Parse("10%");
                            bf.MinWidth = 100;
                            bf.MaxWidth = 120;
                            bf.AdaptivePriority = 0;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "PMANO":
                            bf.Caption = "產品代碼";
                            bf.Width = Unit.Parse("14%");
                            bf.MinWidth = 140;
                            bf.MaxWidth = 160;
                            bf.AdaptivePriority = 0;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "MADESC":
                            bf.Caption = "品名";
                            bf.Width = Unit.Parse("20%");
                            bf.MinWidth = 200;
                            bf.MaxWidth = 220;
                            bf.AdaptivePriority = 0;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "MACHINID":
                            bf.Caption = "機台代碼";
                            bf.Width = Unit.Parse("6%");
                            bf.MinWidth = 60;
                            bf.MaxWidth = 80;
                            bf.AdaptivePriority = 0;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "MACHINNM":
                            bf.Caption = "機台名稱";
                            bf.Width = Unit.Parse("16%");
                            bf.MinWidth = 160;
                            bf.MaxWidth = 180;
                            bf.AdaptivePriority = 0;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "WOQTY":
                            bf.Caption = "製令數量";
                            bf.Width = Unit.Parse("4%");
                            bf.MinWidth = 40;
                            bf.MaxWidth = 50;
                            bf.AdaptivePriority = 1;
                            bf.UnboundExpression = "[WOQTY] + [RTUNITNM]";
                            bf.UnboundType = DevExpress.Data.UnboundColumnType.String;
                            bf.FieldName = "U" + c.ColumnName;
                            break;
                        /*
                    case "RTUNITNM":
                        bf.Caption = "單位";
                        bf.Width = Unit.Parse("3%");
                        bf.MinWidth = 30;
                        bf.MaxWidth = 40;
                        bf.AdaptivePriority = 1;
                        //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                        break;
                        */
                        case "PBEGDT":
                            bf.Caption = "預定開工日";
                            bf.PropertiesEdit.DisplayFormatString = "{0:yy/MM/dd}";
                            bf.Width = Unit.Parse("7%");
                            bf.MinWidth = 75;
                            bf.MaxWidth = 90;
                            bf.AdaptivePriority = 1;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "BEGDT":
                            bf.Caption = "實際開工日";
                            bf.PropertiesEdit.DisplayFormatString = "{0:yy/MM/dd HH:mm}";
                            bf.Width = Unit.Parse("9%");
                            bf.MinWidth = 90;
                            bf.MaxWidth = 110;
                            bf.AdaptivePriority = 1;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "FINDT":
                            bf.Caption = "實際完工日";
                            bf.PropertiesEdit.DisplayFormatString = "{0:yy/MM/dd HH:mm}";
                            bf.Width = Unit.Parse("9%");
                            bf.MinWidth = 90;
                            bf.MaxWidth = 110;
                            bf.AdaptivePriority = 1;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        /*
                        case "MPROCID":
                            bf.Caption = "製程代碼";
                            bf.AdaptivePriority = 0;
                            bf.Width = Unit.Parse("5%");
                            bf.MinWidth = 50;
                            bf.MaxWidth = 60;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        */
                        case "MPROCNM":
                            bf.Caption = "製程名稱";
                            bf.AdaptivePriority = 0;
                            bf.Width = Unit.Parse("8%");
                            bf.MinWidth = 80;
                            bf.MaxWidth = 95;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "PUTNQTY":
                            bf.Caption = "投入數量";
                            bf.AdaptivePriority = 0;
                            bf.Width = Unit.Parse("4%");
                            bf.MinWidth = 40;
                            bf.MaxWidth = 50;
                            bf.UnboundExpression = "[PUTNQTY] + [PNUNITNM]";
                            bf.UnboundType = DevExpress.Data.UnboundColumnType.String;
                            bf.FieldName = "U" + c.ColumnName;
                            break;
                        /*
                    case "PNUNITNM":
                        bf.Caption = "單位";
                        bf.Width = Unit.Parse("3%");
                        bf.MinWidth = 30;
                        bf.MaxWidth = 40;
                        bf.AdaptivePriority = 1;
                        //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                        break;
                        */
                        case "FNSHQTY":
                            bf.Caption = "完成數量";
                            bf.AdaptivePriority = 0;
                            bf.Width = Unit.Parse("4%");
                            bf.MinWidth = 40;
                            bf.MaxWidth = 50;
                            bf.UnboundExpression = "[FNSHQTY] + [PCUNITNM]";
                            bf.UnboundType = DevExpress.Data.UnboundColumnType.String;
                            bf.FieldName = "U" + c.ColumnName;
                            break;
                        /*
                    case "PCUNITNM":
                        bf.Caption = "單位";
                        bf.Width = Unit.Parse("2%");
                        bf.MinWidth = 20;
                        bf.MaxWidth = 30;
                        bf.AdaptivePriority = 1;
                        //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                        break;
                        */
                        case "PROCSTDT":
                            bf.Caption = "開始時程";
                            bf.PropertiesEdit.DisplayFormatString = "{0:yy/MM/dd HH:mm}";
                            bf.Width = Unit.Parse("9%");
                            bf.MinWidth = 90;
                            bf.MaxWidth = 110;
                            bf.AdaptivePriority = 1;
                            bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        case "PROCEDDT":
                            bf.Caption = "完成時程";
                            bf.PropertiesEdit.DisplayFormatString = "{0:yy/MM/dd HH:mm}";
                            bf.Width = Unit.Parse("9%");
                            bf.MinWidth = 90;
                            bf.MaxWidth = 110;
                            bf.AdaptivePriority = 1;
                            //bf.CellStyle.VerticalAlign = VerticalAlign.Middle;
                            break;
                        default:
                            bf.Caption = c.ColumnName;
                            bf.Width = Unit.Parse("5%");
                            bf.AdaptivePriority = 2;
                            break;
                    }  //case I of

                    if (bf != null)
                    {
                        bf.HeaderStyle.CssClass = "StyleHead";
                        bf.CellStyle.CssClass = "StyleItem";
                        PanelGrid.Columns.Add(bf);
                    }
                }
            }
            //設定列的格式 
            PanelGrid.Styles.Footer.CssClass = "StyleFooter";
            PanelGrid.Styles.Row.CssClass = "StyleRow";
            //PanelGrid.PagerStyle.CssClass = "StylePageNo";
            PanelGrid.Styles.SelectedRow.CssClass = "StyleSelectedRow";
            PanelGrid.Styles.Header.CssClass = "StyleHead";
            PanelGrid.Styles.InlineEditRow.CssClass = "StyleEditRow";
            PanelGrid.Styles.AlternatingRow.CssClass = "StyleAlternatingRow";
            PanelGrid.Visible = true;
            //將DataSource指向DataTable 
            PanelGrid.DataSource = DigiPanelTable;
            PanelGrid.DataBind();
            PanelGrid.PageIndex = 0;
        }  //if (OPNO == "PKC00")
        else if (OPNO == "PKD00")
        {
            /*
            //逾交製令清單
SELECT Wo.WONO, Wo.WODT, Wo.PMANO, Wo.WOQTY, Wo.PBEGDT, Wo.PFINDT, Wo.BEGDT, Wo.FINDT, Wo.FNSHQTY, Wo.SCRPQTY, Wo.REMARK, Wo.ISOVER,  Wo.REQNO, Ma.MADESC, Ma.MASPEC, Ma.RTUNITNM,
 CAST(null AS varchar(20)) MPRODSTAT, CAST(null AS varchar(20)) MPRODING,
FROM Wo LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO)
             * 
SELECT Wo.WONO, Wo.WODT, Wo.PMANO, Wo.WOQTY, Wo.PBEGDT, Wo.PFINDT, Wo.BEGDT, Wo.FINDT, Wo.FNSHQTY, Wo.SCRPQTY, Wo.REMARK, Wo.ISOVER,  Wo.REQNO, Ma.MADESC, Ma.MASPEC, Ma.RTUNITNM,
   (CASE WHEN WoRout.PROCSTDT<>NULL AND WoRout.PROCEDDT=NULL) AS WoRout.MPROCID
FROM Wo LEFT JOIN WoRout ON (Wo.WONO = WoRout.WONO)
 LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO)
             * 
SELECT Wo.WONO, Wo.WODT, Wo.PMANO, Wo.WOQTY, Wo.PBEGDT, Wo.PFINDT, Wo.BEGDT, Wo.FINDT, Wo.FNSHQTY, Wo.SCRPQTY, Wo.REMARK, Wo.ISOVER,  Wo.REQNO, Ma.MADESC, Ma.MASPEC, Ma.RTUNITNM,
  'MPROCIDING'=
   CASE 
   WHEN WoRout.PROCSTDT IS NOT NULL AND WoRout.PROCEDDT IS NULL THEN WoRout.MPROCID
   ELSE '已完成'
   END ,
  'MPROCIDFIN'=
   CASE 
   WHEN WoRout.PROCSTDT IS NOT NULL AND WoRout.PROCEDDT IS NULL THEN WoRout.MPROCID
   ELSE '已完成'
   END ,
FROM Wo LEFT JOIN WoRout ON (Wo.WONO = WoRout.WONO)
 LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO)
             *   
            */
            SelOrgStr = "SELECT Wo.WONO, Wo.WODT, Wo.PMANO, Ma.MADESC, Ma.MASPEC, Wo.WOQTY, Ma.RTUNITNM, Wo.FNSHQTY, Wo.SCRPQTY, "
                       + "Wo.PBEGDT, Wo.PFINDT, Wo.BEGDT, Wo.FINDT, (CASE WHEN Wo.WOQTY=0 then 0 else Wo.FNSHQTY/Wo.WOQTY*100 END) MFNSHRATE, Wo.ISOVER "
                       + "FROM Wo LEFT JOIN Ma ON (Wo.PMANO=Ma.MANO) "
                       + " ";
            if (WhereSetStr == "")
                SelOrgStr = SelOrgStr + "WHERE Wo.FINDT IS NULL AND Wo.PFINDT<'" + DateTime.Today.ToString("yyyy/MM/dd") + "'";
            else
                SelOrgStr = SelOrgStr + "WHERE " + WhereSetStr;
            SelOrgStr = SelOrgStr + "ORDER BY Wo.PBEGDT ";
            DataTable dtbWo = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, SelOrgStr);
            if (PanelGridCaption == "")
                PanelGrid.Caption = "逾交製令清單 :" + "實際完工日空白 且 預定完工日 < " + System.DateTime.Today.ToString("yyyy/MM/dd");
            else
                PanelGrid.Caption = PanelGridCaption;

            DataTable DigiPanelTable = dtbWo;
            //DataTable DigiPanelTable = new DataTable();
            //DigiPanelTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, SelOrgStr + GroupByOrgStr + OrderByOrgStr);
            HttpContext.Current.Session[PanelGrid.ID] = DigiPanelTable;

            //動態建立ASPxGridView
            //DevExpress.Web.ASPxGridView ASPxPanelGrid = new DevExpress.Web.ASPxGridView();
            //清除上一次查詢的Columns設定
            PanelGrid.Columns.Clear();
            //動態設定每一欄型態、欄名與格式化，並將DataTable的資料自動對應每一欄中 
            foreach (DataColumn c in DigiPanelTable.Columns)
            {
                if (c.ColumnName == "WONO" || c.ColumnName == "PMANO" || c.ColumnName == "MADESC" || c.ColumnName == "RTUNITNM" ||
                    c.ColumnName == "PBEGDT" || c.ColumnName == "WOQTY" || c.ColumnName == "MPRODSTAT" || c.ColumnName == "MPROCNM" ||
                    c.ColumnName == "BEGDT" || c.ColumnName == "FINDT" || c.ColumnName == "FNSHQTY" || c.ColumnName == "SCRPQTY"
                   )
                //c.ColumnName == "LIGHTSIGN" || c.ColumnName == "MFNSHRATE")
                {
                    DevExpress.Web.GridViewDataTextColumn bf = new DevExpress.Web.GridViewDataTextColumn();
                    bf.FieldName = c.ColumnName;
                    switch (c.ColumnName)
                    {
                        case "WONO":
                            bf.Caption = "製令單號";
                            bf.Width = Unit.Parse("11%");
                            bf.MinWidth = 110;
                            bf.MaxWidth = 130;
                            bf.AdaptivePriority = 0;
                            break;
                        case "PMANO":
                            bf.Caption = "產品代碼";
                            bf.Width = Unit.Parse("10%");
                            bf.MinWidth = 100;
                            bf.MaxWidth = 115;
                            bf.AdaptivePriority = 1;
                            break;
                        case "MADESC":
                            bf.Caption = "品名";
                            bf.Width = Unit.Parse("16%");
                            bf.MinWidth = 160;
                            bf.MaxWidth = 175;
                            bf.AdaptivePriority = 0;
                            break;
                        case "WOQTY":
                            bf.Caption = "製令數量";
                            bf.Width = Unit.Parse("6%");
                            bf.MinWidth = 50;
                            bf.MaxWidth = 65;
                            bf.AdaptivePriority = 1;
                            break;
                        case "RTUNITNM":
                            bf.Caption = "單位";
                            bf.Width = Unit.Parse("2%");
                            bf.MinWidth = 20;
                            bf.MaxWidth = 35;
                            bf.AdaptivePriority = 1;
                            break;
                        case "PBEGDT":
                            bf.Caption = "預定開工日";
                            bf.PropertiesEdit.DisplayFormatString = "{0:yy/MM/dd}";
                            bf.Width = Unit.Parse("8%");
                            bf.MinWidth = 80;
                            bf.MaxWidth = 95;
                            bf.AdaptivePriority = 1;
                            break;
                        case "MPRODSTAT":
                            bf.Caption = "目前狀況";
                            bf.AdaptivePriority = 0;
                            bf.Width = Unit.Parse("6%");
                            bf.MinWidth = 60;
                            bf.MaxWidth = 75;
                            break;
                        case "MPROCNM":
                            bf.Caption = "目前製程";
                            bf.AdaptivePriority = 0;
                            bf.Width = Unit.Parse("6%");
                            bf.MinWidth = 60;
                            bf.MaxWidth = 75;
                            break;
                        case "FNSHQTY":
                            bf.Caption = "良品數量";
                            bf.AdaptivePriority = 0;
                            bf.Width = Unit.Parse("5%");
                            bf.MinWidth = 50;
                            bf.MaxWidth = 65;
                            break;
                        case "BEGDT":
                            bf.Caption = "實際開工日";
                            bf.PropertiesEdit.DisplayFormatString = "{0:yy/MM/dd HH:mm}";
                            bf.Width = Unit.Parse("8%");
                            bf.MinWidth = 80;
                            bf.MaxWidth = 95;
                            bf.AdaptivePriority = 1;
                            break;
                        case "FINDT":
                            bf.Caption = "實際完工日";
                            bf.PropertiesEdit.DisplayFormatString = "{0:yy/MM/dd HH:mm}";
                            bf.Width = Unit.Parse("8%");
                            bf.MinWidth = 80;
                            bf.MaxWidth = 95;
                            bf.AdaptivePriority = 1;
                            break;
                        case "SCRPQTY":
                            bf.Caption = "不良品數量";
                            bf.Width = Unit.Parse("5%");
                            bf.Width = Unit.Parse("5%");
                            bf.MinWidth = 50;
                            bf.MaxWidth = 65;
                            bf.AdaptivePriority = 0;
                            break;
                        /*
                    case "MFNSHRATE":
                        bf.Caption = "完工率";
                        bf.Width = Unit.Parse("4%");
                        bf.MinWidth = 40;
                        bf.MaxWidth = 55;
                        bf.AdaptivePriority = 0;
                        bf.PropertiesEdit.DisplayFormatString = "{0:0,0.00}%";
                        //bf.PropertiesEdit.DisplayFormatString = "p0";
                        break;
                        */
                        case "LIGHTSIGN":
                            DevExpress.Web.GridViewDataImageColumn ibf = new DevExpress.Web.GridViewDataImageColumn();
                            ibf.FieldName = c.ColumnName;
                            ibf.Caption = "燈號";
                            ibf.Width = Unit.Parse("3%");
                            ibf.MinWidth = 30;
                            ibf.MaxWidth = 45;
                            ibf.AdaptivePriority = 1;
                            ibf.PropertiesImage.ImageUrlFormatString = "~/Images/{0}";
                            //
                            ibf.HeaderStyle.CssClass = "StyleHead";
                            ibf.CellStyle.CssClass = "StyleItem";
                            PanelGrid.Columns.Add(ibf);
                            bf = null;
                            break;
                        default:
                            bf.Caption = c.ColumnName;
                            bf.Width = Unit.Parse("8%");
                            bf.AdaptivePriority = 2;
                            break;
                    }  //case I of

                    if (bf != null)
                    {
                        bf.HeaderStyle.CssClass = "StyleHead";
                        bf.CellStyle.CssClass = "StyleItem";
                        PanelGrid.Columns.Add(bf);
                    }
                }
            }
            //設定列的格式 
            PanelGrid.Styles.Footer.CssClass = "StyleFooter";
            PanelGrid.Styles.Row.CssClass = "StyleRow";
            //PanelGrid.PagerStyle.CssClass = "StylePageNo";
            PanelGrid.Styles.SelectedRow.CssClass = "StyleSelectedRow";
            PanelGrid.Styles.Header.CssClass = "StyleHead";
            PanelGrid.Styles.InlineEditRow.CssClass = "StyleEditRow";
            PanelGrid.Styles.AlternatingRow.CssClass = "StyleAlternatingRow";
            PanelGrid.Visible = true;
            //將DataSource指向DataTable 
            PanelGrid.DataSource = DigiPanelTable;
            PanelGrid.DataBind();
            PanelGrid.PageIndex = 0;

        }  //if (OPNO == "PKD00")

    }

}

