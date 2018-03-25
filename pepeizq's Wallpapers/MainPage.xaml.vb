Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.System.UserProfile
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Page_Loaded(sender As FrameworkElement, args As Object)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        MasCosas.Generar()
        ImagenesDia.Generar()

        '--------------------------------------------------------

        Dim transpariencia As New UISettings
        TransparienciaEfectosFinal(transpariencia.AdvancedEffectsEnabled)
        AddHandler transpariencia.AdvancedEffectsEnabledChanged, AddressOf TransparienciaEfectosCambia

    End Sub

    Private Sub TransparienciaEfectosCambia(sender As UISettings, e As Object)

        TransparienciaEfectosFinal(sender.AdvancedEffectsEnabled)

    End Sub

    Private Async Sub TransparienciaEfectosFinal(estado As Boolean)

        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If estado = True Then
                                                                       gridMasCosas.Background = App.Current.Resources("GridAcrilico")
                                                                   Else
                                                                       gridMasCosas.Background = New SolidColorBrush(App.Current.Resources("ColorPrimario"))
                                                                   End If
                                                               End Sub)

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

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

    Private Sub BotonGuardarImagen_Click(sender As Object, e As RoutedEventArgs) Handles botonGuardarImagen.Click

        Dim imagen As ImageEx = gridSeleccionarUbicacion.Tag

        AñadirImagen(imagen, 2)

    End Sub

    Public Async Sub AñadirImagen(imagen As ImageEx, tipo As Integer)

        botonAñadirFondoEscritorio.IsEnabled = False
        botonAñadirFondoBloqueo.IsEnabled = False
        botonGuardarImagen.IsEnabled = False

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
            ElseIf tipo = 2 Then
                Dim ficherosImagen As New List(Of String) From {
                    ".png"
                }

                Dim guardarPicker As New Pickers.FileSavePicker With {
                    .SuggestedStartLocation = Pickers.PickerLocationId.PicturesLibrary,
                    .SuggestedFileName = "Image"
                }

                guardarPicker.FileTypeChoices.Add("Image Files", ficherosImagen)

                Dim ficheroGuardado As StorageFile = Await guardarPicker.PickSaveFileAsync

                Await ficheroImagen.CopyAndReplaceAsync(ficheroGuardado)
            End If
        End If

        botonAñadirFondoEscritorio.IsEnabled = True
        botonAñadirFondoBloqueo.IsEnabled = True
        botonGuardarImagen.IsEnabled = True

    End Sub


End Class
