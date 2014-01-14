Imports System.Xml.Serialization

Namespace Models
  <XmlInclude(GetType(Models.Folder))>
  <XmlInclude(GetType(Models.ReferenceRecord))>
  <XmlInclude(GetType(Models.Task))>
  <XmlInclude(GetType(Models.Job))>
  <XmlInclude(GetType(Models.EDocument))>
  <DataContract>
  Public Class BaseObject
    <DataMember()>
    Public Title As String
    <DataMember()>
    Public Created As String
    <DataMember()>
    Public Modified As String
    <DataMember()>
    Public Author As Models.User
    <DataMember()>
    Public ID As Integer
    <DataMember()>
    Public Type As String
    <DataMember()>
    Public SubType As String

    ''' <remarks>Protected для JSON сериализации</remarks>
    Protected Friend Sub New()
      MyBase.New()
    End Sub


    Public Sub New(DataObject As API.BaseDataObject)
      'Общие данные
      Me.Title = DataObject.Name
      Me.Created = Utils.DateToString(DataObject.Created)
      Me.Modified = Utils.DateToString(DataObject.Modified)
      Me.ID = DataObject.ID
      Me.Type = DataObject.Type.ToString().ToLower()
      If DataObject.Author IsNot Nothing Then Me.Author = New Models.User(DataObject.Author)

      Select Case DataObject.Type
        Case ObjectType.Job
          Me.Title = CType(DataObject, API.Job).Subject
          Me.SubType = CType(DataObject, API.Job).Kind.ToString().ToLower()
        Case ObjectType.Task
          Me.Title = CType(DataObject, API.Task).Subject
        Case ObjectType.EDocument
          'Для вывода CSS          
          'Me.SubType = CType(DataObject, API.EDocument).Editor.Extension.ToString().ToLower()
        Case ObjectType.Folder
          Me.SubType = CType(DataObject, API.Folder).ContentType.ToString().ToLower()
      End Select
    End Sub

    Shared Function GetObject(DataObject As API.BaseDataObject, Optional IsBase As Boolean = False) As Models.BaseObject
      Dim obj As Models.BaseObject
      Select Case DataObject.Type
        Case ObjectType.EDocument
          obj = New Models.EDocument(DataObject)
        Case ObjectType.ReferenceRecord
          obj = New Models.ReferenceRecord(DataObject)
        Case ObjectType.Folder
          obj = New Models.Folder(DataObject)
        Case ObjectType.Job
          If IsBase Then
            obj = New Models.JobBase(DataObject)
          Else
            obj = New Models.Job(DataObject)
          End If
        Case ObjectType.Task
          If IsBase Then
            obj = New Models.TaskBase(DataObject)
          Else
            obj = New Models.Task(DataObject)
          End If
        Case Else
          obj = New Models.BaseObject(DataObject)
      End Select

      Return obj
    End Function

  End Class
End Namespace

