Namespace Models
  <DataContract>
  Public Class ReferenceRecord
    Inherits Models.BaseObject
    <DataMember()>
    Public ReferenceName As String
    <DataMember()>
    Public Requisites As List(Of Models.Requisite)
    <DataMember()>
    Public Details As List(Of Models.DetailSection)

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(ReferenceRecord As API.ReferenceRecord, Optional Extended As Boolean = False)
      MyBase.New(ReferenceRecord)
      Me.ReferenceName = ReferenceRecord.ParentReference.Name

      If Extended Then
        Me.Requisites = (From Requisite In ReferenceRecord.Requisites Select New Models.Requisite(Requisite)).ToList()
        'Me.Details = (From DetailSection In ReferenceRecord.Details
        '              Where DetailSection IsNot Nothing
        '              Select New Models.DetailSection(DetailSection)).ToList()
      End If
    End Sub
  End Class
End Namespace