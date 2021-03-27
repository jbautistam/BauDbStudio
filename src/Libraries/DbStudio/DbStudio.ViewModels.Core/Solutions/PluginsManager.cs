using System;
using System.Collections.Generic;

namespace Bau.Libraries.DbStudio.ViewModels.Core.Solutions
{
	/// <summary>
	///		Manager de plugins
	/// </summary>
	public class PluginsManager
	{
		/// <summary>
		///		Añade un plugin a la lista
		/// </summary>
		public void Add(BasePluginViewModel plugin)
		{
			Plugins.Add(plugin);
		}

		/// <summary>
		///		Obtiene un plugin
		/// </summary>
		public TypeData Get<TypeData>() where TypeData : BasePluginViewModel
		{
			// Recorre los plugins buscando el adecuado
			foreach (BasePluginViewModel plugin in Plugins)
				if (plugin is TypeData pluginFound)
					return pluginFound;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Plugins
		/// </summary>
		private	List<BasePluginViewModel> Plugins { get; }
	}
}
