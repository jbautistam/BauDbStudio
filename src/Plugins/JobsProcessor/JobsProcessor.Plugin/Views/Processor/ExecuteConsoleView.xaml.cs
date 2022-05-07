using System;
using System.Windows.Controls;

using Bau.Libraries.JobsProcessor.ViewModel.Processor;

namespace Bau.Libraries.JobsProcessor.Plugin.Views.Processor
{
	/// <summary>
	///		Vista para ejecutar un proyecto de ETL
	/// </summary>
	public partial class ExecuteEtlConsoleView : UserControl
	{
		public ExecuteEtlConsoleView(ExecuteConsoleViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
			ViewModel.PropertyChanged += (sender, args) => 
												{
													if (args.PropertyName.Equals(nameof(ViewModel.LogText), StringComparison.CurrentCultureIgnoreCase))
														udtLog.Text = viewModel.LogText;
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
			{
				udtEditor.FileName = string.Empty;
				udtEditor.Text = string.Empty;
			}
			else
			{
				udtEditor.FileName = fileName;
				udtEditor.Text = Libraries.LibHelper.Files.HelperFiles.LoadTextFile(fileName);
				udtEditor.ChangeHighLightByExtension(System.IO.Path.GetExtension(fileName));
			}
		}

		/// <summary>
		///		Interpreta el archivo
		/// </summary>
		private void ParseFile()
		{
			ViewModel.Parse();
		}

		private void trvExplorer_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
		{
			if ((sender as TreeView)?.SelectedItem is Bau.Libraries.JobsProcessor.ViewModel.Processor.LogTree.LogNodeViewModel node)
				ViewModel.TreeLogViewModel.SelectedNode = node;
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ExecuteConsoleViewModel ViewModel { get; }
	}
}
