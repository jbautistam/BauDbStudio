﻿using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Reports
{
	/// <summary>
	///		ViewModel de mantenimiento de un <see cref="ReportDataSourceModel"/>
	/// </summary>
	public class ReportDataSourceViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private ComboViewModel _comboDataSources;
		private Relations.ListRelationViewModel _listRelationsViewModel;

		public ReportDataSourceViewModel(ReportingSolutionViewModel reportingSolutionViewModel, ReportModel report, ReportDataSourceModel reportDataSource)
		{
			// Inicializa los objetos
			ReportingSolutionViewModel = reportingSolutionViewModel;
			Report = report;
			ReportDataSource = reportDataSource;
			// Inicializa las propiedades
			InitViewModel();
			// Inicializa el manejador de eventos sobre el combo
			ComboDataSources.PropertyChanged += (sender, args) => {
																	if (args.PropertyName.Equals(nameof(ComboDataSources.SelectedItem)))
																	{
																		// Carga las relaciones
																		LoadRelations();
																		// Indica que se ha hecho alguna modificación
																		IsUpdated = true;
																	}
																  };
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		private void InitViewModel()
		{
			LoadComboDataSources();
			LoadRelations();
		}

		/// <summary>
		///		Carga el combo de orígenes de datos
		/// </summary>
		private void LoadComboDataSources()
		{
			// Inicializa el combo
			ComboDataSources = new ComboViewModel(this);
			// Ordena las dimensiones por nombre
			Report.DataWarehouse.DataSources.SortByName();
			// Añade los elementos
			ComboDataSources.AddItem(-1, "<Seleccione un origen de datos>");
			foreach (BaseDataSourceModel dataSource in Report.DataWarehouse.DataSources)
			{
				// Añade el elemento
				ComboDataSources.AddItem(ComboDataSources.Items.Count + 1, dataSource.Name, dataSource);
				// Selecciona el elemento si es la misma dimensión
				if (ReportDataSource != null && ReportDataSource.DataSource != null && 
						dataSource.GlobalId.Equals(ReportDataSource.DataSource.GlobalId, StringComparison.CurrentCultureIgnoreCase))
					ComboDataSources.SelectedItem = ComboDataSources.Items[ComboDataSources.Items.Count - 1];
			}
			// Selecciona el primer elemento
			if (ComboDataSources.SelectedItem == null)
				ComboDataSources.SelectedItem = ComboDataSources.Items[0];
		}

		/// <summary>
		///		Carga las relaciones
		/// </summary>
		private void LoadRelations()
		{
			BaseDataSourceModel dataSource = GetDataSource();

				if (dataSource == null)
					ListRelationsViewModel = null;
				else
				{
					System.Collections.Generic.List<DimensionRelationModel> relations = new System.Collections.Generic.List<DimensionRelationModel>();

						// Busca las relaciones
						foreach (ReportDataSourceModel reportDataSource in Report.ReportDataSources)
							if (reportDataSource.DataSource.GlobalId.Equals(dataSource.GlobalId))
								relations.AddRange(reportDataSource.Relations);
						// Asocia la lista de relaciones
						ListRelationsViewModel = new Relations.ListRelationViewModel(ReportingSolutionViewModel, dataSource, ReportDataSource.Relations);
						// Carga las relaciones
						ListRelationsViewModel.Load();
				}
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		internal bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos
				if (GetDataSource() == null)
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione el origen de datos");
				else
					validated = ListRelationsViewModel.ValidateData();
				// Devuelve el valor que indica si se ha podido grabar
				return validated;
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		protected override void Save()
		{
			if (ValidateData())
			{
				// Indica que ya no es nuevo y está grabado
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		Obtiene el origen de datos seleccionado
		/// </summary>
		private BaseDataSourceModel GetDataSource()
		{
			return ComboDataSources.SelectedItem.Tag as BaseDataSourceModel;
		}

		/// <summary>
		///		Obtiene el origen de datos del informe con sus relaciones
		/// </summary>
		internal ReportDataSourceModel GetReportDataSource()
		{
			ReportDataSourceModel dataSource = new ReportDataSourceModel(Report)
														{ 
															DataSource = GetDataSource()
														};

				// Añade las relaciones
				dataSource.Relations.AddRange(ListRelationsViewModel.GetRelations());
				// Devuelve el origen de datos del informe
				return dataSource;
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

		/// <summary>
		///		Informe padre del origen de datos
		/// </summary>
		public ReportModel Report { get; }

		/// <summary>
		///		Origen de datos del informe
		/// </summary>
		public ReportDataSourceModel ReportDataSource { get; }

		/// <summary>
		///		Combo de dimensiones
		/// </summary>
		public ComboViewModel ComboDataSources
		{
			get { return _comboDataSources; }
			set { CheckObject(ref _comboDataSources, value); }
		}

		/// <summary>
		///		Lista de relacioness
		/// </summary>
		public Relations.ListRelationViewModel ListRelationsViewModel
		{
			get { return _listRelationsViewModel; }
			set { CheckObject(ref _listRelationsViewModel, value); }
		}
	}
}