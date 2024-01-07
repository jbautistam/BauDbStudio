using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project;

/// <summary>
///		ViewModel de una lista de parámetros (cabeceras / variables)
/// </summary>
public class RestParametersListViewModel : BauMvvm.ViewModels.Forms.ControlItems.ListView.ControlGenericListViewModel<RestParametersListItemViewModel>
{
	public RestParametersListViewModel(RestFileViewModel restFileViewModel, ParametersCollectionModel parameters)
	{
		// Asigna las propiedades
		RestFileViewModel = restFileViewModel;
		Parameters = parameters;
		// Inicializa la lista
		InitList();
	}

	/// <summary>
	///		Inicializa la lista
	/// </summary>
	private void InitList()
	{
		// Limpia la lista
		Items.Clear();
		// Añade los elementos
		foreach (ParameterModel parameter in Parameters)
			Items.Add(new RestParametersListItemViewModel(this, parameter));
	}

	/// <summary>
	///		Modifica un elemento
	/// </summary>
	protected override void UpdateItem(RestParametersListItemViewModel? item)
	{
		ParameterViewModel parameterViewModel = new(this, item?.Parameter);

			// Abre el cuadro de diálogo y recoge los valores
			if (RestFileViewModel.MainViewModel.ViewsController.OpenDialog(parameterViewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Añade / modifica el elemento
				if (item is null)
					Items.Add(new RestParametersListItemViewModel(this, new ParameterModel(parameterViewModel.Key, parameterViewModel.Value)));
				else
				{
					item.Key = parameterViewModel.Key;
					item.Value = parameterViewModel.Value;
				}
				// Indica que se ha modificado el proyecto
				RestFileViewModel.IsUpdated = true;
			}
	}

	/// <summary>
	///		Borra un elemento
	/// </summary>
	protected override void DeleteItem(RestParametersListItemViewModel item)
	{
		if (RestFileViewModel.MainViewModel.ViewsController.MainWindowController
				.SystemController.ShowQuestionCancel($"Do you want to delete the item '{item.Key}'?") == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
		{
			// Elimina el elemento
			Items.Remove(item);
			// Indica que se ha modificado el proyecto
			RestFileViewModel.IsUpdated = true;
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public RestFileViewModel RestFileViewModel { get; } 
	
	/// <summary>
	///		Parámetros
	/// </summary>
	public ParametersCollectionModel Parameters { get; }
}
