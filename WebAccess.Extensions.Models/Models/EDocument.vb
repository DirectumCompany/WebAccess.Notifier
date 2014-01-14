Namespace Models
  <DataContract>
  Public Class EDocument
    Inherits Models.BaseObject

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(EDocument As API.EDocument)
      MyBase.New(EDocument)
    End Sub
  End Class
End Namespace
