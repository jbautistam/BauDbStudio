using Markdig;

namespace Bau.Libraries.FileTools.ViewModel.Processor;

/// <summary>
///		ViewModel para ejecutar un archivo de comandos
/// </summary>
public class MarkdownProcessor
{
	/// <summary>
	///		Interpreta el HTML a partir de un archivo Markdown
	/// </summary>
	public string ParseFile(string fileName)
	{
        if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
			return ParseText(LibHelper.Files.HelperFiles.LoadTextFile(fileName));
		else
			return string.Empty;
	}

	/// <summary>
	///		Interpreta el HTML a partir del Markdown
	/// </summary>
	public string ParseText(string text)
	{
        if (!string.IsNullOrWhiteSpace(text))
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

                // Convierte el texto a HTML
                return Markdown.ToHtml(text, pipeline);
        }
		else
			return string.Empty;
	}
}
