Imports System.Xml
Imports System.IO
Imports System.Xml.Xsl
Imports System.Web.Hosting

Namespace Models
  <DataContract>
  Public Class Job
    Inherits Models.JobBase
    <DataMember()>
    Public WorkTree As String
    <DataMember()>
    Public MainTaskID As Integer
    <DataMember()>
    Public Performer As User
    <DataMember()>
    Public Initiator As Models.User
    <DataMember()>
    Public LeaderJobID As Integer
    <DataMember()>
    Public LeaderTaskID As Integer
    <DataMember()>
    Public PerformResults As List(Of Models.PerformResult)
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

    Public Sub New(Job As API.Job)
      MyBase.New(Job)

      Me.Initiator = New Models.User(Job.Initiator)
      Me.Performer = New Models.User(Job.Performer)
      Me.LeaderJobID = Job.LeaderJobID
      Me.LeaderTaskID = Job.LeaderTaskID
      Me.MainTaskID = Job.MainTaskID
      'Try
      Job.MarkAsRead()
      Me.Instruction = Job.Instruction
      Me.WorkTree = Job.WorkTree.DocumentElement.OuterXml
      Me.WorkHTML = Utils.GetText(Job.ID, Job.WorkTree)

      If Job.IsStandardRouted AndAlso Job.PerformResults IsNot Nothing AndAlso Job.PerformResults.Any() Then
        Me.PerformResults = Job.PerformResults.Select(Function(x) New Models.PerformResult(x)).ToList()
      End If
      Me.Attachments = Job.Attachments.Select(Function(x) Models.BaseObject.GetObject(x)).ToList()
      'Finally
      '  Job.GlobalLock.TryUnlock()
      'End Try
    End Sub
  End Class
End Namespace