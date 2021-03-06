﻿using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Dimension
{
	/// <summary>
	///		ViewModel de mantenimiento de un <see cref="DimensionModel"/>
	/// </summary>
	public class DimensionViewModel : BaseObservableObject, IDetailViewModel
	{
		// Variables privadas
		private string _key, _dataSourceId, _name, _description;
		private bool _isNew;
		private Relations.ListRelationViewModel _listRelationsViewModel;

		public DimensionViewModel(ReportingSolutionViewModel reportingSolutionViewModel, DimensionModel dimension, bool isNew)
		{
			// Inicializa los objetos
			ReportingSolutionViewModel = reportingSolutionViewModel;
			Dimension = dimension;
			// Inicializa las variables
			_isNew = isNew;
			// Inicializa las propiedades
			InitViewModel();
			// Asigna los manejadores de eventos
			ListRelationsViewModel.PropertyChanged += (sender, args) => {
																			if (args.PropertyName.Equals(nameof(ListRelationsViewModel.IsUpdated)))
																				IsUpdated |= ListRelationsViewModel.IsUpdated;
																		};
		}

		/// <summary>
		///		Inicializa el viewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades básicas
			if (_isNew)
			{
				switch (Dimension.DataSource)
				{
					case LibReporting.Models.DataWarehouses.DataSets.DataSourceTableModel dataSourceTable:
							Name = $"Dim{dataSourceTable.Table}";
							DataSourceId = dataSourceTable.GlobalId;
						break;
					case LibReporting.Models.DataWarehouses.DataSets.DataSourceSqlModel dataSourceSql:
							Name = $"Dim{dataSourceSql.Name}";
							DataSourceId = dataSourceSql.GlobalId;
						break;
				}
				Key = Name;
			}
			else
			{
				Key = Dimension.GlobalId;
				Name = Dimension.Name;
				DataSourceId = Dimension.DataSource.GlobalId;
			}
			// Asigna el resto de propiedades
			Description = Dimension.Description;
			// Carga las relaciones hijas
			ListRelationsViewModel = new Relations.ListRelationViewModel(ReportingSolutionViewModel, Dimension.DataSource, Dimension.Relations);
			ListRelationsViewModel.Load();
			// Indica que por ahora no ha habido modificaciones
			IsUpdated = false;
			ListRelationsViewModel.IsUpdated = false;
		}

		/// <summary>
		///		Obtiene el mensaje de grabación
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return $"¿Desea grabar las modificaciones de la dimensión '{Name}'?";
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos
				if (string.IsNullOrWhiteSpace(Key))
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca la clave de la dimensión");
				else if (string.IsNullOrWhiteSpace(Name))
					ReportingSolutionViewModel.SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la dimensión");
				else
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
				// Asigna las propiedades a la dimensión
				Dimension.GlobalId = Key;
				Dimension.Name = Name;
				Dimension.Description = Description;
				// Si es nuevo se añade a la colección
				if (_isNew)
				{
					// Añade la dimensión
					Dimension.DataSource.DataWarehouse.Dimensions.Add(Dimension);
					// Indica que ya no es nuevo
					_isNew = false;
				}
				// Asigna las relaciones
				Dimension.Relations.Clear();
				Dimension.Relations.AddRange(ListRelationsViewModel.GetRelations());
				// Graba la solución
				ReportingSolutionViewModel.SaveDataWarehouse(Dimension.DataSource.DataWarehouse);
				// Indica que no ha habido modificaciones
				IsUpdated = false;
				ListRelationsViewModel.IsUpdated = false;
			}
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

		/// <summary>
		///		Dimensión
		/// </summary>
		public DimensionModel Dimension { get; }

		/// <summary>
		///		Cabecera de la ventana
		/// </summary>
		public string Header
		{
			get { return Name; }
		}

		/// <summary>
		///		Id de la ficha en la aplicación
		/// </summary>
		public string TabId
		{
			get { return $"{GetType().ToString()}_{Dimension.GlobalId}"; }
		}

		/// <summary>
		///		Clave de la dimensión
		/// </summary>
		public string Key
		{
			get { return _key; }
			set { CheckProperty(ref _key, value); }
		}

		/// <summary>
		///		Id del origen de datos
		/// </summary>
		public string DataSourceId
		{
			get { return _dataSourceId; }
			set { CheckProperty(ref _dataSourceId, value); }
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
		///		ViewModel con las relaciones con otras dimensiones
		/// </summary>
		public Relations.ListRelationViewModel ListRelationsViewModel
		{
			get { return _listRelationsViewModel; }
			set { CheckObject(ref _listRelationsViewModel, value); }
		}
	}
}