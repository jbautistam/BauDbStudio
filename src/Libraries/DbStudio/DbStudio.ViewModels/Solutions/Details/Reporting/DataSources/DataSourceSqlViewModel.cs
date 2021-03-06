﻿using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.DataSources
{
	/// <summary>
	///		ViewModel de mantenimiento de un <see cref="DataSourceSqlModel"/>
	/// </summary>
	public class DataSourceSqlViewModel : BaseObservableObject, IDetailViewModel
	{
		// Variables privadas
		private string _key, _name, _description, _sql, _header;
		private ListDataSourceColumnsViewModel _columns;

		public DataSourceSqlViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DataSourceSqlModel dataSource)
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
			if (string.IsNullOrWhiteSpace(DataSource.Name))
			{
				Header = "Nuevo origen de datos";
				Key = string.Empty;
			}
			else
			{
				Header = DataSource.Name;
				Key = DataSource.GlobalId;
			}
			Name = DataSource.Name;
			Sql = DataSource.Sql;
			Description = DataSource.Description;
			// Carga las columnas
			ColumnsViewModel = new ListDataSourceColumnsViewModel(ReportingSolutionViewModel, DataSource, true);
			// Indica que por ahora no ha habido modificaciones
			IsUpdated = false;
		}

		/// <summary>
		///		Obtiene el mensaje de grabación
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return $"¿Desea grabar las modificaciones del origen de datos '{Name}'?";
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos
				if (string.IsNullOrWhiteSpace(Key))
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca la clave del origen de datos");
				else if (string.IsNullOrWhiteSpace(Name))
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del origen de datos");
				else if (string.IsNullOrWhiteSpace(Sql))
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el comando SQL del origen de datos");
				else if (ColumnsViewModel.Items.Count == 0)
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
				// Añade el origen de datos si es nuevo
				if (DataSource.DataWarehouse.DataSources.Search(DataSource.GlobalId) == null)
					DataSource.DataWarehouse.DataSources.Add(DataSource);
				// Asigna las propiedades
				DataSource.GlobalId = Key;
				DataSource.Name = Name;
				DataSource.Description = Description;
				DataSource.Sql = Sql;
				// Asigna las columnas
				DataSource.Columns.Clear();
				DataSource.Columns.AddRange(ColumnsViewModel.GetColumns());
				// Graba la solución
				ReportingSolutionViewModel.SaveDataWarehouse(DataSource.DataWarehouse);
				// Cambia la cabecera
				Header = DataSource.Name;
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
		public DataSourceSqlModel DataSource { get; }

		/// <summary>
		///		Cabecera de la ventana
		/// </summary>
		public string Header
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
		}

		/// <summary>
		///		Id de la ficha en la aplicación
		/// </summary>
		public string TabId
		{
			get { return $"{GetType().ToString()}_{DataSource.GlobalId}"; }
		}

		/// <summary>
		///		Clave
		/// </summary>
		public string Key
		{
			get { return _key; }
			set { CheckProperty(ref _key, value); }
		}

		/// <summary>
		///		Nombre
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { CheckProperty(ref _name, value); }
		}

		/// <summary>
		///		Descripción
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { CheckProperty(ref _description, value); }
		}

		/// <summary>
		///		Sql
		/// </summary>
		public string Sql
		{
			get { return _sql; }
			set { CheckProperty(ref _sql, value); }
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