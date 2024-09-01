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
	public void AddPane(string id, string name)
	{
		CreateWindow(id, name, WindowViewModel.WindowType.Pane);
	}

	/// <summary>
	///		Elimina un panel de la colección
	/// </summary>
	public void AddDocument(string id, string name)
	{
		CreateWindow(id, name, WindowViewModel.WindowType.Document);
	}

	/// <summary>
	///		Crea una ventana
	/// </summary>
	private void CreateWindow(string id, string name, WindowViewModel.WindowType type)
	{
		Items.Add(new WindowViewModel(MainViewModel, id, name, type));
	}

	/// <summary>
	///		Cierra una ventana
	/// </summary>
	public void Close(string id)
	{
		for (int index = Items.Count - 1; index >= 0; index--)
			if (Items[index].Id.Equals(id, StringComparison.CurrentCultureIgnoreCase))
				Close(Items[index]);
	}

	/// <summary>
	///		Marca una ventana como cerrada
	/// </summary>
	private void Close(WindowViewModel viewModel)
	{
		if (viewModel.Type == WindowViewModel.WindowType.Pane)
			viewModel.Visible = false;
		else
			Items.Remove(viewModel);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Ventanas
	/// </summary>
	public ObservableCollection<WindowViewModel> Items { get; } = [];
}