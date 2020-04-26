using System;

using Bau.Libraries.LibCsvFiles.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files
{
	/// <summary>
	///		ViewModel con las propieddes de un archivo CSV
	/// </summary>
	public class CsvFilePropertiesViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _dateFormat, _decimalSeparator, _thousandsSeparator, _trueValue, _falseValue, _separator;
		private bool _skipFirstLine;

		public CsvFilePropertiesViewModel(SolutionViewModel solutionViewModel, FileModel fileParameters)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			FileParameters = fileParameters;
			// Inicializa el viewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			DateFormat = FileParameters.DateFormat;
			DecimalSeparator = FileParameters.DecimalSeparator;
			ThousandsSeparator = FileParameters.ThousandsSeparator;
			TrueValue = FileParameters.TrueValue;
			FalseValue = FileParameters.FalseValue;
			Separator = FileParameters.Separator;
			SkipFirstLine = FileParameters.WithHeader;
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
				if (string.IsNullOrWhiteSpace(Separator))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el separador");
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
				// Asigna las propiedades seleccionadas
				FileParameters.DateFormat = DateFormat;
				FileParameters.DecimalSeparator = DecimalSeparator;
				FileParameters.ThousandsSeparator = ThousandsSeparator;
				FileParameters.TrueValue = TrueValue;
				FileParameters.FalseValue = FalseValue;
				FileParameters.Separator = Separator;
				FileParameters.WithHeader = SkipFirstLine;
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
		///		Parámetros del archivo
		/// </summary>
		public FileModel FileParameters { get; }

		/// <summary>
		///		Formato de fecha
		/// </summary>
		public string DateFormat
		{
			get { return _dateFormat; }
			set { CheckProperty(ref _dateFormat, value); }
		}

		/// <summary>
		///		Separador de decimales
		/// </summary>
		public string DecimalSeparator
		{
			get { return _decimalSeparator; }
			set { CheckProperty(ref _decimalSeparator, value); }
		}

		/// <summary>
		///		Separador de miles
		/// </summary>
		public string ThousandsSeparator
		{
			get { return _thousandsSeparator; }
			set { CheckProperty(ref _thousandsSeparator, value); }
		}

		/// <summary>
		///		Cadena para los valores verdaderos
		/// </summary>
		public string TrueValue
		{
			get { return _trueValue; }
			set { CheckProperty(ref _trueValue, value); }
		}

		/// <summary>
		///		Cadena para los valores falsos
		/// </summary>
		public string FalseValue
		{
			get { return _falseValue; }
			set { CheckProperty(ref _falseValue, value); }
		}

		/// <summary>
		///		Separador de campos
		/// </summary>
		public string Separator
		{
			get { return _separator; }
			set { CheckProperty(ref _separator, value); }
		}

		/// <summary>
		///		Indica si se debe saltar la primera línea de cabecera
		/// </summary>
		public bool SkipFirstLine 
		{
			get { return _skipFirstLine; }
			set { CheckProperty(ref _skipFirstLine, value); }
		}
	}
}