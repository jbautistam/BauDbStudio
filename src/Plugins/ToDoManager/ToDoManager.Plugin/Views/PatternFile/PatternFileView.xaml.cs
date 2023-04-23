using System;
using System.Windows.Controls;
using System.Windows;

using Bau.Libraries.ToDoManager.ViewModel.PatternsFile;

namespace Bau.Libraries.ToDoManager.Plugin.Views.PatternFile;

/// <summary>
///		Formulario para mostrar el contenido de un grupo de tareas
/// </summary>
public partial class PatternFileView : UserControl
{
	// Variables privadas
	private bool _isLoaded;

	public PatternFileView(PatternFileViewModel viewModel)
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
			ViewModel.LoadFile();
			// Asigna las propiedades
			txtSource.Text = ViewModel.Source;
			txtCommand.Text = ViewModel.Formula;
		}
	}

	/// <summary>
	///		Ejecuta la fórmula
	/// </summary>
	private void ExecuteFormula()
	{
		txtResult.Text = ViewModel.Execute();
	}

	/// <summary>
	///		ViewModel asociado al control
	/// </summary>
	public PatternFileViewModel ViewModel { get; }

	private void UserControl_Loaded(object sender, EventArgs e)
	{
		InitControl();
	}

	private void txtCommand_TextChanged(object sender, EventArgs e)
	{
		ViewModel.Formula = txtCommand.Text;
	}

	private void txtSource_TextChanged(object sender, EventArgs e)
	{
		ViewModel.Source = txtSource.Text;
	}

	private void cmdExecute_Click(object sender, RoutedEventArgs e)
	{
		ExecuteFormula();
    }

	private void cmdDelete_Click(object sender, RoutedEventArgs e)
	{
		txtResult.Text = string.Empty;
	}
}