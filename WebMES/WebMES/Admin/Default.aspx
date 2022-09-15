<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebMES.Admin.CDefault" %>

<%@ Register Src="~/Admin/MainMenuPanel.ascx" TagPrefix="uc1" TagName="MainMenuPanel" %>
<%@ Register Src="~/Admin/ManuMenuPanel.ascx" TagPrefix="uc1" TagName="ManuMenuPanel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0" />
    <title>範例首頁</title>
        <style>
        body, form {
            padding: 0;
            margin: 0;
            overflow: auto;
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
        <uc1:ManuMenuPanel runat="server" ID="ManuMenuPanel" />
        <div style="overflow: visible;">
            1.Web.config：<br />
                        　(1)資料庫連結要改成自己的SQL SSERVER名稱<br />
                        　(2)如果出現ASPxHttpHandlerModule錯誤就在Web.config把DevExpress.Web.ASPxHttpHandlerModule加上<br />
                        　(3)要確定Web.config中元件版本與實際版本一致，否則會出現版本問題<br />
            <br />
            2.App_Code：<br />
                        　(1)DBUtility.cs 是存取資料庫用的<br />
                        　(2)Query_Publ.cs 是設定報表SQL用的<br />
                        　(3)Lead_Publ.cs 是製程機器(ManuMach)用的<br />
                        　(4)預設建置動作為「內容」，如果有放檔案但仍報錯，試著把.cs檔的建置動作改為「編譯」<br />
            <br />
            3.Global.asax：<br />
                        　(1)新建置專案如果選空白會沒有這個檔案，要自己加入<br />
            <br />
            4.參考：<br />
                        　(1)新建置專案後，如果要建立基本有上方和左方選單的功能，需要加入基本的參考(下列)才能運作，其他參考項目則是是開發情況若有逾缺就加<br />
                        　(2)自訂元件 (加入參考→瀏覽→選擇檔案)<br />
                        　　　→ComnUtility.dll<br />
                        　　　→CPC.Utility.dll<br />
                        　　　→CPC.Web.UI.dll<br />
                        　　　→FormUtility.dll<br />
                        　(3)DevExpress元件 (加入參考→組件→搜尋DevExpress)<br />
                        　　　→DevExpress.Web<br />
                        　　　→DevExpress.data<br />
                        　　　→DevExpress.Printing.Core<br />
                        　　　→DevExpress.Web.ASPxTreeList<br />
                        　　　→DevExpress.Web.ASPxScheduler<br />
            <br />
            5.修改/增加選單：<br />
                        　(1)非正式設計，方便建立連結展示使用<br />
                        　(2)PrsnMenuPanel.ascx：使用者登入頁面的左邊選單<br />
                        　(3)MainMenuPanel.ascx：上方模組選單<br />
                        　(4)ManuMenuPanel.ascx：開發範例模組的左邊選單<br />
            <br />
            6.範例用資料表：<br />
            <table border="1">
                <tr>
                    <td>Table名稱</td>
                    <td>Table內容</td>
                </tr>
                <tr>
                    <td>Cus</td>
                    <td>客戶資料</td>
                </tr>
                <tr>
                    <td>Cuskd</td>
                    <td>客戶類別</td>
                </tr>
                <tr>
                    <td>Dist</td>
                    <td>地區</td>
                </tr>
                <tr>
                    <td>Dpt</td>
                    <td>部門</td>
                </tr>
                <tr>
                    <td>Fact</td>
                    <td>廠</td>
                </tr>
                <tr>
                    <td>Ma</td>
                    <td>物品</td>
                </tr>
                <tr>
                    <td>Macd</td>
                    <td>物品種類</td>
                </tr>
                <tr>
                    <td>Machine</td>
                    <td>機器</td>
                </tr>
                <tr>
                    <td>Makd</td>
                    <td>物品類別</td>
                </tr>
                <tr>
                    <td>ManuLine</td>
                    <td>產線</td>
                </tr>
                <tr>
                    <td>ManuMach</td>
                    <td>製程機器</td>
                </tr>
                <tr>
                    <td>ManuProc</td>
                    <td>製程</td>
                </tr>
                <tr>
                    <td>MaRout</td>
                    <td>產品製途</td>
                </tr>
                <tr>
                    <td>MaRoutProc</td>
                    <td>產品製途工序</td>
                </tr>
                <tr>
                    <td>Prsn</td>
                    <td>人員</td>
                </tr>
                <tr>
                    <td>Unit</td>
                    <td>計量單位</td>
                </tr>
                <tr>
                    <td>Wo</td>
                    <td>製令</td>
                </tr>
                <tr>
                    <td>WoRout</td>
                    <td>製令工序</td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                </tr>
            </table>
            <br />
        </div>

    </form>
</body>
</html>
