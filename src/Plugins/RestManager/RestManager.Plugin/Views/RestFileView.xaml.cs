using System.Windows.Controls;

using Bau.Libraries.RestManager.ViewModel.Project;
using Bau.Libraries.RestManager.ViewModel.Project.Steps;

namespace Bau.Libraries.RestManager.Plugin.Views;

/// <summary>
///		Vista para edición de un archivo Rest
/// </summary>
public partial class RestFileView : UserControl
{
	public RestFileView(RestFileViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Asigna el contexto
		DataContext = ViewModel = viewModel;
		// Asigna los manejadores de eventos
		ViewModel.StepsViewModel.PropertyChanged += (sender, args) =>
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName))
												{
													if (args.PropertyName.Equals(nameof(RestFileListStepsViewModel.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
														udtStep.Step = ViewModel.StepsViewModel.SelectedItem;
												}
											};
		// Selecciona el primer paso en el control
		udtStep.Step = ViewModel.StepsViewModel.SelectedItem;
		// Inicializa el editor
		InitEditor();
    }

	/// <summary>
	///		Inicializa el editor
	/// </summary>
	private void InitEditor()
	{
		// Asigna la configuración al editor
		udtLog.EditorFontName = ViewModel.MainViewModel.ViewsController.PluginController.ConfigurationController.EditorFontName;
		udtLog.EditorFontSize = ViewModel.MainViewModel.ViewsController.PluginController.ConfigurationController.EditorFontSize;
		udtLog.ShowLinesNumber = ViewModel.MainViewModel.ViewsController.PluginController.ConfigurationController.EditorShowLinesNumber;
		// Asigna el nombre de archivo
		udtLog.Text = ViewModel.Log ?? string.Empty;
		udtLog.ChangeHighLightByExtension(".md");
		// Asigna el manejador de eventos
		ViewModel.PropertyChanged += (sender, args) =>
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName))
												{
													if (args.PropertyName.Equals(nameof(RestFileViewModel.Log), StringComparison.CurrentCultureIgnoreCase))
														udtLog.Text = ViewModel.Log ?? string.Empty;
												}
											};
	}


	/// <summary>
	///		ViewModel
	/// </summary>
	public RestFileViewModel ViewModel { get; }
}
