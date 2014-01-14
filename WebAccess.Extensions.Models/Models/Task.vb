Imports System.Xml
Imports System.IO
Imports System.Xml.Xsl
Imports System.Web.Hosting

Namespace Models
  <DataContract>
  Public Class Task
    Inherits Models.TaskBase
    <DataMember()>
    Public WorkTree As String
    <DataMember()>
    Public Initiator As Models.User
    <DataMember()>
    Dim LeaderJobID As Integer
    <DataMember()>
    Dim LeaderTaskID As Integer
    <DataMember()>
    Dim MainTaskID As Integer
    <DataMember()>
    Public Attachments As List(Of Models.BaseObject)
    <DataMember()>
    Public Instruction As String
    <DataMember()>
    Public WorkHTML As String

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub


    Public Sub New(Task As API.Task)
      MyBase.New(Task)

      Me.LeaderJobID = Task.LeaderJobID
      Me.LeaderTaskID = Task.LeaderTaskID
      Me.MainTaskID = Task.MainTaskID
      Me.Initiator = New Models.User(Task.Initiator)

      Me.Instruction = Task.Instruction
      Me.WorkTree = Task.WorkTree.DocumentElement.OuterXml
      Me.WorkHTML = Utils.GetText(Task.ID, Task.WorkTree)

      Me.Attachments = (From Attachment In Task.Attachments
                       Select Models.BaseObject.GetObject(Attachment)).ToList()
    End Sub

  End Class
End Namespace