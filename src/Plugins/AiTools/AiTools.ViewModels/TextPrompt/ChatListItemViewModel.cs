using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.AiTools.ViewModels.TextPrompt;

/// <summary>
///		Elemento de la lista <see cref="ChatListViewModel"/>
/// </summary>
public class ChatListItemViewModel : BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
{
	/// <summary>
	///		Origen del mensaje
	/// </summary>
	public enum SourceType
	{
		/// <summary>Humano</summary>
		Human,
		/// <summary>Inteligencia artificial</summary>
		ArtificialIntelligence,
		/// <summary>Mensaje de error del sistema</summary>
		Error
	}

	// Variables privadas
	private SourceType _source;
	private bool _canCopy;
	private MvvmColor _background = default!;

	public ChatListItemViewModel(ChatViewModel chatListViewModel, SourceType source, string text, bool isBold = false) : base(text, null, isBold)
	{
		ChatListViewModel = chatListViewModel;
		Source = source;
		CanCopy = source == SourceType.ArtificialIntelligence;
		CopyCommand = new BaseCommand(_ => Copy());
	}

	/// <summary>
	///		Copia el texto del elemento en el portapapeles
	/// </summary>
	private void Copy()
	{
		ChatListViewModel.MainViewModel.ViewsController.MainWindowController.CopyToClipboard(Text);
	}

	/// <summary>
	///		ViewModel de la lista
	/// </summary>
	public ChatViewModel ChatListViewModel { get; }

	/// <summary>
	///		Origen del mensaje
	/// </summary>
	public SourceType Source
	{
		get { return _source; }
		set 
		{ 
			if (CheckProperty(ref _source, value))
				switch (_source)
				{
					case SourceType.Human:
							Background = MvvmColor.AntiqueWhite;
							Foreground = MvvmColor.Black;
						break;
					case SourceType.ArtificialIntelligence:
							Background = MvvmColor.PaleVioletRed;
							Foreground = MvvmColor.Navy;
						break;
					case SourceType.Error:
							Background = MvvmColor.PaleVioletRed;
							Foreground = MvvmColor.Red;
						break;
				}
		}
	}

	/// <summary>
	///		Color de fondo
	/// </summary>
	public MvvmColor Background
	{
		get { return _background; }
		set { CheckObject(ref _background, value); }
	}

	/// <summary>
	///		Indica si se puede copiar el texto
	/// </summary>
	public bool CanCopy
	{
		get { return _canCopy; }
		set { CheckProperty(ref _canCopy, value); }
	}

	/// <summary>
	///		Comando para copiar el texto
	/// </summary>
	public BaseCommand CopyCommand { get; }
}
