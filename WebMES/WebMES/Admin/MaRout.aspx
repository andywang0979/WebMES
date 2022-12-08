<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaRout.aspx.cs" Inherits="WebMES.Admin.MaRout" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/Admin/ManuMenuPanel.ascx" TagPrefix="uc1" TagName="ManuMenuPanel" %>
<%@ Register Assembly="DevExpress.Web.ASPxScheduler.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxScheduler.Controls" TagPrefix="dxwschsc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0" />
    <title></title>
    <style>
        body, form {
            padding: 0;
            margin: 0;
            overflow: hidden;
            min-height: 240px;
            min-width: 340px;
            font-weight: 700;
        }

        .title {
            float: left;
            padding: 1px 4px 2px;
            font-size: 2.2em;
        }

        .main-menu {
            float: right !important;
            margin: 8px 0 4px;
        }

        .grid,
        .grid .dxgvHSDC,
        .grid .dxgvCSD {
            border-left: 0 !important;
            border-right: 0 !important;
            border-bottom: 0 !important;
        }

        .auto-style1 {
            width: 3%;
        }

        .auto-style2 {
            width: 15%;
        }

        .auto-style3 {
            width: 20%;
            height: 12px;
        }

        .auto-style5 {
            width: 55%;
            height: 5px;
        }

        .auto-style6 {
            height: 5px;
            width: 20%;
        }

        .auto-style7 {
            width: 60%;
            height: 15px;
        }

        .auto-style8 {
            width: 15%;
            height: 15px;
        }
    </style>
    <script type="text/javascript">

        function calcPFNHQTY() {
            //var strPUTNQTY = SubDBGrid.GetEditor("PUTNINPTFORMAT").GetText();
            ////var edFNSHSPEC = SubDBGrid.GetEditor("FNSHSPEC");
            ////edFNSHSPEC.SetText(strPUTNQTY.replace(/_/g, ''));

            var valPUTNQTY;
            //if (strPUTNQTY === "")
            //    valPUTNQTY = SubDBGrid.GetEditor("PUTNQTY").GetText();
            //else
            //    去掉_
            //    valPUTNQTY = eval(strPUTNQTY.replace(/_/g, ''));
            //var edPUTNQTY = SubDBGrid.GetEditor("PUTNQTY");
            //edPUTNQTY.SetText(valPUTNQTY);
            valPUTNQTY = SubDBGrid.GetEditor("PUTNQTY").GetText();

            var valMATHOPTR = SubDBGrid.GetEditor("MATHOPTR").GetText();
            var valPN2RTEXGRATE = SubDBGrid.GetEditor("PN2RTEXGRATE").GetText();
            var edPFNHQTY = SubDBGrid.GetEditor("PFNHQTY");
            var strPFNHQTY = (valPUTNQTY + valMATHOPTR + valPN2RTEXGRATE);
            //var str = (PUTNQTY + MATHOPTR + PN2RTEXGRATE).replace(/[^-()d/*+.]/g, '');
            edPFNHQTY.SetText(eval(strPFNHQTY));
        }

        function pclMRoutDuplPanelTargMANOSet() {
            //lblRPBEGDT.SetValue("製令單號");
            //tbxRWONO.SetVisible(true);
            //tbxRSTPBEGDT.SetVisible(false);
            //lblRPBEGDTDASH.SetVisible(false);
            //tbxREDPBEGDT.SetVisible(false);
            MainDBGrid.GetRowValues(MainDBGrid.GetFocusedRowIndex(), 'MANO;MROUTID;BTCHPDQTY', MainDBGridOnGetRowValuesMANOS);
            //MainDBGrid.GetRowValues(MainDBGrid.GetFocusedRowIndex(), 'MANO', MainDBGridOnGetRowValuesMANO);
            //MainDBGrid.GetRowValues(MainDBGrid.GetFocusedRowIndex(), 'MROUTID', MainDBGridOnGetRowValuesMROUTID);
            //MainDBGrid.GetRowValues(MainDBGrid.GetFocusedRowIndex(), 'BTCHPDQTY', MainDBGridOnGetRowValuesTARGBTCHPDQTY);
        }

        function MainDBGridOnGetRowValuesMANOS(Values) {
            cbxRMANO.SetValue(Values[0]);
            //設定Combo內容:產品代碼的所有製途
            cbxRMROUTID.PerformCallback(Values[0]);
            cbxRMROUTID.SetValue(Values[1]);
            tbxRTARGBTCHPDQTY.SetValue(Values[2]);
        }

        function MainDBGridOnGetRowValuesMANO(Value) {
            cbxRMANO.SetValue(Value);
            //設定Combo內容:產品代碼的所有製途
            cbxRMROUTID.PerformCallback(Value);
        }

        function MainDBGridOnGetRowValuesMROUTID(Value) {
            cbxRMROUTID.SetValue(Value);
            //cbxRMROUTID.EndUpdate();
        }

        function MainDBGridOnGetRowValuesTARGBTCHPDQTY(Value) {
            tbxRTARGBTCHPDQTY.SetValue(Value);
        }

        function OnBMACDNOChanged(cbxBMACDNO) {
            cbxBMAKDNO.PerformCallback(cbxBMACDNO.GetSelectedItem().value.toString());
        }

        function OnRMANOChanged(cbxRMANO) {
            //設定Combo內容:產品代碼的所有製途
            cbxRMROUTID.PerformCallback(cbxRMANO.GetSelectedItem().value.toString());
            //預設製令數量 BTCHPDQTY
            cbkPanelWoqty.PerformCallback(cbxRMANO.GetSelectedItem().value.toString());
        }

        var lastDPTNO = null;
        function OnDPTNOChanged(cbxDPTNO) {
            if (MainDBGrid.GetEditor("PRSNNO").InCallback())
                lastDPTNO = cbxDPTNO.GetValue().toString();
            else
                MainDBGrid.GetEditor("PRSNNO").PerformCallback(cbxDPTNO.GetValue().toString());
        }
        function OnEndCallback(s, e) {
            if (lastDPTNO) {
                cmbPRSNNO.PerformCallback(lastDPTNO);
                lastDPTNO = null;
            }
        }

        function OnListBoxValueChanged() {
            MainDBGrid.Refresh();
        }
        function UpdateGridHeight() {
            MainDBGrid.SetHeight(0);
            var containerHeight = ASPxClientUtils.GetDocumentClientHeight();
            if (document.body.scrollHeight > containerHeight)
                containerHeight = document.body.scrollHeight;
            MainDBGrid.SetHeight(containerHeight - topPanel.GetHeight() - MainDBGridMenu.GetHeight());
        }

        //偵測螢幕寬度自動調整ASPxPopupControl的顯示寬度 = 螢幕寬度 * 0.5, 以便適應手機平板電腦的螢幕寬度
        //在 pclSearchPanel的 Init 事件裏 設定執行pclSearchPanelInit();
        function pclSearchPanelInit(s, e) {
            pclSearchPanelAdjustSize();
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                pclSearchPanelAdjustSize();
                if (pclSearchPanel.IsVisible())
                    pclSearchPanel.UpdatePosition();
            });
        }

        function pclSearchPanelAdjustSize() {
            var width = Math.max(0, document.documentElement.clientWidth) * 0.5;
            pclSearchPanel.SetWidth(width);
        }

        function pclEditPagePanelInit(s, e) {
            pclEditPagePanelAdjustSize();
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                pclEditPagePanelAdjustSize();
                if (pclEditPagePanel.IsVisible())
                    pclEditPagePanel.UpdatePosition();
            });
        }

        function pclEditPagePanelAdjustSize() {
            var width = Math.max(0, document.documentElement.clientWidth) * 0.85;
            var height = Math.max(0, document.documentElement.clientHeight) * 0.85;
            pclEditPagePanel.SetWidth(width);
            pclEditPagePanel.SetHeight(height);
        }

        /*
        function OnNavgInsertClick(s, e) {
            showPopup = true;
            //var width = Math.max(0, document.documentElement.clientWidth) * 0.8;
            //var height = Math.max(0, document.documentElement.clientHeight) * 0.8;
            //pclEditPagePanel.SetWidth(width);
            //pclEditPagePanel.SetHeight(height);
            pclEditPagePanel.SetContentUrl('MaRoutPage.aspx?&EditMode=1');
            pclEditPagePanel.SetHeaderText('技術服務 - 新增');
            pclEditPagePanel.Show();
        }

        function OnNavgEditClick(s, e) {
            MainDBGrid.GetRowValues(MainDBGrid.GetFocusedRowIndex(), 'MANO;MROUTID', OnNavgEditPopup);
        }

        function OnNavgEditPopup(values) {
            showPopup = true;
            //var width = Math.max(0, document.documentElement.clientWidth) * 0.8;
            //var height = Math.max(0, document.documentElement.clientHeight) * 0.8;
            //pclEditPagePanel.SetWidth(width);
            //pclEditPagePanel.SetHeight(height);
            pclEditPagePanel.SetContentUrl('MaRoutPage.aspx?EditMode=2&MANO=' + values[0] + '&MROUTID=' + values[1]);
            pclEditPagePanel.SetHeaderText('技術服務 - 編輯');
            pclEditPagePanel.Show();
        }

        function HidepclEditPagePanel(returnValue) {
            //window.location.reload(true);
            pclEditPagePanel.SetContentHtml(null);
            pclEditPagePanel.Hide();
            if (returnValue == 'btnSave') {
                //'btnSave' or 'btnCancel'
                MainDBGrid.Refresh();
            }
        }
        */

        function pclMRoutDuplPanelInit(s, e) {
            pclMRoutDuplPanelAdjustSize();
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                pclMRoutDuplPanelAdjustSize();
                if (pclMRoutDuplPanel.IsVisible())
                    pclMRoutDuplPanel.UpdatePosition();
            });
        }

        function pclMRoutDuplPanelAdjustSize() {
            var width = Math.max(0, document.documentElement.clientWidth) * 0.5;
            pclMRoutDuplPanel.SetWidth(width);
        }

    </script>
</head>
<body>
    <form id="KRF00" runat="server">
        <uc1:MainMenuPanel runat="server" ID="MainMenuPanel" />
        <uc1:ManuMenuPanel runat="server" ID="ManuMenuPanel" />
        <div>
            <dx:ASPxTextBox ID="SearchTextBox" runat="server" Style="float: left; vertical-align: middle; margin-top: 6px; margin-left: 6px; margin-right: 8px" ClientInstanceName="SearchTextBox" Width="30%" NullText="請輸入關鍵字">
            </dx:ASPxTextBox>
            <dx:ASPxMenu ID="MainDBGridMenu" runat="server" ShowAsToolbar="True" ShowPopOutImages="True" EnableTheming="True" EnableAdaptivity="True" OnItemClick="MainDBGridMenu_ItemClick" Width="50%">
                <ClientSideEvents ItemClick="function(s, e) {
var i = e.item.name;
e.processOnServer = false;
if (i==&quot;NavgFilter&quot;)
  pclSearchPanel.Show();
else if (i==&quot;NavgBackPage&quot;)
  window.history.back() ;
else if (i==&quot;NavgforwardPage&quot;)
  window.history.forward();
else if (i==&quot;NavgRefresh&quot;)
  MainDBGrid.Refresh();
else if (i.substring(0,9) ==&quot;NavgPrint&quot;)
  e.processOnServer = true;
else if (i.substring(0,13) ==&quot;NavgMRoutDupl&quot;)
  {
    pclMRoutDuplPanelTargMANOSet();
    pclMRoutDuplPanel.Show();
  }
}" />
                <Items>
                    <dx:MenuItem Name="NavgFilter" ToolTip="進階查詢" GroupName="DataBar">
                        <Image IconID="filter_crossdatasourcefiltering_32x32office2013"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem BeginGroup="true" Name="NavgRefresh" ToolTip="資料更新" GroupName="DataBar">
                        <Image IconID="scheduling_recurrence_32x32office2013"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem ToolTip="資料列印" GroupName="DataBar" Name="NavgPrint">
                        <Items>
                            <dx:MenuItem Text="Pdf" Name="NavgPrintToPdf">
                            </dx:MenuItem>
                            <dx:MenuItem Text="Xls" Name="NavgPrintToXls">
                            </dx:MenuItem>
                            <dx:MenuItem Name="NavgPrintToXls" Text="Xlsx">
                            </dx:MenuItem>
                            <dx:MenuItem Text="Rtf" Name="NavgPrintToRtf">
                            </dx:MenuItem>
                            <dx:MenuItem Text="Csv" Name="NavgPrintToCsv">
                            </dx:MenuItem>
                        </Items>
                        <Image IconID="print_print_32x32office2013"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem ToolTip="儲存" GroupName="DataBar" Visible="False">
                        <Image IconID="save_save_32x32"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem ToolTip="回到首頁" GroupName="NavgBar" AdaptivePriority="1" Name="NavgHomePage">
                        <Image IconID="navigation_home_32x32office2013"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem GroupName="NavgBar" ToolTip="回上一頁" AdaptivePriority="1" Name="NavgBackPage">
                        <Image IconID="navigation_backward_32x32office2013"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem GroupName="NavgBar" ToolTip="到下一頁" AdaptivePriority="1" Name="NavgForwardPage">
                        <Image IconID="navigation_forward_32x32office2013"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem Name="NavgMRoutDupl" ToolTip="複製產品製途">
                        <Image IconID="scheduling_groupbydate_32x32">
                        </Image>
                    </dx:MenuItem>
                </Items>
            </dx:ASPxMenu>
        </div>
        <dx:ASPxPanel ID="MainDBGridPanel" runat="server" ClientInstanceName="MainDBGridPanel" EnableTheming="True" Width="100%">
            <SettingsAdaptivity CollapseAtWindowInnerWidth="600" />
            <PanelCollection>
                <dx:PanelContent runat="server">
                    <dx:ASPxGridView ID="MainDBGrid" runat="server" ClientInstanceName="MainDBGrid" AutoGenerateColumns="False" DataSourceID="sdsMaRout" EnableTheming="True" KeyFieldName="MANO;MROUTID" CssClass="grid" Width="100%" KeyboardSupport="True" OnCustomColumnDisplayText="MainDBGrid_CustomColumnDisplayText" OnCustomUnboundColumnData="MainDBGrid_CustomUnboundColumnData" OnCellEditorInitialize="MainDBGrid_CellEditorInitialize" OnInitNewRow="MainDBGrid_InitNewRow" OnCustomCallback="MainDBGrid_CustomCallback" OnRowInserting="MainDBGrid_RowInserting" OnHtmlEditFormCreated="MainDBGrid_HtmlEditFormCreated">
                        <SettingsDetail AllowOnlyOneMasterRowExpanded="True" ShowDetailRow="True" />
                        <SettingsAdaptivity AdaptivityMode="HideDataCells" HideDataCellsAtWindowInnerWidth="600">
                            <AdaptiveDetailLayoutProperties>
                                <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="580" />
                            </AdaptiveDetailLayoutProperties>
                        </SettingsAdaptivity>
                        <Templates>
                            <DetailRow>
                                <dx:ASPxGridView ID="SubDBGrid" runat="server" ClientInstanceName="SubDBGrid" AutoGenerateColumns="False" DataSourceID="sdsMaRoutProc" EnableTheming="True" KeyFieldName="MANO;MROUTID;MROUTSRNO" CssClass="grid" Width="100%" KeyboardSupport="True" OnCellEditorInitialize="SubDBGrid_CellEditorInitialize" OnInitNewRow="SubDBGrid_InitNewRow" OnBeforePerformDataSelect="SubDBGrid_BeforePerformDataSelect" OnRowInserting="SubDBGrid_RowInserting">
                                    <SettingsAdaptivity AdaptivityMode="HideDataCells" HideDataCellsAtWindowInnerWidth="580">
                                        <AdaptiveDetailLayoutProperties ColCount="1">
                                        </AdaptiveDetailLayoutProperties>
                                    </SettingsAdaptivity>
                                    <SettingsPager NumericButtonCount="6" PageSize="20">
                                        <FirstPageButton Visible="True">
                                        </FirstPageButton>
                                        <LastPageButton Visible="True">
                                        </LastPageButton>
                                    </SettingsPager>
                                    <SettingsEditing EditFormColumnCount="3" UseFormLayout="True" NewItemRowPosition="Bottom">
                                    </SettingsEditing>
                                    <SettingsBehavior AllowSelectByRowClick="True" />
                                    <SettingsCommandButton>
                                        <NewButton ButtonType="Image">
                                            <Image Url="~/Images/Navigator/Grid_Add.png">
                                            </Image>
                                        </NewButton>
                                        <UpdateButton ButtonType="Image">
                                            <Image Url="~/Images/Navigator/Grid_Post.png">
                                            </Image>
                                        </UpdateButton>
                                        <CancelButton ButtonType="Image">
                                            <Image Url="~/Images/Navigator/Grid_Cancel.png">
                                            </Image>
                                        </CancelButton>
                                        <EditButton ButtonType="Image">
                                            <Image Url="~/Images/Navigator/Grid_Edit.png">
                                            </Image>
                                        </EditButton>
                                        <DeleteButton ButtonType="Image">
                                            <Image Url="~/Images/Navigator/Grid_Delete.png">
                                            </Image>
                                        </DeleteButton>
                                    </SettingsCommandButton>
                                    <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />
                                    <SettingsPopup>
                                        <EditForm AllowResize="True" Modal="True" Width="800px" />
                                        <HeaderFilter MinHeight="140px">
                                        </HeaderFilter>
                                    </SettingsPopup>
                                    <SettingsSearchPanel CustomEditorID="SearchTextBox" />
                                    <SettingsText CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" SearchPanelEditorNullText="請輸入關鍵字" CommandCancel="取消" CommandUpdate="存入" Title="員工履歷" />
                                    <EditFormLayoutProperties ColCount="3">
                                        <Items>
                                            <dx:GridViewColumnLayoutItem ColumnName="MPROCID">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="MROUTSRNO">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="FNSHSPEC">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="PUTNINPTFORMAT">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="PUTNQTY">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="PNUNITNM">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="MATHOPTR">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="PN2RTEXGRATE">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="PFNHQTY">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="FNUNITNM">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="SPREPHMANTM">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="SPREPMACHTM">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="UNITCOST">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="SBTCHHMANTM">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="SBTCHMACHTM">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColumnName="MLINEID">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="MPROCMD">
                                            </dx:GridViewColumnLayoutItem>
                                            <dx:EditModeCommandLayoutItem ColSpan="3" HorizontalAlign="Right">
                                            </dx:EditModeCommandLayoutItem>
                                        </Items>
                                    </EditFormLayoutProperties>
                                    <Columns>
                                        <dx:GridViewCommandColumn VisibleIndex="0" Width="6%" MaxWidth="60" MinWidth="75" Visible="False">
                                        </dx:GridViewCommandColumn>
                                        <dx:GridViewDataTextColumn FieldName="MPROCNM" VisibleIndex="3" Caption="製程名稱" Width="10%" Visible="False">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="MROUTSRNO" VisibleIndex="1" Caption="加工順序" Width="5%" MaxWidth="65" MinWidth="50">
                                            <HeaderStyle Wrap="True" />
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="MLINENM" VisibleIndex="23" Caption="線別名稱" Width="10%" Visible="False">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataComboBoxColumn Caption="製程" FieldName="MPROCID" VisibleIndex="2" Width="7%" MaxWidth="85" MinWidth="70">
                                            <PropertiesComboBox DataSourceID="sdsManuProc" TextField="MPROCNM" ValueField="MPROCID">
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataComboBoxColumn Caption="投入公式" FieldName="PROCFMULAOPT" VisibleIndex="4" Width="7%" MaxWidth="85" MinWidth="70">
                                            <PropertiesComboBox>
                                                <Items>
                                                    <dx:ListEditItem Text="標準" Value="0" />
                                                    <dx:ListEditItem Text="單-單-寬" Value="1" />
                                                    <dx:ListEditItem Text="單-單-長" Value="2" />
                                                    <dx:ListEditItem Text="雙-單" Value="3" />
                                                    <dx:ListEditItem Text="單-雙" Value="4" />
                                                    <dx:ListEditItem Text="雙-雙" Value="5" />
                                                    <dx:ListEditItem Text="雙-雙-寬長" Value="6" />
                                                </Items>
                                                <ClientSideEvents ValueChanged="function(s, e) {
  cbkPanelMANO.PerformCallback(s.GetValue());
}" />
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataTextColumn Caption="投入格式" FieldName="PUTNINPTFORMAT" VisibleIndex="5" Visible="False">
                                            <PropertiesTextEdit NullText="設定投入格式(長*寬)--可空白">
                                            </PropertiesTextEdit>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="投入參數1" FieldName="PNPARA1" VisibleIndex="6" Width="4%" MaxWidth="55" MinWidth="40">
                                            <PropertiesTextEdit ClientInstanceName="edPNPARA1">
                                                <ClientSideEvents ValueChanged="function(s, e) {clnPFNHQTYSet();}" />
                                            </PropertiesTextEdit>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="投入參數2" FieldName="PNPARA2" VisibleIndex="7" Width="4%" MaxWidth="55" MinWidth="40">
                                            <PropertiesTextEdit ClientInstanceName="edPNPARA2">
                                                <ClientSideEvents ValueChanged="function(s, e) {clnPFNHQTYSet();}" />
                                            </PropertiesTextEdit>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="投入數量" FieldName="PUTNQTY" ShowInCustomizationForm="True" VisibleIndex="8" MaxWidth="85" MinWidth="70" Width="7%">
                                            <PropertiesTextEdit>
                                                <ClientSideEvents ValueChanged="function(s, e) {
calcPFNHQTY();	
}" />
                                            </PropertiesTextEdit>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataComboBoxColumn Caption="投入單位" FieldName="PNUNITNM" ShowInCustomizationForm="True" VisibleIndex="9" Width="7%" MaxWidth="85" MinWidth="70">
                                            <PropertiesComboBox DataSourceID="sdsUnit" TextField="UNITNM" ValueField="UNITNM" DropDownStyle="DropDown">
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataComboBoxColumn Caption="*/" FieldName="MATHOPTR" VisibleIndex="10" Width="3%" MaxWidth="45" MinWidth="30">
                                            <PropertiesComboBox>
                                                <Items>
                                                    <dx:ListEditItem Text="*" Value="*" />
                                                    <dx:ListEditItem Text="/" Value="/" />
                                                </Items>
                                                <ClientSideEvents ValueChanged="function(s, e) {
calcPFNHQTY();	
}" />
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataTextColumn Caption="換算比率" FieldName="PN2RTEXGRATE" VisibleIndex="11" Width="4%" MaxWidth="55" MinWidth="45" Visible="False">
                                            <PropertiesTextEdit>
                                                <ClientSideEvents ValueChanged="function(s, e) {
calcPFNHQTY();	
}" />
                                            </PropertiesTextEdit>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="產出規格1" FieldName="PHPARA1" VisibleIndex="12" Width="4%" MaxWidth="55" MinWidth="40">
                                            <PropertiesTextEdit ClientInstanceName="edPHPARA1">
                                                <ClientSideEvents ValueChanged="function(s, e) {clnPFNHQTYSet();}" />
                                            </PropertiesTextEdit>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="產出規格2" FieldName="PHPARA2" VisibleIndex="13" Width="4%" MaxWidth="55" MinWidth="40">
                                            <PropertiesTextEdit ClientInstanceName="edPHPARA2">
                                                <ClientSideEvents ValueChanged="function(s, e) {clnPFNHQTYSet();}" />
                                            </PropertiesTextEdit>
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="預計數量" FieldName="PFNHQTY" ShowInCustomizationForm="True" VisibleIndex="14" Width="7%" MaxWidth="85" MinWidth="70">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataComboBoxColumn Caption="產出單位" FieldName="FNUNITNM" VisibleIndex="15" Width="7%" MaxWidth="85" MinWidth="70">
                                            <PropertiesComboBox DataSourceID="sdsUnit" TextField="UNITNM" ValueField="UNITNO" DropDownStyle="DropDown">
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataTextColumn Caption="完成規格" FieldName="FNSHSPEC" ShowInCustomizationForm="True" VisibleIndex="16" Width="6%" MaxWidth="75" MinWidth="60">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="準備人時" FieldName="SPREPHMANTM" VisibleIndex="17" Width="6%" MaxWidth="75" MinWidth="60">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="加工機時" FieldName="SBTCHMACHTM" VisibleIndex="20" Width="6%" MaxWidth="75" MinWidth="60">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="加工人時" FieldName="SBTCHHMANTM" VisibleIndex="19" Width="6%" MaxWidth="75" MinWidth="60">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="準備機時" FieldName="SPREPMACHTM" VisibleIndex="18" Width="6%" MaxWidth="75" MinWidth="60">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn Caption="製程成本" FieldName="UNITCOST" VisibleIndex="21" Width="6%" MaxWidth="75" MinWidth="60">
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataComboBoxColumn Caption="線別" FieldName="MLINEID" VisibleIndex="22" Width="6%" MaxWidth="75" MinWidth="60">
                                            <PropertiesComboBox DataSourceID="sdsManuLine" TextField="MLINENM" ValueField="MLINEID">
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                        <dx:GridViewDataComboBoxColumn Caption="製程模式" FieldName="MPROCMD" VisibleIndex="24" Width="4%" MaxWidth="55" MinWidth="40">
                                            <PropertiesComboBox>
                                                <Items>
                                                    <dx:ListEditItem Text="自製" Value="1" />
                                                    <dx:ListEditItem Text="外包" Value="2" />
                                                </Items>
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn>
                                    </Columns>
                                    <Styles>
                                        <AlternatingRow Enabled="True">
                                        </AlternatingRow>
                                    </Styles>
                                </dx:ASPxGridView>
                            </DetailRow>

                        </Templates>
                        <SettingsPager NumericButtonCount="6" PageSize="20">
                            <FirstPageButton Visible="True">
                            </FirstPageButton>
                            <LastPageButton Visible="True">
                            </LastPageButton>
                        </SettingsPager>
                        <SettingsEditing EditFormColumnCount="3" Mode="PopupEditForm">
                        </SettingsEditing>
                        <Settings VerticalScrollableHeight="500" VerticalScrollBarMode="Visible" />
                        <SettingsBehavior AllowSelectByRowClick="True" ConfirmDelete="True" />
                        <SettingsCommandButton>
                            <NewButton ButtonType="Image">
                                <Image Url="~/Images/Navigator/Grid_Add.png">
                                </Image>
                            </NewButton>
                            <UpdateButton ButtonType="Image">
                                <Image Url="~/Images/Navigator/Grid_Post.png">
                                </Image>
                            </UpdateButton>
                            <CancelButton ButtonType="Image">
                                <Image Url="~/Images/Navigator/Grid_Cancel.png">
                                </Image>
                            </CancelButton>
                            <EditButton ButtonType="Image">
                                <Image Url="~/Images/Navigator/Grid_Edit.png">
                                </Image>
                            </EditButton>
                            <DeleteButton ButtonType="Image">
                                <Image Url="~/Images/Navigator/Grid_Delete.png">
                                </Image>
                            </DeleteButton>
                        </SettingsCommandButton>

                        <SettingsDataSecurity AllowReadUnlistedFieldsFromClientApi="True" />

                        <SettingsPopup>
                            <EditForm AllowResize="True" Width="850">
                                <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchAtWindowInnerWidth="768" />
                            </EditForm>

                            <HeaderFilter MinHeight="140px"></HeaderFilter>

<FilterControl AutoUpdatePosition="False"></FilterControl>
                        </SettingsPopup>
                        <SettingsSearchPanel CustomEditorID="SearchTextBox" />
                        <SettingsText CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" SearchPanelEditorNullText="請輸入關鍵字" CommandCancel="取消" CommandUpdate="存入" Title="產品製途設定" ConfirmDelete="確定刪除此筆紀錄 ?" PopupEditFormCaption="產品製途編輯" />
                        <EditFormLayoutProperties ColCount="3">
                            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="768" />
                            <Items>
                                <dx:GridViewColumnLayoutItem ColumnName="品號">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="MADESC">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="MASPEC">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="MROUTID">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="製途名稱">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="MROUTDESC">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="BTCHPDQTY">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="單位">
                                </dx:GridViewColumnLayoutItem>
                                <dx:EditModeCommandLayoutItem ColSpan="3" HorizontalAlign="Right">
                                </dx:EditModeCommandLayoutItem>
                            </Items>
                        </EditFormLayoutProperties>
                        <Columns>
                            <dx:GridViewCommandColumn ShowInCustomizationForm="True" ShowNewButtonInHeader="True" VisibleIndex="0" Width="70px">
                                <HeaderTemplate>
                                    <dx:ASPxButton ID="NavgInsert" runat="server" ClientInstanceName="NavgInsert" AutoPostBack="False" RenderMode="Link" ToolTip="新增製途" OnLoad="NavgInsert_Load">
                                        <ClientSideEvents Click="function(s, e) {OnNavgInsertClick(s, e); 
}" />
                                        <Image Url="~/Images/Navigator/Grid_Add.png">
                                        </Image>
                                    </dx:ASPxButton>
                                </HeaderTemplate>
                                <CustomButtons>
                                    <dx:GridViewCommandColumnCustomButton ID="NavgEdit" Text=" ">
                                        <Image ToolTip="編輯製途" Url="~/Images/Navigator/Grid_Edit.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                    <dx:GridViewCommandColumnCustomButton ID="NavgDelete" Text=" ">
                                        <Image ToolTip="刪除製途" Url="~/Images/Navigator/Grid_Delete.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                </CustomButtons>
                            </dx:GridViewCommandColumn>
                            <dx:GridViewDataTextColumn Caption="品名" FieldName="MADESC" ShowInCustomizationForm="True" VisibleIndex="2">
                                <EditItemTemplate>
                                    <dx:ASPxTextBox ID="edMADESC" runat="server" ClientEnabled="false" ClientInstanceName="edMADESC" Width="100%" Value='<%#Eval("MADESC") %>'>
                                    </dx:ASPxTextBox>
                                </EditItemTemplate>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="規格" FieldName="MASPEC" ShowInCustomizationForm="True" VisibleIndex="3" AdaptivePriority="1">
                                <EditItemTemplate>
                                    <dx:ASPxTextBox ID="edMASPEC" runat="server" ClientEnabled="false" ClientInstanceName="edMASPEC" Width="100%" Value='<%#Eval("MASPEC") %>'>
                                    </dx:ASPxTextBox>
                                </EditItemTemplate>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataComboBoxColumn Caption="單位" FieldName="RTUNITNM" ShowInCustomizationForm="True" VisibleIndex="5" Width="70px">
                                <PropertiesComboBox DataSourceID="sdsUnit" TextField="UNITNM" ValueField="UNITNO" DropDownStyle="DropDown">
                                </PropertiesComboBox>
                            </dx:GridViewDataComboBoxColumn>
                            <dx:GridViewDataTextColumn Caption="製途說明" FieldName="MROUTDESC" ShowInCustomizationForm="True" VisibleIndex="9" AdaptivePriority="1">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="生產批量" FieldName="BTCHPDQTY" ShowInCustomizationForm="True" VisibleIndex="4">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="製途品號" FieldName="MANO" ShowInCustomizationForm="True" VisibleIndex="1">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="製途名稱" FieldName="MROUTNM" ShowInCustomizationForm="True" VisibleIndex="8">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="製途代碼" FieldName="MROUTID" ShowInCustomizationForm="True" VisibleIndex="6">
                                <EditItemTemplate>
                                    <dx:ASPxTextBox ID="edMROUTID" runat="server" ClientInstanceName="edMROUTID" Width="100%" Value='<%#Eval("MROUTID") %>'>
                                    </dx:ASPxTextBox>
                                </EditItemTemplate>
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataComboBoxColumn Caption="品號" FieldName="MANO" VisibleIndex="10" Visible="False">
                                <PropertiesComboBox ClientInstanceName="edMANO" TextField="MADESC" ValueField="MANO" ValueType="System.String" TextFormatString="{0} {1} {2}" FilterMinLength="2" OnItemsRequestedByFilterCondition="edMANO_ItemsRequestedByFilterCondition" EnableCallbackMode="True" OnItemRequestedByValue="edMANO_ItemRequestedByValue" CallbackPageSize="30" NullText="輸入兩個字元(以上)啟動查詢">
                                    <Columns>
                                        <dx:ListBoxColumn Caption="代碼" FieldName="MANO" />
                                        <dx:ListBoxColumn Caption="品名" FieldName="MADESC" />
                                        <dx:ListBoxColumn Caption="規格" FieldName="MASPEC" />
                                    </Columns>
                                    <ClearButton DisplayMode="Always">
                                    </ClearButton>
                                    <ClientSideEvents ValueChanged="function(s, e) {
                                                    MainDBGrid.PerformCallback(s.GetValue());	
}" />
                                </PropertiesComboBox>
                            </dx:GridViewDataComboBoxColumn>
                            <dx:GridViewDataCheckColumn Caption="預設製途" FieldName="ISWODEFAULT" ShowInCustomizationForm="True" VisibleIndex="11">
                            </dx:GridViewDataCheckColumn>
                            <dx:GridViewDataTextColumn Caption="準備人時/批" FieldName="TSPREPHMANTICK" ShowInCustomizationForm="True" VisibleIndex="16" Width="6%" MinWidth="60" MaxWidth="75" AdaptivePriority="1">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="準備機時/批" FieldName="TSPREPMACHTICK" ShowInCustomizationForm="True" VisibleIndex="18" Width="6%" MinWidth="60" MaxWidth="75" AdaptivePriority="1">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="加工人時/批" FieldName="TSBTCHHMANTICK" ShowInCustomizationForm="True" VisibleIndex="17" Width="6%" MinWidth="60" MaxWidth="75" AdaptivePriority="1">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="加工機時/批" FieldName="TSBTCHMACHTICK" ShowInCustomizationForm="True" VisibleIndex="19" Width="6%" MinWidth="60" MaxWidth="75" AdaptivePriority="1">
                            </dx:GridViewDataTextColumn>
                        </Columns>
                        <Styles>
                            <AlternatingRow Enabled="True">
                            </AlternatingRow>
                        </Styles>
                    </dx:ASPxGridView>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>


        <script type="text/javascript">
            // <![CDATA[
            ASPxClientControl.GetControlCollection().ControlsInitialized.AddHandler(function (s, e) {
                UpdateGridHeight();
            });
            ASPxClientControl.GetControlCollection().BrowserWindowResized.AddHandler(function (s, e) {
                UpdateGridHeight();
            });
            // ]]>
        </script>

        <dx:ASPxPopupControl ID="pclSearchPanel" runat="server" CloseAction="CloseButton" CloseOnEscape="True" Modal="True"
            PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pclSearchPanel"
            HeaderText="設定查詢條件" AllowDragging="True" PopupAnimationType="None" EnableViewState="False" Width="500px" Height="189px">
            <ClientSideEvents PopUp="function(s, e) { cbxBMAKDNO.Focus(); }" Init="function(s, e) {
pclSearchPanelInit();
}" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxPanel ID="Panel1" runat="server" DefaultButton="btOK">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <table width="100%">
                                    <tr>
                                        <td rowspan="8" class="auto-style1">
                                            <div class="pcmSideSpacer">
                                            </div>
                                        </td>
                                        <td class="auto-style8">
                                            <dx:ASPxLabel ID="ASPxLabel9" runat="server" AssociatedControlID="lblBMROUTNM" Text="製途名稱">
                                            </dx:ASPxLabel>
                                        </td>
                                        <td class="auto-style7">
                                            <dx:ASPxTextBox ID="edBMROUTNM" runat="server" Width="100%" ClientInstanceName="edBMROUTNM">
                                                <ClientSideEvents LostFocus="function(s, e) {
 if (edBMROUTNM.GetValue()!=null)   {  
   cbxBMACDNO.SetSelectedIndex(-1);
   cbxBMAKDNO.SetSelectedIndex(-1);
  } 
}" />
                                            </dx:ASPxTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style6"></td>
                                        <td class="auto-style5"></td>
                                    </tr>

                                    <tr>
                                        <td class="auto-style2">
                                            <dx:ASPxLabel ID="ASPxLabel5" runat="server" AssociatedControlID="lblBDPTNO" Text="貨品區分">
                                            </dx:ASPxLabel>
                                        </td>
                                        <td class="auto-style7">
                                            <dx:ASPxComboBox ID="cbxBMACDNO" runat="server" ClientInstanceName="cbxBMACDNO" DataSourceID="sdsMacd" TextField="MACDNM" ValueField="MACDNO" Width="100%" AllowNull="True">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) {OnBMACDNOChanged(s)	
}" />
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td class="auto-style6"></td>
                                        <td class="auto-style5"></td>
                                    </tr>
                                    <tr>
                                        <td rowspan="1" class="auto-style2">
                                            <dx:ASPxLabel ID="ASPxLabel1" runat="server" AssociatedControlID="lblBACNTYR" Text="貨品種類">
                                            </dx:ASPxLabel>
                                        </td>
                                        <td class="auto-style7">
                                            <dx:ASPxComboBox ID="cbxBMAKDNO" runat="server" ClientInstanceName="cbxBMAKDNO" DataSourceID="SdsMakd" OnCallback="cbxBMAKDNO_Callback" TextField="MAKDNM" ValueField="MAKDNO" Width="100%" AllowNull="True" NullText="全部種類">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) {	
}" />
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style6"></td>
                                        <td class="auto-style5"></td>
                                    </tr>

                                    <tr>
                                        <td class="auto-style2"></td>
                                        <td class="auto-style7">
                                            <div class="pcmButton">
                                                <dx:ASPxButton ID="btnGoFilter" runat="server" AutoPostBack="False" OnClick="btnGoFilter_Click" Style="float: left; margin-right: 8px" Text="設定" Width="80px" CausesValidation="False" ValidationGroup="entryGroup" UseSubmitBehavior="False">
                                                    <ClientSideEvents Click="function(s, e) {
}" />
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" Style="float: right; margin-right: 8px" Text="取消" Width="80px" UseSubmitBehavior="False">
                                                    <ClientSideEvents Click="function(s, e) { pclSearchPanel.Hide(); }" />
                                                </dx:ASPxButton>
                                            </div>

                                        </td>
                                    </tr>
                                </table>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                    <script type="text/javascript">
                        // <![CDATA[
                        function DefaultAppointmentMenuHandler(scheduler, s, args) {
                            if (args.item.GetItemCount() <= 0)
                                scheduler.RaiseCallback("USRAPTMENU|" + args.item.name);
                        }
                        // ]]>
                    </script>

                </dx:PopupControlContentControl>
            </ContentCollection>
            <ContentStyle>
                <Paddings PaddingBottom="5px" />
            </ContentStyle>
        </dx:ASPxPopupControl>
        <dx:ASPxPopupControl ID="pclMRoutDuplPanel" runat="server" CloseAction="CloseButton" CloseOnEscape="True" Modal="True"
            PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pclMRoutDuplPanel"
            HeaderText="複製當筆產品製途" AllowDragging="True" PopupAnimationType="None" EnableViewState="False" Width="500px" Height="189px">
            <ClientSideEvents PopUp="function(s, e) { cbxRMANO.Focus(); }" Init="function(s, e) {
pclMRoutDuplPanelInit();	
}"
                CloseUp="function(s, e) {
ASPxClientEdit.ClearEditorsInContainer(s.GetMainElement(), '', true);	
}" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxPanel ID="ASPxPanel1" runat="server" DefaultButton="btOK">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <dx:ASPxCallbackPanel ID="cbkPanelMRoutDupl" runat="server" Width="100%" ClientInstanceName="cbkPanelMRoutDupl" OnCallback="cbkPanelMRoutDupl_Callback">
                                    <PanelCollection>
                                        <dx:PanelContent runat="server">

                                            <table width="100%">
                                                <tr>
                                                    <td rowspan="17" class="auto-style1">
                                                        <div class="pcmSideSpacer">
                                                        </div>
                                                    </td>
                                                    <td class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel2" runat="server" AssociatedControlID="lblBDPTNO" Text="貨品區分">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="cbxRMACDNO" runat="server" ClientInstanceName="cbxRMACDNO" DataSourceID="sdsMacd" TextField="MACDNM" ValueField="MACDNO" Width="100%">
                                                            <ClientSideEvents SelectedIndexChanged="function(s, e) {OnBMACDNOChanged(s)	
}" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style5"></td>
                                                </tr>
                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel3" runat="server" AssociatedControlID="lblBACNTYR" Text="貨品種類">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="cbxRMAKDNO" runat="server" ClientInstanceName="cbxRMAKDNO" DataSourceID="sdsMakd" OnCallback="cbxBMAKDNO_Callback" TextField="MAKDNM" ValueField="MAKDNO" Width="100%" AllowNull="True" NullText="全部種類">
                                                            <ClientSideEvents SelectedIndexChanged="function(s, e) {	
}" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style5"></td>
                                                </tr>

                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel7" runat="server" AssociatedControlID="lblBACNTYR" Text="產品">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="cbxRMANO" runat="server" ClientInstanceName="cbxRMANO" DataSourceID="sdsMa" TextField="MADESC" ValueField="MANO" Width="100%">
                                                            <ClientSideEvents SelectedIndexChanged="function(s, e) {OnRMANOChanged(s);	
}" />
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style5"></td>
                                                </tr>
                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel8" runat="server" AssociatedControlID="lblBACNTYR" Text="途程">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="cbxRMROUTID" runat="server" ClientInstanceName="cbxRMROUTID" DataSourceID="sdsMaRouts" TextField="MROUTNM" ValueField="MROUTID" Width="100%" OnCallback="cbxRMROUTID_Callback">
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style3"></td>
                                                    <td class="auto-style3" align="center">
                                                        <dx:ASPxLabel ID="lblRDupl" runat="server" ClientInstanceName="lblRDupl" Text="複 製  ↓↓" ForeColor="#009933">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel4" runat="server" AssociatedControlID="lblBACNTYR" Text="目標-產品">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="cbxRTARGMANO" runat="server" ClientInstanceName="cbxRTARGMANO" DataSourceID="sdsMa" TextField="MADESC" ValueField="MANO" Width="100%" FilterMinLength="2" OnItemsRequestedByFilterCondition="cbxRTARGMANO_ItemsRequestedByFilterCondition" EnableCallbackMode="True" OnItemRequestedByValue="cbxRTARGMANO_ItemRequestedByValue" CallbackPageSize="30" NullText="輸入兩個字元(以上)啟動查詢">
                                                            <Columns>
                                                                <dx:ListBoxColumn Caption="代碼" FieldName="MANO" />
                                                                <dx:ListBoxColumn Caption="品名" FieldName="MADESC" />
                                                                <dx:ListBoxColumn Caption="規格" FieldName="MASPEC" />
                                                            </Columns>
                                                            <ClientSideEvents ValueChanged="function(s, e) {
  cbkPanelMRoutDupl.PerformCallback('cbxRTARGMANO');
	
}" />
                                                            <ClearButton DisplayMode="Always">
                                                            </ClearButton>
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style5"></td>
                                                </tr>

                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel6" runat="server" AssociatedControlID="lblBACNTYR" Text="目標-途程">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxTextBox ID="cbxRTARGMROUTID" runat="server" ClientInstanceName="cbxRTARGMROUTID" DataSourceID="sdsMaRouts" TextField="MROUTNM" ValueField="MROUTID" Width="100%" OnCallback="cbxRMROUTID_Callback">
                                                        </dx:ASPxTextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style5"></td>
                                                </tr>
                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel11" runat="server" AssociatedControlID="lblBACNTYR" Text="生產批量">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxTextBox ID="tbxRTARGBTCHPDQTY" runat="server" ClientInstanceName="tbxRTARGBTCHPDQTY" Width="100%">
                                                        </dx:ASPxTextBox>

                                                    </td>
                                                    <td rowspan="15" class="auto-style1">
                                                        <div class="pcmSideSpacer">
                                                        </div>
                                                    </td>

                                                </tr>
                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style5"></td>
                                                </tr>

                                                <tr>
                                                    <td class="auto-style2">&nbsp;</td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxLabel ID="lblErrorMessage" runat="server" ClientInstanceName="lblErrorMessage" ForeColor="#CC6600">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style5"></td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style2"></td>
                                                    <td class="auto-style7">
                                                        <div class="pcmButton">
                                                            <dx:ASPxButton ID="btnMRoutDupl" runat="server" AutoPostBack="False" Style="float: left; margin-right: 8px" Text="複製" Width="80px" CausesValidation="False" ValidationGroup="entryGroup" UseSubmitBehavior="False" OnClick="btnMRoutDupl_Click">
                                                                <ClientSideEvents Click="function(s, e) {
}" />
                                                            </dx:ASPxButton>
                                                            <dx:ASPxButton ID="btnRCancel" runat="server" AutoPostBack="False" Style="float: right; margin-right: 8px" Text="取消" Width="80px" UseSubmitBehavior="False">
                                                                <ClientSideEvents Click="function(s, e) { pclMRoutDuplPanel.Hide(); }" />
                                                            </dx:ASPxButton>
                                                        </div>

                                                    </td>
                                                </tr>
                                            </table>
                                        </dx:PanelContent>
                                    </PanelCollection>
                                </dx:ASPxCallbackPanel>

                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                    <script type="text/javascript">
                        // <![CDATA[
                        function DefaultAppointmentMenuHandler(scheduler, s, args) {
                            if (args.item.GetItemCount() <= 0)
                                scheduler.RaiseCallback("USRAPTMENU|" + args.item.name);
                        }
                        // ]]>
                    </script>

                </dx:PopupControlContentControl>
            </ContentCollection>
            <ContentStyle>
                <Paddings PaddingBottom="5px" />
            </ContentStyle>
        </dx:ASPxPopupControl>
        <dx:ASPxGridViewExporter ID="MainDBGridExporter" runat="server" GridViewID="MainDBGrid">
            <Styles>
                <Header Font-Names="Arial Unicode MS">
                </Header>
                <Cell Font-Names="Arial Unicode MS">
                </Cell>
            </Styles>
        </dx:ASPxGridViewExporter>
        <dx:ASPxPopupControl ID="pclEditPagePanel" runat="server" CloseAction="CloseButton" CloseOnEscape="True" Modal="True"
            PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pclEditPagePanel"
            HeaderText="服務編輯" AllowDragging="True" PopupAnimationType="None" EnableViewState="False" Height="300px">
            <ClientSideEvents
                Init="function(s, e) {
  pclEditPagePanelInit();
}"
                CloseButtonClick="function(s, e) {
HidepclEditPagePanel('btnSubmit');
}"
                Closing="function(s, e) {
}" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                </dx:PopupControlContentControl>
            </ContentCollection>
            <ContentStyle>
                <Paddings PaddingBottom="5px" />
            </ContentStyle>

        </dx:ASPxPopupControl>

        <asp:SqlDataSource ID="sdsMaRout" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [MaRout] WHERE [MANO] = @MANO AND [MROUTID] = @MROUTID" InsertCommand="INSERT INTO MaRout(MANO, MROUTID, MROUTNM, MROUTDESC, BTCHPDQTY, TSPREPHMANTICK, TSPREPMACHTICK, TSBTCHHMANTICK, TSBTCHMACHTICK, ISWODEFAULT) VALUES (@MANO, @MROUTID, @MROUTNM, @MROUTDESC, @BTCHPDQTY, @TSPREPHMANTICK, @TSPREPMACHTICK, @TSBTCHHMANTICK, @TSBTCHMACHTICK, @ISWODEFAULT)" SelectCommand="SELECT MaRout.MANO, Ma.MADESC, Ma.MASPEC, Ma.RTUNITNM, MaRout.MROUTID, MaRout.MROUTNM, MaRout.MROUTDESC, MaRout.BTCHPDQTY, MaRout.TSPREPHMANTICK ,MaRout.TSPREPMACHTICK ,MaRout.TSBTCHHMANTICK  ,MaRout.TSBTCHMACHTICK,  MaRout.ISWODEFAULT, Ma.RTUNITNM
FROM MaRout LEFT JOIN Ma ON (MaRout.MANO = Ma.MANO)"
            UpdateCommand="UPDATE [MaRout] SET [MROUTNM] = @MROUTNM, [MROUTDESC] = @MROUTDESC, [BTCHPDQTY] = @BTCHPDQTY WHERE [MANO] = @MANO AND [MROUTID] = @MROUTID" OnDeleted="sdsMaRout_Deleted">
            <DeleteParameters>
                <asp:Parameter Name="MANO" Type="String" />
                <asp:Parameter Name="MROUTID" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="MANO" Type="String" />
                <asp:Parameter Name="MROUTID" Type="String" />
                <asp:Parameter Name="MROUTNM" Type="String" />
                <asp:Parameter Name="MROUTDESC" Type="String" />
                <asp:Parameter Name="BTCHPDQTY" Type="Double" />
                <asp:Parameter Name="TSPREPHMANTICK" />
                <asp:Parameter Name="TSPREPMACHTICK" />
                <asp:Parameter Name="TSBTCHHMANTICK" />
                <asp:Parameter Name="TSBTCHMACHTICK" />
                <asp:Parameter Name="ISWODEFAULT" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="MROUTNM" Type="String" />
                <asp:Parameter Name="MROUTDESC" Type="String" />
                <asp:Parameter Name="BTCHPDQTY" Type="Double" />
                <asp:Parameter Name="MANO" Type="String" />
                <asp:Parameter Name="MROUTID" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMaRoutProc" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [MaRoutProc] WHERE [MANO] = @MANO AND [MROUTID] = @MROUTID AND [MROUTSRNO] = @MROUTSRNO" InsertCommand="INSERT INTO MaRoutProc(MANO, MROUTID, MROUTSRNO, MPROCID, UNITCOST, SPREPHMANTM, SPREPMACHTM, SBTCHHMANTM, SBTCHMACHTM, PROCFMULAOPT, PUTNINPTFORMAT, PNPARA1, PNMATHOPTR, PNPARA2, PUTNQTY, PNUNITNM, MATHOPTR, PN2RTEXGRATE, PHPARA1, PHMATHOPTR, PHPARA2, PFNHQTY, FNUNITNM, FNSHSPEC, MPROCMD, SPREPHMANTICK, SPREPMACHTICK, SBTCHHMANTICK, SBTCHMACHTICK, FNSHVALDMD, FNSHTLRNRATE) VALUES (@MANO, @MROUTID, @MROUTSRNO, @MPROCID, @UNITCOST, @SPREPHMANTM, @SPREPMACHTM, @SBTCHHMANTM, @SBTCHMACHTM, @PROCFMULAOPT, @PUTNINPTFORMAT, @PNPARA1, @PNMATHOPTR, @PNPARA2, @PUTNQTY, @PNUNITNM, @MATHOPTR, @PN2RTEXGRATE, @PHPARA1, @PHMATHOPTR, @PHPARA2, @PFNHQTY, @FNUNITNM, @FNSHSPEC, @MPROCMD, @SPREPHMANTICK, @SPREPMACHTICK, @SBTCHHMANTICK, @SBTCHMACHTICK, @FNSHVALDMD, @FNSHTLRNRATE)" SelectCommand="SELECT MaRoutProc.MANO, MaRoutProc.MROUTID, MaRoutProc.MROUTSRNO, MaRoutProc.MPROCID, ManuProc.MPROCNM, ManuProc.MPROCDESC, ManuProc.MLINEID, ManuLine.MLINENM, MaRoutProc.UNITCOST, 
 MaRoutProc.SPREPHMANTICK, MaRoutProc.SPREPMACHTICK, MaRoutProc.SBTCHHMANTICK, MaRoutProc.SBTCHMACHTICK, MaRoutProc.SPREPHMANTM ,MaRoutProc.SPREPMACHTM, MaRoutProc.SBTCHHMANTM, MaRoutProc.SBTCHMACHTM, 
 MaRoutProc.PUTNINPTFORMAT, MaRoutProc.PROCFMULAOPT, MaRoutProc.PNPARA1, MaRoutProc.PNMATHOPTR, MaRoutProc.PNPARA2, MaRoutProc.PUTNQTY, MaRoutProc.PNUNITNM, MaRoutProc.MATHOPTR, MaRoutProc.PN2RTEXGRATE,
 MaRoutProc.PHPARA1, MaRoutProc.PHMATHOPTR, MaRoutProc.PHPARA2, MaRoutProc.PFNHQTY, MaRoutProc.FNUNITNM, MaRoutProc.FNSHSPEC, MaRoutProc.MPROCMD, MaRoutProc.FNSHVALDMD, MaRoutProc.FNSHTLRNRATE
FROM MaRoutProc LEFT JOIN ManuProc ON (MaRoutProc.MPROCID = ManuProc.MPROCID)
 LEFT JOIN ManuLine ON (ManuProc.MLINEID = ManuLine.MLINEID)
WHERE MaRoutProc.MANO=@MANO AND MaRoutProc.MROUTID=@MROUTID
ORDER BY MaRoutProc.MROUTSRNO"
            UpdateCommand="UPDATE [MaRoutProc] SET [MPROCID] = @MPROCID, [UNITCOST] = @UNITCOST, [SPREPHMANTM] = @SPREPHMANTM, [SPREPMACHTM] = @SPREPMACHTM, [SBTCHHMANTM] = @SBTCHHMANTM, [SBTCHMACHTM] = @SBTCHMACHTM, [PROCFMULAOPT] = @PROCFMULAOPT, [PUTNINPTFORMAT] = @PUTNINPTFORMAT, [PNPARA1] = @PNPARA1, [PNMATHOPTR] = @PNMATHOPTR, [PNPARA2] = @PNPARA2, [PUTNQTY] = @PUTNQTY, [PNUNITNM] = @PNUNITNM, [MATHOPTR] = @MATHOPTR, [PN2RTEXGRATE] = @PN2RTEXGRATE, [PHPARA1] = @PHPARA1, [PHMATHOPTR] = @PHMATHOPTR, [PHPARA2] = @PHPARA2, [PFNHQTY] = @PFNHQTY, [FNUNITNM] = @FNUNITNM, [FNSHSPEC] = @FNSHSPEC, [MPROCMD] = @MPROCMD WHERE [MANO] = @MANO AND [MROUTID] = @MROUTID AND [MROUTSRNO] = @MROUTSRNO">
            <DeleteParameters>
                <asp:Parameter Name="MANO" Type="String" />
                <asp:Parameter Name="MROUTID" Type="String" />
                <asp:Parameter Name="MROUTSRNO" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="MANO" Type="String" />
                <asp:Parameter Name="MROUTID" Type="String" />
                <asp:Parameter Name="MROUTSRNO" Type="String" />
                <asp:Parameter Name="MPROCID" Type="String" />
                <asp:Parameter Name="UNITCOST" Type="Double" />
                <asp:Parameter DbType="Time" Name="SPREPHMANTM" />
                <asp:Parameter DbType="Time" Name="SPREPMACHTM" />
                <asp:Parameter DbType="Time" Name="SBTCHHMANTM" />
                <asp:Parameter DbType="Time" Name="SBTCHMACHTM" />
                <asp:Parameter Name="PROCFMULAOPT" Type="Int32" />
                <asp:Parameter Name="PUTNINPTFORMAT" Type="String" />
                <asp:Parameter Name="PNPARA1" Type="Double" />
                <asp:Parameter Name="PNMATHOPTR" Type="String" />
                <asp:Parameter Name="PNPARA2" Type="Double" />
                <asp:Parameter Name="PUTNQTY" Type="Double" />
                <asp:Parameter Name="PNUNITNM" Type="String" />
                <asp:Parameter Name="MATHOPTR" Type="String" />
                <asp:Parameter Name="PN2RTEXGRATE" Type="Int32" />
                <asp:Parameter Name="PHPARA1" Type="Double" />
                <asp:Parameter Name="PHMATHOPTR" Type="String" />
                <asp:Parameter Name="PHPARA2" Type="Double" />
                <asp:Parameter Name="PFNHQTY" Type="Double" />
                <asp:Parameter Name="FNUNITNM" Type="String" />
                <asp:Parameter Name="FNSHSPEC" Type="String" />
                <asp:Parameter Name="MPROCMD" Type="Int32" />
                <asp:Parameter Name="SPREPHMANTICK" />
                <asp:Parameter Name="SPREPMACHTICK" />
                <asp:Parameter Name="SBTCHHMANTICK" />
                <asp:Parameter Name="SBTCHMACHTICK" />
                <asp:Parameter Name="FNSHVALDMD" />
                <asp:Parameter Name="FNSHTLRNRATE" />
            </InsertParameters>
            <SelectParameters>
                <asp:SessionParameter Name="MANO" SessionField="MANO" />
                <asp:SessionParameter Name="MROUTID" SessionField="MROUTID" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="MPROCID" Type="String" />
                <asp:Parameter Name="UNITCOST" Type="Double" />
                <asp:Parameter Name="SPREPHMANTM" DbType="Time" />
                <asp:Parameter Name="SPREPMACHTM" DbType="Time" />
                <asp:Parameter Name="SBTCHHMANTM" DbType="Time" />
                <asp:Parameter DbType="Time" Name="SBTCHMACHTM" />
                <asp:Parameter Name="PROCFMULAOPT" Type="Int32" />
                <asp:Parameter Name="PUTNINPTFORMAT" Type="String" />
                <asp:Parameter Name="PNPARA1" Type="Double" />
                <asp:Parameter Name="PNMATHOPTR" Type="String" />
                <asp:Parameter Name="PNPARA2" Type="Double" />
                <asp:Parameter Name="PUTNQTY" Type="Double" />
                <asp:Parameter Name="PNUNITNM" Type="String" />
                <asp:Parameter Name="MATHOPTR" Type="String" />
                <asp:Parameter Name="PN2RTEXGRATE" Type="Int32" />
                <asp:Parameter Name="PHPARA1" Type="Double" />
                <asp:Parameter Name="PHMATHOPTR" Type="String" />
                <asp:Parameter Name="PHPARA2" Type="Double" />
                <asp:Parameter Name="PFNHQTY" Type="Double" />
                <asp:Parameter Name="FNUNITNM" Type="String" />
                <asp:Parameter Name="FNSHSPEC" Type="String" />
                <asp:Parameter Name="MPROCMD" Type="Int32" />
                <asp:Parameter Name="MANO" Type="String" />
                <asp:Parameter Name="MROUTID" Type="String" />
                <asp:Parameter Name="MROUTSRNO" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMacd" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Macd.MACDNO, Macd.MACDNM
FROM  Macd
ORDER BY Macd.MACDNO"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SdsMakd" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Makd.MACDNO, Makd.MAKDNO, Makd.MAKDNM, Makd.ISRESERV
FROM Makd
WHERE Makd.MACDNO=@MACDNO
ORDER BY Makd.MAKDNO">
            <SelectParameters>
                <asp:SessionParameter Name="MACDNO" SessionField="MACDNO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsManuProc" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT ManuProc.MPROCID, ManuProc.MPROCNM, ManuProc.MPROCDESC, ManuProc.MLINEID, ManuLine.MLINENM
FROM ManuProc LEFT JOIN  ManuLine ON ManuProc.MLINEID = ManuLine.MLINEID
ORDER BY ManuProc.MPROCID"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsManuLine" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT ManuLine.MLINEID, ManuLine.MLINENM, ManuLine.FACTNO, Fact.FACTNM
FROM ManuLine LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO)
ORDER BY ManuLine.MLINEID"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMa" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB,
 Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST,
 Ma.UNITPRIC, Ma.BTCHPDQTY, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO
FROM Ma WHERE Ma.MANO IS NULL"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMaRouts" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT MaRout.MANO, MaRout.MROUTID, MaRout.MROUTNM, MaRout.MROUTDESC
FROM MaRout 
WHERE MaRout.MANO=@MANO

">
            <SelectParameters>
                <asp:SessionParameter Name="MANO" SessionField="MANO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsUnit" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [UNITNO], [UNITNM] FROM [Unit]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMaPop" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMaPop1" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>"></asp:SqlDataSource>
    </form>
</body>
</html>

