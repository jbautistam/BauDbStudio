using System;

using Bau.Libraries.DbStudio.Conversor;
using Bau.Libraries.DbStudio.Models.Deployments;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.DbStudio.Application.Controllers.Databricks
{
	/// <summary>
	///		Controlador del exportador de archivos a notebooks de databricks
	/// </summary>
	internal class DatabrickExporterController
	{
		internal DatabrickExporterController(SolutionManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Exporta los archivos
		/// </summary>
		internal void Export(DeploymentModel deployment)
		{
			using (BlockLogModel block = Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, $"Start deployment '{deployment.Name}'"))
			{
				(NormalizedDictionary<object> constants, string error) = GetParameters(deployment.JsonParameters);

					if (!string.IsNullOrWhiteSpace(error))
						block.Error(error);
					else
					{
						ExporterOptions options = new ExporterOptions(deployment.SourcePath, deployment.TargetPath);
						
							// Asigna las propiedades
							options.WriteComments = deployment.WriteComments;
							options.LowcaseFileNames = deployment.LowcaseFileNames;
							options.ReplaceArguments = deployment.ReplaceArguments;
							// Añade los parámetros
							foreach ((string key, object value) in constants.Enumerate())
								options.AddParameter(key, value);
							// Exporta los archivos
							new DatabrickExporter(Manager.Logger, options).Export();
					}
			}
		}

		/// <summary>
		///		Obtiene los parámetros de una cadena Json
		/// </summary>
		private (NormalizedDictionary<object> parameters, string error) GetParameters(string jsonParameters)
		{
			NormalizedDictionary<object> parameters = new NormalizedDictionary<object>();
			string error = string.Empty;

				// Carga los parámetros si es necesario
				if (!string.IsNullOrWhiteSpace(jsonParameters))
					try
					{
						System.Data.DataTable table = new LibJsonConversor.JsonToDataTableConversor().ConvertToDataTable(jsonParameters);

							// Crea la colección de parámetros a partir de la tabla
							if (table.Rows.Count == 0)
								error = "No se ha encontrado ningún parámetro en el archivo";
							else
								foreach (System.Data.DataColumn column in table.Columns)
									parameters.Add(column.ColumnName, table.Rows[0][column.Ordinal]);
					}
					catch (Exception exception)
					{
						error = $"Error cuando se cargaba el archivo de parámetros. {exception.Message}";
					}
				// Devuelve el resultado
				return (parameters, error);
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		internal SolutionManager Manager { get; }
	}
}
