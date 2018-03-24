Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.UI.Core

Module Bing

    Public Async Sub GenerarImagenDia()

        Await Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                                                              Descarga()
                                                                                                          End Sub)

    End Sub

    Private Async Sub Descarga()

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

                Dim frame As Frame = Window.Current.Content
                Dim pagina As Page = frame.Content

                Dim imagen As ImageEx = pagina.FindName("imagenFondoDiaBing")
                imagen.Source = New Uri(enlace)

                Dim gvItem As GridViewItem = pagina.FindName("gvItemBing")
                gvItem.Visibility = Visibility.Visible
            End If
        End If

    End Sub

End Module
