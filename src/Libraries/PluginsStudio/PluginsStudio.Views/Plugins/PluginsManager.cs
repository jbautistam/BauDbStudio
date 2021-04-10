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
		///		Plugins
		/// </summary>
		private List<Base.Interfaces.IPlugin> Plugins { get; } = new();
	}
}
