using System;

using Bau.Libraries.ToDoManager.Application.Models;
using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.ToDoManager.ViewModel.Reader;

/// <summary>
///		ViewModel para la edición de un grupo
/// </summary>
public class GroupViewModel : BaseObservableObject
{
	// Variables privadas
	private string _name = string.Empty, _description = string.Empty;

	public GroupViewModel(ToDoFileViewModel fileViewModel, GroupModel group)
	{
		// Asigna las propiedades
		FileViewModel = fileViewModel;
		Group = group;
		// Inicializa el control
		InitControl();
		// Asigna el controlador de modificaciones
		PropertyChanged += (sender, args) => {
												if ((args.PropertyName ?? string.Empty).Equals(nameof(IsUpdated), StringComparison.CurrentCultureIgnoreCase))
													FileViewModel.IsUpdated = true;
											 };
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	private void InitControl()
	{
		// Asigna las propiedades
		Name = Group.Name;
		Description = Group.Description;
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Comprueba si los datos introducidos son correctos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(Name))
				FileViewModel.MainViewModel.ViewsController.HostController.SystemController.ShowMessage("Introduzca el nombre del grupo");
			else
				validated = true;
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Actualiza el grupo
	/// </summary>
	internal bool UpdateGroup()
	{
		bool validated = ValidateData();

			// Si los datos son correctos, se modifica el grupo
			if (validated)
			{
				// Cambia los datos
				Group.Name = Name;
				Group.Description = Description;
				// Actualiza el árbol
				FileViewModel.Tree.Refresh();
			}
			// Devuelve el valor que indica si se ha actualizado
			return validated;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoFileViewModel FileViewModel { get; }

	/// <summary>
	///		Datos del grupo
	/// </summary>
	public GroupModel Group { get; }

	/// <summary>
	///		Nombre del grupo
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { CheckProperty(ref _name, value); }
	}

	/// <summary>
	///		Descripción del grupo
	/// </summary>
	public string Description
	{
		get { return _description; }
		set { CheckProperty(ref _description, value); }
	}
}