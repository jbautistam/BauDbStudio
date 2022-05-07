using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.JobsProcessor.Application.EventArguments;
using Bau.Libraries.JobsProcessor.Application.Models;

namespace Bau.Libraries.JobsProcessor.ViewModel.Processor.LogTree
{
	/// <summary>
	///		ViewModel para el árbol de páginas de log de contexto / comandos en ejecución
	/// </summary>
	public class TreeLogViewModel : PluginsStudio.ViewModels.Base.Explorers.BaseTreeViewModel
	{   
		// Enumerados públicos
		/// <summary>
		///		Tipo de nodo
		///	¡</summary>
		public enum NodeType
		{
			/// <summary>Desconocido. No se debería uitlizar</summary>
			Unknown,
			/// <summary>Nodo de contexto</summary>
			Context,
			/// <summary>Nodo de comando</summary>
			Command,
			/// <summary>Nodo de texto</summary>
			Text,
			/// <summary>Nodo de parámetro</summary>
			Parameter
		}

		// Variables privadas
		private System.Threading.SynchronizationContext _contextUi = System.Threading.SynchronizationContext.Current;

		public TreeLogViewModel(ExecuteConsoleViewModel mainViewModel)
		{
			MainViewModel = mainViewModel;
		}

		/// <summary>
		///		Carga los nodos
		/// </summary>
		protected override void AddRootNodes()
		{
			// ... no hace nada, sólo implementa la interface
		}

		/// <summary>
		///		Escribe la información en el log
		/// </summary>
		internal void WriteLog(JobProcessEventArgs item)
		{
			object state = new object();

				//? _contexUi mantiene el contexto de sincronización que creó el ViewModel (que debería ser la interface de usuario)
				//? Al generarse el log en un evento interno, no se puede añadir a ObservableCollection sin una
				//? excepción del tipo "Este tipo de CollectionView no admite cambios en su SourceCollection desde un hilo diferente del hilo Dispatcher"
				//? Por eso se tiene que añadir el mensaje de log desde el contexto de sincronización de la UI
				// Añade el mensaje
				_contextUi.Send(_ => AddNode(item), state);
		}

		/// <summary>
		///		Añade los datos de progreso al árbol
		/// </summary>
		private void AddNode(JobProcessEventArgs item)
		{
			if (item.Context == null || item.Command == null)
				AddLogMessage(null, item);
			else
			{
				LogNodeViewModel contextNode = GetNode(item.Context);
				LogNodeViewModel commandNode = GetNode(item.Context, item.Command, contextNode);

					// Asigna visualmente los datos al nodo adecuado
					if (commandNode != null)
						AddLog(commandNode, item);
					else if (contextNode != null)
						AddLog(contextNode, item);
			}
		}

		/// <summary>
		///		Escribe el log
		/// </summary>
		private void AddLog(LogNodeViewModel nodeViewModel, JobProcessEventArgs item)
		{
			// Cambia el color del nodo
			UpdateNodeColor(nodeViewModel, item.Status);
			// Añade el mensaje
			if (!string.IsNullOrWhiteSpace(item.Message))
				AddLogMessage(nodeViewModel, item);
		}

		/// <summary>
		///		Añade un mensaje de log al nodo
		/// </summary>
		private void AddLogMessage(LogNodeViewModel parent, JobProcessEventArgs item)
		{
			LogNodeViewModel node = new LogNodeViewModel(this, parent, GetMessage(item), NodeType.Text, item);
			
				// Cambia el color del nodo
				UpdateNodeColor(node, item.Status);
				// Añade el nodo
				if (parent == null)
					Children.Add(node);
				else
					parent.Children.Add(node);
		}

		/// <summary>
		///		Obtiene el mensaje de un evento
		/// </summary>
		private string GetMessage(JobProcessEventArgs item)
		{
			string message = $"{item.Date:HH:mm:ss.fff} - {item.Message}";

				// Añade el progreso
				if (item.Actual != null && item.Total != null)
					message += $" {item.Actual:#,##0} / {item.Total:#,##0}";
				// Devuelve el mensaje completo
				return message;
		}

		/// <summary>
		///		Cambia el color del nodo
		/// </summary>
		private void UpdateNodeColor(LogNodeViewModel nodeViewModel, JobProcessEventArgs.StatusType status)
		{
			switch (status)
			{
				case JobProcessEventArgs.StatusType.Error:
						nodeViewModel.Foreground = BauMvvm.ViewModels.Media.MvvmColor.Red;
					break;
				case JobProcessEventArgs.StatusType.Start:
						nodeViewModel.Foreground = BauMvvm.ViewModels.Media.MvvmColor.Navy;
					break;
				case JobProcessEventArgs.StatusType.End:
						nodeViewModel.Foreground = BauMvvm.ViewModels.Media.MvvmColor.Black;
					break;
			}
		}

		/// <summary>
		///		Obtiene el nodo asociado al contexto en el árbol o crea uno nuevo
		/// </summary>
		private LogNodeViewModel GetNode(ContextModel context)
		{
			// Busca el nodo en el árbol
			foreach (LogNodeViewModel node in Children)
				if (node.Tag is ContextModel nodeContext && nodeContext.Id.Equals(context.Id, StringComparison.CurrentCultureIgnoreCase))
					return node;
			// Si no existía, crea uno nuevo
			return CreateNode(context);
		}

		/// <summary>
		///		Obtiene el nodo asociado al comando en el árbol o crea uno nuevo
		/// </summary>
		private LogNodeViewModel GetNode(ContextModel context, CommandModel command, LogNodeViewModel contextNode)
		{
			// Busca el nodo hijo
			foreach (LogNodeViewModel commandNode in contextNode.Children)
				if (commandNode.Tag is CommandModel nodeCommand && nodeCommand.Id.Equals(command.Id, StringComparison.CurrentCultureIgnoreCase))
					return commandNode;
			// Si no existía, crea uno
			return CreateNode(contextNode, context, command);
		}

		/// <summary>
		///		Crea un nodo con los datos de contexto
		/// </summary>
		private LogNodeViewModel CreateNode(ContextModel context)
		{
			LogNodeViewModel node = new LogNodeViewModel(this, null, "Context", NodeType.Context, context);

				// Añade los parámetros del contexto
				CreateParameterNodes(node, context);
				// Añade el nodo
				Children.Add(node);
				// Devuelve el nodo creado
				return node;
		}

		/// <summary>
		///		Añade los nodos de parámetros
		/// </summary>
		private void CreateParameterNodes(LogNodeViewModel parent, ContextModel context)
		{
			LogNodeViewModel folder = new LogNodeViewModel(this, parent, "Parameters", NodeType.Parameter, null);

				// Añade la carpeta
				parent.Children.Add(folder);
				folder.IsBold = true;
				// Añade los parámetros
				if (context.Parameters.Count > 0)
					foreach (ParameterModel parameter in context.Parameters)
						folder.Children.Add(new LogNodeViewModel(this, folder, $"{parameter.Name}: {parameter.Value}", NodeType.Parameter, parameter));
				else
					folder.Children.Add(new LogNodeViewModel(this, folder, $"No context parameters", NodeType.Parameter, null));
		}

		/// <summary>
		///		Crea un nodo con los datos de un comando
		/// </summary>
		private LogNodeViewModel CreateNode(LogNodeViewModel contextNode, ContextModel context, CommandModel command)
		{
			LogNodeViewModel node = new LogNodeViewModel(this, contextNode, $"Command {System.IO.Path.GetFileName(command.FileName)}", 
														 NodeType.Command, command);

				// Añade el nodo
				contextNode.IsExpanded = true;
				contextNode.Children.Add(node);
				// Añade los nodos de argumentos
				CreateArgumentNodes(node, command.GetConvertedArguments(context));
				// Devuelve el nodo creado
				return node;
		}

		/// <summary>
		///		Añade los nodos de argumentos
		/// </summary>
		private void CreateArgumentNodes(LogNodeViewModel parent, List<ArgumentModel> arguments)
		{
			LogNodeViewModel folder = new LogNodeViewModel(this, parent, "Parameters", NodeType.Parameter, null);

				// Añade la carpeta
				parent.Children.Add(folder);
				folder.IsBold = true;
				// Añade los parámetros
				if (arguments.Count > 0)
					foreach (ArgumentModel argument in arguments)
						folder.Children.Add(new LogNodeViewModel(this, folder, $"{argument.Parameter.Name} ({argument.Position.ToString()}): {argument.Parameter.Value}", 
																 NodeType.Parameter, argument));
				else
					folder.Children.Add(new LogNodeViewModel(this, folder, $"No command parameters", NodeType.Parameter, null));
		}

		/// <summary>
		///		Obtiene el tipo de nodo
		/// </summary>
		public NodeType GetNodeType(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return NodeType.Context;
			else
				return NodeType.Command;
		}

		/// <summary>
		///		Lanza el evento que indica si se ha creado un nodo
		/// </summary>
		internal void RaiseEvent(NodeType type, string text)
		{
			MainViewModel.WriteTextLog(type, text);
		}

		/// <summary>
		///		Comprueba si se puede ejecutar una acción
		/// </summary>
		protected override bool CanExecuteAction(string action)
		{
			return false;
		}

		/// <summary>
		///		Abre la ventana de propiedades
		/// </summary>
		protected override void OpenProperties()
		{
		}

		/// <summary>
		///		Borra un elemento
		/// </summary>
		protected override void DeleteItem()
		{
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public ExecuteConsoleViewModel MainViewModel { get; }
	}
}
