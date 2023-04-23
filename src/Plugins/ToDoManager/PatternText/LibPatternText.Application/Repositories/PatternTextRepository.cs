using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibPatternText.Models;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibPatternText.Application.Repositories;

/// <summary>
///		Repositorio para los archivos de patrones
/// </summary>
internal class PatternTextRepository
{
	// Constantes privadas
	private const string TagRoot = "Patterns";
	private const string TagSource = "Source";
	private const string TagWithHeader = "WithHeader";
	private const string TagSeparator = "Separator";
	private const string TagQuotes = "Quotes";
	private const string TagFormula = "Formula";

	/// <summary>
	///		Carga los datos de un archivo
	/// </summary>
	internal PatternModel Load(string fileName)
	{
		PatternModel pattern = new();
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Si hay algo que leer
			if (fileML is not null)
				foreach	(MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagFormula:
										pattern.Formula = nodeML.Value.TrimIgnoreNull();
									break;
								case TagSource:
										pattern.Source = nodeML.Value.TrimIgnoreNull();
										pattern.WithHeader = nodeML.Attributes[TagWithHeader].Value.GetBool(true);
										pattern.Separator = nodeML.Attributes[TagSeparator].Value.TrimIgnoreNull();
										pattern.QuoteChar = nodeML.Attributes[TagQuotes].Value.TrimIgnoreNull();
										if (string.IsNullOrEmpty(pattern.Separator))
											pattern.Separator = ",";
										if (string.IsNullOrEmpty(pattern.QuoteChar))
											pattern.QuoteChar = "\"";
									break;
							}
			// Devuelve el patrón leído
			return pattern;
	}

	/// <summary>
	///		Graba un patrón en un archivo
	/// </summary>
	internal void Save(string fileName, PatternModel pattern)
	{
		MLFile fileML = new();
		MLNode rootML = fileML.Nodes.Add(TagRoot);
		MLNode nodeML = rootML.Nodes.Add(TagSource, pattern.Source);
			
			// Añade los datos del origen
			nodeML.Attributes.Add(TagWithHeader, pattern.WithHeader);
			nodeML.Attributes.Add(TagSeparator, pattern.Separator);
			nodeML.Attributes.Add(TagQuotes, pattern.QuoteChar);
			// Añade la fórmula
			rootML.Nodes.Add(TagFormula, pattern.Formula);
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}
}
