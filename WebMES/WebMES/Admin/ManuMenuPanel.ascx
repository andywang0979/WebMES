<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManuMenuPanel.ascx.cs" Inherits="WebMES.Admin.ManuMenuPanel" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<dx:ASPxPanel ID="LeftPanel" runat="server" Collapsible="True" FixedPosition="WindowLeft" Width="230px" ClientInstanceName="LeftPanel">
    <SettingsAdaptivity CollapseAtWindowInnerWidth="900" />
    <PanelCollection>
        <dx:PanelContent runat="server">
            <dx:ASPxNavBar ID="NavBarManu" runat="server" EnableTheming="True" Theme="Office2010Black" Font-Size="Small" Width="100%" AllowSelectItem="True" AutoCollapse="True">
                <Groups>
                    <dx:NavBarGroup Text="開發練習範例" Expanded="False" Name="T0000">
                        <Items>
                            <dx:NavBarItem Text="專案設定說明" NavigateUrl="~/Admin/Default.aspx" Name="Default">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="空白範例" NavigateUrl="~/Test/TW001.aspx" Name="TW001" ToolTip="空白範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="查詢功能A" NavigateUrl="~/Test/TW002.aspx" Name="TW002" ToolTip="僅查詢功能，繼承自 System.Web.UI.Page">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="查詢功能B" NavigateUrl="~/Test/TW003.aspx" Name="TW003" ToolTip="僅查詢功能，繼承自 DevSFEditGrid">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="編輯功能" NavigateUrl="~/Test/TW004.aspx" Name="TW004" ToolTip="包含增刪修，繼承自 DevSFEditGrid">
                            </dx:NavBarItem>
                        </Items>
                        <ItemStyle>
                            <Paddings PaddingLeft="20px" PaddingRight="0px" />
                        </ItemStyle>
                    </dx:NavBarGroup>
                    <dx:NavBarGroup Text="單檔案例展示" Expanded="False" Name="TA000">
                        <Items>
                            <dx:NavBarItem Text="機器設定" NavigateUrl="~/Admin/Machine.aspx" Name="TA001" ToolTip="單檔-網格-編輯 DevSFEditGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="客戶資料維護" NavigateUrl="~/Admin/CustAC.aspx" Name="TA002" ToolTip="單檔-網格-調閱編輯 DevSFEditTPagGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="組織設定" NavigateUrl="~/Admin/OrganMain.aspx" Name="TA003" ToolTip="單檔-樹狀-編輯 DevSFEditTree 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="機台生產現況" NavigateUrl="~/Query/WoRoutMachNowList.aspx" Name="TA004" ToolTip="單檔-網格-查詢 DevSFQuryGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="閒置製令清單" NavigateUrl="~/Query/WoODueList.aspx" Name="TA005" ToolTip="單檔-網格-查詢 DevSFQuryGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="DevSFQuryTPagGrid(無)" NavigateUrl="~/Admin/Default.aspx" Name="TA006" ToolTip="單檔-網格-查詢 DevSFQuryTPagGrid 範例">
                            </dx:NavBarItem>
                        </Items>
                        <ItemStyle>
                            <Paddings PaddingLeft="20px" PaddingRight="0px" />
                        </ItemStyle>
                    </dx:NavBarGroup>
                    <dx:NavBarGroup Text="雙檔案例展示" Expanded="False" Name="TB000">
                        <Items>
                            <dx:NavBarItem Text="產品製途設定" NavigateUrl="~/Admin/MaRout.aspx" Name="TB001" ToolTip="雙檔-網格-編輯(主檔畫面) DevDFEditGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="製程機器設定" NavigateUrl="~/Admin/ManuMach.aspx" Name="TB002" ToolTip="雙檔-網格-編輯(主檔畫面) DevDFEditGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="DevDFEditSpltGrid(無)" NavigateUrl="~/Admin/Default.aspx" Name="TB003" ToolTip="雙檔-網格-編輯(分割畫面) DevDFEditSpltGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="DevDFEditTPagGrid(無)" NavigateUrl="~/Admin/Default.aspx" Name="TB004" ToolTip="雙檔-網格-調閱編輯 DevDFEditTPagGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="DevDFEditSTreeGrid(無)" NavigateUrl="~/Admin/Default.aspx" Name="TB005" ToolTip="雙檔-主檔網格副檔樹狀-編輯 DevDFEditSTreeGrid 範例">
                            </dx:NavBarItem>
                        </Items>
                        <ItemStyle>
                            <Paddings PaddingLeft="20px" PaddingRight="0px" />
                        </ItemStyle>
                    </dx:NavBarGroup>
                    <dx:NavBarGroup Text="一對多案例展示" Expanded="False" Name="TC000">
                        <Items>
                            <dx:NavBarItem Text="DevMFEditDTPagGrid(無)" NavigateUrl="~/Admin/Default.aspx" Name="TC001" ToolTip="一對多檔-網格-編輯(主檔畫面) DevMFEditDTPagGrid 範例">
                            </dx:NavBarItem>
                        </Items>
                        <ItemStyle>
                            <Paddings PaddingLeft="20px" PaddingRight="0px" />
                        </ItemStyle>
                    </dx:NavBarGroup>
                     <%--單檔開發 star--%> 
                    <dx:NavBarGroup Text="單檔-網格-編輯:DevSFEditGrid" Expanded="False" Name="TD000">
                        <Items>
                            <dx:NavBarItem Text="機器設定" NavigateUrl="~/Admin/Machine2.aspx" Name="TD001" ToolTip="單檔-網格-編輯 DevSFEditGrid 範例">
                            </dx:NavBarItem>
                              <dx:NavBarItem Text="客戶資料維護" NavigateUrl="~/Admin/CustAC2.aspx" Name="TD002" ToolTip="單檔-網格-編輯 DevSFEditGrid 範例">
                            </dx:NavBarItem>
                            <dx:NavBarItem Text="員工資料維護" NavigateUrl="~/Admin/Emp.aspx" Name="TD003" ToolTip="單檔-網格-調閱編輯 DevDFEditTPagGrid">
                            </dx:NavBarItem>
                        </Items>
                        <ItemStyle>
                            <Paddings PaddingLeft="20px" PaddingRight="0px" />
                        </ItemStyle>
                    </dx:NavBarGroup>
                    <%--單檔開發 end--%>
                    <dx:NavBarGroup Expanded="False" Name="L0000" NavigateUrl="~\Admin\Default.aspx" Text="尚未登入">
                    </dx:NavBarGroup>

                </Groups>
            </dx:ASPxNavBar>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxPanel>
