using System.Data.SqlClient;
using FluentAssertions;

using Bau.Libraries.LibReporting.Solution;
using Bau.Libraries.LibReporting.Requests.Models;
using AnalyticAlways.RoiReporting.Service;

namespace LibReporting.Tests;

/// <summary>
///		Pruebas de ejecución de una consulta de informes y su transformación a JSON con una plantilla
/// </summary>
public class report_execution_json_should
{
	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes y ejecutar la cadena SQL contra la base de datos
	/// </summary>
	[Fact]
	public void convert_to_json()
	{
		Dictionary<string, List<string>> reports = Tools.FileHelper.GetReports();
		string error = string.Empty;

			// Recorre los esquemas e informes procesando las solicitudes / respuestas
			foreach (KeyValuePair<string, List<string>> report in reports)
				foreach (string reportFile in report.Value)
				{
					string pathRequest = Path.Combine(Path.GetDirectoryName(reportFile) ?? string.Empty, 
													  reportFile.Substring(0, reportFile.Length - ".report.xml".Length));

						if (!Directory.Exists(pathRequest))
							error += $"Can't find request for report '{Path.GetFileName(reportFile)}'";
						else
							foreach (string requestFile in Directory.GetFiles(pathRequest, "*.request.xml"))
							{
								string executionError = ExecuteTemplate(report.Key, requestFile);

									if (!string.IsNullOrWhiteSpace(executionError))
										error += executionError + Environment.NewLine;
							}
				}
			// Comprueba los errores
			error.Should().BeNullOrWhiteSpace();
	}

	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes y ejecutar la cadena SQL contra la base de datos
	///	(el método execute_files_to_sql lo hace para todos los archivos, este es sólo por si queremos ejecutar uno en concreto)
	/// </summary>
	[Theory]
	[InlineData("ReportingRoi/Test-Reporting-Roi.Reporting.xml",
				"ReportingRoi/Replenishment/NoDimensions.request.xml")]
	public void convert_to_json_file(string fileName, string fileRequest)
	{
		ExecuteTemplate(Tools.FileHelper.GetFullFileName(fileName),
						Tools.FileHelper.GetFullFileName(fileRequest))
				.Should().BeNullOrWhiteSpace();
	}

	/// <summary>
	///		Ejecuta la generación de SQL, la ejecución en base de datos y la conversión. Compara
	///	el resultado con el archivo de salida
	/// </summary>
	private string ExecuteTemplate(string schemaFile, string requestFile)
	{
		string error = string.Empty;

			// Comprueba y ejecuta la plantilla
			if (!File.Exists(schemaFile))
				error = $"Can't find the file '{schemaFile}'";
			else if (!File.Exists(requestFile))
				error = $"Can't find the file request '{requestFile}'";
			else
			{
				string output = Tools.FileHelper.GetOutputResponseFileName(requestFile);

					if (!File.Exists(output))
						error = $"Can't find the file for output '{output}'";
					else
					{
						try
						{
							string json = ExecuteOutput(schemaFile, requestFile);

								if (!CompareJson(json, File.ReadAllText(output)))
									error = $"Error when compare result for request '{requestFile}'";
						}
						catch (Exception exception)
						{
							error = $"Error when convert report. {exception.Message}";
						}
					}
			}
			// Devuelve la cadena de error
			return error;
	}

	/// <summary>
	///		Ejecuta la SQL generada por una solicitud sobre la base de datos y procesa la plantilla
	/// </summary>
	private string ExecuteOutput(string schemaFile, string requestFile)
	{
		string output = string.Empty;
		ReportingSolutionManager manager = new();
		ReportRequestModel request = manager.LoadRequest(requestFile);

			// Cambia la paginación
			request.Pagination.Page = 1;
			request.Pagination.RecordsPerPage = 20;
			// Agrega el dataWarehouse
			manager.AddDataWarehouse(schemaFile);
			// Comprueba que realmente se haya cargado una solicitud
			request.Should().NotBeNull();
			// Obtiene la SQL del informe
			using (SqlConnection connection = new(Tools.ConnectionsHelper.GetConnectionStringForSchema(schemaFile)))
			{
				SqlCommand command = connection.CreateCommand();

					// Añade los argumentos al comando
					foreach (KeyValuePair<string, object?> parameter in request.Parameters)
					{
						SqlParameter sqlParameter = command.CreateParameter();
						string key = parameter.Key;

							// Normaliza el parámetro
							if (!key.StartsWith("@"))
								key = "@" + key;
							// Asigna el parámetro
							sqlParameter.ParameterName = key;
							sqlParameter.Value = parameter.Value;
							// Añade el parámetro a la colección
							command.Parameters.Add(sqlParameter);
					}
					// Asigna las cadena al comando
					command.CommandTimeout = (int) TimeSpan.FromMinutes(2).TotalSeconds;
					command.CommandText = manager.GetSqlResponse(request);
					command.CommandType = System.Data.CommandType.Text;
					// Abre la conexión
					connection.Open();
					// Ejecuta la consulta SQL
					output = ConvertToJson(schemaFile, command.ExecuteReader(), request);
					// Cierra la conexión
					connection.Close();
			}
			// Devuelve la cadena convertida
			return output;
	}

	/// <summary>
	///		Convierte el reader en una cadena JSON utilizando la plantilla
	/// </summary>
	private string ConvertToJson(string schemaFile, System.Data.IDataReader reader, ReportRequestModel request)
	{
		ReaderService service = GetService(schemaFile);
        AnalyticAlways.RoiReporting.Service.Models.Request output = new(request.Pagination.Page, request.Pagination.RecordsPerPage);

			// Carga las plantillas del directorio
			service.Load();
			// Convierte el datareader a JSON utilizando la plantilla
			return service.TransformToJson(request.ReportId, output, reader);
	}

	/// <summary>
	///		Obtiene el servicio
	/// </summary>
	private ReaderService GetService(string schemaFile)
	{
		AnalyticAlways.RoiReporting.Service.Models.Configuration configuration = new(Path.Combine(Path.GetDirectoryName(schemaFile) ?? string.Empty, "_OutputTemplates"));

			return new ReaderService(configuration);
	}

	/// <summary>
	///		Compara las cadenas JSON generadas y leidas del archivo
	/// </summary>
	private bool CompareJson(string generated, string readed)
	{
		// Log
		System.Diagnostics.Debug.WriteLine("Compare generated " + new string('-', 80));
		System.Diagnostics.Debug.WriteLine(Normalize(generated));
		System.Diagnostics.Debug.WriteLine("Compare source " + new string('-', 80));
		System.Diagnostics.Debug.WriteLine(Normalize(readed));
		System.Diagnostics.Debug.WriteLine(new string('-', 80));
		// Compara
		return Normalize(generated).Equals(Normalize(readed));
	}

	/// <summary>
	///		Normaliza una cadena JSON
	/// </summary>
	private string Normalize(string json)
	{
		// Quita saltos de línea y tabuladores
		json = json.Replace('\n', ' ');
		json = json.Replace('\r', ' ');
		json = json.Replace('\t', ' ');
		// Quita espacios dobles
		while (!string.IsNullOrWhiteSpace(json) && json.IndexOf("  ") >= 0)
			json = json.Replace("  ", " ");
		// Quita los malos paréntesis, corchetes...
		json = json.Replace("( ", "(");
		json = json.Replace(" )", ")");
		json = json.Replace("[ ", "[");
		json = json.Replace(" ]", "]");
		json = json.Replace("{ ", "{");
		json = json.Replace(" }", "}");
		json = json.Replace(" :", ":");
		json = json.Replace(": ", ":");
		// Quita espacios iniciales / finales
		if (!string.IsNullOrWhiteSpace(json))
			json = json.Trim();
		// Devuelve la cadena normalizada
		return json;
	}
}