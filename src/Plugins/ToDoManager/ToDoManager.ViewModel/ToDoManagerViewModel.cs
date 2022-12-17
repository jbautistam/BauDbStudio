using System;

namespace Bau.Libraries.ToDoManager.ViewModel;

/// <summary>
///		ViewModel principal del manager de listas de tareas
/// </summary>
public class ToDoManagerViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Constantes públicas
	public const string ToDoFileExtension = ".bau.todo";

	public ToDoManagerViewModel(Controllers.IToDoManagerController mainController)
	{
		ViewsController = mainController;
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		// no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Carga el directorio
	/// </summary>
	public void Load(string path)
	{
		// no hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Abre un archivo de entradas
	/// </summary>
	public bool OpenFile(string fileName)
	{
		if (!string.IsNullOrWhiteSpace(fileName) && 
			(fileName.EndsWith(ToDoFileExtension, StringComparison.CurrentCultureIgnoreCase)))
		{
			Reader.ToDoFileViewModel viewModel = new(this, fileName);

				// Carga el archivo con la contraseña pasada como parámetro y si puede hacerlo, abre el documento
				if (viewModel.LoadFile())
					ViewsController.OpenWindow(viewModel);
				// e indica que ha podido abrir el archivo (para que no se abra ningún documento)
				return true;
		}
		else
			return false;
	}

	/// <summary>
	///		Controlador de vistas de aplicación
	/// </summary>
	public Controllers.IToDoManagerController ViewsController { get; }
}