/*

 ASPxGridView 使用 ASPxFormLayout in EditFormTemplate 
  . ASPxFormLayout無須設定DataSourceID
  . 每個LayoutItem須使用<%#Eval("欄位名稱") %>'綁定欄位
  . LayoutItem 使用DateTime元件若設定為 Enabled="false" 在ProcessLayoutItem程序 會造成editBase.Value = null
  . ASPxFormLayout的存入Button.Click事件自行設定存入邏輯, 
    可搭配SQLDataSource的InsertQuery與 UpddateQuery進行更新如下例:
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (MainDBGrid.IsNewRowEditing)
            {
                //新增
                (MainDBGrid.FindEditFormTemplateControl("FormLayoutPrsn") as ASPxFormLayout).ForEach(ProcessLayoutItem);
                sdsMaRout.Insert(); //Uncomment this line to allow updating.
            }
            else
            {
                //更改
                (MainDBGrid.FindEditFormTemplateControl("FormLayoutPrsn") as ASPxFormLayout).ForEach(ProcessLayoutItem);
                sdsMaRout.Update(); //Uncomment this line to allow updating.
            }


勿設定 ....TextField="UNITNM" ValueField="UNITNM" --> 按儲存後內容若有中文會顯示為Unicode編碼 (Ex.捲 會顯示為 &#25458;)
  請修正為....TextField="UNITNM" ValueField="UNITNO"
            <dx:GridViewDataComboBoxColumn Caption="投入單位" FieldName="PNUNITNM" ShowInCustomizationForm="True" VisibleIndex="6">
                <PropertiesComboBox DataSourceID="sdsUnit" TextField="UNITNM" ValueField="UNITNO" DropDownStyle="DropDown">
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>

投產公式 :
  0.標準             PUTNQTY               MATHOPTR    PN2RTEXGRATE    PFNHQTY                  

                    投入                                    產出
              PNPARA1 PNMATHOPTR PNPARA2   MATHOPTR    PHPARA1 PHMATHOPTR  PHPARA2
                 A                 B                      C                   D 
  1.單-單-寬     A                         MATHOPTR       C                  
  2.單-單-長                       B       MATHOPTR                           D
  3.雙-單        A    PNMATHOPTR   B       MATHOPTR       C                  
  4.單-雙        A                         MATHOPTR       C    PHMATHOPTR     D         
  5.雙-雙        A    PNMATHOPTR   B       MATHOPTR       C    PHMATHOPTR     D         
  6.雙-雙        A    PNMATHOPTR   C       MATHOPTR       B    PHMATHOPTR     D  

  
  Ex. 覆捲 : 投入                       1000M  產出  1250mm * 100M
                                   A      B             C      D

      PUTNQTY 支數,  預設 1
      投產公式 : 2 --> B  MATHOPTR D  
                 1000  / 100 * PUTNQTY   (1000M 投入 PUTNQTY支)
      
  Ex. 裁切 : 投入                1300mm  27M  產出     48mm * 27M
                                   A      B             C      D

      PUTNQTY 支數,  預設 1
      投產公式 : 1 --> A  MATHOPTR C  
                 1300  / 48 * PUTNQTY   (1300mm 投入 PUTNQTY支)
      
  Ex. 分條 : 投入 jumble        1600mm*8000M    產出  48mm *  45M
                                   A      B             C      D
      PUTNQTY 支數, jumble 預設 1
      投產公式 : 6 --> A  PNMATHOPTR   C    MATHOPTR    B   PHMATHOPTR   D 
                 (1600 / 48 * 8000 / 45) * PUTNQTY  (1600mm*8000M 投入 PUTNQTY支)

使用Excel匯入後, 需再補上下列參數 :
  覆捲
    UPDATE MaRoutProc
    SET 
    PNUNITNM='支',
    MATHOPTR ='/',
    FNUNITNM='支'
    WHERE       (PROCFMULAOPT = 2)

  切捲
    UPDATE MaRoutProc
    SET 
    PNUNITNM='支',
    MATHOPTR ='/'
    WHERE       (PROCFMULAOPT = 1)

  分條
    UPDATE MaRoutProc
    SET 
    PNMATHOPTR ='/',
    PNUNITNM='支',
    MATHOPTR ='*',
    PHMATHOPTR ='/',
    FNUNITNM='捲'
    WHERE       (PROCFMULAOPT = 6)


 * 
SELECT Dpt.DPTNO, Dpt.DPTNM, Prsn.PRSNNO, Prsn.PRSNNM, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.AGENDAKDNO, Agendakd.AGENDAKDNM, 
 Agenda.DUTYSTTM, Agenda.DUTYEDTM, Dutykd.LATECAPTMM, Agenda.AGENDAID
FROM  Dpt INNER JOIN Prsn ON (Dpt.DPTNO = Prsn.DPTNO)
 LEFT JOIN Agenda ON (Dpt.DPTNO = Agenda.DPTNO AND Prsn.PRSNNO = Agenda.PRSNNO)
 LEFT JOIN Agendakd ON (Agenda.AGENDAKDNO = Agendakd.AGENDAKDNO)  
 LEFT JOIN Dutykd ON (Agendakd.AGENDAKDNO = Dutykd.DUTYKDNO)  
 * 
SELECT Dpt.DPTNO, Dpt.DPTNM, Prsn.PRSNNO, Prsn.PRSNNM, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.DUTYKDNO, Dutykd.DUTYKDNM, 
 Agenda.DUTYSTTM, Agenda.DUTYEDTM, Dutykd.LATECAPTMM, Agenda.AGENDAID
FROM  Dpt INNER JOIN Prsn ON (Dpt.DPTNO = Prsn.DPTNO)
 LEFT JOIN Agenda ON (Dpt.DPTNO = Agenda.DPTNO AND Prsn.PRSNNO = Agenda.PRSNNO)
 LEFT JOIN Dutykd ON (Agenda.DUTYKDNO = Dutykd.DUTYKDNO)  
 * 
SELECT Dpt.DPTNO, Dpt.DPTNM, Prsn.PRSNNO, Prsn.PRSNNM, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.DUTYKDNO, Dutykd.DUTYKDNM, 
 Agenda.DUTYSTTM, Agenda.DUTYEDTM, Dutykd.LATECAPTMM, Agenda.AGENDAID
FROM  Dpt INNER JOIN Prsn ON (Dpt.DPTNO = Prsn.DPTNO)
 LEFT JOIN Agenda ON (Dpt.DPTNO = Agenda.DPTNO AND Prsn.PRSNNO = Agenda.PRSNNO)
 LEFT JOIN Dutykd ON (Agenda.DUTYKDNO = Dutykd.DUTYKDNO) 
WHERE Agenda.DPTNO='0S001' AND Agenda.AGENDASTTIME>='2016/07/18' AND Agenda.AGENDASTTIME<='2016/07/24' 
 * 
SELECT Dpt.DPTNO, Dpt.DPTNM, Prsn.PRSNNO, Prsn.PRSNNM, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.DUTYKDNO, Dutykd.DUTYKDNM, 
 Dutykd.WORKSTTM01, Dutykd.WORKEDTM01, Dutykd.WORKSTTM02, Dutykd.WORKEDTM02, Dutykd.WORKSTTM03, Dutykd.WORKEDTM03, Dutykd.WORKSTTM04, 
 Dutykd.WORKEDTM04, Dutykd.WORKSTTM05, Dutykd.WORKEDTM05, Dutykd.WORKSTTM06, Dutykd.WORKEDTM06, Dutykd.WORKSTTM07, Dutykd.WORKEDTM07, 
 Dutykd.LATECAPTMM
FROM  Dpt INNER JOIN Prsn ON (Dpt.DPTNO = Prsn.DPTNO)
 LEFT JOIN Agenda ON (Dpt.DPTNO = Agenda.DPTNO)
 LEFT JOIN Dutykd ON (Agenda.DUTYKDNO = Dutykd.DUTYKDNO) 

SELECT Prsn.PRSNNO, Prsn.PRSNNM, Prsn.DPTNO,  Prsn.TITLEID, Prsn.DUTYKDNO,
 Dutykd.WORKSTTM01, Dutykd.WORKEDTM01, Dutykd.WORKSTTM02, Dutykd.WORKEDTM02, Dutykd.WORKSTTM03, Dutykd.WORKEDTM03, Dutykd.WORKSTTM04, 
 Dutykd.WORKEDTM04, Dutykd.WORKSTTM05, Dutykd.WORKEDTM05, Dutykd.WORKSTTM06, Dutykd.WORKEDTM06, Dutykd.WORKSTTM07, Dutykd.WORKEDTM07, 
 Dutykd.LATECAPTMM
FROM Prsn LEFT JOIN Dutykd ON (Prsn.DUTYKDNO = Dutykd.DUTYKDNO)
WHERE Prsn.QUITDT IS NULL 
ORDER BY  Prsn.PRSNNO

SELECT MaRout.MANO, Ma.MADESC, Ma.MASPEC, Ma.PKUNITNM, MaRout.MROUTID, MaRout.MROUTNM, MaRout.MROUTDESC
FROM MaRout LEFT JOIN Ma ON (MaRout.MANO = Ma.MANO)

SELECT MaRoutProc.MANO, MaRoutProc.MROUTID, MaRoutProc.MROUTSRNO, MaRoutProc.MPROCID, ManuProc.MPROCNM, ManuProc.MPROCDESC, ManuProc.MLINEID, ManuLine.MLINENM, 
 MaRoutProc.UNITCOST ,MaRoutProc.SPREPHMANTM ,MaRoutProc.SPREPMACHTM ,MaRoutProc.SBTCHHMANTM  ,MaRoutProc.SBTCHMACHTM
FROM MaRoutProc LEFT JOIN ManuProc ON (MaRoutProc.MPROCID = ManuProc.MPROCID)
 LEFT JOIN ManuLine ON (ManuProc.MLINEID = ManuLine.MLINEID)
WHERE MaRoutProc.MANO=@MANO AND MaRoutProc.MROUTID=@MROUTID
ORDER BY MaRoutProc.MROUTSRNO

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
//Export 使用
using DevExpress.Export;
using DevExpress.XtraPrinting;

namespace WebMES.Admin
{
    public partial class MaRout : DevDFEditGrid
    {
        protected override void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            //調閱畫面使用外在VRecACPage.aspx顯示時, 因此不會呼叫下列 cbkfloMainDB_Callback 來設定
            AEDContentUrl = "MaRoutPage.aspx";
            strEditKey = "MANO;MROUTID";
            strEditContentUrl = "'MaRoutPage.aspx?PopupForm=MaRout.aspx.aspx&EditMode=2&MANO=' + values[0] + '&MROUTID=' + values[1]";
            strDeleteContentUrl = "'MaRoutPage.aspx?PopupForm=MaRout.aspx.aspx&EditMode=3&MANO=' + values[0] + '&MROUTID=' + values[1]";
            if (!IsPostBack)
            {
                if (Session["MACDNO"] == null)
                    //預設值
                    Session["MACDNO"] = "3";
                cbxBMACDNO.Value = Session["MACDNO"] == null ? "3" : Session["MACDNO"];
                FillcbxBMAKDNOCombo(cbxBMACDNO.Value.ToString());
                /*
                //本月的第一天及最後一天
                DateTime FirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime LastDay = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1).AddDays(-1);
                tbxBSTAGENDADT.Text = FirstDay.ToString("yyyy/MM/dd");
                tbxBEDAGENDADT.Text = LastDay.ToString("yyyy/MM/dd");
                //動態指定編輯畫面
                //MainDBGrid.Templates.EditForm = new EdiFormTemplate();
                 */
            }
        }
        /*
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnGoFilter_Click(sender, e);
            }
        }
        */
        protected override void btnGoFilter_Click(object sender, EventArgs e)
        {
            pclSearchPanel.ShowOnPageLoad = false;
            string WhereSetStr = "";
            if (edBMROUTNM.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " MaRout.MROUTNM LIKE '%" + edBMROUTNM.Value.ToString() + "%' ";
                else
                    WhereSetStr += " AND MaRout.MROUTNM LIKE '%" + edBMROUTNM.Value.ToString() + "%' ";
            }
            if (cbxBMACDNO.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Ma.MACDNO='" + cbxBMACDNO.Value.ToString() + "'";
                else
                    WhereSetStr += " AND Ma.MACDNO='" + cbxBMACDNO.Value.ToString() + "'";
            }
            if (cbxBMAKDNO.Value != null)
            {
                if (WhereSetStr == "")
                    WhereSetStr += " Ma.MAKDNO='" + cbxBMAKDNO.Value.ToString() + "'";
                else
                    WhereSetStr += " AND Ma.MAKDNO='" + cbxBMAKDNO.Value.ToString() + "'";
            }
            if (sdsMaRout.SelectCommand.IndexOf("ORDER BY") >= 0)
                sdsMaRout.SelectCommand = sdsMaRout.SelectCommand.Substring(0, sdsMaRout.SelectCommand.IndexOf("ORDER BY") - 1);
            if (sdsMaRout.SelectCommand.IndexOf("WHERE ") >= 0)
                sdsMaRout.SelectCommand = sdsMaRout.SelectCommand.Substring(0, sdsMaRout.SelectCommand.IndexOf("WHERE "));
            if (WhereSetStr != "")
                sdsMaRout.SelectCommand += " WHERE " + WhereSetStr;
            //sdsMaRout.SelectCommand += " ORDER BY Prsn.PRSNNO";
            sdsMaRout.SelectCommandType = SqlDataSourceCommandType.Text;
        }
        /*
        protected void SubDBGrid_BeforePerformDataSelect(object sender, EventArgs e)
        {
            string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            Session["MANO"] = MasterKeyValue.Split('|')[0].Trim();
            Session["MROUTID"] = MasterKeyValue.Split('|')[1].Trim();
            //根據權限代碼動態設定增刪修按鍵的顯示否
            int index = MainDBGrid.FindVisibleIndexByKeyValue(MasterKeyValue);
            string FormID = this.Form.ID;
            string ParentHeadStr;
            ParentHeadStr = FormID.Substring(0, FormID.IndexOf("0"));
            DBUtility.CmdbtnSetRight(ParentHeadStr, ((MainDBGrid.FindDetailRowTemplateControl(index, "SubDBGrid") as ASPxGridView).Columns[0] as DevExpress.Web.GridViewCommandColumn), 2);
        }
        */
        protected void sdsMaRout_Inserting(object sender, SqlDataSourceCommandEventArgs e)
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
                    sdsMaRout.SelectCommand = (string)mState[1];
                if (mState[2] != null)
                    sdsMaRoutProc.SelectCommand = (string)mState[2];
            }
        }

        //SqlDataSource.SaveViewState 方法
        protected override Object SaveViewState()
        {
            object baseState = base.SaveViewState();
            object[] mState = new object[3];
            mState[0] = baseState;
            mState[1] = sdsMaRout.SelectCommand;
            mState[2] = sdsMaRoutProc.SelectCommand;
            return mState;
        }

        protected void MainDBGrid_CustomColumnDisplayText(object sender, DevExpress.Web.ASPxGridViewColumnDisplayTextEventArgs e)
        {
            //if (e.Column.FieldName != "WEGTPCNT")
            //    return;
            //e.DisplayText = e.Value+"%";

        }

        protected void ASPxFormLayoutPrsn_E19_Click(object sender, EventArgs e)
        {
            //sdsMaRout.UpdateParameters["PRSNNO"].DefaultValue = tbxPRSNNO.Value.ToString();
            //sdsMaRout.Update();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (MainDBGrid.IsNewRowEditing)
            {
                //新增
                (MainDBGrid.FindEditFormTemplateControl("fltDutyAttend") as ASPxFormLayout).ForEach(ProcessLayoutItem);
                sdsMaRout.Insert(); //Uncomment this line to allow updating.
            }
            else
            {
                //更改
                (MainDBGrid.FindEditFormTemplateControl("fltDutyAttend") as ASPxFormLayout).ForEach(ProcessLayoutItem);
                //ASPxFormLayout內無AGENDAID的對應元件, 須以下列取得AGENDAID值才能執行更新
                sdsMaRout.UpdateParameters["AGENDAID"].DefaultValue = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "AGENDAID").ToString();
                sdsMaRout.Update(); //Uncomment this line to allow updating.
            }
            MainDBGrid.CancelEdit();

        }

        private void ProcessLayoutItem(LayoutItemBase item)
        {
            LayoutItem layoutItem = item as LayoutItem;
            if (layoutItem != null)
            {
                //Control control = layoutItem.GetNestedControl();
                ASPxEditBase editBase = layoutItem.GetNestedControl() as ASPxEditBase;
                if ((editBase != null) && (editBase.Value != null))
                {
                    if (MainDBGrid.IsNewRowEditing)
                    {
                        //新增
                        if (sdsMaRout.InsertParameters[layoutItem.FieldName] != null)
                        {
                            sdsMaRout.InsertParameters[layoutItem.FieldName].DefaultValue = editBase.Value.ToString();
                            if (layoutItem.FieldName == "ATTENDDUTYTXT")
                            {
                                TimeSpan kk;
                                if (TimeSpan.TryParse(editBase.Value.ToString(), out kk))
                                {
                                    sdsMaRout.InsertParameters["ATTENDDUTYSEC"].DefaultValue = kk.TotalSeconds.ToString();
                                }
                            }

                        }
                    }
                    else
                    {
                        //更改
                        if (sdsMaRout.UpdateParameters[layoutItem.FieldName] != null)
                        {
                            sdsMaRout.UpdateParameters[layoutItem.FieldName].DefaultValue = editBase.Value.ToString();
                            if (layoutItem.FieldName == "ATTENDDUTYTXT")
                            {
                                TimeSpan kk;
                                if (TimeSpan.TryParse(editBase.Value.ToString(), out kk))
                                {
                                    sdsMaRout.UpdateParameters["ATTENDDUTYSEC"].DefaultValue = kk.TotalSeconds.ToString();
                                }
                            }
                        }
                    }
                }
                /*
                ASPxTextBox editBase = layoutItem.GetNestedControl() as ASPxTextBox;
                if (editBase != null)
                    sdsMaRout.UpdateParameters[layoutItem.FieldName].DefaultValue = editBase.Text;
                 */
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            MainDBGrid.CancelEdit();
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

        protected void MainDBGrid_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "UDutyhr")
            {
                if ((e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME").ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME").ToString() != ""))
                //if ((e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME") != null) && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME") != null))
                {
                    //e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME") != null 當物件null 會跳出
                    DateTime CurAGENDASTTIME = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME");
                    DateTime CurAGENDAEDTIME = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME");
                    TimeSpan CurAgendahr = CurAGENDAEDTIME - CurAGENDASTTIME;
                    e.Value = CurAgendahr.Hours + ":" + CurAgendahr.Minutes;
                    //e.Value = CurAgendahr.TotalHours;
                }
            }
            else if (e.Column.FieldName == "UDutySpan")
            {
                if ((e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM").ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYSTTM").ToString() != ""))
                {
                    //e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM") != null 當物件null 會跳出
                    DateTime CurDUTYSTTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYSTTM");
                    DateTime CurDUTYEDTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM");
                    object CurLATECAPTMM = e.GetListSourceFieldValue(e.ListSourceRowIndex, "LATECAPTMM");
                    e.Value = CurDUTYSTTM.ToString("HH:mm") + "~" + CurDUTYEDTM.ToString("HH:mm") + " + " + string.Format("{0:##0}", CurLATECAPTMM) + "分";
                    //e.Value = CurDUTYSTTM.ToString("HH:mm") + "+" + string.Format("{0:##0}", CurLATECAPTMM) + "分" + "~" + CurDUTYEDTM.ToString("HH:mm");
                }
            }
            else if (e.Column.FieldName == "UDutyStat")
            {
                if ((e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM").ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYSTTM").ToString() != ""))
                {
                    if (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAKDNO").ToString() != "28")
                    {
                        e.Value = "出勤";
                        if (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME").ToString() == "")
                            e.Value = "未出勤";
                        else if (e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME").ToString() == "")
                            e.Value = "出勤中";
                        else
                        {
                            DateTime CurAGENDASTTIME = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDASTTIME");
                            DateTime CurAGENDAEDTIME = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "AGENDAEDTIME");
                            double CurATTENDDUTYSEC = e.GetListSourceFieldValue(e.ListSourceRowIndex, "ATTENDDUTYSEC").ToString() == "" ? 0 : Convert.ToDouble(e.GetListSourceFieldValue(e.ListSourceRowIndex, "ATTENDDUTYSEC"));
                            DateTime CurDUTYSTTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYSTTM");
                            DateTime CurDUTYEDTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM");
                            //若Agenda.DUTYKDNO=null --> LATECAPTMM; VALIDDUTYHR 會是空值
                            object CurLATECAPTMM = e.GetListSourceFieldValue(e.ListSourceRowIndex, "LATECAPTMM");
                            double LATECAPTMM = (CurLATECAPTMM.ToString() == "" ? 0 : Convert.ToDouble(CurLATECAPTMM));
                            TimeSpan CurVALIDDUTYHR = CurDUTYEDTM - CurDUTYSTTM;
                            if (e.GetListSourceFieldValue(e.ListSourceRowIndex, "VALIDDUTYHR").ToString() != "")
                                CurVALIDDUTYHR = (TimeSpan)e.GetListSourceFieldValue(e.ListSourceRowIndex, "VALIDDUTYHR");
                            TimeSpan CurDutyhr = CurAGENDAEDTIME - CurAGENDASTTIME;
                            //TimeSpan CurDutyhr = CurDUTYEDTM - CurDUTYSTTM;
                            if (CurATTENDDUTYSEC > 0)
                                //優先使用CurATTENDDUTYSEC做為是否正常出勤的判斷
                                CurDutyhr = TimeSpan.FromSeconds(CurATTENDDUTYSEC);
                            if (CurAGENDASTTIME > CurDUTYSTTM.AddMinutes(LATECAPTMM))
                                e.Value = "遲到";
                            if (CurDutyhr < CurVALIDDUTYHR)
                                e.Value = "工時不足";
                            else
                                e.Value = "正常出勤";
                        }
                    }
                }
            }
            /*
            else if (e.Column.FieldName == "UDutydt")
            {
                if ((e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM").ToString() != "") && (e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYSTTM").ToString() != ""))
                {
                    //e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYEDTM") != null 當物件null 會跳出
                    DateTime CurDUTYSTTM = (DateTime)e.GetListSourceFieldValue(e.ListSourceRowIndex, "DUTYSTTM");
                    e.Value = CurDUTYSTTM.ToString("HH:mm") +"~"+ CurDUTYEDTM.ToString("HH:mm");
                }
            }
            */

        }

        protected void cbxBMAKDNO_Callback(object sender, CallbackEventArgsBase e)
        {
            FillcbxBMAKDNOCombo(e.Parameter);
        }

        protected void FillcbxBMAKDNOCombo(string CurMACDNO)
        {
            if (string.IsNullOrEmpty(CurMACDNO))
                return;
            Session["MACDNO"] = CurMACDNO;
            cbxBMAKDNO.DataBind();
        }

        protected void MainDBGrid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            if (MainDBGrid.IsNewRowEditing)
                //新增時可編輯內容
                return;
            if (e.Column.FieldName == "MANO")
                //更改時不可編輯內容
                e.Editor.ClientEnabled = false;
            else
            //else if ((e.Column.FieldName != "MROUTNM") && (e.Column.FieldName != "MROUTDESC") && (e.Column.FieldName != "BTCHPDQTY") && (e.Column.FieldName != "RTUNITNM"))
            {
                //更改時不可編輯內容
                //若Columns["MROUTID"] 設定 EditItemTemplate Control時, e.Column.FieldName不會出現
                ASPxTextBox edMROUTID = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["MROUTID"], "edMROUTID");
                edMROUTID.ClientEnabled = false;
            }
            /*
            if (MainDBGrid.IsEditing)
            {
                if (e.Column.FieldName == "PRSNNO")
                {
                    object val = null;
                    if (e.KeyValue == DBNull.Value || e.KeyValue == null)
                        //新增時
                        val = Session["CurDeptID"];
                    else
                        val = MainDBGrid.GetRowValuesByKeyValue(e.KeyValue, "DPTNO");
                    if (val == DBNull.Value)
                        return;
                    string DPTNO = (string)val;
                    ASPxComboBox combo = e.Editor as ASPxComboBox;
                    FillIndexCombo(combo, DPTNO);
                    combo.Callback += new CallbackEventHandlerBase(cmbPRSNNO_OnCallback);

                }
                else if (e.Column.FieldName == "AGENDAKDNO")
                {
                    ASPxComboBox cmbAGENDAKDNO = e.Editor as ASPxComboBox;
                    cmbAGENDAKDNO.Callback += new CallbackEventHandlerBase(cmbAGENDAKDNO_OnCallback);
                }
            }
            */
        }

        void cmbAGENDAKDNO_OnCallback(object source, CallbackEventArgsBase e)
        {
            //FillIndexCombo(source as ASPxComboBox, e.Parameter);
        }

        protected void MainDBGrid_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            e.NewValues["MROUTID"] = "00001";
            e.NewValues["BTCHPDQTY"] = 1;
        }

        protected void SubDBGrid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            int index = MainDBGrid.FindVisibleIndexByKeyValue(MasterKeyValue);
            ASPxGridView CurSubDBGrid = MainDBGrid.FindDetailRowTemplateControl(index, "SubDBGrid") as ASPxGridView;
            if (CurSubDBGrid.IsNewRowEditing)
                return;
            if (e.Column.FieldName == "MROUTSRNO")
                //if ((e.Column.FieldName == "MPROCID") || (e.Column.FieldName == "MROUTSRNO"))
                //更改時不可變動
                e.Editor.ClientEnabled = false;
        }

        /*
        void cmbPRSNNO_OnCallback(object source, CallbackEventArgsBase e)
        {
            FillIndexCombo(source as ASPxComboBox, e.Parameter);
        }

        protected void FillIndexCombo(ASPxComboBox cmb, string CurDPTNO)
        {
            if (string.IsNullOrEmpty(CurDPTNO))
                return;
            Session["DPTNO"] = CurDPTNO;
            cmb.DataBind();
        }
        */

        protected void SubDBGrid_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            string MasterKeyValue = (sender as ASPxGridView).GetMasterRowKeyValue().ToString();
            e.NewValues["MANO"] = MasterKeyValue.Split('|')[0].Trim();
            e.NewValues["MROUTID"] = MasterKeyValue.Split('|')[1].Trim();
            //抓編碼最大值
            string NewMROUTSRNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "MaRoutProc", "MROUTSRNO", "MANO='" + e.NewValues["MANO"] + "' AND MROUTID='" + e.NewValues["MROUTID"] + "'", true, "###", Convert.ToDateTime(null), "");
            e.NewValues["MROUTSRNO"] = NewMROUTSRNO;
            e.NewValues["MATHOPTR"] = "*";
            e.NewValues["PN2RTEXGRATE"] = "1";

            //e.NewValues["LOGID"] = Session["CurUserID"];
            //e.NewValues["LOGDT"] = DateTime.Now;
        }

        protected void cbxRMROUTID_Callback(object sender, CallbackEventArgsBase e)
        {
            FillRMROUTCombo(e.Parameter);
        }

        protected void FillRMROUTCombo(string CurMANO)
        {
            if (string.IsNullOrEmpty(CurMANO))
                return;
            Session["MANO"] = CurMANO;
            cbxRMROUTID.DataBind();
        }

        protected void btnMRoutDupl_Click(object sender, EventArgs e)
        {
            if (cbxRTARGMANO.Value == null)
            {
                lblErrorMessage.Text = "尚未選擇目標-產品 !!";
                cbxRTARGMANO.Focus();
                return;
            }
            else if (cbxRMROUTID.Value == null)
            {
                lblErrorMessage.Text = "尚未選擇途程 !!";
                cbxRMROUTID.Focus();
                return;
            }
            else if (cbxRTARGMROUTID.Value == null)
            {
                lblErrorMessage.Text = "尚未輸入目標-途程 !!";
                cbxRTARGMROUTID.Focus();
                return;
            }
            string SourMANO = "";
            string lblErrorMessageText = "";
            SourMANO = cbxRMANO.Value.ToString();
            /*
            sdsMaRout.SelectParameters["MANO"].DefaultValue = cbxRMANO.Value.ToString();
            sdsMaRout.SelectParameters["MROUTID"].DefaultValue = cbxRMROUTID.Value.ToString();
            sdsMaRout.SelectCommandType = SqlDataSourceCommandType.Text;
            DataTable dtbMaRout = ((DataView)sdsMaRout.Select(new DataSourceSelectArguments())).ToTable();
             */
            //新增製途
            object CurBTCHPDQTY = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "BTCHPDQTY");
            object ojtTSPREPHMANTICK = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "TSPREPHMANTICK");
            object ojtTSPREPMACHTICK = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "TSPREPMACHTICK");
            object ojtTSBTCHHMANTICK = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "TSBTCHHMANTICK");
            object ojtTSBTCHMACHTICK = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "TSBTCHMACHTICK");
            double PUTNQTYScale = 1;
            if ((CurBTCHPDQTY != null) && (Convert.ToDouble(CurBTCHPDQTY) > 0))
                //計算比例
                PUTNQTYScale = Convert.ToDouble(tbxRTARGBTCHPDQTY.Value) / Convert.ToDouble(CurBTCHPDQTY);
            //
            sdsMaRoutProc.SelectParameters["MANO"].DefaultValue = cbxRMANO.Value.ToString();
            sdsMaRoutProc.SelectParameters["MROUTID"].DefaultValue = cbxRMROUTID.Value.ToString();
            //sdsWoRoutProc.SelectParameters["MPROCID"].DefaultValue = fltWoRoutProc.GetNestedControlValueByFieldName("MPROCID").ToString();
            sdsMaRoutProc.SelectCommandType = SqlDataSourceCommandType.Text;

            string TargMANO = cbxRTARGMANO.Value.ToString();
            string TargMROUTID = cbxRTARGMROUTID.Value.ToString();
            sdsMaRout.InsertParameters["MANO"].DefaultValue = TargMANO;
            sdsMaRout.InsertParameters["MROUTID"].DefaultValue = TargMROUTID;
            sdsMaRout.InsertParameters["MROUTNM"].DefaultValue = cbxRMROUTID.Text + "-" + TargMROUTID;
            sdsMaRout.InsertParameters["BTCHPDQTY"].DefaultValue = tbxRTARGBTCHPDQTY.Value.ToString();
            sdsMaRout.InsertParameters["TSPREPHMANTICK"].DefaultValue = ojtTSPREPHMANTICK == null ? "0" : ojtTSPREPHMANTICK.ToString();
            sdsMaRout.InsertParameters["TSPREPMACHTICK"].DefaultValue = ojtTSPREPMACHTICK == null ? "0"  : ojtTSPREPMACHTICK.ToString();
            sdsMaRout.InsertParameters["TSBTCHHMANTICK"].DefaultValue = ojtTSBTCHHMANTICK  == null ? "0"  : ojtTSBTCHHMANTICK.ToString();
            sdsMaRout.InsertParameters["TSBTCHMACHTICK"].DefaultValue = ojtTSBTCHMACHTICK  == null ? "0"  : ojtTSBTCHMACHTICK.ToString();
            sdsMaRout.Insert();

            //新增製途程序
            double PFNHQTY = 0;
            DataTable dtbMaRoutProc = ((DataView)sdsMaRoutProc.Select(new DataSourceSelectArguments())).ToTable();
            for (int i = 0; i < dtbMaRoutProc.Rows.Count; i++)
            {
                //抓編碼最大值
                string NewMROUTSRNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "MaRoutProc", "MROUTSRNO", "MANO='" + TargMANO + "' AND MROUTID='" + TargMROUTID + "'", true, "###", Convert.ToDateTime(null), "");
                sdsMaRoutProc.InsertParameters["MANO"].DefaultValue = TargMANO;
                sdsMaRoutProc.InsertParameters["MROUTID"].DefaultValue = TargMROUTID;
                sdsMaRoutProc.InsertParameters["MROUTSRNO"].DefaultValue = NewMROUTSRNO;
                sdsMaRoutProc.InsertParameters["MPROCID"].DefaultValue = dtbMaRoutProc.Rows[i]["MPROCID"].ToString();
                sdsMaRoutProc.InsertParameters["PUTNINPTFORMAT"].DefaultValue = dtbMaRoutProc.Rows[i]["PUTNINPTFORMAT"].ToString();
                if (PFNHQTY > 0)
                    //=上一製程的完成量
                    sdsMaRoutProc.InsertParameters["PUTNQTY"].DefaultValue = PFNHQTY.ToString();
                else
                {
                    if (dtbMaRoutProc.Rows[i]["PUTNQTY"] != DBNull.Value)
                        sdsMaRoutProc.InsertParameters["PUTNQTY"].DefaultValue = (Convert.ToDouble(dtbMaRoutProc.Rows[i]["PUTNQTY"].ToString()) * PUTNQTYScale).ToString();
                }
                sdsMaRoutProc.InsertParameters["PNUNITNM"].DefaultValue = dtbMaRoutProc.Rows[i]["PNUNITNM"].ToString();
                sdsMaRoutProc.InsertParameters["MATHOPTR"].DefaultValue = dtbMaRoutProc.Rows[i]["MATHOPTR"].ToString();
                sdsMaRoutProc.InsertParameters["PN2RTEXGRATE"].DefaultValue = dtbMaRoutProc.Rows[i]["PN2RTEXGRATE"].ToString();
                int PROCFMULAOPT = 0;
                string strPNPARA1 = "";
                string strPNPARA2 = "";
                string strPHPARA1 = "";
                string strPHPARA2 = "";
                string strPNMATHOPTR = "";
                string strPHMATHOPTR = "";
                string valMATHOPTR = "";
                if (dtbMaRoutProc.Rows[i]["PROCFMULAOPT"] != DBNull.Value)
                {
                    sdsMaRoutProc.InsertParameters["PROCFMULAOPT"].DefaultValue = dtbMaRoutProc.Rows[i]["PROCFMULAOPT"].ToString();
                    PROCFMULAOPT = Convert.ToInt16(dtbMaRoutProc.Rows[i]["PROCFMULAOPT"]);
                }
                switch (PROCFMULAOPT)
                {
                    case 0:
                        if ((dtbMaRoutProc.Rows[i]["MATHOPTR"].ToString() != "") && (dtbMaRoutProc.Rows[i]["PN2RTEXGRATE"].ToString() != ""))
                        {
                            //計算下一製程的投入量
                            PFNHQTY = DBUtility.CalcByDataTable(sdsMaRoutProc.InsertParameters["PUTNQTY"].DefaultValue + sdsMaRoutProc.InsertParameters["MATHOPTR"].DefaultValue + sdsMaRoutProc.InsertParameters["PN2RTEXGRATE"].DefaultValue);
                            sdsMaRoutProc.InsertParameters["PFNHQTY"].DefaultValue = PFNHQTY.ToString();
                        }
                        break;
                    case 1:
                        //1.單-單-寬
                        //投產公式:  1 --> A  MATHOPTR C
                        //  1300  / 48 * PUTNQTY   (1300mm 投入 PUTNQTY支)
                        //string strPNPARA2 = "";
                        //string strPHPARA2 = "";
                        if (dtbMaRoutProc.Rows[i]["PNPARA1"] != DBNull.Value)
                        {
                            strPNPARA1 = dtbMaRoutProc.Rows[i]["PNPARA1"].ToString();
                            sdsMaRoutProc.InsertParameters["PNPARA1"].DefaultValue = strPNPARA1;
                        }
                        if (dtbMaRoutProc.Rows[i]["PHPARA1"] != DBNull.Value)
                        {
                            strPHPARA1 = dtbMaRoutProc.Rows[i]["PHPARA1"].ToString();
                            sdsMaRoutProc.InsertParameters["PHPARA1"].DefaultValue = strPHPARA1;
                        }
                        if (dtbMaRoutProc.Rows[i]["MATHOPTR"] != DBNull.Value)
                        {
                            valMATHOPTR = dtbMaRoutProc.Rows[i]["MATHOPTR"].ToString();
                            sdsMaRoutProc.InsertParameters["MATHOPTR"].DefaultValue = valMATHOPTR;
                        }
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
                            PFNHQTY = Math.Floor(DBUtility.CalcByDataTable(dblPFNHOpnt1 + valMATHOPTR + dblPFNHOpnt2));
                            //PUTNQTY 覆捲 : 投入支數, 預設 1
                            string strPUTNQTY = dtbMaRoutProc.Rows[i]["PUTNQTY"].ToString() == "" ? "1" : dtbMaRoutProc.Rows[i]["PUTNQTY"].ToString();
                            PFNHQTY = DBUtility.CalcByDataTable(PFNHQTY + "*" + strPUTNQTY);
                            sdsMaRoutProc.InsertParameters["PFNHQTY"].DefaultValue = PFNHQTY.ToString();
                        }
                        break;
                    case 2:
                        //1.單-單-長
                        //投產公式: 2-- > B  MATHOPTR D
                        //  1000 / 100 * PUTNQTY(1000M 投入 PUTNQTY支)
                        //string strPNPARA2 = "";
                        //string strPHPARA2 = "";
                        if (dtbMaRoutProc.Rows[i]["PNPARA2"] != DBNull.Value)
                        {
                            strPNPARA2 = dtbMaRoutProc.Rows[i]["PNPARA2"].ToString();
                            sdsMaRoutProc.InsertParameters["PNPARA2"].DefaultValue = strPNPARA2;
                        }
                        if (dtbMaRoutProc.Rows[i]["PHPARA2"] != DBNull.Value)
                        {
                            strPHPARA2 = dtbMaRoutProc.Rows[i]["PHPARA2"].ToString();
                            sdsMaRoutProc.InsertParameters["PHPARA2"].DefaultValue = strPHPARA2;
                        }
                        if (dtbMaRoutProc.Rows[i]["MATHOPTR"] != DBNull.Value)
                        {
                            valMATHOPTR = dtbMaRoutProc.Rows[i]["MATHOPTR"].ToString();
                            sdsMaRoutProc.InsertParameters["MATHOPTR"].DefaultValue = valMATHOPTR;
                        }
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
                            PFNHQTY = Math.Floor(DBUtility.CalcByDataTable(dblPFNHOpnt1 + valMATHOPTR + dblPFNHOpnt2));
                            //PUTNQTY 覆捲 : 投入支數, 預設 1
                            string strPUTNQTY = dtbMaRoutProc.Rows[i]["PUTNQTY"].ToString() == "" ? "1" : dtbMaRoutProc.Rows[i]["PUTNQTY"].ToString();
                            PFNHQTY = DBUtility.CalcByDataTable(PFNHQTY + "*" + strPUTNQTY);
                            sdsMaRoutProc.InsertParameters["PFNHQTY"].DefaultValue = PFNHQTY.ToString();
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
                        if (dtbMaRoutProc.Rows[i]["PNPARA1"] != DBNull.Value)
                        {
                            strPNPARA1 = dtbMaRoutProc.Rows[i]["PNPARA1"].ToString();
                            sdsMaRoutProc.InsertParameters["PNPARA1"].DefaultValue = strPNPARA1;
                        }
                        if (dtbMaRoutProc.Rows[i]["PNPARA2"] != DBNull.Value)
                        {
                            strPNPARA2 = dtbMaRoutProc.Rows[i]["PNPARA2"].ToString();
                            sdsMaRoutProc.InsertParameters["PNPARA1"].DefaultValue = strPNPARA2;
                        }

                        if (dtbMaRoutProc.Rows[i]["PHPARA1"] != DBNull.Value)
                        {
                            strPHPARA1 = dtbMaRoutProc.Rows[i]["PHPARA1"].ToString();
                            sdsMaRoutProc.InsertParameters["PHPARA1"].DefaultValue = strPHPARA1;
                            //strPHPARA1 = Convert.ToInt16(dtbWoRoutProc.Rows[0]["PHPARA1"]);
                        }
                        if (dtbMaRoutProc.Rows[i]["PHPARA2"] != DBNull.Value)
                        {
                            strPHPARA2 = dtbMaRoutProc.Rows[i]["PHPARA2"].ToString();
                            sdsMaRoutProc.InsertParameters["PHPARA2"].DefaultValue = strPHPARA2;
                            //strPHPARA2 = Convert.ToInt16(dtbWoRoutProc.Rows[0]["PHPARA2"]);
                        }
                        if (dtbMaRoutProc.Rows[i]["PNMATHOPTR"] != DBNull.Value)
                        {
                            strPNMATHOPTR = dtbMaRoutProc.Rows[i]["PNMATHOPTR"].ToString();
                            sdsMaRoutProc.InsertParameters["PNMATHOPTR"].DefaultValue = strPNMATHOPTR;
                        }
                        if (dtbMaRoutProc.Rows[i]["PHMATHOPTR"] != DBNull.Value)
                        {
                            strPHMATHOPTR = dtbMaRoutProc.Rows[i]["PHMATHOPTR"].ToString();
                            sdsMaRoutProc.InsertParameters["PHMATHOPTR"].DefaultValue = strPHMATHOPTR;
                        }
                        if (dtbMaRoutProc.Rows[i]["MATHOPTR"] != DBNull.Value)
                        {
                            valMATHOPTR = dtbMaRoutProc.Rows[i]["MATHOPTR"].ToString();
                            sdsMaRoutProc.InsertParameters["MATHOPTR"].DefaultValue = valMATHOPTR;
                        }
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
                            string strPUTNQTY = dtbMaRoutProc.Rows[i]["PUTNQTY"].ToString() == "" ? "1" : dtbMaRoutProc.Rows[i]["PUTNQTY"].ToString();
                            PFNHQTY = DBUtility.CalcByDataTable("(" + dblPFNHOpnt1 + valMATHOPTR + dblPFNHOpnt2 + ")" + "*" + strPUTNQTY);
                            sdsMaRoutProc.InsertParameters["PFNHQTY"].DefaultValue = PFNHQTY.ToString();
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
                }  //switch (PROCFMULAOPT)

                sdsMaRoutProc.InsertParameters["FNUNITNM"].DefaultValue = dtbMaRoutProc.Rows[i]["FNUNITNM"].ToString();
                sdsMaRoutProc.InsertParameters["FNSHSPEC"].DefaultValue = dtbMaRoutProc.Rows[i]["FNSHSPEC"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["FNSHSPEC"].ToString();
                //if (dtbMaRoutProc.Rows[i]["FNSHSPEC"] != DBNull.Value)
                //    sdsMaRoutProc.InsertParameters["FNSHSPEC"].DefaultValue = dtbMaRoutProc.Rows[i]["FNSHSPEC"].ToString();
                sdsMaRoutProc.InsertParameters["SPREPHMANTICK"].DefaultValue = dtbMaRoutProc.Rows[i]["SPREPHMANTICK"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["SPREPHMANTICK"].ToString();
                sdsMaRoutProc.InsertParameters["SPREPMACHTICK"].DefaultValue = dtbMaRoutProc.Rows[i]["SPREPMACHTICK"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["SPREPMACHTICK"].ToString();
                sdsMaRoutProc.InsertParameters["SBTCHHMANTICK"].DefaultValue = dtbMaRoutProc.Rows[i]["SBTCHHMANTICK"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["SBTCHHMANTICK"].ToString();
                sdsMaRoutProc.InsertParameters["SBTCHMACHTICK"].DefaultValue = dtbMaRoutProc.Rows[i]["SBTCHMACHTICK"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["SBTCHMACHTICK"].ToString();

                sdsMaRoutProc.InsertParameters["SPREPHMANTM"].DefaultValue = dtbMaRoutProc.Rows[i]["SPREPHMANTM"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["SPREPHMANTM"].ToString();
                sdsMaRoutProc.InsertParameters["SPREPMACHTM"].DefaultValue = dtbMaRoutProc.Rows[i]["SPREPMACHTM"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["SPREPMACHTM"].ToString();
                sdsMaRoutProc.InsertParameters["SBTCHHMANTM"].DefaultValue = dtbMaRoutProc.Rows[i]["SBTCHHMANTM"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["SBTCHHMANTM"].ToString();
                sdsMaRoutProc.InsertParameters["UNITCOST"].DefaultValue = "0";
                sdsMaRoutProc.InsertParameters["FNSHVALDMD"].DefaultValue = dtbMaRoutProc.Rows[i]["FNSHVALDMD"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["FNSHVALDMD"].ToString();
                sdsMaRoutProc.InsertParameters["FNSHTLRNRATE"].DefaultValue = dtbMaRoutProc.Rows[i]["FNSHTLRNRATE"] == DBNull.Value ? null : dtbMaRoutProc.Rows[i]["FNSHTLRNRATE"].ToString();
                sdsMaRoutProc.Insert();
            }
            /*
            //加入材料清單
            //GetMaBom : 取得最底階的所有實體材料清單(不限階數)
            string CurMaList = DBUtility.GetMaBom(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cbxRMANO.Value.ToString());
            string WhereSetStr = "";
            if (CurMaList.IndexOf(",") >= 0)
                WhereSetStr = "MaBom.MANO IN (" + CurMaList + ")";
            else
                WhereSetStr = "MaBom.MANO = " + CurMaList + " ";

            if (sdsMaBom.SelectCommand.IndexOf("ORDER BY") >= 0)
                sdsMaBom.SelectCommand = sdsMaBom.SelectCommand.Substring(0, sdsMaBom.SelectCommand.IndexOf("ORDER BY") - 1);
            if (sdsMaBom.SelectCommand.IndexOf("WHERE ") >= 0)
                sdsMaBom.SelectCommand = sdsMaBom.SelectCommand.Substring(0, sdsMaBom.SelectCommand.IndexOf("WHERE "));
            if (WhereSetStr != "")
                sdsMaBom.SelectCommand += " WHERE " + WhereSetStr;
            sdsMaBom.SelectCommand += " ORDER BY MaBom.MANO,MaBom.SUBMANO";
            sdsMaBom.SelectCommandType = SqlDataSourceCommandType.Text;
            DataTable dtbMaBom = ((DataView)sdsMaBom.Select(new DataSourceSelectArguments())).ToTable();
            for (int i = 0; i < dtbMaBom.Rows.Count; i++)
            {
                //抓編碼最大值
                string NewWOITNO = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "WoWip", "WOITNO", "WONO='" + CurWONO + "'", true, "###", Convert.ToDateTime(null), "");
                sdsWoWip.InsertParameters["WONO"].DefaultValue = CurWONO;
                sdsWoWip.InsertParameters["WOITNO"].DefaultValue = NewWOITNO;
                sdsWoWip.InsertParameters["MANO"].DefaultValue = dtbMaBom.Rows[i]["MANO"].ToString();
                sdsWoWip.InsertParameters["NEEDQTY"].DefaultValue = (Convert.ToDouble(dtbMaBom.Rows[i]["LEVELQTY"].ToString()) * PUTNQTYScale).ToString();
                sdsWoWip.InsertParameters["RTUNITNM"].DefaultValue = dtbMaBom.Rows[i]["RTUNITNM"].ToString();
                sdsWoWip.InsertParameters["HANDQTY"].DefaultValue = "0";
                sdsWoWip.InsertParameters["USEDQTY"].DefaultValue = "0";
                sdsWoWip.InsertParameters["MPROCID"].DefaultValue = dtbMaBom.Rows[i]["MPROCID"].ToString();
                sdsWoWip.Insert();
            }
            */
            lblErrorMessage.Text = lblErrorMessageText + "複製完成!!";
        }


        protected void cbkPanelMRoutDupl_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter == "cbxRTARGMANO")
            {
                //抓編碼最大值
                string NewMROUTID = DBUtility.RAdoGetFormatCode(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, "MaRout", "MROUTID", "MANO='" + cbxRTARGMANO.Value + "'", true, "#####", Convert.ToDateTime(null), "");
                cbxRTARGMROUTID.Value = NewMROUTID;
            }
        }

        protected void SubDBGrid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            e.NewValues["MANO"] = Session["MANO"];
            e.NewValues["MROUTID"] = Session["MROUTID"];
            //e.NewValues["MANO"] = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "MANO");
            //e.NewValues["MROUTID"] = MainDBGrid.GetRowValues(MainDBGrid.FocusedRowIndex, "MROUTID");
        }

        protected void cbxRTARGMANO_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
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

        protected void cbxRTARGMANO_ItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
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


        protected void MainDBGrid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string CurMANO = e.Parameters;
            ASPxTextBox edMADESC = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["MADESC"], "edMADESC");
            ASPxTextBox edMASPEC = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["MASPEC"], "edMASPEC");
            ASPxTextBox edMROUTID = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["MROUTID"], "edMROUTID");
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

        protected void MainDBGrid_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            //若Columns["MROUTID"] 設定 EditItemTemplate Control時, 在 Save時e.NewValues["MROUTID"]無法自動對應Columns["MROUTID"]的輸入值
            //取出 EditItemTemplate Control 輸入值 對應到e.NewValues["MROUTID"]
            ASPxTextBox edMROUTID = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl(MainDBGrid.Columns["MROUTID"] as GridViewDataColumn, "edMROUTID");
            e.NewValues["MROUTID"] = edMROUTID.Value;
        }

        protected void MainDBGrid_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            //set the popup edit form's Width size to the grid size when used with Docking Manager & Panels 
            ((ASPxPopupControl)e.EditForm.NamingContainer).ClientSideEvents.PopUp = string.Format(@"function(s, e) {{
             s.SetWidth(MainDBGrid.GetWidth()-15);
             s.UpdatePosition();

        }}");

            // If your popup is modal
            //ASPxPopupControl popup = e.EditForm.NamingContainer as ASPxPopupControl;
            //popup.ScrollBars = ScrollBars.Vertical;
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

        /*
        protected void SubDBGrid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string CurPUTNQTY = e.Parameters;
            ASPxTextBox edMATHOPTR = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["MATHOPTR"], "edMATHOPTR");
            ASPxTextBox edPN2RTEXGRATE = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["PN2RTEXGRATE"], "edPN2RTEXGRATE");
            ASPxTextBox edPFNHQTY = (ASPxTextBox)MainDBGrid.FindEditRowCellTemplateControl((GridViewDataTextColumn)MainDBGrid.Columns["PFNHQTY"], "edPFNHQTY");
            float PFNHQTY = 0;
            if ((edMATHOPTR.Text != "") && (edPN2RTEXGRATE.Text != ""))
            {
                //計算下一製程的投入量
                PFNHQTY = DBUtility.CalcByDataTable(CurPUTNQTY + edMATHOPTR.Text + edPN2RTEXGRATE.Text);
            }
            edPFNHQTY.Text = PFNHQTY.ToString();
        }
        */

    }
}