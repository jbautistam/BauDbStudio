using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.CloudStudio.Application;
using Bau.Libraries.CloudStudio.Models;

namespace Bau.Libraries.CloudStudio.ViewModels;

/// <summary>
///		ViewModel de la solución
/// </summary>
public class CloudStudioViewModel : BaseObservableObject
{
	// Variables privadas
	private string _text = default!;
	private Explorers.Cloud.TreeStorageViewModel _treeStoragesViewModel = default!;

	public CloudStudioViewModel(string appName, Controllers.ICloudStudioController mainController)
	{
		// Título de la aplicación
		Text = appName;
		// Asigna las propiedades
		MainController = mainController;
		// Asigna el manager de soluciones
		Manager = new SolutionManager(MainController.Logger);
		// Asigna los árboles de exploración
		TreeStoragesViewModel = new Explorers.Cloud.TreeStorageViewModel(this);
	}

	/// <summary>
	///		Carga un archivo de solución
	/// </summary>
	public void Load(string path)
	{
		// Guarda el directorio
		PathData = path;
		// Carga la solución
		Solution = Manager.LoadConfiguration(GetSolutionFileName(path, "CloudStudio"));
		// Carga los exploradores
		TreeStoragesViewModel.Load();
	}

	/// <summary>
	///		Actualiza el árbol
	/// </summary>
	internal void Refresh()
	{
		Load(PathData);
	}

	/// <summary>
	///		Graba la solución
	/// </summary>
	internal void SaveSolution()
	{
		Save(PathData);
	}

	/// <summary>
	///		Graba la solución
	/// </summary>
	internal void Save(string path)
	{
		// Graba la solución
		Manager.SaveSolution(Solution, GetSolutionFileName(path, "CloudStudio"));
	}

	/// <summary>
	///		Obtiene el nombre del archivo de solución
	/// </summary>
	private string GetSolutionFileName(string path, string project) => Path.Combine(path, $"{project}.xml");

	/// <summary>
	///		Abre un archivo (si reconoce la extensión)
	/// </summary>
	public bool OpenFile(string fileName) => false;

	/// <summary>
	///		Solución
	/// </summary>
	public SolutionModel Solution { get; private set; } = default!;

	/// <summary>
	///		Manager de solución
	/// </summary>
	internal SolutionManager Manager { get; }

	/// <summary>
	///		Controlador principal
	/// </summary>
	public Controllers.ICloudStudioController MainController { get; }

	/// <summary>
	///		Directorio de datos
	/// </summary>
	public string PathData { get; private set; } = default!;

	/// <summary>
	///		Título de la ventana
	/// </summary>
	public string Text 
	{
		get { return _text; }
		set { CheckProperty(ref _text, value); }
	}

	/// <summary>
	///		ViewModel del árbol de storage
	/// </summary>
	public Explorers.Cloud.TreeStorageViewModel TreeStoragesViewModel
	{
		get { return _treeStoragesViewModel; }
		set { CheckObject(ref _treeStoragesViewModel, value); }
	}
}