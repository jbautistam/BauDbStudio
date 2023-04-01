using System;

namespace Bau.Libraries.PluginsStudio.ViewModels.Files
{
	/// <summary>
	///		ViewModel de un archivo de texto
	/// </summary>
	public class FileTextViewModel : Base.Files.BaseTextFileViewModel
	{
		public FileTextViewModel(PluginsStudioViewModel mainViewModel, string fileName) : base(mainViewModel.PluginsStudioController.PluginsController, fileName)
		{
			MainViewModel = mainViewModel;
		}

		/// <summary>
		///		Obtiene el texto asociado a un nodo arrastrado a la pantalla
		/// </summary>
		public override async Task<string> TreatTextDroppedAsync(string content, bool shiftPressed, CancellationToken cancellationToken)
		{
			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Obtiene el texto asociado a un nombre de archivo
			if (IsFileName(content))
			{
				if (FileName.EndsWith(".md", StringComparison.CurrentCultureIgnoreCase))
					content = GetTextDroppedOnMarkdown(content);
			}
			// Devuelve el contenido
			return content;
		}

		/// <summary>
		///		Obtiene el texto que se debe insertar en un archivo MarkDown
		/// </summary>
		private string GetTextDroppedOnMarkdown(string droppedFile)
		{
			string name = Path.GetFileName(droppedFile);
			string? link = string.Empty;

				// Obtiene la cadena adecuada
				if (LibHelper.Files.HelperFiles.CheckIsImage(droppedFile))
					link = $"![{name}]({droppedFile.Replace('\\', '/')} \"{name}\")";
				else
				{
					// Asigna el nombre de archivo (sin extensión o sin directorio final)
					if (droppedFile.EndsWith("_index.md", StringComparison.CurrentCultureIgnoreCase))
						link = Path.GetDirectoryName(droppedFile);
					else if (droppedFile.EndsWith(".md", StringComparison.CurrentCultureIgnoreCase))
						link = Path.Combine(Path.GetDirectoryName(droppedFile) ?? string.Empty, Path.GetFileNameWithoutExtension(droppedFile));
					else
						link = droppedFile;
					// Asigna el vínculo
					if (!string.IsNullOrWhiteSpace(link))
						link = $"[{name}]({link.Replace('\\', '/')})";
					else
						link = $"[{name}]";
				}
				// Devuelve el vínculo
				return link;
		}

		/// <summary>
		///		Comprueba si un texto se corresponde con un nombre de archivo
		/// </summary>
		private bool IsFileName(string text)
		{
			return !string.IsNullOrWhiteSpace(text) && text.Length < 8_000 && text.IndexOf('\r') < 0 && text.IndexOf('.') >= 0;
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public PluginsStudioViewModel MainViewModel { get; }
	}
}
