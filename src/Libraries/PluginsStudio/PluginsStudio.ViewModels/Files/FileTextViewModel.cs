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
		public override string TreatTextDropped(string content, bool shiftPressed)
		{
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
			string name = System.IO.Path.GetFileName(droppedFile);

				// Obtiene la cadena adecuada
				if (LibHelper.Files.HelperFiles.CheckIsImage(droppedFile))
					return $"![{name}]({droppedFile.Replace('\\', '/')} \"{name}\")";
				else
					return $"[{name}]({System.IO.Path.Combine(System.IO.Path.GetDirectoryName(droppedFile), System.IO.Path.GetFileNameWithoutExtension(droppedFile)).Replace('\\', '/')})";
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
