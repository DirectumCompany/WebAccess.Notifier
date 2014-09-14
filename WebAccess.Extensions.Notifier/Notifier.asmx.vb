Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports NpoComputer.WebAccess
Imports NpoComputer.WebAccess.API
Imports System.Collections.Generic
Imports System.Threading
Imports System.Globalization

<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://directum.ru/WebAccess/Notifier")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Service
  Inherits System.Web.Services.WebService

  ''' <summary>Метод закрытия сессии</summary>
  ''' <remarks></remarks>
  <WebMethod()> _
  Public Sub Logout()
    If WAAPIEntry.Context IsNot Nothing Then WAAPIEntry.Context.Logout()
  End Sub


  ''' <summary>Проверка наличия новых заданий</summary>
  ''' <param name="LastUpdate">Дата последней проверки</param>
  ''' <returns>Количестко непрочтенных заданий + список объектов Заданий</returns>
  <WebMethod()> _
  Public Function CheckInbox(LastUpdate As Date) As CheckInboxResult
    Dim Inbox As API.Folder = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.Inbox)
    Dim NewJobs As List(Of Models.JobBase)
    Dim Expression As String
    Dim Culture As Globalization.CultureInfo = SBRSE.SessionManager.InternalCulture
    SetupLanguage()
    WAAPIEntry.Context.Session("KeepAlive") = (New Random).Next(0, 10000)
    WAAPIEntry.Context.UpdateClientActivity()

    LastUpdate = LastUpdate.AddSeconds(-LastUpdate.Second).ToLocalTime()

    Expression = String.Format("[StartDate] > #{0}# & [{1}] = {2} & [{3}] = ""{4}""",
                                             LastUpdate.ToString(SBRSE.SessionManager.InternalCulture.DateTimeFormat.FullDateTimePattern, SBRSE.SessionManager.InternalCulture),
                                             SBRSE.Constants.CustomWorkRequisites.REQ_JOB_PERFORMER, WAAPIEntry.Context.CurrentUser.ID,
                                             SBRSE.Constants.CustomWorkRequisites.REQ_JOB_READED, "N")

    Log.LogInfo("Notifier: Получение новых входящих от {0} для {1}", LastUpdate.ToString(Culture.DateTimeFormat.FullDateTimePattern), WAAPIEntry.Context.CurrentUser.FullName)
    NewJobs = WAAPIEntry.Context.Jobs.GetJobs(Expression).Select(Function(x) New Models.JobBase(x)).ToList()
    Return New CheckInboxResult() With {.UnreadJobCount = Inbox.UnreadJobCount, .NewJobs = NewJobs}
  End Function





  ''' <summary>Запрос содержимого папки</summary>
  ''' <param name="FolderType">Тип или ИД папки</param>
  ''' <param name="ContentType">Тип содержимого</param>
  ''' <returns></returns>
  ''' <remarks>В качестве параметра "FolderType" может быть указан ИД папки или код предопределенной папки "inbox", "outbox", "shortcuts", "favorites". </remarks>
  <WebMethod()> _
  Public Function GetContent(FolderType As String, ContentType As String, Top As Integer, Skip As Integer) As List(Of Models.BaseObject)
    Dim Content As List(Of Models.BaseObject)
    Dim Folders As IEnumerable(Of API.BaseDataObject) = {}
    Dim Contents As IEnumerable(Of BaseDataObject) = {}
    Dim ObjectType As ObjectType = ObjectType.Any
    Dim FolderID As Integer
    SetupLanguage()

    If String.IsNullOrEmpty(ContentType) Then
      ObjectType = API.ObjectType.Any
    Else
      ObjectType = GetObjectType(ContentType)
    End If

    Select Case FolderType.ToLower()
      Case "inbox"
        Folders = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.Inbox).Contents(Folder.FolderContentType.Folder, Nothing, Nothing, SortCriteria.Name, Nothing, SortDirection.Ascending)
        Contents = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.Inbox).Contents(Folder.FolderContentType.Job, Nothing, Nothing, SortCriteria.Created, Nothing, SortDirection.Descending)
      Case "outbox"
        Folders = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.Outbox).Contents(Folder.FolderContentType.Folder, Nothing, Nothing, SortCriteria.Name, Nothing, SortDirection.Ascending)
        Contents = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.Outbox).Contents(Folder.FolderContentType.Task, Nothing, Nothing, SortCriteria.Created, Nothing, SortDirection.Descending)
      Case "shortcuts"
        Contents = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.Shortcuts).Contents(ObjectType, Nothing, Nothing, SortCriteria.Type, Nothing, SortDirection.Ascending)
      Case "favorites"
        Contents = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.Favorites).Contents(ObjectType, Nothing, Nothing, SortCriteria.Type, Nothing, SortDirection.Ascending)
      Case "userfolder"
        Contents = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.UserFolder).Contents(ObjectType, Nothing, Nothing, SortCriteria.Type, Nothing, SortDirection.Ascending)
      Case "commonfolder"
        Contents = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.CommonFolder).Contents(ObjectType, Nothing, Nothing, SortCriteria.Type, Nothing, SortDirection.Ascending)
      Case Else
        If Integer.TryParse(FolderType, FolderID) Then
          Dim Folder As API.Folder
          Folder = WAAPIEntry.Context.Folders.GetFolderByID(FolderID)
          If Folder IsNot Nothing Then
            If Folder.IsSearch Then              
              Contents = WAAPIEntry.Context.Folders.GetFolderByID(FolderID).Contents(Folder.ContentType, Nothing, Nothing, SortCriteria.Type, Nothing, SortDirection.Ascending)
            Else
              Contents = WAAPIEntry.Context.Folders.GetFolderByID(FolderID).Contents(ObjectType, Nothing, Nothing, SortCriteria.Type, Nothing, SortDirection.Ascending)
            End If
          End If
        Else
          Contents = WAAPIEntry.Context.Folders.RootFolder(API.Folder.SpecFolderType.Inbox).Contents(Folder.FolderContentType.Job, Nothing, Nothing, SortCriteria.Created, Nothing, SortDirection.Descending)
          Contents = (From Obj In Contents Where Not CType(Obj, Job).IsRead).AsQueryable
        End If
    End Select

    Content = (From Folder In Folders Select Models.BaseObject.GetObject(Folder, True)).ToList
    Content.AddRange((From Obj In Contents Select Models.BaseObject.GetObject(Obj, True)).ToList)
    If Top > 0 Then Content = Content.Skip(Skip).Take(Top).ToList()
    Return Content
  End Function

  Private Function GetObjectType(ObjectType As String) As API.ObjectType
    If String.IsNullOrEmpty(ObjectType) Then Return API.ObjectType.Unknown

    Select Case ObjectType.ToLower
      Case "edocument"
        Return API.ObjectType.EDocument
      Case "folder"
        Return API.ObjectType.Folder
      Case "job"
        Return API.ObjectType.Job
      Case "referencerecord"
        Return API.ObjectType.ReferenceRecord
      Case "task"
        Return API.ObjectType.Task
      Case "any"
        Return API.ObjectType.Any
      Case Else
        Throw New System.NotSupportedException
        'Return API.ObjectType.Unknown
    End Select
  End Function

  ''' <summary>
  ''' Устанавливает параметры языка в контексте.
  ''' </summary>
  ''' <remarks></remarks>
  Protected Sub SetupLanguage()
    If WAAPIEntry.Context Is Nothing Then
      Dim CurrentCultureInfo As CultureInfo = CultureInfo.CreateSpecificCulture("en-US")

      ' Для английской базы устанавливаем всегда язык английский.
      If API.Application.Language <> API.ApplicationLanguage.English Then
        CurrentCultureInfo = CultureInfo.CreateSpecificCulture(API.Application.Settings.DefaultLanguage)
      End If

      ' Если контекста нет, то просто означим культуру потока.
      Thread.CurrentThread.CurrentCulture = CurrentCultureInfo
      Thread.CurrentThread.CurrentUICulture = CurrentCultureInfo
    Else
      WAAPIEntry.SetupLanguage()
    End If
  End Sub
End Class
