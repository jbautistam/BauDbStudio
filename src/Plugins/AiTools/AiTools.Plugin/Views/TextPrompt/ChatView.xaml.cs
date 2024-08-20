using System.Windows.Controls;

namespace Bau.Libraries.AiTools.Plugin.Views.TextPrompt;

/// <summary>
///		Vista de chat
/// </summary>
public partial class ChatView : UserControl
{
	// Variables privadas
	private bool _isInitialized;

	public ChatView(ViewModels.TextPrompt.ChatViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	private async Task InitControlAsync()
	{
		if (!_isInitialized)
		{
			await ViewModel.InitAsync(CancellationToken.None);
			ViewModel.PropertyChanged += (sender, args) => {
																if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																		args.PropertyName.Equals(nameof(ViewModel.SelectedItem), StringComparison.CurrentCultureIgnoreCase))
																	lstChat.ScrollIntoView(ViewModel.SelectedItem);
															};
			_isInitialized = true;
		}
	}

	private async void UserControl_Initialized(object sender, System.Windows.RoutedEventArgs e)
	{
		await InitControlAsync();
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ViewModels.TextPrompt.ChatViewModel ViewModel { get; }
}