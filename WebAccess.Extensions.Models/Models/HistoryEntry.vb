Namespace Models
  <DataContract>
  Public Class HistoryEntry
    <DataMember()>
    Public ActionDate As String
    <DataMember()>
    Public ActionText As String
    <DataMember()>
    Public Detail As String
    <DataMember()>
    Public HostName As String
    <DataMember()>
    Public ID As Integer
    <DataMember()>
    Public Number As Integer
    <DataMember()>
    Public User As Models.User

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(HistoryEntry As API.HistoryEntry)
      Me.ActionDate = Utils.DateToString(HistoryEntry.ActionDate)
      Me.ActionText = HistoryEntry.ActionText
      Me.Detail = HistoryEntry.Detail
      Me.HostName = HistoryEntry.HostName
      Me.ID = HistoryEntry.ID
      Me.Number = HistoryEntry.Number
      Me.User = New Models.User(HistoryEntry.User)
    End Sub
  End Class
End Namespace
