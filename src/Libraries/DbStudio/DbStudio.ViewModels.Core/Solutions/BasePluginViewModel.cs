using System;

namespace Bau.Libraries.DbStudio.ViewModels.Core.Solutions
{
	/// <summary>
	///		ViewModel base para las soluciones
	/// </summary>
	public abstract class BasePluginViewModel
	{
		public BasePluginViewModel(MainViewModel mainViewModel, string pluginName)
		{
			MainViewModel = mainViewModel;
			PluginName = pluginName;
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public MainViewModel MainViewModel { get; }

		/// <summary>
		///		Nombre del plugin
		/// </summary>
		public string PluginName { get; }
	}
}
