namespace Bau.Libraries.LibStableHorde.Api.Controllers;

/// <summary>
///		Controlador para llamadas a rutinas relacionadas con modelos
/// </summary>
internal class ModelsController : BaseController
{
	internal ModelsController(StableHordeManager manager) : base(manager) {}

	/// <summary>
	///		Obtiene los modelos que se pueden utilizar
	/// </summary>
	internal async Task<List<Dtos.ModelDto>?> GetAsync(CancellationToken cancellationToken)
	{
		return await Manager.RestManager.GetResponseDataAsync<List<Dtos.ModelDto>>("/api/v2/status/models", cancellationToken);
	}
}
