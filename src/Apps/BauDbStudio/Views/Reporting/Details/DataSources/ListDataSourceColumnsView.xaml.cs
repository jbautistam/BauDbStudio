using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.DataSources;

namespace Bau.DbStudio.Views.Reporting.Details.DataSources
{
	/// <summary>
	///		Ventana para mantenimiento de <see cref="ListDataSourceColumnsViewModel"/>
	/// </summary>
	public partial class ListDataSourceColumnsView : UserControl
	{
		public ListDataSourceColumnsView()
		{
			InitializeComponent();
		}

		/// <summary>
		///		Carga las columnas
		/// </summary>
		public void Load(ListDataSourceColumnsViewModel viewModel)
		{
			DataContext = ViewModel = viewModel;
		}

		/// <summary>
		///		ViewModel
		/// </summary>
		public ListDataSourceColumnsViewModel ViewModel { get; private set; }
	}
}