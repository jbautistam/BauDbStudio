using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;

/// <summary>
///		ViewModel para mostrar el esquema de tablas de una conexión
/// </summary>
public class TreeConnectionTablesViewModel : TreeSolutionBaseViewModel
{
	public TreeConnectionTablesViewModel(DbStudioViewModel solutionViewModel) : base(solutionViewModel) {}

	/// <summary>
	///		Carga el árbol de una conexión
	/// </summary>
	public void LoadConnection(ConnectionModel? connection)
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
		if (Connection is not null)
			Children.Add(new NodeConnectionViewModel(this, null, Connection));
		else
			Children.Clear();
	}

	/// <summary>
	///		Comprueba si se puede ejecutar una acción
	/// </summary>
	protected override bool CanExecuteAction(string action) => false;

	/// <summary>
	///		Abre la ventana de propiedades de un nodo
	/// </summary>
	protected override void OpenProperties()
	{
	}

	/// <summary>
	///		Selecciona una tabla
	/// </summary>
	internal void CheckTable(ConnectionTableModel table)
	{
		foreach (ControlHierarchicalViewModel node in Children)
			if (node is NodeConnectionViewModel nodeConnection)
			{
				// Expande el nodo
				nodeConnection.IsExpanded = true;
				// Busca en los hijos
				foreach (ControlHierarchicalViewModel childRoot in nodeConnection.Children)
					if (childRoot is NodeTableViewModel nodeTableNoSchema)
						childRoot.IsChecked = IsTable(nodeTableNoSchema, table);
					else if (childRoot is NodeRootViewModel nodeSchema)
					{
						// Expande el nodo
						nodeSchema.IsExpanded = true;
						// Busca en los hijos
						foreach (ControlHierarchicalViewModel child in nodeSchema.Children)
							if (child is NodeTableViewModel nodeTable)
								childRoot.IsChecked = IsTable(nodeTable, table);
					}
			}

		// Comprueba si el nodo se corresponde con la tabla
		bool IsTable(NodeTableViewModel node, ConnectionTableModel table)
		{
			return node.Tag is ConnectionTableModel connectionTable &&
				   connectionTable.GlobalId.Equals(table.GlobalId, StringComparison.CurrentCultureIgnoreCase);
		}
	}

	/// <summary>
	///		Obtiene las tablas seleccionadas en el árbol
	/// </summary>
	internal List<ConnectionTableModel> GetSelectedTables()
	{
		List<ConnectionTableModel> tables = new();

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
	internal ConnectionModel? Connection { get; private set; } = default!;
}