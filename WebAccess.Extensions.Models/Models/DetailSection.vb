Namespace Models
  <DataContract>
  Public Class DetailSection
    <DataMember()>
    Public Number As Integer
    <DataMember()>
    Public Records As List(Of Models.DetailRecord)

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(DetailSection As API.DetailSection)
      MyBase.New()
      Me.Number = DetailSection.SectionNumber
      Me.Records = (From Record In DetailSection.Records Select New Models.DetailRecord(Record)).ToList()
    End Sub
  End Class
End Namespace
