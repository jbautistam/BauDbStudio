using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibPatternText.Models;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibPatternText.Repositories;

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
	private const string TagExtensionHighlight = "Extension";
	private const string SeparatorTab = "Tab";
	private const string SeparatorSpace = "Space";

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
										if (!string.IsNullOrWhiteSpace(pattern.Separator))
										{
											if (pattern.Separator.Equals(SeparatorTab, StringComparison.CurrentCultureIgnoreCase))
												pattern.Separator = "\t";
											else if (pattern.Separator.Equals(SeparatorSpace, StringComparison.CurrentCultureIgnoreCase))
												pattern.Separator = " ";
										}
										pattern.QuoteChar = nodeML.Attributes[TagQuotes].Value.TrimIgnoreNull();
										pattern.ExtensionHighlight = nodeML.Attributes[TagExtensionHighlight].Value.TrimIgnoreNull();
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
			if (!string.IsNullOrEmpty(pattern.Separator))
			{
				if (pattern.Separator == "\t")
					nodeML.Attributes.Add(TagSeparator, SeparatorTab);
				else if (pattern.Separator == " ")
					nodeML.Attributes.Add(TagSeparator, SeparatorSpace);
				else
					nodeML.Attributes.Add(TagSeparator, pattern.Separator);
			}
			nodeML.Attributes.Add(TagQuotes, pattern.QuoteChar);
			nodeML.Attributes.Add(TagExtensionHighlight, pattern.ExtensionHighlight);
			// Añade la fórmula
			rootML.Nodes.AddCData(TagFormula, pattern.Formula);
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}
}