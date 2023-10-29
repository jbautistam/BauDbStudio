using System.Diagnostics;
using System.Windows;

namespace Bau.DbStudio.Views.Tools;

/// <summary>
///		Ventana "Acerca de"
/// </summary>
public partial class AboutView : Window
{
	public AboutView(string version)
	{
		InitializeComponent();
		lblVersion.Content = "Versión: " + version;
	}

	private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
	{
		try
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) 
									{ 
										UseShellExecute = true 
									}
						);
		}
		catch {}
		e.Handled = true;
	}
}
