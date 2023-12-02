namespace Bau.Libraries.DbStudio.Application.Controllers.EtlProjects;

/// <summary>
///		Opciones para la generación del archivo de importación
/// </summary>
public class ScriptsImportOptions
{
	/// <summary>
	///		Conexión
	/// </summary>
	public Models.Connections.ConnectionModel Connection { get; set; } 

	/// <summary>
	///		Variable con el nombre de base de datos
	/// </summary>
	public string DataBaseVariable { get; set; }

	/// <summary>
	///		Prefijo de las tablas de salida
	/// </summary>
	public string PrefixOutputTable { get; set; }

	/// <summary>
	///		Variable con el directorio de montaje
	/// </summary>
	public string MountPathVariable { get; set; }

	/// <summary>
	///		Subdirectorio donde se encuentran los archivos de validación
	/// </summary>
	public string SubPath { get; set; }

	/// <summary>
	///		Directorio donde se encuentran los archivos a importar
	/// </summary>
	public string PathInputFiles { get; set; }

	/// <summary>
	///		Nombre del archivo de salida
	/// </summary>
	public string OutputFileName { get; set; }
}
