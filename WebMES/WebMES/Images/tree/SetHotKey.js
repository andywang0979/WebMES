//捕捉A~Z,1~10..一般按鍵 => 透過onkeypress事件
//捕捉F2~F10 ...特殊按鍵 => 透過onkeydown事件
var travel=true
var hotkey_f2=113  
var hotkey_f3=114 
var hotkey_f4=115 
var hotkey_f5=116 
var hotkey_f6=117 
var hotkey_f7=118 
var hotkey_f12=123 
if (document.layers)
  document.captureEvents(Event.KEYPRESS)
function FastKeyGo(e)
{
  if (document.layers)
  {
    if (e.which==hotkey_f2&&travel)
      SubmitClickConfirm('NAVGINSERT','', true, '')
  }
  else if (document.all)
  {
    if (event.keyCode==hotkey_f2)
    {
      //新增資料, F2
      if (document.getElementById('NAVGINSERT'))
        SubmitClickConfirm('NAVGINSERT','', true, '')
    }
    else if (event.keyCode==hotkey_f3)
    { 
      //更改資料, F3
      if (document.getElementById('NAVGEDIT'))
        SubmitClickConfirm('NAVGEDIT','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if (event.keyCode==hotkey_f4)
    { 
      //刪除資料, F4
      if (document.getElementById('NAVGDELETE'))
        SubmitClickConfirm('NAVGDELETE','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if (event.keyCode==hotkey_f5)
    { 
      //重新載入最新資料, F5
      if (document.getElementById('NAVGREFRESH'))
        SubmitClickConfirm('NAVGREFRESH','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if (event.keyCode==hotkey_f6)
    { 
      //儲存資料異動, F6
      if (document.getElementById('NAVGPOST'))
        SubmitClickConfirm('NAVGPOST','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if (event.keyCode==hotkey_f7)
    { 
      //取消資料異動, F7
      if (document.getElementById('NAVGCANCEL'))
        SubmitClickConfirm('NAVGCANCEL','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if (event.keyCode==hotkey_f12)
    { 
      //主副檔切換, F12
      if (document.getElementById('NAVGFLIP'))
        SubmitClickConfirm('NAVGFLIP','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if ((event.ctrlKey) && (event.keyCode==80 || event.keyCode==112))
    { 
      //列印資料, Ctrl+P or Ctrl+p
      if (document.getElementById('NAVGPRINT'))
        SubmitClickConfirm('NAVGPRINT','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if ((event.ctrlKey) && (event.keyCode==81 || event.keyCode==113))
    { 
      //設定查詢條件, Ctrl+Q or Ctrl+q
      if (document.getElementById('NAVGFILTER'))
        SubmitClickConfirm('NAVGFILTER','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if ((event.ctrlKey) && (event.keyCode==36))
    { 
      //回到首頁, Ctrl+Home
      if (document.getElementById('NAVGHOME'))
        SubmitClickConfirm('NAVGHOME','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if ((event.altKey) && (event.keyCode==37))
    { 
      //回到上頁, Alt+←
      if (document.getElementById('NAVGBACK'))
        SubmitClickConfirm('NAVGBACK','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
    else if (event.keyCode==13)
    { 
      //Enter鍵(在設定查詢條件頁面)
      if (document.getElementById('GOFILTER'))
      {
        SubmitClickConfirm('GOFILTER','', true, '')
        event.keyCode=0
        event.returnValue=false
      }
    }
    else if (event.keyCode==27)
    { 
      //Escape鍵(在設定查詢條件頁面)
      if (document.getElementById('EXITFILTER'))
        SubmitClickConfirm('EXITFILTER','', true, '')
      event.keyCode=0
      event.returnValue=false
    }
  }
}

function DuplKeyGo(e)
{
  if (document.layers)
  {
  }
  else if (document.all)
  {
  }
}

document.onkeydown=FastKeyGo
document.onkeypress=DuplKeyGo 
 
