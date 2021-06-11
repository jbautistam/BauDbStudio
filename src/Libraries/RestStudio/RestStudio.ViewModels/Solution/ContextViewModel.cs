using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.RestStudio.Models.Rest;

namespace Bau.Libraries.RestStudio.ViewModels.Solution
{
	/// <summary>
	///		ViewModel de <see cref="ContextModel"/>
	/// </summary>
	public class ContextViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _name, _url;
		private int _timeout;
		private string _urlAuthority, _user, _password, _scope;
		private ComboViewModel _comboTypes;

		public ContextViewModel(RestStudioViewModel mainViewModel, ContextModel context)
		{
			// Inicializa las propiedades
			MainViewModel = mainViewModel;
			if (context == null)
				Context = new ContextModel();
			else
				Context = context;
			// Inicializa el ViewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		private void InitViewModel()
		{
			// Carga el combo de tipos
			LoadComboTypes();
			// Asigna las propiedades
			Name = Context.Name;
			Url = Context.Url;
			Timeout = (int) Context.Timeout.TotalMinutes;
			ComboTypes.SelectedId = (int) Context.Credentials.Authentication;
			UrlAuthority = Context.Credentials.UrlAuthority;
			User = Context.Credentials.User;
			Password = Context.Credentials.Password;
			Scope = Context.Credentials.Scope;
		}

		/// <summary>
		///		Carga el combo de tipos
		/// </summary>
		private void LoadComboTypes()
		{
			// Crea el combo
			ComboTypes = new ComboViewModel(this);
			// Añade los elementos
			ComboTypes.AddItem(null, "<Seleccione un tipo de autenticación>");
			ComboTypes.AddItem((int) CredentialsModel.AuthenticationType.Noauthentication, "Sin autentificación");
			ComboTypes.AddItem((int) CredentialsModel.AuthenticationType.Basic, "Básica");
			ComboTypes.AddItem((int) CredentialsModel.AuthenticationType.Jwt, "JWT");
			// Selecciona el primer elemento
			ComboTypes.SelectedItem = ComboTypes.Items[0];
		}

		/// <summary>
		///		Comprueba si los datos son correctos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos
				if (string.IsNullOrWhiteSpace(Name))
					MainViewModel.RestStudioController.MainWindowController.SystemController.ShowMessage("Introduzca el nombre");
				else
					validated = true;
				// Devuelve el valor que indica si los datos son correctos
				return validated;
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		protected override void Save()
		{
			if (ValidateData())
			{
				// Actualiza los datos del modelo
				Context.Name = Name;
				Context.Url = Url;
				Context.Timeout = TimeSpan.FromMinutes(Timeout);
				Context.Credentials.Authentication = (CredentialsModel.AuthenticationType) (ComboTypes.SelectedId ?? 0);
				Context.Credentials.UrlAuthority = UrlAuthority;
				Context.Credentials.User = User;
				Context.Credentials.Password = Password;
				Context.Credentials.Scope = Scope;
				// Indica que ya no es nuevo y está grabado
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public RestStudioViewModel MainViewModel { get; }

		/// <summary>
		///		Modelo de los datos del contexto de la API Rest
		/// </summary>
		public ContextModel Context { get; }

		/// <summary>
		///		Nombre
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { CheckProperty(ref _name, value); }
		}

		/// <summary>
		///		Timeout en minutos
		/// </summary>
		public int Timeout
		{
			get { return _timeout; }
			set { CheckProperty(ref _timeout, value); }
		}

		/// <summary>
		///		Url
		/// </summary>
		public string Url
		{
			get { return _url; }
			set { CheckProperty(ref _url, value); }
		}

		/// <summary>
		///		Tipos de conexión
		/// </summary>
		public ComboViewModel ComboTypes
		{
			get { return _comboTypes; }
			set { CheckObject(ref _comboTypes, value); }
		}

		/// <summary>
		///		Url de la autoridad
		/// </summary>
		public string UrlAuthority
		{
			get { return _urlAuthority; }
			set { CheckProperty(ref _urlAuthority, value); }
		}

		/// <summary>
		///		Usuario
		/// </summary>
		public string User
		{
			get { return _user; }
			set { CheckProperty(ref _user, value); }
		}

		/// <summary>
		///		Contraseña
		/// </summary>
		public string Password
		{
			get { return _password; }
			set { CheckProperty(ref _password, value); }
		}

		/// <summary>
		///		Ambito
		/// </summary>
		public string Scope
		{
			get { return _scope; }
			set { CheckProperty(ref _scope, value); }
		}
	}
}
