Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json

Module NationalGeographic

    Public Async Sub Generar()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://www.nationalgeographic.com/photography/photo-of-the-day/_jcr_content/.gallery." + Date.Now.Year.ToString + "-" + Date.Now.Month.ToString + ".json"))

        If Not html = Nothing Then
            Dim fondo As NationalGeographicFondo = JsonConvert.DeserializeObject(Of NationalGeographicFondo)(html)

            If Not fondo Is Nothing Then
                Dim imagenFondo As ImageEx = pagina.FindName("imagenFondo")
                imagenFondo.Source = fondo.Datos(0).Imagen.Enlace

                If Not fondo.Datos(0).Imagen.Titulo Is Nothing Then
                    Dim tbTitulo As TextBlock = pagina.FindName("tbTituloFondo")
                    tbTitulo.Text = fondo.Datos(0).Imagen.Titulo
                    tbTitulo.Visibility = Visibility.Visible
                End If

                If Not fondo.Datos(0).Imagen.Descripcion Is Nothing Then
                    Dim descripcion As String = fondo.Datos(0).Imagen.Descripcion
                    descripcion = descripcion.Replace("<p>", Nothing)
                    descripcion = descripcion.Replace("</p>", Nothing)
                    descripcion = descripcion.Trim

                    Dim tbDescripcion As TextBlock = pagina.FindName("tbDescripcionFondo")
                    tbDescripcion.Text = descripcion
                    tbDescripcion.Visibility = Visibility.Visible
                End If
            End If
        End If

    End Sub

End Module

Public Class NationalGeographicFondo

    <JsonProperty("items")>
    Public Datos As List(Of NationalGeographicFondoDatos)

End Class

Public Class NationalGeographicFondoDatos

    <JsonProperty("image")>
    Public Imagen As NationalGeographicFondoDatosImagen

End Class

Public Class NationalGeographicFondoDatosImagen

    <JsonProperty("title")>
    Public Titulo As String

    <JsonProperty("alt_text")>
    Public Descripcion As String

    <JsonProperty("uri")>
    Public Enlace As String

End Class