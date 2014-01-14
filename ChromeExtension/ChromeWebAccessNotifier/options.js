var isPasswordChanged = false;
window.addEventListener('load', function () {
  options.isActivated.checked = JSON.parse(localStorage.isActivated);
  options.frequency.value = localStorage.frequency;
  options.URL.value = localStorage.URL;
  options.login.value = localStorage.login;
  if (localStorage.theme) options.theme.value = localStorage.theme;  
  if (localStorage.password != null && localStorage.password != "") {
    options.password.placeholder = "Введите чтобы изменить";
  }

  options.isDrillDown.checked = JSON.parse(localStorage.isDrillDown);
  options.isAutoLogin.checked = JSON.parse(localStorage.isAutoLogin);
  options.login.disabled = !options.isAutoLogin.checked;
  options.password.disabled = !options.isAutoLogin.checked;
  options.check.disabled = !options.isAutoLogin.checked;
  options.clean.disabled = !options.isAutoLogin.checked;

  options.isActivated.onchange = function () {
    var isActivated = options.isActivated.checked;
    localStorage.isActivated = isActivated;
    options.frequency.disabled = !isActivated;
    notification.style.color = isActivated ? '' : 'graytext';
  };

  options.frequency.onchange = function () {
    localStorage.frequency = options.frequency.value;
    clearInterval(localStorage.setInterval);
    chrome.extension.getBackgroundPage().runNotifier();
  };

  options.theme.onchange = function () {
    localStorage.theme = options.theme.value;
  };


  options.URL.onchange = function () {
    document.getElementById("alert_https").style.display = "none";
    document.getElementById("alert_http").style.display = "none";

    if (options.URL.value.indexOf("http://") != -1) document.getElementById("alert_http").style.display = "block"; 
    if (options.URL.value.indexOf("https://") != -1) document.getElementById("alert_https").style.display = "block";
    localStorage.URL = options.URL.value;
  };

  options.login.onchange = function () {
    localStorage.login = options.login.value;
  };

  options.password.onchange = function () {
    localStorage.password = options.password.value;
    isPasswordChanged = true;
  };

  options.isDrillDown.onchange = function () {
    localStorage.isDrillDown = options.isDrillDown.checked;
  };

  options.isAutoLogin.onchange = function () {
    options.login.disabled = !options.isAutoLogin.checked;
    options.password.disabled = !options.isAutoLogin.checked;
    options.check.disabled = !options.isAutoLogin.checked;
    options.clean.disabled = !options.isAutoLogin.checked;
    localStorage.isAutoLogin = options.isAutoLogin.checked;
  };


  options.clean.onclick = function () {
    options.login.value = "";
    options.password.value = "";
    options.password.placeholder = "Пароль";
    localStorage.password = "";
    localStorage.login = "";
    return false;
  }
  options.check.onclick = function () {
    var url = options.URL.value;
    if (url.indexOf("http://") == -1 && url.indexOf("https://") == -1) options.URL.value = "https://" + url;
    localStorage.URL = options.URL.value;
    localStorage.serviceURL = localStorage.URL + '/Notifier.asmx/';
    localStorage.CheckInboxURL = localStorage.serviceURL + 'CheckInbox';
    localStorage.GetContentURL = localStorage.serviceURL + 'GetContent';
    localStorage.LoginURL = localStorage.URL + "/UserLogin.asmx/Login";
    localStorage.login = options.login.value;
    if(isPasswordChanged) localStorage.password = options.password.value;

    if (url.indexOf("http://") != -1) document.getElementById("alert_http").style.display = "block";
    if (url.indexOf("https://") != -1) document.getElementById("alert_https").style.display = "block";
    chrome.extension.getBackgroundPage().SetOnlineIcon(false);
    clearInterval(localStorage.setInterval);
    chrome.extension.getBackgroundPage().TryLogin(true);
    return false;
  }

});