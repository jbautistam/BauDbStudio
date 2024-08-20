using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.AiTools.Application.Models.Prompts;

namespace Bau.Libraries.AiTools.Application.Repository;

/// <summary>
///		Clase para manejo de archivos de prompt
/// </summary>
internal class PromptsRepository
{
	// Constantes privadas
	private const string TagRoot = "PromptFile";
	private const string TagPrompt = "Prompt";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagVersion = "Version";
	private const string TagPositive = "Positive";
	private const string TagNegative = "Negative";
	private const string TagProvider = "Provider";
	private const string TagModel = "Model";
	private const string TagNsfw = "Nsfw";
	private const string TagSampler = "Sampler";
	private const string TagCfgScale = "CfgScale";
	private const string TagDenoisingStrength = "DenoisingStrength";
	private const string TagSeed = "Seed";
	private const string TagHeight = "Height";
	private const string TagWidth = "Width";
	private const string TagPostProcessing = "PostProcessing";
	private const string TagKarras = "Karras";
	private const string TagSteps = "Steps";
	private const string TagImagesToGenerate = "ImagesToGenerate";

	/// <summary>
	///		Carga el archivo
	/// </summary>
	internal PromptFileModel Load(string fileName)
	{
		PromptFileModel promptFile = new(fileName);
		MLFile? fileML = new Bau.Libraries.LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga el archivo
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagName:
										promptFile.Name = nodeML.Value.TrimIgnoreNull();
									break;
								case TagDescription:
										promptFile.Description = nodeML.Value.TrimIgnoreNull();
									break;
								case TagPrompt:
										promptFile.Prompts.Add(LoadPrompt(nodeML));
									break;
							}
			// Devuelve el archivo cargado
			return promptFile;
	}

	/// <summary>
	///		Carga el prompt de un nodo
	/// </summary>
	private PromptModel LoadPrompt(MLNode rootML)
	{
		PromptModel prompt = new(rootML.Attributes[TagProvider].Value.TrimIgnoreNull())
						{
							Version = rootML.Attributes[TagVersion].Value.GetInt(1),
							Positive = rootML.Nodes[TagPositive].Value.TrimIgnoreNull(),
							Negative = rootML.Nodes[TagNegative].Value.TrimIgnoreNull(),
							Model = rootML.Attributes[TagModel].Value.TrimIgnoreNull(),
							Nsfw = rootML.Attributes[TagNsfw].Value.GetBool(),
							Sampler = rootML.Attributes[TagSampler].Value.GetEnum(PromptModel.SamplerType.k_euler),
							CfgScale = rootML.Attributes[TagCfgScale].Value.GetDouble(7.5),
							DenoisingStrength = rootML.Attributes[TagDenoisingStrength].Value.GetDouble(0.75),
							Seed = rootML.Attributes[TagSeed].Value.TrimIgnoreNull(),
							Height = rootML.Attributes[TagHeight].Value.GetInt(512),
							Width = rootML.Attributes[TagWidth].Value.GetInt(512),
							Karras = rootML.Attributes[TagKarras].Value.GetBool(),
							Steps = rootML.Attributes[TagSteps].Value.GetInt(30),
							ImagesToGenerate = rootML.Attributes[TagImagesToGenerate].Value.GetInt(4)
						};

			// Añade los datos de postproceso
			prompt.PostProcessing.AddRange(GetPostprocessing(rootML.Attributes[TagPostProcessing].Value.TrimIgnoreNull()));
			// Devuelve el prompt
			return prompt;
	}

	/// <summary>
	///		Obtiene los datos de postprocesamiento
	/// </summary>
	private List<PromptModel.PostProcessType> GetPostprocessing(string postProcess)
	{
		List<PromptModel.PostProcessType> processes = new();

			// Asigna los datos
			if (!string.IsNullOrWhiteSpace(postProcess))
				foreach (string part in postProcess.Split(';'))
					if (!string.IsNullOrWhiteSpace(part))
						processes.Add(part.GetEnum(PromptModel.PostProcessType.RealESRGAN_x4plus));
			// Devuelve los datos
			return processes;
	}

	/// <summary>
	///		Graba los archivos de un nodo
	/// </summary>
	internal void Save(string fileName, PromptFileModel promptFile)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los datos del archivo
			rootML.Nodes.Add(TagName, promptFile.Name);
			rootML.Nodes.Add(TagDescription, promptFile.Description);
			// Añade los prompts
			foreach (PromptModel prompt in promptFile.Prompts)
				rootML.Nodes.Add(GetPromptNode(prompt));
			// Graba el archivo
			new Bau.Libraries.LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene el nodo de un <see cref="PromptModel"/>
	/// </summary>
	private MLNode GetPromptNode(PromptModel prompt)
	{
		MLNode rootML = new(TagPrompt);

			// Añade los datos
			rootML.Attributes.Add(TagVersion, prompt.Version);
			rootML.Nodes.Add(TagPositive, prompt.Positive);
			rootML.Nodes.Add(TagNegative, prompt.Negative);
			rootML.Attributes.Add(TagProvider, prompt.Provider);
			rootML.Attributes.Add(TagModel, prompt.Model);
			rootML.Attributes.Add(TagNsfw, prompt.Nsfw);
			rootML.Attributes.Add(TagSampler, (int) prompt.Sampler);
			rootML.Attributes.Add(TagCfgScale, prompt.CfgScale);
			rootML.Attributes.Add(TagDenoisingStrength, prompt.DenoisingStrength);
			rootML.Attributes.Add(TagSeed, prompt.Seed);
			rootML.Attributes.Add(TagHeight, prompt.Height);
			rootML.Attributes.Add(TagWidth, prompt.Width);
			rootML.Attributes.Add(TagPostProcessing, GetXmlPostprocessing(prompt.PostProcessing));
			rootML.Attributes.Add(TagKarras, prompt.Karras);
			rootML.Attributes.Add(TagSteps, prompt.Steps);
			rootML.Attributes.Add(TagImagesToGenerate, prompt.ImagesToGenerate);
			// Devuelve el nodo generado
			return rootML;
	}

	/// <summary>
	///		Obtiene la cadena XML de postprocesamiento
	/// </summary>
	private string GetXmlPostprocessing(List<PromptModel.PostProcessType> postProcessing)
	{
		string result = string.Empty;

			// Concatena los tipos de postproceso
			foreach (PromptModel.PostProcessType postProcess in postProcessing)
				result = result.AddWithSeparator(postProcess.ToString(), ";");
			// Devuelve el resultado
			return result;
	}
}
