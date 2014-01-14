Namespace Models
  <DataContract>
  Public Class DetailRecord
    <DataMember()>
    Public Number As Integer
    <DataMember()>
    Public Requisites As List(Of Models.Requisite)

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(DetailRecord As API.DetailRecord)
      MyBase.New()
      Me.Number = DetailRecord.Number
      Me.Requisites = (From Requisite In DetailRecord.Requisites
                      Select New Models.Requisite(Requisite)).ToList()

    End Sub
  End Class
End Namespace
