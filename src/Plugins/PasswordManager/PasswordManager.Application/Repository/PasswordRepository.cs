using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.PasswordManager.Application.Models;
using Bau.Libraries.LibCryptography.Cryptography;

namespace Bau.Libraries.PasswordManager.Application.Repository;

/// <summary>
///		Repositorio para un archivo de contraseñas
/// </summary>
internal class PasswordRepository
{
	// Constantes privadas
	private const string TagRoot = "BauPassword";
	private const string TagSalt = "Salt";
	private const string TagContent = "Content";
	private const string TagFolder = "Folder";
	private const string TagEntry = "Entry";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagId = "Id";
	private const string TagUrl = "Url";
	private const string TagNotes = "Notes";
	private const string TagUser = "User";
	private const string TagPassword = "Password";
	private const string TagOtp = "Otp";
	private const string TagKey = "Key";
	private const string TagEncoding = "Encoding";
	private const string TagHashAlgorithm = "HashAlgorithm";
	private const string TagDigits = "Digits";
	private const string TagCounter = "Counter";
	private const string TagInterval = "Interval";
	private const string TagCreatedAt = "CreatedAt";

	/// <summary>
	///		Carga los datos de un archivo
	/// </summary>
	internal FileModel Load(string fileName, string password)
	{
		FileModel file = new();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los datos
			if (fileML != null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
					{
						// Carga la cadena de salto
						if (!string.IsNullOrWhiteSpace(rootML.Attributes[TagSalt].Value))
							file.Salt = rootML.Attributes[TagSalt].Value.TrimIgnoreNull();
						// Carga las carpetas del contenido desencriptado
						LoadEncryptedContent(file, Decrypt(rootML.Nodes[TagContent].Value, password, file.Salt));
					}
			// Devuelve el archivo leido
			return file;
	}

	/// <summary>
	///		Obtiene el controlador para criptografía
	/// </summary>
	private CryptographyController GetCryptographyController(string password, string salt)
	{
		return new CryptographyController(CryptographyController.CryptographyProviders.Aes, password, salt);
	}

	/// <summary>
	///		Carga el contenido encriptado
	/// </summary>
	private void LoadEncryptedContent(FileModel file, string xml)
	{
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().ParseText(xml);

			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							if (nodeML.Name == TagFolder) // ... va hasta la primera carpeta, dentro están las carpetas y entradas reales
								LoadFolders(file.Root, nodeML);
			else
				throw new Exceptions.PasswordFileException("Check the password");
	}

	/// <summary>
	///		Carga las carpetas hija
	/// </summary>
	private void LoadFolders(FolderModel parent, MLNode rootML)
	{
		foreach (MLNode nodeML in rootML.Nodes)
			switch (nodeML.Name)
			{
				case TagFolder:
						parent.Folders.Add(LoadFolder(nodeML));
					break;
				case TagEntry:
						parent.Entries.Add(LoadEntry(nodeML));
					break;
			}
	}

	/// <summary>
	///		Carga los datos de una carpeta
	/// </summary>
	private FolderModel LoadFolder(MLNode rootML)
	{
		FolderModel folder = new();

			// Asigna las propiedades
			folder.GlobalId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			folder.Name = rootML.Attributes[TagName].Value.TrimIgnoreNull();
			// Carga los hijos
			LoadFolders(folder, rootML);
			// Devuelve la carpeta cargada
			return folder;
	}

	/// <summary>
	///		Carga los datos de una entrada
	/// </summary>
	private EntryModel LoadEntry(MLNode rootML)
	{
		EntryModel entry = new();

			// Asigna las propiedades
			entry.GlobalId = rootML.Attributes[TagId].Value.TrimIgnoreNull();
			entry.Name = rootML.Attributes[TagName].Value.TrimIgnoreNull();
			entry.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
			entry.Url = rootML.Attributes[TagUrl].Value.TrimIgnoreNull();
			entry.Notes = rootML.Nodes[TagNotes].Value.TrimIgnoreNull();
			entry.User = rootML.Attributes[TagUser].Value.TrimIgnoreNull();
			entry.Password = rootML.Attributes[TagPassword].Value.TrimIgnoreNull();
			entry.CreatedAt = rootML.Attributes[TagCreatedAt].Value.GetDateTime(DateTime.Now);
			// Carga los datos de Otp
			foreach (MLNode otpML in rootML.Nodes)
				if (otpML.Name == TagOtp)
				{
					entry.AuthKey = otpML.Nodes[TagKey].Value.TrimIgnoreNull();
					entry.Encoding = otpML.Attributes[TagEncoding].Value.GetEnum(EntryModel.SecretEncoding.Plain);
					entry.HashAlgorithm = otpML.Attributes[TagHashAlgorithm].Value.GetEnum(EntryModel.HmacAlgorithm.Sha1);
					entry.Digits = otpML.Attributes[TagDigits].Value.GetInt(6);
					entry.Counter = otpML.Attributes[TagCounter].Value.GetLong(0);
					entry.Interval = otpML.Attributes[TagInterval].Value.GetInt(30);
				}
			// Devuelve la entrada
			return entry;
	}

	/// <summary>
	///		Graba los datos de un archivo
	/// </summary>
	internal void Save(string fileName, FileModel file, string password)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade la clave de salto
			rootML.Attributes.Add(TagSalt, file.Salt);
			rootML.Nodes.Add(TagContent, CreateXmlContent(file, password));
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Crea el contenido XML
	/// </summary>
	private string CreateXmlContent(FileModel file, string password)
	{
		MLNode rootML = GetXmlFolder(file.Root);

			if (rootML.Nodes.Count > 0)
				return Encrypt($"<{TagRoot}>" + new LibMarkupLanguage.Services.XML.XMLWriter().ConvertToString(rootML) + $"</{TagRoot}>", 
							   password, file.Salt);
			else
				return string.Empty;
	}

	/// <summary>
	///		Encripta el contenido XML
	/// </summary>
	private string Encrypt(string xml, string password, string salt) => GetCryptographyController(password, salt).EncryptToBase64(xml);

	/// <summary>
	///		Desencripta una cadena en XML
	/// </summary>
	private string Decrypt(string content, string password, string salt)
	{
		try
		{
			return GetCryptographyController(password, salt).DecryptFromBase64(content);
		}
		catch (Exception exception)
		{
			throw new Exceptions.PasswordFileException("Error when decrypt file. Check the password", exception);
		}
	}

	/// <summary>
	///		Obtiene los nodos de una carpeta
	/// </summary>
	private MLNode GetXmlFolder(FolderModel folder)
	{
		MLNode rootML = new(TagFolder);

			// Añade los datos de la carpeta
			rootML.Attributes.Add(TagId, folder.GlobalId);
			rootML.Attributes.Add(TagName, folder.Name);
			// Crea los nodos de las carpetas hija
			foreach (FolderModel child in folder.Folders)
				rootML.Nodes.Add(GetXmlFolder(child));
			// Añade las entradas
			rootML.Nodes.AddRange(GetXmlEntries(folder));
			// Devuelve el nodo
			return rootML;
	}

	/// <summary>
	///		Obtiene los nodos de una serie de entradas
	/// </summary>
	private MLNodesCollection GetXmlEntries(FolderModel folder)
	{
		MLNodesCollection nodesML = new();

			// Añade las entradas
			foreach (EntryModel entry in folder.Entries)
				nodesML.Add(GetXmlEntry(entry));
			// Devuelve la colección de nodos
			return nodesML;
	}

	/// <summary>
	///		Obtiene los datos del nodo de una entrada
	/// </summary>
	private MLNode GetXmlEntry(EntryModel entry)
	{
		MLNode rootML = new(TagEntry);

			// Añade las propiedades
			rootML.Attributes.Add(TagId, entry.GlobalId);
			rootML.Attributes.Add(TagName, entry.Name);
			rootML.Nodes.Add(TagDescription, entry.Description);
			rootML.Attributes.Add(TagUrl, entry.Url);
			rootML.Nodes.Add(TagNotes, entry.Notes);
			rootML.Attributes.Add(TagUser, entry.User);
			rootML.Attributes.Add(TagPassword, entry.Password);
			rootML.Attributes.Add(TagCreatedAt, entry.CreatedAt);
			// Añade los datos de Otp
			if (!string.IsNullOrEmpty(entry.AuthKey))
			{
				MLNode otpML = rootML.Nodes.Add(TagOtp);

					// Añade los datos de la entrada para OTP
					otpML.Nodes.Add(TagKey, entry.AuthKey);
					otpML.Attributes.Add(TagEncoding, (int) entry.Encoding);
					otpML.Attributes.Add(TagHashAlgorithm, (int) entry.HashAlgorithm);
					otpML.Attributes.Add(TagDigits, entry.Digits);
					otpML.Attributes.Add(TagCounter, entry.Counter);
					otpML.Attributes.Add(TagInterval, entry.Interval);
			}
			// Devuelve el nodo
			return rootML;
	}
}