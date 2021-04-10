using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bau.Libraries.PluginsStudio.Views
{
	/// <summary>
	///		Manager de las vistas de PluginStudio
	/// </summary>
	public class PluginsStudioViewManager
	{
		public PluginsStudioViewManager(DbStudio.ViewModels.Core.Controllers.IDbStudioCoreController mainStudioController)
		{
			// Inicializa el ViewModel
			PluginsStudioViewModel = new DbStudio.ViewModels.Core.PluginsStudioViewModel(mainStudioController);
		}

		/// <summary>
		///		Inicializa los plugins
		/// </summary>
		public void InitializePlugins()
		{
		}

		/// <summary>
		///		Carga los datos
		/// </summary>
		public void Load(string path, string workspace)
		{
			PluginsStudioViewModel.Load(path, workspace);
		}

		/// <summary>
		///		Seleccina un espacio de trabajo
		/// </summary>
		public void SelectWorkspace(string workspace)
		{
			PluginsStudioViewModel.SelectWorkspace(workspace);
		}

		/// <summary>
		///		Manager de plugins
		/// </summary>
		internal Plugins.PluginsManager PluginsManager { get; } = new();

		/// <summary>
		///		ViewModel
		/// </summary>
		public DbStudio.ViewModels.Core.PluginsStudioViewModel PluginsStudioViewModel { get; }
	}
}
