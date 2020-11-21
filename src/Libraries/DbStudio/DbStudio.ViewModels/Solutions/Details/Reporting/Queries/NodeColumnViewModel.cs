using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Queries
{
	/// <summary>
	///		ViewModel de un nodo de columna
	/// </summary>
	public class NodeColumnViewModel : BaseTreeNodeViewModel
	{
		// Variables privadas
		private bool _canSelect, _canSort, _canFilterWhere, _canAggregate, _canFilterHaving;
		private BaseColumnRequestModel.SortOrder _sortOrder;
		private ComboViewModel _comboAggregationTypes;
		private ListReportColumnFilterViewModel _filterWhere, _filterHaving;

		public NodeColumnViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, string text, DataSourceColumnModel column) :
					base(trvTree, parent, text, NodeType.ConnectionRoot, IconType.Connection, column, false, false, MvvmColor.Black)
		{
			// Asigna la columna
			Column = column;
			// Asigna las propiedades
			if (column == null) // ... si no es una columna, es una cabecera
			{
				IsBold = true;
				Foreground = MvvmColor.Red;
			}
			else // ... es una columna, le asigna sus propiedades
			{
				CanSelect = !column.IsPrimaryKey;
				SortOrder = BaseColumnRequestModel.SortOrder.Undefined;
			}
			// Asigna los filtros
			if (column != null && trvTree is TreeQueryReportViewModel tree)
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
													if (args.PropertyName.Equals(nameof(IsChecked)) && Column != null)
														NormalizeProperties();
												 };
			ComboAggregationTypes.PropertyChanged += (sender, args) => 
												{ 
													if (args.PropertyName.Equals(nameof(ComboViewModel.SelectedItem)) && Column != null)
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
			CanFilterWhere = CanSelect;
			CanSort = IsChecked;
			CanAggregate = IsChecked && !string.IsNullOrWhiteSpace(DataSourceId);
			CanFilterHaving = CanAggregate && GeSelectedAggregation() != ExpressionRequestModel.AggregationType.NoAggregated;
		}

		/// <summary>
		///		Carga el combo de tipos de agregación
		/// </summary>
		private void LoadComboAggregation()
		{
			ComboAggregationTypes = new ComboViewModel(this);
			ComboAggregationTypes.AddItem((int) ExpressionRequestModel.AggregationType.NoAggregated, "No agregado");
			ComboAggregationTypes.AddItem((int) ExpressionRequestModel.AggregationType.Sum, "Suma");
			ComboAggregationTypes.AddItem((int) ExpressionRequestModel.AggregationType.Average, "Media");
			ComboAggregationTypes.AddItem((int) ExpressionRequestModel.AggregationType.Max, "Máximo");
			ComboAggregationTypes.AddItem((int) ExpressionRequestModel.AggregationType.Min, "Mínimo");
			ComboAggregationTypes.AddItem((int) ExpressionRequestModel.AggregationType.StandardDeviation, "Desviación estándar");
			ComboAggregationTypes.SelectedItem = ComboAggregationTypes.Items[0];
		}

		/// <summary>
		///		Modifica la ordenación del campo
		/// </summary>
		private void UpdateSortOrder()
		{
			switch (SortOrder)
			{
				case BaseColumnRequestModel.SortOrder.Undefined:
						SortOrder = BaseColumnRequestModel.SortOrder.Ascending;
					break;
				case BaseColumnRequestModel.SortOrder.Ascending:
						SortOrder = BaseColumnRequestModel.SortOrder.Descending;
					break;
				case BaseColumnRequestModel.SortOrder.Descending:
						SortOrder = BaseColumnRequestModel.SortOrder.Undefined;
					break;
			}
		}

		/// <summary>
		///		Obtiene el tipo de agregación seleccionado en el combo
		/// </summary>
		internal ExpressionRequestModel.AggregationType GeSelectedAggregation()
		{
			return (ExpressionRequestModel.AggregationType) (ComboAggregationTypes.SelectedId ?? 0);
		}

		/// <summary>
		///		Modifica el filtro de la columna
		/// </summary>
		private void UpdateFilter(bool whereClause)
		{
			if (TreeViewModel is TreeQueryReportViewModel tree)
				tree.SolutionViewModel.MainViewModel.MainController.OpenDialog(GetFilter(whereClause));
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
		///		Carga los nodos del tipo
		/// </summary>
		protected override void LoadNodes()
		{
			// No hace nada, simplemente implementa la interface
		}

		/// <summary>
		///		Columna
		/// </summary>
		public DataSourceColumnModel Column { get; }

		/// <summary>
		///		Id de la dimensión (para poder hacer la solicitud de informe)
		/// </summary>
		public string DimensionId { get; set; }

		/// <summary>
		///		Id del origen de datos del informe (para poder hacer la solicitud de informe)
		/// </summary>
		public string DataSourceId { get; set; }

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
		///		Modo de ordenación
		/// </summary>
		public BaseColumnRequestModel.SortOrder SortOrder
		{
			get { return _sortOrder; }
			set { CheckProperty(ref _sortOrder, value); }
		}

		/// <summary>
		///		Filtro de la cláusula WHERE
		/// </summary>
		public ListReportColumnFilterViewModel FilterWhere
		{
			get { return _filterWhere; }
			set { CheckObject(ref _filterWhere, value); }
		}

		/// <summary>
		///		Filtro de la cláusula HAVING
		/// </summary>
		public ListReportColumnFilterViewModel FilterHaving
		{
			get { return _filterHaving; }
			set { CheckObject(ref _filterHaving, value); }
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
}
