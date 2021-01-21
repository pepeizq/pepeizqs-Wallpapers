Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Nv_Loaded(sender As Object, e As RoutedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        'nvPrincipal.MenuItems.Add(NavigationViewItems.Generar(recursos.GetString("Home"), FontAwesomeIcon.Home, 0))

    End Sub

    Private Sub Nv_ItemInvoked(sender As NavigationView, args As NavigationViewItemInvokedEventArgs)

        Dim recursos As New Resources.ResourceLoader()

        'Dim item As TextBlock = args.InvokedItem

        'If Not item Is Nothing Then
        '    If item.Text = recursos.GetString("Home") Then
        '        GridVisibilidad(gridFondos, item.Text)
        '    End If
        'End If

    End Sub

    Private Sub Page_Loaded(sender As FrameworkElement, args As Object)

        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES"
        'Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US"

        Interfaz.Fondos.Cargar()
        MasCosas.Cargar()

    End Sub

End Class
