﻿using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.JobsProcessor.Application.Models;

namespace Bau.Libraries.JobsProcessor.Application.Repository;

/// <summary>
///		Repositorio de <see cref="Models.CommandModel"/>
/// </summary>
internal class ConsoleRepository
{
	// Constantes privadas
	private const string TagRoot = "Project";
	private const string TagContextsRoot = "Contexts";
	private const string TagContext = "Context";
	private const string TagParameter = "Parameter";
	private const string TagCommandsRoot = "Commands";
	private const string TagCommand = "Command";
	private const string TagFileName = "FileName";
	private const string TagStopWhenError = "StopWhenError";
	private const string TagArgument = "Argument";
	private const string TagEnvironment = "Environment";
	private const string TagName = "Name";
	private const string TagValue = "Value";

	/// <summary>
	///		Carga los datos de consola de un archivo
	/// </summary>
	internal ProjectModel Load(string fileName)
	{
		MLFile fileML = new Bau.Libraries.LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);
		ProjectModel project = new();

			// Carga los datos del proyecto
			foreach (MLNode rootML in fileML.Nodes)
				if (rootML.Name == TagRoot)
					foreach (MLNode nodeML in rootML.Nodes)
						switch (nodeML.Name)
						{
							case TagContextsRoot:
									project.Contexts.AddRange(LoadContexts(nodeML));
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
		CommandModel command = new CommandModel
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
				}
			// Devuelve los datos del comando
			return command;
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
	private IEnumerable<ContextModel> LoadContexts(MLNode rootML)
	{
		foreach (MLNode nodeML in rootML.Nodes)
			if (nodeML.Name == TagContext)
				yield return LoadContext(nodeML);
	}

	/// <summary>
	///		Carga los datos de un contexto
	/// </summary>
	private ContextModel LoadContext(MLNode rootML)
	{
		ContextModel context = new();

			// Añade los parámetros
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagParameter)
				{
					string name = nodeML.Attributes[TagName].Value.TrimIgnoreNull();
					string value = nodeML.Attributes[TagValue].Value.TrimIgnoreNull();

						// Recoge el valor del cuerpo del nodo
						if (string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(nodeML.Value))
							value = nodeML.Value.TrimIgnoreNull();
						// Añade el valor al diccionario
						context.Parameters.Add(new ParameterModel
														{
															Name = name, 
															Value = value
														}
												);
				}
			// Devuelve los datos
			return context;
	}
}