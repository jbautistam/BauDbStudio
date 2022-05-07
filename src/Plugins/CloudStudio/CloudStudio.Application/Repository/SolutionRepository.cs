using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.CloudStudio.Models;

namespace Bau.Libraries.CloudStudio.Application.Repository
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
		private const string TagId = "Id";
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
							// Carga los objetos
							LoadStorages(solution, rootML);
						}
				// Devuelve la solución
				return solution;
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
				// Añade los objetos
				rootML.Nodes.AddRange(GetStoragesNodes(solution));
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
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
	}
}
