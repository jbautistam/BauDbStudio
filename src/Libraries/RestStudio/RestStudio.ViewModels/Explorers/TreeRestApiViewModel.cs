using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.RestStudio.Models.Rest;

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
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Api rest</summary>
			RestApi,
			/// <summary>Raíz de los nodos de contexto</summary>
			ContextsRoot,
			/// <summary>Contexto</summary>
			Context,
			/// <summary>Raíz de los nodos de métodos</summary>
			MethodsRoot,
			/// <summary>Nodo de método</summary>
			Method
		}
		/// <summary>Tipo de icono</summary>
		public enum IconType
		{
			Unknown,
			RestApi,
			ContextsRoot,
			Context,
			MethodsRoot,
			Method
		}

		public TreeRestApiViewModel(RestStudioViewModel mainViewModel)
		{
			// Asigna las propiedades
			MainViewModel = mainViewModel;
			// Asigna los comandos
			NewRestApiCommand = new BaseCommand(_ => UpdateRestApi(null))
										.AddListener(this, nameof(SelectedNode));
			NewContextCommand = new BaseCommand(_ => UpdateContext(SelectedNode as NodeRestViewModel), _ => CanExecuteAction(nameof(NewContextCommand)))
										.AddListener(this, nameof(SelectedNode));
			NewMethodCommand = new BaseCommand(_ => UpdateMethod(SelectedNode as NodeRestViewModel), _ => CanExecuteAction(nameof(NewMethodCommand)))
										.AddListener(this, nameof(SelectedNode));
		}

		/// <summary>
		///		Añade los nodos raíz
		/// </summary>
		protected override void AddRootNodes()
		{
			foreach (RestApiModel rest in MainViewModel.Solution.RestApis)
				Children.Add(new NodeRestViewModel(this, null, rest.Name, NodeType.RestApi, rest, true, BauMvvm.ViewModels.Media.MvvmColor.Navy));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			NodeType type = GetNodeType();

				switch (action)
				{
					case nameof(OpenCommand):
						return type == NodeType.RestApi || type == NodeType.Context || type == NodeType.Method;
					case nameof(NewContextCommand):
					case nameof(NewMethodCommand):
						return type == NodeType.ContextsRoot || type == NodeType.MethodsRoot || type ==  NodeType.RestApi;
					default:
						return false;
				}
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
			switch (GetNodeType())
			{
				case NodeType.RestApi:
						UpdateRestApi(SelectedNode as NodeRestViewModel);
					break;
				case NodeType.Context:
						UpdateContext(SelectedNode as NodeRestViewModel);
					break;
				case NodeType.Method:
						UpdateMethod(SelectedNode as NodeRestViewModel);
					break;
			}
		}

		/// <summary>
		///		Modifica un nodo de API Rest
		/// </summary>
		private void UpdateRestApi(NodeRestViewModel node)
		{
			Solution.RestApiViewModel viewModel = null;
			bool isNew = true;

				// Obtiene el modelo
				if (node != null && node.Tag is RestApiModel rest)
				{
					viewModel = new Solution.RestApiViewModel(MainViewModel, rest);
					isNew = false;
				}
				else
					viewModel = new Solution.RestApiViewModel(MainViewModel, null);
				// Abre la ventana
				if (MainViewModel.RestStudioController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
				{
					// Si es nuevo, se añade a la colección
					if (isNew)
						MainViewModel.Solution.RestApis.Add(viewModel.Rest);
					// Graba y actualiza
					Save();
				}
		}

		/// <summary>
		///		Modifica un nodo de contexto
		/// </summary>
		private void UpdateContext(NodeRestViewModel node)
		{
			RestApiModel restApi = node?.GetRestParent();

				if (restApi == null)
					MainViewModel.RestStudioController.HostController.SystemController.ShowMessage("Seleccione un nodo de API para añadirle un contexto");
				else
				{
					Solution.ContextViewModel viewModel = null;
					bool isNew = true;

						// Obtiene el modelo
						if (node != null && node.Tag is ContextModel context)
						{
							viewModel = new Solution.ContextViewModel(MainViewModel, context);
							isNew = false;
						}
						else
							viewModel = new Solution.ContextViewModel(MainViewModel, null);
						// Abre la ventana
						if (MainViewModel.RestStudioController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
						{
							// Si es nuevo, se añade a la colección
							if (isNew)
								restApi.Contexts.Add(viewModel.Context);
							// Graba y actualiza
							Save();
						}
				}
		}

		/// <summary>
		///		Modifica un nodo de método
		/// </summary>
		private void UpdateMethod(NodeRestViewModel node)
		{
			RestApiModel restApi = node?.GetRestParent();

				if (restApi == null)
					MainViewModel.RestStudioController.HostController.SystemController.ShowMessage("Seleccione un nodo de API para añadirle un método");
				else
				{
					Solution.MethodViewModel viewModel = null;
					bool isNew = true;

						// Obtiene el modelo
						if (node != null && node.Tag is MethodModel method)
						{
							viewModel = new Solution.MethodViewModel(MainViewModel, method);
							isNew = false;
						}
						else
							viewModel = new Solution.MethodViewModel(MainViewModel, null);
						// Abre la ventana
						if (MainViewModel.RestStudioController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
						{
							// Si es nuevo, se añade a la colección
							if (isNew)
								restApi.Methods.Add(viewModel.Method);
							// Graba y actualiza
							Save();
						}
				}
		}

		/// <summary>
		///		Borra un elemento
		/// </summary>
		protected override void DeleteItem()
		{
			// throw new NotImplementedException();
		}

		/// <summary>
		///		Graba la solución y actualiza el árbol
		/// </summary>
		private void Save()
		{
			// Graba la solución
			MainViewModel.Save();
			// Actualiza el árbol
			Load();
		}

		/// <summary>
		///		Obtiene el nodo de tipo seleccionado
		/// </summary>
		private NodeType GetNodeType()
		{
			if (SelectedNode != null && SelectedNode is NodeRestViewModel nodeViewModel)
				return nodeViewModel.NodeType;
			else
				return NodeType.Unknown;
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public RestStudioViewModel MainViewModel { get; }

		/// <summary>
		///		Comando de nueva API
		/// </summary>
		public BaseCommand NewRestApiCommand { get; }

		/// <summary>
		///		Comando de nuevo contexto
		/// </summary>
		public BaseCommand NewContextCommand { get; }

		/// <summary>
		///		Comando de nuevo método
		/// </summary>
		public BaseCommand NewMethodCommand { get; }
	}
}
