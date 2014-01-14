Namespace Models
  <DataContract>
  Public Class User
    <DataMember()>
    Public ID As Integer
    <DataMember()>
    Public FullName As String
    <DataMember()>
    Public Name As String
    <DataMember()>
    Public Code As String

    '<DataMember()>
    'Public Login As String
    '<DataMember()>
    'Public ComponentID As Integer
    '<DataMember()>
    'Public IsClosed As Boolean
    '<DataMember()>
    'Public IsGroup As Boolean

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub

    Public Sub New(User As API.User)
      If User Is Nothing Then Throw New NullReferenceException
      Me.ID = User.ID
      Me.FullName = User.FullName
      Me.Code = User.Code
      Me.Name = User.Name

      'Me.ComponentID = User.ComponentID
      'Me.IsClosed = User.IsClosed
      'Me.IsGroup = User.IsGroup
      'Me.Login = User.Login
    End Sub

  End Class
End Namespace
