Imports System.Xml
Imports System.IO
Imports System.Xml.Xsl
Imports System.Web.Hosting


Namespace Models
  <DataContract>
  Public Class PerformResult
    <DataMember()>
    Public IsAbort As Boolean
    <DataMember()>
    Public IsHidden As Boolean
    <DataMember()>
    Public Code As String
    <DataMember()>
    Public DefaultDisplayText As String
    <DataMember()>
    Public DefaultText As String
    <DataMember()>
    Public DisplayName As String
    <DataMember()>
    Public Name As String
    <DataMember()>
    Public AskableParams As List(Of Models.AskableParam)

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New(PerformResult As API.PerformResult)
      MyBase.New()
      Me.Code = PerformResult.Code
      Me.DefaultDisplayText = PerformResult.DefaultDisplayText
      Me.DefaultText = PerformResult.DefaultText
      Me.DisplayName = PerformResult.DisplayName
      Me.IsAbort = PerformResult.IsAbort
      Me.IsHidden = PerformResult.IsHidden
      Me.Name = PerformResult.Name
      'Me.AskableParams = PerformResult.AskableParams.Select(Function(x) New Models.AskableParam(x))
    End Sub

  End Class
End Namespace
