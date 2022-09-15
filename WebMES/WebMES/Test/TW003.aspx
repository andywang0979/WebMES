<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TW003.aspx.cs" Inherits="WebMES.Admin.TW003" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/Admin/ManuMenuPanel.ascx" TagPrefix="uc1" TagName="ManuMenuPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0" />
    <title>查詢功能-繼承DevSFEditGrid</title>
    <link href="~/stylesheets/main.css" rel="stylesheet">
    <script type="text/javascript">
        //不知道做啥
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
        //查詢視窗使用
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
    <form id="form1" runat="server">
        <uc1:MainMenuPanel runat="server" ID="MainMenuPanel" />
        <uc1:ManuMenuPanel runat="server" ID="ManuMenuPanel" />
        <!--上方工具列 Start-->
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
        <dx:ASPxGridViewExporter ID="MainDBGridExporter" runat="server" GridViewID="MainDBGrid">
            <Styles>
                <Header Font-Names="Arial Unicode MS">
                </Header>
                <Cell Font-Names="Arial Unicode MS">
                </Cell>
            </Styles>
        </dx:ASPxGridViewExporter>
        <!--上方工具列 End-->

        <!--下方grid Start-->
        <dx:ASPxPanel ID="MainDBGridPanel" runat="server" ClientInstanceName="MainDBGridPanel" EnableTheming="True" Width="100%">
            <SettingsAdaptivity CollapseAtWindowInnerWidth="580" />
            <PanelCollection>
                <dx:PanelContent runat="server">
                    <dx:ASPxGridView ID="MainDBGrid" runat="server" ClientInstanceName="MainDBGrid" AutoGenerateColumns="False" DataSourceID="sdsMachine" EnableTheming="True" KeyFieldName="MACHINID" CssClass="grid" Width="100%" KeyboardSupport="True" OnCustomColumnDisplayText="MainDBGrid_CustomColumnDisplayText">
                        <SettingsAdaptivity AdaptivityMode="HideDataCells">
                        </SettingsAdaptivity>
                        <SettingsPager NumericButtonCount="6" PageSize="20">
                            <FirstPageButton Visible="True">
                            </FirstPageButton>
                            <LastPageButton Visible="True">
                            </LastPageButton>
                        </SettingsPager>
                        <SettingsEditing EditFormColumnCount="3" UseFormLayout="False" Mode="Inline">
                        </SettingsEditing>
                        <Settings VerticalScrollableHeight="500" VerticalScrollBarMode="Visible" />
                        <SettingsBehavior AllowSelectByRowClick="True" />
                        <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />
                        <SettingsPopup>
                            <EditForm AllowResize="True" Modal="True" Width="800px" />
                            <HeaderFilter MinHeight="140px"></HeaderFilter>
                            <FilterControl AutoUpdatePosition="False">
                            </FilterControl>
                        </SettingsPopup>
                        <SettingsSearchPanel CustomEditorID="SearchTextBox" />
                        <SettingsText CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" SearchPanelEditorNullText="請輸入關鍵字" CommandCancel="取消" CommandUpdate="存入" Title="員工履歷" />
                        <EditFormLayoutProperties ColCount="3">
                        </EditFormLayoutProperties>
                        <Columns>
                            <dx:GridViewCommandColumn ShowInCustomizationForm="True" VisibleIndex="0" Width="1%">
                            </dx:GridViewCommandColumn>
                            <dx:GridViewDataTextColumn Caption="機器代碼" FieldName="MACHINID" ShowInCustomizationForm="True" VisibleIndex="1" Width="8%" MaxWidth="115" MinWidth="105">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="機器名稱" FieldName="MACHINNM" ShowInCustomizationForm="True" VisibleIndex="2" Width="17%" MaxWidth="115" MinWidth="105">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="機器產能" FieldName="CAPACITY" ShowInCustomizationForm="True" VisibleIndex="3" Width="6%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="產線代碼" FieldName="MLINEID" ShowInCustomizationForm="True" VisibleIndex="4" Width="6%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="產線名稱" FieldName="MLINENM" ShowInCustomizationForm="True" VisibleIndex="5" Width="12%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="廠區名稱" FieldName="FACTNM" ShowInCustomizationForm="True" VisibleIndex="6" Width="15%">
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
        <asp:SqlDataSource ID="sdsMachine" runat="server" 
            ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" 
            SelectCommand="SELECT Machine.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, 
                             Machine.MLINEID, ManuLine.MLINENM, Fact.FACTNM
                            FROM Machine LEFT JOIN ManuLine ON (Machine.MLINEID = ManuLine.MLINEID)
                             LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO) 
                            ORDER BY Machine.MACHINID"
           >
        </asp:SqlDataSource>
        <!--下方grid End-->

        <!--查詢視窗 Start-->
        <dx:ASPxPopupControl ID="pclSearchPanel" runat="server" CloseAction="CloseButton" CloseOnEscape="True" Modal="True"
            PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pclSearchPanel"
            HeaderText="設定查詢條件" AllowDragging="True" PopupAnimationType="None" EnableViewState="False" Width="500px" Height="189px">
            <ClientSideEvents PopUp="function(s, e) { tbxBSTACPTDT.Focus(); }" 
                Init="function(s, e) {pclSearchPanelInit();}"
                Shown="function(s, e) {cbkSearchPanel.PerformCallback('pclSearchPanel');}" />
            <ContentCollection>
                <dx:PopupControlContentControl runat="server">
                    <dx:ASPxPanel ID="Panel1" runat="server" DefaultButton="btOK">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <dx:ASPxCallbackPanel ID="cbkSearchPanel" runat="server" Width="100%" ClientInstanceName="cbkSearchPanel" OnCallback="cbkSearchPanel_Callback">
                                    <PanelCollection>
                                        <dx:PanelContent runat="server">
                                            <table width="100%">
                                                <tr>
                                                    <td rowspan="7" class="auto-style1">
                                                        <div class="pcmSideSpacer">
                                                        </div>
                                                    </td>
                                                    <td class="auto-style2">
                                                        廠區</td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="edFactNo" runat="server" ClientInstanceName="edFactNo" DataSourceID="sdsFact" TextField="FACTNM" ValueField="FACTNO" Width="100%" AllowNull="True" SelectedIndex="1">
                                                        </dx:ASPxComboBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style2"></td>
                                                    <td class="auto-style2"></td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style2"></td>
                                                    <td class="auto-style7">
                                                        <div class="pcmButton">
                                                            <dx:ASPxButton ID="btnGoFilter" runat="server" AutoPostBack="False" OnClick="btnGoFilter_Click" Style="float: left; margin-right: 8px" Text="設定" Width="80px" CausesValidation="False" ValidationGroup="entryGroup">
                                                                <ClientSideEvents Click="function(s, e) {}" />
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
        <asp:SqlDataSource ID="sdsFact" runat="server" 
            ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" 
            SelectCommand="SELECT Fact.FACTNO, Fact.FACTNM FROM Fact  ORDER BY Fact.FACTNO" >
        </asp:SqlDataSource>
        <!--查詢視窗 End-->

        <!--還不知作用區-->
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
        <!--還不知作用區-->
    </form>
</body>
</html>
