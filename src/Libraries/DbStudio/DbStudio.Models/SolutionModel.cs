namespace Bau.Libraries.DbStudio.Models;

/// <summary>
///		Clase con los datos de la solución
/// </summary>
public class SolutionModel : LibDataStructures.Base.BaseExtendedModel
{
	// Variables privadas
	private string _lastConnectionParameters = string.Empty;

	/// <summary>
	///		Añade el parámetro
	/// </summary>
	private void AddConnectionParameter(string parametersFile)
	{
		if (!string.IsNullOrWhiteSpace(parametersFile))
			QueueConnectionParameters.Add(parametersFile);
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; set; } = string.Empty;

	/// <summary>
	///		Directorio base de la solución
	/// </summary>
	public string Path
	{
		get 
		{
			if (string.IsNullOrWhiteSpace(FileName))
				return string.Empty;
			else
				return System.IO.Path.GetDirectoryName(FileName) ?? string.Empty;
		}
	}

	/// <summary>
	///		Nombre del último archivo de parámetros de conexión seleccionado
	/// </summary>
	public string LastConnectionParametersFileName 
	{ 
		get { return _lastConnectionParameters; }
		set
		{
			// Guarda el parámetro
			_lastConnectionParameters = value;
			// lo añade a la cola
			AddConnectionParameter(_lastConnectionParameters);
		}
	}

	/// <summary>
	///		Cola de los últimos archivos de parámetros
	/// </summary>
	public LibDataStructures.Collections.QueueLimited<string> QueueConnectionParameters { get; } = new();

	/// <summary>
	///		Nombre del último archivo de parámetros para proyectos ETL seleccionado
	/// </summary>
	public string LastEtlParametersFileName { get; set; } = string.Empty;

	/// <summary>
	///		Id de la última conexión seleccionada
	/// </summary>
	public string LastConnectionSelectedGlobalId { get; set; } = string.Empty;

	/// <summary>
	///		Conexiones
	/// </summary>
	public Connections.ConnectionModelCollection Connections { get; } = new();
}
