using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.DataSources
{
	/// <summary>
	///		ViewModel de mantenimiento de un <see cref="DataSourceTableModel"/>
	/// </summary>
	public class DataSourceTableViewModel : BaseObservableObject, IDetailViewModel
	{
		// Variables privadas
		private string _schema, _table;
		private ListDataSourceColumnsViewModel _columns;

		public DataSourceTableViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DataSourceTableModel dataSource)
		{
			// Inicializa los objetos
			ReportingSolutionViewModel = reportingSolutionViewModel;
			DataSource = dataSource;
			// Inicializa las propiedades
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			Schema = DataSource.Schema;
			Table = DataSource.Table;
			// Carga las columnas
			ColumnsViewModel = new ListDataSourceColumnsViewModel(ReportingSolutionViewModel, DataSource, false);
			// Indica que por ahora no ha habido modificaciones
			IsUpdated = false;
		}

		/// <summary>
		///		Obtiene el mensaje de grabación
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return $"¿Desea grabar las modificaciones del origen de datos '{Schema}.{Table}'?";
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos
				if (ColumnsViewModel.Items.Count == 0)
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("No se ha definido ninguna columna");
				else if (ColumnsViewModel.ValidateData())
					validated = true;
				// Devuelve el valor que indica si se ha podido grabar
				return validated;
		}

		/// <summary>
		///		Graba la dimensión
		/// </summary>
		public void SaveDetails(bool newName)
		{
			if (ValidateData())
			{
				// Asigna las columnas
				DataSource.Columns.Clear();
				DataSource.Columns.AddRange(ColumnsViewModel.GetColumns());
				// Graba la solución
				ReportingSolutionViewModel.SaveDataWarehouse(DataSource.DataWarehouse);
				// Indica que no ha habido modificaciones
				IsUpdated = false;
			}
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

		/// <summary>
		///		Origen de datos
		/// </summary>
		public DataSourceTableModel DataSource { get; }

		/// <summary>
		///		Cabecera de la ventana
		/// </summary>
		public string Header
		{
			get { return DataSource.FullName; }
		}

		/// <summary>
		///		Id de la ficha en la aplicación
		/// </summary>
		public string TabId
		{
			get { return $"{GetType().ToString()}_{DataSource.Id}"; }
		}

		/// <summary>
		///		Esquema
		/// </summary>
		public string Schema
		{
			get { return _schema; }
			set { CheckProperty(ref _schema, value); }
		}

		/// <summary>
		///		Tabla
		/// </summary>
		public string Table
		{
			get { return _table; }
			set { CheckProperty(ref _table, value); }
		}

		/// <summary>
		///		Columnas
		/// </summary>
		public ListDataSourceColumnsViewModel ColumnsViewModel
		{
			get { return _columns; }
			set { CheckProperty(ref _columns, value); }
		}
	}
}