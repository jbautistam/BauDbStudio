namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Base para los pasos
/// </summary>
public abstract class BaseStepModel(RestProjectModel project)
{
	/// <summary>
	///		<see cref="RestProjectModel"/> al que se asocia el <see cref="RestStepModel"/>
	/// </summary>
	public RestProjectModel Project { get; } = project;

	/// <summary>
	///		Indica si el paso está activo
	/// </summary>
	public bool Enabled { get; set; } = true;
}
