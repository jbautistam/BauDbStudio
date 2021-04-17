using System;
using System.Collections.Generic;

namespace Bau.Libraries.PluginsStudio.Views.Plugins
{
	/// <summary>
	///		Manager de plugins
	/// </summary>
	internal class PluginsManager
	{
		/// <summary>
		///		Añade un plugin
		/// </summary>
		internal void Add(Base.Interfaces.IPlugin plugin)
		{
			Plugins.Add(plugin);
		}

		/// <summary>
		///		Ejecuta un comando sobre un plugin
		/// </summary>
		internal void ExecutePluginCommand(string plugin, string viewModel, string command)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Comprueba si se puede ejecutar un comando sobre un plugin
		/// </summary>
		internal bool CheckCanExecutePluginCommand(string plugin, string viewModel, string command)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Plugins
		/// </summary>
		private List<Base.Interfaces.IPlugin> Plugins { get; } = new();
	}
}
