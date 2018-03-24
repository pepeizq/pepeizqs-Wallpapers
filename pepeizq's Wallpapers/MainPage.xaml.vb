Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.System.UserProfile
Imports Windows.UI
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Async Sub Page_Loaded(sender As FrameworkElement, args As Object)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        MasCosas.Generar()

        Bing.GenerarImagenes()
        Nasa.GenerarImagenes()

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

    Private Sub UsuarioPresionaImagen(sender As Object, e As PointerRoutedEventArgs)

        Dim imagen As ImageEx = sender

        Dim transpariencia As New UISettings
        Dim boolTranspariencia As Boolean = transpariencia.AdvancedEffectsEnabled

        gridSeleccionarUbicacion.Visibility = Visibility.Visible

        If boolTranspariencia = True Then
            gridSeleccionarUbicacion.Background = App.Current.Resources("GridTituloBackground")
        Else
            gridSeleccionarUbicacion.Background = New SolidColorBrush(Colors.LightGray)
        End If

    End Sub

    Private Sub BotonVolver_Click(sender As Object, e As RoutedEventArgs) Handles botonVolver.Click

        gridSeleccionarUbicacion.Visibility = Visibility.Collapsed

    End Sub

    Public Async Sub DescargaImagen(imagen As ImageEx, clave As String)

        Dim ficheroImagen As StorageFile = Nothing
        Dim descargaFinalizada As Boolean = False

        Dim fuente As Object = imagen.Source

        If TypeOf fuente Is Uri Then
            ficheroImagen = Await ApplicationData.Current.LocalFolder.CreateFileAsync(clave + ".png", CreationCollisionOption.ReplaceExisting)
            Dim descargador As New BackgroundDownloader

            Try
                Dim descarga As DownloadOperation = descargador.CreateDownload(fuente, ficheroImagen)
                Await descarga.StartAsync
                descargaFinalizada = True
            Catch ex As Exception

            End Try
        End If

        If TypeOf fuente Is BitmapImage Then
            Dim ficheroOrigen As StorageFile = imagen.Tag
            ficheroImagen = Await ApplicationData.Current.LocalFolder.CreateFileAsync(clave + ".png", CreationCollisionOption.ReplaceExisting)

            Await ficheroOrigen.CopyAndReplaceAsync(ficheroImagen)
            descargaFinalizada = True
        End If

        If descargaFinalizada = True Then
            Await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(ficheroImagen)
        End If
    End Sub


End Class
