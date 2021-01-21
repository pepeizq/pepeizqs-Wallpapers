Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json

Module Nasa

    Public Async Sub Generar()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://api.nasa.gov/planetary/apod?api_key=Uqit7gyg7GiUGHf2pNclhJeKFCBZyFSS4Uc7qbSB"))

        If Not html = Nothing Then
            Dim fondo As NasaFondo = JsonConvert.DeserializeObject(Of NasaFondo)(html)

            If Not fondo Is Nothing Then
                Dim imagenFondo As ImageEx = pagina.FindName("imagenFondo")
                imagenFondo.Source = fondo.Enlace2

                If Not fondo.Titulo Is Nothing Then
                    Dim tbTitulo As TextBlock = pagina.FindName("tbTituloFondo")
                    tbTitulo.Text = fondo.Titulo
                    tbTitulo.Visibility = Visibility.Visible
                End If

                If Not fondo.Detalles Is Nothing Then
                    Dim tbDescripcion As TextBlock = pagina.FindName("tbDescripcionFondo")
                    tbDescripcion.Text = fondo.Detalles
                    tbDescripcion.Visibility = Visibility.Visible
                End If
            End If
        End If

    End Sub

End Module

Public Class NasaFondo

    <JsonProperty("title")>
    Public Titulo As String

    <JsonProperty("hdurl")>
    Public Enlace As String

    <JsonProperty("explanation")>
    Public Detalles As String

    <JsonProperty("url")>
    Public Enlace2 As String

End Class
