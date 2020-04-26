using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Cloud.Repository
{
	/// <summary>
	///		Clase para cargar datos de proceso
	/// </summary>
	internal class JobCloudRepository
	{
		// Constantes privadas
		private const string TagRoot = "CloudScript";
		private const string TagBlock = "Block";
		private const string TagMessage = "Message";
		private const string TagSource = "Source";
		private const string TagTarget = "Target";
		private const string TagEnabled = "Enabled";
		private const string TagTimeout = "Timeout";
		private const string TagUploadBlob = "Upload";
		private const string TagDownloadBlob = "Download";
		private const string TagBlobContainer = "Container";
		private const string TagBlobFile = "Blob";
		private const string TagFileName = "FileName";
		private const string TagCopy = "Copy";
		private const string TagTransformFileName = "TransformFileName";
		private const string TagMove = "Move";
		private const string TagFrom = "From";
		private const string TagTo = "To";

		/// <summary>
		///		Carga los datos del proceso
		/// </summary>
		internal List<BaseSentence> Load(string fileName)
		{
			List<BaseSentence> program = new List<BaseSentence>();

				// Carga los datos
				if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(fileName))
				{
					MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

						if (fileML != null)
							foreach (MLNode rootML in fileML.Nodes)
								if (rootML.Name == TagRoot)
									program.AddRange(LoadSentences(rootML));
				}
				// Devuelve las instrucciones
				return program;
		}

		/// <summary>
		///		Carga las sentencias de un nodo
		/// </summary>
		private List<BaseSentence> LoadSentences(MLNode rootML)
		{
			List<BaseSentence> sentences = new List<BaseSentence>();

				// Carga las sentencias
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagBlock:
								sentences.Add(LoadBlockSentence(nodeML));
							break;
						case TagUploadBlob:
								sentences.Add(LoadUploadBlobSentence(nodeML));
							break;
						case TagDownloadBlob:
								sentences.Add(LoadDownloadBlobSentence(nodeML));
							break;
						case TagCopy:
								sentences.Add(LoadCopyBlobSentence(nodeML, false));
							break;
						case TagMove:
								sentences.Add(LoadCopyBlobSentence(nodeML, true));
							break;
					}
				// Devuelve la lista de sentencias
				return sentences;
		}

		/// <summary>
		///		Carga un bloque de sentencias
		/// </summary>
		private BaseSentence LoadBlockSentence(MLNode rootML)
		{
			BlockSentence sentence = new BlockSentence();

				// Asigna las propiedades
				sentence.Enabled = rootML.Attributes[TagEnabled].Value.GetBool(true);
				sentence.Message = rootML.Attributes[TagMessage].Value;
				// Carga las sentencias del bloque
				sentence.Sentences.AddRange(LoadSentences(rootML));
				// Devuelve la sentencia leida
				return sentence;
		}

		/// <summary>
		///		Obtiene el timeout definido en el nodo
		/// </summary>
		private TimeSpan GetTimeout(MLNode rootML, TimeSpan defaultTimeOut)
		{
			TimeSpan? timeout = null;
			string attribute = rootML.Attributes[TagTimeout].Value;

				// Interpreta la cadena de timeout
				if (!string.IsNullOrWhiteSpace(attribute) && attribute.Length > 1)
				{
					string time = attribute.Substring(attribute.Length - 1).ToUpper();
					int value = attribute.Substring(0, attribute.Length - 1).GetInt(0);

						if (value != 0)
							switch (time)
							{
								case "D":
										if (value < 5)
											timeout = TimeSpan.FromDays(value);
										else
											timeout = TimeSpan.FromDays(4);
									break;
								case "H":
										timeout = TimeSpan.FromHours(value);
									break;
								case "M":
										timeout = TimeSpan.FromMinutes(value);
									break;
								case "S":
										timeout = TimeSpan.FromSeconds(value);
									break;
							}
				}
				// Devuelve el timeout de la sentencia
				return timeout ?? defaultTimeOut;
		}

		/// <summary>
		///		Carga la sentencia para subir un archivo
		/// </summary>
		private BaseSentence LoadUploadBlobSentence(MLNode rootML)
		{
			UploadBlobSentence sentence = new UploadBlobSentence();

				// Carga los datos de la sentencia
				AssignBlobSentences(sentence, rootML);
				sentence.StorageKey = rootML.Attributes[TagTarget].Value;
				sentence.Target.Container = rootML.Attributes[TagBlobContainer].Value;
				sentence.Target.Blob = rootML.Attributes[TagBlobFile].Value;
				sentence.FileName = rootML.Attributes[TagFileName].Value;
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga la sentencia para descargar un archivo
		/// </summary>
		private BaseSentence LoadDownloadBlobSentence(MLNode rootML)
		{
			DownloadBlobSentence sentence = new DownloadBlobSentence();

				// Carga los datos de la sentencia
				AssignBlobSentences(sentence, rootML);
				sentence.StorageKey = rootML.Attributes[TagSource].Value;
				sentence.Source.Container = rootML.Attributes[TagBlobContainer].Value;
				sentence.Source.Blob = rootML.Attributes[TagBlobFile].Value;
				sentence.FileName = rootML.Attributes[TagFileName].Value;
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de copia entre blobs
		/// </summary>
		private BaseSentence LoadCopyBlobSentence(MLNode rootML, bool move)
		{
			CopyBlobSentence sentence = new CopyBlobSentence();

				// Carga los datos básicos de la sentencia
				AssignBlobSentences(sentence, rootML);
				// Carga el resto de datos
				sentence.StorageKey = rootML.Attributes[TagSource].Value;
				sentence.Move = move;
				sentence.TransformFileName = rootML.Attributes[TagTransformFileName].Value.GetBool();
				foreach (MLNode nodeML in rootML.Nodes)
					switch (nodeML.Name)
					{
						case TagFrom:
								sentence.Source.Container = nodeML.Attributes[TagBlobContainer].Value;
								sentence.Source.Blob = nodeML.Attributes[TagBlobFile].Value;
							break;
						case TagTo:
								sentence.Target.Container = nodeML.Attributes[TagBlobContainer].Value;
								sentence.Target.Blob = nodeML.Attributes[TagBlobFile].Value;
							break;
					}
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Asigna los datos de una sentencia asociada con un blob
		/// </summary>
		private void AssignBlobSentences(BaseBlobSentence sentence, MLNode rootML)
		{
			sentence.Enabled = rootML.Attributes[TagEnabled].Value.GetBool(true);
			sentence.Timeout = GetTimeout(rootML, TimeSpan.FromMinutes(5));
		}
	}
}
