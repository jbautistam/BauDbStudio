using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.DbStudio.Models;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.Application.Repository;

/// <summary>
///		Repository de <see cref="SolutionModel"/>
/// </summary>
internal class SolutionRepository
{
	// Constantes privadas
	private const string TagRoot = "ScriptSolution";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagFileParameters = "FileParameters";
	private const string TagEtlFileParameters = "EtlFileParameters";
	private const string TagId = "Id";
	private const string TagConnection = "Connection";
	private const string TagtimeoutExecuteScript = "TimeoutExecuteScript";
	private const string TagType = "Type";
	private const string TagParameter = "Parameter";
	private const string TagValue = "Value";
	private const string TagConnectionSelected = "ConnectionSelected";
	private const string TagQueueJsonParameters = "QueueJsonParameter";

	/// <summary>
	///		Carga los datos de una solución
	/// </summary>
	internal SolutionModel Load(string fileName)
	{
		SolutionModel solution = new();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los datos de la solución
			if (fileML != null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
					{
						// Asigna las propiedades
						solution.FileName = fileName;
						solution.GlobalId = rootML.Attributes[TagId].Value;
						solution.Name = rootML.Nodes[TagName].Value.TrimIgnoreNull();
						solution.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
						solution.LastConnectionParametersFileName = rootML.Nodes[TagFileParameters].Value.TrimIgnoreNull();
						solution.LastConnectionSelectedGlobalId = rootML.Nodes[TagConnectionSelected].Value.TrimIgnoreNull();
						solution.LastEtlParametersFileName = rootML.Nodes[TagEtlFileParameters].Value.TrimIgnoreNull();
						// Carga los objetos
						LoadConnections(solution, rootML);
						LoadQueueConnectionParameters(solution, rootML);
					}
			// Devuelve la solución
			return solution;
	}

	/// <summary>
	///		Carga los datos de conexión
	/// </summary>
	private void LoadConnections(SolutionModel solution, MLNode rootML)
	{
		foreach (MLNode nodeML in rootML.Nodes)
			if (nodeML.Name == TagConnection)
			{
				ConnectionModel connection = new(solution);

					// Asigna las propiedades
					connection.GlobalId = nodeML.Attributes[TagId].Value;
					connection.Name = nodeML.Nodes[TagName].Value.TrimIgnoreNull();
					connection.Description = nodeML.Nodes[TagDescription].Value.TrimIgnoreNull();
					connection.Type = nodeML.Attributes[TagType].Value.GetEnum(ConnectionModel.ConnectionType.Spark);
					connection.TimeoutExecuteScript = TimeSpan.FromMinutes(nodeML.Attributes[TagtimeoutExecuteScript].Value.GetInt(40));
					// Carga los parámetros
					foreach (MLNode childML in nodeML.Nodes)
						if (childML.Name == TagParameter)
							connection.Parameters.Add(childML.Attributes[TagName].Value.TrimIgnoreNull(), childML.Attributes[TagValue].Value.TrimIgnoreNull());
					// Añade los datos a la solución
					solution.Connections.Add(connection);
			}
	}

	/// <summary>
	///		Carga la cola de parámetros de conexión
	/// </summary>
	private void LoadQueueConnectionParameters(SolutionModel solution, MLNode rootML)
	{
		// Limpia la cola de conexiones
		solution.QueueConnectionParameters.Clear();
		// Carga la cola de conexiones
		foreach (MLNode nodeML in rootML.Nodes)
			if (nodeML.Name == TagQueueJsonParameters)
				solution.QueueConnectionParameters.Add(nodeML.Value);
	}

	/// <summary>
	///		Graba los datos de una solución
	/// </summary>
	internal void Save(SolutionModel solution, string fileName)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los datos de la solución
			rootML.Attributes.Add(TagId, solution.GlobalId);
			rootML.Nodes.Add(TagName, solution.Name);
			rootML.Nodes.Add(TagDescription, solution.Description);
			rootML.Nodes.Add(TagFileParameters, solution.LastConnectionParametersFileName);
			rootML.Nodes.Add(TagConnectionSelected, solution.LastConnectionSelectedGlobalId);
			rootML.Nodes.Add(TagEtlFileParameters, solution.LastEtlParametersFileName);
			// Añade los objetos
			rootML.Nodes.AddRange(GetConnectionsNodes(solution));
			rootML.Nodes.AddRange(GetQueueConnectionsNodes(solution));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene los nodos para los datos de conexión de una solución
	/// </summary>
	private MLNodesCollection GetConnectionsNodes(SolutionModel solution)
	{
		MLNodesCollection nodesML = [];

			// Añade los datos
			foreach (ConnectionModel connection in solution.Connections)
			{
				MLNode nodeML = nodesML.Add(TagConnection);

					// Añade los datos
					nodeML.Attributes.Add(TagId, connection.GlobalId);
					nodeML.Nodes.Add(TagName, connection.Name);
					nodeML.Nodes.Add(TagDescription, connection.Description);
					nodeML.Attributes.Add(TagType, connection.Type.ToString());
					nodeML.Attributes.Add(TagtimeoutExecuteScript, connection.TimeoutExecuteScript.TotalMinutes);
					// Añade los parámetros
					foreach ((string key, string value) in connection.Parameters.Enumerate())
					{
						MLNode parameterML = nodeML.Nodes.Add(TagParameter);

							// Añade los atributos
							parameterML.Attributes.Add(TagName, key);
							parameterML.Attributes.Add(TagValue, value);
					}
			}
			// Devuelve la colección de nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene los nodos de la cola de parámetros de conexión
	/// </summary>
	private MLNodesCollection GetQueueConnectionsNodes(SolutionModel solution)
	{
		MLNodesCollection nodes = [];

			// Añade los nodos con os parámetrosde la cola
			foreach (string connectionParameters in solution.QueueConnectionParameters.Enumerate())
				if (!string.IsNullOrWhiteSpace(connectionParameters))
					nodes.Add(new MLNode(TagQueueJsonParameters, connectionParameters));
			// Devuelve la colección de nodos
			return nodes;
	}
}
