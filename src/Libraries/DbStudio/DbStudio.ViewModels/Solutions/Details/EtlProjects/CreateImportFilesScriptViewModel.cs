﻿using System;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects
{
	/// <summary>
	///		ViewModel de creación de archivos SQL de importación de archivos
	/// </summary>
	public class CreateImportFilesScriptViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _dataBaseVariable, _prefixOutputTable, _mountPathVariable, _subPath, _pathInputFiles;
		private string _outputFileName;
		private Connections.ComboConnectionsViewModel _comboConnections;

		public CreateImportFilesScriptViewModel(SolutionViewModel solutionViewModel)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			ComboConnections = new Connections.ComboConnectionsViewModel(SolutionViewModel, string.Empty);
			// Inicializa el viewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel()
		{
			// Asigna las propiedades
			DataBaseVariable = "DbCompute";
			PrefixOutputTable = "SRC_";
			MountPathVariable = "MountPath";
			SubPath = "Copy";
			PathInputFiles = SolutionViewModel.MainViewModel.LastPathSelected;
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos introducidos
				if (ComboConnections.GetSelectedConnection() == null)
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione una conexión");
				else if (string.IsNullOrWhiteSpace(DataBaseVariable))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la base de datos");
				else if (string.IsNullOrWhiteSpace(MountPathVariable))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la variable con el directorio de archivos");
				else if (string.IsNullOrWhiteSpace(SubPath))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el subdirectorio de archivos");
				else if (string.IsNullOrWhiteSpace(PathInputFiles) && !System.IO.Directory.Exists(PathInputFiles))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del directorio donde se encuentran los archivos a importar");
				else if (string.IsNullOrWhiteSpace(OutputFileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre del archivo a generar");
				else 
					validated = true;
				// Devuelve el valor que indica si los datos son correctos
				return validated;
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		protected override void Save()
		{
			if (ValidateData())
			{
				// Guarda el directorio seleccionado
				SolutionViewModel.MainViewModel.LastPathSelected = PathInputFiles;
				// Indica que ya no es nuevo y está grabado
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Combo de conexiones
		/// </summary>
		public Connections.ComboConnectionsViewModel ComboConnections
		{
			get { return _comboConnections; }
			set { CheckObject(ref _comboConnections, value); }
		}

		/// <summary>
		///		Variable con el nombre de base de datos
		/// </summary>
		public string DataBaseVariable
		{
			get { return _dataBaseVariable; }
			set { CheckProperty(ref _dataBaseVariable, value); }
		}

		/// <summary>
		///		Prefijo de las tablas de salida
		/// </summary>
		public string PrefixOutputTable
		{
			get { return _prefixOutputTable; }
			set { CheckProperty(ref _prefixOutputTable, value); }
		}

		/// <summary>
		///		Variable con el directorio de montaje
		/// </summary>
		public string MountPathVariable
		{
			get { return _mountPathVariable; }
			set { CheckProperty(ref _mountPathVariable, value); }
		}

		/// <summary>
		///		Subdirectorio donde se encuentran los archivos de validación
		/// </summary>
		public string SubPath
		{
			get { return _subPath; }
			set { CheckProperty( ref _subPath, value); }
		}

		/// <summary>
		///		Directorio donde se encuentran los archivos a importar
		/// </summary>
		public string PathInputFiles
		{
			get { return _pathInputFiles; }
			set { CheckProperty(ref _pathInputFiles, value); }
		}

		/// <summary>
		///		Nombre del archivo de salida
		/// </summary>
		public string OutputFileName
		{
			get { return _outputFileName; }
			set { CheckProperty(ref _outputFileName, value); }
		}
	}
}