using System;

using Bau.Libraries.LibReporting.Application;
using Bau.Libraries.LibReporting.Models.DataWarehouses;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting
{
	/// <summary>
	///		ViewModel para una solución de informes
	/// </summary>
	public class ReportingSolutionViewModel : BauMvvm.ViewModels.BaseObservableObject
	{
		// Variables privadas
		private Explorers.TreeReportingViewModel _treeReportingViewModel;

		public ReportingSolutionViewModel(SolutionViewModel solutionViewModel)
		{
			SolutionViewModel = solutionViewModel;
			ReportingManager = new ReportingManager();
			TreeReportingViewModel = new Explorers.TreeReportingViewModel(this);
		}

		/// <summary>
		///		Carga la solución
		/// </summary>
		public void Load(string fileName)
		{
			// Carga la solución
			ReportingManager.LoadSolution(fileName);
			// Carga el árbol del explorador
			TreeReportingViewModel.Load();
		}

		/// <summary>
		///		Graba la solución
		/// </summary>
		internal void SaveSolution()
		{
			ReportingManager.SaveSolution();
		}

		/// <summary>
		///		Graba un origen de datos
		/// </summary>
		internal void SaveDataWarehouse(DataWarehouseModel dataWarehouse)
		{
			// Graba el archivo
			ReportingManager.SaveDataWarehouse(dataWarehouse);
			// Actualiza el árbol
			TreeReportingViewModel.Load();
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Manager de reporting
		/// </summary>
		public ReportingManager ReportingManager { get; }

		/// <summary>
		///		ViewModel del árbol de informes
		/// </summary>
		public Explorers.TreeReportingViewModel TreeReportingViewModel
		{
			get { return _treeReportingViewModel; }
			set { CheckObject(ref _treeReportingViewModel, value); }
		}
	}
}
