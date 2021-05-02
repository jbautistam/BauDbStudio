using System;

namespace Bau.Libraries.RestStudio.ViewModels.Explorers
{
	/// <summary>
	///		ViewModel del árbol de las Api Rest
	/// </summary>
	public class TreeRestApiViewModel : PluginsStudio.ViewModels.Base.Explorers.BaseTreeViewModel
	{
		// Tipos públicos
		/// <summary>Tipo de nodo</summary>
		public enum NodeType
		{
			Unknown
		}
		/// <summary>Tipo de icono</summary>
		public enum IconType
		{
			Unknown
		}

		public TreeRestApiViewModel(RestStudioViewModel mainViewModel)
		{
			// Asigna las propiedades
			MainViewModel = mainViewModel;
			// Asigna los comandos
		}

		/// <summary>
		///		Añade los nodos raíz
		/// </summary>
		protected override void AddRootNodes()
		{
			foreach (Models.Rest.RestModel rest in MainViewModel.Solution.RestApis)
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
		///		ViewModel principal
		/// </summary>
		public RestStudioViewModel MainViewModel { get; }
	}
}
