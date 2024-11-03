using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.ViewModel.Project.Connections;

/// <summary>
///		ViewModel para una conexión de un proyecto
/// </summary>
public class ConnectionAuthenticationViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private ComboViewModel? _comboTypes;

	public ConnectionAuthenticationViewModel(ConnectionViewModel connectionViewModel)
	{
		// Asigna las propiedades
		ConnectionViewModel = connectionViewModel;
		// Inicializa los datos
		InitViewModel();
	}

	/// <summary>
	///		Inicializa los datos
	/// </summary>
	private void InitViewModel()
	{
		// Asigna las propiedades
		// Asigna el manejador de eventos
		PropertyChanged += (sender, args) => ConnectionViewModel.IsUpdated = true;
	}

	/// <summary>
	///		Carga el combo de métodos
	/// </summary>
	private ComboViewModel LoadComboMethods()
	{
		ComboViewModel cboTypes = new(this);

			// Añade los elementos
			cboTypes.AddItem((int) AuthenticationModel.AuthenticationType.None, "None");
			cboTypes.AddItem((int) AuthenticationModel.AuthenticationType.Basic, "Basic");
			cboTypes.AddItem((int) AuthenticationModel.AuthenticationType.Jwt, "Jwt");
			cboTypes.AddItem((int) AuthenticationModel.AuthenticationType.ApiKey, "Api key");
			cboTypes.AddItem((int) AuthenticationModel.AuthenticationType.Bearer, "Bearer");
			cboTypes.AddItem((int) AuthenticationModel.AuthenticationType.OAuth, "OAuth");
			// Selecciona el primer elemento
			cboTypes.SelectedItem = cboTypes.Items[0];
			// Devuelve el combo
			return cboTypes;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///		Combo con los métodos
	/// </summary>
	public ComboViewModel? ComboType
	{
		get { return _comboTypes; }
		set { CheckObject(ref _comboTypes, value); }
	}

	/// <summary>
	///		ViewModel de la conexión
	/// </summary>
	public ConnectionViewModel ConnectionViewModel { get; }
}
