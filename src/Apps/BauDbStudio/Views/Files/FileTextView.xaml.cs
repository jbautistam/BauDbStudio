using System.Windows.Controls;

using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Files;

namespace Bau.DbStudio.Views.Files;

/// <summary>
///		Ventana para mostrar el contenido de un archivo de texto
/// </summary>
public partial class FileTextView : UserControl
{
	// Variables privadas
	private DragDropTreeController _dragDropController;

	public FileTextView(BaseTextFileViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		_dragDropController = new DragDropTreeController(this, "BaseNodeViewModel");
	}

	/// <summary>
	///		Inicializa el formulario
	/// </summary>
	private void InitForm()
	{
		if (!IsLoadedViewModel)
		{
			// Asigna la configuración al editor
			udtEditor.EditorFontName = ViewModel.PluginsController.ConfigurationController.EditorFontName;
			udtEditor.EditorFontSize = ViewModel.PluginsController.ConfigurationController.EditorFontSize;
			udtEditor.ShowLinesNumber = ViewModel.PluginsController.ConfigurationController.EditorShowLinesNumber;
			// Inicializa los combos
			InitZoom();
			// Ajusta el editor con los datos del viewModel
			if (ViewModel != null && !string.IsNullOrWhiteSpace(ViewModel.FileName) && !IsLoadedViewModel)
			{
				// Asigna el nombre de archivo
				udtEditor.FileName = ViewModel.FileName;
				// Carga el texto
				try
				{
					ViewModel.Load();
					udtEditor.Text = ViewModel.Content; 
				}
				catch (Exception exception)
				{
					udtEditor.IsEnabled = false;
					ViewModel.PluginsController.MainWindowController.SystemController.ShowMessage($"Error al abrir el archivo {exception.Message}");
				}
				// Asigna los manejadores de eventos
				ViewModel.SelectedTextRequired += (sender, args) => args.SelectedText = udtEditor.GetSelectedText();
				ViewModel.GoToLineRequired += (sender, args) => {
																	Focus();
																	udtEditor.Focus();
																	udtEditor.GoToLine(args.Line, args.TextSelected);
																};
				// Cambia el resaltado en los archivos SQLx
				if (!string.IsNullOrWhiteSpace(ViewModel.FileName) && ViewModel.FileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
					udtEditor.ChangeHighLightByExtension(".sql");
				// Indica que no ha habido modificaciones y que se ha cargado el archivo, si no se lanza
				ViewModel.IsUpdated = false;
				IsLoadedViewModel = true;
			}
			// Selecciona el editor (el behaviour no funciona con esta ventana)
			udtEditor.Focus();
		}
	}

	/// <summary>
	///		Inicializa el Zoom
	/// </summary>
	private void InitZoom()
	{
		// Limpia el combo
		cboZoom.Items.Clear();
		// Añade los elementos
		cboZoom.Items.Add("50%");
		cboZoom.Items.Add("75%");
		cboZoom.Items.Add("100%");
		cboZoom.Items.Add("125%");
		cboZoom.Items.Add("150%");
		cboZoom.Items.Add("200%");
		// Asigna el manejador de eventos
		cboZoom.SelectionChanged += (sender, args) => ChangeZoom(GetZoom(cboZoom.SelectedIndex));
		// Asigna el zoom seleccionado en la confiburación
		for (int index = cboZoom.Items.Count - 1; index >= 0; index--)
			if (cboZoom.SelectedIndex < 0 && ViewModel.PluginsController.ConfigurationController.EditorZoom >= GetZoom(index))
				cboZoom.SelectedIndex = index;
	}

	/// <summary>
	///		Cambia el zoom dependiendo de la rueda del ratón
	/// </summary>
	private void ChangeZoomByWheel(int delta)
	{
		if (delta > 0 && cboZoom.SelectedIndex < cboZoom.Items.Count - 1)
			cboZoom.SelectedIndex += 1;
		else if (delta < 0 && cboZoom.SelectedIndex > 0)
			cboZoom.SelectedIndex -= 1;
	}

	/// <summary>
	///		Obtiene el zoom asociado a un elemento del combo
	/// </summary>
	private double GetZoom(int zoomSelectedIndex)
	{
		return zoomSelectedIndex switch
				{
					0 => 0.5,
					1 => 0.75,
					3 => 1.25,
					4 => 1.5,
					5 => 2,
					_ => 1
				};
	}

	/// <summary>
	///		Cambia el zoom
	/// </summary>
	private void ChangeZoom(double zoom)
	{
		// Cambia el zoom en el editor
		udtEditor.EditorFontSize = zoom * ViewModel.PluginsController.ConfigurationController.EditorFontSize;
		// Cambia la configuración de zoom
		ViewModel.PluginsController.ConfigurationController.EditorZoom = zoom;
	}

	/// <summary>
	///		Obtiene el texto seleccionado
	/// </summary>
	public string GetSelectedText() => udtEditor.GetSelectedText();

	/// <summary>
	///		Abre / cierra la ventana de búsqueda
	/// </summary>
	public void OpenSearch(bool show)
	{
		udtEditor.ShowSearch(show);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public BaseTextFileViewModel ViewModel { get; }

	/// <summary>
	///		Indica si se ha cargado el archivo de ViewModel una vez (a usercontrol_loaded se llama cada vez que cambia de ficha)
	/// </summary>
	public bool	IsLoadedViewModel { get; private set; }

	private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
	{
		InitForm();
	}

	private void udtEditor_TextChanged(object sender, EventArgs e)
	{
		ViewModel.Content = udtEditor.Text;
		ViewModel.IsUpdated = true;
	}

	private void udtEditor_DragEnter(object sender, System.Windows.DragEventArgs e)
	{
		e.Effects = System.Windows.DragDropEffects.All;
	}

	private async void udtEditor_Drop(object sender, System.Windows.DragEventArgs e)
	{
		if (_dragDropController.GetDragDropFileNode(e.Data) is Libraries.PluginsStudio.ViewModels.Base.Explorers.PluginNodeViewModel nodeViewModel)
		{
			string text = nodeViewModel.GetTextForEditor(e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey);

				if (!string.IsNullOrWhiteSpace(text))
					udtEditor.InsertText(await ViewModel.TreatTextDroppedAsync(text, e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey, CancellationToken.None), 
										 e.GetPosition(udtEditor));
		}
	}

	private void udtEditor_PositionChanged(object sender, EventArgs e)
	{
		lblColumn.Text = udtEditor.Column.ToString();
		lblRow.Text = udtEditor.Line.ToString();
	}

	private void udtEditor_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
	{
		if (System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
		{
			// Cambia el zoom
			ChangeZoomByWheel(e.Delta);
			// Indica que se ha tratado el evento
			e.Handled = true;
		}
	}
}