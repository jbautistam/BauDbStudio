using System;

using Bau.Libraries.LibCsvFiles.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files.Structured
{
	/// <summary>
	///		ViewModel con las propiedades de un archivo CSV
	/// </summary>
	public class CsvFilePropertiesViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _dateFormat, _decimalSeparator, _thousandsSeparator, _trueValue, _falseValue, _separator;
		private bool _skipFirstLine;
		private CsvFileListColumnsViewModel _listColumnsViewModel;

		public CsvFilePropertiesViewModel(SolutionViewModel solutionViewModel, CsvFileViewModel fileViewModel)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			FileViewModel = fileViewModel;
			// Inicializa el viewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			DateFormat = FileViewModel.FileParameters.DateFormat;
			DecimalSeparator = FileViewModel.FileParameters.DecimalSeparator;
			ThousandsSeparator = FileViewModel.FileParameters.ThousandsSeparator;
			TrueValue = FileViewModel.FileParameters.TrueValue;
			FalseValue = FileViewModel.FileParameters.FalseValue;
			Separator = FileViewModel.FileParameters.Separator;
			SkipFirstLine = FileViewModel.FileParameters.WithHeader;
			// Lista de columnas
			ListColumnsViewModel = new CsvFileListColumnsViewModel(FileViewModel);
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
				FileViewModel.FileParameters.DateFormat = DateFormat;
				FileViewModel.FileParameters.DecimalSeparator = DecimalSeparator;
				FileViewModel.FileParameters.ThousandsSeparator = ThousandsSeparator;
				FileViewModel.FileParameters.TrueValue = TrueValue;
				FileViewModel.FileParameters.FalseValue = FalseValue;
				FileViewModel.FileParameters.Separator = Separator;
				FileViewModel.FileParameters.WithHeader = SkipFirstLine;
				// Asigna las columnas
				FileViewModel.FileColumns.Clear();
				foreach (CsvFileListItemColumnViewModel columnViewModel in ListColumnsViewModel.Items)
					FileViewModel.FileColumns.Add(new ColumnModel
															{
																Name = columnViewModel.ColumnId,
																Type = ConvertColumnType(columnViewModel.GetSelectedType())
															}
												 );
				// Indica que ya no es nuevo y está grabado
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		Convierte el tipo de columna
		/// </summary>
		private ColumnModel.ColumnType ConvertColumnType(BaseFileViewModel.FieldType fieldType)
		{
			switch (fieldType)
			{
				case BaseFileViewModel.FieldType.Boolean:
					return ColumnModel.ColumnType.Boolean;
				case BaseFileViewModel.FieldType.Integer:
					return ColumnModel.ColumnType.Integer;
				case BaseFileViewModel.FieldType.Decimal:
					return ColumnModel.ColumnType.Decimal;
				case BaseFileViewModel.FieldType.Date:
					return ColumnModel.ColumnType.DateTime;
				default:
					return ColumnModel.ColumnType.String;
			}
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		ViewModel del archivo
		/// </summary>
		public CsvFileViewModel FileViewModel { get; }

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

		/// <summary>
		///		Lista de columnas
		/// </summary>
		public CsvFileListColumnsViewModel ListColumnsViewModel
		{
			get { return _listColumnsViewModel; }
			set { CheckObject(ref _listColumnsViewModel, value); }
		}
	}
}