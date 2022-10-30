using System;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
//using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.DbStudio.ViewModels.Details.Queries;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries
{
	/// <summary>
	///		ViewModel para mostrar un informe
	/// </summary>
	public class ReportViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
	{
		// Variables privadas
		private string _header;
		private TreeQueryReportViewModel _treeColumns;

		public ReportViewModel(ReportingSolutionViewModel viewModel, ReportBaseModel report) : base(false)
		{
			// Asigna las propiedades
			ViewModel = viewModel;
			QueryViewModel = new QueryViewModel(ViewModel.SolutionViewModel, string.Empty, string.Empty, true);
			Report = report;
			Header = report.Id;
			// Inicializa el árbol de campos
			TreeColumns = new TreeQueryReportViewModel(this);
			// Apunta al manejador de eventos de la consulta
			QueryViewModel.ExecutionRequested += async (sender, args) => await ExecuteQueryAsync();
		}

		/// <summary>
		///		Carga los datos
		/// </summary>
		public void Load()
		{
			TreeColumns.Load();
		}

		/// <summary>
		///		Ejecuta la consulta
		/// </summary>
		private async Task ExecuteQueryAsync()
		{
			ReportRequestModel reportRequest = TreeColumns.GetReportRequest();

				// Actualiza el informe recargando el archivo
				if (Report is ReportAdvancedModel report)
					ViewModel.ReportingSolutionManager.RefreshAdvancedReport(Report.DataWarehouse, report.FileName);
				// Obtiene la consulta
				QueryViewModel.Query = ViewModel.ReportingSolutionManager.GetSqlResponse(reportRequest);
				// Añade los parámetros
				foreach (System.Collections.Generic.KeyValuePair<string, object> parameter in reportRequest.Parameters)
					QueryViewModel.Arguments.Parameters.Add(parameter.Key, parameter.Value);
				// y la ejecuta
				await QueryViewModel.ExecuteQueryAsync();
		}

		/// <summary>
		///		Obtiene el mensaje para grabar y cerrar
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return string.Empty;
		}

		/// <summary>
		///		Graba el contenido
		/// </summary>
		public void SaveDetails(bool newName)
		{
			// No hace nada, sólo implementa la interface
		}

		/// <summary>
		///		Cierra el viewmodel
		/// </summary>
		public void Close()
		{
			// ... no hace nada, sólo implementa la interface
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public ReportingSolutionViewModel ViewModel { get; }

		/// <summary>
		///		ViewModel de ejecución de la consulta
		/// </summary>
		public QueryViewModel QueryViewModel { get; }

		/// <summary>
		///		Informe
		/// </summary>
		public ReportBaseModel Report { get; }

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
		}

		/// <summary>
		///		Id de la pestaña
		/// </summary>
		public string TabId
		{
			get { return $"{GetType().ToString()}_{Report.Id}"; }
		}

		/// <summary>
		///		Arbol de columnas de la consulta
		/// </summary>
		public TreeQueryReportViewModel TreeColumns
		{
			get { return _treeColumns; }
			set { CheckObject(ref _treeColumns, value); }
		}
	}
}