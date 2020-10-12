using System;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.DbScripts.Manager.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel para un archivo
	/// </summary>
	public class FileViewModel : BaseObservableObject, IDetailViewModel
	{
		// Eventos públicos
		public event EventHandler<Controllers.EventArguments.EditorGoToLineEventArgs> GoToLineRequired;
		public event EventHandler<Controllers.EventArguments.EditorSelectedTextRequiredEventArgs> SelectedTextRequired;
		// Variables privadas
		private string _header, _fileName, _content;
		private System.Text.Encoding _fileEncoding;
		private bool _fileWithBom;

		public FileViewModel(SolutionViewModel solutionViewModel, string fileName)
		{
			SolutionViewModel = solutionViewModel;
			FileName = fileName;
		}

		/// <summary>
		///		Carga el texto del archivo
		/// </summary>
		public void Load()
		{
			// Carga el archivo
			if (!string.IsNullOrWhiteSpace(FileName) && System.IO.File.Exists(FileName))
			{
				// Obtiene la codificación del archivo (para grabarlo después con la misma codificación)
				FileEncoding = LibHelper.Files.HelperFiles.GetFileEncoding(FileName);
				if (FileEncoding == null)
				{
					FileEncoding = System.Text.Encoding.UTF8;
					FileWithBom = false;
				}
				else
					FileWithBom = true;
				// Carga el archivo
				Content = LibHelper.Files.HelperFiles.LoadTextFile(FileName);
			}
			// Añade el archivo a los últimos archivos abiertos
			SolutionViewModel.MainViewModel.LastFilesViewModel.Add(FileName);
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void SaveDetails(bool newName)
		{
			// Graba el archivo
			if (string.IsNullOrWhiteSpace(FileName) || newName)
			{
				string newFileName = SolutionViewModel.MainViewModel.OpenDialogSave(FileName, "Script SQL (*.sql)|*.sql|Todos los archivos (*.*)|*.*", ".sql");

					// Cambia el nombre de archivo si es necesario
					if (!string.IsNullOrWhiteSpace(newFileName))
						FileName = newFileName;
			}
			// Graba el archivo
			if (!string.IsNullOrWhiteSpace(FileName))
			{
				// Graba el archivo
				if (FileWithBom)
					LibHelper.Files.HelperFiles.SaveTextFile(FileName, Content, FileEncoding);
				else
					LibHelper.Files.HelperFiles.SaveTextFileWithoutBom(FileName, Content);
				// Actualiza el árbol
				SolutionViewModel.TreeFoldersViewModel.Load();
				// Añade el archivo a los últimos archivos abiertos
				SolutionViewModel.MainViewModel.LastFilesViewModel.Add(FileName);
				// Indica que no ha habido modificaciones
				IsUpdated = false;
			}
		}

		/// <summary>
		///		Ejecuta el script
		/// </summary>
		internal async Task ExecuteSqlScriptAsync(ConnectionModel connection, ArgumentListModel arguments, System.Threading.CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(Content))
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca una consulta para ejecutar");
			else if (connection == null)
				SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
			else 
				using (LibLogger.Models.Log.BlockLogModel block = SolutionViewModel.MainViewModel.Manager.Logger.Default.CreateBlock(LibLogger.Models.Log.LogModel.LogType.Info,
																																	 $"Comienza la ejecución de la consulta"))
				{
					string selectedText = GetEditorSelectedText();

						// Si no hay nada seleccionado, se ejecuta todo el contenido
						if (string.IsNullOrWhiteSpace(selectedText))
							selectedText = Content;
						// Ejecuta la consulta
						if (FileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
							await SolutionViewModel.MainViewModel.Manager.ExecuteQueryAsync(connection, selectedText, arguments, 
																							connection.TimeoutExecuteScript, cancellationToken);
						else if (FileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
							await SolutionViewModel.MainViewModel.Manager.ExecuteInterpretedQueryAsync(connection, selectedText, arguments, cancellationToken);
						else
							block.Error("No se reconoce el tipo de archivo coo SQL");
						// Muestra el tiempo de ejecución
						block.Info($"Tiempo de ejecución: {SolutionViewModel.ConnectionExecutionViewModel.ExecutionTime}");
				}
		}

		/// <summary>
		///		Ejecuta el script XML
		/// </summary>
		internal async Task ExecuteXmlScriptAsync(string contextFileName, System.Threading.CancellationToken cancellationToken)
		{
			ScriptsManager.JobXmlProjectManager manager = new ScriptsManager.JobXmlProjectManager(SolutionViewModel.MainViewModel.Manager.Logger);

				// Ejecuta el script XML
				await manager.ExecuteAsync(FileName, contextFileName, cancellationToken);
				// Libera el log
				SolutionViewModel.MainViewModel.Manager.Logger.Flush();
		}

		/// <summary>
		///		Lanza un evento para solicitar el texto seleccionado al editor
		/// </summary>
		private string GetEditorSelectedText()
		{
			Controllers.EventArguments.EditorSelectedTextRequiredEventArgs eventArgs = new Controllers.EventArguments.EditorSelectedTextRequiredEventArgs();

				// Lanza el evento
				SelectedTextRequired?.Invoke(this, eventArgs);
				// Recupera el texto
				return eventArgs.SelectedText;
		}

		/// <summary>
		///		Lanza un evento para colocar el editor en una línea
		/// </summary>
		internal void GoToLine(string textSelected, int line)
		{
			GoToLineRequired?.Invoke(this, new Controllers.EventArguments.EditorGoToLineEventArgs(textSelected, line));
		}

		/// <summary>
		///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			if (string.IsNullOrWhiteSpace(FileName))
				return "¿Desea grabar el archivo antes de continuar?";
			else
				return $"¿Desea grabar el archivo '{System.IO.Path.GetFileName(FileName)}' antes de continuar?";
		}

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Id de la ficha
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + FileName; } 
		}

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
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
					if (!string.IsNullOrWhiteSpace(value))
						Header = System.IO.Path.GetFileName(value);
					else
						Header = "Archivo";
				}
			}
		}

		/// <summary>
		///		Codificación del archivo
		/// </summary>
		public System.Text.Encoding FileEncoding
		{
			get { return _fileEncoding; }
			set { CheckObject(ref _fileEncoding, value); }
		}

		/// <summary>
		///		Indica si el archivo tiene una cabecera BOM
		/// </summary>
		public bool FileWithBom
		{
			get { return _fileWithBom; }
			set { CheckProperty(ref _fileWithBom, value); }
		}

		/// <summary>
		///		Contenido del archivo
		/// </summary>
		public string Content
		{
			get { return _content; }
			set { CheckProperty(ref _content, value); }
		}
	}
}
