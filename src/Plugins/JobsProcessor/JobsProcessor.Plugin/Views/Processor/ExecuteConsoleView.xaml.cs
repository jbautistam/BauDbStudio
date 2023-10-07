using System.Windows.Controls;

using Bau.Libraries.JobsProcessor.ViewModel.Processor;

namespace Bau.Libraries.JobsProcessor.Plugin.Views.Processor;

/// <summary>
///		Vista para ejecutar una consola
/// </summary>
public partial class ExecuteEtlConsoleView : UserControl
{
	public ExecuteEtlConsoleView(ExecuteConsoleViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
		ViewModel.PropertyChanged += (sender, args) => 
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName))
												{
													if (args.PropertyName.Equals(nameof(ViewModel.LogText), StringComparison.CurrentCultureIgnoreCase))
														udtLog.Text = viewModel.LogText;
													else if (args.PropertyName.Equals(nameof(ViewModel.FileText), StringComparison.CurrentCultureIgnoreCase))
														udtEditor.Text = ViewModel.FileText;
												}
											};
		LoadFile(ViewModel.FileName);
		ParseFile();
	}

	/// <summary>
	///		Carga el archivo en el editor
	/// </summary>
	private void LoadFile(string fileName)
	{
		// Asigna la configuración al editor
		udtEditor.EditorFontName = ViewModel.MainViewModel.MainController.PluginController.ConfigurationController.EditorFontName;
		udtEditor.EditorFontSize = ViewModel.MainViewModel.MainController.PluginController.ConfigurationController.EditorFontSize;
		udtEditor.ShowLinesNumber = ViewModel.MainViewModel.MainController.PluginController.ConfigurationController.EditorShowLinesNumber;
		// Carga el contenido en el editor
		if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName))
			udtEditor.FileName = string.Empty;
		else
		{
			udtEditor.FileName = fileName;
			udtEditor.ChangeHighLightByExtension(System.IO.Path.GetExtension(fileName));
			// Carga el archivo
			ViewModel.LoadFile();
		}
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	private void ParseFile()
	{
		ViewModel.Parse();
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public ExecuteConsoleViewModel ViewModel { get; }
}
