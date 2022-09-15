using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebMES
{
    public partial class PrsnMenuPanel : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //檢查登入
            if (DBUtility.Logoned(false))
            {
                //設定Left menu 的登入Link, 顯示已登入人名
                NavBarManu.Groups.FindByName("L0000").Text = Session["CurUserNM"].ToString() + " - 登出";
                //NavBarStrategic.Items.FindByName("LA000").Text = Session["CurUserNM"].ToString() + " - 登出";
                //(NavBarStrategic.FindControl("L0000") as DevExpress.Web.NavBarItem).Text = Session["CurUserNM"].ToString() + " - 登出";
            }
            else
            {
                //設定Left menu 的登入Link, 顯示尚未登入 
                NavBarManu.Groups.FindByName("L0000").Text = "尚未登入";
                //NavBarStrategic.Items.FindByName("LA000").Text = "尚未登入";
            }
            //根據權限動態設定該功能顯示否 
            for (int i = 0; i < NavBarManu.Items.Count; i++)
            {
                if ((NavBarManu.Items[i] as DevExpress.Web.NavBarItem).Visible)
                    (NavBarManu.Items[i] as DevExpress.Web.NavBarItem).Visible = DBUtility.IsOwnChild((NavBarManu.Items[i] as DevExpress.Web.NavBarItem).Name)
                                                                               || DBUtility.IsOwnRight((NavBarManu.Items[i] as DevExpress.Web.NavBarItem).Name, false);
            }
        }
    }
}