<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustACPage.aspx.cs" Inherits="WebMES.Admin.CustACPage" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/Admin/ManuMenuPanel.ascx" TagPrefix="uc1" TagName="ManuMenuPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0" />
    <title></title>
    <style type="text/css">
        .main-menu {
            float: right !important;
            margin: 8px 0 4px;
        }
    </style>
    <script type="text/javascript">
        function OnGoodsClick(CurMACDNO) {
            cvwMa.PerformCallback(CurMACDNO);
        }

        var postponedCallbackRequired = false;
        function tbxBMANOValidation(s, e) {
            //if (!postponedCallbackRequired) {
            //    MaShftdDBGrid.PerformCallback(tbxBMANO.GetValue());
            //    postponedCallbackRequired = true;
            //}
            MaShftdDBGrid.PerformCallback(tbxBMANO.GetValue());
        }
        function OnEndCallback(s, e) {
            if (postponedCallbackRequired) {
                MaShftdDBGrid.PerformCallback(tbxBMANO.GetValue());
            }
            postponedCallbackRequired = false;
        }

        //
        //function OnMANOChanged(edMANO) {
        //    MaShftdDBGrid.GetEditor("MANO").PerformCallback(edMANO.GetValue().toString());
        //}


        function EnterToTab(s, e) {
            if (event.keyCode == 13)
                event.keyCode = 9;
        }

        //偵測螢幕寬度自動調整ASPxPopupControl的顯示寬度 = 螢幕寬度 * 0.6, 以便適應手機平板電腦的螢幕寬度
        //
        function SelectCustPanelInit(s, e) {
            AdjustSize();
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                AdjustSize();
                if (pclSelectCustPanel.IsVisible())
                    pclSelectCustPanel.UpdatePosition();
            });
        }

        function AdjustSize() {
            var width = Math.max(0, document.documentElement.clientWidth) * 0.6;
            pclSelectCustPanel.SetWidth(width);
        }

    </script>

</head>
<body>
    <form id="BD000" runat="server">
        <uc1:MainMenuPanel runat="server" ID="MainMenuPanel" />
        <uc1:ManuMenuPanel runat="server" ID="ManuMenuPanel" />
        <dx:ASPxHiddenField runat="server" ID="hfdPassnoChanged" ClientInstanceName="hfdPassnoChanged">
        </dx:ASPxHiddenField>
sdsCus        <dx:ASPxCallbackPanel ID="cbkfloCust" runat="server" Width="100%" ClientInstanceName="cbkfloCust" OnCallback="cbkfloCust_Callback">
            <PanelCollection>
                <dx:PanelContent runat="server">
                    <dx:ASPxFormLayout ID="floCust" runat="server" ColCount="3" Width="100%" EnableTheming="True" AlignItemCaptionsInAllGroups="True" ShowItemCaptionColon="False" ColumnCount="3" DataSourceID="sdsCus" ClientInstanceName="floCust" Theme="Office2010Black" Font-Size="Small">
                        <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="800" />
                        <Items>
                            <dx:LayoutGroup Caption="客戶資訊" ColCount="3" Width="100%">
                                <Items>
                                    <dx:LayoutItem Caption="客戶姓名" FieldName="CUSNM">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCUSNM" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="客戶簡稱" FieldName="CUSCM">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCUSCM" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="客戶編號" FieldName="CUSNO">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCUSNO" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="出生日期" FieldName="BIRDT">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxDateEdit ID="edBIRDT" runat="server">
                                                </dx:ASPxDateEdit>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="身證字號" FieldName="IDNO">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edIDNO" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="性別" ColSpan="1" FieldName="SEXNO">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxComboBox ID="edSEXNO" runat="server">
                                                    <Items>
                                                        <dx:ListEditItem Text="男" Value="1" />
                                                        <dx:ListEditItem Text="女" Value="2" />
                                                        <dx:ListEditItem Text="其他" Value="3" />
                                                    </Items>
                                                </dx:ASPxComboBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="登入帳號" FieldName="LOGONID" ColSpan="1">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edLOGONID" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="登入密碼" FieldName="PASSNO">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edPASSNO" runat="server" Password="True" OnDataBound="edPASSNO_DataBound">
                                                    <ClientSideEvents ValueChanged="function(s, e) {
  hfdPassnoChanged.Set('PassnoChanged',true);	
}" />
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="啟用帳戶" ColSpan="1" FieldName="ENABLED">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxCheckBox ID="edENABLED" runat="server" CheckState="Unchecked">
                                                </dx:ASPxCheckBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="登錄店家" ColSpan="1" FieldName="DPTNO">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxComboBox ID="edDPTNO" runat="server" DataSourceID="sdsDpts" TextField="DPTNM" ValueField="DPTNO">
                                                    <ClientSideEvents ValueChanged="function(s, e) {
  cbkfloCust.PerformCallback('edDPTNO');
}" />
                                                </dx:ASPxComboBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="設計師" ColSpan="1" FieldName="PRSNNO">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxComboBox ID="edPRSNNO" runat="server" DataSourceID="sdsPrsns" TextField="PRSNNM" ValueField="PRSNNO">
                                                </dx:ASPxComboBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                </Items>
                            </dx:LayoutGroup>
                            <dx:LayoutGroup Caption="聯絡資訊" ColCount="3" Width="100%">
                                <Items>
                                    <dx:LayoutItem Caption="行動電話" FieldName="CNTMOVTEL">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCNTMOVTEL" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="聯絡電話" FieldName="CNTTELNO">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCNTTELNO" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="聯絡傳真" FieldName="CNTFAXNO">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCNTFAXNO" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="聯絡地址" FieldName="CNTADDR" ColSpan="2" Width="66.66%">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCNTADDR" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="聯絡郵區" FieldName="CNTZIPCD" Width="33.33%">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                    <dx:ASPxComboBox ID="edCNTZIPCD" runat="server" DataSourceID="sdsZipDist" TextField="DISTNM" ValueField="ZIPCD" TextFormatString="{0} {1}" CallbackPageSize="20">
                                                        <Columns>
                                                            <dx:ListBoxColumn Caption="代碼" FieldName="ZIPCD" />
                                                            <dx:ListBoxColumn Caption="區域" FieldName="DISTNM" />
                                                        </Columns>
                                                    </dx:ASPxComboBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="電子信箱" FieldName="CNTEMAILBOX">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCNTEMAILBOX" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                </Items>
                            </dx:LayoutGroup>
                            <dx:LayoutGroup Caption="消費資訊" ColCount="3" Width="100%">
                                <Items>
                                    <dx:LayoutItem Caption="啟用點數" FieldName="ISHOPAMT">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edISHOPAMT" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="+消費點數" FieldName="CSHOPAMT">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edCSHOPAMT" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="=累積點數" FieldName="TSHOPAMT">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxTextBox ID="edPAYAMT" runat="server">
                                                </dx:ASPxTextBox>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                </Items>
                            </dx:LayoutGroup>

                            <dx:LayoutGroup ColCount="3" ShowCaption="False" Width="100%">
                                <Items>
                                    <dx:LayoutItem FieldName="REMARK" Caption="備    註" ColSpan="3" Width="100%">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxMemo ID="edREMARK" runat="server" Height="90px" Width="100%">
                                                </dx:ASPxMemo>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>

                                </Items>
                            </dx:LayoutGroup>
                            <dx:LayoutGroup ColCount="3" ShowCaption="False" Width="100%" Name="lgpSubmit">
                                <GroupBoxStyle>
                                    <border borderstyle="Dotted" />
                                </GroupBoxStyle>
                                <Items>
                                    <dx:LayoutItem Caption="" ColSpan="2" ColumnSpan="2" Width="50%">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxLabel ID="lblErrorMessage" runat="server" ForeColor="#CC6600">
                                                </dx:ASPxLabel>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="" Name="limbtnSave" HorizontalAlign="Right">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxButton ID="btnSave" runat="server" AutoPostBack="False" OnClick="btnSave_Click" Text="儲存" Width="50%">
                                                    <Image IconID="iconbuilder_actions_check_svg_16x16">
                                                    </Image>
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnCancel" runat="server" AutoPostBack="False" OnClick="btnCancel_Click" Text="取消" Width="50%">
                                                    <Image IconID="iconbuilder_actions_delete_svg_16x16">
                                                    </Image>
                                                </dx:ASPxButton>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>

                                </Items>
                            </dx:LayoutGroup>
                        </Items>
                    </dx:ASPxFormLayout>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

        <dx:ASPxPageControl Width="100%" ID="ASPxPageControl1" runat="server" ActiveTabIndex="0"
            EnableViewState="False">
            <TabPages>
                <dx:TabPage Name="TabPageCSal" Text="消費紀錄">
                    <ContentCollection>
                        <dx:ContentControl runat="server">
                            <code runat="server" id="CSharpCodeHolder">
                                <dx:ASPxCallbackPanel ID="cbkPanelMANO" runat="server" ClientInstanceName="cbkPanelMANO" Width="100%">
                                    <PanelCollection>
                                        <dx:PanelContent runat="server">
                                            <dx:ASPxGridView ID="CSalDBGrid" runat="server" AutoGenerateColumns="False" ClientIDMode="AutoID" ClientInstanceName="CSalDBGrid" CssClass="grid" DataSourceID="sdsCusSal" EnableCallBacks="False" EnableTheming="True" KeyboardSupport="True" KeyFieldName="SHFTNO;SHFTITNO" Width="100%">
                                                <SettingsDetail AllowOnlyOneMasterRowExpanded="True" />
                                                <SettingsAdaptivity AdaptivityMode="HideDataCells" HideDataCellsAtWindowInnerWidth="750">
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
                                                    <NewButton ButtonType="Image" RenderMode="Image">
                                                        <Image Url="~/Images/Navigator/Grid_Add.png">
                                                        </Image>
                                                    </NewButton>
                                                    <UpdateButton ButtonType="Image" RenderMode="Image">
                                                        <Image Url="~/Images/Navigator/Grid_Post.png">
                                                        </Image>
                                                    </UpdateButton>
                                                    <CancelButton ButtonType="Image" RenderMode="Image">
                                                        <Image Url="~/Images/Navigator/Grid_Cancel.png">
                                                        </Image>
                                                    </CancelButton>
                                                    <EditButton ButtonType="Image" RenderMode="Image">
                                                        <Image Url="~/Images/Navigator/Grid_Edit.png">
                                                        </Image>
                                                    </EditButton>
                                                    <DeleteButton ButtonType="Image" RenderMode="Image">
                                                        <Image Url="~/Images/Navigator/Grid_Delete.png">
                                                        </Image>
                                                    </DeleteButton>
                                                </SettingsCommandButton>
                                                <SettingsDataSecurity AllowDelete="False" />
                                                <SettingsText CommandCancel="取消" CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" CommandUpdate="存入" SearchPanelEditorNullText="請輸入關鍵字" Title="策略目標" />
                                                <EditFormLayoutProperties ColCount="2" ColumnCount="2">
                                                    <Items>
                                                        <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="MANO">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="MADESC">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="SALQTY">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="SLUNITNM">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="SALAMT">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="USECARDID">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="USECARDAMT">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:GridViewColumnLayoutItem ColSpan="1" ColumnName="DUEAMT">
                                                        </dx:GridViewColumnLayoutItem>
                                                        <dx:EditModeCommandLayoutItem ColSpan="2" ColumnSpan="2" HorizontalAlign="Right">
                                                        </dx:EditModeCommandLayoutItem>
                                                    </Items>
                                                </EditFormLayoutProperties>
                                                <Columns>
                                                    <dx:GridViewCommandColumn ShowInCustomizationForm="True" VisibleIndex="0" MinWidth="20" MaxWidth="30" Width="3%" AdaptivePriority="2">
                                                    </dx:GridViewCommandColumn>
                                                    <dx:GridViewDataTextColumn FieldName="SALNO" ReadOnly="True" ShowInCustomizationForm="True" Visible="False" VisibleIndex="1">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="消費模式" FieldName="SALMDID" ShowInCustomizationForm="True" Visible="False" VisibleIndex="3">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataDateColumn Caption="銷售日期" FieldName="SALDT" ShowInCustomizationForm="True" VisibleIndex="4" MinWidth="90" MaxWidth="100" Width="8%">
                                                    </dx:GridViewDataDateColumn>
                                                    <dx:GridViewDataTextColumn Caption="貨品編號" FieldName="MANO" ShowInCustomizationForm="True" VisibleIndex="7" MaxWidth="100" MinWidth="90" Width="7%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="品名" FieldName="MADESC" ShowInCustomizationForm="True" VisibleIndex="8" MinWidth="100" MaxWidth="150" Width="15%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn FieldName="MASPEC" ShowInCustomizationForm="True" Visible="False" VisibleIndex="9">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="銷貨數量" FieldName="SALQTY" ShowInCustomizationForm="True" VisibleIndex="19" MinWidth="30" MaxWidth="60" Width="5%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="銷貨單位" FieldName="SLUNITNM" ShowInCustomizationForm="True" VisibleIndex="20" MinWidth="20" MaxWidth="30" Width="3%" AdaptivePriority="1">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="銷貨金額" FieldName="SALAMT" ShowInCustomizationForm="True" VisibleIndex="26" MinWidth="30" MaxWidth="80" Width="6%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="-預收金額" FieldName="USECARDAMT" ShowInCustomizationForm="True" VisibleIndex="37" MinWidth="30" MaxWidth="80" Width="6%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="=應收金額" FieldName="DUEAMT" ShowInCustomizationForm="True" VisibleIndex="38" MinWidth="30" MaxWidth="80" Width="6%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn Caption="經辦人員" FieldName="HPRSNNM" ShowInCustomizationForm="True" VisibleIndex="30" MinWidth="90" MaxWidth="110" Width="7%">
                                                    </dx:GridViewDataTextColumn>
                                                    <dx:GridViewDataTextColumn FieldName="DPTNO" ReadOnly="True" ShowInCustomizationForm="True" Visible="False" VisibleIndex="45">
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
                            </code>
                        </dx:ContentControl>
                    </ContentCollection>
                </dx:TabPage>
                <dx:TabPage Name="TabPageCCard" Text="消費卡劵">
                    <ContentCollection>
                        <dx:ContentControl runat="server">
                            <dx:ASPxGridView ID="CCardDBGrid" runat="server" AutoGenerateColumns="False" ClientIDMode="AutoID" ClientInstanceName="CSalDBGrid" CssClass="grid" DataSourceID="sdsCusCCard" EnableCallBacks="False" EnableTheming="True" KeyboardSupport="True" KeyFieldName="SHFTNO;SHFTITNO" Width="100%">
                                <SettingsDetail AllowOnlyOneMasterRowExpanded="True" />
                                <SettingsAdaptivity AdaptivityMode="HideDataCells" HideDataCellsAtWindowInnerWidth="750">
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
                                    <NewButton ButtonType="Image" RenderMode="Image">
                                        <Image Url="~/Images/Navigator/Grid_Add.png">
                                        </Image>
                                    </NewButton>
                                    <UpdateButton ButtonType="Image" RenderMode="Image">
                                        <Image Url="~/Images/Navigator/Grid_Post.png">
                                        </Image>
                                    </UpdateButton>
                                    <CancelButton ButtonType="Image" RenderMode="Image">
                                        <Image Url="~/Images/Navigator/Grid_Cancel.png">
                                        </Image>
                                    </CancelButton>
                                    <EditButton ButtonType="Image" RenderMode="Image">
                                        <Image Url="~/Images/Navigator/Grid_Edit.png">
                                        </Image>
                                    </EditButton>
                                    <DeleteButton ButtonType="Image" RenderMode="Image">
                                        <Image Url="~/Images/Navigator/Grid_Delete.png">
                                        </Image>
                                    </DeleteButton>
                                </SettingsCommandButton>
                                <SettingsDataSecurity AllowDelete="False" AllowEdit="False" AllowInsert="False" />
                                <SettingsText CommandCancel="取消" CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" CommandUpdate="存入" SearchPanelEditorNullText="請輸入關鍵字" Title="策略目標" />
                                <Columns>
                                    <dx:GridViewCommandColumn ShowInCustomizationForm="True" VisibleIndex="0" MinWidth="20" MaxWidth="30" Width="3%" AdaptivePriority="2">
                                    </dx:GridViewCommandColumn>
                                    <dx:GridViewDataTextColumn Caption="卡號" FieldName="CARDNO" ReadOnly="True" ShowInCustomizationForm="True" VisibleIndex="3" Visible="False">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataDateColumn Caption="發卡日期" FieldName="INITDT" ShowInCustomizationForm="True" VisibleIndex="11" MinWidth="90" MaxWidth="100" Width="8%" AdaptivePriority="1">
                                    </dx:GridViewDataDateColumn>
                                    <dx:GridViewDataDateColumn Caption="有效期限" FieldName="EXPRDT" ShowInCustomizationForm="True" VisibleIndex="12" MinWidth="90" MaxWidth="100" Width="8%" AdaptivePriority="1">
                                    </dx:GridViewDataDateColumn>
                                    <dx:GridViewDataTextColumn Caption="儲值金額" FieldName="DUEAMT" ShowInCustomizationForm="True" VisibleIndex="9" Visible="False">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="已用金額" FieldName="USEAMT" ShowInCustomizationForm="True" VisibleIndex="10" Visible="False">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="可用餘額" FieldName="VALDAMT" ShowInCustomizationForm="True" VisibleIndex="6" MinWidth="30" MaxWidth="80" Width="6%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="可用點數" FieldName="VSHOPAMT" ShowInCustomizationForm="True" VisibleIndex="7" MinWidth="30" MaxWidth="60" Width="5%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="開卡點數" FieldName="ISHOPAMT" ShowInCustomizationForm="True" VisibleIndex="8" Visible="False" MinWidth="30" MaxWidth="60" Width="5%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataComboBoxColumn Caption="卡片狀態" FieldName="CARDSTATNO" ShowInCustomizationForm="True" VisibleIndex="13" MinWidth="90" MaxWidth="100" Width="8%">
                                        <PropertiesComboBox DataSourceID="sdsCardStat" TextField="CARDSTATNM" ValueField="CARDSTATNO">
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataTextColumn Caption="卡片序號" FieldName="CSRLNO" ShowInCustomizationForm="True" VisibleIndex="2" AdaptivePriority="1" ReadOnly="True" MinWidth="90" MaxWidth="100" Width="8%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="卡別" FieldName="MADESC" ShowInCustomizationForm="True" VisibleIndex="1" MinWidth="100" MaxWidth="110" Width="9%" AdaptivePriority="1">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="可抵用金額" FieldName="DUEAMT" ShowInCustomizationForm="True" VisibleIndex="4" MinWidth="30" MaxWidth="80" Width="6%">
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn Caption="可抵用次數" FieldName="DUECNT" ShowInCustomizationForm="True" VisibleIndex="5" MinWidth="30" MaxWidth="60" Width="5%">
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
            </TabPages>
        </dx:ASPxPageControl>

        <dx:ASPxLoadingPanel ID="lplProcessing" runat="server" Modal="True" Text="處理中&amp;hellip;" ClientInstanceName="lplProcessing">
        </dx:ASPxLoadingPanel>
        <asp:SqlDataSource ID="sdsCus" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [Cus] WHERE [CUSNO] = @CUSNO" InsertCommand="INSERT INTO [Cus] ([CUSNO], [CUSCM], [CUSNM], [LOGONID], [CUSKDNO], [BIRDT], [IDNO], [SEXNO], [REGNO], [DPTNO], [PRSNNO], [BOSSNM], [BOSSTIT], [BOSSMOVTEL], [CRPADDR], [CRPZIPCD], [CRPTELNO], [CRPFAXNO], [CRPEMAILBOX], [CRPIPADDR], [CNTMAN], [CNTTIT], [CNTADDR], [CNTZIPCD], [CNTMOVTEL], [CNTTELNO], [CNTFAXNO], [CNTEMAILBOX], [DLYMAN], [DLYTIT], [DLYADDR], [DLYZIPCD], [DLYMOVTEL], [DLYTELNO], [DLYEMAILBOX], [INVCMAN], [INVCTIT], [INVCADDR], [INVCZIPCD], [INVCMOVTEL], [INVCTELNO], [INVCEMAILBOX], [AMTACNTNO], [CHKACNTNO], [BANKNO], [SAVENO], [TAXRATE], [TAXMDNO], [PAYTERMNO], [INVCMD], [ENABLED], [REMARK], [PASSNO], [PASSMDNO], [PASSVALDDAY], [LPASSCHGDT], [ISLOGIN], [LLOGINDT], [DUEAMT], [PAYAMT], [MAXCREDIT], [ORDCREDIT], [USECREDIT], [ISHOPAMT], [CSHOPAMT], [HAIRTXTRID], [HAIRAMNTID], [HAIRWCLRID], [HAIRCOLRID], [HAIRSTYLID], [HAIRSTYLDESC], [HAIRNOTICE], [LOGID], [LOGDT]) VALUES (@CUSNO, @CUSCM, @CUSNM, @LOGONID, @CUSKDNO, @BIRDT, @IDNO, @SEXNO, @REGNO, @DPTNO, @PRSNNO, @BOSSNM, @BOSSTIT, @BOSSMOVTEL, @CRPADDR, @CRPZIPCD, @CRPTELNO, @CRPFAXNO, @CRPEMAILBOX, @CRPIPADDR, @CNTMAN, @CNTTIT, @CNTADDR, @CNTZIPCD, @CNTMOVTEL, @CNTTELNO, @CNTFAXNO, @CNTEMAILBOX, @DLYMAN, @DLYTIT, @DLYADDR, @DLYZIPCD, @DLYMOVTEL, @DLYTELNO, @DLYEMAILBOX, @INVCMAN, @INVCTIT, @INVCADDR, @INVCZIPCD, @INVCMOVTEL, @INVCTELNO, @INVCEMAILBOX, @AMTACNTNO, @CHKACNTNO, @BANKNO, @SAVENO, @TAXRATE, @TAXMDNO, @PAYTERMNO, @INVCMD, @ENABLED, @REMARK, @PASSNO, @PASSMDNO, @PASSVALDDAY, @LPASSCHGDT, @ISLOGIN, @LLOGINDT, @DUEAMT, @PAYAMT, @MAXCREDIT, @ORDCREDIT, @USECREDIT, @ISHOPAMT, @CSHOPAMT, @HAIRTXTRID, @HAIRAMNTID, @HAIRWCLRID, @HAIRCOLRID, @HAIRSTYLID, @HAIRSTYLDESC, @HAIRNOTICE, @LOGID, @LOGDT)" SelectCommand="SELECT Cus.CUSNO, Cus.CUSCM, Cus.CUSNM, Cus.LOGONID, Cus.CUSKDNO, Cuskd.CUSKDNM,
 Cus.BIRDT, Cus.IDNO, Cus.SEXNO, Cus.REGNO, Cus.DPTNO, Cus.PRSNNO, Cus.BOSSNM,
 Cus.BOSSTIT, Cus.BOSSMOVTEL, Cus.CRPADDR, Cus.CRPZIPCD, Cus.CRPTELNO, Cus.CRPFAXNO,
 Cus.CRPEMAILBOX, Cus.CRPIPADDR, Cus.CNTMAN, Cus.CNTTIT, Cus.CNTADDR, Cus.CNTZIPCD,
 Cus.CNTMOVTEL, Cus.CNTTELNO, Cus.CNTFAXNO, Cus.CNTEMAILBOX, Cus.DLYMAN,
 Cus.DLYTIT, Cus.DLYADDR, Cus.DLYZIPCD, Cus.DLYMOVTEL, Cus.DLYTELNO,
 Cus.DLYEMAILBOX, Cus.INVCMAN, Cus.INVCTIT, Cus.INVCADDR, Cus.INVCZIPCD,
 Cus.INVCMOVTEL, Cus.INVCTELNO, Cus.INVCEMAILBOX, Cus.AMTACNTNO, Cus.CHKACNTNO,
 Cus.BANKNO, Cus.SAVENO, Cus.TAXRATE, Cus.TAXMDNO, Cus.PAYTERMNO, Cus.INVCMD,
 Cus.ENABLED, Cus.REMARK, Cus.DUEAMT, Cus.PAYAMT,
 Cus.MAXCREDIT, Cus.ORDCREDIT, Cus.USECREDIT, Cus.ISHOPAMT, Cus.CSHOPAMT, Cus.ISHOPAMT+Cus.CSHOPAMT TSHOPAMT,
 Cus.HAIRTXTRID, Cus.HAIRAMNTID, Cus.HAIRWCLRID, Cus.HAIRCOLRID, Cus.HAIRSTYLID, Cus.HAIRSTYLDESC,
 Cus.PASSNO, Cus.ISLOGIN, Cus.LLOGINDT, Cus.PASSMDNO, Cus.PASSVALDDAY, Cus.LPASSCHGDT,
 Cus.LOGID, Cus.LOGDT
FROM Cus LEFT JOIN Cuskd ON (Cus.CUSKDNO=Cuskd.CUSKDNO)
WHERE Cus.CUSNO=@CUSNO
ORDER BY CUSNO"
            UpdateCommand="UPDATE [Cus] SET [CUSCM] = @CUSCM, [CUSNM] = @CUSNM, [LOGONID] = @LOGONID, [CUSKDNO] = @CUSKDNO, [BIRDT] = @BIRDT, [IDNO] = @IDNO, [SEXNO] = @SEXNO, [REGNO] = @REGNO, [DPTNO] = @DPTNO, [PRSNNO] = @PRSNNO, [BOSSNM] = @BOSSNM, [BOSSTIT] = @BOSSTIT, [BOSSMOVTEL] = @BOSSMOVTEL, [CRPADDR] = @CRPADDR, [CRPZIPCD] = @CRPZIPCD, [CRPTELNO] = @CRPTELNO, [CRPFAXNO] = @CRPFAXNO, [CRPEMAILBOX] = @CRPEMAILBOX, [CRPIPADDR] = @CRPIPADDR, [CNTMAN] = @CNTMAN, [CNTTIT] = @CNTTIT, [CNTADDR] = @CNTADDR, [CNTZIPCD] = @CNTZIPCD, [CNTMOVTEL] = @CNTMOVTEL, [CNTTELNO] = @CNTTELNO, [CNTFAXNO] = @CNTFAXNO, [CNTEMAILBOX] = @CNTEMAILBOX, [DLYMAN] = @DLYMAN, [DLYTIT] = @DLYTIT, [DLYADDR] = @DLYADDR, [DLYZIPCD] = @DLYZIPCD, [DLYMOVTEL] = @DLYMOVTEL, [DLYTELNO] = @DLYTELNO, [DLYEMAILBOX] = @DLYEMAILBOX, [INVCMAN] = @INVCMAN, [INVCTIT] = @INVCTIT, [INVCADDR] = @INVCADDR, [INVCZIPCD] = @INVCZIPCD, [INVCMOVTEL] = @INVCMOVTEL, [INVCTELNO] = @INVCTELNO, [INVCEMAILBOX] = @INVCEMAILBOX, [AMTACNTNO] = @AMTACNTNO, [CHKACNTNO] = @CHKACNTNO, [BANKNO] = @BANKNO, [SAVENO] = @SAVENO, [TAXRATE] = @TAXRATE, [TAXMDNO] = @TAXMDNO, [PAYTERMNO] = @PAYTERMNO, [INVCMD] = @INVCMD, [ENABLED] = @ENABLED, [REMARK] = @REMARK, [PASSNO] = @PASSNO, [PASSMDNO] = @PASSMDNO, [PASSVALDDAY] = @PASSVALDDAY, [LPASSCHGDT] = @LPASSCHGDT, [ISLOGIN] = @ISLOGIN, [LLOGINDT] = @LLOGINDT, [DUEAMT] = @DUEAMT, [PAYAMT] = @PAYAMT, [MAXCREDIT] = @MAXCREDIT, [ORDCREDIT] = @ORDCREDIT, [USECREDIT] = @USECREDIT, [ISHOPAMT] = @ISHOPAMT, [CSHOPAMT] = @CSHOPAMT, [HAIRTXTRID] = @HAIRTXTRID, [HAIRAMNTID] = @HAIRAMNTID, [HAIRWCLRID] = @HAIRWCLRID, [HAIRCOLRID] = @HAIRCOLRID, [HAIRSTYLID] = @HAIRSTYLID, [HAIRSTYLDESC] = @HAIRSTYLDESC, [HAIRNOTICE] = @HAIRNOTICE, [LOGID] = @LOGID, [LOGDT] = @LOGDT WHERE [CUSNO] = @CUSNO">
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
            <SelectParameters>
                <asp:Parameter Name="CUSNO" />
            </SelectParameters>
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

        <asp:SqlDataSource ID="sdsCusSal" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Sal.CUSNO, Sal.SALNO, Sal.SALDT, Sal.DPTNO, Dpt.DPTNM, 
 Sald.MANO, Sald.SALQTY, Sald.SLUNITNM, Sald.SALAMT, Ma.MADESC, Ma.MASPEC, 
 Sald.USECARDNO, Sald.USECARDAMT, Sald.DUEAMT, Prsn.PRSNNM HPRSNNM
FROM Sal INNER JOIN Sald ON (Sal.SALNO=Sald.SALNO)
 LEFT JOIN Dpt ON (Sal.DPTNO=Dpt.DPTNO)
 LEFT JOIN Prsn ON (Sald.HPRSNNO=Prsn.PRSNNO)
 LEFT JOIN Ma ON (Sald.SALMDID=Ma.MACDNO AND Sald.MANO=Ma.MANO)
WHERE Sal.CUSNO=@CUSNO
ORDER BY Sal.SALDT DESC">
            <SelectParameters>
                <asp:SessionParameter Name="CUSNO" SessionField="CUSNO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsCusCCard" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Ma.MAKDNO, CCardAcnt.CARDID, Ma.MADESC, CCardAcnt.CARDNO, CCardAcnt.CSRLNO, CCardAcnt.CUSNO, Cus.CUSNM,
 CCardAcnt.DUEAMT, CCardAcnt.DUECNT, CCardAcnt.INITDT, CCardAcnt.EXPRDT, CCardAcnt.ISHOPAMT, CCardAcnt.CSHOPAMT,
 CCardAcnt.DUEAMT-CCardAcnt.USEAMT VALDAMT, CCardAcnt.ISHOPAMT+CCardAcnt.CSHOPAMT-CCardAcnt.USHOPAMT VSHOPAMT, CCardAcnt.CARDSTATNO
FROM CCardAcnt LEFT JOIN Cus ON (CCardAcnt.CUSNO=Cus.CUSNO)
 LEFT JOIN Ma ON (CCardAcnt.CARDID=Ma.MANO)
WHERE CCardAcnt.CUSNO=@CUSNO">
            <SelectParameters>
                <asp:SessionParameter Name="CUSNO" SessionField="CUSNO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMa" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Ma.MANO, Ma.MADESC, Ma.MASPEC, Ma.MACOLOR, Ma.MAUSAGE, Ma.ATTRIB,
 Ma.PKUNITNM, Ma.RTUNITNM, Ma.PK2RTEXGRATE, Ma.INVQTY, Ma.ISINV, Ma.UNITCOST,
 Ma.UNITPRIC, Ma.LOCANO, Ma.BARCODE, Ma.MACDNO, Ma.MAKDNO
FROM Ma"></asp:SqlDataSource>

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
        <asp:SqlDataSource ID="sdsDpts" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Dpt.DPTNO, Dpt.DPTNM, Dpt.DPTHANDLER, Dpt.DPTADDR, Dpt.DPTZIPCD, Dpt.DPTTELNO, Dpt.DPTFAXNO, Dpt.DPTEMAILBOX,Dpt.WEBSITE ,Dpt.OPRTDESC ,Dpt.OPRTPERD, Dpt.PHOTOFILENM FROM Dpt WHERE Dpt.DISSOLVDT IS NULL"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsPrsns" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [PRSNNO], [PRSNNM] FROM [Prsn]
WHERE Prsn.DPTNO=@DPTNO AND Prsn.QUITDT IS NULL">
            <SelectParameters>
                <asp:SessionParameter Name="DPTNO" SessionField="DPTNO" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsCardStat" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [CARDSTATNO], [CARDSTATNM] FROM [CCardStat]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsUnit" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [UNITNO], [UNITNM] FROM [Unit]"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SdsTitles" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Titles.TITLEID, Titles.TITLENM
FROM Titles
WHERE Titles.ENABLED=1
ORDER BY Titles.TITLEID"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsZipDist" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT CITYNO, ZIPCD, DISTNM 
FROM Dist
ORDER BY ZIPCD"></asp:SqlDataSource>

    </form>
</body>
</html>
