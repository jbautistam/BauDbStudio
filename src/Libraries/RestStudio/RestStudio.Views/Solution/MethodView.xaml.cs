using System;
using System.Windows;

using Bau.Libraries.RestStudio.ViewModels.Solution;

namespace Bau.Libraries.RestStudio.Views.Solution
{
	/// <summary>
	///		Vista para el mantenimiento de <see cref="ContextViewModel"/>
	/// </summary>
	public partial class MethodView : Window
	{
		public MethodView(MethodViewModel viewModel)
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
		///		ViewModel
		/// </summary>
		public MethodViewModel ViewModel { get; }
	}
}
