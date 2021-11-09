using System;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Web
{
	/// <summary>
	///		ViewModel de web
	/// </summary>
	public class WebViewModel : BauMvvm.ViewModels.BaseObservableObject, Base.Interfaces.IDetailViewModel
	{
		// Eventos públicos
		public event EventHandler Closed;
		// Variables privadas
		private string _url;

		public WebViewModel(PluginsStudioViewModel mainViewModel, string url) : base(false)
		{
			Url = url;
			Header = GetTitle(url);
			MainViewModel = mainViewModel;
		}

		/// <summary>
		///		Obtiene el título a partir de la URL
		/// </summary>
		private string GetTitle(string url)
		{
			if (Uri.TryCreate(url, UriKind.Absolute, out Uri converted))
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
		///		Obtiene el mensaje de grabar y cerrar
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return "¿Desea grabar esta página?";
		}

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
			set { CheckProperty(ref _url, value); }
		}

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header { get; }

		/// <summary>
		///		Id de la ficha
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + Guid.NewGuid().ToString(); }
		}
	}
}
