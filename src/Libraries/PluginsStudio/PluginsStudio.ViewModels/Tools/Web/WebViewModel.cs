using System;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools.Web
{
	/// <summary>
	///		ViewModel de web
	/// </summary>
	public class WebViewModel : BauMvvm.ViewModels.BaseObservableObject, Base.Interfaces.IDetailViewModel
	{
		// Variables privadas
		private string _url;

		public WebViewModel(PluginsStudioViewModel mainViewModel, string url) : base(false)
		{
			Url = url;
			Header = url;
			MainViewModel = mainViewModel;
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
