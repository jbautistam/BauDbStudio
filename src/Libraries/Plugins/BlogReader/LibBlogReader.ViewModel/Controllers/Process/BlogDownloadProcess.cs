using System;

using Bau.Libraries.LibBlogReader.Application.Services.Reader;
using Bau.Libraries.LibBlogReader.Application.Services.EventArguments;

namespace Bau.Libraries.LibBlogReader.ViewModel.Controllers.Process
{
	/// <summary>
	///		Proceso de descarga de blogs
	/// </summary>
	internal class BlogDownloadProcess : IDisposable
	{   
		// Variables privadas
		private RssDownload _downloader = null;
		private System.Timers.Timer _timer;
		private object _lock = new();

		internal BlogDownloadProcess(BlogReaderViewModel mainViewModel)
		{
			MainViewModel = mainViewModel;
		}

		/// <summary>
		///		Arranca el proceso de descarga
		/// </summary>
		public void Start()
		{
			// Inicializa el temporizador
			_timer = new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
			_timer.Enabled = true;
			// Asigna el manejador de eventos
			_timer.Elapsed += (sender, args) => Execute();
			// Arranca el temporizador
			_timer.Start();
		}

		/// <summary>
		///		Ejecuta la descarga de blogs
		/// </summary>
		private void Execute()
		{
			lock (_lock)
			{
				if (_timer != null && (DateTime.Now - LastDownload).TotalMinutes > MainViewModel.ConfigurationViewModel.MinutesBetweenDownload)
				{
					// Detiene el temporizador
					_timer.Stop();
					// Guarda la fecha de última descarga
					LastDownload = DateTime.Now;
					// Descarga los datos
					Downloader.Download(false);
					// Y arranca el temporizador de nuevo
					_timer.Start();
				}
			}
		}

		/// <summary>
		///		Trata el proceso de descarga
		/// </summary>
		private void TreatDownloadProcess(DownloadEventArgs.ActionType type, string description)
		{
			switch (type)
			{
				case DownloadEventArgs.ActionType.StartDownload:
						MainViewModel.ViewsController.Logger.Default.LogItems.Info("Start blogs download");
					break;
				case DownloadEventArgs.ActionType.StartDownloadBlog:
				case DownloadEventArgs.ActionType.EndDownloadBlog:
						MainViewModel.ViewsController.Logger.Default.LogItems.Info(description);
					break;
				case DownloadEventArgs.ActionType.ErrorDonwloadBlog:
						MainViewModel.ViewsController.Logger.Default.LogItems.Error(description);
					break;
				case DownloadEventArgs.ActionType.EndDownload:
						MainViewModel.ViewsController.Logger.Default.LogItems.Info("End blogs download");
						MainViewModel.TreeBlogs.Load();
					break;
			}
			MainViewModel.ViewsController.Logger.Flush();
		}

		/// <summary>
		///		Proceso de descarga
		/// </summary>
		private RssDownload Downloader
		{
			get
			{ 
				// Crea el objeto si no existía
				if (_downloader == null)
				{ 
					// Crea el objeto
					_downloader = new RssDownload(MainViewModel.BlogManager);
					// Asigna los manejadores de eventos
					_downloader.Process += (sender, evntArgs) => TreatDownloadProcess(evntArgs.Action, evntArgs.Description);
				}
				// Devuelve el objeto de descarga
				return _downloader;
			}
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				// Libera los recursos
				if (disposing)
				{
					if (_timer != null)
					{
						_timer.Stop();
						_timer = null;
					}
				}
				// Indica que se han liberado los recursos
				Disposed = true;
			}
		}

		/// <summary>
		///		Libera la memoria
		/// </summary>
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public BlogReaderViewModel MainViewModel { get; }

		/// <summary>
		///		Fecha de última descarga
		/// </summary>
		public DateTime LastDownload { get; private set; } = DateTime.Now.AddDays(-1);

		/// <summary>
		///		Indica si se ha liberado la memoria
		/// </summary>
		public bool Disposed { get; private set; }
	}
}
