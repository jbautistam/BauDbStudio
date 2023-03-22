using System;
using System.Collections.Generic;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.JobsProcessor.Application;
using Bau.Libraries.JobsProcessor.Application.EventArguments;
using Bau.Libraries.JobsProcessor.Application.Models;

namespace Bau.Libraries.JobsProcessor.ViewModel.Processor
{
	/// <summary>
	///		ViewModel para ejecutar un archivo de comandos
	/// </summary>
	public class ExecuteConsoleViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
	{
		// Variables privadas
		private string _fileName, _logText;
		private bool _isExecuting;
		private System.Threading.CancellationTokenSource _projectCancellationTokenSource;

		public ExecuteConsoleViewModel(JobsProcessorViewModel mainViewModel, string fileName) : base(false)
		{ 
			// Inicializa las propiedades
			MainViewModel = mainViewModel;
			FileName = fileName;
			// Inicializa los objetos
			TreeLogViewModel = new LogTree.TreeLogViewModel(this);
			// Inicializa los comandos
			ExecuteCommand = new BaseCommand(_ => ExecuteProject(), _ => !IsExecuting)
										.AddListener(this, nameof(IsExecuting));
			CancelCommand = new BaseCommand(_ => CancelProject(), _ => IsExecuting)
										.AddListener(this, nameof(IsExecuting));
		}

		/// <summary>
		///		Interpreta el archivo
		/// </summary>
		public ProjectModel Parse()
		{
			ProjectModel project = null;

				// Carga el archivo
				if (!System.IO.File.Exists(FileName))
					WriteLog(JobProcessEventArgs.StatusType.Error, $"Can't find the file {FileName}");
				else
				{
					// Carga el archivo
					try
					{
						project = new JobsProcessorManager().Load(FileName);
					}
					catch (Exception exception)
					{
						WriteLog(JobProcessEventArgs.StatusType.Error, $"Error when parse file {exception.Message}");
					}
				}
				// Devuelve el proyecto
				return project;
		}

		/// <summary>
		///		Carga los datos del proyecto
		/// </summary>
		private async void ExecuteProject()
		{
			ProjectModel project = Parse();

				if (project is not null)
				{
					JobsProcessorManager manager = new();

						if (Validate(manager, project))
						{
							// Crea el token de cancelación
							_projectCancellationTokenSource = new();
							// Asigna el manejador de eventos
							manager.JobProcessing += (_, args) => AddLog(args);
							// Ejecuta el proceso
							IsExecuting = true;
							await manager.ExecuteAsync(project, _projectCancellationTokenSource.Token);
							IsExecuting = false;
						}
				}
		}

		/// <summary>
		///		Cancela la ejecución de un proyecto
		/// </summary>
		private void CancelProject()
		{
			if (IsExecuting && _projectCancellationTokenSource is not null && _projectCancellationTokenSource.Token.CanBeCanceled)
				_projectCancellationTokenSource.Cancel();
		}

		/// <summary>
		///		Añade los datos de proceso al log
		/// </summary>
		private void AddLog(JobProcessEventArgs args)
		{
			TreeLogViewModel.WriteLog(args);
		}

		/// <summary>
		///		Valida los datos del proyecto
		/// </summary>
		private bool Validate(JobsProcessorManager manager, ProjectModel project)
		{
			List<string> errors = manager.Validate(project);
			
				// y lo valida
				if (errors.Count > 0)
					foreach (string error in errors)
						WriteLog(JobProcessEventArgs.StatusType.Error, error);
				// Devuelve el valor que indica si el proyecto es correcto
				return errors.Count == 0;
		}

		/// <summary>
		///		Escribe un mensaje en el árbol de log
		/// </summary>
		private void WriteLog(JobProcessEventArgs.StatusType status, string message)
		{
			TreeLogViewModel.WriteLog(new JobProcessEventArgs(null, null, status, message));
		}

		/// <summary>
		///		Obtiene el mensaje para grabar y cerrar
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return string.Empty;
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void SaveDetails(bool newName)
		{
			// ... no hace nada, sólo implementa la interface
		}

		/// <summary>
		///		Cierra la ventana de detalles
		/// </summary>
		public void Close()
		{
			System.Diagnostics.Debug.WriteLine("Debería cerrar los procesos");
		}

		/// <summary>
		///		Escribe el texto del log
		/// </summary>
		internal void WriteTextLog(LogTree.TreeLogViewModel.NodeType type, string text)
		{
			LogText += text + Environment.NewLine;
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public JobsProcessorViewModel MainViewModel { get; set; }

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header { get; private set; }

		/// <summary>
		///		Id de la ficha en pantalla
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + FileName; }
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set 
			{ 
				if (CheckProperty(ref _fileName, value))
				{
					if (string.IsNullOrWhiteSpace(_fileName))
						Header = "New filename";
					else
						Header = System.IO.Path.GetFileName(_fileName);
				}
			}
		}

		/// <summary>
		///		ViewModel con el árbol de log
		/// </summary>
		public LogTree.TreeLogViewModel TreeLogViewModel { get; }

		/// <summary>
		///		Texto de log
		/// </summary>
		public string LogText
		{
			get { return _logText;}
			set { CheckProperty(ref _logText, value); }
		}

		/// <summary>
		///		Indica si se está ejecutando algún proceso
		/// </summary>
		public bool IsExecuting
		{
			get { return _isExecuting; }
			set { CheckProperty(ref _isExecuting, value); }
		}

		/// <summary>
		///		Comando para ejecutar los comandos de un archivo
		///	</summary>
		public BaseCommand ExecuteCommand { get; }

		/// <summary>
		///		Comando para cancelar la ejecución de un archivo
		/// </summary>
		public BaseCommand CancelCommand { get; }
	}
}
