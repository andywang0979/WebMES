<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManuMach.aspx.cs" Inherits="WebMES.Admin.ManuMach" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/Admin/ManuMenuPanel.ascx" TagPrefix="uc1" TagName="ManuMenuPanel" %>

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
    </style>
    <script type="text/javascript">
        function OnBMACDNOChanged(cbxBMACDNO) {
            cbxBMPROCID.PerformCallback(cbxBMACDNO.GetSelectedItem().value.toString());
        }

        var lastMROUTID = null;
        function OnMROUTIDChanged(cbxMROUTID) {
            if (MainDBTreeList.GetEditor("MPROCID").InCallback())
                lastMROUTID = cbxMROUTID.GetValue().toString();
            else
                MainDBTreeList.GetEditor("MPROCID").PerformCallback(cbxMROUTID.GetValue().toString());
        }
        function OnMPROCIDEndCallback(s, e) {
            if (lastMROUTID) {
                cbxMPROCID.PerformCallback(lastMROUTID);
                lastMROUTID = null;
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

    </script>
</head>
<body>
    <form id="KRD00" runat="server">
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
else if (i.substring(0,14) ==&quot;NavgDutyAssign&quot;)
  pclDutyAssignPanel.Show();
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
                </Items>
            </dx:ASPxMenu>
        </div>
        <dx:ASPxPanel ID="MainDBGridPanel" runat="server" ClientInstanceName="MainDBGridPanel" EnableTheming="True" Width="100%">
            <SettingsAdaptivity CollapseAtWindowInnerWidth="600" />
            <PanelCollection>
                <dx:PanelContent runat="server">
                    <dx:ASPxGridView ID="MainDBGrid" runat="server" ClientInstanceName="MainDBGrid" AutoGenerateColumns="False" DataSourceID="sdsManuProc" EnableTheming="True" KeyFieldName="MPROCID" CssClass="grid" Width="100%" KeyboardSupport="True" OnDetailRowExpandedChanged="MainDBGrid_DetailRowExpandedChanged" OnDataBound="MainDBGrid_DataBound">
                        <ClientSideEvents RowClick="function(s, e) {
}"
                            DetailRowExpanding="function(s, e) {
MainDBGrid.SetFocusedRowIndex(e.visibleIndex);
}"
                            RowFocusing="function(s, e) {
MainDBGrid.ExpandDetailRow(e.visibleIndex);
}" />
                        <SettingsDetail AllowOnlyOneMasterRowExpanded="True" ShowDetailRow="True" />
                        <SettingsAdaptivity AdaptivityMode="HideDataCells" HideDataCellsAtWindowInnerWidth="600">
                            <AdaptiveDetailLayoutProperties>
                                <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="580" />
                            </AdaptiveDetailLayoutProperties>
                        </SettingsAdaptivity>
                        <Templates>
                            <DetailRow>
                                <dx:ASPxCallbackPanel ID="cbkPanelMPROCID" runat="server" ClientInstanceName="cbkPanelMPROCID" OnCallback="cbkPanelMPROCID_Callback" Width="100%">
                                    <ClientSideEvents EndCallback="function(s, e) {
if (s.cpISedMACHINID) {
  SubDBGrid.GetEditor('MACHINNM').SetValue(s.cpMACHINNM);
  SubDBGrid.GetEditor('CAPACITY').SetValue(s.cpCAPACITY);
  delete s.cpISedMACHINID;
  delete s.cpMACHINNM;
  delete s.cpCAPACITY;
}
}" />
                                    <PanelCollection>
                                        <dx:PanelContent runat="server">
                                            <dx:ASPxGridView ID="SubDBGrid" runat="server" AutoGenerateColumns="False" ClientInstanceName="SubDBGrid" DataSourceID="sdsManuMach" EnableTheming="True" KeyFieldName="MPROCID;MACHINID" OnBeforePerformDataSelect="SubDBGrid_BeforePerformDataSelect" OnInitNewRow="SubDBGrid_InitNewRow" Width="100%" OnRowInserting="SubDBGrid_RowInserting">
                                                <SettingsAdaptivity AdaptivityMode="HideDataCells">
                                                </SettingsAdaptivity>

                                                <SettingsPager NumericButtonCount="6">
                                                    <FirstPageButton Visible="True">
                                                    </FirstPageButton>
                                                    <LastPageButton Visible="True">
                                                    </LastPageButton>
                                                </SettingsPager>
                                                <SettingsEditing Mode="Inline">
                                                </SettingsEditing>
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
                                                <SettingsPopup>
                                                    <HeaderFilter MinHeight="140px">
                                                    </HeaderFilter>
                                                </SettingsPopup>
                                                <SettingsText CommandCancel="取消" CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" CommandUpdate="存入" SearchPanelEditorNullText="請輸入關鍵字" ConfirmDelete="確認刪除此筆紀錄" />
                                                <EditFormLayoutProperties ColCount="3">
                                                    <Items>
                                                        <dx:GridViewColumnLayoutItem ColSpan="2" ColumnName="NODENM" Width="70%">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColumnName="NODEID">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColSpan="2" ColumnName="NODEDESC" RowSpan="2">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColumnName="WEGTPCNT">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:EditModeCommandLayoutItem ColSpan="3" HorizontalAlign="Right">
                                                        </dx:EditModeCommandLayoutItem>
                                                    </Items>
                                                </EditFormLayoutProperties>
                                                <Columns>
                                                    <dx:GridViewCommandColumn VisibleIndex="0" ShowDeleteButton="True" ShowEditButton="True" ShowNewButtonInHeader="True" Width="6%">
                                                    </dx:GridViewCommandColumn>
                                                    <dx:GridViewDataTextColumn Caption="機器名稱" FieldName="MACHINNM" ShowInCustomizationForm="True" VisibleIndex="2" Width="14%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="機器產能" FieldName="CAPACITY" ShowInCustomizationForm="True" VisibleIndex="3" Width="6%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="產線代碼" FieldName="MLINEID" ShowInCustomizationForm="True" VisibleIndex="4" Width="6%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="產線名稱" FieldName="MLINENM" ShowInCustomizationForm="True" VisibleIndex="5" Width="12%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataComboBoxColumn Caption="機器代碼" FieldName="MACHINID" VisibleIndex="1" Width="10%">
                                                        <PropertiesComboBox DataSourceID="sdsMachine" TextField="MACHINID" ValueField="MACHINID" TextFormatString="{0}">
                                                            <Columns>
                                                                <dx:ListBoxColumn Caption="代碼" FieldName="MACHINID" Width="100px" />
                                                                <dx:ListBoxColumn Caption="機器名稱" FieldName="MACHINNM" Width="300px" />
                                                            </Columns>
                                                            <ClientSideEvents SelectedIndexChanged="function(s, e) {cbkPanelMPROCID.PerformCallback(s.GetValue());}" />
                                                        </PropertiesComboBox>
                                                    </dx:GridViewDataComboBoxColumn>
                                                    <dx:GridViewDataTextColumn Caption="製程代碼" FieldName="MPROCID" ShowInCustomizationForm="True" VisibleIndex="6" Width="6%" Visible="False">
                                                    </dx:GridViewDataTextColumn>
                                                </Columns>
                                                <Styles>
                                                    <AlternatingRow Enabled="True">
                                                    </AlternatingRow>
                                                </Styles>
                                            </dx:ASPxGridView>

                                        </dx:PanelContent>
                                    </PanelCollection>
                                </dx:ASPxCallbackPanel>

                            </DetailRow>
                        </Templates>
                        <SettingsPager NumericButtonCount="6" PageSize="20">
                            <FirstPageButton Visible="True">
                            </FirstPageButton>
                            <LastPageButton Visible="True">
                            </LastPageButton>
                        </SettingsPager>
                        <SettingsEditing EditFormColumnCount="3">
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

                        <SettingsPopup>
                            <EditForm AllowResize="True" Modal="True" Width="800px" />

                            <HeaderFilter MinHeight="140px"></HeaderFilter>
                        </SettingsPopup>
                        <SettingsSearchPanel CustomEditorID="SearchTextBox" />
                        <SettingsText CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" SearchPanelEditorNullText="請輸入關鍵字" CommandCancel="取消" CommandUpdate="存入" Title="員工履歷" ConfirmDelete="確定刪除此筆紀錄 ?" />
                        <EditFormLayoutProperties ColCount="3">
                            <Items>
                                <dx:GridViewColumnLayoutItem ColumnName="MANO">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="MADESC">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="MASPEC">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="BTCHPDQTY">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="RTUNITNM">
                                </dx:GridViewColumnLayoutItem>
                                <dx:GridViewColumnLayoutItem ColumnName="LDTIME">
                                </dx:GridViewColumnLayoutItem>
                                <dx:EditModeCommandLayoutItem ColSpan="3" HorizontalAlign="Right">
                                </dx:EditModeCommandLayoutItem>
                            </Items>
                        </EditFormLayoutProperties>
                        <Columns>
                            <dx:GridViewCommandColumn ShowDeleteButton="True" ShowEditButton="True" ShowInCustomizationForm="True" ShowNewButtonInHeader="True" VisibleIndex="0" Width="70px">
                            </dx:GridViewCommandColumn>
                            <dx:GridViewDataTextColumn Caption="製程代碼" FieldName="MPROCID" ShowInCustomizationForm="True" VisibleIndex="1">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="製程名稱" FieldName="MPROCNM" ShowInCustomizationForm="True" VisibleIndex="2">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="製程說明" FieldName="MPROCDESC" ShowInCustomizationForm="True" VisibleIndex="4">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="生產線別" FieldName="MLINENM" ShowInCustomizationForm="True" VisibleIndex="5">
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
            <ClientSideEvents PopUp="function(s, e) { cbxBMPROCID.Focus(); }" Init="function(s, e) {
pclSearchPanelInit();
}" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxPanel ID="Panel1" runat="server" DefaultButton="btOK">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <table width="100%">
                                    <tr>
                                        <td rowspan="4" class="auto-style1">
                                            <div class="pcmSideSpacer">
                                            </div>
                                        </td>
                                        <td rowspan="1" class="auto-style2">
                                            <dx:ASPxLabel ID="ASPxLabel1" runat="server" AssociatedControlID="lblBACNTYR" Text="製程">
                                            </dx:ASPxLabel>
                                        </td>
                                        <td class="auto-style7">
                                            <dx:ASPxComboBox ID="cbxBMPROCID" runat="server" ClientInstanceName="cbxBMPROCID" DataSourceID="sdsManuProc" TextField="MPROCNM" ValueField="MPROCID" Width="100%" AllowNull="True" NullText="全部種類">
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
                                                <dx:ASPxButton ID="btnGoFilter" runat="server" AutoPostBack="False" OnClick="btnGoFilter_Click" Style="float: left; margin-right: 8px" Text="設定" Width="80px" CausesValidation="False" ValidationGroup="entryGroup">
                                                    <ClientSideEvents Click="function(s, e) {
}" />
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" Style="float: right; margin-right: 8px" Text="取消" Width="80px">
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

        <dx:ASPxGridViewExporter ID="MainDBGridExporter" runat="server" GridViewID="MainDBGrid">
            <Styles>
                <Header Font-Names="Arial Unicode MS">
                </Header>
                <Cell Font-Names="Arial Unicode MS">
                </Cell>
            </Styles>
        </dx:ASPxGridViewExporter>


        <asp:SqlDataSource ID="sdsManuProc" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [ManuProc] WHERE [MPROCID] = @MPROCID" InsertCommand="INSERT INTO [ManuProc] ([MPROCID], [MPROCNM], [MPROCDESC], [MLINEID]) VALUES (@MPROCID, @MPROCNM, @MPROCDESC, @MLINEID)" SelectCommand="SELECT ManuProc.MPROCID, ManuProc.MPROCNM, ManuProc.MPROCDESC, ManuProc.MLINEID, ManuLine.MLINENM
FROM ManuProc LEFT JOIN  ManuLine ON ManuProc.MLINEID = ManuLine.MLINEID
ORDER BY ManuProc.MPROCID"
            UpdateCommand="UPDATE [ManuProc] SET [MPROCNM] = @MPROCNM, [MPROCDESC] = @MPROCDESC, [MLINEID] = @MLINEID WHERE [MPROCID] = @MPROCID">
            <DeleteParameters>
                <asp:Parameter Name="MPROCID" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="MPROCID" Type="String" />
                <asp:Parameter Name="MPROCNM" Type="String" />
                <asp:Parameter Name="MPROCDESC" Type="String" />
                <asp:Parameter Name="MLINEID" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="MPROCNM" Type="String" />
                <asp:Parameter Name="MPROCDESC" Type="String" />
                <asp:Parameter Name="MLINEID" Type="String" />
                <asp:Parameter Name="MPROCID" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsManuMach" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [ManuMach] WHERE [MPROCID] = @MPROCID AND [MACHINID] = @MACHINID" InsertCommand="INSERT INTO [ManuMach] ([MPROCID], [MACHINID]) VALUES (@MPROCID, @MACHINID)" SelectCommand="SELECT ManuMach.MPROCID, ManuMach.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, 
 Machine.MLINEID, ManuLine.MLINENM
FROM  ManuMach LEFT JOIN Machine ON ManuMach.MACHINID = Machine.MACHINID
 LEFT JOIN ManuLine ON (Machine.MLINEID = ManuLine.MLINEID)
WHERE ManuMach.MPROCID=@MPROCID
ORDER BY ManuMach.MACHINID">
            <DeleteParameters>
                <asp:Parameter Name="MPROCID" Type="String" />
                <asp:Parameter Name="MACHINID" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="MPROCID" Type="String" />
                <asp:Parameter Name="MACHINID" Type="String" />
            </InsertParameters>
            <SelectParameters>
                <asp:SessionParameter Name="MPROCID" SessionField="MPROCID" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMachine" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [Machine] WHERE [MACHINID] = @MACHINID" InsertCommand="INSERT INTO [Machine] ([MACHINID], [MACHINNM], [CAPACITY], [LOADRATE], [MACHINDESC], [MLINEID]) VALUES (@MACHINID, @MACHINNM, @CAPACITY, @LOADRATE, @MACHINDESC, @MLINEID)" SelectCommand="SELECT Machine.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, 
 Machine.MLINEID, ManuLine.MLINENM, Fact.FACTNM
FROM Machine LEFT JOIN ManuLine ON (Machine.MLINEID = ManuLine.MLINEID)
 LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO) 
ORDER BY Machine.MACHINID"
            UpdateCommand="UPDATE [Machine] SET [MACHINNM] = @MACHINNM, [CAPACITY] = @CAPACITY, [LOADRATE] = @LOADRATE, [MACHINDESC] = @MACHINDESC, [MLINEID] = @MLINEID WHERE [MACHINID] = @MACHINID" ProviderName="<%$ ConnectionStrings:WinSisTmplConnectionString.ProviderName %>">
            <DeleteParameters>
                <asp:Parameter Name="MACHINID" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="MACHINID" Type="String" />
                <asp:Parameter Name="MACHINNM" Type="String" />
                <asp:Parameter Name="CAPACITY" Type="Double" />
                <asp:Parameter Name="LOADRATE" Type="Double" />
                <asp:Parameter Name="MACHINDESC" Type="String" />
                <asp:Parameter Name="MLINEID" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="MACHINNM" Type="String" />
                <asp:Parameter Name="CAPACITY" Type="Double" />
                <asp:Parameter Name="LOADRATE" Type="Double" />
                <asp:Parameter Name="MACHINDESC" Type="String" />
                <asp:Parameter Name="MLINEID" Type="String" />
                <asp:Parameter Name="MACHINID" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </form>
</body>
</html>
