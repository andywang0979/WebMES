/*
ASPxComboBox in the EditForm template gives the "Databinding methods such as Eval(), XPath(), and Bind() can only be used in the context of a databound control" error

To solve this issue, try the following recommendations:
- Disable the ViewState for a container control (i.e. ASPxGridView, ASPxPageControl, asp:FormView);
- Call the container's DataBind method in the page/container's Init event handler;
- If you call the DataBind method for ASPxComboBox nested in ASPxGridView, replace it with the ASPxComboBox.DataBindItems method call;
-- In case of other editors, do not call the DataBind method at all - ASPxGridView will bind the editors automatically.
- If you are using ASPxGridView, disable its caching mechanism via the ASPxGridView.EnableRowsCache property. In case you bind it to a datasource at runtime, data-binding should be performed in the Page_Init event handler.

設定下列, 避免產生 Databinding methods such as Eval(), XPath(), and Bind() can only be used....  error
    SubDBGrid
      EnableRowsCache="False"--> 即可避免Databinding.....  error
      EnableViewState="False" --> EditForm 的ComboBox在編輯下拉 CallBack時, 螢幕較不易閃爍 

VRecAC.aspx搭配VRecACPage.aspx
  在VRecAC.aspx瀏覽頁任一筆按Edit時或在SubDBGrid增刪修後, 可在此事件判斷 SubDBGrid.VisibleRowCount == 0
        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
                       :
                       :
            if (SubDBGrid.VisibleRowCount == 0)
            {
                edRECDT.ClientEnabled = true;
                edMANO.ClientEnabled = true;
                edDPTNO.ClientEnabled = true;
            }
            else
            {
                edRECDT.ClientEnabled = false;
                edMANO.ClientEnabled = false;
                edDPTNO.ClientEnabled = false;
            }
        }

注意!!
ASPxTextBox若使用Mask, 因預留validate空間, 顯示寬度會縮小一半, 若要避免這種狀況, 可設定ValidationSettings.Display="Dynamic"即可
範例如下:
Since the mask is always validated, the editor reserves a place for the error image.
You can avoid this behavior using the ValidationSettings.Display property. Please set it to Dynamic.
    <dx:ASPxTextBox ID="edSPREPHMANTICK" runat="server" Text='<%#Eval("SPREPHMANTICK") %>' OnDataBound="edSPREPHMANTICK_DataBound">
        <MaskSettings Mask="99.99:99:99" />
        <ValidationSettings Display="Dynamic">
        </ValidationSettings>
    </dx:ASPxTextBox>



SELECT MaRoutProc.MANO, MaRoutProc.MROUTID, MaRoutProc.MROUTSRNO, MaRoutProc.MPROCID, ManuProc.MPROCNM, ManuProc.MPROCDESC, ManuProc.MLINEID, ManuLine.MLINENM, MaRoutProc.UNITCOST,
 MaRoutProc.SPREPHMANTICK, MaRoutProc.SPREPMACHTICK, MaRoutProc.SBTCHHMANTICK, MaRoutProc.SBTCHMACHTICK, MaRoutProc.SPREPHMANTM, MaRoutProc.SPREPMACHTM, MaRoutProc.SBTCHHMANTM, MaRoutProc.SBTCHMACHTM,
 MaRoutProc.PROCFMULAOPT, MaRoutProc.PUTNINPTFORMAT, MaRoutProc.PNPARA1, MaRoutProc.PNMATHOPTR, MaRoutProc.PNPARA2, MaRoutProc.PUTNQTY, MaRoutProc.PNUNITNM, MaRoutProc.MATHOPTR, MaRoutProc.PN2RTEXGRATE,
 MaRoutProc.PHPARA1, MaRoutProc.PHMATHOPTR, MaRoutProc.PHPARA2, MaRoutProc.PFNHQTY, MaRoutProc.FNUNITNM, MaRoutProc.FNSHSPEC, MaRoutProc.MPROCMD, MaRoutProc.FNSHVALDMD, MaRoutProc.FNSHTLRNRATE 
FROM MaRoutProc LEFT OUTER JOIN ManuProc ON MaRoutProc.MPROCID = ManuProc.MPROCID 
 LEFT OUTER JOIN ManuLine ON ManuProc.MLINEID = ManuLine.MLINEID 
WHERE (MaRoutProc.MANO = @MANO) AND (MaRoutProc.MROUTID = @MROUTID) 
ORDER BY MaRoutProc.MROUTSRNO

 FNSHVALDMD 完成數量檢查模式
   0: 完成>=預設產出數量 (檢查產出數量是否>=完成數量 --> 按比例換算)
   1: 完成>=投入數量 (檢查產出數量是否>=投入數量 --> 跳過比例換算)


SELECT  MaRoutProc.MANO, MaRoutProc.MROUTID, MaRoutProc.MROUTSRNO, MaRoutProc.MPROCID, MaRoutProc.PFNHQTY, MaRoutProc.FNUNITNM, MaRout.MROUTNM
FROM      MaRoutProc LEFT JOIN MaRout ON (MaRoutProc.MANO=MaRout.MANO AND MaRoutProc.MROUTID=MaRout.MROUTID)
WHERE MPROCID='00003' AND MaRoutProc.FNUNITNM IS NOT NULL

UPDATE  MaRoutProc
SET 
MaRoutProc.FNUNITNM='捲'
FROM  MaRoutProc LEFT JOIN MaRout ON (MaRoutProc.MANO=MaRout.MANO AND MaRoutProc.MROUTID=MaRout.MROUTID)
WHERE MaRout.MROUTNM='包裝' AND MPROCID='00003' AND MaRoutProc.FNUNITNM='箱'


SELECT  MaRoutProc.MANO, MaRoutProc.MROUTID, MaRoutProc.MROUTSRNO, MaRoutProc.MPROCID, MaRout.MROUTNM, MaRoutProc.FNSHVALDMD, MaRoutProc.FNSHTLRNRATE
FROM      MaRoutProc LEFT JOIN MaRout ON (MaRoutProc.MANO=MaRout.MANO AND MaRoutProc.MROUTID=MaRout.MROUTID)
WHERE MPROCID='00001' AND MaRoutProc.FNSHVALDMD=1

*/

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//
using System.Web.Configuration;
using System.Data;
//SqlHelper
using CPC.Utility.SQL;
//AjaxHelper
//
using DevExpress.Web;

namespace WebMES.Admin
{
    public partial class MaRoutPage : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            //設定Grid中文訊息、主題、字型、權限...
            DBUtility.InitGridSetting(SubDBGrid, 2);
            string CurEditMode = Request.QueryString["EditMode"];
            if (CurEditMode != null)
            {
                MainMenuPanel.Visible = false;
                if (CurEditMode == "0")
                {
                    floMaRout.FindItemOrGroupByName("lgpSubmit").Visible = false;
                    //floCus.FindItemOrGroupByName("limbtnSave").Visible = false;
                    //floCus.FindItemOrGroupByName("limbtnCancel").Visible = false;
                    (SubDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowNewButtonInHeader = false;
                    (SubDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowEditButton = false;
                    (SubDBGrid.Columns[0] as DevExpress.Web.GridViewCommandColumn).ShowDeleteButton = false;
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
            /*
            if ((bool)Session["ISINVBTCH"])
            {
                //ISINVBTCH = true 管制批號庫存
                SubDBGrid.Columns["BTCHNO"].Visible = true;
                SubDBGrid.Columns["MANUDT"].Visible = true;
            }
            if ((bool)Session["RECAUTOPAS"])
                //自動驗收
                SubDBGrid.Columns["RECQTY"].Visible = false;
            if ((bool)Session["ISINVMULSTOR"])
                //ISINVMULSTOR = true 多倉制
                SubDBGrid.Columns["STORID"].Visible = true;
            */
            if (!IsPostBack)
            {
                if (CurEditMode != null)
                {
                    Session["MainDBNavgMode"] = Convert.ToInt32(CurEditMode);
                    //string CurMANO = "920500000000001";
                    string CurMANO = Request.QueryString["MANO"];
                    if (CurMANO == null)
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
                        floMaRout.DataSourceID = null;
                        cbkfloMaRoutNewData(System.DateTime.Today);

                        /*
                        //設定欄位初始值
                        sdsMaRout.InsertParameters["RECNO"].DefaultValue = CurMANO;
                        sdsMaRout.InsertParameters["RECDT"].DefaultValue = System.DateTime.Today.ToString("yyyy/MM/dd");
                        //sdsMaRout.InsertParameters["RECDT"].DefaultValue = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                        sdsMaRout.InsertParameters["DPTNO"].DefaultValue = Session["ActDeptID"].ToString();
                        //if (Session["ActDeptID"].ToString() == Session["ActDeptID"].ToString())
                        //    sdsMaRout.InsertParameters["PRSNNO"].DefaultValue = Session["CurUserID"].ToString();
                        sdsMaRout.Insert();
                        */
                    }
                    else
                    {
                        //更改 S01201602030001
                        Session["MANO"] = CurMANO;
                        string CurMROUTID = Request.QueryString["MROUTID"];
                        Session["MROUTID"] = CurMROUTID;

                    }

                    //sdsMaRout.SelectParameters["RECNO"].DefaultValue = CurMANO;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //cvwMa.DataSource = sdsMa;
            //cvwMa.DataBind();
            //BuildMakdMenu(mnuMakd,sdsMakd);
            //cvwMa.FilterExpression = "[MAKDNO]='1'";
            if (!IsPostBack)
            {
                //SubDBGrid_CustomCallback(sender, null);
            }
        }

        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                //在Load事件無法抓取CrmMenuPanel.cbxDptCusToNow.SelectedIndexChanged事件內的Session["CRMCUSNO"] 設定值
                //在LoadComplete事件才能抓取CrmMenuPanel.cbxDptCusToNow.SelectedIndexChanged事件內的Session["CRMCUSNO"] 設定值
                //設定協助服務: sdsPrsnInPstn的參數
                //根據人事異動檔(DptPP)抓取該店擔當員工
                //(DptPP.EXPIRDT IS NULL OR DptPP.EXPIRDT>帳單日期)
            }
            /*
            if (edVENNO.Value != null)
            {
                Session["VENNO"] = edVENNO.Value.ToString();
            }
            //在VRecAC.aspx的瀏覽頁任一筆按Edit時, 可在此事件判斷 SubDBGrid.VisibleRowCount == 0
            //在SubDBGrid增刪修後, 可在此事件判斷 SubDBGrid.VisibleRowCount == 0
            if (SubDBGrid.VisibleRowCount == 0)
            {
                edRECDT.ClientEnabled = true;
                edMANO.ClientEnabled = true;
                edDPTNO.ClientEnabled = true;
            }
            else
            {
                edRECDT.ClientEnabled = false;
                edMANO.ClientEnabled = false;
                edDPTNO.ClientEnabled = false;
            }
            */
        }

        /*
        protected void cbkPanelMANO_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter == "tbxBMANO")
            {
                //在cbkPanelMANO_Callback事件的新增消費項目, SubDBGrid.DataBind() 無法同步更新;
                //請改用 SubDBGrid_CustomCallback事件進行新增消費項目, SubDBGrid.DataBind() 才能同步更新
                //放在ASPxCallbackPanel的 SubDBGrid.務必搭配 EnableCallBacks="False" 
                if ((tbxBMANO.Value != null) && (tbxBMANO.Value.ToString() != ""))
                {
                    if (InsertVRecdFromBarcode(tbxBMANO.Value.ToString(), 0, edBPURNO.Value == null ? null : edBPURNO.Value.ToString()))
                        //objMaQty=0, 會跳回不作新增, 同時設定 tbxBMAQTY.Value = "1"
                        tbxBMAQTY.Focus();
                    else
                    {
                        tbxBMANO.Value = "";
                        tbxBMANO.Focus();
                    }

                }  //if ((tbxBMANO.Value != null) && (tbxBMANO.Value.ToString() != ""))
            }
            else if (e.Parameter == "btnBCodeConfirm")
            {
                if ((tbxBMANO.Value != null) && (tbxBMANO.Value.ToString() != "") && (tbxBMAQTY.Value != null) && (tbxBMAQTY.Value.ToString() != ""))
                {
                    InsertVRecdFromBarcode(tbxBMANO.Value.ToString(), tbxBMAQTY.Value);
                    tbxBMANO.Focus();
                }

            }
            else if (e.Parameter == "edSHFTQTY")
            {
                //檢查庫存
                //轉到 SubDBGrid_RowValidating 事件檢查
            }
            else if ((e.Parameter.Length > 8) && (e.Parameter.Substring(0, 8) == "edMAKDNO"))
            {
                Session["MAKDNO"] = e.Parameter.Split('|')[1].Trim();
                //ASPxComboBox edMANO = (ASPxComboBox)SubDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)SubDBGrid.Columns["MANO"], "edMANO");
                //edMANO.DataBindItems();
                ASPxCallbackPanel CurcbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                ASPxFormLayout CurfltMaRoutProc = CurcbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                ((ASPxComboBox)CurfltMaRoutProc.FindControl("edPURITNO")).DataBindItems();
                ((ASPxComboBox)CurfltMaRoutProc.FindControl("edMANO")).DataBindItems();
            }
            else if ((e.Parameter.Length > 7) && (e.Parameter.Substring(0, 7) == "edPURNO"))
            {
                //SubDBGrid的 edPURNO
                Session["PURNO"] = e.Parameter.Split('|')[1].Trim();
                ASPxCallbackPanel CurcbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                ASPxFormLayout CurfltMaRoutProc = CurcbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                ((ASPxComboBox)CurfltMaRoutProc.FindControl("edPURITNO")).DataBindItems();
                //未執行下列edMANO.DataBindItems()會產生
                //  System.InvalidOperationException: 'Databinding methods such as Eval(), XPath(), and Bind() can only be used in the context of a databound control
                ((ASPxComboBox)CurfltMaRoutProc.FindControl("edMANO")).DataBindItems();
            }
            else if ((e.Parameter.Length > 9) && (e.Parameter.Substring(0, 9) == "edPURITNO"))
            {
                //SubDBGrid的 edPURITNO
                string CurPURNO = e.Parameter.Split('|')[2].Trim();
                string CurPURITNO = e.Parameter.Split('|')[1].Trim();

                string cmd = "SELECT Purd.MANO, Purd.PRUNITNM, Purd.PR2RTEXGRATE, Purd.PRUTPRIC, Purd.PURQTY, Purd.PASQTY, "
                         + "Ma.MADESC, Ma.MASPEC, Ma.ISINV "
                         + "FROM Purd LEFT JOIN Ma ON (Purd.MANO=Ma.MANO) "
                         + "WHERE Purd.PURNO='" + CurPURNO + "' AND Purd.PURITNO='" + CurPURITNO + "' ";
                DataTable MaDataTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                if (MaDataTable.Rows.Count == 0)
                {
                    lblBMADESC.Text = "無此訂購單";
                    return;
                }
                else
                {
                    //使用EditForm
                    ASPxCallbackPanel CurcbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                    ASPxFormLayout fltMaRoutProc = CurcbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                    //ASPxFormLayout fltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("fltMaRoutProc") as ASPxFormLayout;
                    ASPxComboBox edMANO = ((ASPxComboBox)fltMaRoutProc.FindControl("edMANO"));
                    ASPxTextBox edMADESC = ((ASPxTextBox)fltMaRoutProc.FindControl("edMADESC"));
                    ASPxTextBox edMASPEC = ((ASPxTextBox)fltMaRoutProc.FindControl("edMASPEC"));
                    ASPxComboBox edRCUNITNM = ((ASPxComboBox)fltMaRoutProc.FindControl("edRCUNITNM"));
                    ASPxTextBox edRC2RTEXGRATE = ((ASPxTextBox)fltMaRoutProc.FindControl("edRC2RTEXGRATE"));
                    ASPxCheckBox edISINV = ((ASPxCheckBox)fltMaRoutProc.FindControl("edISINV"));
                    ASPxTextBox edRCUTPRIC = ((ASPxTextBox)fltMaRoutProc.FindControl("edRCUTPRIC"));
                    ASPxTextBox edPASQTY = ((ASPxTextBox)fltMaRoutProc.FindControl("edPASQTY"));
                    ASPxTextBox edPASAMT = ((ASPxTextBox)fltMaRoutProc.FindControl("edPASAMT"));

                    edMANO.Text = MaDataTable.Rows[0]["MANO"].ToString();
                    edMADESC.Text = MaDataTable.Rows[0]["MADESC"].ToString();
                    edMASPEC.Text = MaDataTable.Rows[0]["MASPEC"].ToString();
                    edRCUNITNM.Text = MaDataTable.Rows[0]["PRUNITNM"].ToString();
                    edRC2RTEXGRATE.Text = MaDataTable.Rows[0]["PR2RTEXGRATE"].ToString();
                    edISINV.Value = (bool)MaDataTable.Rows[0]["ISINV"];
                    edRCUTPRIC.Value = MaDataTable.Rows[0]["PRUTPRIC"].ToString();
                    int CurPASQTY = Convert.ToInt16(MaDataTable.Rows[0]["PURQTY"]) - Convert.ToInt16(MaDataTable.Rows[0]["PASQTY"]);
                    double CurPASAMT = CurPASQTY * Convert.ToInt16(MaDataTable.Rows[0]["PRUTPRIC"]);
                    edPASQTY.Value = CurPASQTY.ToString();
                    edPASAMT.Value = CurPASAMT.ToString();

                    //未執行下列edMANO.DataBindItems()會產生
                    //  System.InvalidOperationException: 'Databinding methods such as Eval(), XPath(), and Bind() can only be used in the context of a databound control
                    ((ASPxComboBox)fltMaRoutProc.FindControl("edMANO")).DataBindItems();
                }
            }
            else if ((e.Parameter.Length > 6) && (e.Parameter.Substring(0, 6) == "edMANO"))
            {
                string CurMANO = e.Parameter.Split('|')[1].Trim();
                //string CurMANO = e.Parameter;
                //object EditingRowVRecd = SubDBGrid.GetDataRow(SubDBGrid.EditingRowVisibleIndex);
                //ASPxTextBox edMADESC = (ASPxTextBox)SubDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)SubDBGrid.Columns["MADESC"], "edMADESC");
                //ASPxTextBox edMASPEC = (ASPxTextBox)SubDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)SubDBGrid.Columns["MASPEC"], "edMASPEC");
                //ASPxCheckBox edISINV = (ASPxCheckBox)SubDBGrid.FindEditRowCellTemplateControl((GridViewDataCheckColumn)SubDBGrid.Columns["ISINV"], "edISINV");
                DataTable MaDataTable;
                //貨品價格處理順序: 1.先找尋該貨品在訂購單最後一次訂價 2.廠商報價 3.貨品基本資料(成本單價)
                //先找尋該貨品在訂購單最後一次訂價
                string cmdPurd = "SELECT TOP 1 Purd.PRUTPRIC, Purd.PRUNITNM ,Purd.PR2RTEXGRATE, "
                         + "Ma.MADESC, Ma.MASPEC, Ma.ISINV "
                         + "FROM Pur INNER JOIN Purd ON (Pur.PURNO=Purd.PURNO) "
                         + " LEFT JOIN Ma ON (Purd.MANO=Ma.MANO) "
                         + "WHERE Pur.VENNO='" + edVENNO.Value.ToString() + "' AND Purd.MANO='" + CurMANO + "' "
                         + "ORDER BY Purd.ETA DESC ";
                MaDataTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdPurd);
                if (MaDataTable.Rows.Count == 0)
                {
                    //廠商報價
                    string cmdVend = "SELECT Vend.PKUNITNM, Vend.PK2RTEXGRATE, Vend.PKUTPRIC, "
                             + "Ma.MADESC, Ma.MASPEC, Ma.ISINV "
                             + "FROM Vend LEFT JOIN Ma ON (Vend.MANO=Ma.MANO) "
                             + "WHERE Vend.VENNO='" + edVENNO.Value.ToString() + "' AND Vend.MANO='" + CurMANO + "' ";
                    MaDataTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdVend);
                    if (MaDataTable.Rows.Count == 0)
                    {
                        //貨品基本資料
                        string cmd = "SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB, "
                                 + "Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST, "
                                 + "Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO, Ma.ISINV "
                                 + "FROM Ma "
                                 + "WHERE Ma.MANO='" + CurMANO + "' ";
                        MaDataTable = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                        if (MaDataTable.Rows.Count == 0)
                        {
                            lblBMADESC.Text = "無此品號";
                            return;
                        }
                        else
                        {
                            //使用EditForm
                            ASPxCallbackPanel CurcbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                            ASPxFormLayout fltMaRoutProc = CurcbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                            //ASPxFormLayout fltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("fltMaRoutProc") as ASPxFormLayout;
                            ASPxTextBox edMADESC = ((ASPxTextBox)fltMaRoutProc.FindControl("edMADESC"));
                            ASPxTextBox edMASPEC = ((ASPxTextBox)fltMaRoutProc.FindControl("edMASPEC"));
                            ASPxComboBox edRCUNITNM = ((ASPxComboBox)fltMaRoutProc.FindControl("edRCUNITNM"));
                            ASPxTextBox edRC2RTEXGRATE = ((ASPxTextBox)fltMaRoutProc.FindControl("edRC2RTEXGRATE"));
                            ASPxCheckBox edISINV = ((ASPxCheckBox)fltMaRoutProc.FindControl("edISINV"));
                            ASPxTextBox edRCUTPRIC = ((ASPxTextBox)fltMaRoutProc.FindControl("edRCUTPRIC"));

                            edMADESC.Text = MaDataTable.Rows[0]["MADESC"].ToString();
                            edMASPEC.Text = MaDataTable.Rows[0]["MASPEC"].ToString();
                            edRCUNITNM.Text = MaDataTable.Rows[0]["PKUNITNM"].ToString();
                            edRC2RTEXGRATE.Text = MaDataTable.Rows[0]["PK2RTEXGRATE"].ToString();
                            edISINV.Value = (bool)MaDataTable.Rows[0]["ISINV"];
                            edRCUTPRIC.Value = MaDataTable.Rows[0]["UNITCOST"].ToString();
                            //設定 UNITCOST 計算成本
                            //((DataRow)EditingRowMaShftd).ISINV = (bool)MaDataTable.Rows[0]["ISINV"];
                            //未執行下列edMANO.DataBindItems()會產生
                            //  System.InvalidOperationException: 'Databinding methods such as Eval(), XPath(), and Bind() can only be used in the context of a databound control
                            ASPxComboBox edMANO = ((ASPxComboBox)fltMaRoutProc.FindControl("edMANO"));
                            edMANO.DataBindItems();
                            ((ASPxComboBox)fltMaRoutProc.FindControl("edPURITNO")).DataBindItems();
                        }
                    }
                }
                else
                {
                    //先貨品在訂購單最後一次訂價
                    //使用EditForm
                    ASPxCallbackPanel CurcbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                    ASPxFormLayout fltMaRoutProc = CurcbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                    //ASPxFormLayout fltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("fltMaRoutProc") as ASPxFormLayout;
                    ASPxTextBox edMADESC = ((ASPxTextBox)fltMaRoutProc.FindControl("edMADESC"));
                    ASPxTextBox edMASPEC = ((ASPxTextBox)fltMaRoutProc.FindControl("edMASPEC"));
                    ASPxComboBox edRCUNITNM = ((ASPxComboBox)fltMaRoutProc.FindControl("edRCUNITNM"));
                    ASPxTextBox edRC2RTEXGRATE = ((ASPxTextBox)fltMaRoutProc.FindControl("edRC2RTEXGRATE"));
                    ASPxCheckBox edISINV = ((ASPxCheckBox)fltMaRoutProc.FindControl("edISINV"));
                    ASPxTextBox edRCUTPRIC = ((ASPxTextBox)fltMaRoutProc.FindControl("edRCUTPRIC"));

                    edMADESC.Text = MaDataTable.Rows[0]["MADESC"].ToString();
                    edMASPEC.Text = MaDataTable.Rows[0]["MASPEC"].ToString();
                    edRCUNITNM.Text = MaDataTable.Rows[0]["PRUNITNM"].ToString();
                    edRC2RTEXGRATE.Text = MaDataTable.Rows[0]["PR2RTEXGRATE"].ToString();
                    edISINV.Value = (bool)MaDataTable.Rows[0]["ISINV"];
                    edRCUTPRIC.Value = MaDataTable.Rows[0]["PRUTPRIC"].ToString();
                    //設定 UNITCOST 計算成本
                    //((DataRow)EditingRowMaShftd).ISINV = (bool)MaDataTable.Rows[0]["ISINV"];
                    //未執行下列edMANO.DataBindItems()會產生
                    //  System.InvalidOperationException: 'Databinding methods such as Eval(), XPath(), and Bind() can only be used in the context of a databound control
                    ASPxComboBox edMANO = ((ASPxComboBox)fltMaRoutProc.FindControl("edMANO"));
                    edMANO.DataBindItems();

                }
            }
        }
        */

        protected void sdsMaRout_Inserting(object sender, SqlDataSourceCommandEventArgs e)
        {
            //sdsMaRout.InsertParameters["RECNO"].DefaultValue = Session["MANO"].ToString();
            //sdsMaRout.InsertParameters["RECDT"].DefaultValue = edRECDT.Value.ToString();
            //sdsMaRout.InsertParameters["CUSNO"].DefaultValue = tbxCUSNO.Value.ToString();
            //sdsMaRout.InsertParameters["CARDNO"].DefaultValue = tbxCARDNO.Value.ToString();
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
            ASPxFormLayout CurfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("fltMaRoutProc") as ASPxFormLayout;
            ((ASPxComboBox)CurfltMaRoutProc.FindControl("edMANO")).DataBindItems();
        }

        protected void btnItemSave_Click(object sender, EventArgs e)
        {
            if (SubDBGrid.IsNewRowEditing)
            {
                //新增
                ASPxCallbackPanel CurcbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                ASPxFormLayout fltMaRoutProc = CurcbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                //
                ASPxLabel lblItemErrorMessage = ((ASPxLabel)fltMaRoutProc.FindControl("lblItemErrorMessage"));
                lblItemErrorMessage.Text = "";
                /*
                ASPxComboBox edPURNO = (ASPxComboBox)fltMaRoutProc.FindControl("edPURNO");
                ASPxComboBox edPURITNO = (ASPxComboBox)fltMaRoutProc.FindControl("edPURITNO");
                if ((edPURNO.Value != null) && (edPURITNO.Value != null))
                {
                    //檢查訂購數量
                    string cmdVPurd = "SELECT Purd.PURNO, Purd.PURITNO, Purd.MANO, Purd.PRUNITNM, Purd.PR2RTEXGRATE, Purd.ETA, Purd.PRUTPRIC, Purd.PURQTY, Purd.PURAMT, Purd.PASQTY "
                                        + "FROM Purd "
                                        + "WHERE Purd.PURNO='" + edPURNO.Value + "' AND Purd.PURITNO = '" + edPURITNO.Value + "' ";
                    DataTable dtbVPurd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdVPurd);
                    if (dtbVPurd.Rows.Count >= 1)
                    {
                        ASPxTextBox edPASQTY = ((ASPxTextBox)fltMaRoutProc.FindControl("edPASQTY"));
                            if (Convert.ToDouble(dtbVPurd.Rows[0]["PASQTY"]) + Convert.ToDouble(edPASQTY.Value) > Convert.ToDouble(dtbVPurd.Rows[0]["PURQTY"]))
                        {
                            lblItemErrorMessage.Text = "驗收數量超過訂購數量 !!";
                            ((ASPxComboBox)fltMaRoutProc.FindControl("edPURITNO")).DataBindItems();
                            ((ASPxComboBox)fltMaRoutProc.FindControl("edMANO")).DataBindItems();
                            return;
                        }
                    }
                }
                */
                DBUtility.FormLayoutMapToData(fltMaRoutProc, sdsMaRoutProc, true);
                sdsMaRoutProc.InsertParameters["SPREPHMANTICK"].DefaultValue = "0";
                sdsMaRoutProc.InsertParameters["SPREPMACHTICK"].DefaultValue = "0";
                sdsMaRoutProc.InsertParameters["SBTCHHMANTICK"].DefaultValue = "0";
                sdsMaRoutProc.InsertParameters["SBTCHMACHTICK"].DefaultValue = "0";
                sdsMaRoutProc.InsertParameters["MANO"].DefaultValue = edMANO.Value.ToString();
                sdsMaRoutProc.InsertParameters["MROUTID"].DefaultValue = edMROUTID.Value.ToString();
                sdsMaRoutProc.Insert();
            }
            else
            {
                //更改
                ASPxCallbackPanel CurcbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                ASPxFormLayout fltMaRoutProc = CurcbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                ASPxLabel lblItemErrorMessage = ((ASPxLabel)fltMaRoutProc.FindControl("lblItemErrorMessage"));
                lblItemErrorMessage.Text = "";
                /*
                ASPxComboBox edPURNO = (ASPxComboBox)fltMaRoutProc.FindControl("edPURNO");
                ASPxComboBox edPURITNO = (ASPxComboBox)fltMaRoutProc.FindControl("edPURITNO");
                if ((edPURNO.Value != null) && (edPURITNO.Value != null))
                {
                    //檢查訂購數量
                    object ojtOldPASQTY = SubDBGrid.GetRowValues(SubDBGrid.FocusedRowIndex, "PASQTY");
                    object ojtPURDPURQTY = SubDBGrid.GetRowValues(SubDBGrid.FocusedRowIndex, "PURDPURQTY");
                    object ojtPURDPASQTY = SubDBGrid.GetRowValues(SubDBGrid.FocusedRowIndex, "PURDPASQTY");
                    ASPxTextBox edPASQTY = ((ASPxTextBox)fltMaRoutProc.FindControl("edPASQTY"));
                    if (Convert.ToDouble(ojtPURDPASQTY) + Convert.ToDouble(edPASQTY.Value) - Convert.ToDouble(ojtOldPASQTY) > Convert.ToDouble(ojtPURDPURQTY))
                    {
                        lblItemErrorMessage.Text = "驗收數量超過訂購數量 !!";
                        ((ASPxComboBox)fltMaRoutProc.FindControl("edPURITNO")).DataBindItems();
                        ((ASPxComboBox)fltMaRoutProc.FindControl("edMANO")).DataBindItems();
                        return;
                    }
                }
                */
                DBUtility.FormLayoutMapToData(fltMaRoutProc, sdsMaRoutProc, false);
                //ASPxFormLayout內無RECNO的對應元件, 須以下列取得RECNO值才能執行更新
                sdsMaRoutProc.UpdateParameters["MANO"].DefaultValue = edMANO.Value.ToString();
                sdsMaRoutProc.UpdateParameters["MROUTID"].DefaultValue = edMROUTID.Value.ToString();
                //sdsMaRoutProc.UpdateParameters["MANO"].DefaultValue = SubDBGrid.GetRowValues(SubDBGrid.FocusedRowIndex, "MANO").ToString();
                ASPxTextBox edSPREPHMANTICK = ((ASPxTextBox)fltMaRoutProc.FindControl("edSPREPHMANTICK"));
                if (edSPREPHMANTICK.Value != null)
                {
                    //固定8位數, MaskSettings Mask="99.99:99:99" --> 日:時:分:秒
                    //空值 : 00000000
                    //sdsMaRoutProc.UpdateParameters["SPREPHMANTICK"].DefaultValue = DBUtility.dhmsText2Tick(edSPREPHMANTICK.Value).ToString();//SY
                    /*
                    string strDays = edSPREPHMANTICK.Value.ToString().Substring(0, 2);
                    string strHours = edSPREPHMANTICK.Value.ToString().Substring(2, 2);
                    string strMins = edSPREPHMANTICK.Value.ToString().Substring(4, 2);
                    string strSecs = edSPREPHMANTICK.Value.ToString().Substring(6, 2);
                    TimeSpan tsSPREPHMAN = new TimeSpan(Convert.ToInt32(strDays), Convert.ToInt32(strHours), Convert.ToInt32(strMins), Convert.ToInt32(strSecs));
                    sdsMaRoutProc.UpdateParameters["SPREPHMANTICK"].DefaultValue = tsSPREPHMAN.Ticks.ToString();
                    */
                }
                ASPxTextBox edSPREPMACHTICK = ((ASPxTextBox)fltMaRoutProc.FindControl("edSPREPMACHTICK"));
                if (edSPREPMACHTICK.Value != null)
                {
                    //固定8位數, MaskSettings Mask="99.99:99:99" --> 日:時:分:秒
                    //空值 : 00000000
                    //sdsMaRoutProc.UpdateParameters["SPREPMACHTICK"].DefaultValue = DBUtility.dhmsText2Tick(edSPREPMACHTICK.Value).ToString();//SY
                    /*
                    string strDays = edSPREPMACHTICK.Value.ToString().Substring(0, 2);
                    string strHours = edSPREPMACHTICK.Value.ToString().Substring(3, 2);
                    string strMins = edSPREPMACHTICK.Value.ToString().Substring(6, 2);
                    string strSecs = edSPREPMACHTICK.Value.ToString().Substring(9, 2);
                    TimeSpan tsSPREPMACH = new TimeSpan(Convert.ToInt32(strDays), Convert.ToInt32(strHours), Convert.ToInt32(strMins), Convert.ToInt32(strSecs));
                    sdsMaRoutProc.UpdateParameters["SPREPMACHTICK"].DefaultValue = tsSPREPMACH.Ticks.ToString();
                    */
                }
                ASPxTextBox edSBTCHHMANTICK = ((ASPxTextBox)fltMaRoutProc.FindControl("edSBTCHHMANTICK"));
                if (edSBTCHHMANTICK.Value != null)
                {
                    //固定8位數, MaskSettings Mask="99.99:99:99" --> 日:時:分:秒
                    //空值 : 00000000
                    //sdsMaRoutProc.UpdateParameters["SBTCHHMANTICK"].DefaultValue = DBUtility.dhmsText2Tick(edSBTCHHMANTICK.Value).ToString();//SY
                    /*
                    string strDays = edSBTCHHMANTICK.Value.ToString().Substring(0, 2);
                    string strHours = edSBTCHHMANTICK.Value.ToString().Substring(3, 2);
                    string strMins = edSBTCHHMANTICK.Value.ToString().Substring(6, 2);
                    string strSecs = edSBTCHHMANTICK.Value.ToString().Substring(9, 2);
                    TimeSpan tsSBTCHHMAN = new TimeSpan(Convert.ToInt32(strDays), Convert.ToInt32(strHours), Convert.ToInt32(strMins), Convert.ToInt32(strSecs));
                    sdsMaRoutProc.UpdateParameters["SBTCHHMANTICK"].DefaultValue = tsSBTCHHMAN.Ticks.ToString();
                    */
                }
                ASPxTextBox edSBTCHMACHTICK = ((ASPxTextBox)fltMaRoutProc.FindControl("edSBTCHMACHTICK"));
                if (edSBTCHMACHTICK.Value != null)
                {
                    //固定8位數, MaskSettings Mask="99.99:99:99" --> 日:時:分:秒
                    //空值 : 00000000
                    //sdsMaRoutProc.UpdateParameters["SBTCHMACHTICK"].DefaultValue = DBUtility.dhmsText2Tick(edSBTCHMACHTICK.Value).ToString();//SY
                    /*
                    string strDays = edSBTCHMACHTICK.Value.ToString().Substring(0, 2);
                    string strHours = edSBTCHMACHTICK.Value.ToString().Substring(3, 2);
                    string strMins = edSBTCHMACHTICK.Value.ToString().Substring(6, 2);
                    string strSecs = edSBTCHMACHTICK.Value.ToString().Substring(9, 2);
                    TimeSpan tsSBTCHMACH = new TimeSpan(Convert.ToInt32(strDays), Convert.ToInt32(strHours), Convert.ToInt32(strMins), Convert.ToInt32(strSecs));
                    sdsMaRoutProc.UpdateParameters["SBTCHMACHTICK"].DefaultValue = tsSBTCHMACH.Ticks.ToString();
                    */
                }
                sdsMaRoutProc.Update();
            }
            SubDBGrid.CancelEdit();
        }

        protected void btnItemCancel_Click(object sender, EventArgs e)
        {
            /*
            ASPxCallbackPanel CurcbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
            ASPxFormLayout fltMaRoutProc = CurcbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
            ASPxLabel lblItemErrorMessage = ((ASPxLabel)fltMaRoutProc.FindControl("lblItemErrorMessage"));
            lblItemErrorMessage.Text = "";
            */
                    SubDBGrid.CancelEdit();

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = "";
            string CurEditMode = Request.QueryString["EditMode"];
            if (CurEditMode == "1")
            {
                /*
                //新增會員資料
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
                DBUtility.FormLayoutMapToData(floMaRout, sdsMaRout, true);
                //if (((ASPxTextBox)floMaRout.FindControl("edCUSNO")).Value == null)
                //    sdsMaRout.InsertParameters["CUSNO"].DefaultValue = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Cus", "CUSNO", "Cus.CUSNO<'99999'", true, "#####", Convert.ToDateTime(null), "");
                //sdsMaRout.InsertParameters["PASSNO"].DefaultValue = DBUtility.EnPcode(edPASSNO.Value.ToString());
                //sdsMaRout.InsertParameters["ENABLED"].DefaultValue = Convert.ToString(true);
                //sdsMaRout.InsertParameters["DPTNO"].DefaultValue = Session["ActDeptID"] == null ? Session["CurDeptID"].ToString() : Session["ActDeptID"].ToString();
                sdsMaRout.InsertParameters["TSPREPHMANTICK"].DefaultValue = "0";
                sdsMaRout.InsertParameters["TSPREPMACHTICK"].DefaultValue = "0";
                sdsMaRout.InsertParameters["TSBTCHHMANTICK"].DefaultValue = "0";
                sdsMaRout.InsertParameters["TSBTCHMACHTICK"].DefaultValue = "0";
                sdsMaRout.Insert();
            }
            else if (CurEditMode == "2")
            {
                //更新會員資料
                DBUtility.FormLayoutMapToData(floMaRout, sdsMaRout, false);
                /*
                bool ISPassnoChanged = (bool)hfdPassnoChanged.Get("PassnoChanged");
                if (ISPassnoChanged)
                {
                    if (edPASSNO.Value == null)
                    {
                        lblErrorMessage.Text = "登入密碼請勿空白!!";
                        edPASSNO.Focus();
                        return;
                    }
                    sdsMaRout.UpdateParameters["PASSNO"].DefaultValue = DBUtility.EnPcode(edPASSNO.Value.ToString());
                }
                else
                    sdsMaRout.UpdateParameters["PASSNO"].DefaultValue = (string)hfdPassnoChanged.Get("OldPASSNO");
                */
                sdsMaRout.Update();
                //hfdPassnoChanged.Set("PassnoChanged", false);
            }
            else if (CurEditMode == "3")
            {
                //檢查工令
                string cmd = "SELECT Wo.WONO "
                        + "FROM Wo "
                        + "WHERE Wo.PMANO='" + edMANO.Value.ToString() + "' AND Wo.MROUTID='" + edMROUTID.Value.ToString() + "' ";
                DataTable dtbWo = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                if (dtbWo.Rows.Count > 0)
                {
                    lblErrorMessage.Text = "工令已使用此製途, 無法刪除";
                    btnCancel.Focus();
                    return;
                }
                sdsMaRout.DeleteParameters["MANO"].DefaultValue = edMANO.Value.ToString();
                sdsMaRout.DeleteParameters["MROUTID"].DefaultValue = edMROUTID.Value.ToString();
                sdsMaRout.Delete();
            }
            string startUpScript = "window.parent.HidepclEditPagePanel('btnSave');";
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
                string startUpScript = "window.parent.HidepclEditPagePanel('btnCancel');";
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY", startUpScript, true);
                /*
                 * 注意 : <dx:ASPxButton btnCancel若在<asp:UpdatePanel ....內, RegisterStartupScript(...無法啟動
                 */
            }
        }

        protected void fltMaRoutProc_DataBound(object sender, EventArgs e)
        {
            /*
            ASPxFormLayout fltMaRoutProc = sender as ASPxFormLayout;
            fltMaRoutProc.FindItemByFieldName("PRSNNO").ClientVisible = false;
            fltMaRoutProc.FindItemByFieldName("WONO").ClientVisible = true;
            fltMaRoutProc.FindItemByFieldName("WOITNO").ClientVisible = true;
            if (Session["BSHFTKDNO"].ToString() == "4")
            {
                //員購
                fltMaRoutProc.FindItemByFieldName("PRSNNO").ClientVisible = true;
                fltMaRoutProc.FindItemByFieldName("WONO").ClientVisible = false;
                fltMaRoutProc.FindItemByFieldName("WOITNO").ClientVisible = false;
            }
            else if (Session["BSHFTKDNO"].ToString() == "6")
            {
                //轉出
                //抓取轉入部門
                fltMaRoutProc.FindItemByFieldName("WONO").ClientVisible = false;
                fltMaRoutProc.FindItemByFieldName("WOITNO").ClientVisible = false;
            }
            else if (Session["BSHFTKDNO"].ToString() == "7")
            {
                //轉入
                //僅允許透過轉入單號匯入
                fltMaRoutProc.FindItemByFieldName("WONO").ClientVisible = false;
                fltMaRoutProc.FindItemByFieldName("WOITNO").ClientVisible = false;
            }
            */
        }

        protected void SubDBGrid_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            /*
            if (edVENNO.Value == null)
            {
                lblErrorMessage.Text = "廠商名稱請勿空白!!";
                edVENNO.Focus();
                return;
            }
            */
            if ((int)Session["MainDBNavgMode"] == 1)
            {
                //新增
                DBUtility.FormLayoutMapToData(floMaRout, sdsMaRout, true);
                sdsMaRout.Insert(); //Uncomment this line to allow updating.
                //Session["MANO"] = sdsMaRout.InsertParameters["RECNO"].DefaultValue;
                Session["MainDBNavgMode"] = 0;
            }

            //抓編碼最大值
            string NewMROUTSRNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "MaRoutProc", "MROUTSRNO", "MANO='" + edMANO.Value + "' AND MROUTID='" + edMROUTID.Value + "'", true, "###", Convert.ToDateTime(null), "");
            //string NewRECITNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Recd", "RECITNO", "RECNO='" + edMANO.Value + "'", true, "###", Convert.ToDateTime(null), "");
            //string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            //e.NewValues["MANO"] = MasterKeyValue.Split('|')[0].Trim();
            e.NewValues["MANO"] = edMANO.Value;
            e.NewValues["MROUTID"] = edMROUTID.Value;
            e.NewValues["MROUTSRNO"] = NewMROUTSRNO;
        }

        protected void SubDBGrid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {

            if (SubDBGrid.IsNewRowEditing)
            {
                //新增時
                if ((e.Column.FieldName == "MADESC") || (e.Column.FieldName == "MASPEC") || (e.Column.FieldName == "ISINV"))
                    e.Editor.ClientEnabled = false;
            }
            else
            {
                if ((e.Column.FieldName == "MADESC") || (e.Column.FieldName == "MASPEC") || (e.Column.FieldName == "ISINV"))
                {
                    //更改時不可變動
                    e.Editor.ClientEnabled = false;
                }
                if ((e.Column.FieldName == "MANO") || (e.Column.FieldName == "PURNO") || (e.Column.FieldName == "PURITNO"))
                    //更改時不可變動
                    e.Editor.ClientEnabled = false;
            }
            //
            /*
            if (e.Column.FieldName == "PURITNO")
            {
                var combo = (ASPxComboBox)e.Editor;
                combo.Callback += new CallbackEventHandlerBase(cbxPURITNO_Callback);
                //
                var grid = e.Column.Grid;
                if (!combo.IsCallback)
                {
                    string PURNO = "";
                    if (!grid.IsNewRowEditing)
                        PURNO = grid.GetRowValues(e.VisibleIndex, "PURNO").ToString();
                    FillcbxPURITNO(combo, PURNO);
                }
            }
             */
        }

        protected void SubDBGrid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            e.NewValues["MANO"] = edMANO.Value.ToString();
            //若Columns["MANO"] 設定 EditItemTemplate Control時, 在 Save時e.NewValues["MANO"]無法自動對應Columns["MANO"]的輸入值
            //取出 EditItemTemplate Control 輸入值 對應到e.NewValues["MANO"]
            //ASPxComboBox edMANO = (ASPxComboBox)SubDBGrid.FindEditRowCellTemplateControl(SubDBGrid.Columns["MANO"] as GridViewDataColumn, "edMANO");
            //e.NewValues["MANO"] = edMANO.Value;
            //ASPxCheckBox edISINV = (ASPxCheckBox)SubDBGrid.FindEditRowCellTemplateControl(SubDBGrid.Columns["ISINV"] as GridViewDataCheckColumn, "edISINV");
            //e.NewValues["ISINV"] = edISINV.Value;
            //sdsMaRoutProc.InsertParameters["RECNO"].DefaultValue = edMANO.Value.ToString();
            //e.NewValues["UNITCOST"] = ShftUNITCOST;
        }

        protected void SubDBGrid_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            //取得該料號的即時庫存Ma.INVQTY & 是否控制庫存Ma.ISINV 
            //ASPxComboBox edMANO = (ASPxComboBox)SubDBGrid.FindEditRowCellTemplateControl(SubDBGrid.Columns["MANO"] as GridViewDataColumn, "edMANO");
            bool CDVRecdISINVValue = false;
            double CDVRecdINVQTYOldValue = 0;
            string cmd = "SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB, "
                     + "Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST, "
                     + "Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO, Ma.ISINV "
                     + "FROM Ma "
                     + "WHERE Ma.MANO='" + (e.IsNewRow ? e.NewValues["MANO"] : e.OldValues["MANO"]) + "' ";
            DataTable dtbVRecd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
            if (dtbVRecd.Rows.Count > 0)
            {
                CDVRecdINVQTYOldValue = (double)dtbVRecd.Rows[0]["INVQTY"];
                CDVRecdISINVValue = (bool)dtbVRecd.Rows[0]["ISINV"];
                //ShftUNITCOST = (double)dtbVRecd.Rows[0]["UNITCOST"];
            }
            if ((CDVRecdISINVValue))
            {
                //INVCANMINUS: 庫存允許負值
                //CDVRecdSHFTKDNO.AsString<>'9' 盤存單不檢查庫存量
                //檢查庫存量
                //INVQTY = 該料號存量 (Ma=>Mano)
                //if 多倉制 then
                //  INVQTY = 該料號的指定倉庫的存量 (MaStor=>Storid)
                //if 批號庫存控制 then
                //  INVQTY = 該批號存量 (MaBtch=>Btchno)
                //

                if (e.IsNewRow)
                {
                    //ASPxGridView.RowUpdating - e.NewValues does not contain the value of the column with EditItemTemplate
                    /*
                    ASPxGridView gv = sender as ASPxGridView;
                    GridViewDataColumn column = gv.Columns["MANO"] as GridViewDataColumn;
                    ASPxComboBox edMANO = (ASPxComboBox)SubDBGrid.FindEditRowCellTemplateControl(column, "edMANO");
                     */
                    //ASPxTextBox edSHFTQTY = (ASPxTextBox)SubDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)SubDBGrid.Columns[10], "edSHFTQTY");
                    /*
                    //取得即時庫存
                    double CDVRecdINVQTYOldValue = 0;
                    string cmd = "SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB, "
                             + "Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST, "
                             + "Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO, Ma.ISINV "
                             + "FROM Ma "
                             + "WHERE Ma.MANO='" + edMANO.Value + "' ";
                    DataTable dtbVRecd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                    if (dtbVRecd.Rows.Count > 0)
                        CDVRecdINVQTYOldValue = (double)dtbVRecd.Rows[0]["INVQTY"];
                    */
                    if (e.NewValues["PASQTY"] == null)
                    {
                        GridViewDataColumn dataColumnSHFTQT = SubDBGrid.Columns["PASQTY"] as GridViewDataColumn;
                        e.Errors[dataColumnSHFTQT] = "尚未輸入異動數量 !!";
                        //若column use EditItemTemplate 不可搭配 ASPxGridView.RowUpdating
                        //若產生e.Errors ,在EditItemTemplate的元件值會被清空
                        //ASPxGridView.RowUpdating - e.NewValues does not contain the value of the column with EditItemTemplate
                    }
                    else if (Convert.ToDouble(e.NewValues["PASQTY"]) > CDVRecdINVQTYOldValue)
                    {
                        GridViewDataColumn dataColumnSHFTQT = SubDBGrid.Columns["PASQTY"] as GridViewDataColumn;
                        e.NewValues["PASQTY"] = "0";
                        e.Errors[dataColumnSHFTQT] = "異動數量超過目前庫存量 !!";
                        //AjaxHelper.showAlert((Control)sender, "異動數量超過目前庫存量 !!");
                        //return;
                    }
                }
                else
                {
                    //更改
                    /*
                    double CDVRecdSHFTQTYOldValue = 0;
                    string cmd = "SELECT VRecd.RECNO, VRecd.RECITNO,  VRecd.RECDT, VRecd.SHFTQTY "
                             + "FROM VRecd "
                             + "WHERE VRecd.RECNO='" + edMANO.Value + "'AND VRecd.RECITNO='" + edMANO.Value + "' ";
                    DataTable dtbVRecd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                    if (dtbVRecd.Rows.Count > 0)
                        CDVRecdSHFTQTYOldValue = (double)dtbVRecd.Rows[0]["PASQTY"];
                    */
                    //ASPxGridView.RowUpdating - e.NewValues does not contain the value of the column with EditItemTemplate
                    /*
                    ASPxGridView gv = sender as ASPxGridView;
                    GridViewDataColumn column = gv.Columns["MANO"] as GridViewDataColumn;
                    ASPxComboBox edMANO = (ASPxComboBox)SubDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)SubDBGrid.Columns[2], "edMANO");
                    //ASPxTextBox edSHFTQTY = (ASPxTextBox)SubDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)SubDBGrid.Columns[10], "edSHFTQTY");
                    //取得即時庫存
                    double CDVRecdINVQTYOldValue = 0;
                    string cmd = "SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB, "
                             + "Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST, "
                             + "Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO, Ma.ISINV "
                             + "FROM Ma "
                             + "WHERE Ma.MANO='" + edMANO.Value + "' ";
                    DataTable dtbVRecd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                    if (dtbVRecd.Rows.Count > 0)
                        CDVRecdINVQTYOldValue = (double)dtbVRecd.Rows[0]["INVQTY"];
                     */
                    if (Convert.ToDouble(e.NewValues["PASQTY"]) > (CDVRecdINVQTYOldValue + Convert.ToDouble(e.OldValues["PASQTY"])))
                    {
                        GridViewDataColumn dataColumnSHFTQT = SubDBGrid.Columns["PASQTY"] as GridViewDataColumn;
                        e.NewValues["SHFTQT"] = e.OldValues["PASQTY"];
                        e.Errors[dataColumnSHFTQT] = "異動數量超過目前庫存量 !!";
                        //AjaxHelper.showAlert((Control)sender, "異動數量超過目前庫存量 !!");
                        //return;
                    }
                }
            }
        }

        protected void floMaRout_DataBound(object sender, EventArgs e)
        {
            Session["MANO"] = floMaRout.GetNestedControlValueByFieldName("RECNO");
        }

        protected void cbkfloMaRoutNewData(DateTime CurRECDT)
        {
            //抓編碼最大值
            string CurMANO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Rec", "RECNO", "", true, "yyyymmdd####", CurRECDT, Session["ActDeptBID"].ToString());
            Session["MANO"] = CurMANO;
            floMaRout.DataBind();
            //避免之前SubDBGrid有新增編輯動作但未解除直接回到瀏覽頁面
            SubDBGrid.CancelEdit();
            //設定控制項初始值
            edMANO.Value = CurMANO;
            //edRECDT.Value = CurRECDT;
            /*
            if (cbxBVENNO.Value != null)
            {
                edVENNO.Value = cbxBVENNO.Value;
                edBPURNO.DataBind();
            }
            */
        }


        protected void edMANO_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            ASPxComboBox comboBox = (ASPxComboBox)source;
            sdsMaPop1.SelectCommand =
                   @"SELECT [MANO], [MADESC], [MASPEC] FROM (select [MANO], [MADESC], [MASPEC], row_number()over(order by t.[MANO]) as [rn] from [Ma] as t where (([MANO] + ' ' + [MADESC] + ' ' + [MASPEC]) LIKE @filter)) as st where st.[rn] between @startIndex and @endIndex";

            sdsMaPop1.SelectParameters.Clear();
            sdsMaPop1.SelectParameters.Add("filter", TypeCode.String, string.Format("%{0}%", e.Filter));
            sdsMaPop1.SelectParameters.Add("startIndex", TypeCode.Int64, (e.BeginIndex + 1).ToString());
            sdsMaPop1.SelectParameters.Add("endIndex", TypeCode.Int64, (e.EndIndex + 1).ToString());
            //comboBox.DataSourceID = null;
            comboBox.DataSource = sdsMaPop1;
            comboBox.DataBind();
            //comboBox.DataBindItems();
        }

        protected void edMANO_ItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
        {
            long value = 0;
            if (e.Value == null || !Int64.TryParse(e.Value.ToString(), out value))
                return;
            ASPxComboBox comboBox = (ASPxComboBox)source;
            sdsMaPop1.SelectCommand = @"SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC FROM Ma WHERE (MANO = @MANO) ORDER BY MANO";

            sdsMaPop1.SelectParameters.Clear();
            sdsMaPop1.SelectParameters.Add("MANO", TypeCode.String, e.Value.ToString());
            //comboBox.DataSourceID = null;
            comboBox.DataSource = sdsMaPop1;
            comboBox.DataBind();
            //comboBox.DataBindItems();
        }

        protected void cbkfloMaRout_Callback(object sender, CallbackEventArgsBase e)
        {
            if ((e.Parameter.Length > 7) && (e.Parameter.Substring(0, 7) == "edRECDT"))
            {
                if ((int)Session["MainDBNavgMode"] == 1)
                {
                    //新增
                    string CurRECDT = e.Parameter.Split('|')[1].Trim();
                    //cbkfloMaRoutNewData(CurRECDT);
                    //抓編碼最大值
                    string CurMANO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "Rec", "RECNO", "", true, "yyyymmdd####", Convert.ToDateTime(CurRECDT), Session["ActDeptBID"].ToString());
                    edMANO.Value = CurMANO;
                    Session["MANO"] = edMANO.Value;
                    //floMaRout.DataBind();
                }
            }
            else if (e.Parameter == "SyncfloMaRout")
            {
                floMaRout.DataBind();
            }
            else
            {
                //edMANO
                string CurMANO = e.Parameter;
                //ASPxTextBox edMADESC = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["MADESC"], "edMADESC");
                //ASPxTextBox edMASPEC = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["MASPEC"], "edMASPEC");
                //ASPxTextBox edMROUTID = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["MROUTID"], "edMROUTID");
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
                //抓編碼最大值
                string NewMROUTID = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "MaRout", "MROUTID", "MANO='" + CurMANO + "'", true, "#####", Convert.ToDateTime(null), "");
                edMROUTID.Value = NewMROUTID;
            }
        }

        protected void sdsMaRout_Deleting(object sender, SqlDataSourceCommandEventArgs e)
        {
            string CurMANO = e.Command.Parameters["@RECNO"].Value.ToString();
            string cmdVRecd = "SELECT Recd.RECNO, Recd.RECITNO, Recd.RECDT, Recd.PURNO, Recd.PURITNO  "
                                 + "FROM Recd  "
                                 + "WHERE Recd.RECNO='" + CurMANO + "' ";
            DataTable dtbVRecd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdVRecd);
            for (int i = 0; i < dtbVRecd.Rows.Count; i++)
            {
                sdsMaRoutProc.DeleteParameters["RECNO"].DefaultValue = dtbVRecd.Rows[i]["RECNO"].ToString();
                sdsMaRoutProc.DeleteParameters["RECITNO"].DefaultValue = dtbVRecd.Rows[i]["RECITNO"].ToString();
                sdsMaRoutProc.Delete();
                //觸發sdsMaRoutProc_Deleting事件-->進行同步庫存+同步訂購單

            }
        }

        protected void sdsMaRout_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (floMaRout.DataSourceID == "")
                floMaRout.DataSourceID = "sdsMaRout";
        }

        protected void fltMaRoutProc_PreRender(object sender, EventArgs e)
        {
            ASPxFormLayout CurfltMaRoutProc = (sender as ASPxFormLayout);
            ASPxComboBox edPROCFMULAOPT = (ASPxComboBox)CurfltMaRoutProc.FindControl("edPROCFMULAOPT") as ASPxComboBox;
            if (edPROCFMULAOPT.Value != null)
                fltMaRoutProcItemsSetting(Convert.ToInt16(edPROCFMULAOPT.Value));
            else
                fltMaRoutProcItemsSetting(0);
            /*
            ASPxComboBox edPURITNO = (ASPxComboBox)CurfltMaRoutProc.FindControl("edPURITNO") as ASPxComboBox;
            ASPxComboBox edMAKDNO = (ASPxComboBox)CurfltMaRoutProc.FindControl("edMAKDNO") as ASPxComboBox;
            ASPxComboBox edMANO = (ASPxComboBox)CurfltMaRoutProc.FindControl("edMANO") as ASPxComboBox;
            ASPxComboBox edSTORID = (ASPxComboBox)CurfltMaRoutProc.FindControl("edSTORID") as ASPxComboBox;
            if (SubDBGrid.IsNewRowEditing)
            {
                //新增時
            }
            else
            {
                //更改時不可變動
                edPURNO.ClientEnabled = false;
                edPURITNO.ClientEnabled = false;
                edMAKDNO.ClientEnabled = false;
                edMANO.ClientEnabled = false;
                edSTORID.ClientEnabled = false;
            }
            */
        }

        protected void sdsMaRout_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            //刪除製程 MaRoutProc
            object CurMANO = e.Command.Parameters["@MANO"].Value;
            object CurMROUTID = e.Command.Parameters["@MROUTID"].Value;
            string DeleteCmd = "DELETE FROM MaRoutProc "
                 + "WHERE MaRoutProc.MANO='" + CurMANO.ToString() + "' AND MaRoutProc.MROUTID='" + CurMROUTID.ToString() + "' ";
            DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, DeleteCmd);
            //刪除材料 MaRoutBom
            DeleteCmd = "DELETE FROM MaRoutBom "
                 + "WHERE MaRoutBom.MANO='" + CurMANO.ToString() + "' AND MaRoutBom.MROUTID='" + CurMROUTID.ToString() + "' ";
            DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, DeleteCmd);
        }

        public void fltMaRoutProcItemsSetting(int PROCFMULAOPT)
        {
            /*
            投產公式 :
                                投入                           產出
                      PNPARA1 PNMATHOPTR PNPARA2   MATHOPTR    PHPARA1 PHMATHOPTR  PHPARA2
                         A                 B                      C                   D 
              1.單-單    A                         MATHOPTR       C                  
              2.單-單                      B       MATHOPTR                           D
              3.雙-單    A    PNMATHOPTR   B       MATHOPTR       C                  
              4.單-雙    A                         MATHOPTR       C    PHMATHOPTR     D         
              5.雙-雙    A    PNMATHOPTR   B       MATHOPTR       C    PHMATHOPTR     D         
              6.雙-雙    A    PNMATHOPTR   C       MATHOPTR       B    PHMATHOPTR     D  
            */
            ASPxCallbackPanel cbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
            ASPxFormLayout fltMaRoutProc = cbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
            //((ASPxComboBox)fltMaRoutProc.FindControl("edPURITNO")).DataBindItems();
            switch (PROCFMULAOPT)
            {
                case 0:
                    //0.預設
                    fltMaRoutProc.FindItemByFieldName("PNPARA1").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PNPARA2").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHPARA1").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHPARA2").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PNMATHOPTR").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHMATHOPTR").ClientVisible = false;
                    break;
                case 1:
                    //1.單-單-寬
                    fltMaRoutProc.FindItemByFieldName("PNPARA1").ClientVisible = true;
                    fltMaRoutProc.FindItemByFieldName("PNPARA2").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHPARA1").ClientVisible = true;
                    fltMaRoutProc.FindItemByFieldName("PHPARA2").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PNMATHOPTR").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHMATHOPTR").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PN2RTEXGRATE").ClientVisible = false;
                    break;
                case 2:
                    //2.單-單-長
                    fltMaRoutProc.FindItemByFieldName("PNPARA1").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PNPARA2").ClientVisible = true;
                    fltMaRoutProc.FindItemByFieldName("PHPARA1").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHPARA2").ClientVisible = true;
                    fltMaRoutProc.FindItemByFieldName("PNMATHOPTR").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHMATHOPTR").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PN2RTEXGRATE").ClientVisible = false;
                    break;
                case 3:
                    //2.雙-單
                    break;
                case 6:
                    //6.雙-雙
                    fltMaRoutProc.FindItemByFieldName("PNPARA1").ClientVisible = true;
                    fltMaRoutProc.FindItemByFieldName("PNPARA2").ClientVisible = true;
                    fltMaRoutProc.FindItemByFieldName("PHPARA1").ClientVisible = true;
                    fltMaRoutProc.FindItemByFieldName("PHPARA2").ClientVisible = true;
                    fltMaRoutProc.FindItemByFieldName("PN2RTEXGRATE").ClientVisible = false;
                    break;
                default:
                    fltMaRoutProc.FindItemByFieldName("PNPARA1").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PNPARA2").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHPARA1").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHPARA2").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PNMATHOPTR").ClientVisible = false;
                    fltMaRoutProc.FindItemByFieldName("PHMATHOPTR").ClientVisible = false;
                    break;
            }
        }

        public void calcPFNHQTYByFmula()
        {
            /*
            投產公式 :
                                投入                           產出
                      PNPARA1 PNMATHOPTR PNPARA2   MATHOPTR    PHPARA1 PHMATHOPTR  PHPARA2
                         A                 B                      C                   D 
              1.單-單    A                         MATHOPTR       C                  
              2.單-單                      B       MATHOPTR                           D
              3.雙-單    A    PNMATHOPTR   B       MATHOPTR       C                  
              4.單-雙    A                         MATHOPTR       C    PHMATHOPTR     D         
              5.雙-雙    A    PNMATHOPTR   B       MATHOPTR       C    PHMATHOPTR     D         
              6.雙-雙    A    PNMATHOPTR   C       MATHOPTR       B    PHMATHOPTR     D  
            */
            int PROCFMULAOPT = 0;
            string strPNPARA1 = "";
            string strPNPARA2 = "";
            string strPHPARA1 = "";
            string strPHPARA2 = "";
            string strPNMATHOPTR = "";
            string strPHMATHOPTR = "";
            string valMATHOPTR = "";
            ASPxCallbackPanel cbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
            ASPxFormLayout fltMaRoutProc = cbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
            ASPxComboBox edPROCFMULAOPT = ((ASPxComboBox)fltMaRoutProc.FindControl("edPROCFMULAOPT"));
            ASPxTextBox edPNPARA1 = ((ASPxTextBox)fltMaRoutProc.FindControl("edPNPARA1"));
            ASPxTextBox edPNPARA2 = ((ASPxTextBox)fltMaRoutProc.FindControl("edPNPARA2"));
            ASPxTextBox edPHPARA1 = ((ASPxTextBox)fltMaRoutProc.FindControl("edPHPARA1"));
            ASPxTextBox edPHPARA2 = ((ASPxTextBox)fltMaRoutProc.FindControl("edPHPARA2"));
            ASPxTextBox edPUTNQTY = ((ASPxTextBox)fltMaRoutProc.FindControl("edPUTNQTY"));
            ASPxTextBox edPFNHQTY = ((ASPxTextBox)fltMaRoutProc.FindControl("edPFNHQTY"));
            ASPxComboBox edPNMATHOPTR = ((ASPxComboBox)fltMaRoutProc.FindControl("edPNMATHOPTR"));
            ASPxComboBox edPHMATHOPTR = ((ASPxComboBox)fltMaRoutProc.FindControl("edPHMATHOPTR"));
            ASPxComboBox edMATHOPTR = ((ASPxComboBox)fltMaRoutProc.FindControl("edMATHOPTR"));
            PROCFMULAOPT = Convert.ToInt16(edPROCFMULAOPT.Value);
            switch (PROCFMULAOPT)
            {
                case 1:
                    //1.單-單-寬
                    //投產公式:  1 --> A  MATHOPTR C
                    //  1300  / 48 * PUTNQTY   (1300mm 投入 PUTNQTY支)
                    //string strPNPARA2 = "";
                    //string strPHPARA2 = "";
                    if (edPNPARA1.Value != null)
                        strPNPARA1 = edPNPARA1.Text;
                    if (edPHPARA1.Value != null)
                        strPHPARA1 = edPHPARA1.Text;
                    if (edMATHOPTR.Value != null)
                        valMATHOPTR = edMATHOPTR.Text;
                    if (strPNPARA1 != "" && strPHPARA1 != "")
                    {
                        //RoutShftDBGrid.GetEditor('PCONFIRM').SetChecked(true);
                        //RoutShftDBGrid.GetEditor('PUTNDT').SetDate(new Date());
                        //RoutShftDBGrid.GetEditor('SHFTQTY').SetValue(strPUTNQTY);
                        //B
                        string dblPFNHOpnt1 = strPNPARA1;
                        //D
                        string dblPFNHOpnt2 = strPHPARA1;
                        //B * D
                        double PFNHQTY = Math.Floor(DBUtility.CalcByDataTable(dblPFNHOpnt1 + valMATHOPTR + dblPFNHOpnt2));
                        //PUTNQTY 覆捲 : 投入支數, 預設 1
                        string strPUTNQTY = edPUTNQTY.Text == "" ? "1" : edPUTNQTY.Text;
                        PFNHQTY = DBUtility.CalcByDataTable(PFNHQTY + "*" + strPUTNQTY);
                        edPFNHQTY.Value = PFNHQTY.ToString();
                    }
                    break;
                case 2:
                    //1.單-單-長
                    //投產公式: 2-- > B  MATHOPTR D
                    //  1000 / 100 * PUTNQTY(1000M 投入 PUTNQTY支)
                    //string strPNPARA2 = "";
                    //string strPHPARA2 = "";
                    if (edPNPARA2.Value != null)
                        strPNPARA2 = edPNPARA2.Text;
                    if (edPHPARA2.Value != null)
                        strPHPARA2 = edPHPARA2.Text;
                    if (edMATHOPTR.Value != null)
                        valMATHOPTR = edMATHOPTR.Text;
                    if (strPNPARA2 != "" && strPHPARA2 != "")
                    {
                        //RoutShftDBGrid.GetEditor('PCONFIRM').SetChecked(true);
                        //RoutShftDBGrid.GetEditor('PUTNDT').SetDate(new Date());
                        //RoutShftDBGrid.GetEditor('SHFTQTY').SetValue(strPUTNQTY);
                        //B
                        string dblPFNHOpnt1 = strPNPARA2;
                        //D
                        string dblPFNHOpnt2 = strPHPARA2;
                        //B * D
                        double PFNHQTY = Math.Floor(DBUtility.CalcByDataTable(dblPFNHOpnt1 + valMATHOPTR + dblPFNHOpnt2));
                        //PUTNQTY 覆捲 : 投入支數, 預設 1
                        string strPUTNQTY = edPUTNQTY.Text == "" ? "1" : edPUTNQTY.Text;
                        PFNHQTY = DBUtility.CalcByDataTable(PFNHQTY + "*" + strPUTNQTY);
                        edPFNHQTY.Value = PFNHQTY.ToString();
                    }
                    break;
                case 3:
                    //2.雙-單
                    break;
                case 6:
                    //6.雙-雙
                    //投產公式: 6-- > A  PNMATHOPTR C    MATHOPTR B   PHMATHOPTR D
                    //     (1600 / 48 * 8000 / 45) * PUTNQTY  (1600mm*8000M 投入 PUTNQTY支)
                    /*
                    int valPNPARA1 = 0;
                    int valPNPARA2 = 0;
                    int valPHPARA1 = 0;
                    int valPHPARA2 = 0;
                    */
                    //string strPNPARA1 = "";
                    //string strPNPARA2 = "";
                    //string strPHPARA1 = "";
                    //string strPHPARA2 = "";
                    if (edPNPARA1.Value != null)
                        strPNPARA1 = edPNPARA1.Text;
                    if (edPNPARA2.Value != null)
                        strPNPARA2 = edPNPARA2.Text;

                    if (edPHPARA1.Value != null)
                        strPHPARA1 = edPHPARA1.Text;
                    //strPHPARA1 = Convert.ToInt16(dtbWoRoutProc.Rows[0]["PHPARA1"]);
                    if (edPHPARA2.Value != null)
                        strPHPARA2 = edPHPARA2.Text;
                    //strPHPARA2 = Convert.ToInt16(dtbWoRoutProc.Rows[0]["PHPARA2"]);
                    if (edPNMATHOPTR.Value != null)
                        strPNMATHOPTR = edPNMATHOPTR.Text;
                    if (edPHMATHOPTR.Value != null)
                        strPHMATHOPTR = edPHMATHOPTR.Text;
                    if (edMATHOPTR.Value != null)
                        valMATHOPTR = edMATHOPTR.Text;
                    if (strPNPARA1 != "" && strPNPARA2 != "" && strPHPARA1 != "" && strPHPARA2 != "")
                    {
                        //RoutShftDBGrid.GetEditor('PCONFIRM').SetChecked(true);
                        //RoutShftDBGrid.GetEditor('PUTNDT').SetDate(new Date());
                        //RoutShftDBGrid.GetEditor('SHFTQTY').SetValue(strPUTNQTY);
                        //A    PNMATHOPTR   C
                        double dblPFNHOpnt1 = DBUtility.CalcByDataTable(strPNPARA1 + strPNMATHOPTR + strPHPARA1);
                        //B    PNMATHOPTR   D
                        double dblPFNHOpnt2 = DBUtility.CalcByDataTable(strPNPARA2 + strPHMATHOPTR + strPHPARA2);
                        //小數去尾
                        dblPFNHOpnt1 = Math.Floor(dblPFNHOpnt1);
                        dblPFNHOpnt2 = Math.Floor(dblPFNHOpnt2);
                        //PUTNQTY 支數, jumble 預設 1
                        string strPUTNQTY = edPUTNQTY.Text == "" ? "1" : edPUTNQTY.Text;
                        double PFNHQTY = DBUtility.CalcByDataTable("(" + dblPFNHOpnt1 + valMATHOPTR + dblPFNHOpnt2 + ")" + "*" + strPUTNQTY);
                        edPFNHQTY.Value = PFNHQTY.ToString();
                    }

                    /*
                    //  覆捲: 輸入 長      = 投入數量
                    //  裁切: 輸入 寬      = 投入數量
                    //  分條: 輸入 長 * 寬 = 投入數量
                    object ojtPUTNINPTFORMAT = dtbWoRoutProc.Rows[0]["PUTNINPTFORMAT"];
                    if (ojtPUTNINPTFORMAT == DBNull.Value)
                    {
                        //  覆捲: 輸入 長      = 投入數量
                        //  裁切: 輸入 寬      = 投入數量
                        tbxBPUTNINPTFORMAT.Visible = false;
                        //tbxBPUTNQTY.Visible = true;
                        tbxBPUTNQTY.ClientEnabled = true;
                    }
                    else
                    {
                        //  分條: 輸入 長 * 寬 = 投入數量
                        tbxBPUTNINPTFORMAT.Visible = true;
                        tbxBPUTNINPTFORMAT.MaskSettings.Mask = ojtPUTNINPTFORMAT.ToString();
                        tbxBPUTNQTY.ClientEnabled = false;
                        //tbxBPUTNQTY.Visible = false;
                        //tbxBPUTNINPTFORMAT.Width = Unit.Parse("100%");
                    }
                    */
                    break;
                default:
                    break;
            }
        }

        protected void cbkPanelMANO_Callback(object sender, CallbackEventArgsBase e)
        {
            if ((e.Parameter != null) && (e.Parameter.Length > 0))
            {
                fltMaRoutProcItemsSetting(Convert.ToInt16(e.Parameter));
            }
        }

        protected void cbpfltMaRoutProc_Callback(object sender, CallbackEventArgsBase e)
        {
            if ((e.Parameter == "PNPARA1") || (e.Parameter == "PNPARA2") || (e.Parameter == "PUTNQTY"))
            {
                calcPFNHQTYByFmula();
                ASPxCallbackPanel cbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                ASPxFormLayout fltMaRoutProc = cbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                ASPxComboBox edPROCFMULAOPT = ((ASPxComboBox)fltMaRoutProc.FindControl("edPROCFMULAOPT"));
                if (edPROCFMULAOPT.Value != null)
                    fltMaRoutProcItemsSetting(Convert.ToInt16(edPROCFMULAOPT.Value));
            }
            else if (e.Parameter == "PROCFMULAOPT")
            {
                ASPxCallbackPanel cbpfltMaRoutProc = SubDBGrid.FindEditFormTemplateControl("cbpfltMaRoutProc") as ASPxCallbackPanel;
                ASPxFormLayout fltMaRoutProc = cbpfltMaRoutProc.FindControl("fltMaRoutProc") as ASPxFormLayout;
                ASPxComboBox edPROCFMULAOPT = ((ASPxComboBox)fltMaRoutProc.FindControl("edPROCFMULAOPT"));
                if (edPROCFMULAOPT.Value != null)
                    fltMaRoutProcItemsSetting(Convert.ToInt16(edPROCFMULAOPT.Value));
            }
        }

        protected void edTSPREPHMANTICK_DataBound(object sender, EventArgs e)
        {
            //Tick轉換為TimeSpan
            ASPxTextBox CurTimeEdit = sender as ASPxTextBox;
            Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
            TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
            string strCurTIME = CurTIME.Days.ToString().PadLeft(2, '0') + CurTIME.Hours.ToString().PadLeft(2, '0') + CurTIME.Minutes.ToString().PadLeft(2, '0') + CurTIME.Seconds.ToString().PadLeft(2, '0');
            CurTimeEdit.Value = strCurTIME;
            /*
            ASPxTimeEdit CurTimeEdit = sender as ASPxTimeEdit;
            Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
            TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
            //DateTime TSPREPHMANTM = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //TSPREPHMANTM.AddTicks(CurTICK);
            DateTime TSPREPHMANTM = new DateTime().AddTicks(CurTICK);
            //CurTimeEdit.Value = TSPREPHMANTM.AddDays(-1);
            //DateTime TSPREPHMANTM = new DateTime()
            CurTimeEdit.Value = TimeSpan.FromTicks(CurTICK).ToString(@"dd\.hh\:mm\:ss");
            //CurTimeEdit.Value = CurTIME;
            */
        }

        protected void edTSPREPMACHTICK_DataBound(object sender, EventArgs e)
        {
            //Tick轉換為TimeSpan
            ASPxTextBox CurTimeEdit = sender as ASPxTextBox;
            //if (CurTimeEdit.Value != null)
            //{
                Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
                TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
                string strCurTIME = CurTIME.Days.ToString().PadLeft(2, '0') + CurTIME.Hours.ToString().PadLeft(2, '0') + CurTIME.Minutes.ToString().PadLeft(2, '0') + CurTIME.Seconds.ToString().PadLeft(2, '0');
                CurTimeEdit.Value = strCurTIME;
            //}
        }

        protected void edTSBTCHHMANTICK_DataBound(object sender, EventArgs e)
        {
            ASPxTextBox CurTimeEdit = sender as ASPxTextBox;
            //if (CurTimeEdit.Value != null)
            //{
                Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
                TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
                string strCurTIME = CurTIME.Days.ToString().PadLeft(2, '0') + CurTIME.Hours.ToString().PadLeft(2, '0') + CurTIME.Minutes.ToString().PadLeft(2, '0') + CurTIME.Seconds.ToString().PadLeft(2, '0');
                CurTimeEdit.Value = strCurTIME;
            //}

            /*
            ASPxTimeEdit edTSPREPHMANTICK = sender as ASPxTimeEdit;
            if (edTSPREPHMANTICK.Value != null)
            {
                //Tick轉換為TimeSpan
                Int64 TSPREPHMANTICK = Convert.ToInt64(edTSPREPHMANTICK.Value);
                DateTime TSPREPHMANTM = new DateTime(TSPREPHMANTICK);
                edTSPREPHMANTICK.Value = TSPREPHMANTM;
            }
            */
        }

        protected void edTSBTCHMACHTICK_DataBound(object sender, EventArgs e)
        {
            ASPxTextBox CurTimeEdit = sender as ASPxTextBox;
            //if (CurTimeEdit.Value != null)
            //{
                Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
                TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
                string strCurTIME = CurTIME.Days.ToString().PadLeft(2, '0') + CurTIME.Hours.ToString().PadLeft(2, '0') + CurTIME.Minutes.ToString().PadLeft(2, '0') + CurTIME.Seconds.ToString().PadLeft(2, '0');
                CurTimeEdit.Value = strCurTIME;
            //}
            /*
            ASPxTimeEdit CurTimeEdit = sender as ASPxTimeEdit;
            if (CurTimeEdit.Value != null)
            {
                //Tick轉換為TimeSpan
                Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
                DateTime CurTIME = new DateTime(CurTICK);
                CurTimeEdit.Value = CurTIME;
            }
            */
        }

        protected void edSPREPHMANTICK_DataBound(object sender, EventArgs e)
        {
            //Tick轉換為TimeSpan
            ASPxTextBox CurTimeEdit = sender as ASPxTextBox;
            //CurTimeEdit.Value = DBUtility.Tick2TSpanText(CurTimeEdit.Value); //SY
            /*
            Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
            TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
            string strCurTIME = CurTIME.Days.ToString().PadLeft(2, '0') + CurTIME.Hours.ToString().PadLeft(2, '0') + CurTIME.Minutes.ToString().PadLeft(2, '0') + CurTIME.Seconds.ToString().PadLeft(2, '0');
            CurTimeEdit.Value = strCurTIME;
            */
        }

        protected void edSPREPMACHTICK_DataBound(object sender, EventArgs e)
        {
            //Tick轉換為TimeSpan
            ASPxTextBox CurTimeEdit = sender as ASPxTextBox;
            //CurTimeEdit.Value = DBUtility.Tick2TSpanText(CurTimeEdit.Value); //SY
            /*
            //if (CurTimeEdit.Value != null)
            //{
            Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
                TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
                string strCurTIME = CurTIME.Days.ToString().PadLeft(2, '0') + CurTIME.Hours.ToString().PadLeft(2, '0') + CurTIME.Minutes.ToString().PadLeft(2, '0') + CurTIME.Seconds.ToString().PadLeft(2, '0');
                CurTimeEdit.Value = strCurTIME;
            //}
            */
        }

        protected void edSBTCHHMANTICK_DataBound(object sender, EventArgs e)
        {
            ASPxTextBox CurTimeEdit = sender as ASPxTextBox;
            //CurTimeEdit.Value = DBUtility.Tick2TSpanText(CurTimeEdit.Value);//SY
            /*
            //if (CurTimeEdit.Value != null)
            //{
            Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
                TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
                string strCurTIME = CurTIME.Days.ToString().PadLeft(2, '0') + CurTIME.Hours.ToString().PadLeft(2, '0') + CurTIME.Minutes.ToString().PadLeft(2, '0') + CurTIME.Seconds.ToString().PadLeft(2, '0');
                CurTimeEdit.Value = strCurTIME;
            //}
            */
        }

        protected void edSBTCHMACHTICK_DataBound(object sender, EventArgs e)
        {
            ASPxTextBox CurTimeEdit = sender as ASPxTextBox;
            //CurTimeEdit.Value = DBUtility.Tick2TSpanText(CurTimeEdit.Value);//SY
            /*
            //if (CurTimeEdit.Value != null)
            //{
            Int64 CurTICK = Convert.ToInt64(CurTimeEdit.Value);
                TimeSpan CurTIME = TimeSpan.FromTicks(CurTICK);
                string strCurTIME = CurTIME.Days.ToString().PadLeft(2, '0') + CurTIME.Hours.ToString().PadLeft(2, '0') + CurTIME.Minutes.ToString().PadLeft(2, '0') + CurTIME.Seconds.ToString().PadLeft(2, '0');
                CurTimeEdit.Value = strCurTIME;
            //}
            */
        }

        protected void sdsMaRoutProc_Inserted(object sender, SqlDataSourceStatusEventArgs e)
        {
            object CurMANO = edMANO.Value;
            object CurMROUTID = edMROUTID.Value;
            TotalMaRoutManuHours(CurMANO.ToString(), CurMROUTID.ToString());
            floMaRout.DataBind();
        }

        protected void sdsMaRoutProc_Updated(object sender, SqlDataSourceStatusEventArgs e)
        {
            object CurMANO = edMANO.Value;
            object CurMROUTID = edMROUTID.Value;
            TotalMaRoutManuHours(CurMANO.ToString(), CurMROUTID.ToString());
            //SyncMaRoutManuHours
            floMaRout.DataBind();
        }

        protected void sdsMaRoutProc_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            object CurMANO = edMANO.Value;
            object CurMROUTID = edMROUTID.Value;
            TotalMaRoutManuHours(CurMANO.ToString(), CurMROUTID.ToString());
            floMaRout.DataBind();
        }

        public bool TotalMaRoutManuHours(string CurMANO, string CurMROUTID)
        {
            string UpdateCmd = "";
            //取得該製程累計投入/轉入計算預計完成數量
            string cmdMaRoutProc = "SELECT SUM(SPREPHMANTICK) SPREPHMANTICK, SUM(SPREPMACHTICK) SPREPMACHTICK, SUM(SBTCHHMANTICK) SBTCHHMANTICK, SUM(SBTCHMACHTICK) SBTCHMACHTICK "
                    + "FROM MaRoutProc "
                    + "WHERE MaRoutProc.MANO='" + CurMANO + "' "
                    + "AND MaRoutProc.MROUTID='" + CurMROUTID + "' ";
            DataTable dtbMaRoutProc = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdMaRoutProc);
            if (dtbMaRoutProc.Rows.Count == 0)
            {
                //製程尚未開始
                //更新累計投入數量 = 0 &開始時程 = 現在時間
                UpdateCmd = "UPDATE MaRout "
                     + "SET "
                     + "TSPREPHMANTICK=0, "
                     + "TSPREPMACHTICK=0, "
                     + "TSBTCHHMANTICK=0, "
                     + "TSBTCHMACHTICK=0 "
                     + "WHERE MaRout.MANO='" + CurMANO + "' AND MaRout.MROUTID='" + CurMROUTID + "' ";
                if (SqlHelper.ExecuteNonQuery(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, CommandType.Text, UpdateCmd) != 1)
                    return false;
                //fltMaRoutProc.DataBind();
            }
            else
            {
                UpdateCmd = "UPDATE MaRout "
                     + "SET "
                     + "TSPREPHMANTICK="+dtbMaRoutProc.Rows[0]["SPREPHMANTICK"] +", "
                     + "TSPREPMACHTICK=" + dtbMaRoutProc.Rows[0]["SPREPMACHTICK"] + ", "
                     + "TSBTCHHMANTICK=" + dtbMaRoutProc.Rows[0]["SBTCHHMANTICK"] + ", "
                     + "TSBTCHMACHTICK=" + dtbMaRoutProc.Rows[0]["SBTCHMACHTICK"] + " "
                     + "WHERE MaRout.MANO='" + CurMANO + "' AND MaRout.MROUTID='" + CurMROUTID + "' ";
                if (SqlHelper.ExecuteNonQuery(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, CommandType.Text, UpdateCmd) != 1)
                    return false;
            }
            return true;
        }

        //根據報工 --> 累計加總製令加工人時
        public bool SyncMaRoutManuHours(string CurMANO, string CurMROUTID, string CurTickFldnm, Int64 CurPROCTICK, string CurSHFTKDNO)
        {
            /* 報工單
             *  新增 : CurSHFTKDNO == 1 
             *  刪除 : CurSHFTKDNO == 3  
             * 
             */
            string UpdateCmd = "";

            if (CurSHFTKDNO == "1")
            {
                UpdateCmd = "UPDATE MaRout "
                     + "SET "
                     + CurTickFldnm + "+= " + CurPROCTICK + " "
                     + "WHERE MaRout.MANO='" + CurMANO + "' AND MaRout.MROUTID = '" + CurMROUTID + "' ";
            }
            else if (CurSHFTKDNO == "3")
            {
                UpdateCmd = "UPDATE MaRout "
                     + "SET "
                     + CurTickFldnm + "-= " + CurPROCTICK + " "
                     + "WHERE MaRout.MANO='" + CurMANO + "' AND MaRout.MROUTID = '" + CurMROUTID + "' ";
            }
            if (DBUtility.ExecuteNonQuery(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd) == 0)
                return false;
            return true;
        }
    }
}