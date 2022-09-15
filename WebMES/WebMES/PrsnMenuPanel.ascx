<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrsnMenuPanel.ascx.cs" Inherits="WebMES.PrsnMenuPanel" %>

<%@ Register Assembly="DevExpress.Web.v22.1, Version=22.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<dx:ASPxPanel ID="LeftPanel" runat="server" Collapsible="True" FixedPosition="WindowLeft" Width="150px" ClientInstanceName="LeftPanel">
    <SettingsAdaptivity CollapseAtWindowInnerWidth="900" />
    <PanelCollection>
        <dx:PanelContent runat="server">
            <dx:ASPxNavBar ID="NavBarManu" runat="server" EnableTheming="True" Theme="Office2010Black" Font-Size="Small" Width="100%" AllowSelectItem="True" AutoCollapse="True">
                <Groups>
                    <dx:NavBarGroup Text="我的工作" Expanded="False" Name="KR000">
                        <Items>
                            <dx:NavBarItem Text="首頁" NavigateUrl="Default.aspx" Name="KRA00">
                            </dx:NavBarItem>
                        </Items>
                        <ItemStyle>
                            <Paddings PaddingLeft="20px" PaddingRight="0px" />
                        </ItemStyle>
                    </dx:NavBarGroup>
                    <dx:NavBarGroup Expanded="False" Name="L0000" NavigateUrl="Deafult.aspx" Text="尚未登入">
                    </dx:NavBarGroup>
                </Groups>
            </dx:ASPxNavBar>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxPanel>
