using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.Application.Controllers.EtlProjects;

/// <summary>
///		Opciones para la generación de los archivos de validación
/// </summary>
public class ScriptsValidationOptions
{
	/// <summary>
	///		Modo de validación
	/// </summary>
	public enum ValidationMode
	{
		/// <summary>Comprueba archivos</summary>
		Files,
		/// <summary>Comprueba base de datos</summary>
		Database
	}

	/// <summary>
	///		Conexión
	/// </summary>
	public ConnectionModel Connection { get; set; } 
	
	/// <summary>
	///		Tablas para las que se genera la validación
	/// </summary>
	public List<ConnectionTableModel> Tables { get; set; } = new();

	/// <summary>
	///		Directorio de salida de los archivos
	/// </summary>
	public string OutputPath { get; set; } 

	/// <summary>
	///		Nombre de la variable de base de datos de cálculo
	/// </summary>
	public string DataBaseComputeVariable { get; set; }

	/// <summary>
	///		Nombre de la variable de base de datos de validación
	/// </summary>
	public string DataBaseValidateVariable { get; set; }

	/// <summary>
	///		Indica el modo de validación
	/// </summary>
	public ValidationMode Mode { get; set; }

	/// <summary>
	///		Nombre de la variable de directorio de archivos
	/// </summary>
	public string MountPathVariable { get; set; }

	/// <summary>
	///		Contenido de la variable de directorio de archivos
	/// </summary>
	public string MountPathContent { get; set; }
	
	/// <summary>
	///		Formato de los archivos
	/// </summary>
	public SolutionManager.FormatType FormatType { get; set; }

	/// <summary>
	///		Subdirectorio de validación
	/// </summary>
	public string SubpathValidate { get; set; }

	/// <summary>
	///		Base de datos a comparar
	/// </summary>
	public string DatabaseTarget { get; set; }

	/// <summary>
	///		Indica si se debe generar un archivo QVS de validación
	/// </summary>
	public bool GenerateQvs { get; set; }

	/// <summary>
	///		Prefijos a eliminar en las tablas al compararlas con archivos separadas por punto y coma (por ejemplo, SRC_; TMP_...)
	/// </summary>
	public string TablePrefixes { get; set; }

	/// <summary>
	///		Indica si en el archivo se van a comparar cadena
	/// </summary>
	public bool CompareString { get; set; }

	/// <summary>
	///		Formato de fechas
	/// </summary>
	public string DateFormat { get; set; }

	/// <summary>
	///		Separador decimal
	/// </summary>
	public string DecimalSeparator { get; set; }

	/// <summary>
	///		Tipo para los coampos decimales
	/// </summary>
	public string DecimalType { get; set; }

	/// <summary>
	///		Campos de tipo bit (se comparan utilizando ABS)
	/// </summary>
	public string BitFields { get; set; }
	
	/// <summary>
	///		Indica si las cadenas se deben comparar utilizando una expresión regular para sólo caracters alfabéticos y dígitos
	/// </summary>
	public bool CompareOnlyAlphaAndDigits { get; set; }
}
