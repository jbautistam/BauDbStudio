using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Connections;

namespace Bau.DbStudio.Views.Connections
{
	/// <summary>
	///		Vista para mostrar los archivos de parámetros de una conexión
	/// </summary>
	public partial class ConnectionParametersView : Window
	{
		public ConnectionParametersView(ConnectionParametersExecutionViewModel viewModel)
		{
			InitializeComponent();
			DataContext = ViewModel = viewModel;
			ViewModel.Close += (sender, eventArgs) => 
									{
										DialogResult = eventArgs.IsAccepted; 
										Close();
									};
		}

		/// <summary>
		///		ViewModel de los parámetros
		/// </summary>
		public ConnectionParametersExecutionViewModel ViewModel { get; }
	}
}
