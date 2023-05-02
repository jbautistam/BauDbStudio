using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.CloudStudio.ViewModels.Explorers
{
	/// <summary>
	///		ViewModel base para los árboles
	/// </summary>
	public abstract class TreeSolutionBaseViewModel : PluginTreeViewModel
	{
		public TreeSolutionBaseViewModel(CloudStudioViewModel solutionViewModel)
		{
			SolutionViewModel = solutionViewModel;
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public CloudStudioViewModel SolutionViewModel { get; }
	}
}