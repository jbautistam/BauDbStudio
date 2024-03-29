﻿using Bau.Libraries.LibBlogReader.Application.Services.Reader;

namespace Bau.Libraries.LibBlogReader.ViewModel.Controllers.Process;

/// <summary>
///		Proceso de descarga de blogs
/// </summary>
internal class BlogDownloadProcessor : IDisposable
{   
	// Constantes privadas
	private const int MinutesBetweenDownload = 5;
	// Variables privadas
	private System.Timers.Timer? _timer;
	private object _lock = new();

	internal BlogDownloadProcessor(BlogReaderViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
	}

	/// <summary>
	///		Arranca el proceso de descarga
	/// </summary>
	public void Start()
	{
		// Inicializa el temporizador
		_timer = new System.Timers.Timer(TimeSpan.FromMinutes(MinutesBetweenDownload).TotalMilliseconds);
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
				Task.Run(async () => await DownloadBlogsAsync(CancellationToken.None));
				// Y arranca el temporizador de nuevo
				_timer.Start();
			}
		}
	}

	/// <summary>
	///		Descarga los blogs
	/// </summary>
	private async Task DownloadBlogsAsync(CancellationToken cancellationToken)
	{
		await DownloadBlogsAsync(MainViewModel.ConfigurationViewModel.DownloadDisabledBlogs, MainViewModel.BlogManager.File.GetBlogsRecursive(), cancellationToken);
	}

	/// <summary>
	///		Descarga los blogs
	/// </summary>
	public async Task DownloadBlogsAsync(Model.BlogsModelCollection blogs, CancellationToken cancellationToken)
	{
		await DownloadBlogsAsync(MainViewModel.ConfigurationViewModel.DownloadDisabledBlogs, blogs, cancellationToken);
	}

	/// <summary>
	///		Descarga los blogs
	/// </summary>
	private async Task DownloadBlogsAsync(bool includeDisabled, Model.BlogsModelCollection blogs, CancellationToken cancellationToken)
	{
		RssDownload downloader = new(MainViewModel.BlogManager);

			// Descarga los blogs
			if (await downloader.DownloadAsync(includeDisabled, blogs, cancellationToken) > 0)
				MainViewModel.TreeBlogs.Load();
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
				if (_timer is not null)
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
		Dispose(true);
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
