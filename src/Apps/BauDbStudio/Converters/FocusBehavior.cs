using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bau.DbStudio.Converters;

/// <summary>
///		Comportamiento para pasar el foco al primer elemento
/// </summary>
public static class FocusBehavior
{
    public static readonly DependencyProperty FocusFirstProperty = DependencyProperty.RegisterAttached("FocusFirst", typeof(bool), typeof(FocusBehavior),
																									   new PropertyMetadata(false, OnFocusFirstPropertyChanged));

    /// <summary>
    ///		Obtiene el valor de la propiedad
    /// </summary>
    public static bool GetFocusFirst(Control control)
    {
        return (bool) control.GetValue(FocusFirstProperty);
    }

    /// <summary>
    ///		Asigna el valor de la propiedad
    /// </summary>
    public static void SetFocusFirst(Control control, bool value)
    {
        control.SetValue(FocusFirstProperty, value);
    }

    /// <summary>
    ///     Trata el evento OnFocusFirst
    /// </summary>
    static void OnFocusFirstPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        try
        {
            if (obj is Control control && ((args.NewValue as bool?) ?? false))
                control.Loaded += (sender, e) => control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception.Message);
        }
    }
}
