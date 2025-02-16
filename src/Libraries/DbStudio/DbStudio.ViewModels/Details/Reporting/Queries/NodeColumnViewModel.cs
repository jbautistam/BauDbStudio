using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.Trees;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries;

/// <summary>
///		ViewModel de un nodo de columna
/// </summary>
public class NodeColumnViewModel : PluginNodeViewModel
{
	// Tipo de nodo
	public enum NodeColumnType
	{
		/// <summary>Raíz de dimensiones</summary>
		DimensionsRoot,
		/// <summary>Nombre de dimensión</summary>
		Dimension,
		/// <summary>Columna de dimensión</summary>
		DimensionColumn,
		/// <summary>Raíz de expresiones</summary>
		ExpressionsRoot,
		/// <summary>Nombre de expresión</summary>
		Expression,
		/// <summary>Columna de expresión</summary>
		ExpressionColumn,
		/// <summary>Campo de expresión de un informe avanzado</summary>
		ExpressionField,
		/// <summary>Raíz de los orígenes de datos</summary>
		DataSourcesRoot,
		/// <summary>Nombre del origen de datos</summary>
		DataSource,
		/// <summary>Columnas del origen de datos</summary>
		DataSourceColumn,
		/// <summary>Raíz de parámetros</summary>
		ParametersRoot,
		/// <summary>Campo de parámetro</summary>
		ParameterField
	}
	// Variables privadas
	private bool _canSelect, _canSort, _canFilterWhere, _canAggregate, _canFilterHaving, _hasFiltersColumn, _hasFiltersHaving;
	private int _sortIndex;
	private ColumnRequestModel.SortOrder _sortOrder;
	private ComboViewModel _comboAggregationTypes = default!;
	private ListReportColumnFilterViewModel _filterWhere = default!, _filterHaving = default!;

	public NodeColumnViewModel(PluginTreeViewModel trvTree, ControlHierarchicalViewModel? parent, NodeColumnType columnNodeType, 
							   string text, DataSourceColumnModel? column) :
				base(trvTree, parent, text, Explorers.TreeReportingViewModel.NodeType.Field.ToString(), 
					 Explorers.TreeReportingViewModel.IconType.DataWarehouse.ToString(), column, false, false, MvvmColor.Black)
	{
		// Asigna la columna
		ColumnNodeType = columnNodeType;
		Column = column;
		// Cambia el texto del nombre del campo por el alias si es necesario
		if (Column is not null)
		{
			if (!string.IsNullOrWhiteSpace(Column.Alias) && !Column.Alias.Equals(Column.Id, StringComparison.CurrentCultureIgnoreCase))
				Text = $"{Column.Alias} ({Column.Id})";
		}
		// Asigna las propiedades
		if (column is null) // ... si no es una columna, es una cabecera
		{
			IsBold = true;
			Foreground = MvvmColor.Red;
		}
		else // ... es una columna, le asigna sus propiedades
		{
			CanSelect = true;
			SortIndex = -1;
			SortOrder = ColumnRequestModel.SortOrder.Undefined;
		}
		// Asigna los filtros
		if (column is not null && trvTree is TreeQueryReportViewModel tree)
		{
			FilterWhere = new ListReportColumnFilterViewModel(tree.ReportViewModel, this);
			FilterHaving = new ListReportColumnFilterViewModel(tree.ReportViewModel, this);
		}
		// Carga el combo
		LoadComboAggregation();
		// Normaliza las propiedades
		NormalizeProperties();
		// Asigna el manejador de eventos
		PropertyChanged += (sender, args) => {
												if ((args.PropertyName ?? string.Empty).Equals(nameof(IsChecked)) && Column != null)
													NormalizeProperties();
											 };
		ComboAggregationTypes.PropertyChanged += (sender, args) => 
											{ 
												if ((args.PropertyName ?? string.Empty).Equals(nameof(ComboViewModel.SelectedItem)) && Column != null)
													NormalizeProperties();
											};
		// Asigna los comandos
		SortOrderCommand = new BaseCommand(_ => UpdateSortOrder(), _ => CanSort)
									.AddListener(this, nameof(CanSort));
		FilterWhereCommand = new BaseCommand(_ => UpdateFilter(true), _ => CanFilterWhere)
									.AddListener(this, nameof(CanSort));
		FilterHavingCommand = new BaseCommand(_ => UpdateFilter(false), _ => CanFilterHaving)
									.AddListener(this, nameof(CanSort));
	}

	/// <summary>
	///		Normaliza las propiedades que cambian la vista
	/// </summary>
	private void NormalizeProperties()
	{
		switch (ColumnNodeType)
		{
			case NodeColumnType.ExpressionField:
					IsBold = false;
					IsChecked = true;
					CanSelect = true;
					CanFilterHaving = true;
					CanFilterWhere = true;
					CanSort = IsChecked;
					Foreground = MvvmColor.Black;
					Icon = Explorers.TreeReportingViewModel.IconType.Dimension.ToString();
				break;
			case NodeColumnType.ParameterField:
			case NodeColumnType.DataSourceColumn:
					IsBold = false;
					CanSelect = false;
					CanFilterHaving = false;
					CanFilterWhere = true;
					CanSort = false;
					Foreground = MvvmColor.Black;
					Icon = Explorers.TreeReportingViewModel.IconType.Field.ToString();
				break;
			default:
					CanFilterWhere = CanSelect;
					CanSort = IsChecked;
					CanAggregate = IsChecked && !string.IsNullOrWhiteSpace(DataSourceId);
					CanFilterHaving = CanAggregate && GeSelectedAggregation() != ColumnRequestModel.AggregationType.NoAggregated;
				break;
		}
	}

	/// <summary>
	///		Carga el combo de tipos de agregación
	/// </summary>
	private void LoadComboAggregation()
	{
		ComboAggregationTypes = new ComboViewModel(this);
		ComboAggregationTypes.AddItem((int) ColumnRequestModel.AggregationType.NoAggregated, "No agregado");
		ComboAggregationTypes.AddItem((int) ColumnRequestModel.AggregationType.Sum, "Suma");
		ComboAggregationTypes.AddItem((int) ColumnRequestModel.AggregationType.Average, "Media");
		ComboAggregationTypes.AddItem((int) ColumnRequestModel.AggregationType.Max, "Máximo");
		ComboAggregationTypes.AddItem((int) ColumnRequestModel.AggregationType.Min, "Mínimo");
		ComboAggregationTypes.AddItem((int) ColumnRequestModel.AggregationType.StandardDeviation, "Desviación estándar");
		ComboAggregationTypes.SelectedItem = ComboAggregationTypes.Items[0];
	}

	/// <summary>
	///		Modifica la ordenación del campo
	/// </summary>
	private void UpdateSortOrder()
	{
		// Cambia el modo de ordenación
		switch (SortOrder)
		{
			case ColumnRequestModel.SortOrder.Undefined:
					SortOrder = ColumnRequestModel.SortOrder.Ascending;
				break;
			case ColumnRequestModel.SortOrder.Ascending:
					SortOrder = ColumnRequestModel.SortOrder.Descending;
				break;
			case ColumnRequestModel.SortOrder.Descending:
					SortOrder = ColumnRequestModel.SortOrder.Undefined;
				break;
		}
		// Asigna el índice de ordenación
		if (SortOrder == ColumnRequestModel.SortOrder.Undefined)
			SortIndex = -1;
		else if (SortIndex == -1)
			SortIndex = (TreeViewModel as TreeQueryReportViewModel)?.GetSortIndex() ?? 0;
	}

	/// <summary>
	///		Obtiene el tipo de agregación seleccionado en el combo
	/// </summary>
	internal ColumnRequestModel.AggregationType GeSelectedAggregation() => (ColumnRequestModel.AggregationType) (ComboAggregationTypes.SelectedId ?? 0);

	/// <summary>
	///		Modifica el filtro de la columna
	/// </summary>
	private void UpdateFilter(bool whereClause)
	{
		if (TreeViewModel is TreeQueryReportViewModel tree)
		{
			bool hasFilters;

				// Abre el cuadro de diálogo que muestra los filtros posibles
				tree.ReportViewModel.ViewModel.SolutionViewModel.MainController.OpenDialog(GetFilter(whereClause));
				// Cambia el valor que indica si tenemos filtros
				hasFilters = GetFilter(whereClause).FiltersViewModel.Count > 0;
				if (whereClause)
					HasFiltersColumn = hasFilters;
				else
					HasFiltersHaving = hasFilters;
		}
	}

	/// <summary>
	///		Comprueba si el nodo se debe añadir a una solicitud de informe
	/// </summary>
	internal bool MustAddToRequest()
	{
		return ColumnNodeType switch
					{
						NodeColumnType.DimensionColumn => IsChecked || HasFiltersColumn || HasFiltersHaving,
						NodeColumnType.ExpressionField => IsChecked || HasFiltersColumn || HasFiltersHaving,
						NodeColumnType.ParameterField => IsChecked || HasFiltersColumn,
						NodeColumnType.DataSourceColumn => IsChecked,
						_ => false
					};
	}

	/// <summary>
	///		Obtiene el filtro adecuado para la cláusula
	/// </summary>
	private ListReportColumnFilterViewModel GetFilter(bool whereClause)
	{
		if (whereClause)
			return FilterWhere;
		else
			return FilterHaving;
	}

	/// <summary>
	///		Añade los filtros
	/// </summary>
	public void AddFilters(List<FilterRequestModel> filtersWhere, List<FilterRequestModel> filtersHaving)
	{
		// Añade los filtros WHERE
		FilterWhere.Clear();
		FilterWhere.AddRange(filtersWhere);
		HasFiltersColumn = filtersWhere.Count > 0;
		// Añade los filtros HAVING
		FilterHaving.Clear();
		FilterHaving.AddRange(filtersHaving);
		HasFiltersHaving = filtersHaving.Count > 0;
	}

	/// <summary>
	///		Carga los nodos del tipo
	/// </summary>
	protected override void LoadNodes()
	{
		// No hace nada, simplemente implementa la interface
	}

	/// <summary>
	///		Columna
	/// </summary>
	public DataSourceColumnModel? Column { get; }

	/// <summary>
	///		Tipo de nodo de la columna
	/// </summary>
	public NodeColumnType ColumnNodeType { get; }

	/// <summary>
	///		Id de la dimensión (para poder hacer la solicitud de informe)
	/// </summary>
	public string DimensionId { get; set; } = string.Empty;

	/// <summary>
	///		Id del origen de datos del informe (para poder hacer la solicitud de informe)
	/// </summary>
	public string DataSourceId { get; set; } = string.Empty;

	/// <summary>
	///		Indica si se puede seleccionar
	/// </summary>
	public bool CanSelect
	{
		get { return _canSelect; }
		set { CheckProperty(ref _canSelect, value); }
	}

	/// <summary>
	///		Indica si se puede ordenar
	/// </summary>
	public bool CanSort 
	{
		get { return _canSort; }
		set { CheckProperty(ref _canSort, value); }
	}

	/// <summary>
	///		Indica si se puede filtrar en la cláusula WHERE
	/// </summary>
	public bool CanFilterWhere 
	{
		get { return _canFilterWhere; }
		set { CheckProperty(ref _canFilterWhere, value); }
	}

	/// <summary>
	///		Indica si se puede agregar
	/// </summary>
	public bool CanAggregate 
	{
		get { return _canAggregate; }
		set { CheckProperty(ref _canAggregate, value); }
	}

	/// <summary>
	///		Combo con los tipos de agregación
	/// </summary>
	public ComboViewModel ComboAggregationTypes
	{
		get { return _comboAggregationTypes; }
		set { CheckObject(ref _comboAggregationTypes, value); }
	}

	/// <summary>
	///		Indica si se puede filtrar en la cláusula HAVING
	/// </summary>
	public bool CanFilterHaving
	{
		get { return _canFilterHaving; }
		set { CheckProperty(ref _canFilterHaving, value); }
	}

	/// <summary>
	///		Indice para la ordenación
	/// </summary>
	public int SortIndex
	{
		get { return _sortIndex; }
		set { CheckProperty(ref _sortIndex, value); }
	}

	/// <summary>
	///		Modo de ordenación
	/// </summary>
	public ColumnRequestModel.SortOrder SortOrder
	{
		get { return _sortOrder; }
		set { CheckProperty(ref _sortOrder, value); }
	}

	/// <summary>
	///		Indica si tiene un filtro sobre la columna
	/// </summary>
	public bool HasFiltersColumn
	{
		get { return _hasFiltersColumn; }
		set { CheckProperty(ref _hasFiltersColumn, value); }
	}

	/// <summary>
	///		Indica si tiene un filtro sobre el Having
	/// </summary>
	public bool HasFiltersHaving
	{
		get { return _hasFiltersHaving; }
		set { CheckProperty(ref _hasFiltersHaving, value); }
	}

	/// <summary>
	///		Filtro de la cláusula WHERE
	/// </summary>
	public ListReportColumnFilterViewModel FilterWhere
	{
		get { return _filterWhere; }
		set { CheckObject(ref _filterWhere!, value); }
	}

	/// <summary>
	///		Filtro de la cláusula HAVING
	/// </summary>
	public ListReportColumnFilterViewModel FilterHaving
	{
		get { return _filterHaving; }
		set { CheckObject(ref _filterHaving!, value); }
	}

	/// <summary>
	///		Comando para modificar la ordenación
	/// </summary>
	public BaseCommand SortOrderCommand { get; }

	/// <summary>
	///		Comando para modificar el filtro en la cláusula WHERE
	/// </summary>
	public BaseCommand FilterWhereCommand { get; }

	/// <summary>
	///		Comando para modificar el filtro en la cláusula HAVING
	/// </summary>
	public BaseCommand FilterHavingCommand { get; }
}
