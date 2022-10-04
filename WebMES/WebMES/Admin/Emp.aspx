<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Emp.aspx.cs" Inherits="WebMES.Admin.Emp" %>

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
                    <dx:ASPxGridView ID="MainDBGrid" runat="server" ClientInstanceName="MainDBGrid" AutoGenerateColumns="False" DataSourceID="sdsMachine" EnableTheming="True" KeyFieldName="EMPNO" CssClass="grid" Width="100%" KeyboardSupport="True" OnCustomColumnDisplayText="MainDBGrid_CustomColumnDisplayText">
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
                            <dx:GridViewDataTextColumn Caption="員工代碼" FieldName="EMPNO" ShowInCustomizationForm="True" VisibleIndex="1" Width="8%">
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn Caption="員工姓名" FieldName="EMPNM" ShowInCustomizationForm="True" VisibleIndex="1" Width="8%">
                            </dx:GridViewDataTextColumn>                   
                            <dx:GridViewDataTextColumn Caption="手機" FieldName="PHONE" ShowInCustomizationForm="True" VisibleIndex="1" Width="8%">
                            </dx:GridViewDataTextColumn>                            
                            <dx:GridViewDataTextColumn Caption="地址" FieldName="ADDRESS" ShowInCustomizationForm="True" VisibleIndex="1" Width="8%">
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

       <%-- <dx:ASPxPopupControl ID="pclSearchPanel" runat="server" CloseAction="CloseButton" CloseOnEscape="True" Modal="True"
            PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="pclSearchPanel"
            HeaderText="設定查詢條件" AllowDragging="True" PopupAnimationType="None" EnableViewState="False" Width="500px" Height="189px">
            <ClientSideEvents PopUp="function(s, e) { cbxBDPTNO.Focus(); }" Init="function(s, e) {
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
                                        <td class="auto-style2">
                                            <dx:ASPxLabel ID="ASPxLabel5" runat="server" AssociatedControlID="lblBDPTNO" Text="部門">
                                            </dx:ASPxLabel>
                                        </td>
                                        <td class="auto-style7">
                                            <dx:ASPxComboBox ID="cbxBDPTNO" runat="server" ClientInstanceName="cbxBDPTNO" DataSourceID="sdsDpts" TextField="DPTNM" ValueField="DPTNO" Width="100%">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) {OnBDPTNOChanged(s)	
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
                                            <dx:ASPxLabel ID="ASPxLabel1" runat="server" AssociatedControlID="lblBACNTYR" Text="設計師">
                                            </dx:ASPxLabel>
                                        </td>
                                        <td class="auto-style7">
                                            <dx:ASPxComboBox ID="cbxBPRSNNO" runat="server" ClientInstanceName="cbxBPRSNNO" TextField="PRSNNM" ValueField="PRSNNO" Width="100%" DataSourceID="sdsPrsns">
                                            </dx:ASPxComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style2">&nbsp;</td>
                                        <td class="auto-style7">&nbsp;</td>
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
                </dx:PopupControlContentControl>
            </ContentCollection>
            <ContentStyle>
                <Paddings PaddingBottom="5px" />
            </ContentStyle>
        </dx:ASPxPopupControl>      --%>
        <dx:ASPxGridViewExporter ID="MainDBGridExporter" runat="server" GridViewID="MainDBGrid">
            <Styles>
                <Header Font-Names="Arial Unicode MS">
                </Header>
                <Cell Font-Names="Arial Unicode MS">
                </Cell>
            </Styles>
        </dx:ASPxGridViewExporter>

        <asp:sqldatasource id="sdsMachine" runat="server" connectionstring="<%$ connectionstrings:winsistmplconnectionstring %>" providername="<%$ connectionstrings:winsistmplconnectionstring.providername %>" selectcommand="select * from emptest" InsertCommand="INSERT INTO emptest(EMPNO, EMPNM,PHONE, ADDRESS)
VALUES(@EMPNO,@EMPNM,@PHONE,@ADDRESS);" DeleteCommand="DELETE FROM emptest
WHERE EMPNO =@EMPNO ;" UpdateCommand="UPDATE EmpTest
SET EMPNO=@EMPNO, EMPNM=@EMPNM, PHONE=@PHONE,ADDRESS=@ADDRESS
WHERE EMPNO=@EMPNO;">
            <InsertParameters>
                <asp:Parameter Name="EMPNO" />
                <asp:Parameter Name="EMPNM" />
                <asp:Parameter Name="PHONE" />
                <asp:Parameter Name="ADDRESS" />
            </InsertParameters>
        </asp:sqldatasource>      

        <asp:SqlDataSource ID="sdsDpts" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Dpt.DPTNO, Dpt.DPTNM, Dpt.DPTHANDLER, Dpt.DPTADDR, Dpt.DPTZIPCD, Dpt.DPTTELNO, Dpt.DPTFAXNO, Dpt.DPTEMAILBOX,Dpt.WEBSITE ,Dpt.OPRTDESC ,Dpt.OPRTPERD, Dpt.PHOTOFILENM FROM Dpt WHERE Dpt.ISSHOP = 1"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsPrsns" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [PRSNNO], [PRSNNM] FROM [Prsn]
WHERE Prsn.DPTNO=@DPTNO AND Prsn.QUITDT IS NULL">
            <SelectParameters>
                <asp:SessionParameter Name="DPTNO" SessionField="DPTNO" />
            </SelectParameters>
        </asp:SqlDataSource>


    </form>
</body>
</html>
