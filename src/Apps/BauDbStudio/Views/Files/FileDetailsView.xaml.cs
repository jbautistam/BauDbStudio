using System;
using System.Windows.Controls;

using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files;

namespace Bau.DbStudio.Views.Files
{
	/// <summary>
	///		Ventana para mostrar el contenido de un archivo
	/// </summary>
	public partial class FileDetailsView : UserControl
	{
		// Variables privadas
		private DragDropTreeExplorerController _dragDropController = new DragDropTreeExplorerController();

		public FileDetailsView(FileViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			if (!IsLoadedViewModel)
			{
				// Asigna la configuración al editor
				udtEditor.EditorFontName = MainWindow.MainController.ConfigurationController.EditorFontName;
				udtEditor.EditorFontSize = MainWindow.MainController.ConfigurationController.EditorFontSize;
				udtEditor.ShowLinesNumber = MainWindow.MainController.ConfigurationController.EditorShowLinesNumber;
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
						ViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage($"Error al abrir el archivo {exception.Message}");
					}
					// Asigna el manejador de eventos
					ViewModel.SelectedTextRequired += (sender, args) => args.SelectedText = udtEditor.GetSelectedText();
					// Cambia el resaltado en los archivo SQLx
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
				if (cboZoom.SelectedIndex < 0 && MainWindow.MainController.ConfigurationController.EditorZoom >= GetZoom(index))
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
			switch (zoomSelectedIndex)
			{
				case 0:
					return 0.5;
				case 1:
					return 0.75;
				case 3:
					return 1.25;
				case 4:
					return 1.5;
				case 5:
					return 2;
				default:
					return 1;
			}
		}

		/// <summary>
		///		Cambia el zoom
		/// </summary>
		private void ChangeZoom(double zoom)
		{
			// Cambia el zoom en el editor
			udtEditor.EditorFontSize = zoom * MainWindow.MainController.ConfigurationController.EditorFontSize;
			// Cambia la configuración de zoom
			MainWindow.MainController.ConfigurationController.EditorZoom = zoom;
		}

		/// <summary>
		///		Obtiene el texto seleccionado
		/// </summary>
		public string GetSelectedText()
		{
			return udtEditor.GetSelectedText();
		}

		/// <summary>
		///		Busca la siguiente coincidencia con el texto
		/// </summary>
		public bool SearchNext(string textToFind, bool upToDown, bool caseSensitive, bool wholeWord, bool useRegex, bool useWildcards)
		{
			return udtEditor.SearchNext(textToFind, upToDown, caseSensitive, wholeWord, useRegex, useWildcards);
		}

		/// <summary>
		///		Reemplaza el texto
		/// </summary>
		internal bool Replace(string textToFind, string textToReplace, bool caseSensitive, bool wholeWord, bool useRegex, bool useWildcards)
		{
			return udtEditor.Replace(textToFind, textToReplace, caseSensitive, wholeWord, useRegex, useWildcards);
		}

		/// <summary>
		///		Reemplaza todas las coincidencias
		/// </summary>
		internal void ReplaceAll(string textToFind, string textToReplace, bool caseSensitive, bool wholeWord, bool useRegex, bool useWildcards)
		{
			udtEditor.ReplaceAll(textToFind, textToReplace, caseSensitive, wholeWord, useRegex, useWildcards);
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public FileViewModel ViewModel { get; }

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

		private void udtEditor_Drop(object sender, System.Windows.DragEventArgs e)
		{
			if (_dragDropController.GetDragDropFileNode(e.Data) is Libraries.DbStudio.ViewModels.Solutions.Explorers.Connections.NodeTableViewModel tableNodeViewModel)
				udtEditor.InsertText(tableNodeViewModel.GetSqlSelect(e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey), 
									 e.GetPosition(udtEditor));
			else if (_dragDropController.GetDragDropFileNode(e.Data) is Libraries.DbStudio.ViewModels.Solutions.Explorers.Connections.NodeTableFieldViewModel fieldNodeViewModel)
				udtEditor.InsertText(fieldNodeViewModel.GetSqlSelect(e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey), 
									 e.GetPosition(udtEditor));
			else if (_dragDropController.GetDragDropFileNode(e.Data) is Libraries.DbStudio.ViewModels.Solutions.Explorers.Files.NodeFileViewModel fileNodeViewModel)
				udtEditor.InsertText(fileNodeViewModel.GetSqlSelect(e.KeyStates == System.Windows.DragDropKeyStates.ShiftKey), 
									 e.GetPosition(udtEditor));
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
}
