<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebMES.Default" %>

<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/PrsnMenuPanel.ascx" TagPrefix="uc1" TagName="PrsnMenuPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0" />
    <title>首頁</title>
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
</head>
<body>
    <form id="form1" runat="server">
        <uc1:MainMenuPanel runat="server" ID="MainMenuPanel" />
        <uc1:PrsnMenuPanel runat="server" ID="PrsnMenuPanel" />
        <div style="text-align:center;">
            <p>&nbsp;</p>
            <p>請點選右上方「開發範例」開始</p>
        </div>
    </form>
</body>
</html>
