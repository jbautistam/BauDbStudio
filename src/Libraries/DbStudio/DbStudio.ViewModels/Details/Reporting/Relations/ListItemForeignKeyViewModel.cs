using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Relations
{
	/// <summary>
	///		ViewModel para un <see cref="DataSourceColumnModel"/>
	/// </summary>
	public class ListItemForeignKeyViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Variables privadas
		private string _columnName;
		private ComboViewModel _comboTargetColumns;

		public ListItemForeignKeyViewModel(DataSourceColumnModel sourceColumn, DimensionModel targetDimension, string targetColumnId)
		{
			// Asigna las propiedades
			SourceColumn = sourceColumn;
			TargetDimension = targetDimension;
			TargetColumnId = targetColumnId;
			// Inicializa el ViewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			ColumnName = SourceColumn.Id;
			// Carga el combo de columnas relacionadas
			LoadComboColumns();
		}

		/// <summary>
		///		Carga el combo de columnas relacionadas
		/// </summary>
		private void LoadComboColumns()
		{
			// Inicializa el combo
			ComboTargetColumns = new ComboViewModel(this);
			// Añade los elementos
			ComboTargetColumns.AddItem(-1, "<Seleccione una columna>", null);
			if (TargetDimension != null)
				foreach (DataSourceColumnModel column in TargetDimension.DataSource.Columns.EnumerateValuesSorted())
				{
					// Añade la columna destino
					ComboTargetColumns.AddItem(-1, column.Id, column);
					// Selecciona la columna destino
					if (!string.IsNullOrWhiteSpace(TargetColumnId) && TargetColumnId.Equals(column.Id, StringComparison.CurrentCultureIgnoreCase))
						ComboTargetColumns.SelectedItem = ComboTargetColumns.Items[ComboTargetColumns.Items.Count - 1];
				}
			// Si no se ha seleccionado nada, se selecciona el primer elemento
			if (ComboTargetColumns.SelectedItem == null)
				ComboTargetColumns.SelectedItem = ComboTargetColumns.Items[0];
		}

		/// <summary>
		///		Obtiene la columna en el combo
		/// </summary>
		internal DataSourceColumnModel? GetRelatedColumn() => ComboTargetColumns.SelectedItem?.Tag as DataSourceColumnModel;

		/// <summary>
		///		Columna origen
		/// </summary>
		public DataSourceColumnModel SourceColumn { get; }

		/// <summary>
		///		Dimensión destino
		/// </summary>
		public DimensionModel TargetDimension { get; }

		/// <summary>
		///		Columna destino
		/// </summary>
		public string TargetColumnId { get; }

		/// <summary>
		///		Nombre de la columna
		/// </summary>
		public string ColumnName
		{
			get { return _columnName; }
			set { CheckProperty(ref _columnName, value); }
		}

		/// <summary>
		///		Combo de columnas relacionadas (destino)
		/// </summary>
		public ComboViewModel ComboTargetColumns
		{
			get { return _comboTargetColumns; }
			set { CheckObject(ref _comboTargetColumns, value); }
		}
	}
}
