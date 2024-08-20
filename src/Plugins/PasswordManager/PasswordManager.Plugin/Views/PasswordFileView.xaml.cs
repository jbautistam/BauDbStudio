using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.PasswordManager.ViewModel.Reader;

namespace Bau.Libraries.PasswordManager.Plugin.Views;

/// <summary>
///		Formulario para mostrar el contenido de un archivo de contraseñas
/// </summary>
public partial class PasswordFileView : UserControl
{
	// Variables privadas
	private bool _isLoaded;

	public PasswordFileView(PasswordFileViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Asigna la clase del documento
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	private void InitControl()
	{
		if (!_isLoaded)
		{
			// Indica que ya no se debe cargar de nuevo
			_isLoaded = true;
			// Carga el archivo
			//ViewModel.LoadFile();
		}
	}

	/// <summary>
	///		ViewModel asociado al control
	/// </summary>
	public PasswordFileViewModel ViewModel { get; }

	private void UserControl_Loaded(object sender, EventArgs e)
	{
		InitControl();
	}

	private void trvEntries_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		PasswordFileViewModel? context = trvEntries.DataContext as PasswordFileViewModel;

			if (context != null && (sender as TreeView)?.SelectedItem is ViewModel.Reader.Explorer.BaseNodeEntryViewModel node)
				context.Tree.SelectedNode = node;
	}

	private void trvEntries_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left)
			ViewModel.Tree.SelectedNode = null;
	}
}