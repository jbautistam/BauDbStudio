using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibDataStructures.Base;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Manager.Repository
{
	/// <summary>
	///		Clase de lectura de <see cref="JobProjectModel"/>
	/// </summary>
	internal class JobProjectRepository
	{
		// Constantes privadas
		private const string TagRoot = "EtlProject";
		private const string TagName = "Name";
		private const string TagDescription = "Description";
		private const string TagProcessor = "Processor";
		private const string TagStep = "Step";
		private const string TagTarget = "Target";
		private const string TagScript = "Script";
		private const string TagParallel = "Parallel";
		private const string TagEnabled = "Enabled";
		private const string TagStartWithPreviousError = "StartWithPreviousError";

		/// <summary>
		///		Carga los datos de un <see cref="JobProjectModel"/>
		/// </summary>
		internal JobProjectModel Load(string fileName)
		{
			JobProjectModel project = new JobProjectModel(System.IO.Path.GetDirectoryName(fileName));

				// Carga los datos del archivo
				if (System.IO.File.Exists(fileName))
				{
					MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

						// Obtiene los nodos
						if (fileML != null)
							foreach (MLNode rootML in fileML.Nodes)
								if (rootML.Name == TagRoot)
								{
									// Asigna el nombre del proyecto
									project.Name = rootML.Nodes[TagName].Value.TrimIgnoreNull();
									project.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
									// Carga los contextos
									project.Contexts.AddRange(new JobContextRepository().LoadContexts(rootML));
									// Carga los trabajos
									foreach (MLNode nodeML in rootML.Nodes)
										switch (nodeML.Name)
										{
											case TagStep:
													project.Jobs.Add(LoadStep(nodeML, project, null));
												break;
										}
								}
				}
				// Asigna el directorio de trabajo del contexto
				project.ContextWorkPath = project.ProjectWorkPath;
				// Devuelve los datos del proyecto
				return project;
		}

		/// <summary>
		///		Carga los datos de un paso
		/// </summary>
		private JobStepModel LoadStep(MLNode rootML, JobProjectModel project, JobStepModel parent)
		{
			JobStepModel step = new JobStepModel(project, parent);

				// Asigna las propiedades básicas
				step.Name = rootML.Nodes[TagName].Value.TrimIgnoreNull();
				step.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
				step.ScriptFileName = rootML.Attributes[TagScript].Value.TrimIgnoreNull();
				step.Target = rootML.Attributes[TagTarget].Value.TrimIgnoreNull();
				step.ProcessorKey = rootML.Attributes[TagProcessor].Value.TrimIgnoreNull();
				step.StartWithPreviousError = rootML.Attributes[TagStartWithPreviousError].Value.GetBool(false);
				step.Enabled = rootML.Attributes[TagEnabled].Value.GetBool(true);
				step.Parallel = rootML.Attributes[TagParallel].Value.GetBool(false);
				// Carga los pasos
				step.Steps.AddRange(LoadSteps(rootML, project, step));
				// Carga el contexto
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == JobContextRepository.TagContext)
						step.Context = new JobContextRepository().LoadContext(rootML, nodeML.Attributes[TagProcessor].Value.TrimIgnoreNull());
				// Carga los parámetros
				step.Context.Parameters.AddRange(new JobParameterRepository().LoadParameters(rootML));
				// Devuelve los datos del paso
				return step;
		}

		/// <summary>
		///		Carga una serie de pasos
		/// </summary>
		private List<JobStepModel> LoadSteps(MLNode rootML, JobProjectModel project, JobStepModel parent)
		{
			List<JobStepModel> steps = new List<JobStepModel>();

				// Carga los pasos
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagStep)
						steps.Add(LoadStep(nodeML, project, parent));
				// Devuelve la colección de pasos
				return steps;
		}
		/// <summary>
		///		Graba un archivo con los datos de un proyecto
		/// </summary>
		internal void Save(JobProjectModel project, string fileName)
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add(TagRoot);

				// Añade los nodos básicos
				rootML.Nodes.Add(TagName, project.Name);
				rootML.Nodes.Add(TagDescription, project.Description);
				// Añade los nodos de contexto
				rootML.Nodes.AddRange(new JobContextRepository().GetContextNodes(project.Contexts));
				// Añade los nodos de trabajo
				rootML.Nodes.AddRange(GetStepNodes(project.Jobs));
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
		}

		/// <summary>
		///		Obtiene los nodos de <see cref="JobStepModel"/>
		/// </summary>
		private MLNodesCollection GetStepNodes(BaseExtendedModelCollection<JobStepModel> steps)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Crea la colección de nodos con los pasos
				foreach (JobStepModel step in steps)
					nodesML.Add(GetStepNode(step));
				// Devuelve la colección de nodos
				return nodesML;
		}

		/// <summary>
		///		Obtiene el nodo de un <see cref="JobStepModel"/>
		/// </summary>
		private MLNode GetStepNode(JobStepModel step)
		{
			MLNode rootML = new MLNode(TagStep);

				// Añade los datos básicos
				rootML.Nodes.Add(TagName, step.Name);
				rootML.Nodes.Add(TagDescription, step.Description);
				rootML.Attributes.Add(TagScript, step.ScriptFileName);
				rootML.Attributes.Add(TagTarget, step.Target);
				rootML.Attributes.Add(TagProcessor, step.ProcessorKey);
				rootML.Attributes.Add(TagStartWithPreviousError, step.StartWithPreviousError);
				rootML.Attributes.Add(TagEnabled, step.Enabled);
				rootML.Attributes.Add(TagParallel, step.Parallel);
				// Añade los nodos de los pasos hijo
				rootML.Nodes.AddRange(GetStepNodes(step.Steps));
				// Añade los nodos del contexto
				rootML.Nodes.Add(new JobContextRepository().GetContextNode(JobContextRepository.TagContext, string.Empty, step.Context));
				// Añade los parámetros
				rootML.Nodes.AddRange(new JobParameterRepository().GetParametersNodes(step.Context.Parameters));
				// Devuelve el nodo
				return rootML;
		}
	}
}