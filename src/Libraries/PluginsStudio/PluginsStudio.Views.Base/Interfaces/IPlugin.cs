using System;

namespace Bau.Libraries.PluginsStudio.Views.Base.Interfaces
{
	/// <summary>
	///		Interface para los plugin
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		///		Inicializa el plugin
		/// </summary>
		void Initialize(ViewModels.Base.Controllers.IPluginsController pluginsController);

		/// <summary>
		///		Carga los datos de un plugin
		/// </summary>
		void Load(string path);
	}
}
