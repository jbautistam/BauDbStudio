using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects;

namespace Bau.DbStudio.Views.EtlProjects
{
	/// <summary>
	///		Vista para ejecutar un proyecto de ETL
	/// </summary>
	public partial class ExecuteEtlConsoleView : UserControl
	{
		public ExecuteEtlConsoleView(ExecuteEtlConsoleViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
			LoadFiles();
			ViewModel.LoadFileRequired += (sender, args) => LoadFiles();
		}

		/// <summary>
		///		Carga los archivos
		/// </summary>
		private void LoadFiles()
		{
			if (!ViewModel.ProjectFileName.Equals(udtProject.FileName, StringComparison.CurrentCultureIgnoreCase))
				LoadFile(udtProject, ViewModel.ProjectFileName);
			if (!ViewModel.ContextFileName.Equals(udtContext.FileName, StringComparison.CurrentCultureIgnoreCase))
				LoadFile(udtContext, ViewModel.ContextFileName);
		}

		/// <summary>
		///		Carga un archivo en un editor
		/// </summary>
		private void LoadFile(Bau.Controls.CodeEditor.ctlEditor udtEditor, string fileName)
		{
			// Asigna la configuración al editor
			udtEditor.EditorFontName = MainWindow.MainController.ConfigurationController.EditorFontName;
			udtEditor.EditorFontSize = MainWindow.MainController.ConfigurationController.EditorFontSize;
			udtEditor.ShowLinesNumber = MainWindow.MainController.ConfigurationController.EditorShowLinesNumber;
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
		///		ViewModel
		/// </summary>
		public ExecuteEtlConsoleViewModel ViewModel { get; }
	}
}
