using System;
using System.Threading;

using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Log
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
			MainViewModel.MainStudioController.MainWindowController.Logger.Logged += (sender, args) => WriteLog(args.Item);
		}

		/// <summary>
		///		Escribe la información en el log
		/// </summary>
		private void WriteLog(LogModel item)
		{
			if (item.Type != LogModel.LogType.Progress)
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
					_contextUi.Send(_ => {
											// Crea un elemento al principio de la lista y lo selecciona
											Items.Insert(0, new LogListItemViewModel(this, item.Type.ToString(), GetLogMessage(item), item.CreatedAt, GetColor(item.Type)));
											SelectedItem = Items[0];
											// Lanza una notificación
											if (item.Type == LogModel.LogType.Error || item.Type == LogModel.LogType.AssertError)
												MainViewModel.MainStudioController.MainWindowController
														.ShowNotification(BauMvvm.ViewModels.Controllers.SystemControllerEnums.NotificationType.Error,
																		  "Error", item.Message);
										  },
									state);
			}
		}

		/// <summary>
		///		Obtiene el mensaje de log
		/// </summary>
		private string GetLogMessage(LogModel item)
		{
			string message = item.Message;

				// Añade los datos de la excepción
				if (item.Exception != null)
					message += Environment.NewLine + item.Exception.Message;
				// Añade los datos de progreso
				if (item.Type == LogModel.LogType.Progress)
					message += $" - {item.ActualProgress:#,##0}";
				// Devuelve el mensaje
				return message;
		}

		/// <summary>
		///		Obtiene el color dependiendo del tipo
		/// </summary>
		private MvvmColor GetColor(LogModel.LogType type)
		{
			switch (type)
			{
				case LogModel.LogType.Error:
					return MvvmColor.Red;
				case LogModel.LogType.Warning:
					return MvvmColor.Navy;
				default:
					return MvvmColor.Black;
			}
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public PluginsStudioViewModel MainViewModel { get; }
	}
}
