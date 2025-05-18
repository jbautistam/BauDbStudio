using System.Collections.ObjectModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

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
		if (!Exists(paneId, Panels))
			CreatePanel(tabId, paneId, name);
	}

	/// <summary>
	///		Crea los paneles por orden del nombre
	/// </summary>
	private void CreatePanel(string tabId, string documentId, string name)
	{
		int beforeIndex = -1;

			// Busca el primer panel con nombe superior al que se quiere insertar
			for (int index = 0; index < Panels.Count; index++)
				if (beforeIndex < 0 && Panels[index].Name.CompareTo(name) > 0)
					beforeIndex = index;
			// Normaliza el índice
			if (beforeIndex < 0)
				beforeIndex = Panels.Count;
			// Añade el panel en la posición adecuada
			Panels.Insert(beforeIndex, new WindowViewModel(MainViewModel, tabId, documentId, name, WindowViewModel.WindowType.Pane));
	}

	/// <summary>
	///		Añade un documento a la colección de ventanas
	/// </summary>
	public void AddDocument(string tabId, string name)
	{
		// Crea la ventana si no existía
		if (!Exists(tabId, Documents))
			Documents.Add(new WindowViewModel(MainViewModel, tabId, tabId, name, WindowViewModel.WindowType.Document));
		// Cambia la ventana activa
		SetActiveDocument(tabId);
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
	///		Cambia el documento activo
	/// </summary>
	public void SetActiveDocument(IDetailViewModel details)
	{
		SetActiveDocument(details.TabId);
	}

	/// <summary>
	///		Cambia el documento activo
	/// </summary>
	public void SetActiveDocument(string tabId)
	{
		foreach (WindowViewModel viewModel in Documents)
			if (viewModel.Id.Equals(tabId, StringComparison.CurrentCultureIgnoreCase))
				viewModel.Active = true;
			else
				viewModel.Active = false;
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