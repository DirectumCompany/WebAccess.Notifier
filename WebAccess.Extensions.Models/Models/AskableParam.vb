
Namespace Models
  <DataContract>
  Public Class AskableParam
    <DataMember()>
    Public IsAbort As Boolean
    <DataMember()>
    Public Caption As String
    <DataMember()>
    Public Hint As String
    <DataMember()>
    Public IsCollection As Boolean
    <DataMember()>
    Public IsRequired As Boolean
    <DataMember()>
    Public Name As String
    <DataMember()>
    Public Type As String
    <DataMember()>
    Public Value As Object
    <DataMember()>
    Public Values As List(Of Object)

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New(AskableParam As API.AskableParam)
      MyBase.New()
      Me.Caption = AskableParam.Caption
      Me.Hint = AskableParam.Hint
      Me.IsCollection = AskableParam.IsCollection
      Me.IsRequired = AskableParam.IsRequired
      Me.Name = AskableParam.Name
      Me.Type = AskableParam.Type.ToString()
      Me.Value = AskableParam.Value
      Me.Values = AskableParam.Values
    End Sub
  End Class
End Namespace
