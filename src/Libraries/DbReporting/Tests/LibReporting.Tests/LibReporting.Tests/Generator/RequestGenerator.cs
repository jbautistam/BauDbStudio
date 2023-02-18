using Bau.Libraries.LibReporting.Requests.Models;
using Bau.Libraries.LibReporting.Solution;

namespace LibReporting.Tests.Generator;

/// <summary>
///		Generador de archivos de respuesta a partir de las solicitudes
/// </summary>
public class RequestGenerator
{
	#if DEBUG
	[Theory(Skip = "Request by user")]
	[InlineData("ReportingSales/Test-Reporting-Schema.xml")]
	public void GenerateResponseFiles(string schemaFile)
	{
		string pathData = Tools.FileHelper.GetDevelopmentDataPath();

			if (string.IsNullOrWhiteSpace(pathData) || !Directory.Exists(pathData))
				throw new ArgumentException("Can't find project path");
			else
			{
				string folder = Path.Combine(pathData, Path.GetDirectoryName(schemaFile) ?? string.Empty, "Requests");

					// Obtiene todos los archivos de solicitud
					foreach (string fileName in Directory.GetFiles(Tools.FileHelper.GetFullFileName(folder), "*.xml"))
						GenerateResponse(schemaFile, fileName);
			}
	}

	/// <summary>
	///		Genera el archivo de respuesta
	/// </summary>
	private void GenerateResponse(string schemaFile, string requestFile)
	{
		ReportingSolutionManager manager = new();
		ReportRequestModel request = manager.LoadRequest(requestFile);

			// Agrega el dataWarehouse
			manager.AddDataWarehouse(Tools.FileHelper.GetFullFileName(schemaFile));
			// Obtiene la SQL del informe y la guarda en el archivo de respuesta
			SaveFile(Tools.FileHelper.GetResponseFile(requestFile), manager.GetSqlResponse(request));
	}

	/// <summary>
	///		Graba el texto de un archivo de respuesta
	/// </summary>
	private void SaveFile(string fileName, string sql)
	{
		File.WriteAllText(fileName, sql);
	}
	#endif
}
