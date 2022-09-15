<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainMenuPanel.ascx.cs" Inherits="WebMES.Admin.MainMenuPanel" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<dx:ASPxPanel ID="TopPanel" runat="server" ClientInstanceName="topPanel" Collapsible="True" EnableTheming="True" FixedPosition="WindowTop" Width="100%">
    <SettingsAdaptivity CollapseAtWindowInnerWidth="580" />
    <SettingsCollapsing ExpandEffect="Slide">
    </SettingsCollapsing>
    <PanelCollection>
        <dx:PanelContent runat="server">
            <dx:ASPxComboBox ID="cbxGlobalLang" runat="server" Style="float: left;width:11%;min-width: 100px;margin:4px 0 4px" Font-Size="small" ClientInstanceName="cbxGlobalLang" AutoPostBack="True" SelectedIndex="0" OnSelectedIndexChanged="cbxGlobalLang_SelectedIndexChanged" Theme="Glass" ToolTip="選擇網頁語系">
                <Items>
                    <dx:ListEditItem Text="繁體中文" Value="0" Selected="True" />
                    <dx:ListEditItem Text="English" Value="1" />
                    <dx:ListEditItem Text="简体中文" Value="2" />
                </Items>
            </dx:ASPxComboBox>
            <dx:ASPxMenu ID="MainMenu" runat="server" ClientInstanceName="MainMenu" Theme="Glass" CssClass="main-menu" Font-Size="small">
                <ClientSideEvents ItemClick="function(s, e) {
}" />
                <Items>
                    <dx:MenuItem Text="首頁" Name="MY000" NavigateUrl="~/Default.aspx" ToolTip="我的工作">
                    </dx:MenuItem>
                    <dx:MenuItem Text="開發範例" Name="T0000" NavigateUrl="~/Admin/Default.aspx" ToolTip="開發範例">
                    </dx:MenuItem>
                    <dx:MenuItem Text="途程規劃" Name="A0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="備料管理" Name="B0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="製造管理" Name="C0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="文件管理" Name="D0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="人員管理" Name="E0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="品質管理" Name="F0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="推播管理" Name="G0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="設備管理" Name="H0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="績效管理" Name="I0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                    <dx:MenuItem Text="資料同步" Name="J0000" NavigateUrl="~/Default.aspx" Visible="False">
                    </dx:MenuItem>
                </Items>
                <BorderLeft BorderStyle="None" />
                <BorderTop BorderStyle="None" />
                <BorderBottom BorderStyle="None" />
            </dx:ASPxMenu>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxPanel>

