﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WoRoutMachNowList.aspx.cs" Inherits="WebMES.Admin.WoRoutMachNowList" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/Admin/ManuMenuPanel.ascx" TagPrefix="uc1" TagName="ManuMenuPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
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

        .auto-style1 {
            width: 3%;
        }

        .auto-style2 {
            width: 15%;
        }

        .auto-style3 {
            width: 60%;
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
            height: 35px;
        }
    </style>

    <script type="text/javascript">
        function UpdateGridHeight() {
            MainDBGrid.SetHeight(0);
            var containerHeight = ASPxClientUtils.GetDocumentClientHeight();
            if (document.body.scrollHeight > containerHeight)
                containerHeight = document.body.scrollHeight;
            MainDBGrid.SetHeight(containerHeight - topPanel.GetHeight() - MainDBGridMenu.GetHeight());
        }

        function OnDPTNOChanged(cbxBDPTNO) {
            cbxBPRSNNO.PerformCallback(cbxBDPTNO.GetSelectedItem().value.toString());
        }

        var timeout;
        function scheduleGridUpdate(grid) {
            window.clearTimeout(timeout);
            timeout = window.setTimeout(
                function () {
                    //grid.Refresh();
                    grid.PerformCallback();
                },
                20000
            );
        }
        function grid_Init(s, e) {
            scheduleGridUpdate(s);
        }
        function grid_BeginCallback(s, e) {
            window.clearTimeout(timeout);
        }
        function grid_EndCallback(s, e) {
            scheduleGridUpdate(s);
        }

        function UpdateMainDBGridZoneWidth() {
            //移到.cs的Page_Load事件設定寬度
            //MainDBGridZone.SetWidth(0);
            //var containerWidth = ASPxClientUtils.GetDocumentClientWidth();
            //if (document.body.scrollWidth > containerWidth)
            //    containerWidth = document.body.scrollWidth;
            //MainDBGridZone.SetWidth(containerWidth - LeftPanel.GetWidth());
            //MainDBGridZone.SetWidth(MainDBGrid.GetWidth());
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

        function OnBMLINEIDChanged(cbxBMLINEID) {
            cbxBMACHINID.PerformCallback(cbxBMLINEID.GetSelectedItem().value.toString());
        }

    </script>

</head>
<body>
    <form id="PKC00" runat="server">
        <uc1:MainMenuPanel runat="server" ID="MainMenuPanel1" />
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
            <SettingsAdaptivity CollapseAtWindowInnerWidth="580" />
            <PanelCollection>
                <dx:PanelContent runat="server">
                    <dx:ASPxGridView ID="MainDBGrid" runat="server" AutoGenerateColumns="False" ClientInstanceName="MainDBGrid" EnableTheming="True" CssClass="grid" Width="100%" OnCustomColumnDisplayText="MainDBGrid_CustomColumnDisplayText" KeyFieldName="WONO;WOSRNO" OnDataBinding="MainDBGrid_DataBinding" OnCustomCallback="MainDBGrid_CustomCallback">
                        <ClientSideEvents Init="grid_Init" BeginCallback="grid_BeginCallback" EndCallback="grid_EndCallback" />
                        <SettingsAdaptivity AdaptivityMode="HideDataCells" HideDataCellsAtWindowInnerWidth="600">
                            <AdaptiveDetailLayoutProperties>
                                <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="580" />
                            </AdaptiveDetailLayoutProperties>
                        </SettingsAdaptivity>

                        <SettingsPager NumericButtonCount="6" PageSize="20" Mode="ShowAllRecords">
                            <FirstPageButton Visible="True">
                            </FirstPageButton>
                            <LastPageButton Visible="True">
                            </LastPageButton>
                        </SettingsPager>
                        <SettingsEditing Mode="Batch">
                        </SettingsEditing>
                        <Settings VerticalScrollableHeight="500" VerticalScrollBarMode="Visible" />
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
                            <HeaderFilter MinHeight="140px"></HeaderFilter>
                        </SettingsPopup>

                        <SettingsSearchPanel CustomEditorID="SearchTextBox" />
                        <SettingsLoadingPanel Mode="ShowOnStatusBar" />
                        <SettingsText CommandCancel="取消" CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" CommandUpdate="存入" SearchPanelEditorNullText="請輸入關鍵字" Title="策略目標" />
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
            <ClientSideEvents PopUp="function(s, e) { tbxBSTPROCSTDT.Focus(); }" Init="function(s, e) {
pclSearchPanelInit();
}" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxPanel ID="ASPxPanel1" runat="server" DefaultButton="btOK">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <dx:ASPxCallbackPanel ID="cbkSearchPanel" runat="server" Width="100%" ClientInstanceName="cbkSearchPanel" OnCallback="cbkSearchPanel_Callback">
                                    <PanelCollection>
                                        <dx:PanelContent runat="server">
                                            <table width="100%">
                                                <tr>
                                                    <td rowspan="12" class="auto-style1">
                                                        <div class="pcmSideSpacer">
                                                        </div>
                                                    </td>
                                                    <td class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel2" runat="server" AssociatedControlID="lblBDPTNO" Text="產線名稱">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="cbxBMLINEID" runat="server" ClientInstanceName="cbxBMLINEID" DataSourceID="sdsManuLine" TextField="MLINENM" ValueField="MLINEID" Width="100%" AllowNull="True">
                                                            <ClientSideEvents SelectedIndexChanged="function(s, e) {OnBMLINEIDChanged(s)	
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
                                                        <dx:ASPxLabel ID="ASPxLabel3" runat="server" AssociatedControlID="lblBACNTYR" Text="機台名稱">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="cbxBMACHINID" runat="server" ClientInstanceName="cbxBMACHINID" DataSourceID="sdsMachine" TextField="MACHINNM" ValueField="MACHINID" Width="100%" AllowNull="True" NullText="全部機台" OnCallback="cbxBMACHINID_Callback">
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style5"></td>
                                                </tr>

                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel4" runat="server" AssociatedControlID="lblBACNTYR" Text="開始時程">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style3">

                                                        <dx:ASPxDateEdit ID="tbxBSTPROCSTDT" runat="server" Style="float: left; margin-right: 2px" ClientInstanceName="tbxBSTPROCSTDT" Width="38%" EditFormat="Custom" EditFormatString="yyyy/MM/dd" UseMaskBehavior="True">
                                                            <ValidationSettings CausesValidation="True" ErrorDisplayMode="Text" ErrorText="" SetFocusOnError="True" ValidateOnLeave="False">
                                                                <ErrorFrameStyle Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                                <RegularExpression ErrorText="" />
                                                                <RequiredField ErrorText="" />
                                                            </ValidationSettings>
                                                        </dx:ASPxDateEdit>
                                                        <dx:ASPxLabel ID="ASPxLabel10" runat="server" Style="float: left; margin-left: 2px; margin-right: 2px" AssociatedControlID="lblBACNTYR" Text="~" Theme="Moderno" Width="3%">
                                                        </dx:ASPxLabel>

                                                        <dx:ASPxDateEdit ID="tbxBEDPROCSTDT" runat="server" Style="float: left; margin-right: 2px" ClientInstanceName="tbxBEDPROCSTDT" Width="38%" EditFormat="Custom" EditFormatString="yyyy/MM/dd" UseMaskBehavior="True">
                                                            <DateRangeSettings StartDateEditID="tbxBSTPROCSTDT" />
                                                            <ValidationSettings CausesValidation="True" ErrorDisplayMode="Text" ErrorText="" SetFocusOnError="True" ValidateOnLeave="False">
                                                                <ErrorFrameStyle Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                                <RegularExpression ErrorText="" />
                                                                <RequiredField ErrorText="" />
                                                            </ValidationSettings>
                                                        </dx:ASPxDateEdit>
                                                        <dx:ASPxComboBox ID="edBDateRangeSel" runat="server" Style="float: right; margin-right: 0px" ClientInstanceName="edBDateRangeSel" Width="18%">
                                                            <ClientSideEvents ValueChanged="function(s, e) {
cbkSearchPanel.PerformCallback('edBDateRangeSel');
}" />

                                                            <Items>
                                                                <dx:ListEditItem Text="今天" Value="0" />
                                                                <dx:ListEditItem Text="昨天" Value="1" />
                                                                <dx:ListEditItem Text="明天" Value="2" />
                                                                <dx:ListEditItem Text="本週" Value="3" />
                                                                <dx:ListEditItem Text="上週" Value="4" />
                                                                <dx:ListEditItem Text="下週" Value="5" />
                                                                <dx:ListEditItem Text="本月" Value="6" />
                                                                <dx:ListEditItem Text="上月" Value="7" />
                                                                <dx:ListEditItem Text="下月" Value="8" />
                                                                <dx:ListEditItem Text="今年" Value="9" />
                                                                <dx:ListEditItem Text="去年" Value="10" />
                                                                <dx:ListEditItem Text="明年" Value="11" />
                                                            </Items>
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                    <td rowspan="7" class="auto-style1">
                                                        <div class="pcmSideSpacer">
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style2">&nbsp;</td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxLabel ID="lblErrorMessage" ClientInstanceName="lblErrorMessage" runat="server" ForeColor="#CC6600">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style2"></td>
                                                    <td class="auto-style7">
                                                        <div class="pcmButton">
                                                            <dx:ASPxButton ID="btnGoFilter" runat="server" AutoPostBack="False" OnClick="btnGoFilter_Click" Style="float: left; margin-right: 8px" Text="設定" Width="80px" CausesValidation="False" ValidationGroup="entryGroup">
                                                                <ClientSideEvents Click="function(s, e) {
}" />
                                                            </dx:ASPxButton>
                                                            <dx:ASPxButton ID="btnRCancel" runat="server" AutoPostBack="False" Style="float: right; margin-right: 8px" Text="取消" Width="80px">
                                                                <ClientSideEvents Click="function(s, e) { pclSearchPanel.Hide(); }" />
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
        <asp:SqlDataSource ID="sdsDpts" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Dpt.DPTNO, Dpt.DPTNM, Dpt.DPTHANDLER, Dpt.DPTADDR, Dpt.DPTZIPCD, Dpt.DPTTELNO, Dpt.DPTFAXNO, Dpt.DPTEMAILBOX,Dpt.WEBSITE ,Dpt.OPRTDESC ,Dpt.OPRTPERD, Dpt.PHOTOFILENM FROM Dpt WHERE Dpt.DISSOLVDT IS NULL"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsPrsns" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [PRSNNO], [PRSNNM] FROM [Prsn]
WHERE Prsn.DPTNO=@DPTNO AND Prsn.QUITDT IS NULL">
            <SelectParameters>
                <asp:SessionParameter Name="DPTNO" SessionField="DPTNO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsManuLine" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [ManuLine] WHERE [MLINEID] = @MLINEID" InsertCommand="INSERT INTO [ManuLine] ([MLINEID], [MLINENM], [FACTNO]) VALUES (@MLINEID, @MLINENM, @FACTNO)" SelectCommand="SELECT ManuLine.MLINEID, ManuLine.MLINENM, ManuLine.FACTNO, Fact.FACTNM
FROM ManuLine LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO)
ORDER BY ManuLine.MLINEID"
            UpdateCommand="UPDATE [ManuLine] SET [MLINENM] = @MLINENM, [FACTNO] = @FACTNO WHERE [MLINEID] = @MLINEID">
            <DeleteParameters>
                <asp:Parameter Name="MLINEID" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="MLINEID" Type="String" />
                <asp:Parameter Name="MLINENM" Type="String" />
                <asp:Parameter Name="FACTNO" Type="String" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="MLINENM" Type="String" />
                <asp:Parameter Name="FACTNO" Type="String" />
                <asp:Parameter Name="MLINEID" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMachine" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [Machine] WHERE [MACHINID] = @MACHINID" InsertCommand="INSERT INTO [Machine] ([MACHINID], [MACHINNM], [CAPACITY], [LOADRATE], [MACHINDESC], [MLINEID]) VALUES (@MACHINID, @MACHINNM, @CAPACITY, @LOADRATE, @MACHINDESC, @MLINEID)" SelectCommand="SELECT Machine.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, 
 Machine.MLINEID, ManuLine.MLINENM, Fact.FACTNM
FROM Machine LEFT JOIN ManuLine ON (Machine.MLINEID = ManuLine.MLINEID)
 LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO)
WHERE Machine.MLINEID=@MLINEID 
ORDER BY Machine.MACHINID"
            UpdateCommand="UPDATE [Machine] SET [MACHINNM] = @MACHINNM, [CAPACITY] = @CAPACITY, [LOADRATE] = @LOADRATE, [MACHINDESC] = @MACHINDESC, [MLINEID] = @MLINEID WHERE [MACHINID] = @MACHINID">
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
            <SelectParameters>
                <asp:SessionParameter Name="MLINEID" SessionField="MLINEID" />
            </SelectParameters>
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
