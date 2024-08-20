using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.StructuredFilesStudio.ViewModels.Details.Files;

/// <summary>
///		Lista de <see cref="CsvFileListItemColumnViewModel"/>
/// </summary>
public class CsvFileListColumnsViewModel : BaseObservableObject 
{
	// Variables privadas
	private ObservableCollection<CsvFileListItemColumnViewModel> _items = default!;
	private CsvFileListItemColumnViewModel _selectedItem = default!;

	public CsvFileListColumnsViewModel(BaseFileViewModel fileViewModel)
	{
		// Asigna las propiedades
		FileViewModel = fileViewModel;
		// Inicializa el viewModel
		InitViewModel();
		// Inicializa los comandos
		ResetFieldsCommand = new BaseCommand(_ => InitViewModel());
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	private void InitViewModel()
	{
		// Crea los elementos de la lista
		Items = new ObservableCollection<CsvFileListItemColumnViewModel>();
		// Añade las columnas
		if (FileViewModel.DataResults is not null)
			foreach (System.Data.DataColumn column in FileViewModel.DataResults.Columns)
				Items.Add(new CsvFileListItemColumnViewModel(this, column.ColumnName));
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	internal bool ValidateData()
	{
		// Comprueba las columnas
		foreach (CsvFileListItemColumnViewModel column in Items)
			if (!column.ValidataData())
				return false;
		// Si ha llegado hasta aquí es porque todo ha ido bien
		return true;
	}

	/// <summary>
	///		Obtiene las columnas
	/// </summary>
	internal List<(string column, BaseFileViewModel.FieldType type)> GetColumns()
	{
		List<(string column, BaseFileViewModel.FieldType type)> columns = new List<(string column, BaseFileViewModel.FieldType type)>();

			// Añade las columnas
			foreach (CsvFileListItemColumnViewModel column in Items)
				columns.Add((column.ColumnId, column.GetSelectedType()));
			// Devuelve las columnas
			return columns;
	}

	/// <summary>
	///		ViewModel del Archivo
	/// </summary>
	public BaseFileViewModel FileViewModel { get; }

	/// <summary>
	///		Elementos de la lista
	/// </summary>
	public ObservableCollection<CsvFileListItemColumnViewModel> Items
	{
		get { return _items; }
		set { CheckObject(ref _items, value); }
	}

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public CsvFileListItemColumnViewModel SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Comando para reiniciar los campos
	/// </summary>
	public BaseCommand ResetFieldsCommand { get; }
}