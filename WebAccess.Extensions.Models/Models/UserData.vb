Imports System.Xml.Serialization

Namespace Models
  <DataContract>
  Public Class UserData
    Public User As Models.User
    Public Folders As Models.UserFolders
    Public QuickLaunch As Models.Folder
    Public Avatar As Byte()

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub
    ''' <remarks>Protected для JSON сериализации</remarks>
    Public Sub New(CurrentUser As API.User)
      Me.User = New Models.User(CurrentUser)
    End Sub


  End Class

  <DataContract>
  Public Class UserFolders
    Public Inbox As Integer
    Public Outbox As Integer
    Public Shortcuts As Integer
    Public Favorites As Integer
    Public UserFolder As Integer
    Public CommonFolder As Integer

  End Class
End Namespace
