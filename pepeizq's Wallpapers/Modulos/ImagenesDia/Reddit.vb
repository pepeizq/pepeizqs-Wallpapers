Imports System.Net
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json

Module Reddit

    Public Async Sub Generar(subEnlace As String)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://www.reddit.com/r/" + subEnlace + "/.json"))

        If Not html = Nothing Then
            Dim fondo As RedditFondo = JsonConvert.DeserializeObject(Of RedditFondo)(html)

            If Not fondo Is Nothing Then
                Dim post As Integer = 0

                Dim i As Integer = 0
                While i < 10
                    If Not fondo.Datos.Hijos(i).Datos.Enlace = "image" Then
                        post = i + 1
                    Else
                        Exit While
                    End If
                    i += 1
                End While

                Dim imagenFondo As ImageEx = pagina.FindName("imagenFondo")
                imagenFondo.Source = fondo.Datos.Hijos(post).Datos.Enlace

                If Not fondo.Datos.Hijos(post).Datos.Titulo Is Nothing Then
                    Dim tbTitulo As TextBlock = pagina.FindName("tbTituloFondo")
                    tbTitulo.Text = WebUtility.HtmlDecode(fondo.Datos.Hijos(post).Datos.Titulo)
                    tbTitulo.Visibility = Visibility.Visible
                End If
            End If
        End If

    End Sub

End Module

Public Class RedditFondo

    <JsonProperty("data")>
    Public Datos As RedditFondoDatos

End Class

Public Class RedditFondoDatos

    <JsonProperty("children")>
    Public Hijos As List(Of RedditFondoDatosHijos)

End Class

Public Class RedditFondoDatosHijos

    <JsonProperty("data")>
    Public Datos As RedditFondoDatosHijosContenido

End Class

Public Class RedditFondoDatosHijosContenido

    <JsonProperty("title")>
    Public Titulo As String

    <JsonProperty("url")>
    Public Enlace As String

    <JsonProperty("post_hint")>
    Public Tipo As String

End Class
