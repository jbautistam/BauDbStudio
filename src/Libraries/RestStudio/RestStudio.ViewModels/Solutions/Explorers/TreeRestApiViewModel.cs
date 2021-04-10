using System;

namespace Bau.Libraries.RestStudio.ViewModels.Solutions.Explorers
{
	/// <summary>
	///		ViewModel del árbol de las Api Rest
	/// </summary>
	public class TreeRestApiViewModel : DbStudio.ViewModels.Core.Explorers.BaseTreeViewModel
	{
		public TreeRestApiViewModel(SolutionViewModel solutionViewModel)
		{
			// Asigna las propiedades
			SolutionViewModel = solutionViewModel;
			// Asigna los comandos
		}

		/// <summary>
		///		Añade los nodos raíz
		/// </summary>
		protected override void AddRootNodes()
		{
			foreach (Models.Rest.RestModel rest in SolutionViewModel.Solution.RestApis)
				Children.Add(new NodeRestApiViewModel(this, null, rest));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		Borra un elemento
		/// </summary>
		protected override void DeleteItem()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }
	}
}
