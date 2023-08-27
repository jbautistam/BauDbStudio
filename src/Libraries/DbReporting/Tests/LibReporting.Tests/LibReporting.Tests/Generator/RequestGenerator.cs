using FluentAssertions;

namespace LibReporting.Tests.Generator;

/// <summary>
///		Generador de archivos de respuesta a partir de las solicitudes
/// </summary>
public class RequestGenerator
{
	#if DEBUG
	[Fact(Skip = "Sólo cuando sea necesario regenerar las respuestas")]
	public void GenerateResponseFiles()
	{
		Dictionary<string, List<string>> reports = Tools.FileHelper.GetReports();
		string error = string.Empty;

			// Recorre los esquemas e informes procesando las solicitudes / respuestas
			foreach (KeyValuePair<string, List<string>> report in reports)
				foreach (string reportFile in report.Value)
				{
					string pathRequest = Path.Combine(Path.GetDirectoryName(reportFile) ?? string.Empty, 
													  reportFile[..^".report.xml".Length]);

						if (!Directory.Exists(pathRequest))
							error += $"Can't find request for report '{Path.GetFileName(reportFile)}'";
						else
							foreach (string requestFile in Directory.GetFiles(pathRequest, "*.request.xml"))
								try
								{
									GenerateResponse(report.Key, requestFile);
								}
								catch (Exception exception)
								{
									error += $"Error when process {requestFile}. {exception.Message}" + Environment.NewLine;
								}
				}
			// Comprueba los errores
			error.Should().BeNullOrWhiteSpace();
	}

	/// <summary>
	///		Genera los archivo de respuesta de una solicitud
	/// </summary>
	private void GenerateResponse(string schemaFile, string requestFile)
	{
		for (int page = 0; page < 3; page++)
			SaveFile(Tools.FileHelper.GetResponseFile(requestFile, page), 
					 Tools.ReportHelper.GetSqlResponse(schemaFile, requestFile, page));
	}

	/// <summary>
	///		Graba el texto de un archivo de respuesta
	/// </summary>
	private void SaveFile(string fileName, string sql)
	{
		// Quita el directorio de depuración para que se graben en el archivo de proyectos
		fileName = fileName.Replace("\\bin\\Debug\\net7.0", string.Empty);
		fileName = fileName.Replace("/bin/Debug/net7.0", string.Empty);
		// Escribe el archivo
		File.WriteAllText(fileName, sql);
	}
	#endif
}