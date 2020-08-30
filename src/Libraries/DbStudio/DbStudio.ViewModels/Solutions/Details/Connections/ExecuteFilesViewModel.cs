using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.DbScripts.Manager.Connections.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Connections
{
	/// <summary>
	///		ViewModel para ejecución de una serie de archivos
	/// </summary>
	public class ExecuteFilesViewModel : BaseObservableObject, IDetailViewModel
	{
		// Variables privadas
		private string _header;
		private BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ExecuteFilesItemViewModel> _files;

		public ExecuteFilesViewModel(SolutionViewModel solutionViewModel, List<string> files) : base(false)
		{
			// Asigna las propiedades
			SolutionViewModel = solutionViewModel;
			Header = "Ejecución de archivos";
			// Carga la lista de archivos
			LoadListFiles(files);
		}

		/// <summary>
		///		Carga la lista de archivos
		/// </summary>
		private void LoadListFiles(List<string> files)
		{
			// Crea la lista de archivos
			Files = new BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ExecuteFilesItemViewModel>();
			// Asigna los archivos
			files.Sort((first, second) => CompareFiles(first, second));
			foreach (string file in files)
				Files.Add(new ExecuteFilesItemViewModel(file, files));
		}

		/// <summary>
		///		Compara dos archivos
		/// </summary>
		private int CompareFiles(string firstFile, string secondFile)
		{
			int comparePath = System.IO.Path.GetDirectoryName(firstFile).ToUpperInvariant().CompareTo(System.IO.Path.GetDirectoryName(secondFile).ToUpperInvariant());

				if (comparePath == 0)
					return System.IO.Path.GetFileName(firstFile).ToUpperInvariant().CompareTo(System.IO.Path.GetFileName(secondFile).ToUpperInvariant());
				else
					return comparePath;
		}

		/// <summary>
		///		Ejecuta el script
		/// </summary>
		internal async Task ExecuteScriptAsync(ConnectionModel connection, ArgumentListModel arguments, System.Threading.CancellationToken cancellationToken)
		{
			// Marca el estado de los scripts
			InitFileStatus();
			// Ejecuta los archivos
			if (CountEnqueuedFiles() == 0)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione al menos un archivo");
			else if (connection == null)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
			else 
				using (BlockLogModel block = SolutionViewModel.MainViewModel.Manager.Logger.Default.CreateBlock(LogModel.LogType.Info, 
																												"Comienza la ejecución de los archivos"))
				{
					System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
					bool hasError = false;

						// Arranca el temporizador
						stopwatch.Start();
						// Ejecuta los archivos
						foreach (ExecuteFilesItemViewModel file in Files)
							if (file.State == ExecuteFilesItemViewModel.Status.Enqueued)
							{
								if (hasError)
									file.SetStatus(ExecuteFilesItemViewModel.Status.Canceled, "Cancelado por un error anterior");
								else
									hasError = await ExecuteFileAsync(block, file, connection, arguments, cancellationToken);
							}
						// Muestra el tiempo de ejecución
						stopwatch.Stop();
						block.Info($"Tiempo de ejecución: {stopwatch.Elapsed.ToString()}");
						SolutionViewModel.MainViewModel.Manager.Logger.Flush();
				}
		}

		/// <summary>
		///		Ejecuta un archivo
		/// </summary>
		private async Task<bool> ExecuteFileAsync(BlockLogModel block, ExecuteFilesItemViewModel file, ConnectionModel connection, 
												  ArgumentListModel arguments, System.Threading.CancellationToken cancellationToken)
		{
			bool executed = false;
			System.Timers.Timer timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(500).TotalMilliseconds);

				// Asigna el manejador de eventos al temporizador
				timer.Elapsed += (sender, args) => file.SetStatus(ExecuteFilesItemViewModel.Status.Start, "Ejecutando ...");
				timer.Start();
				// Ejecuta el archivo
				try
				{
					string content = LibHelper.Files.HelperFiles.LoadTextFile(System.IO.Path.Combine(file.Path, file.FileName));

						// Ejecuta el contenido del archivo
						if (string.IsNullOrWhiteSpace(content))
							file.SetStatus(ExecuteFilesItemViewModel.Status.Error, "El archivo está vacío");
						else
						{
							// Arranca la ejecución
							file.SetStatus(ExecuteFilesItemViewModel.Status.Start, "Ejecutando ...");
							// Ejecuta la consulta
							await SolutionViewModel.MainViewModel.Manager.ExecuteQueryAsync(connection, content, arguments, 
																							connection.TimeoutExecuteScript, cancellationToken);
							// Detiene la ejecución
							file.SetStatus(ExecuteFilesItemViewModel.Status.End, "Fin de ejecución");
						}
						// Indica que se ha ejecutado correctamente
						executed = true;
				}
				catch (Exception exception)
				{
					block.Error($"Error al ejecutar el archivo '{file.FileName}'");
					block.Error(exception.Message);
					file.SetStatus(ExecuteFilesItemViewModel.Status.Error, $"Error al ejecutar el archivo. {exception.Message}");
				}
				// Detiene el reloj
				timer.Stop();
				timer.Dispose();
				// Devuelve el valor que indica si se ha ejecutado correctamente
				return !executed;
		}

		/// <summary>
		///		Inicializa el estado de los archivos
		/// </summary>
		private void InitFileStatus()
		{
			foreach (ExecuteFilesItemViewModel fileItem in Files)
				if (!fileItem.IsChecked)
					fileItem.SetStatus(ExecuteFilesItemViewModel.Status.Canceled, "Cancelado por el usuario");
				else
					fileItem.SetStatus(ExecuteFilesItemViewModel.Status.Enqueued, "En espera de ejecución");
		}

		/// <summary>
		///		Cuenta el número de archivos encolados
		/// </summary>
		private int CountEnqueuedFiles()
		{
			int number = 0;

				// Cuenta el número
				foreach (ExecuteFilesItemViewModel file in Files)
					if (file.State == ExecuteFilesItemViewModel.Status.Enqueued)
						number++;
				// Devuelve el resultado
				return number;
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void SaveDetails(bool newName)
		{
			IsUpdated = false;
		}

		/// <summary>
		///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return "¿Desea grabar la consulta antes de continuar?";
		}

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header 
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
		}

		/// <summary>
		///		Id de la ficha
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + Guid.NewGuid().ToString(); } 
		}

		/// <summary>
		///		Lista de archivos a ejecutar
		/// </summary>
		public BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ExecuteFilesItemViewModel> Files
		{
			get { return _files; }
			set { CheckObject(ref _files, value); }
		}
	}
}