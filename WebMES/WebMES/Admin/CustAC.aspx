<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustAC.aspx.cs" Inherits="WebMES.Admin.CustAC" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/Admin/ManuMenuPanel.ascx" TagPrefix="uc1" TagName="ManuMenuPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0" />
    <title></title>
    <style>
        html, body, form {
            height: 100%;
            padding: 0;
            margin: 0;
            overflow: hidden;
        }

        .main-menu {
            float: right !important;
            margin: 8px 0 4px;
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

        #tableBMANO {
            margin-bottom: 5px;
        }

            #tableBMANO td {
                padding-right: 5px;
            }
    </style>
    <script type="text/javascript">
        /*
        function OnNavgInsertClick(s, e) {
            MainTabPage.SetActiveTab(MainTabPage.GetTabByName('TabPageView'));
            sptPageView.GetPaneByName("sppPageView").SetContentUrl('CustACPage.aspx?EditMode=1');
        }
        function OnNavgEditClick(s, e) {
            MainDBGrid.GetRowValues(MainDBGrid.GetFocusedRowIndex(), 'CUSNO', OnNavgEditPopup);
        }

        function OnNavgEditPopup(values) {
            MainTabPage.SetActiveTab(MainTabPage.GetTabByName('TabPageView'));
            sptPageView.GetPaneByName("sppPageView").SetContentUrl('CustACPage.aspx?EditMode=2&CUSNO=' + values);
        }
        function OnNavgDeleteClick(s, e) {
            MainDBGrid.GetRowValues(MainDBGrid.GetFocusedRowIndex(), 'CUSNO', OnNavgDeletePopup);
        }

        function OnNavgDeletePopup(values) {
            MainTabPage.SetActiveTab(MainTabPage.GetTabByName('TabPageView'));
            sptPageView.GetPaneByName("sppPageView").SetContentUrl('CustACPage.aspx?EditMode=3&CUSNO=' + values);
        }
        */

        //提示框
        //function OnNavgDeleteClick(s, e) {
        //    pclCSalDeleteConfirm.Show();
        //}

        function OnNavgViewClick(s, e) {
            MainDBGrid.GetRowValues(MainDBGrid.GetFocusedRowIndex(), 'CUSNO', OnNavgViewPopup);
        }

        function OnNavgViewPopup(values) {
            MainTabPage.SetActiveTab(MainTabPage.GetTabByName('TabPageView'));
            sptPageView.GetPaneByName("sppPageView").SetContentUrl('CustACPage.aspx?EditMode=0&CUSNO=' + values);
        }


        function OnBDPTNOChanged(cbxBDPTNO) {
            cbxBPRSNNO.PerformCallback(cbxBDPTNO.GetSelectedItem().value.toString());
        }

        function MainTabPageTabClick() {
            if (MainTabPage.GetActiveTabIndex() == 0) {
                OnNavgViewClick();
                //sptPageView.GetPaneByName("sppPageView").SetContentUrl("CustACPage.aspx");
                //sptPageView.GetPaneByName("sppPageView").SetContentUrl('CustACPage.aspx?CUSNO=06824');
                //cbkfloCus.PerformCallback('NavgView');
            }
        }

        function MainTabPageToPageBrow(EditMode) {
            //切換至瀏覽頁籤
            MainTabPage.SetActiveTab(MainTabPage.GetTabByName('TabPageBrow'));
            if (EditMode > 0)
                //1:新增 2:更改 3:刪除
                MainDBGrid.Refresh();
        }

        //程式設定ActiveTab會觸發 TabPageTabChanging 和 TabPageTabChanged事件, 但不會觸發TabClick 事件
        //  在NavgEdit的Click事件執行 MainTabPage.SetActiveTab(MainTabPage.GetTabByName('TabPageView'));
        //  會觸發TabPageTabChanging 和 TabPageTabChanged事件
        //function MainTabPageTabChanged() {
        //    if (MainTabPage.GetActiveTabIndex() == 1) {
        //        cbkfloCus.PerformCallback('NavgView');
        //    }
        //}

        function UpdateGridHeight() {
            MainDBGrid.SetHeight(0);
            var containerHeight = ASPxClientUtils.GetDocumentClientHeight();
            if (document.body.scrollHeight > containerHeight)
                containerHeight = document.body.scrollHeight;
            MainDBGrid.SetHeight(containerHeight - topPanel.GetHeight() - MainDBGridMenu.GetHeight() - 60);

            //CusSalDBGrid.SetHeight(0);
            //var containerHeight = ASPxClientUtils.GetDocumentClientHeight();
            //if (document.body.scrollHeight > containerHeight)
            //    containerHeight = document.body.scrollHeight;
            //CusSalDBGrid.SetHeight(containerHeight - topPanel.GetHeight() - 400 - cbkPanelMANO.GetHeight()-60);
            //CusSalDBGrid.SetHeight(120);
            sptPageView.SetHeight(containerHeight - topPanel.GetHeight() - MainDBGridMenu.GetHeight() - 100);
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
    <form id="BG000" runat="server">
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
                    <dx:MenuItem Name="NavgFilter" ToolTip="進階查詢" GroupName="DataBar" AdaptivePriority="1">
                        <Image IconID="filter_crossdatasourcefiltering_32x32office2013"></Image>
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
                    <dx:MenuItem BeginGroup="true" Name="NavgRefresh" ToolTip="資料更新" GroupName="DataBar" AdaptivePriority="1">
                        <Image IconID="scheduling_recurrence_32x32office2013"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem ToolTip="資料列印" GroupName="DataBar" AdaptivePriority="1" Name="NavgPrint">
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
                    <dx:MenuItem ToolTip="儲存" GroupName="DataBar" Visible="False" AdaptivePriority="1">
                        <Image IconID="save_save_32x32"></Image>
                    </dx:MenuItem>
                </Items>
            </dx:ASPxMenu>
        </div>

        <dx:ASPxPageControl ID="MainTabPage" ClientInstanceName="MainTabPage" Width="100%" Height="100%" runat="server" CssClass="dxtcFixed" ActiveTabIndex="0">
            <TabPages>
                <dx:TabPage Text="瀏覽" Name="TabPageBrow">
                    <ContentCollection>
                        <dx:ContentControl ID="ContentControl1" runat="server">
                            <dx:ASPxGridView ID="MainDBGrid" runat="server" ClientInstanceName="MainDBGrid" AutoGenerateColumns="False" DataSourceID="sdsCus" EnableTheming="True" KeyFieldName="CUSNO" CssClass="grid" Width="100%" KeyboardSupport="True" OnDataBound="MainDBGrid_DataBound">
                                <ClientSideEvents CustomButtonClick="function(s, e) {
}" />
                                <SettingsDetail AllowOnlyOneMasterRowExpanded="True" ShowDetailButtons="False" />
                                <SettingsAdaptivity AdaptivityMode="HideDataCells">
                                    <AdaptiveDetailLayoutProperties>
                                        <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="580" />
                                    </AdaptiveDetailLayoutProperties>
                                </SettingsAdaptivity>
                                <SettingsPager NumericButtonCount="6" PageSize="20">
                                    <FirstPageButton Visible="True">
                                    </FirstPageButton>
                                    <LastPageButton Visible="True">
                                    </LastPageButton>
                                </SettingsPager>
                                <SettingsEditing Mode="PopupEditForm" EditFormColumnCount="3" UseFormLayout="False">
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
                                    <dx:GridViewCommandColumn VisibleIndex="0" MinWidth="80" MaxWidth="100" Width="5%">
                                        <HeaderTemplate>
                                            <dx:ASPxButton ID="NavgInsert" runat="server" AutoPostBack="False" RenderMode="Link" ToolTip="新增客戶" OnLoad="NavgInsert_Load">
                                                <ClientSideEvents Click="function(s, e) {OnNavgInsertClick(s, e); 
}" />
                                                <Image Url="~/Images/Navigator/Grid_Add.png">
                                                </Image>
                                            </dx:ASPxButton>
                                        </HeaderTemplate>

                                        <CustomButtons>
                                            <dx:GridViewCommandColumnCustomButton ID="NavgEdit" Text=" ">
                                                <Image ToolTip="編輯客戶" Url="~/Images/Navigator/Grid_Edit.png" />
                                            </dx:GridViewCommandColumnCustomButton>
                                            <dx:GridViewCommandColumnCustomButton ID="NavgDelete" Text=" ">
                                                <Image ToolTip="刪除客戶" Url="~/Images/Navigator/Grid_Delete.png" />
                                            </dx:GridViewCommandColumnCustomButton>
                                        </CustomButtons>
                                    </dx:GridViewCommandColumn>
                                    <dx:GridViewDataTextColumn Caption="客戶編號" FieldName="CUSNO" ReadOnly="True" ShowInCustomizationForm="True" VisibleIndex="1" MinWidth="90" MaxWidth="110" Width="7%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="客戶名稱" FieldName="CUSNM" ShowInCustomizationForm="True" VisibleIndex="2" MinWidth="90" MaxWidth="110" Width="7%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="行動電話" FieldName="CNTMOVTEL" ShowInCustomizationForm="True" VisibleIndex="9" MinWidth="90" MaxWidth="100" Width="8%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataDateColumn Caption="生日" FieldName="BIRDT" ShowInCustomizationForm="True" VisibleIndex="6" MinWidth="90" MaxWidth="100" Width="8%">
                                    </dx:GridViewDataDateColumn>
                                    <dx:GridViewDataTextColumn Caption="聯絡郵區" FieldName="CNTZIPCD" ShowInCustomizationForm="True" VisibleIndex="17" MinWidth="20" MaxWidth="40" Width="3%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="聯絡地址" FieldName="CNTADDR" ShowInCustomizationForm="True" VisibleIndex="18" MinWidth="100" MaxWidth="220" Width="30%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="聯絡電話" FieldName="CNTTELNO" ShowInCustomizationForm="True" VisibleIndex="16" MinWidth="90" MaxWidth="110" Width="9%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="客戶類別" FieldName="CUSKDNM" ShowInCustomizationForm="True" VisibleIndex="5" MinWidth="90" MaxWidth="100" Width="8%">
                                    </dx:GridViewDataTextColumn>
                                </Columns>
                                <Styles>
                                    <AlternatingRow Enabled="True">
                                    </AlternatingRow>
                                </Styles>
                            </dx:ASPxGridView>

                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
                <dx:TabPage Text="調閱" Name="TabPageView">
                    <ContentCollection>
                        <dx:ContentControl ID="ContentControl2" runat="server">

                            <dx:ASPxSplitter runat="server" ID="sptPageView" ResizingMode="Live" ClientInstanceName="sptPageView" Height="250px">
                                <Panes>
                                    <dx:SplitterPane Name="sppPageView" ScrollBars="Auto" ContentUrl="CustACPage.aspx">
                                        <ContentCollection>
<dx:SplitterContentControl runat="server"></dx:SplitterContentControl>
</ContentCollection>
                                    </dx:SplitterPane>
                                </Panes>
                            </dx:ASPxSplitter>

                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
            </TabPages>
            <ClientSideEvents TabClick="function(s, e) {
  MainTabPageTabClick();	
}" />
        </dx:ASPxPageControl>
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
        </dx:ASPxPopupControl>
        <dx:ASPxGridViewExporter ID="MainDBGridExporter" runat="server" GridViewID="MainDBGrid">
            <Styles>
                <Header Font-Names="Arial Unicode MS">
                </Header>
                <Cell Font-Names="Arial Unicode MS">
                </Cell>
            </Styles>
        </dx:ASPxGridViewExporter>


        <asp:SqlDataSource ID="sdsCus" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [Cus] WHERE [CUSNO] = @CUSNO" InsertCommand="INSERT INTO [Cus] ([CUSNO], [CUSCM], [CUSNM], [LOGONID], [CUSKDNO], [BIRDT], [IDNO], [SEXNO], [REGNO], [DPTNO], [PRSNNO], [BOSSNM], [BOSSTIT], [BOSSMOVTEL], [CRPADDR], [CRPZIPCD], [CRPTELNO], [CRPFAXNO], [CRPEMAILBOX], [CRPIPADDR], [CNTMAN], [CNTTIT], [CNTADDR], [CNTZIPCD], [CNTMOVTEL], [CNTTELNO], [CNTFAXNO], [CNTEMAILBOX], [DLYMAN], [DLYTIT], [DLYADDR], [DLYZIPCD], [DLYMOVTEL], [DLYTELNO], [DLYEMAILBOX], [INVCMAN], [INVCTIT], [INVCADDR], [INVCZIPCD], [INVCMOVTEL], [INVCTELNO], [INVCEMAILBOX], [AMTACNTNO], [CHKACNTNO], [BANKNO], [SAVENO], [TAXRATE], [TAXMDNO], [PAYTERMNO], [INVCMD], [ENABLED], [REMARK], [SELFPHOTO], [PASSNO], [PASSMDNO], [PASSVALDDAY], [LPASSCHGDT], [ISLOGIN], [LLOGINDT], [DUEAMT], [PAYAMT], [MAXCREDIT], [ORDCREDIT], [USECREDIT], [ISHOPAMT], [CSHOPAMT], [HAIRTXTRID], [HAIRAMNTID], [HAIRWCLRID], [HAIRCOLRID], [HAIRSTYLID], [HAIRSTYLDESC], [HAIRNOTICE], [LOGID], [LOGDT]) VALUES (@CUSNO, @CUSCM, @CUSNM, @LOGONID, @CUSKDNO, @BIRDT, @IDNO, @SEXNO, @REGNO, @DPTNO, @PRSNNO, @BOSSNM, @BOSSTIT, @BOSSMOVTEL, @CRPADDR, @CRPZIPCD, @CRPTELNO, @CRPFAXNO, @CRPEMAILBOX, @CRPIPADDR, @CNTMAN, @CNTTIT, @CNTADDR, @CNTZIPCD, @CNTMOVTEL, @CNTTELNO, @CNTFAXNO, @CNTEMAILBOX, @DLYMAN, @DLYTIT, @DLYADDR, @DLYZIPCD, @DLYMOVTEL, @DLYTELNO, @DLYEMAILBOX, @INVCMAN, @INVCTIT, @INVCADDR, @INVCZIPCD, @INVCMOVTEL, @INVCTELNO, @INVCEMAILBOX, @AMTACNTNO, @CHKACNTNO, @BANKNO, @SAVENO, @TAXRATE, @TAXMDNO, @PAYTERMNO, @INVCMD, @ENABLED, @REMARK, @SELFPHOTO, @PASSNO, @PASSMDNO, @PASSVALDDAY, @LPASSCHGDT, @ISLOGIN, @LLOGINDT, @DUEAMT, @PAYAMT, @MAXCREDIT, @ORDCREDIT, @USECREDIT, @ISHOPAMT, @CSHOPAMT, @HAIRTXTRID, @HAIRAMNTID, @HAIRWCLRID, @HAIRCOLRID, @HAIRSTYLID, @HAIRSTYLDESC, @HAIRNOTICE, @LOGID, @LOGDT)" SelectCommand="SELECT Cus.CUSNO, Cus.CUSCM, Cus.CUSNM, Cus.LOGONID, Cus.CUSKDNO, Cuskd.CUSKDNM,
 Cus.BIRDT, Cus.IDNO, Cus.SEXNO, Cus.REGNO, Cus.DPTNO, Cus.PRSNNO, Cus.BOSSNM,
 Cus.BOSSTIT, Cus.BOSSMOVTEL, Cus.CRPADDR, Cus.CRPZIPCD, Cus.CRPTELNO, Cus.CRPFAXNO,
 Cus.CRPEMAILBOX, Cus.CRPIPADDR, Cus.CNTMAN, Cus.CNTTIT, Cus.CNTADDR, Cus.CNTZIPCD,
 Cus.CNTMOVTEL, Cus.CNTTELNO, Cus.CNTFAXNO, Cus.CNTEMAILBOX, Cus.DLYMAN,
 Cus.DLYTIT, Cus.DLYADDR, Cus.DLYZIPCD, Cus.DLYMOVTEL, Cus.DLYTELNO,
 Cus.DLYEMAILBOX, Cus.INVCMAN, Cus.INVCTIT, Cus.INVCADDR, Cus.INVCZIPCD,
 Cus.INVCMOVTEL, Cus.INVCTELNO, Cus.INVCEMAILBOX, Cus.AMTACNTNO, Cus.CHKACNTNO,
 Cus.BANKNO, Cus.SAVENO, Cus.TAXRATE, Cus.TAXMDNO, Cus.PAYTERMNO, Cus.INVCMD,
 Cus.ENABLED, Cus.REMARK, Cus.SELFPHOTO, Cus.DUEAMT, Cus.PAYAMT,
 Cus.MAXCREDIT, Cus.ORDCREDIT, Cus.USECREDIT, Cus.ISHOPAMT, Cus.CSHOPAMT,
 Cus.HAIRTXTRID, Cus.HAIRAMNTID, Cus.HAIRWCLRID, Cus.HAIRCOLRID, Cus.HAIRSTYLID, Cus.HAIRSTYLDESC,
 Cus.PASSNO, Cus.ISLOGIN, Cus.LLOGINDT, Cus.PASSMDNO, Cus.PASSVALDDAY, Cus.LPASSCHGDT,
 Cus.LOGID, Cus.LOGDT
FROM Cus LEFT JOIN Cuskd ON (Cus.CUSKDNO=Cuskd.CUSKDNO)"
            UpdateCommand="UPDATE [Cus] SET [CUSCM] = @CUSCM, [CUSNM] = @CUSNM, [LOGONID] = @LOGONID, [CUSKDNO] = @CUSKDNO, [BIRDT] = @BIRDT, [IDNO] = @IDNO, [SEXNO] = @SEXNO, [REGNO] = @REGNO, [DPTNO] = @DPTNO, [PRSNNO] = @PRSNNO, [BOSSNM] = @BOSSNM, [BOSSTIT] = @BOSSTIT, [BOSSMOVTEL] = @BOSSMOVTEL, [CRPADDR] = @CRPADDR, [CRPZIPCD] = @CRPZIPCD, [CRPTELNO] = @CRPTELNO, [CRPFAXNO] = @CRPFAXNO, [CRPEMAILBOX] = @CRPEMAILBOX, [CRPIPADDR] = @CRPIPADDR, [CNTMAN] = @CNTMAN, [CNTTIT] = @CNTTIT, [CNTADDR] = @CNTADDR, [CNTZIPCD] = @CNTZIPCD, [CNTMOVTEL] = @CNTMOVTEL, [CNTTELNO] = @CNTTELNO, [CNTFAXNO] = @CNTFAXNO, [CNTEMAILBOX] = @CNTEMAILBOX, [DLYMAN] = @DLYMAN, [DLYTIT] = @DLYTIT, [DLYADDR] = @DLYADDR, [DLYZIPCD] = @DLYZIPCD, [DLYMOVTEL] = @DLYMOVTEL, [DLYTELNO] = @DLYTELNO, [DLYEMAILBOX] = @DLYEMAILBOX, [INVCMAN] = @INVCMAN, [INVCTIT] = @INVCTIT, [INVCADDR] = @INVCADDR, [INVCZIPCD] = @INVCZIPCD, [INVCMOVTEL] = @INVCMOVTEL, [INVCTELNO] = @INVCTELNO, [INVCEMAILBOX] = @INVCEMAILBOX, [AMTACNTNO] = @AMTACNTNO, [CHKACNTNO] = @CHKACNTNO, [BANKNO] = @BANKNO, [SAVENO] = @SAVENO, [TAXRATE] = @TAXRATE, [TAXMDNO] = @TAXMDNO, [PAYTERMNO] = @PAYTERMNO, [INVCMD] = @INVCMD, [ENABLED] = @ENABLED, [REMARK] = @REMARK, [SELFPHOTO] = @SELFPHOTO, [PASSNO] = @PASSNO, [PASSMDNO] = @PASSMDNO, [PASSVALDDAY] = @PASSVALDDAY, [LPASSCHGDT] = @LPASSCHGDT, [ISLOGIN] = @ISLOGIN, [LLOGINDT] = @LLOGINDT, [DUEAMT] = @DUEAMT, [PAYAMT] = @PAYAMT, [MAXCREDIT] = @MAXCREDIT, [ORDCREDIT] = @ORDCREDIT, [USECREDIT] = @USECREDIT, [ISHOPAMT] = @ISHOPAMT, [CSHOPAMT] = @CSHOPAMT, [HAIRTXTRID] = @HAIRTXTRID, [HAIRAMNTID] = @HAIRAMNTID, [HAIRWCLRID] = @HAIRWCLRID, [HAIRCOLRID] = @HAIRCOLRID, [HAIRSTYLID] = @HAIRSTYLID, [HAIRSTYLDESC] = @HAIRSTYLDESC, [HAIRNOTICE] = @HAIRNOTICE, [LOGID] = @LOGID, [LOGDT] = @LOGDT WHERE [CUSNO] = @CUSNO">
            <DeleteParameters>
                <asp:Parameter Name="CUSNO" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="CUSNO" Type="String" />
                <asp:Parameter Name="CUSCM" Type="String" />
                <asp:Parameter Name="CUSNM" Type="String" />
                <asp:Parameter Name="LOGONID" Type="String" />
                <asp:Parameter Name="CUSKDNO" Type="String" />
                <asp:Parameter Name="BIRDT" Type="DateTime" />
                <asp:Parameter Name="IDNO" Type="String" />
                <asp:Parameter Name="SEXNO" Type="String" />
                <asp:Parameter Name="REGNO" Type="String" />
                <asp:Parameter Name="DPTNO" Type="String" />
                <asp:Parameter Name="PRSNNO" Type="String" />
                <asp:Parameter Name="BOSSNM" Type="String" />
                <asp:Parameter Name="BOSSTIT" Type="String" />
                <asp:Parameter Name="BOSSMOVTEL" Type="String" />
                <asp:Parameter Name="CRPADDR" Type="String" />
                <asp:Parameter Name="CRPZIPCD" Type="String" />
                <asp:Parameter Name="CRPTELNO" Type="String" />
                <asp:Parameter Name="CRPFAXNO" Type="String" />
                <asp:Parameter Name="CRPEMAILBOX" Type="String" />
                <asp:Parameter Name="CRPIPADDR" Type="String" />
                <asp:Parameter Name="CNTMAN" Type="String" />
                <asp:Parameter Name="CNTTIT" Type="String" />
                <asp:Parameter Name="CNTADDR" Type="String" />
                <asp:Parameter Name="CNTZIPCD" Type="String" />
                <asp:Parameter Name="CNTMOVTEL" Type="String" />
                <asp:Parameter Name="CNTTELNO" Type="String" />
                <asp:Parameter Name="CNTFAXNO" Type="String" />
                <asp:Parameter Name="CNTEMAILBOX" Type="String" />
                <asp:Parameter Name="DLYMAN" Type="String" />
                <asp:Parameter Name="DLYTIT" Type="String" />
                <asp:Parameter Name="DLYADDR" Type="String" />
                <asp:Parameter Name="DLYZIPCD" Type="String" />
                <asp:Parameter Name="DLYMOVTEL" Type="String" />
                <asp:Parameter Name="DLYTELNO" Type="String" />
                <asp:Parameter Name="DLYEMAILBOX" Type="String" />
                <asp:Parameter Name="INVCMAN" Type="String" />
                <asp:Parameter Name="INVCTIT" Type="String" />
                <asp:Parameter Name="INVCADDR" Type="String" />
                <asp:Parameter Name="INVCZIPCD" Type="String" />
                <asp:Parameter Name="INVCMOVTEL" Type="String" />
                <asp:Parameter Name="INVCTELNO" Type="String" />
                <asp:Parameter Name="INVCEMAILBOX" Type="String" />
                <asp:Parameter Name="AMTACNTNO" Type="String" />
                <asp:Parameter Name="CHKACNTNO" Type="String" />
                <asp:Parameter Name="BANKNO" Type="String" />
                <asp:Parameter Name="SAVENO" Type="String" />
                <asp:Parameter Name="TAXRATE" Type="Int16" />
                <asp:Parameter Name="TAXMDNO" Type="String" />
                <asp:Parameter Name="PAYTERMNO" Type="String" />
                <asp:Parameter Name="INVCMD" Type="String" />
                <asp:Parameter Name="ENABLED" Type="Boolean" />
                <asp:Parameter Name="REMARK" Type="String" />
                <asp:Parameter Name="SELFPHOTO" Type="Object" />
                <asp:Parameter Name="PASSNO" Type="String" />
                <asp:Parameter Name="PASSMDNO" Type="String" />
                <asp:Parameter Name="PASSVALDDAY" Type="Int32" />
                <asp:Parameter Name="LPASSCHGDT" Type="DateTime" />
                <asp:Parameter Name="ISLOGIN" Type="Boolean" />
                <asp:Parameter Name="LLOGINDT" Type="DateTime" />
                <asp:Parameter Name="DUEAMT" Type="Double" />
                <asp:Parameter Name="PAYAMT" Type="Double" />
                <asp:Parameter Name="MAXCREDIT" Type="Double" />
                <asp:Parameter Name="ORDCREDIT" Type="Double" />
                <asp:Parameter Name="USECREDIT" Type="Double" />
                <asp:Parameter Name="ISHOPAMT" Type="Double" />
                <asp:Parameter Name="CSHOPAMT" Type="Double" />
                <asp:Parameter Name="HAIRTXTRID" Type="Int16" />
                <asp:Parameter Name="HAIRAMNTID" Type="Int16" />
                <asp:Parameter Name="HAIRWCLRID" Type="Int16" />
                <asp:Parameter Name="HAIRCOLRID" Type="Int16" />
                <asp:Parameter Name="HAIRSTYLID" Type="Int16" />
                <asp:Parameter Name="HAIRSTYLDESC" Type="String" />
                <asp:Parameter Name="HAIRNOTICE" Type="String" />
                <asp:Parameter Name="LOGID" Type="String" />
                <asp:Parameter Name="LOGDT" Type="DateTime" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="CUSCM" Type="String" />
                <asp:Parameter Name="CUSNM" Type="String" />
                <asp:Parameter Name="LOGONID" Type="String" />
                <asp:Parameter Name="CUSKDNO" Type="String" />
                <asp:Parameter Name="BIRDT" Type="DateTime" />
                <asp:Parameter Name="IDNO" Type="String" />
                <asp:Parameter Name="SEXNO" Type="String" />
                <asp:Parameter Name="REGNO" Type="String" />
                <asp:Parameter Name="DPTNO" Type="String" />
                <asp:Parameter Name="PRSNNO" Type="String" />
                <asp:Parameter Name="BOSSNM" Type="String" />
                <asp:Parameter Name="BOSSTIT" Type="String" />
                <asp:Parameter Name="BOSSMOVTEL" Type="String" />
                <asp:Parameter Name="CRPADDR" Type="String" />
                <asp:Parameter Name="CRPZIPCD" Type="String" />
                <asp:Parameter Name="CRPTELNO" Type="String" />
                <asp:Parameter Name="CRPFAXNO" Type="String" />
                <asp:Parameter Name="CRPEMAILBOX" Type="String" />
                <asp:Parameter Name="CRPIPADDR" Type="String" />
                <asp:Parameter Name="CNTMAN" Type="String" />
                <asp:Parameter Name="CNTTIT" Type="String" />
                <asp:Parameter Name="CNTADDR" Type="String" />
                <asp:Parameter Name="CNTZIPCD" Type="String" />
                <asp:Parameter Name="CNTMOVTEL" Type="String" />
                <asp:Parameter Name="CNTTELNO" Type="String" />
                <asp:Parameter Name="CNTFAXNO" Type="String" />
                <asp:Parameter Name="CNTEMAILBOX" Type="String" />
                <asp:Parameter Name="DLYMAN" Type="String" />
                <asp:Parameter Name="DLYTIT" Type="String" />
                <asp:Parameter Name="DLYADDR" Type="String" />
                <asp:Parameter Name="DLYZIPCD" Type="String" />
                <asp:Parameter Name="DLYMOVTEL" Type="String" />
                <asp:Parameter Name="DLYTELNO" Type="String" />
                <asp:Parameter Name="DLYEMAILBOX" Type="String" />
                <asp:Parameter Name="INVCMAN" Type="String" />
                <asp:Parameter Name="INVCTIT" Type="String" />
                <asp:Parameter Name="INVCADDR" Type="String" />
                <asp:Parameter Name="INVCZIPCD" Type="String" />
                <asp:Parameter Name="INVCMOVTEL" Type="String" />
                <asp:Parameter Name="INVCTELNO" Type="String" />
                <asp:Parameter Name="INVCEMAILBOX" Type="String" />
                <asp:Parameter Name="AMTACNTNO" Type="String" />
                <asp:Parameter Name="CHKACNTNO" Type="String" />
                <asp:Parameter Name="BANKNO" Type="String" />
                <asp:Parameter Name="SAVENO" Type="String" />
                <asp:Parameter Name="TAXRATE" Type="Int16" />
                <asp:Parameter Name="TAXMDNO" Type="String" />
                <asp:Parameter Name="PAYTERMNO" Type="String" />
                <asp:Parameter Name="INVCMD" Type="String" />
                <asp:Parameter Name="ENABLED" Type="Boolean" />
                <asp:Parameter Name="REMARK" Type="String" />
                <asp:Parameter Name="SELFPHOTO" Type="Object" />
                <asp:Parameter Name="PASSNO" Type="String" />
                <asp:Parameter Name="PASSMDNO" Type="String" />
                <asp:Parameter Name="PASSVALDDAY" Type="Int32" />
                <asp:Parameter Name="LPASSCHGDT" Type="DateTime" />
                <asp:Parameter Name="ISLOGIN" Type="Boolean" />
                <asp:Parameter Name="LLOGINDT" Type="DateTime" />
                <asp:Parameter Name="DUEAMT" Type="Double" />
                <asp:Parameter Name="PAYAMT" Type="Double" />
                <asp:Parameter Name="MAXCREDIT" Type="Double" />
                <asp:Parameter Name="ORDCREDIT" Type="Double" />
                <asp:Parameter Name="USECREDIT" Type="Double" />
                <asp:Parameter Name="ISHOPAMT" Type="Double" />
                <asp:Parameter Name="CSHOPAMT" Type="Double" />
                <asp:Parameter Name="HAIRTXTRID" Type="Int16" />
                <asp:Parameter Name="HAIRAMNTID" Type="Int16" />
                <asp:Parameter Name="HAIRWCLRID" Type="Int16" />
                <asp:Parameter Name="HAIRCOLRID" Type="Int16" />
                <asp:Parameter Name="HAIRSTYLID" Type="Int16" />
                <asp:Parameter Name="HAIRSTYLDESC" Type="String" />
                <asp:Parameter Name="HAIRNOTICE" Type="String" />
                <asp:Parameter Name="LOGID" Type="String" />
                <asp:Parameter Name="LOGDT" Type="DateTime" />
                <asp:Parameter Name="CUSNO" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>


        <asp:SqlDataSource ID="sdsCusSal" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Sal.CUSNO, Sal.SALNO, Sal.SALDT, Sal.DPTNO, Dpt.DPTNM, Sald.SALMDID,
 Sald.MANO, Sald.SALQTY, Sald.SLUNITNM, Sald.SALAMT, Ma.MADESC, Ma.MASPEC
FROM Sal INNER JOIN Sald ON (Sal.SALNO=Sald.SALNO)
 LEFT JOIN Dpt ON (Sal.DPTNO=Dpt.DPTNO)
 LEFT JOIN Ma ON (Sald.SALMDID=Ma.MACDNO AND Sald.MANO=Ma.MANO)
WHERE Sal.CUSNO=@CUSNO
ORDER BY Sal.SALDT DESC">
            <SelectParameters>
                <asp:SessionParameter Name="CUSNO" SessionField="CUSNO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsCusCCard" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Ma.MAKDNO, CCardAcnt.CARDID, Ma.MADESC, CCardAcnt.CARDNO, CCardAcnt.CUSNO, Cus.CUSNM,
 CCardAcnt.DUEAMT, CCardAcnt.DUECNT, CCardAcnt.INITDT, CCardAcnt.EXPRDT,
 CCardAcnt.ISHOPAMT, CCardAcnt.CSHOPAMT
FROM CCardAcnt LEFT JOIN Cus ON (CCardAcnt.CUSNO=Cus.CUSNO)
 LEFT JOIN Ma ON (CCardAcnt.CARDID=Ma.MANO)">
            <SelectParameters>
                <asp:SessionParameter Name="CUSNO" SessionField="CUSNO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsDpts" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Dpt.DPTNO, Dpt.DPTNM, Dpt.DPTHANDLER, Dpt.DPTADDR, Dpt.DPTZIPCD, Dpt.DPTTELNO, Dpt.DPTFAXNO, Dpt.DPTEMAILBOX,Dpt.WEBSITE ,Dpt.OPRTDESC ,Dpt.OPRTPERD, Dpt.PHOTOFILENM FROM Dpt WHERE Dpt.ISSHOP = 1"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsPrsns" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [PRSNNO], [PRSNNM] FROM [Prsn]
WHERE Prsn.DPTNO=@DPTNO AND Prsn.QUITDT IS NULL">
            <SelectParameters>
                <asp:SessionParameter Name="DPTNO" SessionField="DPTNO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMaInMacd" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB,
 Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST,
 Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO
FROM Ma
WHERE Ma.MACDNO=@MACDNO">
            <SelectParameters>
                <asp:SessionParameter Name="MACDNO" SessionField="MACDNO" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsMa" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB,
 Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.ISINV, Ma.UNITCOST,
 Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO
FROM Ma WHERE Ma.MANO IS NULL"></asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsMakd" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [MACDNO], [MAKDNO], [MAKDNM], [ISRESERV] FROM [Makd]
WHERE MACDNO=@MACDNO">
            <SelectParameters>
                <asp:SessionParameter Name="MACDNO" SessionField="MACDNO" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsMas" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB,
 Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.UNITCOST,
 Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO
FROM Ma
WHERE Ma.MAKDNO=@MAKDNO">
            <SelectParameters>
                <asp:SessionParameter Name="MAKDNO" SessionField="MAKDNO" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsStores" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [STORID], [STORNM] FROM [Stor]"></asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsPrsn" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Prsn.PRSNNO, Prsn.PRSNNM, Prsn.DPTNO,  Prsn.TITLEID
FROM Prsn 
WHERE Prsn.QUITDT IS NULL 
ORDER BY  Prsn.PRSNNO"></asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsUnit" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [UNITNO], [UNITNM] FROM [Unit]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMaPop" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>"></asp:SqlDataSource>

    </form>
</body>
</html>
