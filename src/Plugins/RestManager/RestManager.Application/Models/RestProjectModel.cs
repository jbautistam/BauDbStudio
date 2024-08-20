namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Datos de un proyecto Rest
/// </summary>
public class RestProjectModel
{
	/// <summary>
	///		Nombre del proyecto
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	///		Descripción del proyecto
	/// </summary>
	public string Description { get; set; } = default!;

	/// <summary>
	///		Parámetros del proyecto
	/// </summary>
	public ParametersCollectionModel Parameters { get; } = new();

	/// <summary>
	///		Cabeceras del proyecto (generales)
	/// </summary>
	public ParametersCollectionModel Headers { get; } = new();

	/// <summary>
	///		Pasos
	/// </summary>
	public List<BaseStepModel> Steps { get; } = new();
}
