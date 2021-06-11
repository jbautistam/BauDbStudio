using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.RestStudio.Models.Rest;

namespace Bau.Libraries.RestStudio.ViewModels.Solution
{
	/// <summary>
	///		ViewModel de <see cref="MethodModel"/>
	/// </summary>
	public class MethodViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _name, _url, _body;
		private ComboViewModel _comboTypes;

		public MethodViewModel(RestStudioViewModel mainViewModel, MethodModel method)
		{
			// Inicializa las propiedades
			MainViewModel = mainViewModel;
			if (method == null)
				Method = new MethodModel();
			else
				Method = method;
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
			Name = Method.Name;
			Url = Method.Url;
			Body = Method.Body;
			ComboTypes.SelectedId = (int) Method.Method;
		}

		/// <summary>
		///		Carga el combo de tipos
		/// </summary>
		private void LoadComboTypes()
		{
			// Crea el combo
			ComboTypes = new ComboViewModel(this);
			// Añade los elementos
			ComboTypes.AddItem(null, "<Seleccione el tipo del método>");
			ComboTypes.AddItem((int) MethodModel.MethodType.Get, "Get");
			ComboTypes.AddItem((int) MethodModel.MethodType.Post, "Post");
			ComboTypes.AddItem((int) MethodModel.MethodType.Put, "Put");
			ComboTypes.AddItem((int) MethodModel.MethodType.Delete, "Delete");
			// Selecciona el primer elemento
			ComboTypes.SelectedItem = ComboTypes.Items[0];
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
				else if (string.IsNullOrWhiteSpace(Url))
					MainViewModel.RestStudioController.MainWindowController.SystemController.ShowMessage("Introduzca la URL");
				else if (ComboTypes.SelectedId == null)
					MainViewModel.RestStudioController.MainWindowController.SystemController.ShowMessage("Seleccione el método");
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
				Method.Name = Name;
				Method.Url = Url;
				Method.Method = (MethodModel.MethodType) (ComboTypes.SelectedId ?? (int) MethodModel.MethodType.Get);
				Method.Body = Body;
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
		///		Modelo de los datos del método de la API Rest
		/// </summary>
		public MethodModel Method { get; }

		/// <summary>
		///		Nombre
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { CheckProperty(ref _name, value); }
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
		///		Cuerpo
		/// </summary>
		public string Body
		{
			get { return _body; }
			set { CheckProperty(ref _body, value); }
		}
	}
}
