namespace Bau.Libraries.LibReporting.Models.Configuration;

/// <summary>
///		Dats de configuración del proveedor de base de datos
/// </summary>
public class ProviderConfigurationModel
{
	/// <summary>
	///		Carácter de inicio de los nombres de campo
	/// </summary>
	public string CharFieldNameStart { get; set; } = "[";

	/// <summary>
	///		Carácter de fin de los nombres de campo
	/// </summary>
	public string CharFieldNameEnd { get; set; } = "]";

	/// <summary>
	///		Función "IsNull"
	/// </summary>
	public string FunctionIsNull { get; set; } = "IsNull";
}
