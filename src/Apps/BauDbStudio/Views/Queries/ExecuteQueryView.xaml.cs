﻿using System;
using System.Windows.Controls;

using Bau.Libraries.BauMvvm.Views.Wpf.Forms.Trees;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Queries;

namespace Bau.DbStudio.Views.Queries
{
	/// <summary>
	///		Ventana para ejecutar una consulta
	/// </summary>
	public partial class ExecuteQueryView : UserControl
	{
		// Variables privadas
		private DragDropTreeExplorerController _dragDropController = new DragDropTreeExplorerController();

		public ExecuteQueryView(ExecuteQueryViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		Inicializa el formulario
		/// </summary>
		private void InitForm()
		{
			udtQuery.LoadControl(ViewModel.QueryViewModel);
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ExecuteQueryViewModel ViewModel { get; }

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			InitForm();
		}
	}
}