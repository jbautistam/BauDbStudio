using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers
{
	/// <summary>
	///		ViewModel base de los árboles de los exploradores de la solución
	/// </summary>
	public abstract class BaseTreeViewModel : BaseObservableObject
	{
		// Variables privadas
		private ObservableCollection<IHierarchicalViewModel> _children;
		private BaseTreeNodeViewModel _selectedNode;

		protected BaseTreeViewModel()
		{
			// Asigna las propiedades
			Children = new ObservableCollection<IHierarchicalViewModel>();
			ContextUI = SynchronizationContext.Current;
			// Asigna los comandos
			OpenCommand = new BaseCommand(_ => OpenProperties(), _ => CanExecuteAction(nameof(OpenCommand)))
										.AddListener(this, nameof(SelectedNode));
			DeleteCommand = new BaseCommand(_ => DeleteItem(), _ => CanExecuteAction(nameof(DeleteCommand)))
										.AddListener(this, nameof(SelectedNode));
		}

		/// <summary>
		///		Carga los datos del árbol
		/// </summary>
		public void Load()
		{
			object state = new object();

				// Carga los nodos en el árbol
				//? _contexUi mantiene el contexto de sincronización que creó el ViewModel (que debería ser la interface de usuario)
				//? Al generarse las tablas en otro hilo o desde un evento, no se puede borrar ObservableCollection sin una
				//? excepción del tipo "Este tipo de CollectionView no admite cambios en su SourceCollection desde un hilo diferente del hilo Dispatcher"
				//? Por eso se tiene que añadir el mensaje de log desde el contexto de sincronización de la UI
				ContextUI.Send(_ => {
										List<IHierarchicalViewModel> nodesExpanded = GetNodesExpanded(Children);

											// Limpia la colección de hijos
											Children.Clear();
											// Añade los nodos raíz
											AddRootNodes();
											// Expande los nodos previamente abiertos
											ExpandNodes(Children, nodesExpanded);
									},
									state
							  );
		}

		/// <summary>
		///		Añade los nodos raíz
		/// </summary>
		protected abstract void AddRootNodes();

		/// <summary>
		///		Obtiene recursivamente los elementos expandidos del árbol (para poder recuperarlos posteriormente y dejarlos abiertos de nuevo)
		/// </summary>
		private List<IHierarchicalViewModel> GetNodesExpanded(ObservableCollection<IHierarchicalViewModel> nodes)
		{
			List<IHierarchicalViewModel> expanded = new List<IHierarchicalViewModel>();

				// Recorre los nodos obteniendo los seleccionados
				foreach (IHierarchicalViewModel node in nodes)
				{ 
					// Añade el nodo si se ha expandido
					if (node.IsExpanded)
						expanded.Add(node);
					// Añade los nodos hijo
					if (node.Children != null && node.Children.Count > 0)
						expanded.AddRange(GetNodesExpanded(node.Children));
				}
				// Devuelve la colección de nodos
				return expanded;
		}

		/// <summary>
		///		Expande todos los nodos
		/// </summary>
		protected void ExpandAll()
		{
			ExpandAll(Children);
		}

		/// <summary>
		///		Expande todos los nodos
		/// </summary>
		private void ExpandAll(ObservableCollection<IHierarchicalViewModel> nodes)
		{
			foreach (IHierarchicalViewModel node in nodes)
			{
				// Expande el nodo
				node.IsExpanded = true;
				// Expande los hijos
				ExpandAll(node.Children);
			}
		}

		/// <summary>
		///		Expande los nodos que se le pasan en la colección <param name="nodesExpanded" />
		/// </summary>
		private void ExpandNodes(ObservableCollection<IHierarchicalViewModel> nodes, List<IHierarchicalViewModel> nodesExpanded)
		{ 
			if (nodes != null)
				foreach (IHierarchicalViewModel node in nodes)
					if (CheckIsExpanded(node, nodesExpanded))
					{ 
						// Expande el nodo
						node.IsExpanded = true;
						// Expande los hijos
						ExpandNodes(node.Children, nodesExpanded);
					}
		}

		/// <summary>
		///		Comprueba si un nodo estaba en la lista de nodos abiertos
		/// </summary>
		private bool CheckIsExpanded(IHierarchicalViewModel node, List<IHierarchicalViewModel> nodesExpanded)
		{ 
			// Recorre la colección
			foreach (IHierarchicalViewModel nodeExpanded in nodesExpanded)
				if ((nodeExpanded as BaseTreeNodeViewModel)?.Text == (node as BaseTreeNodeViewModel)?.Text &&
						(nodeExpanded as BaseTreeNodeViewModel)?.Icon == (node as BaseTreeNodeViewModel)?.Icon)
					return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected abstract bool CanExecuteAction(string action);

		/// <summary>
		///		Obtiene el tipo de nodo seleccionado
		/// </summary>
		protected string GetSelectedNodeType()
		{
			return SelectedNode?.Type;
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected abstract void OpenProperties();

		/// <summary>
		///		Borra el elemento seleccionado
		/// </summary>
		protected abstract void DeleteItem();

		/// <summary>
		///		Nodos
		/// </summary>
		public ObservableCollection<IHierarchicalViewModel> Children 
		{ 
			get { return _children; }
			set { CheckObject(ref _children, value); }
		}

		/// <summary>
		///		Nodo seleccionado
		/// </summary>
		public BaseTreeNodeViewModel SelectedNode
		{	
			get { return _selectedNode; }
			set { CheckObject(ref _selectedNode, value); }
		}

		/// <summary>
		///		Comando para abrir la ventana de propiedades
		/// </summary>
		public BaseCommand OpenCommand { get; }

		/// <summary>
		///		Comando para borrar un nodo
		/// </summary>
		public BaseCommand DeleteCommand { get; }

		/// <summary>
		///		Contexto de sincronización
		/// </summary>
		internal SynchronizationContext ContextUI { get; }
	}
}