using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Web;

/// <summary>
///		ViewModel de web
/// </summary>
public class WebViewModel : BaseObservableObject, Base.Interfaces.IDetailViewModel
{
	// Eventos públicos
	public event EventHandler? Closed;
	public event EventHandler? RefreshPage;
	// Variables privadas
	private string _header = string.Empty, _url = string.Empty, _html = string.Empty;

	public WebViewModel(PluginsStudioViewModel mainViewModel, string url) : base(false)
	{
		// Asigna las propiedades
		Url = url;
		Header = GetTitle(url);
		MainViewModel = mainViewModel;
		// Asigna los comandos
		RefreshCommand = new BaseCommand(_ => RefreshBrowser(), _ => CanUpdatePage());
		OpenExplorerCommand = new BaseCommand(_ => OpenExplorer(), _ => CanUpdatePage());
	}

	/// <summary>
	///		Obtiene el título a partir de la URL
	/// </summary>
	private string GetTitle(string url)
	{
		if (Uri.TryCreate(url, UriKind.Absolute, out Uri? converted))
		{
			const int MaxLength = 20;
			string [] paths = converted.PathAndQuery.Split('/');
			string result = string.Empty;

				// Quita los directorios
				for (int index = paths.Length - 1; index >= 0; index--)
					if (!string.IsNullOrWhiteSpace(paths[index]) && string.IsNullOrWhiteSpace(result))
						result = paths[index];
				// Si no se ha obtenido nada, recoge el dato inicial
				if (string.IsNullOrWhiteSpace(result))
					result = converted.PathAndQuery;
				// Sólo los primeros caracteres
				if (result.Length > MaxLength)
					result = result.Substring(0, MaxLength);
				// Devuelve los datos
				return result;
		}
		else
			return url;
	}

	/// <summary>
	///		Comprueba si se puede actualizar una página
	/// </summary>
	private bool CanUpdatePage() => !string.IsNullOrWhiteSpace(Url);

	/// <summary>
	///		Actualiza la página en el navegador
	/// </summary>
	private void RefreshBrowser()
	{
		RefreshPage?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Abre otra ventana del navegador sobre una URL
	/// </summary>
	public void OpenBrowser(string url)
	{
		MainViewModel.MainController.HostPluginsController.OpenWebBrowser(url);
	}

	/// <summary>
	///		Abre el explorador externo
	/// </summary>
	private void OpenExplorer()
	{
		if (Uri.TryCreate(Url, UriKind.Absolute, out Uri? url))
		{
			MainViewModel.MainController.MainWindowController.OpenWindowsWebBrowser(url);
			MainViewModel.MainController.MainWindowController.CloseWindow(TabId);
		}
	}

	/// <summary>
	///		Ejecuta un comando
	/// </summary>
	public void Execute(Base.Models.Commands.ExternalCommand externalCommand)
	{
		System.Diagnostics.Debug.WriteLine($"Execute command {externalCommand.Type.ToString()} at {Header}");
	}

	/// <summary>
	///		Obtiene el mensaje de grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage() => "¿Desea grabar esta página?";

	/// <summary>
	///		Graba los detalles
	/// </summary>
	public void SaveDetails(bool newName)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///		Cierra el viewmodel
	/// </summary>
	public void Close()
	{
		Closed?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		ViewModel de la aplicación principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Url
	/// </summary>
	public string Url
	{
		get { return _url; }
		set 
		{ 
			if (CheckProperty(ref _url, value) && !string.IsNullOrWhiteSpace(value) &&
					!value.Equals("about:blank", StringComparison.CurrentCultureIgnoreCase) &&
					!IsLastUrl(value))
				Urls.Add(value);

			// Comprueba si una URL es la última
			bool IsLastUrl(string url) => Urls.Count > 0 && Urls[Urls.Count - 1].Equals(url, StringComparison.CurrentCultureIgnoreCase);
		}
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
	///		Html
	/// </summary>
	public string Html
	{
		get { return _html; }
		set { CheckProperty(ref _html, value); }
	}

	/// <summary>
	///		Id de la ficha
	/// </summary>
	public string TabId => GetType().ToString() + "_" + Guid.NewGuid().ToString();

	/// <summary>
	///		Urls en la lista
	/// </summary>
	public ObservableCollection<string> Urls { get; } = new();

	/// <summary>
	///		Comando para actualizar
	/// </summary>
	public BaseCommand RefreshCommand { get; }

	/// <summary>
	///		Comando para abrir el explorador
	/// </summary>
	public BaseCommand OpenExplorerCommand { get; }
}
