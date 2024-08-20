namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers.EventArguments;

/// <summary>
///		Argumentos del evento de solicitud de texto seleccionado en un editor
/// </summary>
public class EditorSelectedTextRequiredEventArgs : EventArgs
{
	public EditorSelectedTextRequiredEventArgs(string? selectedText = null)
	{
		SelectedText = selectedText;
	}

	/// <summary>
	///		Texto seleccionado
	/// </summary>
	public string? SelectedText { get; set; }
}
