using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers
{
	/// <summary>
	///		ViewModel base para los árboles
	/// </summary>
	public abstract class TreeSolutionBaseViewModel : BaseTreeViewModel
	{
		public TreeSolutionBaseViewModel(DbStudioViewModel solutionViewModel)
		{
			SolutionViewModel = solutionViewModel;
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public DbStudioViewModel SolutionViewModel { get; }
	}
}