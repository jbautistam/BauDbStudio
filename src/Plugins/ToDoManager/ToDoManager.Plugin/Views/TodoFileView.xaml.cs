using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

using Bau.Libraries.ToDoManager.ViewModel.Reader;

namespace Bau.Libraries.ToDoManager.Plugin.Views;

/// <summary>
///		Formulario para mostrar el contenido de un grupo de tareas
/// </summary>
public partial class TodoFileView : UserControl
{
	// Variables privadas
	private bool _isLoaded;

	public TodoFileView(ToDoFileViewModel viewModel)
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
	public ToDoFileViewModel ViewModel { get; }

	private void UserControl_Loaded(object sender, EventArgs e)
	{
		InitControl();
	}

	private void trvEntries_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		ToDoFileViewModel context = trvEntries.DataContext as ToDoFileViewModel;

			if (context != null && (sender as TreeView).SelectedItem is ViewModel.Reader.Explorer.BaseNodeViewModel node)
				context.Tree.SelectedNode = node;
	}

	private void trvEntries_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left)
			ViewModel.Tree.SelectedNode = null;
	}
}