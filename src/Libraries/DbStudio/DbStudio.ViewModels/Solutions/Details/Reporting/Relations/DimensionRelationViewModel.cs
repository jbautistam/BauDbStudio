using System;
using System.Collections.ObjectModel;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Relations
{
	/// <summary>
	///		ViewModel de mantenimiento de un <see cref="DimensionRelationModel"/>
	/// </summary>
	public class DimensionRelationViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _targetDimensionName, _foreignKeysTitle;
		private ComboViewModel _comboDimensions;
		private ObservableCollection<ListItemForeignKeyViewModel> _foreignKeys;

		public DimensionRelationViewModel(ReportingSolutionViewModel reportingSolutionViewModel, BaseDataSourceModel dataSource, DimensionRelationModel relation)
		{
			// Inicializa los objetos
			ReportingSolutionViewModel = reportingSolutionViewModel;
			DataSource = dataSource;
			Relation = relation;
			// Inicializa las propiedades
			InitViewModel();
			// Inicializa el manejador de eventos sobre el combo
			ComboDimensions.PropertyChanged += (sender, args) => {
																	if (args.PropertyName.Equals(nameof(ComboDimensions.SelectedItem)))
																		LoadListForeignKeys();
																 };
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		private void InitViewModel()
		{
			// Carga el combo de dimensiones
			LoadComboDimensions();
			// Carga la lista de claves foráneas
			LoadListForeignKeys();
			// Asigna las propiedades
			TargetDimensionName = Relation?.Dimension?.Name;
			ForeignKeysTitle = GetForeignKeysName();
		}

		/// <summary>
		///		Carga el combo de dimensiones
		/// </summary>
		private void LoadComboDimensions()
		{
			// Inicializa el combo
			ComboDimensions = new ComboViewModel(this);
			// Ordena las dimensiones por nombre
			//DataSource.DataWarehouse.Dimensions.SortByName();
			// Añade los elementos
			ComboDimensions.AddItem(-1, "<Seleccione una dimensión>");
			foreach (DimensionModel dimension in DataSource.DataWarehouse.Dimensions.EnumerateValues())
			{
				// Añade el elemento
				ComboDimensions.AddItem(ComboDimensions.Items.Count + 1, dimension.Name, dimension);
				// Selecciona el elemento si es la misma dimensión
				if (Relation != null && Relation.Dimension != null && 
						dimension.Id.Equals(Relation.Dimension.Id, StringComparison.CurrentCultureIgnoreCase))
					ComboDimensions.SelectedItem = ComboDimensions.Items[ComboDimensions.Items.Count - 1];
			}
			// Selecciona el primer elemento
			if (ComboDimensions.SelectedItem == null)
				ComboDimensions.SelectedItem = ComboDimensions.Items[0];
		}

		/// <summary>
		///		Carga la lista de claves foráneas
		/// </summary>
		private void LoadListForeignKeys()
		{
			// Limpia la lista
			ForeignKeys = new ObservableCollection<ListItemForeignKeyViewModel>();
			// Añade los elementos a la lista
			foreach (DataSourceColumnModel column in DataSource.Columns)
			{
				string targetColumnId = string.Empty;

					// Obtiene la columna relacionada
					if (Relation != null)
						foreach (RelationForeignKey relationKey in Relation.ForeignKeys)
							if (relationKey.ColumnId.Equals(column.ColumnId, StringComparison.CurrentCultureIgnoreCase))
								targetColumnId = relationKey.TargetColumnId;
					// Añade la columna
					ForeignKeys.Add(new ListItemForeignKeyViewModel(column, GetDimension(), targetColumnId));
			}
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		internal bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos
				if (GetDimension() == null)
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione la dimensión");
				else if (!ValidateForeignKeys())
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione al menos una clave foránea");
				else
					validated = true;
				// Devuelve el valor que indica si se ha podido grabar
				return validated;
		}

		/// <summary>
		///		Comprueba que haya al menos una clave foránea
		/// </summary>
		private bool ValidateForeignKeys()
		{
			// Busca si hay alguna clave foránea definida
			foreach (ListItemForeignKeyViewModel foreignKey in ForeignKeys)
				if (foreignKey.GetRelatedColumn() != null)
					return true;
			// Si ha llegado hasta aquí es porque no se ha seleccionado ninguna clave foránea
			return false;
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		protected override void Save()
		{
			if (ValidateData())
			{
				// Asigna las propiedades que se van a mostrar en la lista
				TargetDimensionName = GetDimension().Name;
				ForeignKeysTitle = GetForeignKeysName();
				// Indica que ya no es nuevo y está grabado
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		Obtiene el título con las claves foráneas
		/// </summary>
		private string GetForeignKeysName()
		{
			string title = string.Empty;

				// Busca si hay alguna clave foránea definida
				foreach (ListItemForeignKeyViewModel foreignKey in ForeignKeys)
				{
					DataSourceColumnModel relatedColumn = foreignKey.GetRelatedColumn();

						// Añade el título a la cadena
						if (relatedColumn != null)
							title = title.AddWithSeparator($"{foreignKey.ColumnName} -> {relatedColumn.ColumnId}", ",");
				}
				// Si ha llegado hasta aquí es porque no se ha seleccionado ninguna clave foránea
				return title;
		}

		/// <summary>
		///		Obtiene la dimensión
		/// </summary>
		internal DimensionModel GetDimension()
		{
			return ComboDimensions.SelectedItem.Tag as DimensionModel;
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

		/// <summary>
		///		Origen de datos sobre el que se define la relación
		/// </summary>
		public BaseDataSourceModel DataSource { get; }

		/// <summary>
		///		Relación
		/// </summary>
		public DimensionRelationModel Relation { get; }

		/// <summary>
		///		Nombre de la dimensión destino
		/// </summary>
		public string TargetDimensionName
		{
			get { return _targetDimensionName; }
			set { CheckProperty(ref _targetDimensionName, value); }
		}

		/// <summary>
		///		Nombre de las columnas de clave primaria
		/// </summary>
		public string ForeignKeysTitle
		{
			get { return _foreignKeysTitle; }
			set { CheckProperty(ref _foreignKeysTitle, value); }
		}

		/// <summary>
		///		Combo de dimensiones
		/// </summary>
		public ComboViewModel ComboDimensions
		{
			get { return _comboDimensions; }
			set { CheckObject(ref _comboDimensions, value); }
		}

		/// <summary>
		///		Lista de claves foráneas
		/// </summary>
		public ObservableCollection<ListItemForeignKeyViewModel> ForeignKeys
		{
			get { return _foreignKeys; }
			set { CheckObject(ref _foreignKeys, value);}
		}
	}
}