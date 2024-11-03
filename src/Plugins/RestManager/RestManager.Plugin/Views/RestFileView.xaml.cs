using System.Windows.Controls;

using Bau.Libraries.RestManager.ViewModel.Project;
using Bau.Libraries.RestManager.ViewModel.Project.Steps;

namespace Bau.Libraries.RestManager.Plugin.Views;

/// <summary>
///		Vista para edición de un archivo Rest
/// </summary>
public partial class RestFileView : UserControl
{
	public RestFileView(RestFileViewModel viewModel)
	{
		// Inicializa los componentes
		InitializeComponent();
		// Asigna el contexto
		DataContext = ViewModel = viewModel;
		// Asigna los manejadores de eventos
		ViewModel.PropertyChanged += (sender, args) => 
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName))
												{
													//if (args.PropertyName.Equals(nameof(ViewModel.LogText), StringComparison.CurrentCultureIgnoreCase))
													//	udtLog.Text = viewModel.LogText;
													//else if (args.PropertyName.Equals(nameof(ViewModel.FileText), StringComparison.CurrentCultureIgnoreCase))
													//	udtEditor.Text = ViewModel.FileText;
												}
											};
		ViewModel.StepsViewModel.PropertyChanged += (sender, args) =>
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName))
												{
													if (args.PropertyName.Equals(nameof(RestFileListStepsViewModel.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
														udtStep.Step = ViewModel.StepsViewModel.SelectedItem;
												}
											};
		// Selecciona el primer paso en el control
		udtStep.Step = ViewModel.StepsViewModel.SelectedItem;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public RestFileViewModel ViewModel { get; }
}
