using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WebMES
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            // 在應用程式啟動時執行的程式碼
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-TW");
            System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";            

            //製造業
            Application["BIZMODEL"] = "1";
            //技服業
            //Application["BIZMODEL"] = "3";

            //預設
            Application["ONLYMES"] = false;
            //ONLYMES = false --> 主選單顯示進銷存Menu 
            //ONLYMES = true --> 主選單不顯示進銷存Menu(庫存僅顯示入庫功能)

            //網站使用
            Application["WebSite-APPDIR"] = @"C:\WebMES";
            Application["WebSite-APPURLBASE"] = "http://localhost/WebMES";
            //部門員工照片
            Application["CPNYID"] = "28307001";
            Application["ORGANPHOTODIR"] = "/OrgnPhoto/";

            /*
            //Application["APPURLBASE"] = "http://localhost/WebERP";
            //Application["APPDIR"] = @"D:\WebERP\WebERP";

            //
            Application["WebHrs-JOBSTEPSFILEDIR"] = "/JobSteps/";
            Application["WebHrs-CLSFILEDIR"] = "/Course/";
            Application["WebHrs-DPT2CLSMDNO"] = "3";
            //1-職位→職掌→工作→職能→課程
            //2-職位→職掌→工作→課程
            //3-職位→職掌→職能→課程
            //4-職位→職掌→課程
            //5-職位→職能→課程
            //6-職位→課程
            Application["WebHrs-CLSNONULLVALUE"] = "zzzzz";
            Application["WebHrs-JOBNOMAXLEN"] = 5;            

            //避免ASP.NET 4.5 網頁加入驗證控制項後運作時有下列錯誤訊息
            //WebForms UnobtrusiveValidationMode 需要 'jquery' 的 ScriptResourceMapping。請加入 ScriptResourceMapping 命名的 jquery (區分大小寫)。 
            //ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.WebForms;
            //
            Application["SMTPHOST"] = "smtp.pchome.com.tw";
            

            //** MES系統 ERP資料庫同步參數
            Application["ToDetectSyncFile"] = "0";
            // "0" 停止偵測同步ERP資料庫
            // "1" 啟動偵測同步ERP資料庫
            Application["ToLogSyncFile"] = "1";
            // "1" 啟動同步ERP資料庫過程紀錄功能
            // "0" 停止同步ERP資料庫過程紀錄功能
            Application["SyncToQueueSpan"] = 3600000;
            //设置间隔时间为1000毫秒=1秒, 3600000=1hr
            Application["QueueToSaveSpan"] = 3601000;
            //设置间隔时间为1000毫秒=1秒, 3600000=1hr

            //**測試使用
            //Application["ToDetectSyncFile"] = "1";

            //是否檢查非合法IP, 無法登入系統 
            //Application["VALIDIP"] = true;
            //VALIDIP = true --> 啟動檢查, 搭配AllowIP清單只有在清單內的才允許登入系統
            //**測試使用
            //Application["VALIDIP"] = false;

            */

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // 在新的工作階段啟動時執行的程式碼
            //原始設定

            //權限
            Session["UserRight"] = "";
            Session["OrgUserRight"] = "";

            //使用者資訊
            Session["CurUserID"] = null;
            Session["CurUserNM"] = "";
            Session["CurDeptID"] = null;
            Session["CurDeptNM"] = "";
            Session["EMAILBOX"] = "";
            Session["CurSTORID"] = "";
            Session["ActDeptID"] = null;
            Session["ActDeptBID"] = "";

            //預設HomeForm
            Session["HomeForm"] = "Default.aspx";

            //**測試使用
            //Session["UserRight"] = "SFA00,SFB00,CFB00,CFD00,AGB00,AGD00,SG000,BGA00,BGB00,BGD00,BCB00,BCD00,BDG00,BF000,PB000";
            Session["UserRight"] = "00000";
            Session["CurUserID"] = "A0001";
            Session["CurUserNM"] = "測試者";
            Session["CurDeptID"] = "0AD00";
            Session["CurDeptNM"] = "測試單位";
            Session["EMAILBOX"] = "";
            Session["CurSTORID"] = "0AD00";
            Session["ActSTORID"] = "0AD00";
            Session["ActDeptID"] = "0AD00";
            Session["ActDeptBID"] = "A00";

            //Default Page紀錄 ListView的目前頁面
            Session["BListViewCurPage"] = 0;

            /*
            Session["ISMULSHOP"] = true;
            //true -->多家分店
            //false -->單店

            //** 資材系統使用
            //多倉制
            Session["ISINVMULSTOR"] = true;
            //ISINVMULSTOR = true 多倉制
            //ISINVMULSTOR = false 單倉制

            Session["ActINVYYMM"] = "202009";
            //目前工作年月
            Session["INVCANMINUS"] = false;
            //INVCANMINUS = true 庫存允許負值
            //INVCANMINUS = false 庫存不允許負值
            Session["ISINVBTCH"] = false;
            //ISINVBTCH = true 管制批號庫存
            Session["INVCOSTMD"] = "2";
            //資材成本計算採用 :
            //  標準成本 : 1
            //  平均成本 : 2

            //** 庫存異動系統使用
            Session["ISSHFTSTOR"] = true;
            //ISSHFTSTOR = true 異動頁面顯示倉庫別
            //ISSHFTSTOR = false 異動頁面不顯示倉庫別
            Session["ISSHFTLOCA"] = true;
            //ISSHFTLOCA = true 異動頁面顯示儲位
            //ISSHFTLOCA = false 異動頁面不顯示儲位

            //** 進貨系統使用
            Session["RECAUTOPAS"] = true;
            //RECAUTOPAS = true 自動驗收
            //RECAUTOPAS = false 收貨後驗收(兩段式)
            Session["VENDCHKVALID"] = false;
            // = true 連結廠商報價(Ma->Vend), (需用日期一年以內)
            // = false 不連結廠商報價(Ma->Ven), (需用日期一年以內)

            //** 生管系統使用
            //批量部分完成能否移轉
            Session["CanPartShftNProc"] = true;
            //CanPartShftNProc = true  批量部分完成即可移轉下一製程
            //CanPartShftNProc = false 批量全部完成才可移轉下一製程
            //Session["CanPartShftNProc"] = false;
            //** MES使用
            //現場只有一台電腦同時支援多張工單掃描&投料時, 可設定Session["RunCardLogin"] = false
            Session["RunCardLogin"] = false;
            //RunCardLogin = true  先登入才能使用MES系統進行工單掃描&投料
            //RunCardLogin = false 免先登入即可進入MES的饋控畫面進行工單掃描&投料
            //根據投入量預先計算完成數量的預設值
            Session["CalcMFnshDefault"] = true;
            //CalcMFnshDefault = true  預先計算完成數量的預設值
            //CalcMFnshDefault = false 完成數量的預設值=0, 由作業人員輸入

            //** 差勤系統使用
            //啟用外出功能
            Session["DUTYOUTENABLED"] = true;
            //啟用加班功能
            Session["DUTYADDENABLED"] = true;
            //
            Session["SetPassCodeForm"] = "MyPassSet.aspx";
            Session["PublBlogHomeForm"] = "MotifList.aspx";
            Session["CusHomeForm"] = "MyProFile.aspx";
            Session["PrsnBlogHomeForm"] = "PrsnMotifList.aspx";
            Session["CallingForm"] = "";
            //
            Session["LoginSender"] = 0;
            //
            Session["SurvyServicePath"] = "http://localhost/WebSisCRM/WebSisCRM.dll";
            //localhost測試用
            //Session["SurvyServicePath"] = "http://127.0.0.1/WebHair/WebSisCRM.dll";
\
            //首頁看板預設不顯示
            Session["IsViewPanels"] = false;
            */

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}