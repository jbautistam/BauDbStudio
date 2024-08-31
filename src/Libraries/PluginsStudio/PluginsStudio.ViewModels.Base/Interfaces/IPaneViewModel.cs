namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

/// <summary>
///		ViewModel para los paneles
/// </summary>
public interface IPaneViewModel : IDocumentViewModel
{
	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	void Execute(Models.Commands.ExternalCommand externalCommand);
}
