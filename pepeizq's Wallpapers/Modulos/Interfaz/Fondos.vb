Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.System.UserProfile

Namespace Interfaz

    Module Fondos

        Public Sub Cargar()

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbTitulo As TextBlock = pagina.FindName("tbTitulo")
            tbTitulo.Text = "pepeizq's Wallpapers (" + Package.Current.Id.Version.Major.ToString + "." + Package.Current.Id.Version.Minor.ToString + "." + Package.Current.Id.Version.Build.ToString + "." + Package.Current.Id.Version.Revision.ToString + ")"

            Dim cbFondos As ComboBox = pagina.FindName("cbFondos")

            AddHandler cbFondos.SelectionChanged, AddressOf CbFondos_SelectionChanged
            AddHandler cbFondos.PointerEntered, AddressOf EfectosHover.Entra_Basico
            AddHandler cbFondos.PointerExited, AddressOf EfectosHover.Sale_Basico

            cbFondos.SelectedIndex = 0

            Dim botonAñadirFondoEscritorio As Button = pagina.FindName("botonAñadirFondoEscritorio")

            AddHandler botonAñadirFondoEscritorio.Click, AddressOf BotonAñadirFondoEscritorio_Click
            AddHandler botonAñadirFondoEscritorio.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_05
            AddHandler botonAñadirFondoEscritorio.PointerExited, AddressOf EfectosHover.Sale_Boton_1_05

            Dim botonAñadirFondoBloqueo As Button = pagina.FindName("botonAñadirFondoBloqueo")

            AddHandler botonAñadirFondoBloqueo.Click, AddressOf BotonAñadirFondoBloqueo_Click
            AddHandler botonAñadirFondoBloqueo.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_05
            AddHandler botonAñadirFondoBloqueo.PointerExited, AddressOf EfectosHover.Sale_Boton_1_05

            Dim botonGuardarImagen As Button = pagina.FindName("botonGuardarImagen")

            AddHandler botonGuardarImagen.Click, AddressOf BotonGuardarImagen_Click
            AddHandler botonGuardarImagen.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_05
            AddHandler botonGuardarImagen.PointerExited, AddressOf EfectosHover.Sale_Boton_1_05

        End Sub

        Private Sub CbFondos_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim cbFondos As ComboBox = pagina.FindName("cbFondos")

            Dim tbTituloFondo As TextBlock = pagina.FindName("tbTituloFondo")
            tbTituloFondo.Visibility = Visibility.Collapsed
            tbTituloFondo.Text = String.Empty

            Dim tbDescripcionFondo As TextBlock = pagina.FindName("tbDescripcionFondo")
            tbDescripcionFondo.Visibility = Visibility.Collapsed
            tbDescripcionFondo.Text = String.Empty

            If cbFondos.SelectedIndex = 0 Then
                Bing.Generar()
            ElseIf cbFondos.SelectedIndex = 1 Then
                Nasa.Generar()
            ElseIf cbFondos.SelectedIndex = 2 Then
                NationalGeographic.Generar()
            ElseIf cbFondos.SelectedIndex = 3 Then
                Space.Generar()
            ElseIf cbFondos.SelectedIndex = 4 Then
                Reddit.Generar("EarthPorn")
            ElseIf cbFondos.SelectedIndex = 5 Then
                Reddit.Generar("spaceporn")
            ElseIf cbFondos.SelectedIndex = 6 Then
                Reddit.Generar("CityPorn")
            ElseIf cbFondos.SelectedIndex = 7 Then
                Reddit.Generar("Map_Porn")
            ElseIf cbFondos.SelectedIndex = 8 Then
                Reddit.Generar("wallpapers")
            ElseIf cbFondos.SelectedIndex = 9 Then
                Reddit.Generar("futureporn")
            End If

        End Sub

        Private Sub BotonAñadirFondoEscritorio_Click(sender As Object, e As RoutedEventArgs)

            AñadirImagen(0)

        End Sub

        Private Sub BotonAñadirFondoBloqueo_Click(sender As Object, e As RoutedEventArgs)

            AñadirImagen(1)

        End Sub

        Private Sub BotonGuardarImagen_Click(sender As Object, e As RoutedEventArgs)

            AñadirImagen(2)

        End Sub

        Private Async Sub AñadirImagen(tipo As Integer)

            Estado(False)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim imagenFondo As ImageEx = pagina.FindName("imagenFondo")

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

            Dim fuente As Object = imagenFondo.Source

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
                Dim ficheroOrigen As StorageFile = imagenFondo.Tag
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
                        MasCosas.CalificarApp(True)
                    Else
                        Toast(recursos.GetString("ImageFail"), Nothing)
                    End If
                ElseIf tipo = 1 Then
                    Dim estado As Boolean = Await UserProfilePersonalizationSettings.Current.TrySetLockScreenImageAsync(ficheroImagen)

                    If estado = True Then
                        Toast(recursos.GetString("ImageSuccess"), Nothing)
                        MasCosas.CalificarApp(True)
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

            Estado(True)

        End Sub

        Private Sub Estado(estado As Boolean)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim botonAñadirFondoEscritorio As Button = pagina.FindName("botonAñadirFondoEscritorio")
            botonAñadirFondoEscritorio.IsEnabled = estado

            Dim botonAñadirFondoBloqueo As Button = pagina.FindName("botonAñadirFondoBloqueo")
            botonAñadirFondoBloqueo.IsEnabled = estado

            Dim botonGuardarImagen As Button = pagina.FindName("botonGuardarImagen")
            botonGuardarImagen.IsEnabled = estado

        End Sub

    End Module

End Namespace