using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Queries;

namespace Bau.Libraries.DbStudio.Views.Reporting.Queries
{
	/// <summary>
	///		Ventana para ejecutar una consulta
	/// </summary>
	public partial class ReportView : UserControl
	{
		public ReportView(ReportViewModel viewModel)
		{
			DataContext = ViewModel = viewModel;
			InitializeComponent();
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			// Inicializa el árbol
			trvFields.LoadControl(ViewModel.TreeColumns);
			// Carga el control interno
			udtQuery.LoadControl(ViewModel.QueryViewModel);
			// Carga el viewModel
			ViewModel.Load();
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ReportViewModel ViewModel { get; }

		private void UserControl_Initialized(object sender, EventArgs e)
		{
			InitForm();
		}
	}
}