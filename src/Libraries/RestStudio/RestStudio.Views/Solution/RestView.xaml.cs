using System;
using System.Windows;

using Bau.Libraries.RestStudio.ViewModels.Solution;

namespace Bau.Libraries.RestStudio.Views.Solution
{
	/// <summary>
	///		Vista para el mantenimiento de <see cref="RestApiViewModel"/>
	/// </summary>
	public partial class RestView : Window
	{
		public RestView(RestApiViewModel viewModel)
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
		public RestApiViewModel ViewModel { get; }
	}
}
