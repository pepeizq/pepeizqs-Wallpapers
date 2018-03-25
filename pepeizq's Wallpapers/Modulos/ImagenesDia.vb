Imports System.Net
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.UI.Core
Imports Windows.UI.Xaml.Media.Animation

Module ImagenesDia

    Public Async Sub Generar()

        Await Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                                                            BuscarImagenes()
                                                                                                        End Sub)

    End Sub

    Private Sub BuscarImagenes()

        Bing()
        Nasa()
        NationalGeographic()
        Space()
        Reddit("EarthPorn", "Earth")
        Reddit("spaceporn", "Space")
        Reddit("CityPorn", "City")
        Reddit("Map_Porn", "Map")

    End Sub

    Private Sub AñadirGvItem(enlaceFondo As String, web As String, imagenWebEnlace As String)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gv As GridView = pagina.FindName("gvFondos")

        Dim gvItem As New GridViewItem With {
            .Margin = New Thickness(10, 10, 10, 10)
        }

        Dim sp As New StackPanel With {
            .Orientation = Orientation.Vertical,
            .Width = 500,
            .Background = New SolidColorBrush(App.Current.Resources("ColorSecundario"))
        }

        Dim imagenFondo As New ImageEx With {
            .IsCacheEnabled = True,
            .Stretch = Stretch.UniformToFill,
            .Margin = New Thickness(3, 3, 3, 0),
            .Height = 300,
            .Source = New Uri(enlaceFondo)
        }

        AddHandler imagenFondo.PointerPressed, AddressOf UsuarioPresionaImagen
        AddHandler imagenFondo.PointerEntered, AddressOf UsuarioEntraBoton
        AddHandler imagenFondo.PointerExited, AddressOf UsuarioSaleBoton

        sp.Children.Add(imagenFondo)

        Dim subSp As New StackPanel With {
            .Orientation = Orientation.Horizontal,
            .Margin = New Thickness(10, 10, 10, 10),
            .HorizontalAlignment = HorizontalAlignment.Center,
            .VerticalAlignment = VerticalAlignment.Center
        }

        Dim imagenWeb As New ImageEx With {
            .Stretch = Stretch.UniformToFill,
            .Width = 16,
            .Height = 16,
            .IsCacheEnabled = True,
            .Source = imagenWebEnlace,
            .Margin = New Thickness(0, 0, 10, 0)
        }

        subSp.Children.Add(imagenWeb)

        Dim tbWeb As New TextBlock With {
            .Text = web,
            .Foreground = New SolidColorBrush(Windows.UI.Colors.White)
        }

        subSp.Children.Add(tbWeb)

        sp.Children.Add(subSp)

        gvItem.Content = sp
        gv.Items.Add(gvItem)

    End Sub

    Private Async Sub Bing()

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US"))

        If Not html = Nothing Then
            If html.Contains(ChrW(34) + "url" + ChrW(34)) Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = html.IndexOf(ChrW(34) + "url" + ChrW(34))
                temp = html.Remove(0, int + 7)

                int2 = temp.IndexOf(ChrW(34))
                temp2 = temp.Remove(int2, temp.Length - int2)

                Dim enlace As String = "https://www.bing.com" + temp2.Trim

                AñadirGvItem(enlace, "Bing", "Assets\Webs\bing.png")
            End If
        End If

    End Sub

    Private Async Sub Nasa()

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://api.nasa.gov/planetary/apod?api_key=Uqit7gyg7GiUGHf2pNclhJeKFCBZyFSS4Uc7qbSB"))

        If Not html = Nothing Then
            If html.Contains(ChrW(34) + "hdurl" + ChrW(34)) Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = html.IndexOf(ChrW(34) + "hdurl" + ChrW(34))
                temp = html.Remove(0, int + 10)

                int2 = temp.IndexOf(ChrW(34))
                temp2 = temp.Remove(int2, temp.Length - int2)

                Dim enlace As String = temp2.Trim

                AñadirGvItem(enlace, "Nasa", "Assets\Webs\nasa.png")
            End If
        End If

    End Sub

    Private Async Sub Space()

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

                AñadirGvItem(enlace, "Space", "Assets\Webs\space.png")
            End If
        End If

    End Sub

    Private Async Sub NationalGeographic()

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://www.nationalgeographic.com/photography/photo-of-the-day/"))

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

                AñadirGvItem(enlace, "National Geographic", "Assets\Webs\nationalgeographic.png")
            End If
        End If

    End Sub

    Private Async Sub Reddit(subEnlace As String, subNombre As String)

        Dim html As String = Await Decompiladores.HttpClient(New Uri("https://www.reddit.com/r/" + subEnlace + "/.json"))

        If Not html = Nothing Then
            html = WebUtility.HtmlDecode(html)

            If html.Contains(ChrW(34) + "distinguished" + ChrW(34) + ": null") Then
                Dim temp, temp2, temp3 As String
                Dim int, int2, int3 As Integer

                int = html.IndexOf(ChrW(34) + "distinguished" + ChrW(34) + ": null")
                temp = html.Remove(0, int + 10)

                int2 = temp.LastIndexOf(ChrW(34) + "source" + ChrW(34) + ": {" + ChrW(34) + "url" + ChrW(34) + ": " + ChrW(34))
                temp2 = temp.Remove(0, int2 + 19)

                int3 = temp2.IndexOf(ChrW(34))
                temp3 = temp2.Remove(int3, temp2.Length - int3)

                Dim enlace As String = temp3.Trim

                AñadirGvItem(enlace, "Reddit (" + subNombre + ")", "Assets\Webs\reddit.png")
            End If
        End If

    End Sub

    Private Sub UsuarioPresionaImagen(sender As Object, e As PointerRoutedEventArgs)

        Dim imagen As ImageEx = sender

        Dim transpariencia As New UISettings
        Dim boolTranspariencia As Boolean = transpariencia.AdvancedEffectsEnabled

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gridSeleccionarUbicacion As Grid = pagina.FindName("gridSeleccionarUbicacion")

        gridSeleccionarUbicacion.Visibility = Visibility.Visible
        gridSeleccionarUbicacion.Tag = imagen

        If boolTranspariencia = True Then
            gridSeleccionarUbicacion.Background = App.Current.Resources("GridTituloBackground")
        Else
            gridSeleccionarUbicacion.Background = New SolidColorBrush(Windows.UI.Colors.LightGray)
        End If

        ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("imagen", imagen)

        Dim animacion As ConnectedAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("imagen")

        If Not animacion Is Nothing Then
            animacion.TryStart(gridSeleccionarUbicacion)
        End If

    End Sub

    Private Sub UsuarioEntraBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Hand, 1)

    End Sub

    Private Sub UsuarioSaleBoton(sender As Object, e As PointerRoutedEventArgs)

        Window.Current.CoreWindow.PointerCursor = New CoreCursor(CoreCursorType.Arrow, 1)

    End Sub

End Module
