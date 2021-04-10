using System;

namespace Bau.Libraries.PluginsStudio.Views.Base.Interfaces
{
	/// <summary>
	///		Interface para los plugin
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		///		Carga los datos de un plugin
		/// </summary>
		void Load(string path);
	}
}
