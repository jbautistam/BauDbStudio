using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Powershell.Models;

namespace Bau.Libraries.LibJobProcessor.Powershell.Repository
{
	/// <summary>
	///		Clase para cargar datos de proceso
	/// </summary>
	internal class JobPowershellRepository
	{
		// Constantes privadas
		private const string TagRoot = "PowershellScript";
		private const string TagExecute = "Execute";
		private const string TagEnabled = "Enabled";
		private const string TagTimeout = "Timeout";
		private const string TagFileName = "FileName";
		private const string TagContent = "Content";
		private const string TagMap = "Map";
		private const string TagFrom = "From";
		private const string TagTo = "To";
		private const string TagPath = "Path";
		private const string TagParameterName = "ParameterName";
		private const string TagValue = "Value";

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
						case TagExecute:
								sentences.Add(LoadScriptSentence(nodeML));
							break;
					}
				// Devuelve la lista de sentencias
				return sentences;
		}

		/// <summary>
		///		Carga una sentencia de ejecución de scripts
		/// </summary>
		private BaseSentence LoadScriptSentence(MLNode rootML)
		{
			ExecuteScriptSentence sentence = new ExecuteScriptSentence();

				// Asigna las propiedades
				AssignProperties(sentence, rootML);
				sentence.FileName = rootML.Attributes[TagFileName].Value.TrimIgnoreNull();
				sentence.Content = rootML.Nodes[TagContent].Value.TrimIgnoreNull();
				// Carga los mapeos
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagMap)
						sentence.Mappings.Add(nodeML.Attributes[TagFrom].Value.TrimIgnoreNull(),
											  nodeML.Attributes[TagTo].Value.TrimIgnoreNull());
				// Carga los directorios
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagPath)
						sentence.Paths.Add(nodeML.Attributes[TagParameterName].Value.TrimIgnoreNull(),
										   nodeML.Attributes[TagValue].Value.TrimIgnoreNull());
				// Devuelve la sentencia leida
				return sentence;
		}

		/// <summary>
		///		Asigna los datos de una sentencia asociada con un blob
		/// </summary>
		private void AssignProperties(BaseSentence sentence, MLNode rootML)
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
