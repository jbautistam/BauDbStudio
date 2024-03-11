using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.AiTools.Application.Models.Chat;

namespace Bau.Libraries.AiTools.ViewModels.TextPrompt;

/// <summary>
///		ViewModel principal para el chat con Ollama
/// </summary>
public class ChatViewModel : BauMvvm.ViewModels.BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Constantes privadas
	private const string DefaultFileName = $"NewFile{AiToolsViewModel.ExtensionAiChatFile}";
	private const string Mask = $"Chat bot files (*{AiToolsViewModel.ExtensionAiChatFile})|*{AiToolsViewModel.ExtensionAiChatFile}|All files (*.*)|*.*";
	// Variables privadas
	private SynchronizationContext? _contextUi = SynchronizationContext.Current;
	private ControlItemCollectionViewModel<ChatListItemViewModel> chatViewModel = default!;
	private ChatListItemViewModel? _selectedItem;
	private string _fileName = default!;
	private string? _prompt;
	private ChatListItemViewModel? _actualAiChat;
	private bool _canHumanSpeak;
	private ComboViewModel _comboModels = default!;

	public ChatViewModel(AiToolsViewModel mainViewModel, string fileName)
	{
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
		Header = Path.GetFileName(fileName);
		ChatItemsViewModel = new ControlItemCollectionViewModel<ChatListItemViewModel>();
		LoadComboModels();
		// Inicializa los comandos
		SendCommand = new BauMvvm.ViewModels.BaseCommand(async _ => await SendAsync(CancellationToken.None), _ => CanHumanSpeak)
								.AddListener(this, nameof(CanHumanSpeak));
	}

	/// <summary>
	///		Inicializa la conversación con Ollama
	/// </summary>
	public async Task InitAsync(CancellationToken cancellationToken)
	{
		// Limpia la lista
		ChatItemsViewModel.Clear();
		// Carga los datos del archivo
		foreach (ChatModel chat in new Application.PromptGenerator().LoadChat(FileName))
			ChatItemsViewModel.Add(new ChatListItemViewModel(this, Convert(chat.Type), chat.Message));
		// Prepara el manager
		await PrepareManagerAsync(cancellationToken);
		// Indica que no ha habido modificaciones
		IsUpdated = false;

		// Convierte los tipos
		ChatListItemViewModel.SourceType Convert(ChatModel.SourceType type)
		{
			return type switch
					{
						ChatModel.SourceType.Human => ChatListItemViewModel.SourceType.Human,
						ChatModel.SourceType.ArtificialIntelligence => ChatListItemViewModel.SourceType.ArtificialIntelligence,
						_ => ChatListItemViewModel.SourceType.Error
					};
		}
	}

	/// <summary>
	///		Prepara el manager de Ollama
	/// </summary>
	private async Task PrepareManagerAsync(CancellationToken cancellationToken)
	{
		if (OllamaManager is null)
		{
			// Crea el manager
			OllamaManager = new Controller.OllamaChatController(MainViewModel.ConfigurationViewModel.OllamaUrl, 
																TimeSpan.FromMinutes(MainViewModel.ConfigurationViewModel.OllamaTimeout));
			// Conecta
			if (!await OllamaManager.ConnectAsync(cancellationToken))
			{
				// Añade el error
				AddChat(ChatListItemViewModel.SourceType.Error, "Error when connect to Ollama");
				// Anula el manager
				OllamaManager = null;
			}
			else
			{
				// Asigna el manejador de eventos
				OllamaManager.ChatReceived += (sender, args) => TreatOllamaResponse(args.Message, args.IsEnd);
				// Indica que se traten la respuesta según se reciba
				OllamaManager.TreatResponseAsStream = true;
				// Carga los modelos
				Models.Clear();
				foreach (LibOllama.Api.Models.ListModelsResponseItem item in OllamaManager.Models)
					Models.Add(item.Name);
				// Carga el combo de modelos
				LoadComboModels();
			}
		}
	}

	/// <summary>
	///		Carga el combo de modelos
	/// </summary>
	private void LoadComboModels()
	{
		// Crea el combo
		ComboModels = new ComboViewModel(this);
		// Añade los elementos
		if (Models.Count == 0)
			ComboModels.AddItem(-1, "No models loaded");
		else
			foreach (string model in Models)
				ComboModels.AddItem(Models.IndexOf(model), model);
		// Asigna el manejador de eventos
		ComboModels.PropertyChanged += (sender, args) =>
											{
												if (!string.IsNullOrWhiteSpace(args.PropertyName) && args.PropertyName.Equals(nameof(ComboModels.SelectedItem)))
													UpdateModel();
											};
		// Selecciona el primer elemento
		ComboModels.SelectedItem = ComboModels.Items[0];
	}

	/// <summary>
	///		Modifica el modelo
	/// </summary>
	private void UpdateModel()
	{
		int modelId = ComboModels.SelectedId ?? -1;

			// Cambia el modelo (el manager va a devolver un mensaje que se va a tratar como un mensaje de chat normal)
			if (modelId >= 0 && modelId < Models.Count && OllamaManager is not null)
				OllamaManager.UpdateModel(Models[modelId]);
	}

	/// <summary>
	///		Trata la respuesta de Ollama: añade el texto al último elemento de AI
	/// </summary>
	private void TreatOllamaResponse(string message, bool isEnd)
	{
		object state = new object();

			// Muestra la respuesta
			//? La respuesta de Ollama viene en otro hilo
			if (_contextUi is not null)
				_contextUi.Send(_ => AddAiChat(message, isEnd), state);
	}

	/// <summary>
	///		Añade el caht de la IA
	/// </summary>
	private void AddAiChat(string message, bool isEnd)
	{
		// Crea un nuevo elemento de chat
		if (_actualAiChat is null)
			_actualAiChat = AddChat(ChatListItemViewModel.SourceType.ArtificialIntelligence, string.Empty);
		// Si tenemos algo, añadimos el mensaje
		if (_actualAiChat is not null)
		{
			// Añade el mensaje
			_actualAiChat.Text += message;
			// Si se ha terminado, se vacía el chat
			if (isEnd)
			{
				// Vacía el mensaje de la AI
				_actualAiChat = null;
				// E indica si el humano puede hablar
				UpdateCanHumanSpeak();
			}
		}
	}

	/// <summary>
	///		Envía el texto del prompt a Ollama
	/// </summary>
	private async Task SendAsync(CancellationToken cancellationToken)
	{
		if (!string.IsNullOrWhiteSpace(Prompt))
		{
			string promptToSend = Prompt;

				// Vacía el prompt (antes de llamar a Ollama para que se vea ya vacío en la interface de usuario)
				Prompt = null;
				// Envía el mensaje
				AddChat(ChatListItemViewModel.SourceType.Human, promptToSend);
				// Llama a Ollama para obtener la respuesta
				if (OllamaManager is not null)
					try
					{
						await Task.Run(() => OllamaManager.PromptAsync(promptToSend, cancellationToken), cancellationToken);
					}
					catch (Exception exception)
					{
						AddChat(ChatListItemViewModel.SourceType.Error, $"Error when call Ollama. {exception.Message}");
					}
		}
	}

	/// <summary>
	///		Añade un elemento de chat y selecciona el último elemento
	/// </summary>
	private ChatListItemViewModel AddChat(ChatListItemViewModel.SourceType source, string text)
	{
		// Añade el elemento
		ChatItemsViewModel.Add(new ChatListItemViewModel(this, source, text));
		// Selecciona el elemento
		SelectedItem = ChatItemsViewModel[ChatItemsViewModel.Count - 1];
		// y lo devuelve
		return SelectedItem;
	}

	/// <summary>
	///		Indica si el humano puede lanzar una conversación o no
	/// </summary>
	private void UpdateCanHumanSpeak()
	{
		CanHumanSpeak = !string.IsNullOrWhiteSpace(Prompt) && _actualAiChat is null;
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			return "Do you want to save the file before continuing?";
		else
			return $"Do you want to save the file '{Path.GetFileName(FileName)}' before continuing?";
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		string? fileName;

			// Si hay que cambiar el nombre de archivo
			if (string.IsNullOrWhiteSpace(FileName) || newName)
				fileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave(null, Mask, DefaultFileName, AiToolsViewModel.ExtensionAiChatFile);
			else
				fileName = FileName;
			// Graba el archivo
			if (!string.IsNullOrWhiteSpace(fileName))
				Save(fileName);
	}

	/// <summary>
	///		Graba un archivo
	/// </summary>
	private void Save(string fileName)
	{
		// Graba el archivo
		new Application.PromptGenerator().SaveChat(GetChats(), fileName);
		// Guarda el nombre de archivo
		FileName = fileName;
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Obtiene la lista de chat
	/// </summary>
	private List<ChatModel> GetChats()
	{
		List<ChatModel> chats = new();

			// Convierte los datos
			foreach (ChatListItemViewModel item in ChatItemsViewModel)
				chats.Add(new ChatModel
								{
									Type = Convert(item.Source),
									Message = item.Text
								}
						 );
			// Devuelve la lista de chats
			return chats;

			// Convierte los tipos
			ChatModel.SourceType Convert(ChatListItemViewModel.SourceType type)
			{
				return type switch
						{
							ChatListItemViewModel.SourceType.Human => ChatModel.SourceType.Human,
							ChatListItemViewModel.SourceType.ArtificialIntelligence => ChatModel.SourceType.ArtificialIntelligence,
							_ => ChatModel.SourceType.Error
						};
			}
	}

	/// <summary>
	///		Cierra la ventana de detalles
	/// </summary>
	public void Close()
	{
		ChatItemsViewModel.Clear();
	}

	/// <summary>
	///		Manager de Ollama
	/// </summary>
	private Controller.OllamaChatController? OllamaManager { get; set; }

	/// <summary>
	///		Modelos cargados
	/// </summary>
	private List<string> Models { get; } = new();

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public AiToolsViewModel MainViewModel { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Id de la ficha en pantalla
	/// </summary>
	public string TabId => GetType().ToString() + "_" + FileName;

	/// <summary>
	///		ViewModel del chat
	/// </summary>
	public ControlItemCollectionViewModel<ChatListItemViewModel> ChatItemsViewModel
	{
		get { return chatViewModel; }
		set { CheckObject(ref chatViewModel, value); }
	}

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public ChatListItemViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Modelos cargados en Ollama
	/// </summary>
	public ComboViewModel ComboModels
	{
		get { return _comboModels; }
		set { CheckObject(ref _comboModels, value); }
	}

	/// <summary>
	///		Prompt
	/// </summary>
	public string? Prompt 
	{ 
		get { return _prompt; } 
		set 
		{ 
			if (CheckProperty(ref _prompt, value))
				UpdateCanHumanSpeak();
		}
	}

	/// <summary>
	///		Indica si el humano puede lanzar un mensaje
	/// </summary>
	public bool CanHumanSpeak
	{
		get { return _canHumanSpeak; }
		set { CheckProperty(ref _canHumanSpeak, value); }
	}

	/// <summary>
	///		Comando de envío
	/// </summary>
	public BauMvvm.ViewModels.BaseCommand SendCommand { get; }
}
