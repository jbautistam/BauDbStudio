using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.RestManager.ViewModel.Project;

namespace Bau.Libraries.RestManager.Plugin.Views;

/// <summary>
///     Control de usuario para la lista de parámetros
/// </summary>
public partial class ParametersListView : UserControl
{
	// Propiedades de dependencia
	public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register(nameof(Parameters), typeof(RestParametersListViewModel), 
																							   typeof(ParametersListView),
																							   new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
																															 new PropertyChangedCallback(Parameters_PropertyChanged)));

    public ParametersListView()
    {
        InitializeComponent();
    }

	private static void Parameters_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (obj is ParametersListView listView && e.NewValue is RestParametersListViewModel listViewModel)
			listView.DataContext = listViewModel;
	}

	/// <summary>
	///		Lista de parámetros
	/// </summary>
	public RestParametersListViewModel? Parameters
	{
		get { return (RestParametersListViewModel) GetValue(ParametersProperty); }
		set { SetValue(ParametersProperty, value); }
	}
}
