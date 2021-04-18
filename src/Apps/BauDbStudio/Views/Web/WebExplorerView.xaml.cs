using System;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Files;

namespace Bau.DbStudio.Views.Web
{
	/// <summary>
	///		Ventana para mostrar el contenido de un archivo
	/// </summary>
	public partial class WebExplorerView : UserControl
	{
		public WebExplorerView(ScriptFileViewModel viewModel)
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{

		}
	}
}
