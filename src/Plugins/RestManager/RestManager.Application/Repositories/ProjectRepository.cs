using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.Application.Repositories;

/// <summary>
///		Repositorio de proyectos
/// </summary>
internal class ProjectRepository
{
	// Constantes privadas
	private const string TagRoot = "RestProject";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagEnabled = "Enabled";
	private const string TagParameter = "Parameter";
	private const string TagKey = "Key";
	private const string TagValue = "Value";
	private const string TagHeader = "Header";
	private const string TagUrl = "Url";
	private const string TagMethod = "Method";
	private const string TagContent = "Content";
	private const string TagRestStep = "RestStep";
	
	/// <summary>
	///		Carga un proyecto del archivo
	/// </summary>
	internal RestProjectModel Load(string fileName)
	{
		RestProjectModel project = new();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los datos
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagName:
										project.Name = nodeML.Value;
									break;
								case TagDescription:
										project.Description = nodeML.Value;
									break;
								case TagParameter:
										project.Parameters.Add(GetParameter(nodeML));
									break;
								case TagHeader:
										project.Headers.Add(GetParameter(nodeML));
									break;
								case TagRestStep:
										project.Steps.Add(GetRestStep(nodeML));
									break;
							}
			// Devuelve el proyecto
			return project;
	}

	/// <summary>
	///		Obtiene los datos de un parámetro
	/// </summary>
	public ParameterModel GetParameter(MLNode rootML)
	{
		return new ParameterModel(rootML.Attributes[TagKey].Value.TrimIgnoreNull(), rootML.Attributes[TagValue].Value.TrimIgnoreNull());
	}

	/// <summary>
	///		Obtiene los datos de un paso Rest
	/// </summary>
	public BaseStepModel GetRestStep(MLNode rootML)
	{
		RestStepModel step = new();

			// Obtiene los datos del paso
			step.Name = rootML.Nodes[TagName].Value.TrimIgnoreNull();
			step.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
			step.Method = rootML.Attributes[TagMethod].Value.GetEnum(RestStepModel.RestMethod.Get);
			step.Url = rootML.Attributes[TagUrl].Value.TrimIgnoreNull();
			step.Enabled = rootML.Attributes[TagEnabled].Value.GetBool();
			step.Content = rootML.Nodes[TagContent].Value.TrimIgnoreNull();
			// Añade los datos
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagHeader)
					step.Headers.Add(GetParameter(nodeML));
			// Devuelve el paso
			return step;
	}

	/// <summary>
	///		Graba los datos del proyecto
	/// </summary>
	internal void Save(RestProjectModel project, string fileName)
	{
		MLFile fileML = new MLFile();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los datos del proyecto
			rootML.Nodes.Add(TagName, project.Name);
			rootML.Nodes.Add(TagDescription, project.Description);
			rootML.Nodes.AddRange(GetXmlParameters(TagParameter, project.Parameters));
			rootML.Nodes.AddRange(GetXmlParameters(TagHeader, project.Headers));
			// Añade los datos de los pasos
			rootML.Nodes.AddRange(GetXmlSteps(project.Steps));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene los nodos de parámetros
	/// </summary>
	private MLNodesCollection GetXmlParameters(string tag, List<ParameterModel> parameters)
	{
		MLNodesCollection nodesML = new();

			// Añade los nodos
			foreach (ParameterModel parameter in parameters)
			{
				MLNode nodeML = new(tag);

					// Añade los atributos
					nodeML.Attributes.Add(TagKey, parameter.Key);
					nodeML.Attributes.Add(TagValue, parameter.Value);
					// Añade el nodo a la colección
					nodesML.Add(nodeML);
			}
			// Devuelve la colección
			return nodesML;
	}

	/// <summary>
	///		Obtiene los nodos de pasos
	/// </summary>
	private MLNodesCollection GetXmlSteps(List<BaseStepModel> steps)
	{
		MLNodesCollection nodesML = new();

			// Añade los nodos de los pasos
			foreach (BaseStepModel step in steps)
				if (step is RestStepModel restStep)
					nodesML.Add(GetXmlRestStep(restStep));
			// Devuelve la colección de nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene el nodo de un paso Rest
	/// </summary>
	private MLNode GetXmlRestStep(RestStepModel step)
	{
		MLNode rootML = new(TagRestStep);

			// Añade los datos
			rootML.Nodes.Add(TagName, step.Name);
			rootML.Nodes.Add(TagDescription, step.Description);
			rootML.Attributes.Add(TagMethod, step.Method.ToString());
			rootML.Attributes.Add(TagUrl, step.Url);
			rootML.Attributes.Add(TagEnabled, step.Enabled);
			rootML.Nodes.Add(TagContent, step.Content);
			// Añade las cabeceras
			rootML.Nodes.AddRange(GetXmlParameters(TagHeader, step.Headers));
			// Devuelve el nodo
			return rootML;
	}
}