using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.DbStudio.Models;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbStudio.Models.Deployments;

namespace Bau.Libraries.DbStudio.Application.Repository
{
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
		private const string TagtimeoutExecuteScript = "timeoutExecuteScript";
		private const string TagFolder = "Folder";
		private const string TagType = "Type";
		private const string TagParameter = "Parameter";
		private const string TagValue = "Value";
		private const string TagDeployment = "Deployment";
		private const string TagSource = "Source";
		private const string TagTarget = "Target";
		private const string TagJsonParameters = "JsonParameters";
		private const string TagWriteComments = "WriteComments";
		private const string TagReplaceArguments = "ReplaceArguments";
		private const string TagLowcaseFileNames = "LowcaseFileNames";
		private const string TagStorage = "Storage";
		private const string TagConnectionSring = "ConnectionString";

		/// <summary>
		///		Carga los datos de una solución
		/// </summary>
		internal SolutionModel Load(string fileName)
		{
			SolutionModel solution = new SolutionModel();
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
							solution.LastEtlParametersFileName = rootML.Nodes[TagEtlFileParameters].Value.TrimIgnoreNull();
							// Carga los objetos
							LoadConnections(solution, rootML);
							LoadDeployments(solution, rootML);
							LoadStorages(solution, rootML);
							LoadFolders(solution, rootML);
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
					ConnectionModel connection = new ConnectionModel(solution);

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
		///		Carga los dispositivos de almacenamiento
		/// </summary>
		private void LoadStorages(SolutionModel solution, MLNode rootML)
		{
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagStorage)
				{
					Models.Cloud.StorageModel storage = new Models.Cloud.StorageModel(solution);

						// Asigna las propiedades
						storage.GlobalId = nodeML.Attributes[TagId].Value;
						storage.Name = nodeML.Nodes[TagName].Value;
						storage.Description = nodeML.Nodes[TagDescription].Value;
						storage.StorageConnectionString = nodeML.Nodes[TagConnectionSring].Value;
						// Añade los datos a la solución
						solution.Storages.Add(storage);
				}
		}

		/// <summary>
		///		Carga los datos de distribución
		/// </summary>
		private void LoadDeployments(SolutionModel solution, MLNode rootML)
		{
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagDeployment)
				{
					DeploymentModel deployment = new DeploymentModel(solution);

						// Carga los datos
						deployment.GlobalId = nodeML.Attributes[TagId].Value;
						deployment.Name = nodeML.Nodes[TagName].Value.TrimIgnoreNull();
						deployment.Description = nodeML.Nodes[TagDeployment].Value.TrimIgnoreNull();
						deployment.Type = nodeML.Attributes[TagType].Value.GetEnum(DeploymentModel.DeploymentType.Databricks);
						deployment.SourcePath = nodeML.Nodes[TagSource].Value.TrimIgnoreNull();
						deployment.TargetPath = nodeML.Nodes[TagTarget].Value.TrimIgnoreNull();
						deployment.JsonParameters = nodeML.Nodes[TagJsonParameters].Value.TrimIgnoreNull();
						deployment.WriteComments = nodeML.Attributes[TagWriteComments].Value.GetBool(true);
						deployment.ReplaceArguments = nodeML.Attributes[TagReplaceArguments].Value.GetBool(true);
						deployment.LowcaseFileNames = nodeML.Attributes[TagLowcaseFileNames].Value.GetBool(true);
						// Añade el objeto a la solución
						solution.Deployments.Add(deployment);
				}
		}

		/// <summary>
		///		Carga las carpetas de la solución
		/// </summary>
		private void LoadFolders(SolutionModel solution, MLNode rootML)
		{
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagFolder)
					if (!string.IsNullOrWhiteSpace(nodeML.Value) && System.IO.Directory.Exists(nodeML.Value))
						solution.Folders.Add(nodeML.Value.TrimIgnoreNull());
		}

		/// <summary>
		///		Graba los datos de una solución
		/// </summary>
		internal void Save(SolutionModel solution, string fileName)
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add(TagRoot);

				// Añade los datos de la solución
				rootML.Attributes.Add(TagId, solution.GlobalId);
				rootML.Nodes.Add(TagName, solution.Name);
				rootML.Nodes.Add(TagDescription, solution.Description);
				rootML.Nodes.Add(TagFileParameters, solution.LastConnectionParametersFileName);
				rootML.Nodes.Add(TagEtlFileParameters, solution.LastEtlParametersFileName);
				// Añade los objetos
				rootML.Nodes.AddRange(GetConnectionsNodes(solution));
				rootML.Nodes.AddRange(GetDeploymentsNodes(solution));
				rootML.Nodes.AddRange(GetFoldersNodes(solution));
				rootML.Nodes.AddRange(GetStoragesNodes(solution));
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
		}

		/// <summary>
		///		Obtiene los nodos para los datos de conexión de una solución
		/// </summary>
		private MLNodesCollection GetConnectionsNodes(SolutionModel solution)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

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
		///		Obtiene los nodos de storage
		/// </summary>
		private MLNodesCollection GetStoragesNodes(SolutionModel solution)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los nodos
				foreach (Models.Cloud.StorageModel storage in solution.Storages)
				{
					MLNode nodeML = nodesML.Add(TagStorage);

						// Añade las propiedades
						nodeML.Attributes.Add(TagId, storage.GlobalId);
						nodeML.Nodes.Add(TagName, storage.Name);
						nodeML.Nodes.Add(TagDescription, storage.Description);
						nodeML.Nodes.Add(TagConnectionSring, storage.StorageConnectionString);
				}
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene los datos para los nodos de distribución de una solución
		/// </summary>
		private MLNodesCollection GetDeploymentsNodes(SolutionModel solution)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los datos
				foreach (DeploymentModel deployment in solution.Deployments)
				{
					MLNode nodeML = nodesML.Add(TagDeployment);

						// Añade los datos
						nodeML.Attributes.Add(TagId, deployment.GlobalId);
						nodeML.Nodes.Add(TagName, deployment.Name);
						nodeML.Nodes.Add(TagDescription, deployment.Description);
						nodeML.Attributes.Add(TagType, deployment.Type.ToString());
						nodeML.Nodes.Add(TagSource, deployment.SourcePath);
						nodeML.Nodes.Add(TagTarget, deployment.TargetPath);
						nodeML.Nodes.Add(TagJsonParameters, deployment.JsonParameters);
						nodeML.Attributes.Add(TagWriteComments, deployment.WriteComments);
						nodeML.Attributes.Add(TagReplaceArguments, deployment.ReplaceArguments);
						nodeML.Attributes.Add(TagLowcaseFileNames, deployment.LowcaseFileNames);
				}
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene los nodos con las carpetas de una solución
		/// </summary>
		private MLNodesCollection GetFoldersNodes(SolutionModel solution)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los datos
				foreach (string folder in solution.Folders)
					nodesML.Add(TagFolder, folder);
				// Devuelve la colección de nodos
				return nodesML;
		}
	}
}
