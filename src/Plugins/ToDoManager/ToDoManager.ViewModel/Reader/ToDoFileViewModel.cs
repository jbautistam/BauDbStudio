using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ToDoManager.Application.Models;

namespace Bau.Libraries.ToDoManager.ViewModel.Reader;

/// <summary>
///		ViewModel para ver el contenido de un archivo de tareas
/// </summary>
public class ToDoFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _fileName;
	private bool _isSelectedGroup;
	private GroupViewModel _selectedGroup;

	public ToDoFileViewModel(ToDoManagerViewModel mainViewModel, string fileName) : base(false)
	{ 
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
		// Asigna los controles
		Tree = new Explorer.TreeTasksViewModel(this);
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	internal bool LoadFile()
	{
		bool loaded = false;

			// Intenta cargar el archivo
			try
			{
				// Carga el archivo
				ToDoManager.Load(FileName);
				// Carga el árbol
				Tree.Load();
				// Indica que se ha cargado el archivo
				loaded = true;
			}
			catch (Exception exception)
			{
				MainViewModel.ViewsController.Logger.Default.LogItems.Add(new(null, LibLogger.Models.Log.LogModel.LogType.Error,
																			  $"Error when load {FileName}. {exception.Message}"));
				MainViewModel.ViewsController.SystemController.ShowMessage($"Error when load {FileName}. {exception.Message}");
			}
			// Devuelve el valor que indica si ha podido cargar el archivo
			return loaded;
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			return "¿Desea grabar el archivo antes de continuar?";
		else
			return $"¿Desea grabar el archivo '{System.IO.Path.GetFileName(FileName)}' antes de continuar?";
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// Graba el archivo
		if (string.IsNullOrWhiteSpace(FileName) || newName)
		{
			string newFileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave
									(string.Empty, 
									 $"Archivo tareas (*{ToDoManagerViewModel.ToDoFileExtension})|*{ToDoManagerViewModel.ToDoFileExtension}|Todos los archivos (*.*)|*.*",
									 FileName, ToDoManagerViewModel.ToDoFileExtension);

				// Cambia el nombre de archivo si es necesario
				if (!string.IsNullOrWhiteSpace(newFileName))
					FileName = newFileName;
		}
		// Graba el archivo
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			// Actualiza la entrada con los datos de la pantalla
			if (IsSelectedGroup)
				SelectedGroup.UpdateGroup();
			// Graba el archivo
			ToDoManager.Save(FileName);
			// Actualiza el árbol
			MainViewModel.ViewsController.HostPluginsController.RefreshFiles();
			// Añade el archivo a los últimos archivos abiertos
			MainViewModel.ViewsController.HostPluginsController.AddFileUsed(FileName);
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
	}

	/// <summary>
	///		Cierra la ventana de detalles
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Comprueba si se puede modificar el grupo seleccionado
	/// </summary>
	internal bool CanUpdateSelectedGroup()
	{
		return SelectedGroup is null || SelectedGroup.UpdateGroup();
	}

	/// <summary>
	///		Descarga el grupo seleccionado (por ejemplo, si se han borrado los datos)
	/// </summary>
	internal void DiscardSelectedGroup()
	{
		SelectedGroup = null;
	}

	/// <summary>
	///		Actualiza el grupo seleccionado para que se visualice
	/// </summary>
	internal void UpdateSelectedGroup(GroupModel group)
	{
		SelectedGroup = new GroupViewModel(this, group);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Manager de la aplicación de lista de tareas
	/// </summary>
	public Application.ToDoManager ToDoManager { get; } = new();

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Id de la ficha en pantalla
	/// </summary>
	public string TabId 
	{ 
		get { return GetType().ToString() + "_" + FileName; }
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
			{
				if (string.IsNullOrWhiteSpace(_fileName))
					Header = "New filename";
				else
					Header = System.IO.Path.GetFileName(_fileName);
			}
		}
	}

	/// <summary>
	///		Arbol del explorador
	/// </summary>
	public Explorer.TreeTasksViewModel Tree { get; }

	/// <summary>
	///		Grupo seleccionado
	/// </summary>
	public GroupViewModel SelectedGroup 
	{ 
		get { return _selectedGroup; }
		set
		{
			if (CheckObject(ref _selectedGroup, value))
				IsSelectedGroup = value is not null;
		}
	}

	/// <summary>
	///		Indica si hay algún grupo seleccionado
	/// </summary>
	public bool IsSelectedGroup
	{
		get { return _isSelectedGroup; }
		set { CheckProperty(ref _isSelectedGroup, value); }
	}
}
