using System;

using Bau.Libraries.RestStudio.Models.Rest;

namespace Bau.Libraries.RestStudio.ViewModels.Solution
{
	/// <summary>
	///		ViewModel de <see cref="RestModel"/>
	/// </summary>
	public class RestApiViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _name, _description;

		public RestApiViewModel(RestStudioViewModel mainViewModel, RestModel rest)
		{
			// Inicializa las propiedades
			MainViewModel = mainViewModel;
			if (rest == null)
				Rest = new RestModel();
			else
				Rest = rest;
			// Inicializa el ViewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		private void InitViewModel()
		{
			Name = Rest.Name;
			Description = Rest.Description;
		}

		/// <summary>
		///		Comprueba si los datos son correctos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Compureba los datos
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
				Rest.Name = Name;
				Rest.Description = Description;
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
		///		Modelo de los datos de la API Rest
		/// </summary>
		public RestModel Rest { get; }

		/// <summary>
		///		Nombre
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { CheckProperty(ref _name, value); }
		}

		/// <summary>
		///		Descripción
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { CheckProperty(ref _description, value); }
		}
	}
}
