var OpenTab = chrome.extension.getBackgroundPage().OpenTab;
var CallService = chrome.extension.getBackgroundPage().CallService;
var ShowPopup = chrome.extension.getBackgroundPage().ShowPopup;
var content = [];
var spinner = $('<img id="spinner" src="images/icon_64.png"/>');
var folderHistory = [];

$(function () {
  content = $("#content");
  if (localStorage.activeFolder == null) localStorage.activeFolder = "inbox";
  if (localStorage.isUnread == "true") $("#unread").attr("checked", "checked");
  $("#unread").bind("click", function () { localStorage.isUnread = $("#unread").is(":checked"); });
  $("nav li").bind("click", function () {
    $(".active").removeClass("active");
    $(this).addClass("active");
    var type = $(this).data("type");
    localStorage.activeFolder = type;
    if (type == "inbox" && $("#unread").is(":checked")) { type = "unread" }
    folderHistory = [];
    GetContent(type);
  });
  $("li[data-type=" + localStorage.activeFolder + "]").click();
  //if (localStorage.setInterval) {
  //  clearInterval(localStorage.setInterval);
  //  chrome.extension.getBackgroundPage().runNotifier();
  //}
});

function ItemClick(item) {
  if (item.Type == "back") { folderHistory.pop(); item.ID = folderHistory.pop(); GetContent(item.ID); return; }
  if (JSON.parse(localStorage.isDrillDown) && item.Type == "folder") GetContent(item.ID);
  else OpenTab(item);
}

function GetContent(type) {
  try {
    folderHistory.push(type);
    content.empty().append(spinner);
    CallService(localStorage.GetContentURL, { FolderType: type, ContentType: "Any", Top: 0, Skip: 0 },
      function (data) {
        list = $("<ul>");
        if (folderHistory.length > 1) {
          var link = $("<a title='Вверх' class='folder'>..</a>");
          var el = { Type: "back" };
          link.data("item", el).wrap("<li>");
          link.bind("click", function () { ItemClick($(this).data("item")); });
          list.append(link);
        };

        if (localStorage.theme != "32") {
          $(data.d).each(function (key, el) {
            var link = $("<a>");
            link.text(el.Title);
            link[0].className = el.Type;
            link[0].title = el.Title;
            if (!!(el.Deadline) && moment(new Date()).isAfter(new Date(el.Deadline))) link.addClass("expired");
            if (link.hasClass("job") && !el.IsRead) link.addClass("unread");
            link.addClass(el.SubType).addClass(el.State).addClass("theme16").data("item", el).wrap("<li>");
            link.bind("click", function () { ItemClick($(this).data("item")); });
            list.append(link);
          });
        } else {
          moment.lang('ru');
          $(data.d).each(function (key, el) {
            var link = $("<a>");
            //link.text(el.Title);
            link[0].className = el.Type;
            link[0].title = el.Title;
            if (link.hasClass("job") && !el.IsRead) link.addClass("unread");
            link.addClass(el.SubType).addClass(el.State).addClass("theme32").data("item", el).wrap("<li>");
            link.bind("click", function () { ItemClick($(this).data("item")); });
            link.append($("<span class='title'>").append(el.Title));
            var author;
            if (el.Author) author = $("<span>").append(el.Author.FullName).addClass("author");
            if (el.Deadline) {
              var date;
              var toDate = new Date(el.Deadline);
              var toDateClear = new Date(el.Deadline).setHours(0, 0, 0, 0);
              var dateTXT;

              if (moment(new Date()).isAfter(toDate)) {
                dateTXT = moment(toDate).fromNow();
                link.addClass("expired");
              } else {
                dateTXT = moment(toDate).calendar();
                if (+toDate == +toDateClear) dateTXT = dateTXT.replace(" в 00:00", "");
              }
              date = $("<span>").append(dateTXT).addClass("date");
              link.append($("<p>").append(author).append("&nbsp;|&nbsp;").append(date).addClass("secondline"))

            } else {
              link.append($("<p>").append(author).addClass("secondline"))
            }
            list.append(link);
          });
        }
        content.empty().append(list);
        if (data.d.length == 0) { content.append("<span id='empty'>Нет объектов для отображения</span>") }
      });

  } catch (s) {
    ShowPopup("Произошла ошибка", s.message);
  }
}