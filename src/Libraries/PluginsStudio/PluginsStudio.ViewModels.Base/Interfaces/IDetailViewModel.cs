namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

/// <summary>
///		Interface para los viewModel de detalles
/// </summary>
public interface IDetailViewModel : IDocumentViewModel
{
	/// <summary>
	///		Inicia una búsqueda (por ejemplo, abre la ventana de búsqueda)
	/// </summary>
	public void StartSearch() {}

	/// <summary>
	///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
	/// </summary>
	string GetSaveAndCloseMessage();

	/// <summary>
	///		Graba los datos
	/// </summary>
	void SaveDetails(bool newName);
}
