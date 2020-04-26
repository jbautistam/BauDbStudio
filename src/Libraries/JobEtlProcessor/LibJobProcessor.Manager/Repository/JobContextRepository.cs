using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDataStructures.Trees;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Manager.Repository
{
	/// <summary>
	///		Clase de lectura de <see cref="JobContextModel"/>
	/// </summary>
	internal class JobContextRepository
	{
		// Constantes privadas
		private const string TagRoot = "EtlContext";
		private const string TagProcessor = "Processor";
		private const string TagGlobalContext = "Global";
		internal const string TagContext = "Context";
		private const string TagPath = "Path";
		private const string TagKey = "Key";
		private const string TagValue = "Value";

		/// <summary>
		///		Carga los datos de una serie de <see cref="JobContextModel"/>
		/// </summary>
		internal List<JobContextModel> Load(string fileName)
		{
			List<JobContextModel> contexts = new List<JobContextModel>();

				// Carga los datos del archivo
				if (System.IO.File.Exists(fileName))
				{
					MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

						if (fileML != null)
							foreach (MLNode rootML in fileML.Nodes)
								if (rootML.Name == TagRoot)
									contexts.AddRange(LoadContexts(rootML));
				}
				// Devuelve los datos de contexto
				return contexts;
		}

		/// <summary>
		///		Carga los contextos de un nodo
		/// </summary>
		internal List<JobContextModel> LoadContexts(MLNode rootML)
		{
			List<JobContextModel> contexts = new List<JobContextModel>();

				// Carga los contextos
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagGlobalContext:
								contexts.Add(LoadContext(nodeML, JobProjectModel.GlobalContext));
							break;
						case TagContext:
								contexts.Add(LoadContext(nodeML, nodeML.Attributes[TagProcessor].Value.TrimIgnoreNull()));
							break;
					}
				// Devuelve la colección de contextos
				return contexts;
		}

		/// <summary>
		///		Carga los datos de un contexto
		/// </summary>
		internal JobContextModel LoadContext(MLNode rootML, string processorKey)
		{
			JobContextModel context = new JobContextModel();

				// Carga los datos del contexto
				context.ProcessorKey = processorKey;
				// Carga los parámetros y los nodos
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case JobParameterRepository.TagParameter:
								JobParameterModel parameter = new JobParameterRepository().LoadParameter(nodeML);

									context.Parameters.Add(parameter.Key, parameter);
							break;
						case TagPath:
								if (string.IsNullOrWhiteSpace(nodeML.Attributes[TagValue].Value))
									context.Paths.Add(nodeML.Attributes[TagKey].Value.TrimIgnoreNull(), nodeML.Value.TrimIgnoreNull());
								else
									context.Paths.Add(nodeML.Attributes[TagKey].Value.TrimIgnoreNull(), 
													  nodeML.Attributes[TagValue].Value.TrimIgnoreNull());
							break;
						default:
								context.Tree.Nodes.Add(LoadTreeNode(nodeML));
							break;
					}
				// Devuelve el contexto
				return context;
		}

		/// <summary>
		///		Carga un árbol del nodo
		/// </summary>
		private TreeNodeModel LoadTreeNode(MLNode rootML)
		{
			TreeNodeModel node = new TreeNodeModel();

				// Asigna el nombre del nodo al Id
				node.Id = rootML.Name;
				// Carga los atributos
				foreach (MLAttribute attribute in rootML.Attributes)
					node.Attributes.Add(attribute.Name.TrimIgnoreNull(), attribute.Value.TrimIgnoreNull());
				// Carga los nodos hijo
				if (rootML.Nodes.Count == 0)
					node.Value = rootML.Value.TrimIgnoreNull();
				else
					foreach (MLNode nodeML in rootML.Nodes)
						node.Nodes.Add(LoadTreeNode(nodeML));
				// Devuelve el nodo
				return node;
		}

		/// <summary>
		///		Graba un archivo con los datos de contexto
		/// </summary>
		internal void Save(List<JobContextModel> contexts, string fileName)
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add(TagRoot);

				// Añade los nodos de contexto
				rootML.Nodes.AddRange(GetContextNodes(contexts));
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
		}

		/// <summary>
		///		Obtiene los nodos de un contexto
		/// </summary>
		internal MLNodesCollection GetContextNodes(List<JobContextModel> contexts)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Obtiene los nodos
				foreach (JobContextModel context in contexts)
					if (context.ProcessorKey.Equals(JobProjectModel.GlobalContext, StringComparison.CurrentCultureIgnoreCase))
						nodesML.Add(GetContextNode(TagGlobalContext, string.Empty, context));
					else
						nodesML.Add(GetContextNode(TagContext, context.ProcessorKey, context));
				// Devuelve los nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene los nodos de una serie de <see cref="JobContextModel"/>
		/// </summary>
		internal MLNode GetContextNode(string tag, string processorKey, JobContextModel context)
		{
			MLNode nodeML = new MLNode(tag);

				// Añade la clave del procesador
				if (string.IsNullOrEmpty(processorKey))
					nodeML.Attributes.Add(TagProcessor, processorKey);
				// Añade el árbol
				nodeML.Nodes.AddRange(GetContextTreeNodes(context.Tree.Nodes));
				// Añade los parámetros
				nodeML.Nodes.AddRange(new JobParameterRepository().GetParametersNodes(context.Parameters));
				// Añade los directorios
				foreach ((string key, string value) in context.Paths.Enumerate())
				{
					MLNode pathML = nodeML.Nodes.Add(TagPath);

						pathML.Attributes.Add(TagKey, key);
						pathML.Value = value;
				}
				// Devuelve el nodo
				return nodeML;
		}

		/// <summary>
		///		Obtiene los nodos XML de los nodos del árbol
		/// </summary>
		private MLNodesCollection GetContextTreeNodes(List<TreeNodeModel> nodes)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los nodos del árbol
				foreach (TreeNodeModel node in nodes)
				{
					MLNode nodeML = nodesML.Add(node.Id);

						// Añade los atributos
						foreach (ParameterModel parameter in node.Attributes)
							node.Attributes.Add(parameter.Id, parameter.Value);
						// Añade los nodos hijo
						if (node.Nodes.Count == 0)
							nodeML.Value = node.Value?.ToString();
						else
							nodeML.Nodes.AddRange(GetContextTreeNodes(node.Nodes));
				}
				// Devuelve la colección de nodos
				return nodesML;
		}
	}
}