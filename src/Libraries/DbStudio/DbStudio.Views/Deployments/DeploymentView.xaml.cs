using System;
using System.Windows;

using Bau.Libraries.DbStudio.ViewModels.Details.Deployments;

namespace Bau.Libraries.DbStudio.Views.Deployments
{
	/// <summary>
	///		Vista para mostrar los datos de una distribución
	/// </summary>
	public partial class DeploymentView : Window
	{
		public DeploymentView(DeploymentViewModel viewModel)
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
		///		ViewModel de la distribución
		/// </summary>
		public DeploymentViewModel ViewModel { get; }
	}
}
