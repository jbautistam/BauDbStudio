using System.Collections.ObjectModel;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project;

/// <summary>
///		Lista de pasos de un proyecto Rest
/// </summary>
public class RestFileListStepsViewModel : BaseObservableObject
{
	// Variables privadas
	private ObservableCollection<RestFileStepItemViewModel>? _items = default!;
	private RestFileStepItemViewModel? _selectedItem;

	public RestFileListStepsViewModel(RestFileViewModel fileViewModel)
	{
		// Inicializa los objetos
		FileViewModel = fileViewModel;
		Items = new ObservableCollection<RestFileStepItemViewModel>();
		// Crea los comandos
		NewCommand = new BaseCommand(_ => UpdateStep(null));
		UpdateCommand = new BaseCommand(_ => UpdateStep(SelectedItem), _ => SelectedItem is not null)
								.AddListener(this, nameof(SelectedItem));
		DeleteCommand = new BaseCommand(_ => DeleteStep(SelectedItem), _ => SelectedItem is not null)
								.AddListener(this, nameof(SelectedItem));
	}

	/// <summary>
	///		Añade un paso
	/// </summary>
	internal void Add(RestStepModel restStep)
	{
		if (Items is not null)
			Items.Add(new RestFileStepItemViewModel(this, restStep));
	}

	/// <summary>
	///		Añade / modifica el paso
	/// </summary>
	private void UpdateStep(RestFileStepItemViewModel? stepViewModel)
	{
		// Crea el paso si es nuevo
		if (Items is not null && stepViewModel is null)
		{
			// Crea el paso
			stepViewModel = new RestFileStepItemViewModel(this, new RestStepModel
																		{
																			Method = RestStepModel.RestMethod.Get,
																			Url = "{{Url}}"
																		}
														 );
			// Añade el paso a la lista
			Items.Add(stepViewModel);
			// Indica que se ha modificado el elemento
			FileViewModel.IsUpdated = true;
		}
		// Selecciona el elemento
		SelectedItem = stepViewModel;
	}

	/// <summary>
	///		Borra el paso
	/// </summary>
	private void DeleteStep(RestFileStepItemViewModel? stepViewModel)
	{
		if (Items is not null && stepViewModel is not null &&
				FileViewModel.MainViewModel.ViewsController.HostController.
						SystemController.ShowQuestionCancel($"Do you want to delete the step '{stepViewModel.Url}'?") == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
		{
			// Elimina el elemento
			Items.Remove(stepViewModel);
			// Indica que se ha modificado el archivo
			FileViewModel.IsUpdated = true;
		}
	}

	/// <summary>
	///		ViewModel del archivo
	/// </summary>
	public RestFileViewModel FileViewModel { get; }

	/// <summary>
	///		Elementos
	/// </summary>
	public ObservableCollection<RestFileStepItemViewModel>? Items 
	{ 
		get { return _items; }
		set { CheckObject(ref _items, value); }
	}

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public RestFileStepItemViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Comando para crear un paso
	/// </summary>
	public BaseCommand NewCommand { get; }

	/// <summary>
	///		Comando para modificar un paso
	/// </summary>
	public BaseCommand UpdateCommand { get; }

	/// <summary>
	///		Comando para borrar un paso
	/// </summary>
	public BaseCommand DeleteCommand { get; }
}