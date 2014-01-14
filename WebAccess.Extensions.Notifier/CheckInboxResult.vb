Imports System.Runtime.Serialization

<DataContract()>
Public Class CheckInboxResult
  <DataMember()>
  Public UnreadJobCount As Integer
  <DataMember()>
  Public NewJobs As List(Of Models.JobBase)

  ''' <summary></summary>
  ''' <remarks>Protected для JSON сериализации</remarks>
  Protected Friend Sub New()
    MyBase.New()
  End Sub

End Class
