using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.Application.Controllers.Export;

namespace Bau.Libraries.DbStudio.ViewModels.Details.EtlProjects;

/// <summary>
///		Parámetros de un archivo CSV
/// </summary>
public class CsvFileViewModel : BaseObservableObject
{
	// Variables privadas
	private string _separator = default!, _decimalSeparator = default!, _dateFormat = default!, _trueValue = default!, _falseValue = default!;
	private bool _withHeader;
	private ComboViewModel _comboSeparators = default!;

	public CsvFileViewModel(DbStudioViewModel mainViewModel)
	{ 
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		// Crea el combo
		ComboSeparators = new ComboViewModel(this);
		// Inicializa el ViewModel
		InitViewModel();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	private void InitViewModel()
	{
		// Carga el combo de separadores
		LoadComboSeparators(Separator);
		// Inicializa los valores predeterminados
		WithHeader = true;
		Separator = ",";
		DecimalSeparator = ".";
		DateFormat = "yyyy-MM-dd";
		TrueValue = "1";
		FalseValue = "0";
	}

	/// <summary>
	///		Carga el combo de separadores
	/// </summary>
	private void LoadComboSeparators(string separator)
	{
		// Limpia el combo
		ComboSeparators.Items.Clear();
		// Añade el separador si no está en la lista de posibles
		if (!string.IsNullOrEmpty(separator) && separator != "," && separator != ";" && separator != "\t" && separator != " ")
			ComboSeparators.AddItem(0, separator);
		// Añade los separadores básicos
		ComboSeparators.AddItem(1, ",");
		ComboSeparators.AddItem(2, ";");
		ComboSeparators.AddItem(3, "Tab");
		ComboSeparators.AddItem(4, "Space");
		// Selecciona el separador
		if (separator == "\t")
			ComboSeparators.SelectedId = 3;
		else if (separator == " ")
			ComboSeparators.SelectedId = 4;
		else
			ComboSeparators.SelectedText = separator;
	}

	/// <summary>
	///		Obtiene los parámetros del archivo CSV
	/// </summary>
	public CsvFileParameters GetCsvParameters()
	{
		return new CsvFileParameters
						{
							WithHeader = WithHeader,
							Separator = Convert(Separator),
							DateFormat = DateFormat,
							DecimalSeparator = DecimalSeparator,
							TrueValue = TrueValue,
							FalseValue = FalseValue
						};

			// Convierte la cadena del separador
			string Convert(string separator)
			{
				if (string.IsNullOrWhiteSpace(separator))
					return ",";
				else if (separator.Equals("Tab", StringComparison.CurrentCultureIgnoreCase))
					return "\t";
				else if (separator.Equals("Space", StringComparison.CurrentCultureIgnoreCase))
					return " ";
				else
					return separator;
			}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public DbStudioViewModel MainViewModel { get; }

	/// <summary>
	///		Indica si el origen de los datos tiene cabecera
	/// </summary>
	public bool WithHeader
	{
		get { return _withHeader; }
		set { CheckProperty(ref _withHeader, value); }
	}

	/// <summary>
	///		Separador de columnas
	/// </summary>
	public string Separator
	{
		get { return _separator; }
		set { CheckProperty(ref _separator, value); }
	}

	/// <summary>
	///		Separadores predeterminados
	/// </summary>
	public ComboViewModel ComboSeparators
	{
		get { return _comboSeparators ?? default!; }
		set { CheckObject(ref _comboSeparators, value); }
	}

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
}
