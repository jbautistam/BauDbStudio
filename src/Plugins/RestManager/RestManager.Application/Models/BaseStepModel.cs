namespace Bau.Libraries.RestManager.Application.Models;

/// <summary>
///		Base para los pasos
/// </summary>
public abstract class BaseStepModel
{
	/// <summary>
	///		Indica si el paso está activo
	/// </summary>
	public bool Enabled { get; set; } = true;
}
