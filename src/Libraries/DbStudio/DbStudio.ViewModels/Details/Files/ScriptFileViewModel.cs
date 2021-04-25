using System;
using System.Threading.Tasks;

using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbScripts.Manager.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Files
{
	/// <summary>
	///		ViewModel para un archivo
	/// </summary>
	public class ScriptFileViewModel : PluginsStudio.ViewModels.Base.Files.BaseTextFileViewModel
	{
		public ScriptFileViewModel(SolutionViewModel solutionViewModel, string fileName) : base(solutionViewModel.MainController.PluginController, fileName)
		{
			SolutionViewModel = solutionViewModel;
		}

		/// <summary>
		///		Ejecuta el script
		/// </summary>
		internal async Task ExecuteSqlScriptAsync(ConnectionModel connection, ArgumentListModel arguments, System.Threading.CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(Content))
				SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
			else if (connection == null)
				SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione una conexión");
			else 
				using (LibLogger.Models.Log.BlockLogModel block = SolutionViewModel.Manager.Logger.Default.CreateBlock(LibLogger.Models.Log.LogModel.LogType.Info,
																													   $"Comienza la ejecución de la consulta"))
				{
					string selectedText = GetEditorSelectedText();

						// Si no hay nada seleccionado, se ejecuta todo el contenido
						if (string.IsNullOrWhiteSpace(selectedText))
							selectedText = Content;
						// Ejecuta la consulta
						if (FileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
							await SolutionViewModel.Manager.ExecuteQueryAsync(connection, selectedText, arguments, 
																			  connection.TimeoutExecuteScript, cancellationToken);
						else if (FileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
							await SolutionViewModel.Manager.ExecuteInterpretedQueryAsync(connection, selectedText, arguments, cancellationToken);
						else
							block.Error("No se reconoce el tipo de archivo como SQL");
						// Muestra el tiempo de ejecución
						block.Info($"Tiempo de ejecución: {SolutionViewModel.ConnectionExecutionViewModel.ExecutionTime}");
				}
		}

		/// <summary>
		///		Ejecuta el script XML
		/// </summary>
		internal async Task ExecuteXmlScriptAsync(string contextFileName, System.Threading.CancellationToken cancellationToken)
		{
			ScriptsManager.JobXmlProjectManager manager = new ScriptsManager.JobXmlProjectManager(SolutionViewModel.Manager.Logger);

				// Ejecuta el script XML
				await manager.ExecuteAsync(FileName, contextFileName, cancellationToken);
				// Libera el log
				SolutionViewModel.Manager.Logger.Flush();
		}

		/// <summary>
		///		Obtiene la cadena asociada a un archivo cuando se arrastra un nodo sobre el editor de texto
		/// </summary>
		public override string TreatTextDropped(string content, bool shiftPressed)
		{
			return new Controllers.DropItems.NodeTextDropHelper().TreatTextDropped(content, shiftPressed);
		}

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }
	}
}