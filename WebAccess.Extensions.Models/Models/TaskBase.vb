Imports System.Xml
Imports System.IO
Imports System.Xml.Xsl
Imports System.Web.Hosting

Namespace Models
  <DataContract>
  Public Class TaskBase
    Inherits Models.BaseObject

    <DataMember()>
    Public IsSigned As Boolean
    <DataMember()>
    Public IsStandardRouted As Boolean
    <DataMember()>
    Public Important As Boolean
    <DataMember()>
    Public Deadline As String
    <DataMember()>
    Public State As String

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(Task As API.Task)
      MyBase.New(Task)
      Me.Important = (Task.Importance = CustomWork.WorkImportance.High)
      Me.IsSigned = Task.IsSigned
      State = Task.State.ToString()

      Me.IsStandardRouted = Task.IsStandardRouted
      Me.Deadline = Utils.DateToString(Task.Deadline)
      Me.Modified = Utils.DateToString(Task.Modified)

    End Sub

  End Class
End Namespace