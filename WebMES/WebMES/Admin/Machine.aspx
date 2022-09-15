<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Machine.aspx.cs" Inherits="WebMES.Admin.Machine" %>

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
    </style>
    <script type="text/javascript">
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
    </script>

</head>
<body>
    <form id="form1" runat="server">
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
                        <SettingsText CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" SearchPanelEditorNullText="請輸入關鍵字" CommandCancel="取消" CommandUpdate="存入" Title="員工履歷" />
                        <EditFormLayoutProperties ColCount="3">
                        </EditFormLayoutProperties>
                        <Columns>
                            <dx:GridViewCommandColumn ShowDeleteButton="True" ShowEditButton="True" ShowInCustomizationForm="True" ShowNewButtonInHeader="True" VisibleIndex="0" Width="7%">
                            </dx:GridViewCommandColumn>
                            <dx:GridViewDataTextColumn Caption="機器代碼" FieldName="MACHINID" ShowInCustomizationForm="True" VisibleIndex="1" Width="8%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="機器名稱" FieldName="MACHINNM" ShowInCustomizationForm="True" VisibleIndex="2" Width="14%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="機器產能" FieldName="CAPACITY" ShowInCustomizationForm="True" VisibleIndex="3" Width="6%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="產線代碼" FieldName="MLINEID" ShowInCustomizationForm="True" VisibleIndex="4" Width="6%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="產線名稱" FieldName="MLINENM" ShowInCustomizationForm="True" VisibleIndex="5" Width="12%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="廠區名稱" FieldName="FACTNM" ShowInCustomizationForm="True" VisibleIndex="6" Width="12%">
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

        <dx:ASPxGridViewExporter ID="MainDBGridExporter" runat="server" GridViewID="MainDBGrid">
            <Styles>
                <Header Font-Names="Arial Unicode MS">
                </Header>
                <Cell Font-Names="Arial Unicode MS">
                </Cell>
            </Styles>
        </dx:ASPxGridViewExporter>
<asp:SqlDataSource ID="sdsMachine" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [Machine] WHERE [MACHINID] = @MACHINID" InsertCommand="INSERT INTO [Machine] ([MACHINID], [MACHINNM], [CAPACITY], [LOADRATE], [MACHINDESC], [MLINEID]) VALUES (@MACHINID, @MACHINNM, @CAPACITY, @LOADRATE, @MACHINDESC, @MLINEID)" SelectCommand="SELECT Machine.MACHINID, Machine.MACHINNM, Machine.CAPACITY, Machine.LOADRATE, Machine.MACHINDESC, 
 Machine.MLINEID, ManuLine.MLINENM, Fact.FACTNM
FROM Machine LEFT JOIN ManuLine ON (Machine.MLINEID = ManuLine.MLINEID)
 LEFT JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO) 
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
            <UpdateParameters>
                <asp:Parameter Name="MACHINNM" Type="String" />
                <asp:Parameter Name="CAPACITY" Type="Double" />
                <asp:Parameter Name="LOADRATE" Type="Double" />
                <asp:Parameter Name="MACHINDESC" Type="String" />
                <asp:Parameter Name="MLINEID" Type="String" />
                <asp:Parameter Name="MACHINID" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsManuLine" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [ManuLine] WHERE [MLINEID] = @MLINEID" InsertCommand="INSERT INTO [ManuLine] ([MLINEID], [MLINENM], [FACTNO]) VALUES (@MLINEID, @MLINENM, @FACTNO)" SelectCommand="SELECT ManuLine.MLINEID, ManuLine.MLINENM, ManuLine.FACTNO, Fact.FACTNM
FROM ManuLine INNER JOIN Fact ON (ManuLine.FACTNO = Fact.FACTNO)
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

    </form>
</body>
</html>
