using System;

namespace Bau.Libraries.DbStudio.Views
{
	/// <summary>
	///		Manager de vistas de DbStudio
	/// </summary>
	public class DbStudioViewManager : PluginsStudio.Views.Base.Interfaces.IPlugin
	{
		/// <summary>
		///		Inicializa el manager de vistas de DbStudio
		/// </summary>
		public void Initialize(PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			MainViewModel = new ViewModels.MainViewModel("DbStudio", new Controllers.DbStudioController(pluginController));
		}

		/// <summary>
		///		Carga los datos del directorio
		/// </summary>
		public void Load(string path)
		{
			MainViewModel.Load(path);
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public ViewModels.MainViewModel MainViewModel { get; private set; }
	}
}
