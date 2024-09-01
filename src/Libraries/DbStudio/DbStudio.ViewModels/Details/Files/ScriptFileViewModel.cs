using Microsoft.Extensions.Logging;

using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.DbScripts.Manager.Builders;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Files;

/// <summary>
///		ViewModel para un archivo
/// </summary>
public class ScriptFileViewModel : PluginsStudio.ViewModels.Base.Files.BaseTextFileViewModel
{
	public ScriptFileViewModel(DbStudioViewModel solutionViewModel, string fileName) 
				: base(solutionViewModel.MainController.PluginController, fileName, "Sql script (*.sql)|*.sql|Sql extended (*.sqlx)|*.sqlx")
	{
		SolutionViewModel = solutionViewModel;
	}

	/// <summary>
	///		Ejecuta el script
	/// </summary>
	internal async Task ExecuteSqlScriptAsync(ConnectionModel connection, ArgumentListModel arguments, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(Content))
			SolutionViewModel.MainController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
		else if (connection == null)
			SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione una conexión");
		else 
		{
			string? selectedText = GetEditorSelectedText();

				// Log
				SolutionViewModel.Manager.Logger.LogInformation($"Comienza la ejecución de la consulta");
				// Si no hay nada seleccionado, se ejecuta todo el contenido
				if (string.IsNullOrWhiteSpace(selectedText))
					selectedText = Content;
				// Ejecuta la consulta
				if (FileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
					await SolutionViewModel.Manager.ExecuteQueryAsync(GetQuery(connection, selectedText, arguments), cancellationToken);
				else if (FileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
					await SolutionViewModel.Manager.ExecuteQueryAsync(GetQuery(connection, selectedText, arguments), cancellationToken);
				else
					SolutionViewModel.Manager.Logger.LogError("No se reconoce el tipo de archivo como SQL");
				// Muestra el tiempo de ejecución
				SolutionViewModel.Manager.Logger.LogInformation($"Tiempo de ejecución: {SolutionViewModel.ConnectionExecutionViewModel.ExecutionTime}");
		}
	}

	/// <summary>
	///		Obtiene la consulta
	/// </summary>
	private QueryModel GetQuery(ConnectionModel connection, string query, ArgumentListModel arguments)
	{
		QueryBuilder builder = new(connection);

			// Añade los datos de la consulta
			builder.WithSql(query, true);
			builder.WithArguments(arguments);
			builder.WithTimeout(connection.TimeoutExecuteScript);
			// Devuelve la consulta
			return builder.Build();
	}

	/// <summary>
	///		Obtiene la cadena asociada a un archivo cuando se arrastra un nodo sobre el editor de texto
	/// </summary>
	public override async Task<string> TreatTextDroppedAsync(string content, bool shiftPressed, CancellationToken cancellationToken)
	{
		return await new Controllers.DropItems.NodeTextDropHelper(false).TreatTextDroppedAsync(content, shiftPressed, cancellationToken);
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public override void Execute(PluginsStudio.ViewModels.Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Solución
	/// </summary>
	public DbStudioViewModel SolutionViewModel { get; }
}