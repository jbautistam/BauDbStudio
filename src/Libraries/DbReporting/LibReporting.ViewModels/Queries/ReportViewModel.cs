using System;
using System.Threading.Tasks;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Queries;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace Bau.Libraries.LibReporting.ViewModels.Queries
{
	/// <summary>
	///		ViewModel para mostrar un informe
	/// </summary>
	public class ReportViewModel : BaseObservableObject, DbStudio.ViewModels.Core.Interfaces.IDetailViewModel
	{
		// Variables privadas
		private string _header;
		private TreeQueryReportViewModel _treeColumns;

		public ReportViewModel(ReportingSolutionViewModel reportingSolutionViewModel, ReportModel report) : base(false)
		{
			// Asigna las propiedades
			ReportingSolutionViewModel = reportingSolutionViewModel;
			QueryViewModel = new QueryViewModel(ReportingSolutionViewModel.SolutionViewModel, string.Empty, string.Empty, true);
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
			// Obtiene la consulta
			QueryViewModel.Query = GetQueryRequested();
			// y la ejecuta
			await QueryViewModel.ExecuteQueryAsync();
		}

		/// <summary>
		///		Obtiene la consulta solicitada para este informe
		/// </summary>
		private string GetQueryRequested()
		{
			return ReportingSolutionViewModel.ReportingSolutionManager.GetSqlResponse(TreeColumns.GetReportRequest());
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
		///		ViewModel de la solución
		/// </summary>
		public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

		/// <summary>
		///		ViewModel de ejecución de la consulta
		/// </summary>
		public QueryViewModel QueryViewModel { get; }

		/// <summary>
		///		Informe
		/// </summary>
		public ReportModel Report { get; }

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