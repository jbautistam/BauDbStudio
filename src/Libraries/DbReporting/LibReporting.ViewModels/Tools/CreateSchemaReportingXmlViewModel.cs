using System;

namespace Bau.Libraries.LibReporting.ViewModels.Tools
{
	/// <summary>
	///		ViewModel de creación de archivos de esquema de reporting a partir de un archivo de esquema
	/// </summary>
	public class CreateSchemaReportingXmlViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _name, _schemaFileName, _outputFileName, _maskFiles;

		public CreateSchemaReportingXmlViewModel(SolutionViewModel solutionViewModel)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			// Inicializa el viewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			OutputFileName = System.IO.Path.Combine(SolutionViewModel.MainViewModel.LastPathSelected, "Schema.xml");
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
				if (string.IsNullOrWhiteSpace(Name))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del almacén de datos");
				else if (string.IsNullOrWhiteSpace(SchemaFileName) || !System.IO.File.Exists(SchemaFileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione el archivo de esquema");
				else if (string.IsNullOrWhiteSpace(OutputFileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione el nombre de archivo");
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
				// Guarda el directorio seleccionado
				SolutionViewModel.MainViewModel.LastPathSelected = System.IO.Path.GetDirectoryName(OutputFileName);
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
		///		Identificador del esquema en el DataWarehouse
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { CheckProperty(ref _name, value); }
		}

		/// <summary>
		///		Nombre del archivo de esquema
		/// </summary>
		public string SchemaFileName
		{
			get { return _schemaFileName; }
			set { CheckProperty(ref _schemaFileName, value); }
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