using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project.Parameters;

/// <summary>
///		ViewModel de una lista de parámetros (cabeceras / variables)
/// </summary>
public class RestParametersListViewModel : BauMvvm.ViewModels.Forms.ControlItems.ListView.ControlGenericListViewModel<RestParametersListItemViewModel>
{
	public RestParametersListViewModel(RestFileViewModel restFileViewModel)
	{
		RestFileViewModel = restFileViewModel;
	}

	/// <summary>
	///		Inicializa la lista
	/// </summary>
	public void AddParameters(ParametersCollectionModel parameters)
	{
		// Limpia la lista
		Items.Clear();
		// Añade los elementos
		parameters.Sort((first, second) => first.Key.CompareTo(second.Key));
		foreach (ParameterModel parameter in parameters)
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
				// Elimina el elemento que se estaba editando
				if (item is not null)
					Items.Remove(item);
				// y lo crea de nuevo
				Items.Add(new RestParametersListItemViewModel(this, new ParameterModel(parameterViewModel.Key, parameterViewModel.Value)));
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
	///		Obtiene los datos de los parámetros
	/// </summary>
	internal ParametersCollectionModel GetParameters()
	{
		ParametersCollectionModel parameters = [];

			// Asigna los datos
			foreach (RestParametersListItemViewModel item in Items)
				parameters.Add(item.Key, item.Value);
			// Devuelve los parámetros
			return parameters;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public RestFileViewModel RestFileViewModel { get; } 
}
