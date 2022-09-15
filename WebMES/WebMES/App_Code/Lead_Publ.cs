using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

/*
using System.Collections.Generic;
using System.Linq;
*/
using System.Web;
// ScriptManager  使用
using System.Web.UI;

public class Lead_Publ
{
    //根據出勤ID計算實際工時(以Second單位儲存在ATTENDDUTYSEC)並更新該出勤ID的ATTENDDUTYSEC;ATTENDDUTYTXT
    //出勤 : 班別
    //差假 : 起訖差勤時間與差勤類別的相關參數(ATTENDDAYSPAN;ATTENDSTTIME;ATTENDEDTIME)計算當天或跨天出勤/差假的時數(秒)
    public static void CalcOnDutySEC(int CurAGENDAID)
    {
        string cmdAttendDuty = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.DUTYSTTM, "
                 + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO, Agenda.AGENDAKDNO, Agendakd.AGENDACGNO "
                 + "FROM Agenda LEFT JOIN Agendakd ON (Agenda.AGENDAKDNO=Agendakd.AGENDAKDNO) "
                 + "WHERE  Agenda.AGENDAID=" + CurAGENDAID;
        DataTable dtbAttendDuty = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendDuty);
        if (dtbAttendDuty.Rows.Count > 0)
        {
            if (dtbAttendDuty.Rows[0]["AGENDACGNO"].ToString() == "1")
            {
                //差假
                double AttendSpanSEC = Lead_Publ.CalcLaborSec((DateTime)dtbAttendDuty.Rows[0]["AGENDASTTIME"], (DateTime)dtbAttendDuty.Rows[0]["AGENDAEDTIME"], dtbAttendDuty.Rows[0]["AGENDAKDNO"].ToString());
                TimeSpan AttendDuty = TimeSpan.FromSeconds(AttendSpanSEC);

                string UpdateCmd = "UPDATE Agenda "
                     + "SET "
                     + "ATTENDDUTYSEC= '" + AttendSpanSEC + "', "
                     + "ATTENDDUTYTXT= '" + AttendDuty + "' "
                     + "WHERE Agenda.AGENDAID=" + dtbAttendDuty.Rows[0]["AGENDAID"];
                DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
            }
            else if (dtbAttendDuty.Rows[0]["AGENDACGNO"].ToString() == "2")
            {
                //出勤
                if ((dtbAttendDuty.Rows.Count > 0) && (dtbAttendDuty.Rows[0]["AGENDAKDNO"].ToString() == "28"))
                {
                    //找出外出對應的出勤班別
                    cmdAttendDuty = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.DUTYSTTM, "
                             + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO, Agenda.AGENDAKDNO "
                             + "FROM Agenda "
                             + "WHERE  Agenda.AGENDASTTIME<='" + ((DateTime)dtbAttendDuty.Rows[0]["AGENDASTTIME"]).ToString("yyyy/MM/dd HH:mm") + "' "
                             + "AND  Agenda.AGENDAEDTIME>='" + ((DateTime)dtbAttendDuty.Rows[0]["AGENDAEDTIME"]).ToString("yyyy/MM/dd HH:mm") + "' "
                             + "AND Agenda.DPTNO='" + dtbAttendDuty.Rows[0]["DPTNO"].ToString() + "' AND Agenda.PRSNNO='" + dtbAttendDuty.Rows[0]["PRSNNO"].ToString() + "' "
                             + "AND  Agenda.AGENDAKDNO<>'28' ";
                    dtbAttendDuty = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendDuty);
                }

                if (dtbAttendDuty.Rows.Count > 0)
                {
                    if ((dtbAttendDuty.Rows[0]["AGENDASTTIME"].ToString() != "") && (dtbAttendDuty.Rows[0]["AGENDAEDTIME"].ToString() != ""))
                    {
                        string OutWhereDateStr = " AND (AGENDASTTIME>='" + ((DateTime)dtbAttendDuty.Rows[0]["AGENDASTTIME"]).ToString("yyyy/MM/dd HH:mm") + "' AND AGENDAEDTIME<='" + ((DateTime)dtbAttendDuty.Rows[0]["AGENDAEDTIME"]).ToString("yyyy/MM/dd HH:mm") + "') ";
                        string cmdAttendOut = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDDUTYSEC "
                             + "FROM Agenda "
                             + "WHERE  (Agenda.AGENDAKDNO='28' AND Agenda.PRSNNO='" + dtbAttendDuty.Rows[0]["PRSNNO"].ToString() + "' AND  Agenda.DPTNO='" + dtbAttendDuty.Rows[0]["DPTNO"].ToString() + "') "
                             + OutWhereDateStr;
                        DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendOut);
                        //計算工時
                        TimeSpan AttendSpan = ((DateTime)dtbAttendDuty.Rows[0]["AGENDAEDTIME"]) - ((DateTime)dtbAttendDuty.Rows[0]["AGENDASTTIME"]);
                        double AttendSpanSEC = AttendSpan.TotalSeconds;
                        //處理多筆外出
                        foreach (DataRow AttendRow in dtbAttendOut.Rows)
                        {
                            //計算工時, 實際工時需減去外出時數
                            if (AttendRow["ATTENDDUTYSEC"].ToString() != "")
                                AttendSpanSEC -= Convert.ToDouble(AttendRow["ATTENDDUTYSEC"]);
                        }
                        TimeSpan AttendDuty = TimeSpan.FromSeconds(AttendSpanSEC);

                        string UpdateCmd = "UPDATE Agenda "
                             + "SET "
                             + "ATTENDDUTYSEC= '" + AttendSpanSEC + "', "
                             + "ATTENDDUTYTXT= '" + AttendDuty + "' "
                             + "WHERE Agenda.AGENDAID=" + dtbAttendDuty.Rows[0]["AGENDAID"];
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                    }
                }
            }
        }
    }

    //計算出勤ID的實際工時(以Time單位儲存在ATTENDDS), 
    public static void CalcAttendTS(int CurAGENDAID)
    {
        string cmdAttendDuty = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.DUTYSTTM, "
                 + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO, Agenda.AGENDAKDNO "
                 + "FROM Agenda "
                 + "WHERE  Agenda.AGENDAID=" + CurAGENDAID;
        DataTable dtbAttendDuty = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendDuty);
        if (dtbAttendDuty.Rows.Count > 0)
        {
            if ((dtbAttendDuty.Rows[0]["AGENDASTTIME"].ToString() != "") && (dtbAttendDuty.Rows[0]["AGENDAEDTIME"].ToString() != ""))
            {
                string OutWhereDateStr = " AND (AGENDASTTIME>='" + ((DateTime)dtbAttendDuty.Rows[0]["AGENDASTTIME"]).ToString("yyyy/MM/dd HH:mm") + "' AND AGENDAEDTIME<='" + ((DateTime)dtbAttendDuty.Rows[0]["AGENDAEDTIME"]).ToString("yyyy/MM/dd HH:mm") + "') ";
                string cmdAttendOut = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDTS "
                     + "FROM Agenda "
                     + "WHERE  (Agenda.AGENDAKDNO='28' AND Agenda.PRSNNO='" + dtbAttendDuty.Rows[0]["PRSNNO"].ToString() + "' AND  Agenda.DPTNO='" + dtbAttendDuty.Rows[0]["DPTNO"].ToString() + "') "
                     + OutWhereDateStr;
                DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendOut);
                //計算工時
                TimeSpan AttendTimeSpan = ((DateTime)dtbAttendDuty.Rows[0]["AGENDAEDTIME"]) - ((DateTime)dtbAttendDuty.Rows[0]["AGENDASTTIME"]);
                //處理多筆外出
                foreach (DataRow AttendRow in dtbAttendOut.Rows)
                {
                    //計算工時, 實際工時需減去外出時數
                    if (AttendRow["ATTENDTS"] != null)
                        AttendTimeSpan -= (TimeSpan)AttendRow["ATTENDTS"];
                }
                string UpdateCmd = "UPDATE Agenda "
                     + "SET "
                     + "ATTENDTS= '" + AttendTimeSpan + "' "
                     + "WHERE Agenda.AGENDAID=" + dtbAttendDuty.Rows[0]["AGENDAID"];
                DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
            }
        }
    }

    //根據起訖差勤時間與差勤類別的相關參數(ATTENDDAYSPAN;ATTENDSTTIME;ATTENDEDTIME)計算當天或跨天出勤/差假的時數(秒)
    public static double CalcLaborSec(DateTime CurAGENDASTTIME, DateTime CurAGENDAEDTIME, string CurAGENDAKDNO)
    {
        DateTime CurLABORSTTIME = CurAGENDASTTIME;
        DateTime CurLABOREDTIME = CurAGENDAEDTIME;
        //DateTime CurLABORSTTIME = new DateTime();
        //DateTime CurLABOREDTIME = new DateTime();
        string cmdAgendakd = "SELECT Agendakd.AGENDAKDNO, Agendakd.AGENDAKDNM, Agendakd.ATTENDSTTIME, Agendakd.ATTENDEDTIME, Agendakd.ATTENDDAYSPAN "
                           + "FROM  Agendakd "
                           + "WHERE  Agendakd.AGENDAKDNO='" + CurAGENDAKDNO + "' ";
        DataTable dtbAgendakd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAgendakd);
        if (dtbAgendakd.Rows[0]["ATTENDSTTIME"] != DBNull.Value)
        {
            bool kk = (DateTime.TryParse(CurAGENDASTTIME.Date.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["ATTENDSTTIME"].ToString(), out CurLABORSTTIME));
        }
        if (dtbAgendakd.Rows[0]["ATTENDEDTIME"] != DBNull.Value)
        {
            //CurAGENDASTTIME = 2016/12/01 00:00:00, CurAGENDAEDTIME = 2016/12/02 00:00:00 => 2016/12/01 全天 從00:00:00~24:00:00
            //需使用 CurAGENDASTTIME.AddDays(1) < CurAGENDAEDTIME來判斷, 勿使用CurAGENDASTTIME.Date判斷
            bool kk;
            if (CurAGENDASTTIME.AddDays(1) < CurAGENDAEDTIME)
            {
                //跨天
                if (CurAGENDAEDTIME.TimeOfDay.ToString() == "00:00:00")
                    //if (CurAGENDAEDTIME.Date.ToString("HH:mm:ss") == "00:00:00")
                    kk = (DateTime.TryParse(CurAGENDAEDTIME.AddDays(-1).ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["ATTENDEDTIME"].ToString(), out CurLABOREDTIME));
                else
                    kk = (DateTime.TryParse(CurAGENDAEDTIME.Date.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["ATTENDEDTIME"].ToString(), out CurLABOREDTIME));

                //kk = (DateTime.TryParse((CurAGENDAEDTIME.Date.ToString("HH:mm:ss") == "00:00:00"? CurAGENDAEDTIME.AddDays(-1).ToString("yyyy/MM/dd") : CurAGENDAEDTIME.Date.ToString("yyyy/MM/dd")) + " " + dtbAgendakd.Rows[0]["ATTENDEDTIME"].ToString(), out CurLABOREDTIME));
            }
            else
                //當天
                kk = (DateTime.TryParse(CurAGENDASTTIME.Date.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["ATTENDEDTIME"].ToString(), out CurLABOREDTIME));
        }
        CurLABORSTTIME = CurAGENDASTTIME < CurLABORSTTIME ? CurLABORSTTIME : CurAGENDASTTIME;
        CurLABOREDTIME = CurAGENDAEDTIME > CurLABOREDTIME ? CurLABOREDTIME : CurAGENDAEDTIME;
        double LABORDAYSPANSEC = 0;
        double ATTENDDAYSPANSEC = 0;
        if (dtbAgendakd.Rows[0]["ATTENDDAYSPAN"] != DBNull.Value)
        {
            LABORDAYSPANSEC = ((TimeSpan)dtbAgendakd.Rows[0]["ATTENDDAYSPAN"]).TotalSeconds;
            ATTENDDAYSPANSEC = ((TimeSpan)dtbAgendakd.Rows[0]["ATTENDEDTIME"] - (TimeSpan)dtbAgendakd.Rows[0]["ATTENDSTTIME"]).TotalSeconds;
        }
        else
        {
            LABORDAYSPANSEC = ((TimeSpan)(CurAGENDAEDTIME - CurAGENDASTTIME)).TotalSeconds;
            ATTENDDAYSPANSEC = LABORDAYSPANSEC;
        }
        if ((CurLABOREDTIME - CurLABORSTTIME) <= TimeSpan.FromHours(24))
        {
            //一天以內
            double CurLABORSSPANSEC = ((TimeSpan)(CurLABOREDTIME - CurLABORSTTIME)).TotalSeconds * LABORDAYSPANSEC / ATTENDDAYSPANSEC;
            return CurLABORSSPANSEC;
        }
        else
        {
            //超過一天
            //CurLABORSTTIME~起始天LABORSTTIME + 天數*LABORDAYSPANSEC + 結束天STTIME~CurLABOREDTIME
            DateTime CurATTENDEDTIME = new DateTime();
            //起始天
            bool kk = (DateTime.TryParse(CurLABORSTTIME.Date.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["ATTENDEDTIME"].ToString(), out CurATTENDEDTIME));
            double BegLABORDAYSPANSEC = ((TimeSpan)(CurATTENDEDTIME - CurLABORSTTIME)).TotalSeconds * LABORDAYSPANSEC / ATTENDDAYSPANSEC;
            // 中間天
            double MidDay = 0;
            double MidLABORDAYSPANSEC = 0;
            if ((CurLABOREDTIME - CurLABORSTTIME) > TimeSpan.FromHours(48))
            {
                MidDay = CurLABOREDTIME.Day - CurLABORSTTIME.Day - 1;
                MidLABORDAYSPANSEC = MidDay * LABORDAYSPANSEC;
            }
            // 結束天
            DateTime CurATTENDSTTIME = new DateTime();
            kk = (DateTime.TryParse(CurLABOREDTIME.Date.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["ATTENDSTTIME"].ToString(), out CurATTENDSTTIME));
            double EndLABORDAYSPANSEC = ((TimeSpan)(CurLABOREDTIME - CurATTENDSTTIME)).TotalSeconds * LABORDAYSPANSEC / ATTENDDAYSPANSEC;
            return BegLABORDAYSPANSEC + MidLABORDAYSPANSEC + EndLABORDAYSPANSEC;
        }
    }

    //根據打卡類別 + 打卡時間找到該員工預排班行程
    public static DataTable RefechAgendaByPunchkd(DateTime CurPUNCHTIME, string CurPUNCHKDNO, string CurPRSNNO)
    {
        //上班 : CurPUNCHKDNO=='1'
        //下列條件式適用於三班制
        //下列 WhereDateStr 是假設當天該員工排班行程只有一個上班別(或打卡當天排班行程可能有上班別 + 加班)-->AGENDAKDNO是唯一
        //DUTYSTTM <= 打卡日期 + 12 hr
        string WhereDateStr = "";
        if (CurPUNCHKDNO == "1")
            //上班
            WhereDateStr = " AND (DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYSTTM<='" + CurPUNCHTIME.AddHours(12).ToString("yyyy/MM/dd HH:mm") + "') ";
        /*
         * 
         *正常班(08:00~16:00)  打卡時間 2017/04/15 07:50
         *   2017/04/15 16:00 >= 2017/04/15 07:50  AND 2017/04/15 08:00 <= 2017/04/15  19:50
         *小夜班(16:00~24:00)  打卡時間 2017/04/15 15:50
         *   2017/04/16 00:00 >= 2017/04/15 15:50  AND 2017/04/15 16:00 <= 2017/04/16  03:50
         *大夜班(00:00~08:00)  打卡時間 2017/04/14 23:50 
         *   2017/04/15 08:00 >= 2017/04/14 23:50  AND 2017/04/15 00:00 <= 2017/04/15  11:50
         */
        //下班 : CurPUNCHKDNO='2'
        //DUTYEDTM <= 打卡日期 - 12 hr
        else if (CurPUNCHKDNO == "2")
            //下班
            WhereDateStr = " AND (DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYEDTM>='" + CurPUNCHTIME.AddHours(-12).ToString("yyyy/MM/dd HH:mm") + "') ";
        /*
         * 
         *正常班(08:00~16:00)  打卡時間 2017/04/15 16:10
         *   2017/04/15 08:00 <= 2017/04/15 16:10  AND 2017/04/15 16:00 >= 2017/04/15  04:10
         *小夜班(16:00~24:00)  打卡時間 2017/04/16 00:10 
         *   2017/04/15 16:00 <= 2017/04/16 00:10  AND 2017/04/16 00:00 >= 2017/04/15  12:10
         *大夜班(00:00~08:00)  打卡時間 2017/04/15 08:10  
         *   2017/04/15 00:00 <= 2017/04/15 08:10  AND 2017/04/15 08:00 >= 2017/04/14  20:10
         */

        //上班 : CurPUNCHKDNO='1'
        //下列條件式不適用於 大夜班(00:00~08:00), 但適用各種日班
        //DUTYSTTM <= 打卡日期 + 23:59
        //string WhereDateStr = " AND (DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
        /*
         * 
         * 
         *正常班(08:00~16:00) 
         *   2017/04/15 16:00 >= 2017/04/15 07:50  AND 2017/04/15 08:00 <= 2017/04/15  23:59
         *小夜班(16:00~24:00) 
         *   2017/04/16 00:00 >= 2017/04/15 15:50  AND 2017/04/15 16:00 <= 2017/04/15  23:59
         *大夜班(00:00~08:00)  
         *   2017/04/15 08:00 >= 2017/04/14 23:50  AND 2017/04/15 00:00 <= 2017/04/14  23:59
         */

        string cmd = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.SUBJECT, Agenda.NOTES, Agenda.DUTYSTTM, "
             + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO, Agenda.AGENDAKDNO "
             + "FROM Agenda INNER JOIN Agendakd ON (Agenda.AGENDAKDNO=Agendakd.AGENDAKDNO) "
             + "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "' AND Agendakd.PUNCHKDNO='1') "
            //Agendakd.PUNCHKDNO='1' --> 過濾掉外出
            //這裡的 Agendakd.PUNCHKDNO 對應 上下班打卡 - 1, 外出返回 - 3
            //+ "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "' AND Agendakd.PUNCHKDNO='" + CurPUNCHKDNO + "') "
             + WhereDateStr;
        return DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
    }

    //根據員工代碼找尋員工預設班別
    public static DataTable RefechDutykdByPrsn(string CurPUNCHKDNO, string CurPRSNNO)
    {
        string Prsncmd = "SELECT Prsn.PRSNNO, Prsn.PRSNNM, Prsn.DPTNO,  Prsn.TITLEID, Prsn.DUTYKDNO, "
             + "Dutykd.WORKSTTM01, Dutykd.WORKEDTM01, Dutykd.WORKSTTM02, Dutykd.WORKEDTM02, Dutykd.WORKSTTM03, Dutykd.WORKEDTM03, Dutykd.WORKSTTM04, "
             + "Dutykd.WORKEDTM04, Dutykd.WORKSTTM05, Dutykd.WORKEDTM05, Dutykd.WORKSTTM06, Dutykd.WORKEDTM06, Dutykd.WORKSTTM07, Dutykd.WORKEDTM07, "
             + "Dutykd.LATECAPTMM "
             + "FROM Prsn INNER JOIN Dutykd ON (Prsn.DUTYKDNO = Dutykd.DUTYKDNO) ";
        Prsncmd += "WHERE Prsn.QUITDT IS NULL " + " AND Prsn.PRSNNO='" + CurPRSNNO + "'";
        DataTable dtbPrsn = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, Prsncmd);
        if (dtbPrsn.Rows.Count == 0)
        {
            //若員工未預設班別, 則在Agendakd找尋第一個PUNCHKDNO = CurPUNCHKDNO的班別
            string cmdAgendakd = "SELECT Agendakd.AGENDAKDNO, "
                 + "Dutykd.WORKSTTM01, Dutykd.WORKEDTM01, Dutykd.WORKSTTM02, Dutykd.WORKEDTM02, Dutykd.WORKSTTM03, Dutykd.WORKEDTM03, Dutykd.WORKSTTM04, "
                 + "Dutykd.WORKEDTM04, Dutykd.WORKSTTM05, Dutykd.WORKEDTM05, Dutykd.WORKSTTM06, Dutykd.WORKEDTM06, Dutykd.WORKSTTM07, Dutykd.WORKEDTM07, "
                 + "Dutykd.LATECAPTMM "
                 + "FROM Agendakd INNER JOIN Dutykd ON (Agendakd.AGENDAKDNO=Dutykd.DUTYKDNO) "
                 + "WHERE  Agendakd.PUNCHKDNO='" + CurPUNCHKDNO + "' ";
            return DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAgendakd);
            //若Agendakd中找不到第一個PUNCHKDNO = CurPUNCHKDNO的班別, 則設定班別DUTYKDNO='20'
        }
        else
            return dtbPrsn;
    }

    /*
     * Agendkd的正常班、下午班、晚班...使用PUNCHKDNO對應卡鐘有四個按鈕 1:上班 2:下班 3:外出 4:返回
     * Agendkd 若屬差勤別的紀錄則PUNCHKDNO不可空白
    */
    public static string SyncPrsnAgenda(DateTime CurPUNCHTIME, string CurPUNCHKDNO, string CurDPTNO, string CurPRSNNO, bool IsDelete = false, bool IsBatch = false)
    {
        //在 thread or timer 事件裏執行此程序 HttpContext.Current = null
        string AgendakdnoOut = "28";
        string PunchStat = null;
        //RepeatPunch = "1" 取第一次刷卡時間
        //RepeatPunch = "2" 取最後一次刷卡時間
        string RepeatPunch = "2";
        if (HttpContext.Current != null)
            RepeatPunch = HttpContext.Current.Application["RepeatPunch"].ToString();
        if (CurPUNCHKDNO == "1")
        {
            //上班
            //班別找尋優先順序:
            //  1.找尋打卡當天Agenda是否已有排班行程 
            //  2.找尋員工預設班別
            //  3.20-正常班
            //用CurPUNCHKDNO=Agendakd.PUNCHKDNO->找出對應班別:Dutykd
            string CurDUTYKDNO;
            int WeekDay = (int)CurPUNCHTIME.DayOfWeek == 0 ? 7 : (int)CurPUNCHTIME.DayOfWeek;
            //找尋打卡當天Agenda是否已有排班行程
            DataTable dtbDutyAgenda = RefechAgendaByPunchkd(CurPUNCHTIME, CurPUNCHKDNO, CurPRSNNO);
            /*
            //下列 WhereDateStr 是假設當天該員工排班行程只有一個上班別(或打卡當天排班行程可能有上班別 + 加班)-->AGENDAKDNO是唯一
            //如果員工排班行程是小夜班(16:00~24:00)或大夜班(00:00~08:00) WhereDateStr如何設定?
            string WhereDateStr = " AND (DUTYSTTM<='" + CurPUNCHTIME + "' AND DUTYEDTM>='" + CurPUNCHTIME.ToString() + "') ";
            //下列條件式不適用於 小夜班(16:00~24:00)
            //string WhereDateStr = " AND (DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            //string WhereDateStr = " AND (DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 00:00" + "' AND DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            string cmd = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.SUBJECT, Agenda.NOTES, Agenda.DUTYSTTM, "
                 + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO, Agenda.AGENDAKDNO "
                 + "FROM Agenda INNER JOIN Agendakd ON (Agenda.AGENDAKDNO=Agendakd.AGENDAKDNO) "
                 //+ " INNER JOIN Dutykd ON (Agendakd.DUTYKDNO=Dutykd.DUTYKDNO) "
                 + "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "' AND Agendakd.PUNCHKDNO='" + CurPUNCHKDNO + "') "
                 //+ "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "' AND Agenda.DUTYKDNO='" + CurDUTYKDNO + "') "
                 + WhereDateStr;
            DataTable dtbDutyAgenda = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
             */
            if (dtbDutyAgenda.Rows.Count == 0)
            {
                PunchStat = "員工未排班";
                //新增上班行程
                string CurDUTYSTTM;
                string CurDUTYEDTM;
                //找尋員工預設班別
                DataTable dtbDutykd = RefechDutykdByPrsn(CurPUNCHKDNO, CurPRSNNO);
                CurDUTYKDNO = dtbDutykd.Rows.Count == 0 ? "20" : dtbDutykd.Rows[0]["DUTYKDNO"].ToString();
                CurDUTYSTTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbDutykd.Rows[0]["WORKSTTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                CurDUTYEDTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbDutykd.Rows[0]["WORKEDTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();

                /*
                //Prsn.DUTYKDNO -> 員工的預設班別
                string Prsncmd = "SELECT Prsn.PRSNNO, Prsn.PRSNNM, Prsn.DPTNO,  Prsn.TITLEID, Prsn.DUTYKDNO, "
                     + "Dutykd.WORKSTTM01, Dutykd.WORKEDTM01, Dutykd.WORKSTTM02, Dutykd.WORKEDTM02, Dutykd.WORKSTTM03, Dutykd.WORKEDTM03, Dutykd.WORKSTTM04, "
                     + "Dutykd.WORKEDTM04, Dutykd.WORKSTTM05, Dutykd.WORKEDTM05, Dutykd.WORKSTTM06, Dutykd.WORKEDTM06, Dutykd.WORKSTTM07, Dutykd.WORKEDTM07, "
                     + "Dutykd.LATECAPTMM "
                     + "FROM Prsn INNER JOIN Dutykd ON (Prsn.DUTYKDNO = Dutykd.DUTYKDNO) ";
                Prsncmd += "WHERE Prsn.QUITDT IS NULL " + " AND Prsn.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Prsn.DPTNO='" + CurDPTNO + "'");
                DataTable dtbPrsn = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, Prsncmd);
                if (dtbPrsn.Rows.Count == 0)
                {
                    PunchStat = "員工未預設班別";
                    //若員工未預設班別, 則在Agendakd找尋第一個PUNCHKDNO = CurPUNCHKDNO的班別
                    string cmdAgendakd = "SELECT Agendakd.AGENDAKDNO, "
                         + "Dutykd.WORKSTTM01, Dutykd.WORKEDTM01, Dutykd.WORKSTTM02, Dutykd.WORKEDTM02, Dutykd.WORKSTTM03, Dutykd.WORKEDTM03, Dutykd.WORKSTTM04, "
                         + "Dutykd.WORKEDTM04, Dutykd.WORKSTTM05, Dutykd.WORKEDTM05, Dutykd.WORKSTTM06, Dutykd.WORKEDTM06, Dutykd.WORKSTTM07, Dutykd.WORKEDTM07, "
                         + "Dutykd.LATECAPTMM "
                         + "FROM Agendakd INNER JOIN Dutykd ON (Agendakd.AGENDAKDNO=Dutykd.DUTYKDNO) "
                         + "WHERE  Agendakd.PUNCHKDNO='" + CurPUNCHKDNO + "' ";
                    DataTable dtbAgendakd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAgendakd);
                    //若Agendakd中找不到第一個PUNCHKDNO = CurPUNCHKDNO的班別, 則設定班別DUTYKDNO='20'
                    CurDUTYKDNO = dtbAgendakd.Rows.Count == 0 ? "20" : dtbAgendakd.Rows[0]["DUTYKDNO"].ToString();
                    CurDUTYSTTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["WORKSTTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                    CurDUTYEDTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["WORKEDTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                }
                else
                {
                    CurDUTYKDNO = dtbPrsn.Rows[0]["DUTYKDNO"].ToString();
                    CurDUTYSTTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbPrsn.Rows[0]["WORKSTTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                    CurDUTYEDTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbPrsn.Rows[0]["WORKEDTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                }
                 */
                //SUBJECT, AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) VALUES (@SUBJECT, @NOTES, @AGENDASTTIME, @AGENDAEDTIME, @REMINDSTTIME, @REMINDDISMISS, @AGENDAKDNO, @DPTNO, @PRSNNO, @DUTYKDNO, @DUTYSTTM, @DUTYEDTM, @ISPUBLIC, @LOGID, @LOGDT)
                string InsertCmd = "INSERT INTO Agenda "
                    + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                    //+ "(SUBJECT, AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                     + "Values "
                    //+ "('上班'" + ", "
                     + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                     + "null, "
                     + "'" + CurDUTYKDNO + "', "
                     + "'" + CurDPTNO + "', "
                     + "'" + CurPRSNNO + "', "
                     + "'" + CurDUTYKDNO + "', "
                     + "'" + CurDUTYSTTM + "', "
                     + "'" + CurDUTYEDTM + "') ";
                //+ "'" + CurDUTYSTTM.ToString("yyyy/MM/dd HH:mm") + "', "
                //+ "'" + CurDUTYEDTM.ToString("yyyy/MM/dd HH:mm") + "') ";
                DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
            }
            else
            {
                //更新
                if (dtbDutyAgenda.Rows[0]["AGENDASTTIME"] == DBNull.Value)
                {
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                        //+ "SUBJECT= '" + "上班" + "', "
                        //+ "AGENDAKDNO= '" + "31" + "', "
                         + "AGENDASTTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbDutyAgenda.Rows[0]["AGENDAID"];
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                }
                else
                {
                    //Session["RepeatPunch"] = "1" 取第一次刷卡時間
                    //Session["RepeatPunch"] = "2" 取最後一次刷卡時間
                    if ((RepeatPunch == "1") && (CurPUNCHTIME < (DateTime)dtbDutyAgenda.Rows[0]["AGENDASTTIME"]))
                    {
                        string UpdateCmd = "UPDATE Agenda "
                             + "SET "
                            //+ "SUBJECT= '" + "上班" + "', "
                            //+ "AGENDAKDNO= '" + "31" + "', "
                             + "AGENDASTTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                             + "WHERE Agenda.AGENDAID=" + dtbDutyAgenda.Rows[0]["AGENDAID"];
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);

                    }
                    else if ((RepeatPunch == "2") && (CurPUNCHTIME > (DateTime)dtbDutyAgenda.Rows[0]["AGENDASTTIME"]))
                    {
                        string UpdateCmd = "UPDATE Agenda "
                             + "SET "
                            //+ "SUBJECT= '" + "上班" + "', "
                            //+ "AGENDAKDNO= '" + "31" + "', "
                             + "AGENDASTTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                             + "WHERE Agenda.AGENDAID=" + dtbDutyAgenda.Rows[0]["AGENDAID"];
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);

                    }
                }
            }
        }
        else if (CurPUNCHKDNO == "2")
        {
            //下班
            string CurDUTYKDNO;
            int WeekDay = (int)CurPUNCHTIME.DayOfWeek == 0 ? 7 : (int)CurPUNCHTIME.DayOfWeek;
            //找尋打卡當天Agenda是否已有排班行程
            DataTable dtbDutyAgenda = RefechAgendaByPunchkd(CurPUNCHTIME, CurPUNCHKDNO, CurPRSNNO);
            if (dtbDutyAgenda.Rows.Count == 0)
            {
                PunchStat = "員工未排班";
                //新增上班行程
                string CurDUTYSTTM;
                string CurDUTYEDTM;
                //找尋員工預設班別
                DataTable dtbDutykd = RefechDutykdByPrsn(CurPUNCHKDNO, CurPRSNNO);
                CurDUTYKDNO = dtbDutykd.Rows.Count == 0 ? "20" : dtbDutykd.Rows[0]["DUTYKDNO"].ToString();
                CurDUTYSTTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbDutykd.Rows[0]["WORKSTTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                CurDUTYEDTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbDutykd.Rows[0]["WORKEDTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();

                string InsertCmd = "INSERT INTO Agenda "
                    + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                    //+ "(SUBJECT, AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                     + "Values "
                    //+ "('上班'" + ", "
                     + "(null, "
                     + "'" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                     + "'" + CurDUTYKDNO + "', "
                     + "'" + CurDPTNO + "', "
                     + "'" + CurPRSNNO + "', "
                     + "'" + CurDUTYKDNO + "', "
                     + "'" + CurDUTYSTTM + "', "
                     + "'" + CurDUTYEDTM + "') ";
                //+ "'" + CurDUTYSTTM.ToString("yyyy/MM/dd HH:mm") + "', "
                //+ "'" + CurDUTYEDTM.ToString("yyyy/MM/dd HH:mm") + "') ";
                DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
            }
            else
            {
                if (dtbDutyAgenda.Rows[0]["AGENDASTTIME"] == DBNull.Value)
                {
                    PunchStat = "員工尚未輸入上班打卡紀錄";
                    //未輸入上班時間無法計算工時
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                        //+ "SUBJECT= '" + "正常出勤" + "', "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbDutyAgenda.Rows[0]["AGENDAID"];
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                }
                else
                {
                    //更新
                    //找尋Agenda是否已有外出行程
                    DateTime CurAGENDASTTIME = (DateTime)dtbDutyAgenda.Rows[0]["AGENDASTTIME"];
                    string OutWhereDateStr = " AND (AGENDASTTIME>='" + CurAGENDASTTIME.ToString("yyyy/MM/dd HH:mm") + "' AND AGENDAEDTIME<='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "') ";
                    string cmd = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDDUTYSEC, Agenda.ATTENDDUTYTXT "
                         + "FROM Agenda "
                         + "WHERE  (Agenda.AGENDAKDNO='" + AgendakdnoOut + "' AND Agenda.PRSNNO='" + CurPRSNNO + "') "
                         //+ "WHERE  (Agenda.AGENDAKDNO='28' AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                         + OutWhereDateStr;
                    DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                    //計算工時, 實際工時需減去外出時數(秒數)
                    //計算外出時數(秒數)
                    TimeSpan AttendSpan = CurPUNCHTIME - CurAGENDASTTIME;
                    double AttendSpanSEC = AttendSpan.TotalSeconds;
                    //處理多筆外出
                    foreach (DataRow AttendRow in dtbAttendOut.Rows)
                    {
                        //計算工時, 實際工時需減去外出時數
                        if (AttendRow["ATTENDDUTYSEC"].ToString() != "")
                            AttendSpanSEC -= Convert.ToDouble(AttendRow["ATTENDDUTYSEC"]);
                    }
                    TimeSpan AttendDuty = TimeSpan.FromSeconds(AttendSpanSEC);
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                        //+ "SUBJECT= '" + "正常出勤" + "', "
                        //+ "AGENDAKDNO= '" + "10" + "', "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "ATTENDDUTYSEC= '" + AttendSpanSEC + "', "
                         + "ATTENDDUTYTXT= '" + AttendDuty + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbDutyAgenda.Rows[0]["AGENDAID"];

                    /*
                    //計算工時
                    TimeSpan AttendTimeSpan = CurPUNCHTIME - CurAGENDASTTIME;
                    //處理多筆外出
                    foreach (DataRow AttendRow in dtbAttendOut.Rows)
                    {
                        //計算工時, 實際工時需減去外出時數
                        AttendTimeSpan -= (TimeSpan)AttendRow["ATTENDTS"];
                    }
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                        //+ "SUBJECT= '" + "正常出勤" + "', "
                        //+ "AGENDAKDNO= '" + "10" + "', "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "ATTENDTS= '" + AttendTimeSpan + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbAgenda.Rows[0]["AGENDAID"];
                    */
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                }
            }
        }
        else if (CurPUNCHKDNO == "3")
        {
            //外出 - 簽出
            //找尋Agenda是否已有對應出勤班別
            DataTable dtbMatchDuty = RefechAgendaByPunchkd(CurPUNCHTIME, "1", CurPRSNNO);
            /*
            string WhereDateStr = " AND (DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            //string WhereDateStr = " AND (DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 00:00" + "' AND DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            string cmdDuty = "SELECT Agenda.AGENDAID, Agenda.AGENDAKDNO, Agenda.DUTYKDNO, Agenda.DUTYSTTM, "
                 + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO "
                 + "FROM Agenda "
                 + "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                 + WhereDateStr;
            DataTable dtbMatchDuty = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdDuty);
             */

            //DateTime CurAGENDASTTIME = (DateTime)dtbPunchLog.Rows[0]["PUNCHTIME"];
            //AGENDAEDTIME 可能是 NULL
            if (dtbMatchDuty.Rows.Count == 0)
            {
                PunchStat = "無對應出勤班別";
                string InsertCmd = "INSERT INTO Agenda "
                    + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "
                     + "Values "
                     + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                     + "null, "
                     + "'" + AgendakdnoOut + "', "
                     + "'" + CurDPTNO + "', "
                     + "'" + CurPRSNNO + "') ";
                /*
                string InsertCmd = "INSERT INTO Agenda "
                    + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                    //+ "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "

                     + "Values "
                     + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                     + "null, "
                     + "'28', "
                     + "'" + CurDPTNO + "', "
                     + "'" + CurPRSNNO + "' "
                     + "'" + CurDUTYKDNO + "', "
                     + "'" + CurDUTYSTTM + "', "
                     + "'" + CurDUTYEDTM + "') ";
                */
                DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);

            }
            else
            {
                //找尋Agenda是否已輸入外出行程
                string OutWhereDateStr = " AND (AGENDASTTIME>='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYSTTM"]).ToString("yyyy/MM/dd HH:mm") + "' AND AGENDASTTIME<='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYEDTM"]).ToString("yyyy/MM/dd HH:mm") + "') ";
                string cmdAttendOut = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDTS "
                     + "FROM Agenda "
                     + "WHERE  (Agenda.AGENDAKDNO='" + AgendakdnoOut + "' AND Agenda.PRSNNO='" + CurPRSNNO + "') "
                    //+ "WHERE  (Agenda.AGENDAKDNO='" + AgendakdnoOut + "' AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                     + OutWhereDateStr;
                DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendOut);
                if (dtbAttendOut.Rows.Count == 0)
                {
                    //新增
                    //int WeekDay = (int)CurPUNCHTIME.DayOfWeek == 0 ? 7 : (int)CurPUNCHTIME.DayOfWeek;
                    //string CurDUTYSTTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbPrsn.Rows[0]["WORKSTTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                    //string CurDUTYEDTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbPrsn.Rows[0]["WORKEDTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                    string InsertCmd = "INSERT INTO Agenda "
                         + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "
                        //+ "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                         + "Values "
                         + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "null, "
                         + "'" + AgendakdnoOut + "', "
                         + "'" + CurDPTNO + "', "
                         + "'" + CurPRSNNO + "') ";
                    //+ "'" + CurDUTYKDNO + "', "
                    //+ "'" + CurDUTYSTTM + "', "
                    //+ "'" + CurDUTYEDTM + "') ";
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
                }
                else
                {
                    /*
                    //不論是否已存在外出, 一律新增另一筆外出 
                    string InsertCmd = "INSERT INTO Agenda "
                         + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "
                         + "Values "
                         + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "null, "
                         + "'" + AgendakdnoOut + "', "
                         + "'" + CurDPTNO + "', "
                         + "'" + CurPRSNNO + "') ";
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
                     */
                    //若已存在一筆外出且返回時間空白, 則優先更新該筆的的外出時間 
                    if (dtbAttendOut.Rows[0]["AGENDAEDTIME"] == DBNull.Value)
                    {
                        //更新第一筆外出的外出時間
                        string UpdateCmd = "UPDATE Agenda "
                             + "SET "
                             + "AGENDASTTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                             + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[0]["AGENDAID"];
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);

                    }
                    else
                    {
                        //新增第二筆外出
                        string InsertCmd = "INSERT INTO Agenda "
                             + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "
                             + "Values "
                             + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                             + "null, "
                             + "'" + AgendakdnoOut + "', "
                             + "'" + CurDPTNO + "', "
                             + "'" + CurPRSNNO + "') ";
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
                    }
                }
            }
        }
        else if (CurPUNCHKDNO == "4")
        {
            //返回 - 簽回
            //找尋Agenda是否已輸入外出行程
            DataTable dtbMatchDuty = RefechAgendaByPunchkd(CurPUNCHTIME, "1", CurPRSNNO);
            /*
            string WhereDateStr = " AND (DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            string cmdDuty = "SELECT Agenda.AGENDAID, Agenda.AGENDAKDNO, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME,  Agenda.ATTENDDUTYSEC, Agenda.DUTYKDNO, Agenda.DUTYSTTM, "
                 + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO "
                 + "FROM Agenda "
                 + "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                 + WhereDateStr;
            DataTable dtbMatchDuty = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdDuty);
             */

            //DateTime CurAGENDASTTIME = (DateTime)dtbPunchLog.Rows[0]["PUNCHTIME"];
            //AGENDAEDTIME 可能是 NULL
            if (dtbMatchDuty.Rows.Count == 0)
            {
                PunchStat = "無對應出勤班別";
                //找尋 外出時間<=返回打卡時間 AND 外出時間>=返回打卡時間-12hr 的外出行程
                string OutWhereDateStr = " AND (AGENDASTTIME<='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND AGENDASTTIME>='" + CurPUNCHTIME.AddHours(-12).ToString("yyyy/MM/dd HH:mm") + "') ";
                //*正常班(08:00~16:00)  打卡時間 2017/04/15 14:10
                //*   2017/04/15 13:00 <= 2017/04/15 14:10  AND 2017/04/15 16:00 >= 2017/04/15  02:10

                //找尋對應此筆的外出 - 簽出時間
                string cmdAttendOut = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDDUTYSEC, Agenda.ATTENDDUTYTXT "
                     + "FROM Agenda "
                     + "WHERE  (Agenda.AGENDAKDNO='" + AgendakdnoOut + "' AND Agenda.PRSNNO='" + CurPRSNNO + "') "
                    //+ "WHERE  (Agenda.AGENDAKDNO='28' AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                     + OutWhereDateStr
                     + "ORDER BY AGENDASTTIME ";
                DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendOut);
                if (dtbAttendOut.Rows.Count == 0)
                {
                    PunchStat = "無對應出勤班別且無對應外出 - 簽出行程";
                    //新增外出
                    string InsertCmd = "INSERT INTO Agenda "
                         + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "
                         + "Values "
                         + "(null, "
                         + "'" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "'" + AgendakdnoOut + "', "
                         + "'" + CurDPTNO + "', "
                         + "'" + CurPRSNNO + "') ";
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
                }
                else
                {
                    if (IsDelete)
                    {
                        //返回 - 簽回被刪除時, 更新外出行程
                        string UpdateCmd = "UPDATE Agenda "
                             + "SET "
                             + "AGENDAEDTIME= null, "
                             + "ATTENDDUTYSEC= 0, "
                             + "ATTENDDUTYTXT= null "
                             + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[dtbAttendOut.Rows.Count - 1]["AGENDAID"];
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                    }
                    else
                    {
                        //兩筆外出, 更新最後一筆
                        //計算外出時數
                        TimeSpan AttendSpan = CurPUNCHTIME - ((DateTime)dtbAttendOut.Rows[dtbAttendOut.Rows.Count - 1]["AGENDASTTIME"]);
                        double AttendSpanSEC = AttendSpan.TotalSeconds;
                        TimeSpan AttendDuty = TimeSpan.FromSeconds(AttendSpanSEC);
                        //更新外出行程
                        string UpdateCmd = "UPDATE Agenda "
                             + "SET "
                             + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                             + "ATTENDDUTYSEC= '" + AttendSpanSEC + "', "
                             + "ATTENDDUTYTXT= '" + AttendDuty + "' "
                             + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[dtbAttendOut.Rows.Count - 1]["AGENDAID"];
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                    }
                }
            }
            else
            {
                //找尋對應此筆的外出 - 簽出時間
                //外出時間>=班別起始時間 AND 外出時間<返回打卡時間
                string OutWhereDateStr = " AND (AGENDASTTIME>='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYSTTM"]).ToString("yyyy/MM/dd HH:mm") + "' AND AGENDASTTIME<'" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "') ";
                //外出時間>=班別起始時間 AND 外出時間<=班別結束時間
                //string OutWhereDateStr = " AND (AGENDASTTIME>='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYSTTM"]).ToString("yyyy/MM/dd HH:mm") + "' AND AGENDASTTIME<='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYEDTM"]).ToString("yyyy/MM/dd HH:mm") + "') ";
                //string OutWhereDateStr = " AND (AGENDASTTIME>='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYSTTM"]).ToString("yyyy/MM/dd HH:mm") + "' AND AGENDASTTIME<='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYEDTM"]).ToString("yyyy/MM/dd HH:mm") + "') AND (AGENDAEDTIME IS NULL) ";
                string cmdAttendOut = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDDUTYSEC, Agenda.ATTENDDUTYTXT "
                     + "FROM Agenda "
                     + "WHERE  (Agenda.AGENDAKDNO='" + AgendakdnoOut + "' AND Agenda.PRSNNO='" + CurPRSNNO + "') "
                    //+ "WHERE  (Agenda.AGENDAKDNO='28' AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                     + OutWhereDateStr
                     + "ORDER BY AGENDASTTIME ";
                DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendOut);
                if (dtbAttendOut.Rows.Count == 0)
                {
                    PunchStat = "無對應外出 - 簽出行程";
                    //新增外出
                    string InsertCmd = "INSERT INTO Agenda "
                         + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "
                         + "Values "
                         + "(null, "
                         + "'" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "'" + AgendakdnoOut + "', "
                         + "'" + CurDPTNO + "', "
                         + "'" + CurPRSNNO + "') ";
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
                }
                else
                {
                    if (IsDelete)
                    {
                        //返回 - 簽回被刪除時, 更新外出行程
                        string UpdateCmd = "UPDATE Agenda "
                             + "SET "
                             + "AGENDAEDTIME= null, "
                             + "ATTENDDUTYSEC= 0, "
                             + "ATTENDDUTYTXT= null "
                             + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[dtbAttendOut.Rows.Count - 1]["AGENDAID"];
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                    }
                    else
                    {
                        //兩筆外出, 更新最後一筆
                        //計算外出時數
                        TimeSpan AttendSpan = CurPUNCHTIME - ((DateTime)dtbAttendOut.Rows[dtbAttendOut.Rows.Count - 1]["AGENDASTTIME"]);
                        double AttendSpanSEC = AttendSpan.TotalSeconds;
                        TimeSpan AttendDuty = TimeSpan.FromSeconds(AttendSpanSEC);
                        //更新外出行程
                        string UpdateCmd = "UPDATE Agenda "
                             + "SET "
                             + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                             + "ATTENDDUTYSEC= '" + AttendSpanSEC + "', "
                             + "ATTENDDUTYTXT= '" + AttendDuty + "' "
                             + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[dtbAttendOut.Rows.Count - 1]["AGENDAID"];
                        DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                    }
                    /*
                    TimeSpan AttendTimeSpan = CurPUNCHTIME - ((DateTime)dtbAttendOut.Rows[0]["AGENDASTTIME"]);
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "ATTENDTS= '" + AttendTimeSpan + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[0]["AGENDAID"];
                    */
                    if (!IsBatch)
                    {
                        //從ImportPunchLog.aspx程序IsBatch = True -> 無須進行下列
                        //若從PunchAttend.aspx單獨新增 返回 時IsBatch = False -> 務必進行下列
                        //計算出勤工時(打卡時數-外出時數(秒數))
                        if (dtbMatchDuty.Rows[0]["ATTENDDUTYSEC"] != DBNull.Value)
                        {
                            TimeSpan AttendSpan = ((DateTime)dtbMatchDuty.Rows[dtbAttendOut.Rows.Count - 1]["AGENDAEDTIME"]) - ((DateTime)dtbMatchDuty.Rows[dtbAttendOut.Rows.Count - 1]["AGENDASTTIME"]);
                            double AttendSpanSEC = AttendSpan.TotalSeconds;
                            //AttendSpanSEC = Convert.ToDouble(dtbMatchDuty.Rows[0]["ATTENDDUTYSEC"]);
                            //重新取得Agenda的所有外出行程
                            dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendOut);
                            //計算工時, 實際工時需減去外出時數(秒數)
                            //計算外出時數(秒數)
                            //處理多筆外出
                            foreach (DataRow AttendRow in dtbAttendOut.Rows)
                            {
                                //計算工時, 實際工時需減去外出時數
                                if (AttendRow["ATTENDDUTYSEC"] != DBNull.Value)
                                    AttendSpanSEC -= Convert.ToDouble(AttendRow["ATTENDDUTYSEC"]);
                            }
                            TimeSpan AttendDuty = TimeSpan.FromSeconds(AttendSpanSEC);
                            string UpdateCmd = "UPDATE Agenda "
                                 + "SET "
                                //+ "SUBJECT= '" + "正常出勤" + "', "
                                //+ "AGENDAKDNO= '" + "10" + "', "
                                 + "ATTENDDUTYSEC= '" + AttendSpanSEC + "', "
                                 + "ATTENDDUTYTXT= '" + AttendDuty + "' "
                                 + "WHERE Agenda.AGENDAID=" + dtbMatchDuty.Rows[0]["AGENDAID"];
                            DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                        }
                    }
                }
            }
        }
        return PunchStat;
    }

    public static void SyncPrsnAgendaLast(DateTime CurPUNCHTIME, string CurPUNCHKDNO, string CurDPTNO, string CurPRSNNO)
    {
        if (CurPUNCHKDNO == "1")
        {
            //上班
            /*
            //找尋員工預設班別
            string Prsncmd = "SELECT Prsn.PRSNNO, Prsn.PRSNNM, Prsn.DPTNO,  Prsn.TITLEID, Prsn.DUTYKDNO, "
                 + "Dutykd.WORKSTTM01, Dutykd.WORKEDTM01, Dutykd.WORKSTTM02, Dutykd.WORKEDTM02, Dutykd.WORKSTTM03, Dutykd.WORKEDTM03, Dutykd.WORKSTTM04, "
                 + "Dutykd.WORKEDTM04, Dutykd.WORKSTTM05, Dutykd.WORKEDTM05, Dutykd.WORKSTTM06, Dutykd.WORKEDTM06, Dutykd.WORKSTTM07, Dutykd.WORKEDTM07, "
                 + "Dutykd.LATECAPTMM "
                 + "FROM Prsn LEFT JOIN Dutykd ON (Prsn.DUTYKDNO = Dutykd.DUTYKDNO) ";
            Prsncmd += "WHERE Prsn.QUITDT IS NULL " + " AND Prsn.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Prsn.DPTNO='" + CurDPTNO + "'");
            DataTable dtbPrsn = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, Prsncmd);
            //若員工未預設班別則找尋20-正常班 -> Agenda.DUTYKDNO='20'
            //Prsn.DUTYKDNO -> 員工的預設班別
            string CurDUTYKDNO = dtbPrsn.Rows.Count == 0 ? "20" : dtbPrsn.Rows[0]["DUTYKDNO"].ToString();
            */
            //用CurPUNCHKDNO 找出對應班別
            string cmdAgendakd = "SELECT Agendakd.AGENDAKDNO, "
                 + "Dutykd.WORKSTTM01, Dutykd.WORKEDTM01, Dutykd.WORKSTTM02, Dutykd.WORKEDTM02, Dutykd.WORKSTTM03, Dutykd.WORKEDTM03, Dutykd.WORKSTTM04, "
                 + "Dutykd.WORKEDTM04, Dutykd.WORKSTTM05, Dutykd.WORKEDTM05, Dutykd.WORKSTTM06, Dutykd.WORKEDTM06, Dutykd.WORKSTTM07, Dutykd.WORKEDTM07, "
                 + "Dutykd.LATECAPTMM "
                 + "FROM Agendakd INNER JOIN Dutykd ON (Agendakd.AGENDAKDNO=Dutykd.DUTYKDNO) "
                 + "WHERE  Agendakd.PUNCHKDNO='" + CurPUNCHKDNO + "' ";
            DataTable dtbAgendakd = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAgendakd);
            string CurDUTYKDNO = dtbAgendakd.Rows.Count == 0 ? "20" : dtbAgendakd.Rows[0]["AGENDAKDNO"].ToString();

            //找尋打卡當天Agenda是否已有排班行程
            //打卡當天排班行程可能有正常班+加班, 當天該員工的AGENDAKDNO是唯一
            string WhereDateStr = " AND (DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            //string WhereDateStr = " AND (DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 00:00" + "' AND DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            string cmd = "SELECT Agenda.AGENDAID, Agenda.SUBJECT, Agenda.NOTES, Agenda.DUTYSTTM, "
                 + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO, Agenda.AGENDAKDNO "
                 + "FROM Agenda "
                 + "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "' AND Agenda.DUTYKDNO='" + CurDUTYKDNO + "') "
                 + WhereDateStr;

            DataTable dtbAgenda = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
            if (dtbAgenda.Rows.Count == 0)
            {
                //新增
                int WeekDay = (int)CurPUNCHTIME.DayOfWeek == 0 ? 7 : (int)CurPUNCHTIME.DayOfWeek;
                string CurDUTYSTTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["WORKSTTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                string CurDUTYEDTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbAgendakd.Rows[0]["WORKEDTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                //string CurDUTYSTTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbPrsn.Rows[0]["WORKSTTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                //string CurDUTYEDTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbPrsn.Rows[0]["WORKEDTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();

                //SUBJECT, AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) VALUES (@SUBJECT, @NOTES, @AGENDASTTIME, @AGENDAEDTIME, @REMINDSTTIME, @REMINDDISMISS, @AGENDAKDNO, @DPTNO, @PRSNNO, @DUTYKDNO, @DUTYSTTM, @DUTYEDTM, @ISPUBLIC, @LOGID, @LOGDT)
                string InsertCmd = "INSERT INTO Agenda "
                    + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                    //+ "(SUBJECT, AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                     + "Values "
                    //+ "('上班'" + ", "
                     + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                     + "null, "
                     + "'" + CurDUTYKDNO + "', "
                     + "'" + CurDPTNO + "', "
                     + "'" + CurPRSNNO + "', "
                     + "'" + CurDUTYKDNO + "', "
                     + "'" + CurDUTYSTTM + "', "
                     + "'" + CurDUTYEDTM + "') ";
                //+ "'" + CurDUTYSTTM.ToString("yyyy/MM/dd HH:mm") + "', "
                //+ "'" + CurDUTYEDTM.ToString("yyyy/MM/dd HH:mm") + "') ";
                DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
            }
            else
            {
                //更新
                string UpdateCmd = "UPDATE Agenda "
                     + "SET "
                    //+ "SUBJECT= '" + "上班" + "', "
                    //+ "AGENDAKDNO= '" + "31" + "', "
                     + "AGENDASTTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                     + "WHERE Agenda.AGENDAID=" + dtbAgenda.Rows[0]["AGENDAID"];
                DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
            }
        }
        else if (CurPUNCHKDNO == "2")
        {
            //下班
            //找尋對應此筆的上班打卡時間
            string cmd = "SELECT MAX(PunchLog.PUNCHTIME) PUNCHTIME "
                 + "FROM PunchLog "
                 + "WHERE PunchLog.PUNCHKDNO='1' AND  PUNCHTIME<'" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                 + "AND PunchLog.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND PunchLog.DPTNO='" + CurDPTNO + "'");

            DataTable dtbPunchLog = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
            if (dtbPunchLog.Rows[0]["PUNCHTIME"] == DBNull.Value)
            {
                //尚未輸入上班打卡紀錄
            }
            else
            {
                //找尋Agenda是否已有上班行程
                DateTime CurAGENDASTTIME = (DateTime)dtbPunchLog.Rows[0]["PUNCHTIME"];
                string WhereDateStr = " AND (AGENDASTTIME='" + CurAGENDASTTIME.ToString("yyyy/MM/dd HH:mm") + "') ";
                cmd = "SELECT Agenda.AGENDAID, Agenda.SUBJECT, Agenda.NOTES, Agenda.DUTYSTTM, "
                     + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO, Agenda.AGENDAKDNO "
                     + "FROM Agenda "
                     + "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                     + WhereDateStr;

                DataTable dtbAgenda = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                if (dtbAgenda.Rows.Count > 0)
                {
                    //更新
                    //找尋Agenda是否已有外出行程
                    string OutWhereDateStr = " AND (AGENDASTTIME>='" + CurAGENDASTTIME.ToString("yyyy/MM/dd HH:mm") + "' AND AGENDAEDTIME<='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "') ";
                    cmd = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDDUTYSEC, Agenda.ATTENDDUTYTXT "
                         + "FROM Agenda "
                         + "WHERE  (Agenda.AGENDAKDNO='28' AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                         + OutWhereDateStr;
                    DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmd);
                    //計算工時, 實際工時需減去外出時數(秒數)
                    //計算外出時數(秒數)
                    TimeSpan AttendSpan = CurPUNCHTIME - CurAGENDASTTIME;
                    double AttendSpanSEC = AttendSpan.TotalSeconds;
                    //處理多筆外出
                    foreach (DataRow AttendRow in dtbAttendOut.Rows)
                    {
                        //計算工時, 實際工時需減去外出時數
                        if (AttendRow["ATTENDDUTYSEC"].ToString() != "")
                            AttendSpanSEC -= Convert.ToDouble(AttendRow["ATTENDDUTYSEC"]);
                    }
                    TimeSpan AttendDuty = TimeSpan.FromSeconds(AttendSpanSEC);
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                        //+ "SUBJECT= '" + "正常出勤" + "', "
                        //+ "AGENDAKDNO= '" + "10" + "', "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "ATTENDDUTYSEC= '" + AttendSpanSEC + "', "
                         + "ATTENDDUTYTXT= '" + AttendDuty + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbAgenda.Rows[0]["AGENDAID"];

                    /*
                    //計算工時
                    TimeSpan AttendTimeSpan = CurPUNCHTIME - CurAGENDASTTIME;
                    //處理多筆外出
                    foreach (DataRow AttendRow in dtbAttendOut.Rows)
                    {
                        //計算工時, 實際工時需減去外出時數
                        AttendTimeSpan -= (TimeSpan)AttendRow["ATTENDTS"];
                    }
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                        //+ "SUBJECT= '" + "正常出勤" + "', "
                        //+ "AGENDAKDNO= '" + "10" + "', "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "ATTENDTS= '" + AttendTimeSpan + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbAgenda.Rows[0]["AGENDAID"];
                    */
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                }
                /*
                if (dtbAgenda.Rows.Count > 0)
                {
                    //更新
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                        //+ "SUBJECT= '" + "正常出勤" + "', "
                        //+ "AGENDAKDNO= '" + "10" + "', "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbAgenda.Rows[0]["AGENDAID"];
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                }
                */
            }
        }
        else if (CurPUNCHKDNO == "3")
        {
            //外出 - 簽出
            //找尋Agenda是否已有對應出勤班別
            string WhereDateStr = " AND (DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            //string WhereDateStr = " AND (DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 00:00" + "' AND DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            string cmdDuty = "SELECT Agenda.AGENDAID, Agenda.AGENDAKDNO, Agenda.DUTYKDNO, Agenda.DUTYSTTM, "
                 + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO "
                 + "FROM Agenda "
                 + "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                 + WhereDateStr;
            DataTable dtbMatchDuty = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdDuty);
            //string CurDUTYKDNO = dtbMatchDuty.Rows.Count == 0 ? "20" : dtbMatchDuty.Rows[0]["DUTYKDNO"].ToString();

            //DateTime CurAGENDASTTIME = (DateTime)dtbPunchLog.Rows[0]["PUNCHTIME"];
            //AGENDAEDTIME 可能是 NULL
            if (dtbMatchDuty.Rows.Count == 0)
            {
                string InsertCmd = "INSERT INTO Agenda "
                    + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "
                     + "Values "
                     + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                     + "null, "
                     + "'28', "
                     + "'" + CurDPTNO + "', "
                     + "'" + CurPRSNNO + "') ";
                /*
                string InsertCmd = "INSERT INTO Agenda "
                    + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                    //+ "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "

                     + "Values "
                     + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                     + "null, "
                     + "'28', "
                     + "'" + CurDPTNO + "', "
                     + "'" + CurPRSNNO + "' "
                     + "'" + CurDUTYKDNO + "', "
                     + "'" + CurDUTYSTTM + "', "
                     + "'" + CurDUTYEDTM + "') ";
                */
                DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);

            }
            else
            {
                //找尋Agenda是否已輸入外出行程
                string OutWhereDateStr = " AND (AGENDASTTIME>='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYSTTM"]).ToString("yyyy/MM/dd HH:mm") + "' AND AGENDASTTIME<='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYEDTM"]).ToString("yyyy/MM/dd HH:mm") + "') ";
                string cmdAttendOut = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDTS "
                     + "FROM Agenda "
                     + "WHERE  (Agenda.AGENDAKDNO='28' AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                     + OutWhereDateStr;
                DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendOut);
                if (dtbAttendOut.Rows.Count == 0)
                {
                    //新增
                    //int WeekDay = (int)CurPUNCHTIME.DayOfWeek == 0 ? 7 : (int)CurPUNCHTIME.DayOfWeek;
                    //string CurDUTYSTTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbPrsn.Rows[0]["WORKSTTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                    //string CurDUTYEDTM = CurPUNCHTIME.ToString("yyyy/MM/dd") + " " + dtbPrsn.Rows[0]["WORKEDTM" + WeekDay.ToString().PadLeft(2, '0')].ToString();
                    string InsertCmd = "INSERT INTO Agenda "
                         + "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO) "
                        //+ "(AGENDASTTIME, AGENDAEDTIME, AGENDAKDNO, DPTNO, PRSNNO, DUTYKDNO, DUTYSTTM, DUTYEDTM) "
                         + "Values "
                         + "('" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "null, "
                         + "'28', "
                         + "'" + CurDPTNO + "', "
                         + "'" + CurPRSNNO + "') ";
                    //+ "'" + CurDUTYKDNO + "', "
                    //+ "'" + CurDUTYSTTM + "', "
                    //+ "'" + CurDUTYEDTM + "') ";
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, InsertCmd);
                }
                else
                {
                    //若是第二筆外出 -> 變成更新第一筆外出的起始時間? , 因此兩筆外出只取時間最後的一筆
                    //更新
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                         + "AGENDASTTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[0]["AGENDAID"];
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                }
            }
        }
        else if (CurPUNCHKDNO == "4")
        {
            //返回 - 簽回
            //找尋對應此筆的外出 - 簽出時間
            string WhereDateStr = " AND (DUTYEDTM>='" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "' AND DUTYSTTM<='" + CurPUNCHTIME.ToString("yyyy/MM/dd") + " 23:59" + "') ";
            string cmdDuty = "SELECT Agenda.AGENDAID, Agenda.AGENDAKDNO, Agenda.DUTYKDNO, Agenda.DUTYSTTM, "
                 + "Agenda.DUTYEDTM, Agenda.DPTNO, Agenda.PRSNNO "
                 + "FROM Agenda "
                 + "WHERE  (Agenda.DUTYKDNO IS NOT NULL AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                 + WhereDateStr;
            DataTable dtbMatchDuty = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdDuty);
            //string CurDUTYKDNO = dtbMatchDuty.Rows.Count == 0 ? "20" : dtbMatchDuty.Rows[0]["DUTYKDNO"].ToString();
            //找尋Agenda是否已輸入外出行程

            //DateTime CurAGENDASTTIME = (DateTime)dtbPunchLog.Rows[0]["PUNCHTIME"];
            //AGENDAEDTIME 可能是 NULL
            if (dtbMatchDuty.Rows.Count > 0)
            {
                string OutWhereDateStr = " AND (AGENDASTTIME>='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYSTTM"]).ToString("yyyy/MM/dd HH:mm") + "' AND AGENDASTTIME<='" + ((DateTime)dtbMatchDuty.Rows[0]["DUTYEDTM"]).ToString("yyyy/MM/dd HH:mm") + "') ";
                string cmdAttendOut = "SELECT Agenda.AGENDAID, Agenda.AGENDASTTIME, Agenda.AGENDAEDTIME, Agenda.ATTENDDUTYSEC, Agenda.ATTENDDUTYTXT "
                     + "FROM Agenda "
                     + "WHERE  (Agenda.AGENDAKDNO='28' AND Agenda.PRSNNO='" + CurPRSNNO + "'" + (CurDPTNO == "" ? "" : " AND Agenda.DPTNO='" + CurDPTNO) + "') "
                     + OutWhereDateStr;
                DataTable dtbAttendOut = DBUtility.RefetchDataTable(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, cmdAttendOut);
                if (dtbAttendOut.Rows.Count > 0)
                {
                    //更新
                    //計算外出時數
                    TimeSpan AttendSpan = CurPUNCHTIME - ((DateTime)dtbAttendOut.Rows[0]["AGENDASTTIME"]);
                    double AttendSpanSEC = AttendSpan.TotalSeconds;
                    TimeSpan AttendDuty = TimeSpan.FromSeconds(AttendSpanSEC);
                    //更新外出行程
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "ATTENDDUTYSEC= '" + AttendSpanSEC + "', "
                         + "ATTENDDUTYTXT= '" + AttendDuty + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[0]["AGENDAID"];

                    /*
                    TimeSpan AttendTimeSpan = CurPUNCHTIME - ((DateTime)dtbAttendOut.Rows[0]["AGENDASTTIME"]);
                    string UpdateCmd = "UPDATE Agenda "
                         + "SET "
                         + "AGENDAEDTIME= '" + CurPUNCHTIME.ToString("yyyy/MM/dd HH:mm") + "', "
                         + "ATTENDTS= '" + AttendTimeSpan + "' "
                         + "WHERE Agenda.AGENDAID=" + dtbAttendOut.Rows[0]["AGENDAID"];
                    */
                    DBUtility.ExecuteScalar(WebConfigurationManager.ConnectionStrings["WinSisTmplConnectionString"].ConnectionString, UpdateCmd);
                }
            }

        }
    }

    //將秒數轉換為時:分:秒字串格式
    public static string SecTohhmmss(double CurSec)
    {
        TimeSpan t = TimeSpan.FromSeconds(CurSec);
        //int HrInDays = 0;
        //if (t.Days > 0)
        //    HrInDays = t.Days * 24;
        string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
            //string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
         (int)t.TotalHours,
            //t.Hours + HrInDays,
         t.Minutes,
         t.Seconds);
        return answer;
    }

    public static void PlayClsTool(System.Web.UI.Page sender, string CurCLSNO, string CurCLSPTNO, string CurCLSSRNO, string CurTOOLHLINK, string CurDEFAULTPAGE, string CurTOOLDESC, Boolean ISCallBack = false)
    {
        //檔案副檔名
        string CurFileExt = System.IO.Path.GetExtension(CurTOOLHLINK);
        string CurSource = HttpContext.Current.Application["WebHrs-APPURLBASE"].ToString() + HttpContext.Current.Application["WebHrs-CLSFILEDIR"].ToString() + CurCLSNO + "/" + CurCLSNO + "_" + CurCLSPTNO + "_" + CurCLSSRNO + CurFileExt;
        if ((CurDEFAULTPAGE != "") && ((CurFileExt == ".zip") || (CurFileExt == ".rar")))
        {
            //數位教材
            CurSource = HttpContext.Current.Application["WebHrs-APPURLBASE"].ToString() + HttpContext.Current.Application["WebHrs-CLSFILEDIR"].ToString() + CurCLSNO + "/" + CurCLSNO + "_" + CurCLSPTNO + "_" + CurCLSSRNO + "/" + CurDEFAULTPAGE;
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = CurSource;
            else
                //Callback事件無法使用ScriptManager.RegisterClientScriptBlock
                //同一視窗開啟
                ScriptManager.RegisterClientScriptBlock(sender, sender.GetType(), "test", "window.open('" + CurSource + "', '_self',false);", true);
            //開啟新視窗(手機會阻擋開啟新視窗)
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "test", "window.open('" + CurSource + "', '_blank');", true);
        }
        else if (CurFileExt == ".mp4")
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = "PlayMedia.aspx?VideoSource=" + CurSource + "&Videotit=" + CurTOOLDESC + "&CurCLSNO=" + CurCLSNO;
            else
                //Callback事件無法使用System.Web.HttpContext.Current.Response.Redirect或Server.Transfer
                System.Web.HttpContext.Current.Response.Redirect("PlayMedia.aspx?VideoSource=" + CurSource + "&Videotit=" + CurTOOLDESC + "&CurCLSNO=" + CurCLSNO);
        else if (CurFileExt == ".mpg")
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = "PlayMedia.aspx?VideoSource=" + CurSource + "&Videotit=" + CurTOOLDESC + "&CurCLSNO=" + CurCLSNO;
            else
                //Callback事件無法使用System.Web.HttpContext.Current.Response.Redirect或Server.Transfer
                System.Web.HttpContext.Current.Response.Redirect("PlayMedia.aspx?VideoSource=" + CurSource + "&Videotit=" + CurTOOLDESC + "&CurCLSNO=" + CurCLSNO);
        else if (CurFileExt == ".pdf")
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ALERT", "alert('訊息');", true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "ALERT", "alert('訊息！');window.location='"+CurSource+"';", true);
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = CurSource;
            else
                //Callback事件無法使用ScriptManager.RegisterClientScriptBlock
                ScriptManager.RegisterClientScriptBlock(sender, sender.GetType(), "test", "window.open('" + CurSource + "', '_self');", true);
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "test", "window.open('" + CurSource + "', '_blank'); location.href='" + CurSource + "';", true);
        else if (CurFileExt != "")
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = CurSource;
            else
                //Callback事件無法使用ScriptManager.RegisterClientScriptBlock
                //同一視窗開啟
                ScriptManager.RegisterClientScriptBlock(sender, sender.GetType(), "test", "window.open('" + CurSource + "', '_self');", true);
        //開啟新視窗(手機會阻擋開啟新視窗)
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "test", "window.open('" + CurSource + "', '_blank');", true);
    }

    public static void PlayJobTool(System.Web.UI.Page sender, string CurJOBNO, string CurMEDIAFILENM, string CurDEFAULTPAGE, Boolean ISCallBack = false)
    {
        //檔案副檔名
        string CurFileExt = System.IO.Path.GetExtension(CurMEDIAFILENM);
        string CurSource = HttpContext.Current.Application["WebHrs-APPURLBASE"].ToString() + HttpContext.Current.Application["WebHrs-JOBSTEPSFILEDIR"].ToString() + CurJOBNO + "/" + CurJOBNO + CurFileExt;
        if ((CurDEFAULTPAGE != "") && ((CurFileExt == ".zip") || (CurFileExt == ".rar")))
        {
            //數位教材
            CurSource = HttpContext.Current.Application["WebHrs-APPURLBASE"].ToString() + HttpContext.Current.Application["WebHrs-CLSFILEDIR"].ToString() + CurJOBNO + "/" + CurDEFAULTPAGE;
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = CurSource;
            else
                //Callback事件無法使用ScriptManager.RegisterClientScriptBlock
                //同一視窗開啟
                ScriptManager.RegisterClientScriptBlock(sender, sender.GetType(), "test", "window.open('" + CurSource + "', '_self',false);", true);
            //開啟新視窗(手機會阻擋開啟新視窗)
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "test", "window.open('" + CurSource + "', '_blank');", true);
        }
        else if (CurFileExt == ".mp4")
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = "PlayMedia.aspx?VideoSource=" + CurSource + "&Videotit=" + CurMEDIAFILENM;
            else
                //Callback事件無法使用System.Web.HttpContext.Current.Response.Redirect或Server.Transfer
                System.Web.HttpContext.Current.Response.Redirect("PlayMedia.aspx?VideoSource=" + CurSource + "&Videotit=" + CurMEDIAFILENM);
        else if (CurFileExt == ".mpg")
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = "PlayMedia.aspx?VideoSource=" + CurSource + "&Videotit=" + CurMEDIAFILENM;
            else
                //Callback事件無法使用System.Web.HttpContext.Current.Response.Redirect或Server.Transfer
                System.Web.HttpContext.Current.Response.Redirect("PlayMedia.aspx?VideoSource=" + CurSource + "&Videotit=" + CurMEDIAFILENM);
        else if (CurFileExt == ".pdf")
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = CurSource;
            else
                //Callback事件無法使用ScriptManager.RegisterClientScriptBlock
                ScriptManager.RegisterClientScriptBlock(sender, sender.GetType(), "test", "window.open('" + CurSource + "', '_self');", true);
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "test", "window.open('" + CurSource + "', '_blank'); location.href='" + CurSource + "';", true);
        else if (CurFileExt != "")
            if (ISCallBack)
                System.Web.HttpContext.Current.Response.RedirectLocation = CurSource;
            else
                //Callback事件無法使用ScriptManager.RegisterClientScriptBlock
                //同一視窗開啟
                ScriptManager.RegisterClientScriptBlock(sender, sender.GetType(), "test", "window.open('" + CurSource + "', '_self');", true);
        //開啟新視窗(手機會阻擋開啟新視窗)
        //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "test", "window.open('" + CurSource + "', '_blank');", true);
    }
}
