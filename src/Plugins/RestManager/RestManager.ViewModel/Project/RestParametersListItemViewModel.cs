using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project;

/// <summary>
///		Elemento de <see cref="RestParametersListItemViewModel"/>
/// </summary>
public class RestParametersListItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Variables privadas 
	private string _key = default!;
	private string? _value;

	public RestParametersListItemViewModel(RestParametersListViewModel restParametersListViewModel, ParameterModel parameter) : base(parameter.Key, parameter)
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
		Key = Parameter.Key;
		Value = Parameter.Value;
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public RestParametersListViewModel RestParametersListViewModel { get; }
	
	/// <summary>
	///		Datos del parámetro
	/// </summary>
	public ParameterModel Parameter { get; }

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