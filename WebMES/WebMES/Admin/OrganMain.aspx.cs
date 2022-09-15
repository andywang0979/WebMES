/*
 設定tbxBACNTYRValidationGroup=entryGroup會造成 .text內容會被清空而無法設定初值 
 搭配按鍵btnGoFilter使用ASPxClientEdit.ValidateGroup檢查輸入的合法性 
 btnGoFilter.Click事件 
   if(ASPxClientEdit.ValidateGroup('entryGroup')) pclSearchPanel.Hide();
 *
 * 
 目前使用
 //前端檢查 
 btnGoFilter.Click事件 
   tbxBACNTYR.Validate();
 搭配後端
 btnGoFilter_Click
 { 
    if (tbxBACNTYR.Text == "")
    {
        return;
    }
    pclSearchPanel.ShowOnPageLoad = false;
 } 
 或 
 後端檢查
 btnGoFilter_Click
 { 
    tbxBACNTYR.Validate();  //後端檢查
    if (tbxBACNTYR.Text == "")
    {
        return;
    }
    pclSearchPanel.ShowOnPageLoad = false;
 } 
 * 
 * 在ASPxTreeList EditForm Template 中的ASPxFormLayout 自行定義 btnSubmit.OnClick="btnSubmit_Click" 無法作動 ?
 *   僅能使用下列 :
            <dx:ASPxTreeListTemplateReplacement runat="server" ReplacementType="UpdateButton" />
            <dx:ASPxTreeListTemplateReplacement runat="server" ReplacementType="CancelButton" />

 * 目前新增時使用NodeInserting事件進行存入
        protected void MainDBTree_NodeInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            ASPxFormLayout floDpt = MainDBTree.FindEditFormTemplateControl("floDpt") as ASPxFormLayout;
            DBUtility.FormLayoutInsertToValues(floDpt, e, true);
            //在MainDBTree_NodeInserted事件新增組織
        }

 * 所以在sdsOrgan_Inserted事件內, 不可使用sdsOrgan.InsertParameters["PHOTOFILENM"].DefaultValue
   因為 DBUtility.FormLayoutInsertToValues程序是使用 e.NewValues[layoutItem.FieldName] =....
        protected void sdsOrgan_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //照片處理
            if (e.Command.Parameters["@PHOTOFILENM"].Value.ToString() != "")
            //if (sdsOrgan.InsertParameters["PHOTOFILENM"].DefaultValue != null)

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
using DevExpress.Web.ASPxTreeList;
//Export 使用
using DevExpress.Export;
using DevExpress.XtraPrinting;
using DevExpress.Web;
//
using System.IO;
using System.IO.Compression;

namespace WebMES.Admin
{
    public partial class OrganMain : DevSFEditTree
    {
        const string UploadDirectory = "~/OrgnPhoto/";

        protected override void Page_Init(object sender, EventArgs e)
        {
            //hfdBACNTYR.Value = Session["ACNTYR"].ToString();
            //tbxBACNTYR.Text = hfdBACNTYR.Value;
        }

        protected override void PageLoadPreAlways(object sender, System.EventArgs e)
        {
            Page.Header.DataBind();
        }
        /*
        protected void Page_Load(object sender, EventArgs e)
        {
            //根據當地語言設定TreeList的Caption
            string CurLangCaption;
            foreach (TreeListColumn column in MainDBTree.Columns)
            {
                if ((column is TreeListTextColumn) && (column.Visible))
                {
                    CurLangCaption = DBUtility.ObtainResCaption("OrganMain", (column as TreeListTextColumn).FieldName);
                    if (CurLangCaption != "?")
                        column.Caption = CurLangCaption;
                }
            }
            //
            if (!IsPostBack)
            {
                //hfdBACNTYR.Value = Session["ACNTYR"].ToString();
                //tbxBACNTYR.Text = hfdBACNTYR.Value;
                btnGoFilter_Click(sender, e);
            }
        }
        */
        protected override void btnGoFilter_Click(object sender, EventArgs e)
        {
            pclSearchPanel.ShowOnPageLoad = false;
            string WhereSetStr = "";
            string WhereSetStrDpt = "";
            string WhereSetStrDptP = "";
            string WhereSetStrDptPP = "";
            string WhereSetStrDISSOLVDT = "";
            //string WhereSetStrDptPDISSOLVDT = "";
            //WhereSetStrDISSOLVDT : 部門裁撤日條件式
            if (cbxBDPTNO.Value != null)
            {
                string CurDptList = "";
                if (cbxBEDBISCONTAINSUBDPT.Checked)
                    CurDptList = DBUtility.GetDptBom(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cbxBDPTNO.Value.ToString());
                else
                    CurDptList = cbxBDPTNO.Value.ToString();

                if (CurDptList.IndexOf(",") > 0)
                    WhereSetStrDpt = " SubDpt.DPTNO IN (" + CurDptList + ") ";
                else
                    WhereSetStrDpt = " SubDpt.DPTNO='" + CurDptList + "' ";

                if (WhereSetStr == "")
                    WhereSetStr += WhereSetStrDpt;
                else
                    WhereSetStr += " AND " + WhereSetStrDpt;
            }
            //部門選項
            switch (EDBDPTSHIFTMD.SelectedItem.Index)
            {
                case 0:
                    //運作中部門
                    if (WhereSetStrDISSOLVDT == "")
                        WhereSetStrDISSOLVDT += " SubDpt.DISSOLVDT IS NULL ";
                    else
                        WhereSetStrDISSOLVDT += " AND SubDpt.DISSOLVDT IS NULL ";
                    break;
                case 1:
                    //歷年部門
                    if (tbxBEDDISSOLVDT.Value != null)
                    {
                        if (WhereSetStrDISSOLVDT == "")
                            WhereSetStrDISSOLVDT += " SubDpt.DISSOLVDT<='" + tbxBEDDISSOLVDT.Text + " 23:59:59' ";
                        else
                            WhereSetStrDISSOLVDT += " AND SubDpt.DISSOLVDT<='" + tbxBEDDISSOLVDT.Text + " 23:59:59' ";
                    }
                    break;
                case 2:
                    //已裁撤部門
                    if (WhereSetStrDISSOLVDT == "")
                        WhereSetStrDISSOLVDT += " SubDpt.DISSOLVDT IS NOT NULL ";
                    else
                        WhereSetStrDISSOLVDT += " AND SubDpt.DISSOLVDT IS NOT NULL ";
                    if (tbxBSTDISSOLVDT.Value != null)
                    {
                        if (WhereSetStrDISSOLVDT == "")
                            WhereSetStrDISSOLVDT += " SubDpt.DISSOLVDT>='" + tbxBSTDISSOLVDT.Text + " 00:00:00'";
                        else
                            WhereSetStrDISSOLVDT += " AND SubDpt.DISSOLVDT>='" + tbxBSTDISSOLVDT.Text + " 00:00:00'";
                    }
                    if (tbxBEDDISSOLVDT.Value != null)
                    {
                        if (WhereSetStrDISSOLVDT == "")
                            WhereSetStrDISSOLVDT += " SubDpt.DISSOLVDT<='" + tbxBEDDISSOLVDT.Text + " 23:59:59'";
                        else
                            WhereSetStrDISSOLVDT += " AND SubDpt.DISSOLVDT<='" + tbxBEDDISSOLVDT.Text + " 23:59:59'";
                    }
                    break;
            }
            if (sdsOrgan.SelectCommand.IndexOf("ORDER BY") >= 0)
                sdsOrgan.SelectCommand = sdsOrgan.SelectCommand.Substring(0, sdsOrgan.SelectCommand.IndexOf("ORDER BY") - 1);
            if (sdsOrgan.SelectCommand.IndexOf("WHERE ") >= 0)
                sdsOrgan.SelectCommand = sdsOrgan.SelectCommand.Substring(0, sdsOrgan.SelectCommand.IndexOf("WHERE "));
            //WhereSetStrDISSOLVDT 有預設值, 不會是空白
            sdsOrgan.SelectCommand += " WHERE " + WhereSetStrDISSOLVDT
                                   + (WhereSetStrDpt == "" ? "" : " AND ") + WhereSetStrDpt;
            sdsOrgan.SelectCommand += " ORDER BY Organ.DPTNO,Organ.SUBDPTNO";

            /*
            string WhereSetStr = "";
            if (cbxBDPTNO.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Organ.DPTNO='" + cbxBDPTNO.Value.ToString() + "'";
                else
                    WhereSetStr += " AND Organ.DPTNO='" + cbxBDPTNO.Value.ToString() + "'";
            }
            if (sdsOrgan.SelectCommand.IndexOf("ORDER BY") >= 0)
                sdsOrgan.SelectCommand = sdsOrgan.SelectCommand.Substring(0, sdsOrgan.SelectCommand.IndexOf("ORDER BY") - 1);
            if (sdsOrgan.SelectCommand.IndexOf("WHERE ") >= 0)
                sdsOrgan.SelectCommand = sdsOrgan.SelectCommand.Substring(0, sdsOrgan.SelectCommand.IndexOf("WHERE "));
            if (WhereSetStr != "")
                sdsOrgan.SelectCommand += " WHERE " + WhereSetStr;
            //sdsOrgan.SelectCommand += " ORDER BY SubDpt.DPTNO";
             */
 
            /*
             * 以Dpt.DPTNO所有部門 為主 
                SELECT SubDpt.DPTNO DPTNO, SubDpt.DPTNM DPTNM, Organ.DPTNO PARDPTNO,
                 SubDpt.DPTHANDLER, SubDpt.VALIDDT, SubDpt.DISSOLVDT,
                 Prsn.PRSNNM, SubDpt.DPTBID, SubDpt.STORID,
                 SubDpt.DPTADDR, SubDpt.DPTZIPCD, SubDpt.DPTTELNO, SubDpt.DPTFAXNO, SubDpt.DPTEMAILBOX,
                 SubDpt.PHOTOFILENM,  SubDpt.PHOTODESC, SubDpt.ISCORP, SubDpt.ISSHOP
                FROM Organ FULL JOIN Dpt SubDpt ON (Organ.SUBDPTNO=SubDpt.DPTNO)
                 LEFT JOIN Prsn ON (SubDpt.DPTHANDLER=Prsn.PRSNNO)
             * 以Organ.DPTNO所有部門 為主
             *  0AD05 行銷部 ; SD014 技研中心 : 不在 Organ.DPTNO 不會出現
                SELECT Organ.DPTNO PARDPTNO, Dpt.DPTNM PARDPTNM, Dpt.ISCORP PARISCORP, Organ.SUBDPTNO DPTNO, SubDpt.DPTNM , SubDpt.ISCORP
                FROM Organ LEFT JOIN Dpt ON (Organ.DPTNO=Dpt.DPTNO)
                 LEFT JOIN Dpt SubDpt ON (Organ.SUBDPTNO=SubDpt.DPTNO)
                ORDER BY Organ.DPTNO,Organ.SUBDPTNO
             */

            sdsOrgan.SelectCommandType = SqlDataSourceCommandType.Text;
            MainDBTree.DataBind();
            MainDBTree.ExpandAll();
            //MainDBTree.ExpandToLevel(2);
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
                MainDBGrid.DataSource = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
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

        protected override void LoadViewState(Object savedState)
        {
            if (savedState != null)
            {
                object[] mState = (object[])savedState;
                if (mState[0] != null)
                    base.LoadViewState(mState[0]);
                if (mState[1] != null)
                    sdsOrgan.SelectCommand = (string)mState[1];
            }
        }

        //SqlDataSource.SaveViewState 方法
        protected override Object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] mState = new object[2];
            mState[0] = baseState;
            mState[1] = sdsOrgan.SelectCommand;
            return mState;
        }

        protected void MainDBGrid_CustomColumnDisplayText(object sender, DevExpress.Web.ASPxGridViewColumnDisplayTextEventArgs e)
        {
            //if (e.Column.FieldName != "WEGTPCNT")
            //    return;
            //e.DisplayText = e.Value+"%";

        }

        protected void MainDBTreeMenu_ItemClick(object source, DevExpress.Web.MenuItemEventArgs e)
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
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //更改
            ASPxFormLayout floDpt = MainDBTree.FindEditFormTemplateControl("floDpt") as ASPxFormLayout;

            DBUtility.FormLayoutMapToData(floDpt, sdsOrgan, false);
            sdsOrgan.Update(); //Uncomment this line to allow updating.
            MainDBTree.CancelEdit();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            MainDBTree.CancelEdit();
        }

        protected void MainDBTree_NodeInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            ASPxFormLayout floDpt = MainDBTree.FindEditFormTemplateControl("floDpt") as ASPxFormLayout;
            DBUtility.FormLayoutInsertToValues(floDpt, e, true);
            //在MainDBTree_NodeInserted事件新增組織
        }

        protected void MainDBTree_InitNewNode(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            //設定新增的部門代號
            object ParentDPTNO = MainDBTree.NewNodeParentKey;
            e.NewValues["PARDPTNO"] = ParentDPTNO;
            e.NewValues["VALIDDT"] = DateTime.Today.ToString("yyyy/MM/dd");
            //object parid = (sender as ASPxTreeList).NewNodeParent;
            //e.NewValues[MainDBTree.ParentFieldName];
        }

        protected void MainDBTree_NodeInserted(object sender, DevExpress.Web.Data.ASPxDataInsertedEventArgs e)
        {
            if (e.NewValues["PARDPTNO"] != null)
            {
                //新增組織
                string UpdateCmd = "INSERT INTO Organ  "
                             + "(DPTNO, SUBDPTNO) "
                             + "VALUES "
                             + "( "
                             + "'" + e.NewValues["PARDPTNO"] + "', "
                             + "'" + e.NewValues["DPTNO"] + "' "
                             + ") ";
                if (SqlHelper.ExecuteNonQuery(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, CommandType.Text, UpdateCmd) != 1)
                {
                    Response.Write("組織新增部門失敗！");
                }
            }

            /*
             * InLine模式使用
            object ParentDPTNO = MainDBTree.NewNodeParentKey;
            string CurDPTNO = e.NewValues["DPTNO"].ToString();
            if (ParentDPTNO != "")
            {
                //Organ新增
                string KeyIndexsDptSql = "INSERT INTO Organ (DPTNO,SUBDPTNO) VALUES("
                               + "'" + ParentDPTNO + "'"
                               + ",'" + CurDPTNO + "'"
                               + ")";
                if (SqlHelper.ExecuteNonQuery(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, CommandType.Text, KeyIndexsDptSql) != 1)
                {
                    Response.Write("組織新增失敗！");
                    return;
                }
            }
            */
        }

        protected void MainDBTree_NodeDeleted(object sender, DevExpress.Web.Data.ASPxDataDeletedEventArgs e)
        {
            TreeListNode CurNode = ((ASPxTreeList)sender).FindNodeByKeyValue(e.Keys[0].ToString());
            string ParentDPTNO = CurNode.ParentNode.Key;
            //string ParentDPTNO = e.Values["PARDPTNO"].ToString();
            string CurDPTNO = e.Values["DPTNO"].ToString();
            if (ParentDPTNO != "")
            {
                //Organ刪除
                string KeyIndexsDptSql = "DELETE FROM Organ WHERE "
                               + "DPTNO='" + ParentDPTNO + "' AND "
                               + "SUBDPTNO='" + CurDPTNO + "'";
                if (SqlHelper.ExecuteNonQuery(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, CommandType.Text, KeyIndexsDptSql) != 1)
                {
                    Response.Write("組織刪除部門失敗！");
                    return;
                }
            }
        }

        /*
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
        */

        protected void MainDBTree_NodeUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            ASPxFormLayout floDpt = MainDBTree.FindEditFormTemplateControl("floDpt") as ASPxFormLayout;
            DBUtility.FormLayoutUpdateToValues(floDpt, e, false);
            //e.NewValues["DPTNM"] = floDpt.GetNestedControlValueByFieldName("DPTNM").ToString();
        }

        /*
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
        */

        protected void cbkSearchPanel_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter == "EDBDPTSHIFTMD")
            {
                if (EDBDPTSHIFTMD.SelectedIndex == 0)
                {
                    //運作中部門
                    tbxBSTDISSOLVDT.Visible = false;
                    tbxBEDDISSOLVDT.Visible = false;
                    lblBSTDISSOLVDT.Visible = false;
                    lblDASHDISSOLVDT.Visible = false;
                }
                else if (EDBDPTSHIFTMD.SelectedIndex == 1)
                {
                    //歷年部門
                    tbxBSTDISSOLVDT.Visible = true;
                    tbxBEDDISSOLVDT.Visible = true;
                    tbxBSTDISSOLVDT.Enabled = false;
                    lblBSTDISSOLVDT.Visible = true;
                    lblDASHDISSOLVDT.Visible = true;
                }
                else if (EDBDPTSHIFTMD.SelectedIndex == 2)
                {
                    //已裁撤部門
                    tbxBSTDISSOLVDT.Visible = true;
                    tbxBEDDISSOLVDT.Visible = true;
                    lblBSTDISSOLVDT.Visible = true;
                    lblDASHDISSOLVDT.Visible = true;
                }
            }
            else
            {
                //On Showing
                if (EDBDPTSHIFTMD.SelectedIndex == 0)
                {
                    //運作中部門
                    tbxBSTDISSOLVDT.Visible = false;
                    tbxBEDDISSOLVDT.Visible = false;
                    lblBSTDISSOLVDT.Visible = false;
                    lblDASHDISSOLVDT.Visible = false;
                }
                else if (EDBDPTSHIFTMD.SelectedIndex == 1)
                {
                    //歷年部門
                    tbxBSTDISSOLVDT.Visible = true;
                    tbxBEDDISSOLVDT.Visible = true;
                    tbxBSTDISSOLVDT.Enabled = false;
                    lblBSTDISSOLVDT.Visible = true;
                    lblDASHDISSOLVDT.Visible = true;
                }
                else if (EDBDPTSHIFTMD.SelectedIndex == 2)
                {
                    //已裁撤部門
                    tbxBSTDISSOLVDT.Visible = true;
                    tbxBEDDISSOLVDT.Visible = true;
                    lblBSTDISSOLVDT.Visible = true;
                    lblDASHDISSOLVDT.Visible = true;
                }
            }
        }

        protected void UploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            //HotNews 目錄與執行程式的目錄 不在同一個根目錄之下
            //  執行程式所在目錄 : C:\WebLead\Admin
            //  HotNews 實際目錄 : C:\inetpub\wwwroot\leadfashion
            string resultFilePath = Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + e.UploadedFile.FileName;
            //HotNews 目錄與執行程式的目錄 在同一個根目錄之下
            //  執行程式所在目錄 : C:\inetpub\wwwroot\leadfashion\Admin
            //  HotNews 實際目錄 : C:\inetpub\wwwroot\leadfashion
            //string resultFileUrl = UploadDirectory + e.UploadedFile.FileName;
            //string resultFilePath = MapPath(resultFileUrl);
            /*
            //使用亂數命名
            string resultExtension = Path.GetExtension(e.UploadedFile.FileName);
            string resultFileName = Path.ChangeExtension(Path.GetRandomFileName(), resultExtension);
            string resultFileUrl = UploadDirectory + resultFileName;
            */
            e.UploadedFile.SaveAs(resultFilePath);
            e.CallbackData = e.UploadedFile.FileName;
            //ASPxFormLayout CurfltClsToolByGrp = (MainDBGrid.FindEditFormTemplateControl("fltClsToolByGrp") as ASPxFormLayout);
            //e.CallbackData = resultFileName;
            //((ASPxTextBox)CurfltClsToolByGrp.FindNestedControlByFieldName("LATECAPTMM")).Text = e.UploadedFile.FileName;

            /*
            //UploadingUtils.RemoveFileWithDelay(resultFileName, resultFilePath, 5);

            string name = e.UploadedFile.FileName;
            string url = ResolveClientUrl(resultFileUrl);
            long sizeInKilobytes = e.UploadedFile.ContentLength / 1024;
            string sizeText = sizeInKilobytes.ToString() + " KB";
            e.CallbackData = name + "|" + url + "|" + sizeText;

            //e.CallbackData = SavePostedFile(e.UploadedFile);
             */
        }

        protected void UploadControlTool_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            //使用原始檔名
            string resultFileUrl = UploadDirectory + e.UploadedFile.FileName;
            /*
            //使用亂數命名
            string resultExtension = Path.GetExtension(e.UploadedFile.FileName);
            string resultFileName = Path.ChangeExtension(Path.GetRandomFileName(), resultExtension);
            string resultFileUrl = UploadDirectory + resultFileName;
            */
            string resultFilePath = MapPath(resultFileUrl);
            e.UploadedFile.SaveAs(resultFilePath);
            //ASPxUploadControl uploadControl = sender as ASPxUploadControl;
            //uploadControl.JSProperties["cpUploadFileName"] = e.UploadedFile.FileName;
            //務必設定 FileUploadMode : OnPageLoad , 否則下列的 CurformLayout = null
            //ASPxFormLayout CurfltClsToolByGrp = (ASPxFormLayout)MainDBGrid.FindEditFormTemplateControl("fltClsToolByGrp");
            //((ASPxTextBox)CurfltClsToolByGrp.FindNestedControlByFieldName("TOOLDESC")).Value = e.UploadedFile.FileName;
            //LayoutItem litmTOOLDESC = CurfltClsToolByGrp.FindItemByFieldName("TOOLDESC") as LayoutItem;
            //ASPxEditBase editBase = litmTOOLDESC.GetNestedControl() as ASPxEditBase;
            //editBase.Value = e.UploadedFile.FileName;

            //LayoutItem litmTOOLHLINK = CurfltClsToolByGrp.FindItemOrGroupByName("UploadTool") as LayoutItem;
            /*
             * litmTOOLHLINK.GetNestedControl()只能抓到 ASPxUploadControl ID="UploadControlTool
            //ASPxEditBase editBase = litmTOOLHLINK.GetNestedControl() as ASPxEditBase;
            //editBase.Value = e.UploadedFile.FileName;

            if (litmTOOLHLINK != null)
            {
                (litmTOOLHLINK.GetNestedControl() as ASPxTextBox).Text = e.UploadedFile.FileName;
            }
             */
            //((ASPxTextBox)CurfltClsToolByGrp.FindNestedControlByFieldName("PICTFILENM")).Value = e.UploadedFile.FileName;
            //使用 JSProperties -->  ((ASPxCallbackPanel)fltCCardAcnt.FindControl("cbkPanelCCardAcnt")).JSProperties["cpPopCusFilter"] = CusFilter;
            //從上載目錄移到指定目錄
            string TarFileName = Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + e.UploadedFile.FileName;
            if (resultFilePath != TarFileName)
            {
                //上載目錄resultFilePath在D:\WebLead\WebLead 與 Application["WebHrs-APPDIR"] 不同虛擬目錄
                if (System.IO.File.Exists(TarFileName))
                    System.IO.File.Delete(TarFileName);
                //從上載目錄移到指定目錄
                System.IO.File.Move(resultFilePath, TarFileName);
            }
            e.CallbackData = e.UploadedFile.FileName;
            //e.CallbackData = Application["WebHrs-APPURLBASE"].ToString() + Application["WebHrs-CLSFILEDIR"].ToString() + e.UploadedFile.FileName;;
        }

        public string ToolImageUrl(string CurToolImageKey)
        {
            string CurPRSNNO = CurToolImageKey.Split(';')[0].Trim();
            string CurPHOTOFILENM = CurToolImageKey.Split(';')[1].Trim();
            string ResultUrl = "#";
            if (CurPHOTOFILENM != "")
                ResultUrl = Application["WebSite-APPURLBASE"].ToString() + Application["ORGANPHOTODIR"].ToString() + Application["CPNYID"] + "/Dept" + CurPRSNNO + System.IO.Path.GetExtension(CurPHOTOFILENM);
            return ResultUrl;
        }

        protected void sdsOrgan_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //照片處理
            if (e.Command.Parameters["@PHOTOFILENM"].Value != DBNull.Value)
            //if (sdsOrgan.InsertParameters["PHOTOFILENM"].DefaultValue != null)
            {
                string CurDPTNO = e.Command.Parameters["@DPTNO"].Value.ToString();

                string TarFileName = Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"] + "\\Dept" + CurDPTNO + System.IO.Path.GetExtension(e.Command.Parameters["@PHOTOFILENM"].Value.ToString());
                //string TarFileName = Application["WebSite-APPURLBASE"].ToString() + "\\OrgnPhoto\\" + sdsOrgan.InsertParameters["POSTID"].DefaultValue + "\\" + sdsOrgan.InsertParameters["POSTID"].DefaultValue + "_" + sdsOrgan.InsertParameters["CLSPTNO"].DefaultValue + "_" + sdsOrgan.InsertParameters["CLSSRNO"].DefaultValue + Path.GetExtension(sdsOrgan.InsertParameters["PHOTOFILENM"].DefaultValue);
                //建立課程資料夾
                if (!System.IO.Directory.Exists(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"]))
                    System.IO.Directory.CreateDirectory(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"]);
                //移動到課程資料夾
                if (System.IO.File.Exists(TarFileName))
                    System.IO.File.Delete(TarFileName);
                System.IO.File.Move(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + e.Command.Parameters["@PHOTOFILENM"].Value.ToString(), TarFileName);
                string UpLoadFileExt = Path.GetExtension(sdsOrgan.InsertParameters["PHOTOFILENM"].DefaultValue);
                /*
                if ((UpLoadFileExt == ".zip") || (UpLoadFileExt == ".rar"))
                {
                    //執行解壓縮
                    string DestFile = Application["WebSite-APPDIR"].ToString() + "\\OrgnPhoto\\" + lastInsertedPostID + @"\" + sdsOrgan.InsertParameters["PHOTOFILENM"].DefaultValue;
                    //使用UnRAR.exe須預先建立解壓縮資料夾, 否則無法動作
                    if (!System.IO.Directory.Exists(Application["WebSite-APPDIR"].ToString() + "\\OrgnPhoto\\" + lastInsertedPostID + @"\" + lastInsertedPostID))
                        System.IO.Directory.CreateDirectory(Application["WebSite-APPDIR"].ToString() + "\\OrgnPhoto\\" + lastInsertedPostID + @"\" + lastInsertedPostID);
                    var processStartInfo = new ProcessStartInfo();
                    //processStartInfo.FileName = @"C:\Program Files\7-Zip\7z.exe";
                    //processStartInfo.Arguments = @"e C:\test.7z";
                    processStartInfo.FileName = @"C:\Program Files (x86)\WinRAR\UnRAR.exe";
                    processStartInfo.Arguments = "x  " + DestFile + " " + Application["WebSite-APPDIR"].ToString() + "\\OrgnPhoto\\" + lastInsertedPostID + @"\" + lastInsertedPostID;
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //processStartInfo.Arguments的正確語法 指定檔名與目錄前後無須使用"", 否則無法動作

                    //ShellExecute的正確語法 指定檔名與目錄前後務必使用"", 避免目錄名稱有空格誤認為下一個參數:
                    // DestFile:='C:\Program Files (x86)\WebHrs\Course\T0009\T0009_000_001.rar';
                    //ShellExecute(0,'open','C:\Program Files (x86)\WinRAR\UnRAR.exe',PChar('x  "'+DestFile+'" "'+StringReplace(TBSysvar.FieldByName('APPDIR').AsString+TBSysvar.FieldByName('CLSFILEDIR').AsString+DeltaDS.FieldByName('CLSNO').OldValue+'\'+DeltaDS.FieldByName('CLSNO').OldValue+'_'+DeltaDS.FieldByName('CLSPTNO').OldValue+'_'+DeltaDS.FieldByName('CLSSRNO').OldValue+'"','/','\',[rfReplaceAll])),nil,sw_hide);
                    //Process.Start(processStartInfo);
                    try
                    {
                        // Start the process with the info we specified.
                        using (Process exeProcess = Process.Start(processStartInfo))
                        {
                            exeProcess.WaitForExit();
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(Page, ex.Message);
                    }

                }
                */
            }  //if (sdsOrgan.InsertParameters["PHOTOFILENM"].DefaultValue != null)
        }

        protected void sdsOrgan_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            //從MainDBTree取得舊檔名稱
            object ojtOldPICTFILENM = MainDBTree.FocusedNode["PHOTOFILENM"];
            //string OldPICTFILENM = MainDBTree.GetRowValues(MainDBTree.FocusedRowIndex, "PHOTOFILENM").ToString();
            if ((e.Command.Parameters["@PHOTOFILENM"].Value.ToString() != "") && (System.IO.File.Exists(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + e.Command.Parameters["@PHOTOFILENM"].Value.ToString())))
            {
                string CurDPTNO = e.Command.Parameters["@DPTNO"].Value.ToString();
                if (ojtOldPICTFILENM != null)
                {
                    //先刪除舊檔
                    string OldPICTFILENM = ojtOldPICTFILENM.ToString();
                    string SurFileName = Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"] + "\\Dept" + CurDPTNO + Path.GetExtension(OldPICTFILENM);
                    if (System.IO.Directory.Exists(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"]))
                        //判斷是否已建立原圖片目錄
                        //若產生錯誤訊息 :Callback request failed due to an internal server error, 則尚未建立原圖片目錄
                        System.IO.File.Delete(SurFileName);
                }
                string SourFilename = Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + e.Command.Parameters["@PHOTOFILENM"].Value.ToString();
                string DestFilename = Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"] + "\\Dept"+ CurDPTNO + Path.GetExtension(e.Command.Parameters["@PHOTOFILENM"].Value.ToString());
                //建立圖片或檔案資料夾
                if (!System.IO.Directory.Exists(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"]))
                    System.IO.Directory.CreateDirectory(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"]);
                //
                File.Move(SourFilename, DestFilename);
            }
        }

        protected void sdsOrgan_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {

            if (MainDBTree.FocusedNode["PHOTOFILENM"] != DBNull.Value)
            //if (MainDBTree.GetRowValues(MainDBTree.FocusedRowIndex, "PHOTOFILENM") != DBNull.Value)
            {
                string CurPICTFILENM = MainDBTree.FocusedNode["PHOTOFILENM"].ToString();
                string CurDPTNO = MainDBTree.FocusedNode["DPTNO"].ToString();
                //string CurPICTFILENM = MainDBTree.GetRowValues(MainDBTree.FocusedRowIndex, "PHOTOFILENM").ToString();
                //string CurDPTNO = MainDBTree.GetRowValues(MainDBTree.FocusedRowIndex, "DPTNO").ToString();
                //
                string UpLoadFileExt = Path.GetExtension(CurPICTFILENM);
                if ((UpLoadFileExt == ".zip") || (UpLoadFileExt == ".rar"))
                {
                    string TarFileUnZipFolder = Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + CurDPTNO;
                    string TarFileName = TarFileUnZipFolder + "\\" + CurPICTFILENM;
                    //string TarFileName = Application["WebSite-APPURLBASE"].ToString() + "HotNews/" + CurDPTNO + "\\" + CurDPTNO + "_" + CurCLSPTNO + "_" + CurCLSSRNO + Path.GetExtension(CurPICTFILENM);
                    if ((UpLoadFileExt == ".zip") || (UpLoadFileExt == ".rar"))
                        //刪除課程資料夾內的教材資料夾
                        System.IO.Directory.Delete(TarFileUnZipFolder, true);
                    //刪除課程資料夾內的教材檔
                    System.IO.File.Delete(TarFileName);
                    //刪除課程資料夾(判斷是否空的)
                    string[] GetFiles = Directory.GetFiles(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + CurDPTNO);
                    if (GetFiles.Length == 0)
                        System.IO.Directory.Delete(Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + CurDPTNO);
                }
                else
                {
                    string TarFileName = Application["WebSite-APPDIR"].ToString() + Application["ORGANPHOTODIR"].ToString().Replace("/", "\\") + Application["CPNYID"] + "\\Dept" + CurDPTNO + Path.GetExtension(CurPICTFILENM);
                    //刪除照片檔
                    System.IO.File.Delete(TarFileName);
                }
            }
        }
    }
}