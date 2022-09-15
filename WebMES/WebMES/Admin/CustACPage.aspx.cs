/*
 *  
 * 編輯時在 edMAKDNO_DataBound事件:
 *   ASPxComboBox CurMAKDNO = sender as ASPxComboBox;
 *   Session["MAKDNO"] = CurMAKDNO.Value;
 * 才能讓edMANO下拉顯示該類別的所有項目, edMANO的sdsMas的參數 Ma.MAKDNO=@MAKDNO --> Session["MAKDNO"]
 * 
 * 在 GridView InLine Mode 編輯 
 *  輸入 貨品編號 MANO 可帶出 品名 規格 :
 *    使用 <EditItemTemplate> <dx:ASPxComboBox ID="edMANO"
 *    設定  SelectedIndexChanged事件 執行 cbkPanelMANO.PerformCallback(s.GetValue());
 *  範例如下:  
        <EditItemTemplate>
            <dx:ASPxComboBox ID="edMANO" runat="server" ClientInstanceName="edMANO" DataSourceID="sdsMa" Text='<%#Eval("MANO") %>' TextField="MADESC" TextFormatString="{0}-{1}" Theme="iOS" ValueField="MANO" Width="100%">
                <ClientSideEvents SelectedIndexChanged="function(s, e) {
                    cbkPanelMANO.PerformCallback(s.GetValue());	
                    }" />
            </dx:ASPxComboBox>
        </EditItemTemplate>

 *  在 protected void cbkPanelMANO_Callback(object sender, CallbackEventArgsBase e) 事件, 設定 品名 規格
                                :
                                :
                string CurMANO = e.Parameter;
                ASPxTextBox edMADESC = (ASPxTextBox)CSalDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)CSalDBGrid.Columns[3], "MADESC");
                ASPxTextBox edMASPEC = (ASPxTextBox)CSalDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)CSalDBGrid.Columns[4], "MASPEC");
                string cmd = "SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB, "
                         + "Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST, "
                         + "Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO, Ma.ISINV "
                         + "FROM Ma "
                         + "WHERE Ma.MANO='" + CurMANO + "' ";
                DataTable MaDataTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                if (MaDataTable.Rows.Count == 0)
                {
                    lblBMADESC.Text = "無此品號";
                    return;
                }
                else
                {
                    edMADESC.Text = MaDataTable.Rows[0]["MADESC"].ToString();
                    edMASPEC.Text = MaDataTable.Rows[0]["MASPEC"].ToString();
                }

select cus.cusno,cus.cusnm,cus.ENABLED, ccardacnt.CARDNO, CARDID, INITDT,PASSNO, LOGONID,CSRLNO
from cus inner join ccardacnt on (cus.cusno=ccardacnt.cusno)
where cus.ENABLED is null and INITDT>='2015/01/01' and CARDID='03'

 * 
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
//
using DevExpress.Web;

namespace WebMES.Admin
{
    public partial class CustACPage : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            string CurEditMode = Request.QueryString["EditMode"];
            if (CurEditMode != null)
            {
                MainMenuPanel.Visible = false;
                ManuMenuPanel.Visible = false;
                Session["MainDBNavgMode"] = CurEditMode;
                if (CurEditMode == "0")
                {
                    floCust.FindItemOrGroupByName("lgpSubmit").Visible = false;
                    //floCus.FindItemOrGroupByName("limbtnSave").Visible = false;
                    //floCus.FindItemOrGroupByName("limbtnCancel").Visible = false;
                }
                else if (CurEditMode == "1")
                {
                    edCUSNO.ClientEnabled = false;
                }
                else if (CurEditMode == "2")
                {
                    edCUSNO.ClientEnabled = false;
                }
                else if (CurEditMode == "3")
                {
                    btnSave.Text = "刪除";
                    //LayoutItemBase limbtnSave=floCus.FindItemOrGroupByName("limbtnSave");
                    //LayoutItem limbtnSave=floCus.FindItemOrGroupByName("limbtnSave") as LayoutItem;
                    //ASPxEditBase btnSave = limbtnSave.GetNestedControl() as ASPxEditBase;
                    //btnSave.Caption = "刪除";

                    //floCus.FindItemOrGroupByName("limbtnCancel").Visible = false;
                }
            }
            if (!IsPostBack)
            {
                hfdPassnoChanged.Set("PassnoChanged", false);
                //string CurCUSNO = "06823";
                string CurCUSNO = Request.QueryString["CUSNO"];
                if (CurCUSNO == null)
                {
                    /* 因Response.Redirect->MessageBox.Show 無法作用
                    if ((Session["ActISSHOP"] == null) || (Session["ActISSHOP"].ToString() == "False") || (Session["ActDeptBID"].ToString() == ""))
                    {
                        //登入者隸屬部門若是店家則有對應的Session["ActDeptBID"];
                        //登入者隸屬部門若是服務處或管理處則無對應的Session["ActDeptBID"];
                        //檢查登入者隸屬部門是否隸屬開立帳單店家或已選擇開立帳單店家";
                        MessageBox.Show(Page, "請重新選擇左上角的帳單店家 !!");
                        Response.Redirect("~/Admin/CSalBill.aspx");
                    }
                    */

                    //新增
                    floCust.DataSourceID = null;
                    //sdsCus.SelectParameters["CUSNO"].DefaultValue = null;
                    //抓編碼最大值
                    CurCUSNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Cus", "CUSNO", "Cus.CUSNO<'99999'", true, "#####", Convert.ToDateTime(null), "");
                    //CurCUSNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Cus", "CUSNO", "", true, "##########", Convert.ToDateTime(null), "");
                    //設定控制項初始值
                    edCUSNO.Value = CurCUSNO;
                    //floCus.FindItemByFieldName("CUSNO").
                    //edLOGDT.Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                    edDPTNO.Value = (Session["ActDeptID"] == null ? Session["CurDeptID"].ToString() : Session["ActDeptID"].ToString());

                    /*
                    //設定欄位初始值
                    sdsCus.InsertParameters["CUSNO"].DefaultValue = CurCUSNO;
                    sdsCus.InsertParameters["LOGDT"].DefaultValue = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                    sdsCus.InsertParameters["DPTNO"].DefaultValue = Session["ActDeptID"].ToString();
                    if (Session["ActDeptID"].ToString() == Session["ActDeptID"].ToString())
                        sdsCus.InsertParameters["LOGID"].DefaultValue = Session["CurUserID"].ToString();
                    sdsCus.Insert();
                     */
                    Session["CUSNO"] = CurCUSNO;
                }
                else
                {
                    //更改 S01201602030001
                    Session["CUSNO"] = CurCUSNO;
                    Session["DPTNO"] = edDPTNO.Value;
                }
                sdsCus.SelectParameters["CUSNO"].DefaultValue = CurCUSNO;
            }
        }

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
            if (!IsPostBack)
            {
                //CSalDBGrid_CustomCallback(sender, null);
            }
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["DPTNO"] = edDPTNO.Value;
                edPRSNNO.DataBind();
            }
            //在Page.Load Page.LoadComplete Page.PreRender事件 , edHBLOCKNO.Value = null
            //SurveyItemsSet();
        }

        protected void floCus_DataBound(object sender, EventArgs e)
        {
            Session["CUSNO"] = floCust.GetNestedControlValueByFieldName("CUSNO");
            //Session["CUSNO"] = floCus.GetNestedControlValueByFieldName("CUSNO").ToString();
            //清除上次輸入內容
            /*
            if (floCus.DataSource == null || ((DataTable)floCus.DataSource).Rows.Count == 0)
            {
                floCus.ForEach(ClearItem);
            }
            */

        }

        protected void sdsCus_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //sdsCus.InsertParameters["CUSNO"].DefaultValue = Session["CUSNO"].ToString();
            //sdsCus.InsertParameters["LOGDT"].DefaultValue = edLOGDT.Value.ToString();
            //sdsCus.InsertParameters["CUSNO"].DefaultValue = tbxCUSNO.Value.ToString();
            //sdsCus.InsertParameters["CARDNO"].DefaultValue = tbxCARDNO.Value.ToString();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //更改
            DBUtility.FormLayoutMapToData(floCust, sdsCus, false);
            sdsCus.Update(); //Uncomment this line to allow updating.
            /*
            floCus.ForEach(ProcessLayoutItem);
            //sdsCus.UpdateParameters["CUSNO"].DefaultValue = Session["CUSNO"].ToString();
            sdsCus.Update(); //Uncomment this line to allow updating.
             */
            /*
            if (MainDBGrid.IsNewRowEditing)
            {
                //新增
                DBUtility.FormLayoutMapToData(floCus, sdsCus, true);
                sdsCus.Insert(); //Uncomment this line to allow updating.
            }
            else
            {
                //更改
                DBUtility.FormLayoutMapToData(floCus, sdsCus, false);
                sdsCus.Update(); //Uncomment this line to allow updating.
            }
            */
        }

        protected void edMANO_Callback(object sender, CallbackEventArgsBase e)
        {
            FilledMANOCombo(e.Parameter);
        }

        protected void FilledMANOCombo(string CurMAKDNO)
        {
            if (string.IsNullOrEmpty(CurMAKDNO))
                return;
            Session["MAKDNO"] = CurMAKDNO;
            ASPxFormLayout CurfltCusd = CSalDBGrid.FindEditFormTemplateControl("fltCusd") as ASPxFormLayout;
            ((ASPxComboBox)CurfltCusd.FindControl("edMANO")).DataBind();
        }

        protected void sdsCusSal_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            /*
            string CurCUSNO = e.Command.Parameters["@CUSNO"].Value.ToString();
            string CurSHFTITNO = e.Command.Parameters["@SHFTITNO"].Value.ToString();
            DateTime CurLOGDT = (DateTime)e.Command.Parameters["@LOGDT"].Value;
            //string CurSALSTNO = e.Command.Parameters["@SALSTNO"].Value.ToString();
            string CurMANO = e.Command.Parameters["@MANO"].Value.ToString();
            double CurSHFTQTY = Convert.ToDouble(e.Command.Parameters["@SHFTQTY"].Value);
            double CurSHFTCST = Convert.ToDouble(e.Command.Parameters["@SALCST"].Value);
            string CurSTORID = e.Command.Parameters["@STORID"].Value.ToString();
            object OldISINV = Session["ISINV"];
            if ((OldISINV != null) && (Convert.ToBoolean(OldISINV)))
            //object OldISINV = CSalDBGrid.GetRowValues(CSalDBGrid.FocusedRowIndex, "ISINV");
            //if ((OldISINV != null) && (Convert.ToBoolean(CSalDBGrid.GetRowValues(CSalDBGrid.FocusedRowIndex, "ISINV"))))
            {
                //同步庫存
                CSal_Publ.SyncMaInv(CurMANO, "", CurSHFTQTY, CurSHFTCST, "2", CurSTORID, CSal_Publ.GetWorkYYMM(CurCUSNO, CurLOGDT, "2"));
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
                //original_SHFTQTY 在 btnSave_Click 事件 CSalDBGrid.GetRowValues(CSalDBGrid.FocusedRowIndex, "SHFTQTY") 設定
                if (e.Command.Parameters["@SHFTQTY"].Value != e.Command.Parameters["@original_SHFTQTY"].Value)
                {
                    double CurSHFTQTY = Convert.ToDouble(e.Command.Parameters["@SHFTQTY"].Value);
                    double OldSHFTQTY = Convert.ToDouble(e.Command.Parameters["@original_SHFTQTY"].Value);
                    string CurCUSNO = e.Command.Parameters["@CUSNO"].ToString();
                    DateTime CurLOGDT = (DateTime)(e.Command.Parameters["@LOGDT"].Value);
                    string CurMANO = e.Command.Parameters["@MANO"].ToString();
                    string CurSTORID = e.Command.Parameters["@STORID"].ToString();
                    double CurSHFTCST = Convert.ToDouble(e.Command.Parameters["@SALCST"]);
                    double OldSHFTCST = Convert.ToDouble(e.Command.Parameters["@original_SALCST"]);
                    CSal_Publ.SyncMaInv(CurMANO, "", CurSHFTQTY, CurSHFTCST, "2", CurSTORID, CSal_Publ.GetWorkYYMM(CurCUSNO, CurLOGDT, "2"));
                    CSal_Publ.SyncMaInv(CurMANO, "", -OldSHFTQTY, -OldSHFTCST, "2", CurSTORID, CSal_Publ.GetWorkYYMM(CurCUSNO, CurLOGDT, "2"));
                }
            }
             */
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = "";
            string CurEditMode = Request.QueryString["EditMode"];
            if (CurEditMode == "1")
            //if ((((ASPxTextBox)fltCCardAcnt.FindControl("tbxCUSNO")).Value == null) && (((ASPxTextBox)fltCCardAcnt.FindControl("tbxCUSNM")).Value != null))
            {
                //新增會員資料
                /*
                 * 登入帳號/登入密碼 允許空白
                if (edLOGONID.Value == null)
                {
                    lblErrorMessage.Text = "登入帳號請勿空白!!";
                    edLOGONID.Focus();
                    return;
                }
                if (edPASSNO.Value == null)
                {
                    lblErrorMessage.Text = "登入密碼請勿空白!!";
                    edPASSNO.Focus();
                    return;
                }
                */
                //檢查客戶資料是否重複-->客戶名稱+行動電話/聯絡電話不能重複
                string WhereSetStrCus = "";
                if (edCUSNM.Value == null)
                {
                    lblErrorMessage.Text = "客戶名稱請勿空白 !!";
                    edCUSNM.Focus();
                    return;
                }
                else
                {
                    if (WhereSetStrCus == "")
                        WhereSetStrCus += " Cus.CUSNM='" + edCUSNM.Value.ToString() + "' ";
                    else
                        WhereSetStrCus += " AND Cus.CUSNM='" + edCUSNM.Value.ToString() + "' ";
                }

                if ((edCNTMOVTEL.Value == null) && (edCNTTELNO.Value == null))
                {
                    lblErrorMessage.Text = "行動電話/聯絡電話請勿同時空白 !!";
                    if (edCNTMOVTEL.Value == null)
                        edCNTMOVTEL.Focus();
                    else
                        edCNTTELNO.Focus();
                    return;
                }
                else
                {
                    if (edCNTMOVTEL.Value != null)
                    {
                        if (WhereSetStrCus == "")
                            WhereSetStrCus += " Cus.CNTMOVTEL='" + edCNTMOVTEL.Value.ToString() + "' ";
                        else
                            WhereSetStrCus += " AND Cus.CNTMOVTEL='" + edCNTMOVTEL.Value.ToString() + "' ";
                    }
                    else
                    {
                        if (WhereSetStrCus == "")
                            WhereSetStrCus += " Cus.CNTTELNO='" + edCNTTELNO.Value.ToString() + "' ";
                        else
                            WhereSetStrCus += " AND Cus.CNTTELNO='" + edCNTTELNO.Value.ToString() + "' ";
                    }
                }
                /*
                if ((edCUSNM.Value == null) && (edIDNO.Value == null) && (edCNTMOVTEL.Value == null))
                {
                    lblErrorMessage.Text = "客戶名稱/身證字號/行動電話請勿同時空白 !!";
                    edCUSNM.Focus();
                    return;
                }
                */
                string cmdSelectCus = "SELECT Cus.CUSNM, Cus.CNTMOVTEL, Cus.CNTTELNO, Cus.CUSNO  "
                     + "FROM Cus "
                     + "WHERE " + WhereSetStrCus;
                DataTable dtbCus = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdSelectCus);
                if (dtbCus.Rows.Count > 0)
                {
                    lblErrorMessage.Text = "已有此筆客戶資料, 勿重複新增 !!";
                    edCUSNM.Focus();
                    return;
                }

                DBUtility.FormLayoutMapToData(floCust, sdsCus, true);
                if (((ASPxTextBox)floCust.FindControl("edCUSNO")).Value == null)
                    sdsCus.InsertParameters["CUSNO"].DefaultValue = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Cus", "CUSNO", "Cus.CUSNO<'99999'", true, "#####", Convert.ToDateTime(null), "");
                if (edPASSNO.Value != null)
                    sdsCus.InsertParameters["PASSNO"].DefaultValue = DBUtility.EnPcode(edPASSNO.Value.ToString());
                sdsCus.InsertParameters["ENABLED"].DefaultValue = Convert.ToString(true);
                sdsCus.InsertParameters["DPTNO"].DefaultValue = Session["ActDeptID"] == null ? Session["CurDeptID"].ToString() : Session["ActDeptID"].ToString();
                sdsCus.Insert();
            }
            else if (CurEditMode == "2")
            {
                //檢查客戶資料是否重複-->客戶名稱+行動電話/聯絡電話不能重複
                string WhereSetStrCus = "Cus.CUSNO<>'"+edCUSNO.Value+"' ";
                if (edCUSNM.Value == null)
                {
                    lblErrorMessage.Text = "客戶名稱請勿空白 !!";
                    edCUSNM.Focus();
                    return;
                }
                else
                {
                    if (WhereSetStrCus == "")
                        WhereSetStrCus += " Cus.CUSNM='" + edCUSNM.Value.ToString() + "' ";
                    else
                        WhereSetStrCus += " AND Cus.CUSNM='" + edCUSNM.Value.ToString() + "' ";
                }

                if ((edCNTMOVTEL.Value == null) && (edCNTTELNO.Value == null))
                {
                    lblErrorMessage.Text = "行動電話/聯絡電話請勿同時空白 !!";
                    if (edCNTMOVTEL.Value == null)
                        edCNTMOVTEL.Focus();
                    else
                        edCNTTELNO.Focus();
                    return;
                }
                else
                {
                    if (edCNTMOVTEL.Value != null)
                    {
                        if (WhereSetStrCus == "")
                            WhereSetStrCus += " Cus.CNTMOVTEL='" + edCNTMOVTEL.Value.ToString() + "' ";
                        else
                            WhereSetStrCus += " AND Cus.CNTMOVTEL='" + edCNTMOVTEL.Value.ToString() + "' ";
                    }
                    else
                    {
                        if (WhereSetStrCus == "")
                            WhereSetStrCus += " Cus.CNTTELNO='" + edCNTTELNO.Value.ToString() + "' ";
                        else
                            WhereSetStrCus += " AND Cus.CNTTELNO='" + edCNTTELNO.Value.ToString() + "' ";
                    }
                }
                string cmdSelectCus = "SELECT Cus.CUSNM, Cus.CNTMOVTEL, Cus.CNTTELNO, Cus.CUSNO  "
                     + "FROM Cus "
                     + "WHERE " + WhereSetStrCus;
                DataTable dtbCus = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdSelectCus);
                if (dtbCus.Rows.Count > 0)
                {
                    lblErrorMessage.Text = "已有此筆客戶資料, 勿重複新增 !!";
                    edCUSNM.Focus();
                    return;
                }

                //更新會員資料
                DBUtility.FormLayoutMapToData(floCust, sdsCus, false);
                bool ISPassnoChanged = (bool)hfdPassnoChanged.Get("PassnoChanged");
                if (ISPassnoChanged)
                {
                    if (edPASSNO.Value == null)
                    {
                        lblErrorMessage.Text = "登入密碼請勿空白!!";
                        edPASSNO.Focus();
                        return;
                    }
                    sdsCus.UpdateParameters["PASSNO"].DefaultValue = DBUtility.EnPcode(edPASSNO.Value.ToString());
                }
                else
                    sdsCus.UpdateParameters["PASSNO"].DefaultValue = (string)hfdPassnoChanged.Get("OldPASSNO");
                sdsCus.Update();
                //hfdPassnoChanged.Set("PassnoChanged", false);
            }
            else if (CurEditMode == "3")
            {
                //刪除會員資料
                string cmd = "SELECT Sal.SALNO "
                        + "FROM Sal "
                        + "WHERE Sal.CUSNO='" + Request.QueryString["CUSNO"] + "' ";
                DataTable dtbSalForCus = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                if (dtbSalForCus.Rows.Count > 0)
                {
                    lblErrorMessage.Text = "已有消費紀錄, 無法刪除";
                    btnCancel.Focus();
                    return;
                }
                sdsCus.DeleteParameters["CUSNO"].DefaultValue = Request.QueryString["CUSNO"];
                sdsCus.Delete();
            }
            string startUpScript = "window.parent.MainTabPageToPageBrow(" + CurEditMode + ");";
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", startUpScript, true);
            /*
             * 注意 : <dx:ASPxButton btnCancel若在<asp:UpdatePanel ....內, RegisterStartupScript(...無法啟動
             */
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string CurEditMode = Request.QueryString["EditMode"];
            if (CurEditMode == null)
                Server.Transfer(VirtualPathUtility.ToAbsolute("~/Admin/CustAC.aspx"));
            else
            {
                //From CustAC
                string startUpScript = "window.parent.MainTabPageToPageBrow(0);";
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", startUpScript, true);
                /*
                 * 注意 : <dx:ASPxButton btnCancel若在<asp:UpdatePanel ....內, RegisterStartupScript(...無法啟動
                 */
            }
        }

        protected void sdsCusSal_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            string CurCUSNO = e.Command.Parameters["@CUSNO"].Value.ToString();
            string CurSHFTITNO = e.Command.Parameters["@SHFTITNO"].Value.ToString();
            string cmdCusd = "SELECT Cusd.CUSNO, Cusd.SHFTITNO, Cusd.LOGDT,  "
                                 + "Cusd.MANO, Ma.ISINV, Cusd.STORID, Cusd.BTCHNO, Cusd.SHFTQTY, Cusd.LTUTPRIC, Cusd.OFFRATE, Cusd.OFUTPRIC, Cusd.SLUTPRIC, "
                                 + "Cusd.STUNITNM, Cusd.ST2RTEXGRATE, Cusd.SALAMT, Cusd.UNITCOST, Cusd.SALCST, Cusd.DUEAMT, Cusd.HPRSNNO, Cusd.SHAREAMT "
                                 + "FROM Sald LEFT JOIN Ma ON (Cusd.SALMDID=Ma.MACDNO AND Cusd.MANO=Ma.MANO) "
                                 + "WHERE Cusd.CUSNO='" + CurCUSNO + "' "
                                 + "AND  Cusd.SHFTITNO='" + CurSHFTITNO + "' ";
            DataTable dtbCusd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdCusd);
            if (dtbCusd.Rows.Count > 0)
            {
                DateTime CurLOGDT = (DateTime)e.Command.Parameters["@LOGDT"].Value;
                //string CurSALSTNO = e.Command.Parameters["@SALSTNO"].Value.ToString();
                string CurMANO = e.Command.Parameters["@MANO"].Value.ToString();
                double CurSHFTQTY = Convert.ToDouble(e.Command.Parameters["@SHFTQTY"].Value);
                double CurSHFTCST = Convert.ToDouble(e.Command.Parameters["@SALCST"].Value);
                string CurSTORID = e.Command.Parameters["@STORID"].Value.ToString();
                //object OldISINV = Session["ISINV"];
                //if ((OldISINV != null) && (Convert.ToBoolean(OldISINV)))
                object OldISINV = CSalDBGrid.GetRowValues(CSalDBGrid.FocusedRowIndex, "ISINV");
                /*
                if ((OldISINV != null) && (Convert.ToBoolean(CSalDBGrid.GetRowValues(CSalDBGrid.FocusedRowIndex, "ISINV"))))
                {
                    //同步庫存
                    CSal_Publ.SyncMaInv(CurMANO, "", CurSHFTQTY, CurSHFTCST, "1", CurSTORID, CSal_Publ.GetWorkYYMM(CurCUSNO, CurLOGDT, "2"));
                }
                */
            }
        }

        protected void ShareDBGrid_BeforePerformDataSelect(object sender, EventArgs e)
        {
            string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            Session["CUSNO"] = MasterKeyValue.Split('|')[0].Trim();
            Session["SHFTITNO"] = MasterKeyValue.Split('|')[1].Trim();

            //根據權限代碼動態設定增刪修按鍵的顯示否
            int index = CSalDBGrid.FindVisibleIndexByKeyValue(MasterKeyValue);
            string FormID = this.Form.ID;
            string ParentHeadStr;
            ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
            DBUtility.CmdbtnSetRight(ParentHeadStr, ((CSalDBGrid.FindDetailRowTemplateControl(index, "ShareDBGrid") as ASPxGridView).Columns[0] as DevExpress.Web.GridViewCommandColumn), 3);

        }


        protected void CSalDBGrid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            if (CSalDBGrid.IsEditing)
            {
                if (e.Column.FieldName == "MANO")
                {
                    /*
                    object val = null;
                    if (e.KeyValue == DBNull.Value || e.KeyValue == null)
                        //新增時
                        val = Session["CurDeptID"];
                    else
                        val = CSalDBGrid.GetRowValuesByKeyValue(e.KeyValue, "DPTNO");
                    if (val == DBNull.Value)
                        return;
                    string DPTNO = (string)val;
                     */
                    //ASPxComboBox combo = e.Editor as ASPxComboBox;
                    //combo.Callback += new CallbackEventHandlerBase(edMANO_OnCallback);
                }
            }
        }

        protected void cbkfloCustNewData()
        {
            //抓編碼最大值
            string CurCUSNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Cus", "CUSNO", "Cus.CUSNO<'99999'", true, "#####", Convert.ToDateTime(null), "");
            Session["CUSNO"] = CurCUSNO;
            floCust.DataBind();
            //設定控制項初始值
            edCUSNO.Value = CurCUSNO;
            edDPTNO.Value = Session["ActDeptID"].ToString();
            if (edDPTNO.Value != null)
            {
                Session["DPTNO"] = edDPTNO.Value;
                edPRSNNO.DataBind();
            }
        }

        protected void cbkfloCust_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter == "NavgInsert")
            {
                //新增
                Session["MainDBNavgMode"] = 1;
                cbkfloCustNewData();

                //清空ASPxFormLayout原有的輸入值
                //DBUtility.FormLayoutClearData(floCust);
                //sdsCus.SelectParameters["CUSNO"].DefaultValue = null;
                //抓編碼最大值
                //string CurCUSNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Rec", "CUSNO", "", true, "yyyymmdd####", DateTime.Today, Session["ActDeptBID"].ToString());
                //Session["CUSNO"] = CurCUSNO;
                //floCust.DataBind();
                //設定控制項初始值
                //floCust.FindItemByFieldName("CUSNO").
                //edRECDT.Value = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                //設定欄位初始值
                //對應到floCust--使用者可修改RECDT CUSNO
                /*
                edCUSNO.Value = sdsCus.InsertParameters["CUSNO"].DefaultValue;
                edRECDT.Value = sdsCus.InsertParameters["RECDT"].DefaultValue;
                edDPTNO.Value = sdsCus.InsertParameters["DPTNO"].DefaultValue;
                edSHFTSTATNO.Value = sdsCus.InsertParameters["SHFTSTATNO"].DefaultValue;
                edPPRSNNO.Value = sdsCus.InsertParameters["PPRSNNO"].DefaultValue;
                 */
                //直接存入--使用者無法修改RECDT CUSNO
                /*
                sdsCus.Insert();
                Session["CUSNO"] = CurCUSNO;
                if (sdsCus.SelectCommand.IndexOf("WHERE ") >= 0)
                    sdsCus.SelectCommand = sdsCus.SelectCommand.Substring(0, sdsCus.SelectCommand.IndexOf("WHERE "));
                sdsCus.SelectCommand += " WHERE " + "CUSNO='" + CurCUSNO + "'";
                sdsCus.SelectCommand += " ORDER BY Rec.CUSNO";
                sdsCus.SelectCommandType = SqlDataSourceCommandType.Text;
                floCust.DataBind();
                 */
            }
            else if (e.Parameter == "NavgEdit")
            {
                //更改
                /*
                Session["MainDBNavgMode"] = 2;
                object ojtCUSNO = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "CUSNO");
                Session["CUSNO"] = ojtCUSNO.ToString();
                floCust.DataBind();
                */
            }
            else if (e.Parameter == "NavgView")
            {
                //調閱
                /*
                object ojtCUSNO = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "CUSNO");
                if (ojtCUSNO != null)
                {
                    Session["CUSNO"] = ojtCUSNO;
                    floCust.DataBind();
                }
                */
            }
            else if (e.Parameter == "SyncfloCust")
            {
                floCust.DataBind();
            }
            else if (e.Parameter == "edDPTNO")
            {
                //先儲存主檔紀錄
                /*
                if ((int)Session["MainDBNavgMode"] == 1)
                {
                    //新增
                    DBUtility.FormLayoutMapToData(floCust, sdsCus, true);
                    sdsCus.Insert(); //Uncomment this line to allow updating.
                    //Session["CUSNO"] = sdsCus.InsertParameters["CUSNO"].DefaultValue;
                    Session["MainDBNavgMode"] = 0;
                }
                */
                //未先儲存主檔紀錄, floCust內的元件執行CallBack後, 畫面資料會消失為空白
                Session["DPTNO"] = edDPTNO.Value;
                edPRSNNO.DataBind();
            }

        }

        protected void edPASSNO_DataBound(object sender, EventArgs e)
        {
            if (edPASSNO.Value == null)
                hfdPassnoChanged.Set("OldPASSNO", null);
            else
                hfdPassnoChanged.Set("OldPASSNO", edPASSNO.Value.ToString());
        }

    }
}