using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMES.Admin
{
    public partial class MainMenuPanel : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            cbxGlobalLang.SelectedIndex = (Session["GlobalLangSel"] == null ? 0 : (int)Session["GlobalLangSel"]);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //根據當地語言設定TreeList的Caption
            string CurLangCaption;
            foreach (DevExpress.Web.MenuItem m in MainMenu.Items)
            {
                CurLangCaption = DBUtility.ObtainResCaption("MainMenuPanel", m.Name);
                if (CurLangCaption != "?")
                    m.Text = CurLangCaption;
            }
            //由商業模式決定貨品異動類別(貨品異動作業)
            if (Application["BIZMODEL"].ToString() == "1")
            {
                //1-製造業
                /*
                if (DBUtility.IsOwnRight("BC000", false))
                    //進貨系統
                    MainMenu.Items.FindByName("A0000").NavigateUrl = @"~\Admin\VRecAA.aspx";
                else
                    //進貨系統空白頁面
                    MainMenu.Items.FindByName("A0000").NavigateUrl = "VPurMenuDefault.aspx";
                if (DBUtility.IsOwnRight("BC000", false))
                    //銷貨系統
                    MainMenu.Items.FindByName("B0000").NavigateUrl = "CSalAA.aspx";
                else
                    //銷貨系統空白頁面
                    MainMenu.Items.FindByName("B0000").NavigateUrl = "CSalMenuDefault.aspx";

                if (DBUtility.IsOwnRight("CF000", false))
                    //資材系統
                    MainMenu.Items.FindByName("C0000").NavigateUrl = "GodsAA.aspx";
                else
                    //資材系統空白頁面
                    MainMenu.Items.FindByName("C0000").NavigateUrl = "InvtMenuDefault.aspx";
                */
            }
            else if (Application["BIZMODEL"].ToString() == "2")
            {
                //2-流通業
            }
            else if (Application["BIZMODEL"].ToString() == "3")
            {
                //3-技服業
            }

            //根據權限代碼查詢是否擁有該功能的子功能並進行動態設定其顯示否
            foreach (DevExpress.Web.MenuItem m in MainMenu.Items)
            {
                //if ((m.Visible) && (m.Name != "MY000") && (m.Name != "J0000"))
                if ((m.Visible) && (m.Name != "MY000"))
                    //MY000-首頁, N0000-後台管理 預設為可見
                    m.Visible = DBUtility.IsOwnChild(m.Name);
            }
            if ((bool)Application["ONLYMES"])
            {
                //設定某個系統不可視
                //MainMenu.Items.FindByName("A0000").Visible = false;  //進貨系統             
            }
            //設定某個系統是連結到外部其他系統
            // MainMenu.Items.FindByName("J0000").NavigateUrl = "http://XXX.XXX.XXX.XXX/Login.aspx";
        }

        //語言切換
        protected void cbxGlobalLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxGlobalLang.SelectedIndex == 0)
            {
                Session["GlobalLangSel"] = 0;
                System.Globalization.CultureInfo currentInfo = new System.Globalization.CultureInfo("zh-TW");
                //System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                //System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
            }
            else if (cbxGlobalLang.SelectedIndex == 1)
            {
                Session["GlobalLangSel"] = 1;
                System.Globalization.CultureInfo currentInfo = new System.Globalization.CultureInfo("en-US");
                //System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                //System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
            }
            else if (cbxGlobalLang.SelectedIndex == 2)
            {
                Session["GlobalLangSel"] = 2;
                System.Globalization.CultureInfo currentInfo = new System.Globalization.CultureInfo("zh-CN");
                //System.Threading.Thread.CurrentThread.CurrentCulture = currentInfo;
                //System.Threading.Thread.CurrentThread.CurrentUICulture = currentInfo;
            }
        }

    }
}