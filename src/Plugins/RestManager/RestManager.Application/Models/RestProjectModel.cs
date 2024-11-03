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
	///		Conexiones
	/// </summary>
	public ConnectionsCollectionModel Connections { get; } = new();

	/// <summary>
	///		Parámetros del proyecto
	/// </summary>
	public ParametersCollectionModel Parameters { get; } = new();

	/// <summary>
	///		Pasos
	/// </summary>
	public List<BaseStepModel> Steps { get; } = [];
}
