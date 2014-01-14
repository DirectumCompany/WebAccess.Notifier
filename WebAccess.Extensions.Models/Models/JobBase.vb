Imports System.Xml
Imports System.IO
Imports System.Xml.Xsl
Imports System.Web.Hosting

Namespace Models
  <DataContract>
  Public Class JobBase
    Inherits Models.BaseObject
    <DataMember()>
    Public IsRead As Boolean
    <DataMember()>
    Public IsSigned As Boolean
    <DataMember()>
    Public IsStandardRouted As Boolean
    <DataMember()>
    Public IsExpired As Boolean
    <DataMember()>
    Public Important As Boolean
    <DataMember()>
    Public State As String
    <DataMember()>
    Public Deadline As String


    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(Job As API.Job)
      MyBase.New(Job)
      Me.Important = (Job.Importance = CustomWork.WorkImportance.High)
      Me.IsRead = Job.IsRead
      Me.IsSigned = Job.IsSigned


      Me.IsStandardRouted = Job.IsStandardRouted
      Me.State = Job.JobState.ToString().ToLower()
      Me.Deadline = Utils.DateToString(Job.Deadline)

      IsExpired = Job.JobState = CustomWork.WorkState.Running AndAlso Job.Deadline <> Date.MinValue
    End Sub


  End Class
End Namespace