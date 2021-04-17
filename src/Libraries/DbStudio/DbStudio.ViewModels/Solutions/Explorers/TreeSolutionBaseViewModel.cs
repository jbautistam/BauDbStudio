using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers
{
	/// <summary>
	///		ViewModel base para los árboles
	/// </summary>
	public abstract class TreeSolutionBaseViewModel : BaseTreeViewModel
	{
		public TreeSolutionBaseViewModel(SolutionViewModel solutionViewModel)
		{
			SolutionViewModel = solutionViewModel;
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }
	}
}