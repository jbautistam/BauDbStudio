using System.Collections.ObjectModel;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.AiTools.Application.Models.Prompts;

namespace Bau.Libraries.AiTools.ViewModels.Prompts;

/// <summary>
///		ViewModel de una versión
/// </summary>
public class PromptVersionListViewModel : BaseObservableObject
{
	// Eventos públicos
	public event EventHandler? Compiled;
	// Variables privadas
	private PromptVersionViewModel? _selectedItem, _previousSelectedItem;

	public PromptVersionListViewModel(PromptFileViewModel promptFileViewModel)
	{
		PromptFileViewModel = promptFileViewModel;
	}

	/// <summary>
	///		Añade una versión (si hay alguna en la lista copia los datos)
	/// </summary>
	public void Add()
	{
		// Añade una versión
		if (SelectedItem is not null)
			Add(SelectedItem.GetPrompt().Clone(GetNewVersion()));
		else
			Add(new PromptModel(string.Empty));
		// Selecciona el último elemento
		SelectLast();

		// Obtiene la siguiente versión
		int GetNewVersion()
		{
			int version = 1;

				// Obtiene el número de versión
				foreach (PromptVersionViewModel promptVersionViewModel in PromptFileViewModel.VersionsViewModel.Items)
					if (promptVersionViewModel.Version >= version)
						version = promptVersionViewModel.Version + 1;
				// Devuelve el número de versión
				return version;
		}
	}

	/// <summary>
	///		Añade un prompt a la lista
	/// </summary>
	public void Add(PromptModel prompt)
	{
		PromptVersionViewModel viewModel = new(this, prompt);

			// Asigna el manejador de eventos
			viewModel.PropertyChanged += (sender, args) => {
																if (!string.IsNullOrWhiteSpace(args.PropertyName))
																	IsUpdated = true;
															};
			// Añade el elemento
			Items.Add(viewModel);
	}

	/// <summary>
	///		Selecciona el último elemento
	/// </summary>
	public void SelectLast()
	{
		if (Items.Count > 0)
			SelectedItem = Items[Items.Count - 1];
		else
			SelectedItem = null;
	}

	/// <summary>
	///		Limpia los elementos
	/// </summary>
	internal void Clear()
	{
		Items.Clear();
		SelectedItem = null;
	}

	/// <summary>
	///		Borra una versión
	/// </summary>
	internal void Delete()
	{
		if (SelectedItem is not null)
		{
			// Elimina el elemento seleccionado
			Items.Remove(SelectedItem);
			// Añade un elemento si no hay nada
			if (Items.Count == 0)
				Add(new PromptModel(string.Empty));
			// Selecciona el último elemento
			SelectLast();
		}
	}

	/// <summary>
	///		Compila el elemento seleccionado
	/// </summary>
	internal async Task CompileAsync(CancellationToken cancellationToken)
	{
		if (SelectedItem is not null)
		{
			Controllers.Processors.AiImageProcessor processor = new(PromptFileViewModel.MainViewModel, SelectedItem, SelectedItem.Positive);

				// Genera el prompt
				await PromptFileViewModel.MainViewModel.ViewsController.MainWindowController.EnqueueProcessAsync(processor, cancellationToken);
		}
	}

	/// <summary>
	///		Carga las imágenes
	/// </summary>
	internal void LoadImages()
	{
		if (SelectedItem is not null)
			SelectedItem.ImagesViewModel.Load();
	}

	/// <summary>
	///		ViewModel del archivo
	/// </summary>
	public PromptFileViewModel PromptFileViewModel { get; }

	/// <summary>
	///		Elementos de la lista
	/// </summary>
	public ObservableCollection<PromptVersionViewModel> Items { get; } = new();

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public PromptVersionViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set 
		{ 
			PreviousSelectedItem = _selectedItem;
			if (CheckObject(ref _selectedItem, value) && _selectedItem is not null)
				_selectedItem.ImagesViewModel.Load();
		}
	}

	/// <summary>
	///		Elemento seleccionado anterior
	/// </summary>
	public PromptVersionViewModel? PreviousSelectedItem
	{
		get { return _previousSelectedItem; }
		set { CheckObject(ref _previousSelectedItem, value); }
	}
}
