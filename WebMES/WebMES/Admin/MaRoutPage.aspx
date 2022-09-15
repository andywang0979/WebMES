<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaRoutPage.aspx.cs" Inherits="WebMES.Admin.MaRoutPage" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>

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
        function OnBDPTNOChanged(cbxBDPTNO) {
            //cbxBPRSNNO.PerformCallback(cbxBDPTNO.GetSelectedItem().value.toString());
        }

        function OnedMAKDNOChanged(ss) {
            cbkPanelMANO.PerformCallback(ss);
        }

        function OnGoodsClick(CurMACDNO) {
            cvwMa.PerformCallback(CurMACDNO);
        }

        var postponedCallbackRequired = false;
        function tbxBMANOValidation(s, e) {
            //if (!postponedCallbackRequired) {
            //    CSaldDBGrid.PerformCallback(tbxBMANO.GetValue());
            //    postponedCallbackRequired = true;
            //}
            CSaldDBGrid.PerformCallback(tbxBMANO.GetValue());
        }
        function OnEndCallback(s, e) {
            if (postponedCallbackRequired) {
                CSaldDBGrid.PerformCallback(tbxBMANO.GetValue());
            }
            postponedCallbackRequired = false;
        }

        function EnterToTab(s, e) {
            if (event.keyCode == 13)
                event.keyCode = 9;
        }

        function clnPFNHQTYSet() {
            var valPNPARA1 = RoutShftDBGrid.GetEditor('PNPARA1').GetValue();
            var valPNPARA2 = RoutShftDBGrid.GetEditor('PNPARA2').GetValue();
            var valPHPARA1 = RoutShftDBGrid.GetEditor('PHPARA1').GetValue();
            var valPHPARA2 = RoutShftDBGrid.GetEditor('PHPARA2').GetValue();
            var valPNMATHOPTR = RoutShftDBGrid.GetEditor("PNMATHOPTR").GetText();
            var valPHMATHOPTR = RoutShftDBGrid.GetEditor("PHMATHOPTR").GetText();
            var valMATHOPTR = RoutShftDBGrid.GetEditor("MATHOPTR").GetText();
            if (valPNPARA1 != "" && valPNPARA2 != "" && valPHPARA1 != "" && valPHPARA2 != "") {
                RoutShftDBGrid.GetEditor('PCONFIRM').SetChecked(true);
                //RoutShftDBGrid.GetEditor('PCONFIRM').SetValue(eval("true");
                RoutShftDBGrid.GetEditor('PUTNDT').SetDate(new Date());
                RoutShftDBGrid.GetEditor('SHFTQTY').SetValue(valPUTNQTY);
                //var valSHFTQTY = RoutShftDBGrid.GetEditor('SHFTQTY').GetValue();
                //A    PNMATHOPTR   C
                var strPFNHOpnt1 = (valPNPARA1 + valPNMATHOPTR + valPHPARA1);
                //B    PNMATHOPTR   D
                var strPFNHOpnt2 = (valPNPARA2 + valPHMATHOPTR + valPHPARA2);
                //小數去尾
                strPFNHOpnt1 = Math.floor(strPFNHOpnt1);
                strPFNHOpnt2 = Math.floor(strPFNHOpnt2);
                var strPFNHQTY = (strPFNHOpnt1 + valMATHOPTR + strPFNHOpnt2);
                var edPFNHQTY = RoutShftDBGrid.GetEditor("PFNHQTY");
                edPFNHQTY.SetText(eval(strPFNHQTY));
            }
        }

    </script>

</head>
<body>
    <form id="BD000" runat="server">
        <uc1:MainMenuPanel runat="server" ID="MainMenuPanel" />
        <dx:ASPxCallbackPanel ID="cbkfloMaRout" runat="server" ClientInstanceName="cbkfloMaRout" OnCallback="cbkfloMaRout_Callback" Width="100%">
            <ClientSideEvents EndCallback="function(s, e) {
if (s.cpPopPurd) {
  delete s.cpPopPurd;
  PurdDBGrid.Refresh();
  pclSelectPurdPanel.Show();
 }	
}" />
            <PanelCollection>
                <dx:PanelContent runat="server">

                    <dx:ASPxFormLayout ID="floMaRout" runat="server" ColCount="3" Width="100%" DataSourceID="sdsMaRout" EnableTheming="True" ClientInstanceName="floMaRout" ColumnCount="3">
                        <SettingsAdaptivity SwitchToSingleColumnAtWindowInnerWidth="580" AdaptivityMode="SingleColumnWindowLimit" />
                        <Styles>
                            <LayoutGroup Cell-Paddings-Padding="0px">
                                <Cell>
                                    <Paddings Padding="0px"></Paddings>
                                </Cell>
                            </LayoutGroup>
                        </Styles>
                        <Paddings Padding="0px" />
                        <Items>
                            <dx:LayoutItem Caption="製途品號" FieldName="MANO">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxComboBox ID="edMANO" runat="server" TextField="MADESC" ValueField="MANO" FilterMinLength="2" OnItemsRequestedByFilterCondition="edMANO_ItemsRequestedByFilterCondition" EnableCallbackMode="True" OnItemRequestedByValue="edMANO_ItemRequestedByValue" CallbackPageSize="30" NullText="輸入兩個字元(以上)啟動查詢">
                                            <Columns>
                                                <dx:ListBoxColumn Caption="代碼" FieldName="MANO" />
                                                <dx:ListBoxColumn Caption="品名" FieldName="MADESC" />
                                                <dx:ListBoxColumn Caption="規格" FieldName="MASPEC" />
                                            </Columns>
                                            <ClientSideEvents ValueChanged="function(s, e) {
                                                    cbkfloMaRout.PerformCallback(s.GetValue());	
}" />
                                            <ClearButton DisplayMode="Always">
                                            </ClearButton>
                                        </dx:ASPxComboBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem FieldName="MADESC" Caption="品名">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edMADESC" runat="server">
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem FieldName="MASPEC" Caption="規格">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edMASPEC" runat="server">
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="製途名稱" FieldName="MROUTNM">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edMROUTNM" runat="server">
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="製途說明" FieldName="MROUTDESC">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edMROUTDESC" runat="server">
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="製途代碼" FieldName="MROUTID">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edMROUTID" runat="server">
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem FieldName="TSPREPHMANTICK" Caption="準備人時">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edTSPREPHMANTICK" runat="server" OnDataBound="edTSPREPHMANTICK_DataBound" ClientEnabled="False">
                                            <MaskSettings Mask="99.99:99:99" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem FieldName="TSPREPMACHTICK" Caption="準備機時">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edTSPREPMACHTICK" runat="server" ClientEnabled="False" OnDataBound="edTSPREPMACHTICK_DataBound">
                                            <MaskSettings Mask="99.99:99:99" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="生產批量" FieldName="BTCHPDQTY">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edBTCHPDQTY" runat="server">
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem FieldName="TSBTCHHMANTICK" Caption="加工人時/批">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edTSBTCHHMANTICK" runat="server" OnDataBound="edTSBTCHHMANTICK_DataBound" ClientEnabled="False">
                                            <MaskSettings Mask="99.99:99:99" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem FieldName="TSBTCHMACHTICK" Caption="加工機時/批">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxTextBox ID="edTSBTCHMACHTICK" runat="server" OnDataBound="edTSBTCHMACHTICK_DataBound" ClientEnabled="False">
                                            <MaskSettings Mask="99.99:99:99" />
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>

                            <dx:LayoutItem Caption="預設製途" FieldName="ISWODEFAULT">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer runat="server">
                                        <dx:ASPxCheckBox ID="edISWODEFAULT" runat="server">
                                        </dx:ASPxCheckBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>

                            <dx:LayoutGroup Caption="" ColSpan="3" Width="100%">
                                <Items>
                                    <dx:LayoutItem Caption="">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxPanel ID="pnlMaShftd" runat="server" ClientInstanceName="pnlMaShftd" CssClass="topPanel">
                                                    <PanelCollection>
                                                        <dx:PanelContent runat="server">
                                                            <dx:ASPxCallbackPanel ID="cbkPanelMANO" runat="server" ClientInstanceName="cbkPanelMANO" Width="100%" OnCallback="cbkPanelMANO_Callback">
                                                                <ClientSideEvents EndCallback="function(s, e) {
if (s.cpISedMANO) {
  SubDBGrid.GetEditor('MADESC').SetValue(s.cpMADESC);
  SubDBGrid.GetEditor('MASPEC').SetValue(s.cpMASPEC);
  SubDBGrid.GetEditor('ISINV').SetValue(s.cpISINV);
  SubDBGrid.GetEditor('RCUTPRIC').SetValue(s.cpRCUTPRIC);
  SubDBGrid.GetEditor('RCUNITNM').SetValue(s.cpRCUNITNM);
  SubDBGrid.GetEditor('RC2RTEXGRATE').SetValue(s.cpRC2RTEXGRATE);
  delete s.cpISedMANO;
  delete s.cpMADESC;
  delete s.cpMASPEC;
  delete s.cpISINV;
  delete s.cpRCUTPRIC;
  delete s.cpRCUNITNM;
  delete s.cpRC2RTEXGRATE;
}
else if (s.cpISedPURITNO) {
  SubDBGrid.GetEditor('MANO').SetValue(s.cpMANO);
  SubDBGrid.GetEditor('MADESC').SetValue(s.cpMADESC);
  SubDBGrid.GetEditor('MASPEC').SetValue(s.cpMASPEC);
  SubDBGrid.GetEditor('ISINV').SetValue(s.cpISINV);
  SubDBGrid.GetEditor('RCUTPRIC').SetValue(s.cpRCUTPRIC);
  SubDBGrid.GetEditor('RCUNITNM').SetValue(s.cpRCUNITNM);
  SubDBGrid.GetEditor('RC2RTEXGRATE').SetValue(s.cpRC2RTEXGRATE);
  //SubDBGrid.GetEditor('PURDPURQTY').SetValue(s.cpPURDPURQTY);
  //SubDBGrid.GetEditor('PURDPASQTY').SetValue(s.cpPURDPASQTY);
  SubDBGrid.GetEditor('PASQTY').SetValue(s.cpPASQTY);
  SubDBGrid.GetEditor('PASAMT').SetValue(s.cpPASAMT);
  delete s.cpISedPURITNO;
  delete s.cpMANO;
  delete s.cpMADESC;
  delete s.cpMASPEC;
  delete s.cpISINV;
  delete s.cpRCUTPRIC;
  delete s.cpRCUNITNM;
  delete s.cpRC2RTEXGRATE;
  //delete s.cpPURDPURQTY;
  //delete s.cpPURDPASQTY;
  delete s.cpPASQTY;
  delete s.cpPASAMT;
}

}" />
                                                                <PanelCollection>
                                                                    <dx:PanelContent runat="server">
                                                                        <dx:ASPxGridView ID="SubDBGrid" runat="server" ClientInstanceName="SubDBGrid" AutoGenerateColumns="False" DataSourceID="sdsMaRoutProc" EnableTheming="True" KeyFieldName="MANO;MROUTID;MROUTSRNO;MPROCID" CssClass="grid" Width="100%" KeyboardSupport="True" EnableCallBacks="False" ClientIDMode="AutoID" OnInitNewRow="SubDBGrid_InitNewRow" OnCellEditorInitialize="SubDBGrid_CellEditorInitialize" OnRowValidating="SubDBGrid_RowValidating" OnRowInserting="SubDBGrid_RowInserting" EnableRowsCache="False" EnableViewState="False">
                                                                            <SettingsDetail AllowOnlyOneMasterRowExpanded="True" />
                                                                            <SettingsAdaptivity AdaptivityMode="HideDataCells">
                                                                                <AdaptiveDetailLayoutProperties ColCount="1">
                                                                                </AdaptiveDetailLayoutProperties>
                                                                            </SettingsAdaptivity>
                                                                            <Templates>
                                                                                <EditForm>
                                                                                    <dx:ASPxCallbackPanel ID="cbpfltMaRoutProc" runat="server" Width="100%" ClientInstanceName="cbpfltMaRoutProc" OnCallback="cbpfltMaRoutProc_Callback">
                                                                                        <PanelCollection>
                                                                                            <dx:PanelContent ID="PanelContent6" runat="server">
                                                                                                <dx:ASPxFormLayout ID="fltMaRoutProc" runat="server" AlignItemCaptionsInAllGroups="True" ColCount="3" EnableTheming="True" ShowItemCaptionColon="False" ColumnCount="3" OnPreRender="fltMaRoutProc_PreRender">
                                                                                                    <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="580" />
                                                                                                    <Items>
                                                                                                        <dx:LayoutItem Caption="製程" FieldName="MPROCID">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edMPROCID" runat="server" ClientInstanceName="edMPROCID" DataSourceID="sdsManuProc" TextField="MPROCNM" ValueField="MPROCID" Text='<%#Eval("MPROCID") %>'>
                                                                                                                    </dx:ASPxComboBox>

                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="MPROCNM" Caption="製程名稱" Visible="False">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="ASPxTextBox3" runat="server" Text='<%#Eval("MPROCNM") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="MROUTSRNO" Caption="加工順序">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edMROUTSRNO" runat="server" Text='<%#Eval("MROUTSRNO") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem Caption="投入公式" FieldName="PROCFMULAOPT">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edPROCFMULAOPT" runat="server" ClientInstanceName="edPROCFMULAOPT" Value='<%#Eval("PROCFMULAOPT") %>' ValueType="System.Int32">
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
  cbpfltMaRoutProc.PerformCallback('PROCFMULAOPT');
}" />

                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="PNPARA1" Caption="投入規格1">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edPNPARA1" runat="server" Text='<%#Eval("PNPARA1") %>'>
                                                                                                                        <ClientSideEvents ValueChanged="function(s, e) {
  cbpfltMaRoutProc.PerformCallback('PNPARA1');
}" />
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem Caption="*/" FieldName="PNMATHOPTR">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edPNMATHOPTR" runat="server" ClientInstanceName="edPNMATHOPTR" Text='<%#Eval("PNMATHOPTR") %>'>
                                                                                                                        <Items>
                                                                                                                            <dx:ListEditItem Text="*" Value="*" />
                                                                                                                            <dx:ListEditItem Text="/" Value="/" />
                                                                                                                        </Items>
                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="PNPARA2" Caption="投入規格2">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edPNPARA2" runat="server" Text='<%#Eval("PNPARA2") %>'>
                                                                                                                        <ClientSideEvents ValueChanged="function(s, e) {
  cbpfltMaRoutProc.PerformCallback('PNPARA2');
}" />
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem Caption="投入數量" FieldName="PUTNQTY">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edPUTNQTY" runat="server" Text='<%#Eval("PUTNQTY") %>'>
                                                                                                                        <ClientSideEvents ValueChanged="function(s, e) {
  cbpfltMaRoutProc.PerformCallback('PUTNQTY');
}" />
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem Caption="投入單位" FieldName="PNUNITNM">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edPNUNITNM" runat="server" ClientInstanceName="edPNUNITNM" DataSourceID="sdsUnit" TextField="UNITNM" ValueField="UNITNM" DropDownStyle="DropDown" Text='<%#Eval("PNUNITNM") %>'>
                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem Caption="*/" FieldName="MATHOPTR">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edMATHOPTR" runat="server" ClientInstanceName="edMATHOPTR" Text='<%#Eval("MATHOPTR") %>'>
                                                                                                                        <Items>
                                                                                                                            <dx:ListEditItem Text="*" Value="*" />
                                                                                                                            <dx:ListEditItem Text="/" Value="/" />
                                                                                                                        </Items>
                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="PN2RTEXGRATE" Caption="換算比率">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edPN2RTEXGRATE" runat="server" Text='<%#Eval("PN2RTEXGRATE") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="PHPARA1" Caption="產出規格1">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edPHPARA1" runat="server" Text='<%#Eval("PHPARA1") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem Caption="*/" FieldName="PHMATHOPTR">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edPHMATHOPTR" runat="server" ClientInstanceName="edPHMATHOPTR" Text='<%#Eval("PHMATHOPTR") %>'>
                                                                                                                        <Items>
                                                                                                                            <dx:ListEditItem Text="*" Value="*" />
                                                                                                                            <dx:ListEditItem Text="/" Value="/" />
                                                                                                                        </Items>
                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="PHPARA2" Caption="產出規格2">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edPHPARA2" runat="server" Text='<%#Eval("PHPARA2") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="PFNHQTY" Caption="預計數量">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edPFNHQTY" runat="server" Text='<%#Eval("PFNHQTY") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="FNUNITNM" Caption="產出單位">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edFNUNITNM" runat="server" ClientInstanceName="edFNUNITNM" DataSourceID="sdsUnit" TextField="UNITNM" ValueField="UNITNM" DropDownStyle="DropDown" Text='<%#Eval("FNUNITNM") %>'>
                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="FNSHSPEC" Caption="完成規格">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edFNSHSPEC" runat="server" Text='<%#Eval("FNSHSPEC") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="FNSHVALDMD" Caption="產出數量檢查模式">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edFNSHVALDMD" runat="server" ClientInstanceName="edFNSHVALDMD" Value='<%#Eval("FNSHVALDMD") %>' ValueType="System.Int32">
                                                                                                                        <Items>
                                                                                                                            <dx:ListEditItem Text=">=預設產出數量" Value="0" />
                                                                                                                            <dx:ListEditItem Text=">=投入數量" Value="1" />
                                                                                                                        </Items>
                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>

                                                                                                        <dx:LayoutItem FieldName="FNSHTLRNRATE" Caption="產出容許誤差率">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edFNSHTLRNRATE" runat="server" Text='<%#Eval("FNSHTLRNRATE") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>

                                                                                                        <dx:LayoutItem FieldName="UNITCOST" Caption="製程成本">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxTextBox ID="edUNITCOST" runat="server" Text='<%#Eval("UNITCOST") %>'>
                                                                                                                    </dx:ASPxTextBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>
                                                                                                        <dx:LayoutItem FieldName="MPROCMD" Caption="製程模式">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edMPROCMD" runat="server" ClientInstanceName="edMPROCMD" Text='<%#Eval("MPROCMD") %>'>
                                                                                                                        <Items>
                                                                                                                            <dx:ListEditItem Text="自製" Value="1" />
                                                                                                                            <dx:ListEditItem Text="外包" Value="2" />
                                                                                                                        </Items>
                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>

                                                                                                        <dx:LayoutItem FieldName="MLINEID" Caption="線別">
                                                                                                            <LayoutItemNestedControlCollection>
                                                                                                                <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                    <dx:ASPxComboBox ID="edMLINEID" runat="server" ClientInstanceName="edMLINEID" DataSourceID="sdsManuLine" TextField="MLINENM" ValueField="MLINEID" Text='<%#Eval("MLINEID") %>'>
                                                                                                                    </dx:ASPxComboBox>
                                                                                                                </dx:LayoutItemNestedControlContainer>
                                                                                                            </LayoutItemNestedControlCollection>
                                                                                                        </dx:LayoutItem>

                                                                                                        <dx:LayoutGroup Caption="" ColCount="4" ColSpan="3" ColumnCount="4" ColumnSpan="3">
                                                                                                            <Items>
                                                                                                                <dx:LayoutItem FieldName="SPREPHMANTICK" Caption="準備人時" ColSpan="1">
                                                                                                                    <LayoutItemNestedControlCollection>
                                                                                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                            <dx:ASPxTextBox ID="edSPREPHMANTICK" runat="server" Text='<%#Eval("SPREPHMANTICK") %>' OnDataBound="edSPREPHMANTICK_DataBound">
                                                                                                                                <MaskSettings Mask="99.99:99:99" />
                                                                                                                                <ValidationSettings Display="Dynamic">
                                                                                                                                </ValidationSettings>
                                                                                                                            </dx:ASPxTextBox>
                                                                                                                        </dx:LayoutItemNestedControlContainer>
                                                                                                                    </LayoutItemNestedControlCollection>
                                                                                                                </dx:LayoutItem>

                                                                                                                <dx:LayoutItem FieldName="SPREPMACHTICK" Caption="準備機時" ColSpan="1">
                                                                                                                    <LayoutItemNestedControlCollection>
                                                                                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                            <dx:ASPxTextBox ID="edSPREPMACHTICK" runat="server" Text='<%#Eval("SPREPMACHTICK") %>' OnDataBound="edSPREPMACHTICK_DataBound">
                                                                                                                                <MaskSettings Mask="99.99:99:99" />
                                                                                                                                <ValidationSettings Display="Dynamic">
                                                                                                                                </ValidationSettings>
                                                                                                                            </dx:ASPxTextBox>
                                                                                                                        </dx:LayoutItemNestedControlContainer>
                                                                                                                    </LayoutItemNestedControlCollection>
                                                                                                                </dx:LayoutItem>
                                                                                                                <dx:LayoutItem FieldName="SBTCHHMANTICK" Caption="加工人時" ColSpan="1">
                                                                                                                    <LayoutItemNestedControlCollection>
                                                                                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                            <dx:ASPxTextBox ID="edSBTCHHMANTICK" runat="server" Text='<%#Eval("SBTCHHMANTICK") %>' OnDataBound="edSBTCHHMANTICK_DataBound">
                                                                                                                                <MaskSettings Mask="99.99:99:99" />
                                                                                                                                <ValidationSettings Display="Dynamic">
                                                                                                                                </ValidationSettings>
                                                                                                                            </dx:ASPxTextBox>
                                                                                                                        </dx:LayoutItemNestedControlContainer>
                                                                                                                    </LayoutItemNestedControlCollection>
                                                                                                                </dx:LayoutItem>

                                                                                                                <dx:LayoutItem FieldName="SBTCHMACHTICK" Caption="加工機時" ColSpan="1">
                                                                                                                    <LayoutItemNestedControlCollection>
                                                                                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                            <dx:ASPxTextBox ID="edSBTCHMACHTICK" runat="server" Text='<%#Eval("SBTCHMACHTICK") %>' OnDataBound="edSBTCHMACHTICK_DataBound">
                                                                                                                                <MaskSettings Mask="99.99:99:99" />
                                                                                                                                <ValidationSettings Display="Dynamic">
                                                                                                                                </ValidationSettings>
                                                                                                                            </dx:ASPxTextBox>
                                                                                                                        </dx:LayoutItemNestedControlContainer>
                                                                                                                    </LayoutItemNestedControlCollection>
                                                                                                                </dx:LayoutItem>

                                                                                                            </Items>
                                                                                                        </dx:LayoutGroup>

                                                                                                        <dx:LayoutGroup ColCount="3" ShowCaption="False" Width="100%">
                                                                                                            <GroupBoxStyle>
                                                                                                                <border borderstyle="Dotted" />
                                                                                                            </GroupBoxStyle>
                                                                                                            <Items>
                                                                                                                <dx:LayoutItem Caption="" ColSpan="2" ColumnSpan="2" Width="50%">
                                                                                                                    <LayoutItemNestedControlCollection>
                                                                                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                                                                                            <dx:ASPxLabel ID="lblItemErrorMessage" runat="server" ForeColor="#CC6600">
                                                                                                                            </dx:ASPxLabel>
                                                                                                                        </dx:LayoutItemNestedControlContainer>
                                                                                                                    </LayoutItemNestedControlCollection>
                                                                                                                </dx:LayoutItem>
                                                                                                                <dx:LayoutItem Caption="" HorizontalAlign="Right">
                                                                                                                    <LayoutItemNestedControlCollection>
                                                                                                                        <dx:LayoutItemNestedControlContainer runat="server">

                                                                                                                            <dx:ASPxButton ID="btnItemSave" runat="server" AutoPostBack="False" OnClick="btnItemSave_Click" Text="儲存" UseSubmitBehavior="False" ValidationGroup="grpMaShftd" CausesValidation="false" Width="50%">
                                                                                                                                <Image IconID="iconbuilder_actions_check_svg_16x16">
                                                                                                                                </Image>
                                                                                                                            </dx:ASPxButton>

                                                                                                                            <dx:ASPxButton ID="btnItemCancel" runat="server" AutoPostBack="False" OnClick="btnItemCancel_Click" Text="取消" UseSubmitBehavior="False" Width="50%">
                                                                                                                                <Image IconID="iconbuilder_actions_delete_svg_16x16">
                                                                                                                                </Image>
                                                                                                                            </dx:ASPxButton>

                                                                                                                        </dx:LayoutItemNestedControlContainer>
                                                                                                                    </LayoutItemNestedControlCollection>
                                                                                                                </dx:LayoutItem>
                                                                                                            </Items>
                                                                                                        </dx:LayoutGroup>
                                                                                                    </Items>
                                                                                                    <Paddings Padding="0px" />
                                                                                                    <Border BorderStyle="Solid" />
                                                                                                </dx:ASPxFormLayout>

                                                                                            </dx:PanelContent>
                                                                                        </PanelCollection>
                                                                                    </dx:ASPxCallbackPanel>

                                                                                </EditForm>

                                                                            </Templates>
                                                                            <SettingsPager NumericButtonCount="6">
                                                                                <FirstPageButton Visible="True">
                                                                                </FirstPageButton>
                                                                                <LastPageButton Visible="True">
                                                                                </LastPageButton>
                                                                            </SettingsPager>
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
                                                                            <SettingsText CommandDelete="刪除" CommandEdit="編輯" CommandNew="新增" SearchPanelEditorNullText="請輸入關鍵字" CommandCancel="取消" CommandUpdate="存入" Title="策略目標" />
                                                                            <EditFormLayoutProperties ColCount="2">
                                                                                <Items>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="MANO">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="MADESC">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="SALQTY">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="SLUNITNM">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="SLUTPRIC">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="SALAMT">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="USECARDID">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="USECARDAMT">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:GridViewColumnLayoutItem ColumnName="DUEAMT">
                                                                                    </dx:GridViewColumnLayoutItem>
                                                                                    <dx:EditModeCommandLayoutItem ColSpan="2" HorizontalAlign="Right">
                                                                                    </dx:EditModeCommandLayoutItem>
                                                                                </Items>
                                                                            </EditFormLayoutProperties>
                                                                            <Columns>
                                                                                <dx:GridViewCommandColumn ShowDeleteButton="True" ShowEditButton="True" ShowNewButtonInHeader="True" VisibleIndex="0" Width="6%" MaxWidth="60" MinWidth="75">
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
                                                                                    <PropertiesComboBox DataSourceID="sdsUnit" TextField="UNITNM" ValueField="UNITNM" DropDownStyle="DropDown">
                                                                                    </PropertiesComboBox>
                                                                                </dx:GridViewDataComboBoxColumn>
                                                                                <dx:GridViewDataTextColumn Caption="完成規格" FieldName="FNSHSPEC" ShowInCustomizationForm="True" VisibleIndex="16" Width="6%" MaxWidth="75" MinWidth="60">
                                                                                </dx:GridViewDataTextColumn>
                                                                                <dx:GridViewDataTextColumn Caption="準備人時" FieldName="SPREPHMANTICK" VisibleIndex="17" Width="6%" MaxWidth="75" MinWidth="60">
                                                                                </dx:GridViewDataTextColumn>
                                                                                <dx:GridViewDataTextColumn Caption="準備機時" FieldName="SPREPMACHTICK" VisibleIndex="18" Width="6%" MaxWidth="75" MinWidth="60">
                                                                                </dx:GridViewDataTextColumn>
                                                                                <dx:GridViewDataTextColumn Caption="加工機時" FieldName="SBTCHMACHTICK" VisibleIndex="20" Width="6%" MaxWidth="75" MinWidth="60">
                                                                                </dx:GridViewDataTextColumn>
                                                                                <dx:GridViewDataTextColumn Caption="加工人時" FieldName="SBTCHHMANTICK" VisibleIndex="19" Width="6%" MaxWidth="75" MinWidth="60">
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
                                                                    </dx:PanelContent>
                                                                </PanelCollection>
                                                            </dx:ASPxCallbackPanel>
                                                        </dx:PanelContent>
                                                    </PanelCollection>
                                                </dx:ASPxPanel>
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


        <dx:ASPxLoadingPanel ID="lplProcessing" runat="server" Modal="True" Text="處理中&amp;hellip;" ClientInstanceName="lplProcessing">
        </dx:ASPxLoadingPanel>
        <asp:SqlDataSource ID="sdsMaRout" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [MaRout] WHERE [MANO] = @MANO AND [MROUTID] = @MROUTID" InsertCommand="INSERT INTO MaRout(MANO, MROUTID, MROUTNM, MROUTDESC, BTCHPDQTY, TSPREPHMANTICK, TSPREPMACHTICK, TSBTCHHMANTICK, TSBTCHMACHTICK, ISWODEFAULT) VALUES (@MANO, @MROUTID, @MROUTNM, @MROUTDESC, @BTCHPDQTY, @TSPREPHMANTICK, @TSPREPMACHTICK, @TSBTCHHMANTICK, @TSBTCHMACHTICK, @ISWODEFAULT)" SelectCommand="SELECT MaRout.MANO, Ma.MADESC, Ma.MASPEC, Ma.RTUNITNM, MaRout.MROUTID, MaRout.MROUTNM, MaRout.MROUTDESC, MaRout.BTCHPDQTY, MaRout.TSPREPHMANTICK ,MaRout.TSPREPMACHTICK ,MaRout.TSBTCHHMANTICK  ,MaRout.TSBTCHMACHTICK,  MaRout.ISWODEFAULT, Ma.RTUNITNM
FROM MaRout LEFT JOIN Ma ON (MaRout.MANO = Ma.MANO)
WHERE MaRout.MANO=@MANO AND MaRout.MROUTID=@MROUTID"
            UpdateCommand="UPDATE MaRout SET MROUTNM = @MROUTNM, MROUTDESC = @MROUTDESC, BTCHPDQTY = @BTCHPDQTY, ISWODEFAULT = @ISWODEFAULT WHERE (MANO = @MANO) AND (MROUTID = @MROUTID)" OnDeleted="sdsMaRout_Deleted">
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
            <SelectParameters>
                <asp:SessionParameter Name="MANO" SessionField="MANO" />
                <asp:SessionParameter Name="MROUTID" SessionField="MROUTID" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="MROUTNM" Type="String" />
                <asp:Parameter Name="MROUTDESC" Type="String" />
                <asp:Parameter Name="BTCHPDQTY" Type="Double" />
                <asp:Parameter Name="MANO" Type="String" />
                <asp:Parameter Name="MROUTID" Type="String" />
                <asp:Parameter Name="ISWODEFAULT" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsMaRoutProc" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [MaRoutProc] WHERE [MANO] = @MANO AND [MROUTID] = @MROUTID AND [MROUTSRNO] = @MROUTSRNO" InsertCommand="INSERT INTO MaRoutProc(MANO, MROUTID, MROUTSRNO, MPROCID, UNITCOST, SPREPHMANTM, SPREPMACHTM, SBTCHHMANTM, SBTCHMACHTM, PROCFMULAOPT, PUTNINPTFORMAT, PNPARA1, PNMATHOPTR, PNPARA2, PUTNQTY, PNUNITNM, MATHOPTR, PN2RTEXGRATE, PHPARA1, PHMATHOPTR, PHPARA2, PFNHQTY, FNUNITNM, FNSHSPEC, MPROCMD, SPREPHMANTICK, SPREPMACHTICK, SBTCHHMANTICK, SBTCHMACHTICK, FNSHVALDMD, FNSHTLRNRATE) VALUES (@MANO, @MROUTID, @MROUTSRNO, @MPROCID, @UNITCOST, @SPREPHMANTM, @SPREPMACHTM, @SBTCHHMANTM, @SBTCHMACHTM, @PROCFMULAOPT, @PUTNINPTFORMAT, @PNPARA1, @PNMATHOPTR, @PNPARA2, @PUTNQTY, @PNUNITNM, @MATHOPTR, @PN2RTEXGRATE, @PHPARA1, @PHMATHOPTR, @PHPARA2, @PFNHQTY, @FNUNITNM, @FNSHSPEC, @MPROCMD, @SPREPHMANTICK, @SPREPMACHTICK, @SBTCHHMANTICK, @SBTCHMACHTICK, @FNSHVALDMD, @FNSHTLRNRATE)" SelectCommand="SELECT MaRoutProc.MANO, MaRoutProc.MROUTID, MaRoutProc.MROUTSRNO, MaRoutProc.MPROCID, ManuProc.MPROCNM, ManuProc.MPROCDESC, ManuProc.MLINEID, ManuLine.MLINENM, MaRoutProc.UNITCOST,
 MaRoutProc.SPREPHMANTICK, MaRoutProc.SPREPMACHTICK, MaRoutProc.SBTCHHMANTICK, MaRoutProc.SBTCHMACHTICK, MaRoutProc.SPREPHMANTM, MaRoutProc.SPREPMACHTM, MaRoutProc.SBTCHHMANTM, MaRoutProc.SBTCHMACHTM,
 MaRoutProc.PROCFMULAOPT, MaRoutProc.PUTNINPTFORMAT, MaRoutProc.PNPARA1, MaRoutProc.PNMATHOPTR, MaRoutProc.PNPARA2, MaRoutProc.PUTNQTY, MaRoutProc.PNUNITNM, MaRoutProc.MATHOPTR, MaRoutProc.PN2RTEXGRATE,
 MaRoutProc.PHPARA1, MaRoutProc.PHMATHOPTR, MaRoutProc.PHPARA2, MaRoutProc.PFNHQTY, MaRoutProc.FNUNITNM, MaRoutProc.FNSHSPEC, MaRoutProc.MPROCMD, MaRoutProc.FNSHVALDMD, MaRoutProc.FNSHTLRNRATE 
FROM MaRoutProc LEFT OUTER JOIN ManuProc ON MaRoutProc.MPROCID = ManuProc.MPROCID 
 LEFT OUTER JOIN ManuLine ON ManuProc.MLINEID = ManuLine.MLINEID 
WHERE (MaRoutProc.MANO = @MANO) AND (MaRoutProc.MROUTID = @MROUTID) 
ORDER BY MaRoutProc.MROUTSRNO"
            UpdateCommand="UPDATE MaRoutProc SET MPROCID = @MPROCID, UNITCOST = @UNITCOST, SPREPHMANTICK = @SPREPHMANTICK, SPREPMACHTICK = @SPREPMACHTICK, SBTCHHMANTICK = @SBTCHHMANTICK, SBTCHMACHTICK = @SBTCHMACHTICK, SPREPHMANTM = @SPREPHMANTM, SPREPMACHTM = @SPREPMACHTM, SBTCHHMANTM = @SBTCHHMANTM, SBTCHMACHTM = @SBTCHMACHTM, PROCFMULAOPT = @PROCFMULAOPT, PUTNINPTFORMAT = @PUTNINPTFORMAT, PNPARA1 = @PNPARA1, PNMATHOPTR = @PNMATHOPTR, PNPARA2 = @PNPARA2, PUTNQTY = @PUTNQTY, PNUNITNM = @PNUNITNM, MATHOPTR = @MATHOPTR, PN2RTEXGRATE = @PN2RTEXGRATE, PHPARA1 = @PHPARA1, PHMATHOPTR = @PHMATHOPTR, PHPARA2 = @PHPARA2, PFNHQTY = @PFNHQTY, FNUNITNM = @FNUNITNM, FNSHSPEC = @FNSHSPEC, MPROCMD = @MPROCMD, FNSHVALDMD = @FNSHVALDMD, FNSHTLRNRATE = @FNSHTLRNRATE WHERE (MANO = @MANO) AND (MROUTID = @MROUTID) AND (MROUTSRNO = @MROUTSRNO)" OnDeleted="sdsMaRoutProc_Deleted" OnInserted="sdsMaRoutProc_Deleted" OnUpdated="sdsMaRoutProc_Updated">
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
                <asp:Parameter Name="SPREPHMANTICK" Type="Int64" />
                <asp:Parameter Name="SPREPMACHTICK" Type="Int64" />
                <asp:Parameter Name="SBTCHHMANTICK" Type="Int64" />
                <asp:Parameter Name="SBTCHMACHTICK" Type="Int64" />
                <asp:Parameter Name="SPREPHMANTM" DbType="Time" />
                <asp:Parameter Name="SPREPMACHTM" DbType="Time" />
                <asp:Parameter Name="SBTCHHMANTM" DbType="Time" />
                <asp:Parameter Name="SBTCHMACHTM" DbType="Time" />
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
                <asp:Parameter Name="FNSHVALDMD" />
                <asp:Parameter Name="FNSHTLRNRATE" />
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
