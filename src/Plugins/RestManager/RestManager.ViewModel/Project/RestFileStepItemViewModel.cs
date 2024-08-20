using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project;

/// <summary>
///		ViewModel para un paso de un proyecto
/// </summary>
public class RestFileStepItemViewModel : BaseObservableObject
{
	// Variables privadas
	private string _url = default!, _content = default!;
	private ComboViewModel? _comboMethods;
	private RestParametersListViewModel? _headers;

	public RestFileStepItemViewModel(RestFileListStepsViewModel listStepsViewModel, RestStepModel restStep)
	{
		// Asigna las propiedades
		ListStepsViewModel = listStepsViewModel;
		RestStep = restStep;
		// Inicializa los datos
		InitViewModel();
	}

	/// <summary>
	///		Inicializa los datos
	/// </summary>
	private void InitViewModel()
	{
		// Carga el combo de tipos
		ComboMethods = LoadComboTypes();
		// Asigna las propiedades
		ComboMethods.SelectedId = (int) RestStep.Method;
		Url = RestStep.Url;
		Headers = new RestParametersListViewModel(ListStepsViewModel.FileViewModel, RestStep.Headers);
		Content = RestStep.Content ?? string.Empty;
		// Asigna el manejador de eventos
		PropertyChanged += (sender, args) => ListStepsViewModel.FileViewModel.IsUpdated = true;
		ComboMethods.PropertyChanged += (sender, args) => ListStepsViewModel.FileViewModel.IsUpdated = true;
	}

	/// <summary>
	///		Carga el combo de métodos
	/// </summary>
	private ComboViewModel LoadComboTypes()
	{
		ComboViewModel cboMethods = new(this);

			// Añade los elementos
			cboMethods.AddItem((int) RestStepModel.RestMethod.Get, "Get");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Post, "Post");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Patch, "Patch");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Delete, "Delete");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Put, "Put");
			cboMethods.AddItem((int) RestStepModel.RestMethod.Options, "Options");
			// Selecciona el primer elemento
			cboMethods.SelectedItem = cboMethods.Items[0];
			// Devuelve el combo
			return cboMethods;
	}

	/// <summary>
	///		Obtiene los datos del paso
	/// </summary>
	internal BaseStepModel GetStep()
	{
		RestStepModel step = new();

			// Asigna los datos
			if (ComboMethods is not null)
				step.Method = (RestStepModel.RestMethod) (ComboMethods.SelectedId ?? 0);
			step.Url = Url;
			step.Content = Content;
			// Añade las cabeceras
			step.Headers.Clear();
			if (Headers is not null)
				foreach (RestParametersListItemViewModel header in Headers.Items)
					step.Headers.Add(new ParameterModel(header.Key, header.Value));
			// Devuelve el paso
			return step;
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public RestFileListStepsViewModel ListStepsViewModel { get; }

	/// <summary>
	///		Datos del paso
	/// </summary>
	public RestStepModel RestStep { get; }

	/// <summary>
	///		Combo con los métodos
	/// </summary>
	public ComboViewModel? ComboMethods
	{
		get { return _comboMethods; }
		set { CheckObject(ref _comboMethods, value); }
	}

	/// <summary>
	///		Url
	/// </summary>
	public string Url 
	{ 
		get { return _url; }
		set { CheckProperty(ref _url, value); }
	}

	/// <summary>
	///		Contenido
	/// </summary>
	public string Content
	{
		get { return _content; }
		set { CheckProperty(ref _content, value); }
	}

	/// <summary>
	///		Lista de cabeceras
	/// </summary>
	public RestParametersListViewModel? Headers
	{
		get { return _headers; }
		set { CheckObject(ref _headers, value); }
	}
}
