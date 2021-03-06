﻿using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Queries;

namespace Bau.DbStudio.Views.Reporting.Queries
{
	/// <summary>
	///		Ventana para ejecutar una consulta
	/// </summary>
	public partial class ReportView : UserControl
	{
		public ReportView(ReportViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
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

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}
	}
}