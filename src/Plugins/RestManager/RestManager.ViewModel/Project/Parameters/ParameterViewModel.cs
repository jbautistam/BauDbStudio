using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project.Parameters;

/// <summary>
///		ViewModel para la ventana de definición del parámetro
/// </summary>
public class ParameterViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas 
	private string _key = default!;
	private string? _value;

	public ParameterViewModel(RestParametersListViewModel restParametersListViewModel, ParameterModel? parameter)
	{
		// Inicializa los objetos
		RestParametersListViewModel = restParametersListViewModel;
		Parameter = parameter;
		// Inicializa las propiedades
		InitProperties();
	}

	/// <summary>
	///		Inicializa las propiedades
	/// </summary>
	private void InitProperties()
	{
		if (Parameter is not null)
		{
			Key = Parameter.Key;
			Value = Parameter.Value;
		}
	}

	/// <summary>
	///		Comprueba los datos introducidos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Key))
				RestParametersListViewModel.RestFileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Enter the key");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{
			// Asigna los datos al objeto
			Parameter = new ParameterModel(Key, Value);
			// Indica que está grabado
			IsUpdated = false;
			// Cierra la ventana
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public RestParametersListViewModel RestParametersListViewModel { get; }
	
	/// <summary>
	///		Datos del parámetro
	/// </summary>
	public ParameterModel? Parameter { get; private set; }

	/// <summary>
	///		Clave del parámetro
	/// </summary>
	public string Key
	{
		get { return _key; }
		set { CheckProperty(ref _key, value); }
	}

	/// <summary>
	///		Valor del parámetro
	/// </summary>
	public string? Value
	{
		get { return _value; }
		set { CheckProperty(ref _value, value); }
	}
}
