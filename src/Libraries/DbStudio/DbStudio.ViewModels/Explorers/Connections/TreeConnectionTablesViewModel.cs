using System;
using System.Collections.Generic;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections
{
	/// <summary>
	///		ViewModel para mostrar el esquema de tablas de una conexión
	/// </summary>
	public class TreeConnectionTablesViewModel : TreeSolutionBaseViewModel
	{
		public TreeConnectionTablesViewModel(SolutionViewModel solutionViewModel) : base(solutionViewModel) {}

		/// <summary>
		///		Carga el árbol de una conexión
		/// </summary>
		public void LoadConnection(ConnectionModel connection)
		{
			// Guarda la conexión
			Connection = connection;
			// Carga los datos
			Load();
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void AddRootNodes()
		{
			Children.Add(new NodeConnectionViewModel(this, null, Connection));
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			return false;
		}

		/// <summary>
		///		Abre la ventana de propiedades de un nodo
		/// </summary>
		protected override void OpenProperties()
		{
		}

		/// <summary>
		///		Obtiene las tablas seleccionadas en el árbol
		/// </summary>
		internal List<ConnectionTableModel> GetSelectedTables()
		{
			List<ConnectionTableModel> tables = new List<ConnectionTableModel>();

				// Obtiene las conexiones
				foreach (ControlHierarchicalViewModel node in Children)
					if (node is NodeConnectionViewModel nodeConnection)
						foreach (ControlHierarchicalViewModel childRoot in nodeConnection.Children)
							if (childRoot is NodeRootViewModel nodeSchema)
							{
								foreach (ControlHierarchicalViewModel child in nodeSchema.Children)
									if (child.IsChecked && child is NodeTableViewModel nodeTable && nodeTable.Table != null)
										tables.Add(nodeTable.Table);
							}
							else if (childRoot is NodeTableViewModel nodeTableNoSchema)
								if (nodeTableNoSchema.IsChecked && nodeTableNoSchema.Table != null)
									tables.Add(nodeTableNoSchema.Table);
				// Devuelve las conexiones y tablas seleccionadas
				return tables;
		}

		/// <summary>
		///		Borra el elemento seleccionado
		/// </summary>
		protected override void DeleteItem()
		{
			// ... no hace nada, sólo implementa la interface
		}

		/// <summary>
		///		Conexión
		/// </summary>
		internal ConnectionModel Connection { get; private set; }
	}
}