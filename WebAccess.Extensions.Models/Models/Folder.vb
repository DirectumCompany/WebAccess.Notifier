Namespace Models
  <DataContract>
  Public Class Folder
    Inherits Models.BaseObject
    <DataMember()>
    Public UnreadJobCount As Integer
    <DataMember()>
    Public Content As List(Of Models.BaseObject)

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(Folder As API.Folder, Optional FillContent As Boolean = False)
      MyBase.New(Folder)
      'Me.UnreadJobCount = Folder.UnreadJobCount
      'TODO: заполнять
      'If FillContent Then Content = Nothing
    End Sub
  End Class
End Namespace