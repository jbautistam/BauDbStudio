using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.AiTools.Application.Models.Chat;

namespace Bau.Libraries.AiTools.Application.Repository;

/// <summary>
///		Repositorio de <see cref="ChatModel"/>
/// </summary>
internal class ChatRepository
{
	// Constantes privadas
	private const string TagRoot = "Chats";
	private const string TagChat = "Chat";
	private const string TagSource = "Source";

	/// <summary>
	///		Carga los datos de chat de un archivo
	/// </summary>
	internal List<ChatModel> Load(string fileName)
	{
		List<ChatModel> chats = new();
		MLFile? fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los datos
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							if (nodeML.Name == TagChat)
								chats.Add(LoadChat(nodeML));
			// Devuelve la lista de chats
			return chats;
	}

	/// <summary>
	///		Carga los datos de chat
	/// </summary>
	private ChatModel LoadChat(MLNode rootML)
	{
		return new ChatModel()
						{
							Type = rootML.Attributes[TagSource].Value.GetEnum(ChatModel.SourceType.Human),
							Message = rootML.Value.TrimIgnoreNull()
						};
	}

	/// <summary>
	///		Graba los mensajes de chat en un archivo
	/// </summary>
	internal void Save(string fileName, List<ChatModel> chats)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los datos de chat
			foreach (ChatModel chat in chats)
				rootML.Nodes.Add(GetNode(chat));
			// Graba los datos
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Obtiene los datos de un nodo de chat
	/// </summary>
	private MLNode GetNode(ChatModel chat)
	{
		MLNode rootML = new(TagChat);

			// Añade los datos
			rootML.Attributes.Add(TagSource,  chat.Type.ToString());
			rootML.Value = chat.Message;
			// Devuelve el nodo
			return rootML;
	}
}
