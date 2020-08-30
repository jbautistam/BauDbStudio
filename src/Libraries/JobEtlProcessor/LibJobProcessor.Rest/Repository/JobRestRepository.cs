using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Rest.Repository
{
	/// <summary>
	///		Clase para cargar datos de proceso
	/// </summary>
	internal class JobRestRepository
	{
		// Constantes privadas
		private const string TagRoot = "RestScript";
		private const string TagBlock = "Block";
		private const string TagEnabled = "Enabled";
		private const string TagTimeout = "Timeout";
		private const string TagMessage = "Message";
		private const string TagCallApiSentence = "CallApi";
		private const string TagUrl = "Url";
		private const string TagUser = "User";
		private const string TagPassword = "Password";
		private const string TagMethod = "Method";
		private const string TagType = "Type";
		private const string TagEndPoint = "EndPoint";
		private const string TagBody = "Body";
		private const string TagResult = "Result";
		private const string TagFrom = "From";
		private const string TagTo = "To";
		private const string TagException = "Exception";

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
						case TagCallApiSentence:
								sentences.Add(LoadApiDefinitionSentence(nodeML));
							break;
						case TagException:
								sentences.Add(LoadExceptionSentence(nodeML));
							break;
					}
				// Devuelve la lista de sentencias
				return sentences;
		}

		/// <summary>
		///		Carga los datos de una excepción
		/// </summary>
		private BaseSentence LoadExceptionSentence(MLNode rootML)
		{
			ExceptionSentence sentence = new ExceptionSentence();

				// Asigna las propiedades
				AssignDefaultProperties(sentence, rootML);
				if (string.IsNullOrWhiteSpace(rootML.Attributes[TagMessage].Value))
					sentence.Message = rootML.Value.TrimIgnoreNull();
				else
					sentence.Message = rootML.Attributes[TagMessage].Value.TrimIgnoreNull();
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga un bloque de sentencias
		/// </summary>
		private BaseSentence LoadBlockSentence(MLNode rootML)
		{
			BlockSentence sentence = new BlockSentence();

				// Asigna las propiedades
				AssignDefaultProperties(sentence, rootML);
				sentence.Message = rootML.Attributes[TagMessage].Value;
				// Carga las sentencias del bloque
				sentence.Sentences.AddRange(LoadSentences(rootML));
				// Devuelve la sentencia leida
				return sentence;
		}

		/// <summary>
		///		Carga una sentencia de deifinición de API
		/// </summary>
		private BaseSentence LoadApiDefinitionSentence(MLNode rootML)
		{
			CallApiSentence sentence = new CallApiSentence();

				// Asigna las propiedades
				AssignDefaultProperties(sentence, rootML);
				sentence.Url = rootML.Attributes[TagUrl].Value.TrimIgnoreNull();
				sentence.User = rootML.Attributes[TagUser].Value.TrimIgnoreNull();
				sentence.Password = rootML.Attributes[TagPassword].Value.TrimIgnoreNull();
				// Obtiene los métodos
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagMethod)
						sentence.Methods.Add(LoadMethod(nodeML));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga los datos de un método
		/// </summary>
		private CallApiMethodSentence LoadMethod(MLNode rootML)
		{
			CallApiMethodSentence sentence = new CallApiMethodSentence();

				// Asigna las propiedades
				AssignDefaultProperties(sentence, rootML);
				sentence.EndPoint = rootML.Attributes[TagEndPoint].Value.TrimIgnoreNull();
				sentence.Method = rootML.Attributes[TagType].Value.GetEnum(CallApiMethodSentence.MethodType.Unkwnown);
				sentence.Body = rootML.Nodes[TagBody].Value.TrimIgnoreNull();
				// Obtiene los resultados
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagResult)
						sentence.Results.Add(LoadResult(nodeML));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Carga la sentencia de tratamiento de resultados
		/// </summary>
		private CallApiResultSentence LoadResult(MLNode rootML)
		{
			CallApiResultSentence sentence = new CallApiResultSentence();

				// Obtiene los resultados
				sentence.ResultFrom = rootML.Attributes[TagFrom].Value.GetInt(0);
				sentence.ResultTo = rootML.Attributes[TagTo].Value.GetInt(0);
				// Carga las sentencias
				sentence.Sentences.AddRange(LoadSentences(rootML));
				// Devuelve la sentencia
				return sentence;
		}

		/// <summary>
		///		Asigna los datos básicos de una sentencia
		/// </summary>
		private void AssignDefaultProperties(BaseSentence sentence, MLNode rootML)
		{
			sentence.Enabled = rootML.Attributes[TagEnabled].Value.GetBool(true);
			sentence.Timeout = GetTimeout(rootML, TimeSpan.FromMinutes(5));
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
	}
}
