namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

/// <summary>
///		ViewModel para los documentos abiertos en paneles o documentos de la ventana principal
/// </summary>
public interface IDocumentViewModel
{
	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	void Execute(Models.Commands.ExternalCommand externalCommand);

	/// <summary>
	///		Cierra la ventana
	/// </summary>
	void Close();

	/// <summary>
	///		Título de la ficha
	/// </summary>
	string Header { get; }

	/// <summary>
	///		Id de la ficha
	/// </summary>
	string TabId { get; }

	/// <summary>
	///		Indica si se ha modificado el ViewModel
	/// </summary>
	bool IsUpdated { get; set; }
}
