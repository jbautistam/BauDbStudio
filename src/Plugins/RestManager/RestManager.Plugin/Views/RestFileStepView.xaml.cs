using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.RestManager.ViewModel.Project.Steps;

namespace Bau.Libraries.RestManager.Plugin.Views;

/// <summary>
///     Control de usuario para mostrar los datos de un paso
/// </summary>
public partial class RestFileStepView : UserControl
{
	// Propiedades de dependencia
	public static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), typeof(RestFileStepItemViewModel),
																						 typeof(RestFileStepView),
																						 new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
																													   new PropertyChangedCallback(Step_PropertyChanged)));

    public RestFileStepView()
    {
        InitializeComponent();
		InitEditor();
    }

	/// <summary>
	///		Inicializa el editor
	/// </summary>
	private void InitEditor()
	{
		// Asigna la configuración al editor
		udtBody.EditorFontName = "";
		udtBody.EditorFontSize = 12;
		udtBody.ShowLinesNumber = true;
		// Asigna el nombre de archivo
		udtBody.Text = Step?.Content ?? string.Empty;
		udtBody.ChangeHighLightByExtension(".json");
		// Asigna el manejador de eventos
		udtBody.TextChanged += (sender, args) => {
													if (DataContext is not null && DataContext is RestFileStepItemViewModel viewModel)
														viewModel.Content = udtBody.Text ?? string.Empty;
												 };
	}

	private static void Step_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (obj is RestFileStepView view && e.NewValue is RestFileStepItemViewModel viewModel)
			view.Step = viewModel;
	}

	/// <summary>
	///		Datos del paso seleccionado
	/// </summary>
	public RestFileStepItemViewModel? Step
	{
		get { return (RestFileStepItemViewModel) GetValue(StepProperty); }
		set 
		{ 
			SetValue(StepProperty, value); 
			DataContext = value;
			lstHeaders.Parameters = value?.Headers;
			udtBody.Text = value?.Content ?? string.Empty;
		}
	}
}
