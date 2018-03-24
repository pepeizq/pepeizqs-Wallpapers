Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.System.UserProfile
Imports Windows.UI
Imports Windows.UI.Core
Imports Windows.UI.Xaml.Media.Animation

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Page_Loaded(sender As FrameworkElement, args As Object)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        MasCosas.Generar()

        Bing.GenerarImagenDia()
        Nasa.GenerarImagenDia()
        NationalGeographic.GenerarImagenDia()
        Space.GenerarImagenDia()

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
        gridSeleccionarUbicacion.Tag = imagen

        If boolTranspariencia = True Then
            gridSeleccionarUbicacion.Background = App.Current.Resources("GridTituloBackground")
        Else
            gridSeleccionarUbicacion.Background = New SolidColorBrush(Colors.LightGray)
        End If

        ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("imagen", imagen)

        Dim animacion As ConnectedAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("imagen")

        If Not animacion Is Nothing Then
            animacion.TryStart(gridSeleccionarUbicacion)
        End If

    End Sub

    Private Sub BotonVolver_Click(sender As Object, e As RoutedEventArgs) Handles botonVolver.Click

        gridSeleccionarUbicacion.Visibility = Visibility.Collapsed

    End Sub

    Private Sub BotonAñadirFondoEscritorio_Click(sender As Object, e As RoutedEventArgs) Handles botonAñadirFondoEscritorio.Click

        Dim imagen As ImageEx = gridSeleccionarUbicacion.Tag

        AñadirImagen(imagen, 0)

    End Sub

    Private Sub BotonAñadirFondoBloqueo_Click(sender As Object, e As RoutedEventArgs) Handles botonAñadirFondoBloqueo.Click

        Dim imagen As ImageEx = gridSeleccionarUbicacion.Tag

        AñadirImagen(imagen, 1)

    End Sub

    Public Async Sub AñadirImagen(imagen As ImageEx, tipo As Integer)

        botonAñadirFondoEscritorio.IsEnabled = False
        botonAñadirFondoBloqueo.IsEnabled = False
        pbAñadirFondo.Visibility = Visibility.Visible

        Dim helper As New LocalObjectStorageHelper
        Dim clave As Integer = 0

        If helper.KeyExists("claveFondo") Then
            clave = helper.Read(Of Integer)("claveFondo")
        End If

        clave = clave + 1

        helper.Save(Of Integer)("claveFondo", clave)

        Dim nombreFondo As String = clave.ToString

        Dim ficheroImagen As StorageFile = Nothing
        Dim descargaFinalizada As Boolean = False

        Dim fuente As Object = imagen.Source

        If TypeOf fuente Is Uri Then
            ficheroImagen = Await ApplicationData.Current.LocalFolder.CreateFileAsync(nombreFondo + ".png", CreationCollisionOption.ReplaceExisting)
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
            ficheroImagen = Await ApplicationData.Current.LocalFolder.CreateFileAsync(nombreFondo + ".png", CreationCollisionOption.ReplaceExisting)

            Await ficheroOrigen.CopyAndReplaceAsync(ficheroImagen)
            descargaFinalizada = True
        End If

        Dim recursos As New Resources.ResourceLoader

        If descargaFinalizada = True Then
            If tipo = 0 Then
                Dim estado As Boolean = Await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(ficheroImagen)

                If estado = True Then
                    Toast(recursos.GetString("ImageSuccess"), Nothing)
                Else
                    Toast(recursos.GetString("ImageFail"), Nothing)
                End If
            ElseIf tipo = 1 Then
                Dim estado As Boolean = Await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(ficheroImagen)

                If estado = True Then
                    Toast(recursos.GetString("ImageSuccess"), Nothing)
                Else
                    Toast(recursos.GetString("ImageFail"), Nothing)
                End If
            End If
        End If

        botonAñadirFondoEscritorio.IsEnabled = True
        botonAñadirFondoBloqueo.IsEnabled = True
        pbAñadirFondo.Visibility = Visibility.Collapsed

    End Sub


End Class
