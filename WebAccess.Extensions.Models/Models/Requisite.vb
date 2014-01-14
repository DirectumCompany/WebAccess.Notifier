Imports System.Xml.Serialization

Namespace Models
  <DataContract>
  Public Class Requisite
    <DataMember()>
    Public Name As String
    <DataMember()>
    Public Value As String
    <DataMember()>
    Public DisplayValue As String
    <DataMember()>
    Public Type As String
    <DataMember()>
    Public ReferenceName As String


    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(Requisite As API.Requisite)
      MyBase.New()
      Me.DisplayValue = Requisite.DisplayValue
      Me.Name = Requisite.Name      
      Me.Type = Requisite.Type.ToString()

      If Not Requisite.IsNull Then
        Select Case Requisite.Type
          Case RequisiteDescription.RequisiteType.Reference
            Me.ReferenceName = Requisite.ReferenceRecord.Name
          Case RequisiteDescription.RequisiteType.Date
            Me.Value = Utils.DateToString(Requisite.Value)
          Case RequisiteDescription.RequisiteType.Text
            Me.Value = Convert.ToBase64String(Requisite.AsBytes())
          Case Else
            Me.Value = Requisite.Value
        End Select
      End If
    End Sub

  End Class

End Namespace
