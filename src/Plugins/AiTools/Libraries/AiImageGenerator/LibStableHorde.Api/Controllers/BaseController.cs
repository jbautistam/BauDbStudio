namespace Bau.Libraries.LibStableHorde.Api.Controllers;

/// <summary>
///		Base para los controladores
/// </summary>
internal abstract class BaseController
{
	protected BaseController(StableHordeManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Manager
	/// </summary>
	internal StableHordeManager Manager { get; }
}
