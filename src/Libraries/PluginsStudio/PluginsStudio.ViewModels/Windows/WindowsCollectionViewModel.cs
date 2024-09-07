using System.Collections.ObjectModel;

namespace Bau.Libraries.PluginsStudio.ViewModels.Windows;

/// <summary>
///		Colección de <see cref="WindowViewModel"/>
/// </summary>
public class WindowsCollectionViewModel
{
	public WindowsCollectionViewModel(PluginsStudioViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
	}

	/// <summary>
	///		Añade un panel a la colección
	/// </summary>
	public void AddPane(string tabId, string paneId, string name)
	{
		if (!ExistsPanel(paneId))
			CreateWindow(tabId, paneId, name, WindowViewModel.WindowType.Pane);
	}

	/// <summary>
	///		Elimina un panel de la colección
	/// </summary>
	public void AddDocument(string tabId, string name)
	{
		CreateWindow(tabId, tabId, name, WindowViewModel.WindowType.Document);
	}

	/// <summary>
	///		Crea una ventana
	/// </summary>
	private void CreateWindow(string tabId, string documentId, string name, WindowViewModel.WindowType type)
	{
		if (type == WindowViewModel.WindowType.Pane)
			Panels.Add(new WindowViewModel(MainViewModel, tabId, documentId, name, type));
		else
			Documents.Add(new WindowViewModel(MainViewModel, tabId, documentId, name, type));
	}

	/// <summary>
	///		Cierra una ventana
	/// </summary>
	public void Close(string id)
	{
		Close(id, Documents);
		Close(id, Panels);
	}

	/// <summary>
	///		Cierra una ventana o un panel
	/// </summary>
	private void Close(string id, ObservableCollection<WindowViewModel> documents)
	{
		for (int index = documents.Count - 1; index >= 0; index--)
			if (documents[index].Id.Equals(id, StringComparison.CurrentCultureIgnoreCase))
				Close(documents[index]);
	}

	/// <summary>
	///		Comprueba si existe un panel
	/// </summary>
	private bool ExistsPanel(string paneId) => Exists(paneId, Panels);

	/// <summary>
	///		Comprueba si existe un documento
	/// </summary>
	private bool Exists(string id, ObservableCollection<WindowViewModel> documents) => documents.Any(item => item.DocumentId.Equals(id, StringComparison.CurrentCultureIgnoreCase));

	/// <summary>
	///		Marca una ventana como cerrada
	/// </summary>
	private void Close(WindowViewModel viewModel)
	{
		if (viewModel.Type == WindowViewModel.WindowType.Pane)
			viewModel.Visible = false;
		else
			Documents.Remove(viewModel);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Ventanas
	/// </summary>
	public ObservableCollection<WindowViewModel> Documents { get; } = [];

	/// <summary>
	///		Ventanas
	/// </summary>
	public ObservableCollection<WindowViewModel> Panels { get; } = [];
}