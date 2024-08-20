namespace Bau.Libraries.DbStudio.Application.Controllers.Export;

/// <summary>
///		Parámetros de un archivo CSV
/// </summary>
public class CsvFileParameters
{
	/// <summary>
	///		Obtiene el modelo de archivo CSV a partir de los parámetros
	/// </summary>
	internal LibCsvFiles.Models.FileModel GetFileModel()
	{
		return new LibCsvFiles.Models.FileModel()
						{
							WithHeader = WithHeader,
							Separator = string.IsNullOrWhiteSpace(Separator) ? ',' : Separator[0],
							DecimalSeparator = string.IsNullOrWhiteSpace(DecimalSeparator) ? '.' : DecimalSeparator[0],
							DateFormat = DateFormat,
							TrueValue = TrueValue,
							FalseValue = FalseValue
						};
	}

	/// <summary>
	///		Indica si el origen de los datos tiene cabecera
	/// </summary>
	public bool WithHeader { get; set; }

	/// <summary>
	///		Separador de columnas
	/// </summary>
	public string Separator { get; set; } = ",";

	/// <summary>
	///		Formato de fecha
	/// </summary>
	public string DateFormat { get; set; } = "yyyy-MM-dd";

	/// <summary>
	///		Separador de decimales
	/// </summary>
	public string DecimalSeparator { get; set; } = ".";

	/// <summary>
	///		Cadena para los valores verdaderos
	/// </summary>
	public string TrueValue { get; set; } = "1";

	/// <summary>
	///		Cadena para los valores falsos
	/// </summary>
	public string FalseValue { get; set; } = "0";
}
