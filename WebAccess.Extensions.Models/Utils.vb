Imports System.Xml
Imports System.IO
Imports System.Xml.Xsl
Imports System.Text
Imports System.Web
Imports System.Web.Hosting

Public Class Utils
  Private Shared WorkTextXSLTTemplate As XslCompiledTransform
  Private Shared Locker As New Object

  Friend Shared Function DateToString(oDate As Date) As String
    If oDate = Date.MinValue Then Return Nothing
    'Dim DateOffset As New DateTimeOffset(oDate, TimeZoneInfo.Local.GetUtcOffset(oDate))
    'Return DateOffset.ToString("o")
    Return oDate.ToUniversalTime.ToString("yyyy-MM-ddTHH:mm:ss.ssZ")
  End Function


  Friend Shared Function GetText(WorkID As Integer, CustomWorkTree As System.Xml.XmlDocument, Optional IsShowFullText As Boolean = True) As String
    Dim TaskText As String = ""
    Dim CurrentCustomWorkNode As XmlNode
    Dim TaskXmlTextReader As XmlReader
    Dim TaskXmlText As String
    Dim TaskTextSB As StringBuilder
    Dim TaskTextWriter As XmlWriter

    Try

      If IsShowFullText Then
        TaskXmlText = CustomWorkTree.DocumentElement.OuterXml
      Else
        ' Удаляем под задачи/задания.
        CurrentCustomWorkNode = CustomWorkTree.SelectSingleNode(String.Format("//Item[@ID={0}]", WorkID))
        ' Если у текущей ЗЗУ нет текста, то отобразим текст родителя.
        If CurrentCustomWorkNode.SelectNodes("Item[@Type='Text']").Count = 0 Then
          CurrentCustomWorkNode = CurrentCustomWorkNode.ParentNode
        End If
        For Each Node As XmlNode In CurrentCustomWorkNode.SelectNodes(String.Format("Item[(@Type='Task' or @Type='Job' or @Type='ControlJob' or @Type='Notice') and @ID!='{0}']", WorkID))
          CurrentCustomWorkNode.RemoveChild(Node)
        Next
        TaskXmlText = CurrentCustomWorkNode.OuterXml
      End If

      TaskXmlTextReader = XmlReader.Create(New StringReader(TaskXmlText))
      TaskTextSB = New StringBuilder()
      Dim writerSettings As XmlWriterSettings = New XmlWriterSettings()
      writerSettings.OmitXmlDeclaration = True
      TaskTextWriter = XmlWriter.Create(New StringWriter(TaskTextSB), writerSettings)


      If WorkTextXSLTTemplate Is Nothing Then
        SyncLock Locker
          If WorkTextXSLTTemplate Is Nothing Then
            WorkTextXSLTTemplate = New XslCompiledTransform
            ' Загрузим шаблон, если еще не загружали, и выполним преобразование.
            Dim TmplName As String
            TmplName = String.Format("~/App_Data/Android/ISBTaskTree_{0}.xsl", WAAPIEntry.Context.CurrentCultureInfo.TwoLetterISOLanguageName)
            WorkTextXSLTTemplate.Load(HostingEnvironment.MapPath(TmplName))
          End If
        End SyncLock
      End If

      Using TaskTextWriter
        ' Выполняем преобразование.
        WorkTextXSLTTemplate.Transform(TaskXmlTextReader, Nothing, TaskTextWriter)
      End Using

      TaskText = TaskTextSB.ToString
    Catch ex As Exception
      Log.LogException(ex)
    End Try

    Return TaskText
  End Function
End Class
