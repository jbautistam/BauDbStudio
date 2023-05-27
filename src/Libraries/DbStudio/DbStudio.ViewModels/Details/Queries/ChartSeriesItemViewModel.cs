using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Queries;

/// <summary>
///		Elemento con los datos de una serie
/// </summary>
public class ChartSeriesItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	// Enumerados públicos 
	/// <summary>
	///		Modo de cálculo
	/// </summary>
	public enum Operation
	{
		/// <summary>Sumar</summary>
		Sum,
		/// <summary>Contar</summary>
		Count,
		/// <summary>Contar diferentes</summary>
		CountDistinct,
	}
	// Variables privadas
	private ComboViewModel _comboOperations = default!;

	public ChartSeriesItemViewModel(string text, object tag, bool isBold = false, MvvmColor? foreground = null) : base(text, tag, isBold, foreground)
	{
		IsChecked = true;
		LoadComboOperations();
	}

	/// <summary>
	///		Carga el combo de operaciones
	/// </summary>
	private void LoadComboOperations()
	{
		// Crea el combo
		ComboOperations = new ComboViewModel(this);	
		// Añade los datos
		ComboOperations.AddItem((int) Operation.Sum, "Total");
		ComboOperations.AddItem((int) Operation.Count, "Contar");
		ComboOperations.AddItem((int) Operation.CountDistinct, "Contar distintos");
		// Selecciona el primer elemento
		ComboOperations.SelectedId = (int) Operation.Sum;
	}

	/// <summary>
	///		Obtiene la operación seleccionada
	/// </summary>
	internal Operation GetSelectedOperation()
	{
		return (Operation) (ComboOperations.SelectedId ?? (int) Operation.Sum);
	}

	/// <summary>
	///		ViewModel del combo con los campos para las series
	/// </summary>
	public ComboViewModel ComboOperations
	{
		get { return _comboOperations; }
		set { CheckObject(ref _comboOperations, value); }
	}
}
