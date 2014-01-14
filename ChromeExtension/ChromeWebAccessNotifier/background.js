
if (localStorage.isInitialized == null) {
  localStorage.frequency = 1;
  localStorage.isActivated = false;
  localStorage.isInitialized = true;
  localStorage.isAutoLogin = false;
  localStorage.isDrillDown = true;
  localStorage.login = "";
  localStorage.password = "";
  localStorage.URL = "";
}

localStorage.lastUpdateTime = new Date();
localStorage.setInterval = 0;
localStorage.serviceURL = localStorage.URL + '/Notifier.asmx/';
localStorage.CheckInboxURL = localStorage.serviceURL + 'CheckInbox';
localStorage.GetContentURL = localStorage.serviceURL + 'GetContent';
localStorage.LoginURL = localStorage.URL + "/UserLogin.asmx/Login";
var page = {};
  page.edocument = "doc.asp";
  page.folder = "folder.asp";
  page.job = "job.asp";
  page.task = "task.asp";
  page.referencerecord = "reference.asp";
SetOnlineIcon(false);

if (window.webkitNotifications) runNotifier();
       
function runNotifier() {
  if (JSON.parse(localStorage.isActivated)) {
    checkInbox(localStorage.lastUpdateTime);
    localStorage.setInterval = setInterval(function () { checkInbox(localStorage.lastUpdateTime) }, localStorage.frequency * 60 * 1000);
  }
}

function checkInbox(lastUpdateTime) {
    chrome.browserAction.setBadgeText({ text: "" });
    CallService(localStorage.CheckInboxURL, { LastUpdate: (new Date(lastUpdateTime)).toISOString() }, function (data) {
      SetOnlineIcon(true);
      localStorage.lastUpdateTime = new Date();
      SetBadgeText(data.d.UnreadJobCount);
      $(data.d.NewJobs).each(function (key, value) {
        var notification = window.webkitNotifications.createNotification(
            "images/popup/" + value.SubType + '.png',
            value.Title,
            "Инициатор: " + value.Author.FullName
          );
        notification.ondisplay = function (event) { setTimeout(function () { event.currentTarget.cancel(); }, 10 * 1000); }
        notification.onclick = function () { OpenTab(value); this.cancel(); };
        notification.show();
      });
    });
}

function TryLogin(showDone) {
  var login = localStorage.login;
  var password = localStorage.password;
  var domain = "";
  var type = 2;
  
  if (login.split("\\").length == 2) {
    domain = login.split("\\")[0];
    login = login.split("\\")[1];
    type = 0;
  }
  
  CallService(localStorage.LoginURL, {
    Username: login,
    Password: password,
    UseWA: false,
    Domain: domain,
    Authentication: type,
    IsPassThroughAuth: false, RememberMe: true, LogonMsgDone: true,
    NeedTrySQLLogin: (type == 2)? true : false,
    IsAgentAvailable: true
  },
    function (data) {
    if (!data.d.Success) {
      ShowPopup("Произошла ошибка подключения", data.d.Error);
      SetOnlineIcon(false);
    } else {
      if (showDone != null && showDone) {
        $.ajax({
          type: "GET",
          url: localStorage.URL + '/Notifier.asmx',
          success: function () {
            SetOnlineIcon(true);
            ShowPopup("Проверка подключения", "Подключение установлено успешно");
            setTimeout(runNotifier(), 2000);          
          },
          async: true, cache: false,
          error: function (xhr, txtStat, errThrown) {
            SetOnlineIcon(false);
            if (xhr.status == 404) { ShowPopup("Произошла ошибка подключения", "Для веб-доступа расположенного по адресу '" + localStorage.URL+ "' не найдено установленное расширение нотификатора."); }
            if (xhr.status == 401) { ShowPopup("Произошла ошибка подключения", "Неправильная пара логин-пароль"); }
            else if (xhr.status != 401) ShowPopup("Ошибка!", errThrown);
          }
        });
      }
    }
  }
  );
}

function SetBadgeText(UnreadJobCount) {
  if (UnreadJobCount == null || UnreadJobCount == 0) chrome.browserAction.setBadgeText({ text: "" });
  else chrome.browserAction.setBadgeText({ text: UnreadJobCount.toString() });
}
function OpenTab(item) {
  var url = localStorage.URL + '/' + page[item.Type] + '?';
  if (item.ReferenceName != null) url += '&compcode=' + item.ReferenceName
  if (item.Type == "edocument") url += "&view=Preview";
  url += "&id=" + item.ID;
  chrome.tabs.create({ 'url': url }, function (tab) {
    setTimeout(function () { chrome.extension.getBackgroundPage().checkInbox(localStorage.lastUpdateTime) }, 2000);
  });
}

function ShowPopup(title, text, item) {
  var notification = window.webkitNotifications.createNotification("", title, text);
  notification.ondisplay = function (event) { setTimeout(function () { event.currentTarget.cancel(); }, 5 * 1000); };
  notification.onclick = function () { this.cancel(); };
  notification.show();
}

function SetOnlineIcon(isOnline) {
  SetBadgeText(0);
  if (isOnline) chrome.browserAction.setIcon({ path: "images/icon_16.png" });
  else chrome.browserAction.setIcon({ path: "images/icon_16_offline.png" });
}

function CallService(service_method, params, callback) {
  params = params || {};
  var error_handler = function () { if (error) error.apply(null, arguments); }
  $.ajax({
    type: "POST",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    url: service_method,
    data: JSON.stringify(params),
    success: callback,
    async: true, cache: false,    
    error: function (xhr, txtStat, errThrown) {      
      SetOnlineIcon(false);
      if (xhr.status == 401 && JSON.parse(localStorage.isAutoLogin)) { TryLogin(false); }
      else if (xhr.status != 401) ShowPopup("Ошибка!", errThrown);
    }
  });
}