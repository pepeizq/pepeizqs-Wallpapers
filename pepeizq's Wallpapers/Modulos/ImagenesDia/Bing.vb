Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json

Module Bing

    Public Async Sub Generar()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US"))

        If Not html = Nothing Then
            Dim fondo As BingFondo = JsonConvert.DeserializeObject(Of BingFondo)(html)

            If Not fondo Is Nothing Then
                Dim enlace As String = fondo.Datos(0).Enlace

                If Not enlace.Contains("https://www.bing.com") Then
                    enlace = "https://www.bing.com" + enlace
                End If

                Dim imagenFondo As ImageEx = pagina.FindName("imagenFondo")
                imagenFondo.Source = New Uri(enlace)

                If Not fondo.Datos(0).Titulo Is Nothing Then
                    Dim tbTitulo As TextBlock = pagina.FindName("tbTituloFondo")
                    tbTitulo.Text = fondo.Datos(0).Titulo
                    tbTitulo.Visibility = Visibility.Visible
                End If

                If Not fondo.Datos(0).Copyright Is Nothing Then
                    Dim tbDescripcion As TextBlock = pagina.FindName("tbDescripcionFondo")
                    tbDescripcion.Text = fondo.Datos(0).Copyright
                    tbDescripcion.Visibility = Visibility.Visible
                End If
            End If
        End If

    End Sub

End Module

Public Class BingFondo

    <JsonProperty("images")>
    Public Datos As List(Of BingFondoDatos)

End Class

Public Class BingFondoDatos

    <JsonProperty("title")>
    Public Titulo As String

    <JsonProperty("url")>
    Public Enlace As String

    <JsonProperty("copyright")>
    Public Copyright As String

End Class
