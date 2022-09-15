<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrganMain.aspx.cs" Inherits="WebMES.Admin.OrganMain" %>

<%@ Register Assembly="DevExpress.Web.ASPxTreeList.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxTreeList" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/Admin/ManuMenuPanel.ascx" TagPrefix="uc1" TagName="ManuMenuPanel" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="../stylesheets/DragAndDrop.css">
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

        .auto-style7 {
            width: 60%;
            height: 15px;
        }

        .auto-style8 {
            width: 27%;
            height: 35px;
        }

        #divTOOLHLINK {
            position: relative;
            float: right;
            margin-top: 10px;
            margin-left: 10px;
            padding: 0px 0px 0px 0px;
            vertical-align: middle;
        }

        #divUploadControl {
            position: relative;
            float: left;
            left: 0px;
        }

        #UploadControlTool {
            position: relative;
            width: 100px;
            float: left;
            left: 5px;
        }
    </style>
    <script type="text/javascript">
        var WebUrlBase = '<%# Application["WebSite-APPURLBASE"].ToString() %>';
        var OrganPhoto = '<%# Application["ORGANPHOTODIR"].ToString() %>';

        function OnListBoxValueChanged() {
            MainDBTree.Refresh();
        }
        function UpdateGridHeight() {
            MainDBTree.SetHeight(0);
            var containerHeight = ASPxClientUtils.GetDocumentClientHeight();
            if (document.body.scrollHeight > containerHeight)
                containerHeight = document.body.scrollHeight;
            MainDBTree.SetHeight(containerHeight - topPanel.GetHeight() - MainDBTreeMenu.GetHeight());
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

        function onUploadControlFileUploadComplete(s, e) {
            if (e.isValid) {
                document.getElementById("uploadedImage").src = WebUrlBase + OrganPhoto + e.callbackData;
                //document.getElementById("uploadedImage").src = "../HotNews/" + e.callbackData;
                edTOOLHLINK.SetText(e.callbackData);
                edPHOTOFILENM.SetText(e.callbackData);
            }
            setElementVisible("uploadedImage", e.isValid);
        }

        function onImageLoad() {
            var ExternalDropZone = document.getElementById("ExternalDropZone");
            var uploadedImage = document.getElementById("uploadedImage");
            uploadedImage.width = Math.min(uploadedImage.width, 378);
            uploadedImage.height = Math.min(uploadedImage.height, 180);
            uploadedImage.style.left = (ExternalDropZone.clientWidth - uploadedImage.width) / 2 + "px";
            uploadedImage.style.top = (ExternalDropZone.clientHeight - uploadedImage.height) / 2 + "px";
            setElementVisible("DragZone", false);
        }

        function setElementVisible(elementId, visible) {
            document.getElementById(elementId).className = visible ? "" : "hidden";
        }
    </script>

</head>
<body>
    <form id="FA000" runat="server">
        <uc1:MainMenuPanel runat="server" ID="MainMenuPanel" />
        <uc1:ManuMenuPanel runat="server" ID="ManuMenuPanel" />
        <div>
            <dx:ASPxTextBox ID="SearchTextBox" runat="server" Style="float: left; vertical-align: middle; margin-top: 6px; margin-left: 6px; margin-right: 8px" ClientInstanceName="SearchTextBox" Width="30%" NullText="請輸入關鍵字">
            </dx:ASPxTextBox>

            <dx:ASPxMenu ID="MainDBTreeMenu" runat="server" ShowAsToolbar="True" ShowPopOutImages="True" EnableTheming="True" EnableAdaptivity="True" OnItemClick="MainDBTreeMenu_ItemClick" Width="50%">
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
                        </Items>
                        <Image IconID="print_print_32x32office2013"></Image>
                    </dx:MenuItem>
                    <dx:MenuItem ToolTip="儲存" GroupName="DataBar" Visible="False" AdaptivePriority="1">
                        <Image IconID="save_save_32x32"></Image>
                    </dx:MenuItem>
                </Items>
            </dx:ASPxMenu>
        </div>
        <dx:ASPxPanel ID="MainDBTreePanel" runat="server" ClientInstanceName="MainDBTreePanel" EnableTheming="True" Width="100%">
            <SettingsAdaptivity CollapseAtWindowInnerWidth="580" />
            <PanelCollection>
                <dx:PanelContent runat="server">

                    <dx:ASPxTreeList ID="MainDBTree" runat="server" AutoGenerateColumns="False" DataSourceID="sdsOrgan" KeyFieldName="DPTNO" ParentFieldName="PARDPTNO" Width="100%" ClientInstanceName="MainDBTree" EnableTheming="True" OnInitNewNode="MainDBTree_InitNewNode" OnNodeDeleted="MainDBTree_NodeDeleted" OnNodeInserted="MainDBTree_NodeInserted" OnNodeInserting="MainDBTree_NodeInserting" OnNodeUpdating="MainDBTree_NodeUpdating">
                        <Columns>
                            <dx:TreeListTextColumn FieldName="DPTNO" ShowInCustomizationForm="True" VisibleIndex="15" Caption="部門代號" Width="7%" AllowEllipsisInText="True">
                            </dx:TreeListTextColumn>
                            <dx:TreeListTextColumn FieldName="DPTNM" ShowInCustomizationForm="True" VisibleIndex="0" Caption="部門名稱" AllowEllipsisInText="True" Width="15%">
                            </dx:TreeListTextColumn>
                            <dx:TreeListTextColumn FieldName="PARDPTNO" ShowInCustomizationForm="True" VisibleIndex="16" Caption="上級部門代號" Width="5%" Visible="False">
                            </dx:TreeListTextColumn>
                            <dx:TreeListTextColumn Caption="聯絡地址" FieldName="DPTADDR" ShowInCustomizationForm="True" VisibleIndex="7" AllowEllipsisInText="True" Width="35%">
                            </dx:TreeListTextColumn>
                            <dx:TreeListTextColumn Caption="聯絡郵區" FieldName="DPTZIPCD" ShowInCustomizationForm="True" VisibleIndex="6" AllowEllipsisInText="True" Width="5%">
                            </dx:TreeListTextColumn>
                            <dx:TreeListTextColumn Caption="聯絡電話" FieldName="DPTTELNO" ShowInCustomizationForm="True" VisibleIndex="8" AllowEllipsisInText="True" Width="15%">
                            </dx:TreeListTextColumn>
                            <dx:TreeListTextColumn Caption="EMail Box" FieldName="DPTEMAILBOX" ShowInCustomizationForm="True" VisibleIndex="10" AllowEllipsisInText="True" Width="15%">
                            </dx:TreeListTextColumn>
                            <dx:TreeListCommandColumn ShowInCustomizationForm="True" VisibleIndex="17" ButtonType="Image" ShowNewButtonInHeader="True" Width="10%">
                                <EditButton Visible="True">
                                    <Image Url="~/Images/Navigator/Grid_Edit.png">
                                    </Image>
                                </EditButton>
                                <NewButton Visible="True">
                                    <Image Url="~/Images/Navigator/Grid_Add.png">
                                    </Image>
                                </NewButton>
                                <DeleteButton Visible="True" Text="刪除">
                                    <Image Url="~/Images/Navigator/Grid_Delete.png">
                                    </Image>
                                </DeleteButton>
                                <UpdateButton>
                                    <Image Url="~/Images/Navigator/Grid_Post.png">
                                    </Image>
                                </UpdateButton>
                                <CancelButton>
                                    <Image Url="~/Images/Navigator/Grid_Cancel.png">
                                    </Image>
                                </CancelButton>
                            </dx:TreeListCommandColumn>
                        </Columns>
                        <Settings HorizontalScrollBarMode="Auto" VerticalScrollBarMode="Auto" />
                        <SettingsBehavior AllowFocusedNode="True" AutoExpandAllNodes="True" />
                        <SettingsPager EnableAdaptivity="True">
                        </SettingsPager>
                        <SettingsEditing AllowNodeDragDrop="True" Mode="EditFormAndDisplayNode" />
                        <SettingsEditing Mode="EditFormAndDisplayNode" />
                        <SettingsPopupEditForm Width="500" />

                        <SettingsPopup>
                            <EditForm Width="500px"></EditForm>

                            <HeaderFilter MinHeight="140px"></HeaderFilter>
                        </SettingsPopup>

                        <Templates>
                            <EditForm>
                                <dx:ASPxFormLayout ID="floDpt" runat="server" AlignItemCaptionsInAllGroups="True" EnableTheming="True" ShowItemCaptionColon="False">
                                    <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="580" />
                                    <Items>
                                        <dx:LayoutGroup Caption="" ColCount="3">
                                            <Items>
                                                <dx:LayoutItem Caption="部門名稱" FieldName="DPTNM">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edDPTNM" runat="server" Text='<%# Eval("DPTNM") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="部門簡碼" FieldName="DPTBID">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edDPTBID" runat="server" Text='<%# Eval("DPTBID") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="部門代號" FieldName="DPTNO">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edDPTNO" runat="server" Value='<%# Eval("DPTNO") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="上級部門" FieldName="SUPDPTNO">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxComboBox ID="edSUPDPTNO" runat="server" DataSourceID="sdsDpts" TextField="DPTNM" ValueField="DPTNO" Value='<%# Eval("PARDPTNO") %>'>
                                                            </dx:ASPxComboBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="部門主管" FieldName="DPTHANDLER">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxComboBox ID="edDPTHANDLER" runat="server" DataSourceID="sdsPrsn" TextField="PRSNNM" ValueField="DPTNO" Value='<%# Eval("DPTHANDLER") %>'>
                                                            </dx:ASPxComboBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="部門掛牌日" FieldName="VALIDDT">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxDateEdit ID="edVALIDDT" runat="server" Value='<%# Eval("VALIDDT") %>'>
                                                            </dx:ASPxDateEdit>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="子公司/組織" FieldName="ISCORP">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxCheckBox ID="edISCORP" runat="server" Value='<%# Eval("ISCORP") %>' ToolTip="組織架構下的子公司">
                                                            </dx:ASPxCheckBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="分店/組織" FieldName="ISSHOP">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxCheckBox ID="edISSHOP" runat="server" Value='<%# Eval("ISSHOP") %>' ToolTip="組織架構下的分店">
                                                            </dx:ASPxCheckBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="部門裁撤日" ColSpan="1" FieldName="DISSOLVDT">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxDateEdit ID="edDISSOLVDT" runat="server" Value='<%# Eval("DISSOLVDT") %>'>
                                                            </dx:ASPxDateEdit>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="電子信箱" ColSpan="1" FieldName="DPTEMAILBOX">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edDPTEMAILBOX" runat="server" Value='<%# Eval("DPTEMAILBOX") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="服務電話" ColSpan="1" FieldName="DPTTELNO">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edDPTTELNO" runat="server" Value='<%# Eval("DPTTELNO") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="服務傳真" ColSpan="1" FieldName="DPTFAXNO">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edDPTFAXNO" runat="server" Value='<%# Eval("DPTFAXNO") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="營業地址" ColSpan="2" FieldName="DPTADDR" ColumnSpan="2" Width="66%">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edDPTADDR" runat="server" Value='<%# Eval("DPTADDR") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="營業時間" ColSpan="1" FieldName="OPRTPERD">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edOPRTPERD" runat="server" Value='<%# Eval("OPRTPERD") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="連結網址" ColSpan="2" FieldName="WEBSITE" ColumnSpan="2" Width="66%">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edWEBSITE" runat="server" Value='<%# Eval("WEBSITE") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="使用貨倉" ColSpan="1" FieldName="STORID">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxComboBox ID="edSTORID" runat="server" DataSourceID="sdsStores" TextField="STORNM" ValueField="STORID" Value='<%# Eval("STORID") %>'>
                                                            </dx:ASPxComboBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="LINEAT網址" ColSpan="2" FieldName="LINEAT" ColumnSpan="2" RowSpan="2" Width="66%">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edLINEAT" runat="server" Value='<%# Eval("LINEAT") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem FieldName="PHOTOFILENM" Caption="圖檔名稱">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxTextBox ID="edPHOTOFILENM" runat="server" ClientInstanceName="edPHOTOFILENM" Value='<%#Eval("PHOTOFILENM") %>'>
                                                            </dx:ASPxTextBox>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem FieldName="PHOTOFILENM" RowSpan="6" ShowCaption="False" Name="UploadTool">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <div id="ExternalDropZone" class="DropZoneExternalTool" style="height: 180px; width: 262px">
                                                                            <div id="DragZone">
                                                                                <span class="DragZoneText">拖曳主題照片(317 : 218)</span>
                                                                            </div>
                                                                            <img id="uploadedImage" src='<%# ToolImageUrl(Eval("DPTNO")+";"+Eval("PHOTOFILENM")) %>' class="DropZoneExternal" style="height: 180px; width: 262px" alt="" onload="onImageLoad()" />
                                                                            <div id="DropZone" class="hidden">
                                                                                <span class="DropZoneText">放置主題照片(317 : 218)</span>
                                                                            </div>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 20px">
                                                                        <div id="divTOOLHLINK">
                                                                            <dx:ASPxTextBox ID="edTOOLHLINK" runat="server" ClientInstanceName="edTOOLHLINK" Value='<%# Eval("PHOTOFILENM") %>' Width="100%">
                                                                            </dx:ASPxTextBox>
                                                                        </div>
                                                                        <dx:ASPxUploadControl ID="UploadControl" runat="server" AutoStartUpload="True" ClientInstanceName="UploadControl" CssClass="UploadControl" DialogTriggerID="ExternalDropZone" OnFileUploadComplete="UploadControl_FileUploadComplete" ShowProgressPanel="True" ShowTextBox="False" UploadMode="Auto">
                                                                            <ValidationSettings AllowedFileExtensions=".jpg, .jpeg, .gif, .png" MaxFileSize="4194304">
                                                                                <ErrorStyle CssClass="validationMessage" />
                                                                            </ValidationSettings>
                                                                            <ClientSideEvents DropZoneEnter="function(s, e) { if(e.dropZone.id == 'ExternalDropZone') setElementVisible('DropZone', true); }" DropZoneLeave="function(s, e) { if(e.dropZone.id == 'ExternalDropZone') setElementVisible('DropZone', false); }" FileUploadComplete="onUploadControlFileUploadComplete" />
                                                                            <BrowseButton Text="">
                                                                                <Image Url="~/Images/Navigator/Upload.png">
                                                                                </Image>
                                                                            </BrowseButton>
                                                                            <RemoveButton Text="">
                                                                                <Image IconID="actions_close_32x32office2013">
                                                                                </Image>
                                                                            </RemoveButton>
                                                                            <AdvancedModeSettings DropZoneText="" EnableDragAndDrop="True" ExternalDropZoneID="ExternalDropZone">
                                                                            </AdvancedModeSettings>
                                                                            <ProgressBarStyle CssClass="UploadControlProgressBar">
                                                                            </ProgressBarStyle>
                                                                            <DropZoneStyle CssClass="UploadControlDropZone">
                                                                            </DropZoneStyle>
                                                                        </dx:ASPxUploadControl>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>

                                                <dx:LayoutItem Caption="營業內容" FieldName="OPRTDESC" ColumnSpan="2" RowSpan="3" Height="120px" Width="66%">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxMemo ID="edOPRTDESC" runat="server" Value='<%# Eval("OPRTDESC") %>'>
                                                            </dx:ASPxMemo>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="價目表" FieldName="PRICDESC" ColumnSpan="2" RowSpan="3" Height="120px" Width="66%">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxMemo ID="edPRICDESC" runat="server" Value='<%# Eval("PRICDESC") %>'>
                                                            </dx:ASPxMemo>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                            </Items>
                                        </dx:LayoutGroup>

                                        <dx:LayoutGroup ColCount="2" ShowCaption="False" Width="100%" Visible="False">
                                            <GroupBoxStyle>
                                                <border borderstyle="Dotted" />
                                            </GroupBoxStyle>
                                            <Items>
                                                <dx:LayoutItem Caption="" HorizontalAlign="Right">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxButton ID="btnSubmit" runat="server" ClientInstanceName="btnSubmit" OnClick="btnSubmit_Click" Text="儲存" Width="50%" AutoPostBack="False">
                                                            </dx:ASPxButton>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                                <dx:LayoutItem Caption="" HorizontalAlign="Right">
                                                    <LayoutItemNestedControlCollection>
                                                        <dx:LayoutItemNestedControlContainer runat="server">
                                                            <dx:ASPxButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" Text="取消" Width="50%" AutoPostBack="False">
                                                            </dx:ASPxButton>
                                                        </dx:LayoutItemNestedControlContainer>
                                                    </LayoutItemNestedControlCollection>
                                                </dx:LayoutItem>
                                            </Items>
                                        </dx:LayoutGroup>

                                    </Items>
                                </dx:ASPxFormLayout>
                                <div style="text-align: right; padding-top: 8px">
                                    <dx:ASPxTreeListTemplateReplacement runat="server" ReplacementType="UpdateButton" />
                                    <dx:ASPxTreeListTemplateReplacement runat="server" ReplacementType="CancelButton" />
                                </div>

                            </EditForm>
                        </Templates>

                    </dx:ASPxTreeList>

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
            <ClientSideEvents PopUp="function(s, e) { cbxBDPTNO.Focus(); }" Init="function(s, e) {
pclSearchPanelInit();
}"
                Shown="function(s, e) {
cbkSearchPanel.PerformCallback('pclSearchPanel');
}" />
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
                                                    <td rowspan="8" class="auto-style1">
                                                        <div class="pcmSideSpacer">
                                                        </div>
                                                    </td>
                                                    <td class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel5" runat="server" AssociatedControlID="lblBDPTNO" Text="部門">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxComboBox ID="cbxBDPTNO" runat="server" Style="float: left; margin-right: 2px" ClientInstanceName="cbxBDPTNO" DataSourceID="sdsDpts" TextField="DPTNM" ValueField="DPTNO" Width="55%">
                                                        </dx:ASPxComboBox>
                                                        <dx:ASPxCheckBox ID="cbxBEDBISCONTAINSUBDPT" runat="server" Style="float: right; margin-right: 2px" CheckState="Checked" Text="含所有子部門" Width="40%" Checked="True">
                                                        </dx:ASPxCheckBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style6"></td>
                                                    <td class="auto-style11"></td>
                                                </tr>
                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="ASPxLabel2" runat="server" AssociatedControlID="lblBACNTYR" Text="部門選項">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style7">
                                                        <dx:ASPxRadioButtonList ID="EDBDPTSHIFTMD" runat="server" RepeatDirection="Horizontal" SelectedIndex="0" RepeatLayout="OrderedList">
                                                            <ClientSideEvents ValueChanged="function(s, e) {
cbkSearchPanel.PerformCallback('EDBDPTSHIFTMD');
}" />
                                                            <Items>
                                                                <dx:ListEditItem Text="運作中部門" Value="1" Selected="True" />
                                                                <dx:ListEditItem Text="歷年部門" Value="2" />
                                                                <dx:ListEditItem Text="已裁撤部門" Value="3" />
                                                            </Items>
                                                            <Border BorderStyle="None" />
                                                        </dx:ASPxRadioButtonList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="auto-style13"></td>
                                                    <td class="auto-style14"></td>
                                                </tr>

                                                <tr>
                                                    <td rowspan="1" class="auto-style2">
                                                        <dx:ASPxLabel ID="lblBSTDISSOLVDT" runat="server" AssociatedControlID="lblBACNTYR" Text="部門裁撤日" Visible="False">
                                                        </dx:ASPxLabel>
                                                    </td>
                                                    <td class="auto-style3">
                                                        <dx:ASPxDateEdit ID="tbxBSTDISSOLVDT" runat="server" Style="float: left; margin-right: 2px" ClientInstanceName="tbxBSTDISSOLVDT" Width="45%" EditFormat="Custom" EditFormatString="yyyy/MM/dd" UseMaskBehavior="True" Visible="False">
                                                            <ValidationSettings CausesValidation="True" ErrorDisplayMode="Text" ErrorText="" SetFocusOnError="True" ValidateOnLeave="False">
                                                                <ErrorFrameStyle Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                                <RegularExpression ErrorText="" />
                                                                <RequiredField ErrorText="" />
                                                            </ValidationSettings>
                                                        </dx:ASPxDateEdit>
                                                        <dx:ASPxLabel ID="lblDASHDISSOLVDT" runat="server" Style="float: left; margin-left: 2px; margin-right: 2px" AssociatedControlID="lblBACNTYR" Text="~" Width="5%" Visible="False">
                                                        </dx:ASPxLabel>

                                                        <dx:ASPxDateEdit ID="tbxBEDDISSOLVDT" runat="server" Style="float: right; margin-right: 2px" ClientInstanceName="tbxBEDDISSOLVDT" Width="45%" EditFormat="Custom" EditFormatString="yyyy/MM/dd" UseMaskBehavior="True" Visible="False">
                                                            <DateRangeSettings StartDateEditID="tbxBSTDISSOLVDT" />
                                                            <ValidationSettings CausesValidation="True" ErrorDisplayMode="Text" ErrorText="" SetFocusOnError="True" ValidateOnLeave="False">
                                                                <ErrorFrameStyle Font-Size="10px">
                                                                    <ErrorTextPaddings PaddingLeft="0px" />
                                                                </ErrorFrameStyle>
                                                                <RegularExpression ErrorText="" />
                                                                <RequiredField ErrorText="" />
                                                            </ValidationSettings>
                                                        </dx:ASPxDateEdit>
                                                    </td>
                                                    <td rowspan="3" class="auto-style1">
                                                        <div class="pcmSideSpacer">
                                                        </div>
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
        <dx:ASPxTreeListExporter ID="MainDBGridExporter" runat="server" TreeListID="MainDBTree">
        </dx:ASPxTreeListExporter>
        <asp:SqlDataSource ID="sdsOrgan" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" DeleteCommand="DELETE FROM [Dpt] WHERE [DPTNO] = @DPTNO" InsertCommand="INSERT INTO [Dpt] ([DPTNO], [DPTHANDLER], [DPTNM], [VALIDDT], [DISSOLVDT], [DPTBID], [STORID], [DPTADDR], [DPTZIPCD], [DPTTELNO], [DPTFAXNO], [DPTEMAILBOX], [WEBSITE], [LINEAT], [OPRTDESC], [OPRTPERD], [PRICDESC], [PHOTOFILENM], [PHOTODESC], [ISCORP], [ISSHOP]) VALUES (@DPTNO, @DPTHANDLER, @DPTNM, @VALIDDT, @DISSOLVDT, @DPTBID, @STORID, @DPTADDR, @DPTZIPCD, @DPTTELNO, @DPTFAXNO, @DPTEMAILBOX, @WEBSITE, @LINEAT, @OPRTDESC, @OPRTPERD, @PRICDESC, @PHOTOFILENM, @PHOTODESC, @ISCORP, @ISSHOP)" SelectCommand="SELECT SubDpt.DPTNO DPTNO, SubDpt.DPTNM DPTNM, Organ.DPTNO PARDPTNO,
 SubDpt.DPTHANDLER, SubDpt.VALIDDT, SubDpt.DISSOLVDT,
 Prsn.PRSNNM, SubDpt.DPTBID, SubDpt.STORID,
 SubDpt.DPTADDR, SubDpt.DPTZIPCD, SubDpt.DPTTELNO, SubDpt.DPTFAXNO, SubDpt.DPTEMAILBOX,
 SubDpt.WEBSITE, SubDpt.LINEAT, SubDpt.OPRTDESC ,SubDpt.OPRTPERD, SubDpt.PRICDESC,
 SubDpt.PHOTOFILENM,  SubDpt.PHOTODESC, SubDpt.ISCORP, SubDpt.ISSHOP
FROM Organ FULL JOIN Dpt SubDpt ON (Organ.SUBDPTNO=SubDpt.DPTNO)
 LEFT JOIN Prsn ON (SubDpt.DPTHANDLER=Prsn.DPTNO)"
            UpdateCommand="UPDATE [Dpt] SET [DPTHANDLER] = @DPTHANDLER, [DPTNM] = @DPTNM, [VALIDDT] = @VALIDDT, [DISSOLVDT] = @DISSOLVDT, [DPTBID] = @DPTBID, [STORID] = @STORID, [DPTADDR] = @DPTADDR, [DPTZIPCD] = @DPTZIPCD, [DPTTELNO] = @DPTTELNO, [DPTFAXNO] = @DPTFAXNO, [DPTEMAILBOX] = @DPTEMAILBOX, [WEBSITE] = @WEBSITE, [LINEAT] = @LINEAT, [OPRTDESC] = @OPRTDESC, [OPRTPERD] = @OPRTPERD, [PRICDESC] = @PRICDESC, [PHOTOFILENM] = @PHOTOFILENM, [PHOTODESC] = @PHOTODESC, [ISCORP] = @ISCORP, [ISSHOP] = @ISSHOP WHERE [DPTNO] = @DPTNO" OnDeleted="sdsOrgan_Deleted" OnInserted="sdsOrgan_Inserted" OnUpdated="sdsOrgan_Updated">
            <DeleteParameters>
                <asp:Parameter Name="DPTNO" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="DPTNO" Type="String" />
                <asp:Parameter Name="DPTHANDLER" Type="String" />
                <asp:Parameter Name="DPTNM" Type="String" />
                <asp:Parameter Name="VALIDDT" Type="DateTime" />
                <asp:Parameter Name="DISSOLVDT" Type="DateTime" />
                <asp:Parameter Name="DPTBID" Type="String" />
                <asp:Parameter Name="STORID" Type="String" />
                <asp:Parameter Name="DPTADDR" Type="String" />
                <asp:Parameter Name="DPTZIPCD" Type="String" />
                <asp:Parameter Name="DPTTELNO" Type="String" />
                <asp:Parameter Name="DPTFAXNO" Type="String" />
                <asp:Parameter Name="DPTEMAILBOX" Type="String" />
                <asp:Parameter Name="WEBSITE" Type="String" />
                <asp:Parameter Name="LINEAT" Type="String" />
                <asp:Parameter Name="OPRTDESC" Type="String" />
                <asp:Parameter Name="OPRTPERD" Type="String" />
                <asp:Parameter Name="PRICDESC" Type="String" />
                <asp:Parameter Name="PHOTOFILENM" Type="String" />
                <asp:Parameter Name="PHOTODESC" Type="String" />
                <asp:Parameter Name="ISCORP" Type="Boolean" />
                <asp:Parameter Name="ISSHOP" Type="Boolean" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="DPTHANDLER" Type="String" />
                <asp:Parameter Name="DPTNM" Type="String" />
                <asp:Parameter Name="VALIDDT" Type="DateTime" />
                <asp:Parameter Name="DISSOLVDT" Type="DateTime" />
                <asp:Parameter Name="DPTBID" Type="String" />
                <asp:Parameter Name="STORID" Type="String" />
                <asp:Parameter Name="DPTADDR" Type="String" />
                <asp:Parameter Name="DPTZIPCD" Type="String" />
                <asp:Parameter Name="DPTTELNO" Type="String" />
                <asp:Parameter Name="DPTFAXNO" Type="String" />
                <asp:Parameter Name="DPTEMAILBOX" Type="String" />
                <asp:Parameter Name="WEBSITE" Type="String" />
                <asp:Parameter Name="LINEAT" Type="String" />
                <asp:Parameter Name="OPRTDESC" Type="String" />
                <asp:Parameter Name="OPRTPERD" Type="String" />
                <asp:Parameter Name="PRICDESC" Type="String" />
                <asp:Parameter Name="PHOTOFILENM" Type="String" />
                <asp:Parameter Name="PHOTODESC" Type="String" />
                <asp:Parameter Name="ISCORP" Type="Boolean" />
                <asp:Parameter Name="ISSHOP" Type="Boolean" />
                <asp:Parameter Name="DPTNO" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsDpts" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Dpt.DPTNO, Dpt.DPTNM, Dpt.DPTHANDLER, Dpt.DPTADDR, Dpt.DPTZIPCD, Dpt.DPTTELNO, Dpt.DPTFAXNO, Dpt.DPTEMAILBOX,Dpt.WEBSITE ,Dpt.OPRTDESC ,Dpt.OPRTPERD, Dpt.PHOTOFILENM FROM Dpt WHERE Dpt.DISSOLVDT IS NULL"></asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [DPTNO], [DPTHANDLER], [DPTNM], [VALIDDT], [DISSOLVDT], [DPTBID], [STORID], [DPTADDR], [DPTZIPCD], [DPTTELNO], [DPTFAXNO], [DPTEMAILBOX], [WEBSITE], [LINEAT], [OPRTDESC], [OPRTPERD], [PRICDESC], [PHOTOFILENM], [PHOTODESC], [ISCORP], [ISSHOP] FROM [Dpt]" DeleteCommand="DELETE FROM [Dpt] WHERE [DPTNO] = @DPTNO" InsertCommand="INSERT INTO [Dpt] ([DPTNO], [DPTHANDLER], [DPTNM], [VALIDDT], [DISSOLVDT], [DPTBID], [STORID], [DPTADDR], [DPTZIPCD], [DPTTELNO], [DPTFAXNO], [DPTEMAILBOX], [WEBSITE], [LINEAT], [OPRTDESC], [OPRTPERD], [PRICDESC], [PHOTOFILENM], [PHOTODESC], [ISCORP], [ISSHOP]) VALUES (@DPTNO, @DPTHANDLER, @DPTNM, @VALIDDT, @DISSOLVDT, @DPTBID, @STORID, @DPTADDR, @DPTZIPCD, @DPTTELNO, @DPTFAXNO, @DPTEMAILBOX, @WEBSITE, @LINEAT, @OPRTDESC, @OPRTPERD, @PRICDESC, @PHOTOFILENM, @PHOTODESC, @ISCORP, @ISSHOP)" UpdateCommand="UPDATE [Dpt] SET [DPTHANDLER] = @DPTHANDLER, [DPTNM] = @DPTNM, [VALIDDT] = @VALIDDT, [DISSOLVDT] = @DISSOLVDT, [DPTBID] = @DPTBID, [STORID] = @STORID, [DPTADDR] = @DPTADDR, [DPTZIPCD] = @DPTZIPCD, [DPTTELNO] = @DPTTELNO, [DPTFAXNO] = @DPTFAXNO, [DPTEMAILBOX] = @DPTEMAILBOX, [WEBSITE] = @WEBSITE, [LINEAT] = @LINEAT, [OPRTDESC] = @OPRTDESC, [OPRTPERD] = @OPRTPERD, [PRICDESC] = @PRICDESC, [PHOTOFILENM] = @PHOTOFILENM, [PHOTODESC] = @PHOTODESC, [ISCORP] = @ISCORP, [ISSHOP] = @ISSHOP WHERE [DPTNO] = @DPTNO">
            <DeleteParameters>
                <asp:Parameter Name="DPTNO" Type="String" />
            </DeleteParameters>
            <InsertParameters>
                <asp:Parameter Name="DPTNO" Type="String" />
                <asp:Parameter Name="DPTHANDLER" Type="String" />
                <asp:Parameter Name="DPTNM" Type="String" />
                <asp:Parameter Name="VALIDDT" Type="DateTime" />
                <asp:Parameter Name="DISSOLVDT" Type="DateTime" />
                <asp:Parameter Name="DPTBID" Type="String" />
                <asp:Parameter Name="STORID" Type="String" />
                <asp:Parameter Name="DPTADDR" Type="String" />
                <asp:Parameter Name="DPTZIPCD" Type="String" />
                <asp:Parameter Name="DPTTELNO" Type="String" />
                <asp:Parameter Name="DPTFAXNO" Type="String" />
                <asp:Parameter Name="DPTEMAILBOX" Type="String" />
                <asp:Parameter Name="WEBSITE" Type="String" />
                <asp:Parameter Name="LINEAT" Type="String" />
                <asp:Parameter Name="OPRTDESC" Type="String" />
                <asp:Parameter Name="OPRTPERD" Type="String" />
                <asp:Parameter Name="PRICDESC" Type="String" />
                <asp:Parameter Name="PHOTOFILENM" Type="String" />
                <asp:Parameter Name="PHOTODESC" Type="String" />
                <asp:Parameter Name="ISCORP" Type="Boolean" />
                <asp:Parameter Name="ISSHOP" Type="Boolean" />
            </InsertParameters>
            <UpdateParameters>
                <asp:Parameter Name="DPTHANDLER" Type="String" />
                <asp:Parameter Name="DPTNM" Type="String" />
                <asp:Parameter Name="VALIDDT" Type="DateTime" />
                <asp:Parameter Name="DISSOLVDT" Type="DateTime" />
                <asp:Parameter Name="DPTBID" Type="String" />
                <asp:Parameter Name="STORID" Type="String" />
                <asp:Parameter Name="DPTADDR" Type="String" />
                <asp:Parameter Name="DPTZIPCD" Type="String" />
                <asp:Parameter Name="DPTTELNO" Type="String" />
                <asp:Parameter Name="DPTFAXNO" Type="String" />
                <asp:Parameter Name="DPTEMAILBOX" Type="String" />
                <asp:Parameter Name="WEBSITE" Type="String" />
                <asp:Parameter Name="LINEAT" Type="String" />
                <asp:Parameter Name="OPRTDESC" Type="String" />
                <asp:Parameter Name="OPRTPERD" Type="String" />
                <asp:Parameter Name="PRICDESC" Type="String" />
                <asp:Parameter Name="PHOTOFILENM" Type="String" />
                <asp:Parameter Name="PHOTODESC" Type="String" />
                <asp:Parameter Name="ISCORP" Type="Boolean" />
                <asp:Parameter Name="ISSHOP" Type="Boolean" />
                <asp:Parameter Name="DPTNO" Type="String" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsPrsn" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT Prsn.DPTNO, Prsn.PRSNNM, Prsn.DPTNO,  Prsn.TITLEID
FROM Prsn 
WHERE Prsn.QUITDT IS NULL 
ORDER BY  Prsn.DPTNO"></asp:SqlDataSource>
        <asp:SqlDataSource ID="sdsStores" runat="server" ConnectionString="<%$ ConnectionStrings:WinSisTmplConnectionString %>" SelectCommand="SELECT [STORID], [STORNM] FROM [Stor]"></asp:SqlDataSource>

    </form>
</body>
</html>
