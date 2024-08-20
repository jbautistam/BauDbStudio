using Bau.Libraries.LibHelper.Extensors;

namespace Bau.Libraries.DbStudio.ViewModels.Configuration;

/// <summary>
///		ViewModel para la configuración
/// </summary>
public class ConfigurationViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Constants privadas
	private const string ApplicationName = "DbStudioApp";
	// Variables privadas
	private bool _seeSystemTables;

	public ConfigurationViewModel(DbStudioViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
	}

	/// <summary>
	///		Carga los datos de configuración
	/// </summary>
	internal void Load()
	{
		SeeSystemTables = MainViewModel.MainController.PluginController.ConfigurationController.GetConfiguration(ApplicationName, nameof(SeeSystemTables)).GetBool();
	}

	/// <summary>
	///		Comprueba si los datos son correctos
	/// </summary>
	public bool ValidateData(out string error)
	{
		// Inicializa los argumentos de salida
		error = string.Empty;
		// Comprueba los datos
		// Devuelve el valor que indica si los datos son correctos
		return error.IsEmpty();
	}

	/// <summary>
	///		Graba los datos (y actualiza el árbol)
	/// </summary>
	public void Save()
	{
		// Grava la configuración
		MainViewModel.MainController.PluginController.ConfigurationController.SetConfiguration(ApplicationName, nameof(SeeSystemTables), SeeSystemTables.ToString());
		// Actualiza el árbol
		MainViewModel.TreeConnectionsViewModel.Load();
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public DbStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Directorio a partir del que se encuentran los datos de los blogs
	/// </summary>
	public bool SeeSystemTables
	{
		get { return _seeSystemTables; }
		set { CheckProperty(ref _seeSystemTables, value); }
	}
}
