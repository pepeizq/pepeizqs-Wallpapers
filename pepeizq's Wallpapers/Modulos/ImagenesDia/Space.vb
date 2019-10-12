Imports Microsoft.Toolkit.Uwp.UI.Controls

Module Space

    Public Async Sub Generar()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://www.space.com/34-image-day.html"))

        If Not html = Nothing Then
            If html.Contains(ChrW(34) + "og:image" + ChrW(34)) Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = html.IndexOf(ChrW(34) + "og:image" + ChrW(34))
                temp = html.Remove(0, int + 1)

                int = temp.IndexOf("content=" + ChrW(34))
                temp = temp.Remove(0, int + 9)

                int2 = temp.IndexOf(ChrW(34))
                temp2 = temp.Remove(int2, temp.Length - int2)

                Dim enlace As String = temp2.Trim

                Dim imagenFondo As ImageEx = pagina.FindName("imagenFondo")
                imagenFondo.Source = New Uri(enlace)
            End If
        End If

    End Sub

End Module
