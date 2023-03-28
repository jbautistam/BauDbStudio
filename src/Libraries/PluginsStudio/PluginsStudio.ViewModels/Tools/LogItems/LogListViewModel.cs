using System;
using System.Threading;
using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.LogItems
{
	/// <summary>
	///		ViewModel con los datos de log
	/// </summary>
	public class LogListViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlGenericListViewModel<LogListItemViewModel>
	{
		// Constantes privadas
		private const int LogMaximum = 4500;
		private const int LogItemsRemove = 500;
		// Variables privadas
		private SynchronizationContext _contextUi = SynchronizationContext.Current;

		public LogListViewModel(PluginsStudioViewModel mainViewModel)
		{
			MainViewModel = mainViewModel;
		}

		/// <summary>
		///		Escribe la información en el log
		/// </summary>
		public void WriteLog(LogLevel level, string content, Exception? exception)
		{
			object state = new object();

			//? _contexUi mantiene el contexto de sincronización que creó el ViewModel (que debería ser la interface de usuario)
			//? Al generarse el log en un evento interno, no se puede añadir a ObservableCollection sin una
			//? excepción del tipo "Este tipo de CollectionView no admite cambios en su SourceCollection desde un hilo diferente del hilo Dispatcher"
			//? Por eso se tiene que añadir el mensaje de log desde el contexto de sincronización de la UI
			// Limpia los elementos antiguos
			if (Items.Count > LogMaximum)
				while (Items.Count > LogMaximum - LogItemsRemove)
					_contextUi.Send(_ => Items.RemoveAt(Items.Count - 1), state);
			// Añade el mensaje
			_contextUi.Send(_ =>
									{
										// Crea un elemento al principio de la lista y lo selecciona
										Items.Insert(0, new LogListItemViewModel(this, level.ToString(), GetLogMessage(content, exception), DateTime.Now, GetColor(level)));
										SelectedItem = Items[0];
										// Lanza una notificación
										if (level == LogLevel.Error)
											MainViewModel.PluginsStudioController.MainWindowController
													.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Error,
																	  "Error", content);
									},
							state);
		}

		/// <summary>
		///		Obtiene el mensaje de log
		/// </summary>
		private string GetLogMessage(string content, Exception exception)
		{
			string message = content;

			// Añade los datos de la excepción
			if (exception != null)
				message += Environment.NewLine + exception.Message;
			// Devuelve el mensaje
			return message;
		}

		/// <summary>
		///		Obtiene el color dependiendo del tipo
		/// </summary>
		private MvvmColor GetColor(LogLevel level)
		{
			return level switch
			{
				LogLevel.Error or LogLevel.Critical => MvvmColor.Red,
				LogLevel.Debug => MvvmColor.OrangeRed,
				LogLevel.Trace => MvvmColor.Brown,
				_ => MvvmColor.Black
			};
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public PluginsStudioViewModel MainViewModel { get; }
	}
}
