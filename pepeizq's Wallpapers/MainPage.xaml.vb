Imports FontAwesome.UWP
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.System.UserProfile
Imports Windows.UI.Core

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Home"), FontAwesomeIcon.Home, 0))

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        Dim item As TextBlock = args.InvokedItem

        If Not item Is Nothing Then
            If item.Text = recursos.GetString("Home") Then
                GridVisibilidad(gridFondos, item.Text)
            End If
        End If

    End Sub

    Private Sub Nv_ItemFlyout(sender As NavigationViewItem, args As TappedRoutedEventArgs)

        FlyoutBase.ShowAttachedFlyout(sender)

    End Sub

    Private Sub Page_Loaded(sender As FrameworkElement, args As Object)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        MasCosas.Generar()
        ImagenesDia.Bing()

        Dim recursos As New Resources.ResourceLoader()

        GridVisibilidad(gridFondos, recursos.GetString("Home"))
        nvPrincipal.IsPaneOpen = False

    End Sub

    Private Sub GridVisibilidad(grid As Grid, tag As String)

        tbTitulo.Text = Package.Current.DisplayName + " (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ") - " + tag

        gridFondos.Visibility = Visibility.Collapsed

        grid.Visibility = Visibility.Visible

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

    Private Async Sub PivotFondos_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles pivotFondos.SelectionChanged

        Dim pivot As Pivot = sender
        Dim item As PivotItem = pivot.SelectedItem

        If Not item Is Nothing Then
            Await Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                                                                If item.Tag = 0 Then
                                                                                                                    ImagenesDia.Bing()
                                                                                                                ElseIf item.Tag = 1 Then
                                                                                                                    ImagenesDia.Nasa()
                                                                                                                ElseIf item.Tag = 2 Then
                                                                                                                    ImagenesDia.NationalGeographic()
                                                                                                                ElseIf item.Tag = 3 Then
                                                                                                                    ImagenesDia.Space()
                                                                                                                ElseIf item.Tag = 4 Then
                                                                                                                    ImagenesDia.Reddit("EarthPorn", "Earth")
                                                                                                                ElseIf item.Tag = 5 Then
                                                                                                                    ImagenesDia.Reddit("spaceporn", "Space")
                                                                                                                ElseIf item.Tag = 6 Then
                                                                                                                    ImagenesDia.Reddit("CityPorn", "City")
                                                                                                                ElseIf item.Tag = 7 Then
                                                                                                                    ImagenesDia.Reddit("Map_Porn", "Map")
                                                                                                                End If
                                                                                                            End Sub)
        End If

    End Sub
End Class
