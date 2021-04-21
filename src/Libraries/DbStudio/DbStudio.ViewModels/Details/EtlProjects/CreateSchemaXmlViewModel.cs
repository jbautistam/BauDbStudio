using System;

namespace Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects
{
	/// <summary>
	///		ViewModel de creación de archivos SQL de importación de archivos
	/// </summary>
	public class CreateSchemaXmlViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _outputFileName, _maskFiles;
		private Connections.ComboConnectionsViewModel _comboConnections;

		public CreateSchemaXmlViewModel(SolutionViewModel solutionViewModel)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			ComboConnections = new Connections.ComboConnectionsViewModel(SolutionViewModel, string.Empty);
			// Inicializa el viewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			OutputFileName = System.IO.Path.Combine(SolutionViewModel.MainController.DialogsController.LastPathSelected, "Schema.xml");
			MaskFiles = "Archivos XML (*.xml)|*.xml|Todos los archivos (*.*)|*.*";
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos introducidos
				if (ComboConnections.GetSelectedConnection() == null)
					SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione una conexión");
				else if (string.IsNullOrWhiteSpace(OutputFileName))
					SolutionViewModel.MainController.SystemController.ShowMessage("Seleccione el nombre de archivo");
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
				// Indica que ya no es nuevo y está grabado
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Combo de conexiones
		/// </summary>
		public Connections.ComboConnectionsViewModel ComboConnections
		{
			get { return _comboConnections; }
			set { CheckObject(ref _comboConnections, value); }
		}

		/// <summary>
		///		Nombre del archivo de salida
		/// </summary>
		public string OutputFileName
		{
			get { return _outputFileName; }
			set { CheckProperty(ref _outputFileName, value); }
		}

		/// <summary>
		///		Máscara de archivos
		/// </summary>
		public string MaskFiles
		{
			get { return _maskFiles; }
			set { CheckProperty(ref _maskFiles, value); }
		}
	}
}