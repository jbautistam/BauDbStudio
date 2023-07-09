namespace LibReporting.Tests.Tools;

/// <summary>
///		Clase de ayuda para tratamiento de archivos
/// </summary>
internal static class FileHelper
{
	/// <summary>
	///		Obtiene el nombre completo de un archivo
	/// </summary>
	internal static string GetFullFileName(string fileName) => Path.Combine(GetDataPath(), fileName);

	/// <summary>
	///		Obtiene el directorio de archivos de datos del proyecto
	/// </summary>
	internal static string GetDataPath() => Path.Combine(GetExecutionPath(), "Data");

	/// <summary>
	///		Obtiene el directorio de ejecución del proyecto
	/// </summary>
	private static string GetExecutionPath() => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? default!;

	/// <summary>
	///		Obtiene el directorio de desarrollo del proyecto
	/// </summary>
	internal static string GetDevelopmentDataPath()
	{
		string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? default!;

			// Busca el directorio del proyecto
			while (!string.IsNullOrWhiteSpace(path) && !File.Exists(Path.Combine(path, "LibReporting.Tests.csproj")))
				path = Path.GetDirectoryName(path) ?? string.Empty;
			// Combina el directorio de datos
			if (!string.IsNullOrWhiteSpace(path))
				path = Path.Combine(path, "Data");
			// Devuelve el directorio
			return path;
	}

	/// <summary>
	///		Obtiene el nombre del archivo de respuesta
	/// </summary>
	internal static string GetResponseFile(string requestFile)
	{
		string responseFile = Path.GetFileNameWithoutExtension(requestFile);

			// Obtiene el nombre del archivo de respuesta
			if (responseFile.Contains(".request.", StringComparison.CurrentCultureIgnoreCase))
			{
				// Cambia "Request_" por "Response_"
				responseFile = responseFile.Replace(".request.", ".response.") + "sql";
				// Devuelve el nombre completo del archivo
				return Path.Combine(Path.GetDirectoryName(requestFile) ?? string.Empty, responseFile);
			}
			else
				throw new NotImplementedException($"Error request file name {requestFile}");
	}
}
