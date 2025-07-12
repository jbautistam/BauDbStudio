using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.JobsProcessor.Application.Models;

namespace Bau.Libraries.JobsProcessor.Application.Repository;

/// <summary>
///		Repositorio de <see cref="CommandModel"/>
/// </summary>
internal class ConsoleRepository
{
	// Constantes privadas
	private const string TagRoot = "Project";
	private const string TagContextsRoot = "Contexts";
	private const string TagContext = "Context";
	private const string TagImport = "Import";
	private const string TagParameter = "Parameter";
	private const string TagCommandsRoot = "Commands";
	private const string TagCommand = "Command";
	private const string TagFileName = "FileName";
	private const string TagStopWhenError = "StopWhenError";
	private const string TagArgument = "Argument";
	private const string TagEnvironment = "Environment";
	private const string TagName = "Name";
	private const string TagValue = "Value";
	private const string TagExitCodeValidWhen = "ExitCodeValidWhen";
	private const string TagMinimum = "Minimum";
	private const string TagMaximum = "Maximum";

	/// <summary>
	///		Carga los datos de consola de un archivo
	/// </summary>
	internal ProjectModel Load(string fileName)
	{
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);
		ProjectModel project = new();

			// Carga los datos del proyecto
			foreach (MLNode rootML in fileML.Nodes)
				if (rootML.Name == TagRoot)
					foreach (MLNode nodeML in rootML.Nodes)
						switch (nodeML.Name)
						{
							case TagContextsRoot:
									project.Contexts.AddRange(LoadContexts(nodeML, Path.GetDirectoryName(fileName)!));
								break;
							case TagCommandsRoot:
									project.Commands.AddRange(LoadCommands(nodeML));	
								break;
						}
			// Devuelve el proyecto
			return project;
	}

	/// <summary>
	///		Carga los comandos
	/// </summary>
	private IEnumerable<CommandModel> LoadCommands(MLNode rootML)
	{
		foreach (MLNode nodeML in rootML.Nodes)
			if (nodeML.Name == TagCommand)
				yield return LoadCommand(nodeML);
	}

	/// <summary>
	///		Carga los datos de un comando
	/// </summary>
	private CommandModel LoadCommand(MLNode rootML)
	{
		CommandModel command = new()
									{
										FileName = rootML.Attributes[TagFileName].Value.TrimIgnoreNull(),
										StopWhenError = rootML.Attributes[TagStopWhenError].Value.GetBool(true)
									};

			// Carga los argumentos del comando
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagArgument:
							command.Arguments.Add(LoadArgument(ArgumentModel.ArgumentPosition.CommandLine, nodeML));
						break;
					case TagEnvironment:
							command.Arguments.Add(LoadArgument(ArgumentModel.ArgumentPosition.Environment, nodeML));
						break;
					case TagExitCodeValidWhen:
							command.ValidationExitCode.Add(LoadExitCodeValid(nodeML));
						break;
				}
			// Devuelve los datos del comando
			return command;
	}

	/// <summary>
	///		Carga los datos de un <see cref="CommandValidationExitCodeModel"/>
	/// </summary>
	private CommandValidationExitCodeModel LoadExitCodeValid(MLNode nodeML)
	{
		return new CommandValidationExitCodeModel
							{
								Minimum = nodeML.Attributes[TagMinimum].Value.GetInt(),
								Maximum = nodeML.Attributes[TagMaximum].Value.GetInt()
							};
	}

	/// <summary>
	///		Carga los datos de un argumento
	/// </summary>
	private ArgumentModel LoadArgument(ArgumentModel.ArgumentPosition position, MLNode rootML)
	{
		ArgumentModel argument = new();

			// Asigna las propiedades
			argument.Parameter.Name = rootML.Attributes[TagName].Value.TrimIgnoreNull();
			argument.Position = position;
			if (string.IsNullOrWhiteSpace(rootML.Value))
				argument.Parameter.Value = rootML.Attributes[TagValue].Value.TrimIgnoreNull();
			else
				argument.Parameter.Value = rootML.Value;
			// Devuelve los datos del objeto
			return argument;
	}

	/// <summary>
	///		Carga la lista de contextos
	/// </summary>
	private IEnumerable<ContextModel> LoadContexts(MLNode rootML, string folder)
	{
		foreach (MLNode nodeML in rootML.Nodes)
			if (nodeML.Name == TagContext)
				yield return LoadContext(nodeML, folder);
	}

	/// <summary>
	///		Carga los datos de un contexto
	/// </summary>
	private ContextModel LoadContext(MLNode rootML, string folder)
	{
		ContextModel context = new();

			// Añade los parámetros y las importaciones
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagImport:
							if (!string.IsNullOrWhiteSpace(nodeML.Attributes[TagFileName].Value))
								ImportContextFile(context, Path.Combine(folder, nodeML.Attributes[TagFileName].Value.TrimIgnoreNull()));
						break;
					case TagParameter:
							context.Add(LoadParameter(nodeML));
						break;
				}
			// Devuelve los datos
			return context;
	}

	/// <summary>
	///		Carga los datos de un parámetro de un nodo
	/// </summary>
	private ParameterModel LoadParameter(MLNode rootML)
	{
		string name = rootML.Attributes[TagName].Value.TrimIgnoreNull();
		string value = rootML.Attributes[TagValue].Value.TrimIgnoreNull();

			// Recoge el valor del cuerpo del nodo
			if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(rootML.Value))
				value = rootML.Value.TrimIgnoreNull();
			// Devuelve los datos del parámetro
			return new ParameterModel
							{
								Name = name, 
								Value = value
							};
	}

	/// <summary>
	///		Importa un archivo XML de contexto sobre el contexto actual
	/// </summary>
	private void ImportContextFile(ContextModel context, string fileName)
	{
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			foreach (MLNode rootML in fileML.Nodes)
				if (rootML.Name == TagContext)
				{
					ContextModel children = LoadContext(rootML, Path.GetDirectoryName(fileName)!);

						// Añade los datos al contexto original
						context.Add(children);
				}
	}
}