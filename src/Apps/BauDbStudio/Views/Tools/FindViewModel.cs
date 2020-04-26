using System;
using System.Collections.Generic;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.DbStudio.Views.Tools
{
	/// <summary>
	///		Controlador para buscar y reemplazar
	/// </summary>
	public class FindViewModel : BaseObservableObject
	{
		/// <summary>
		///		Elemento encontrado
		/// </summary>
		private class FoundItem
		{
			private string FileName { get; set; }
		}

		// Variables privadas
		private string _textToFind, _textToReplace;
		private bool _caseSensitive, _wholeWord, _useRegex, _useWildCards;

		public FindViewModel(MainWindow mainWindow)
		{
			FindNextCommand = new BaseCommand(_ => SearchNext(true));
			FindPreviousCommand = new BaseCommand(_ => SearchNext(false));
			ReplaceCommand = new BaseCommand(_ => Replace());
			ReplaceAllCommand = new BaseCommand(_ => ReplaceAll());
		}

		/// <summary>
		///		Abre el cuadro de búsqueda
		/// </summary>
		public void Open(Files.FileDetailsView fileView)
		{
			// Guarda la ventana
			FileDetails = fileView;
			// Asigna las propiedades
			if (!string.IsNullOrWhiteSpace(FileDetails.GetSelectedText()))
				TextToFind = FileDetails.GetSelectedText();
			// Indica que está abierta
			IsOpened = true;
		}

		/// <summary>
		///		Busca el siguiente elemento
		/// </summary>
		private void SearchNext(bool searchUpToDown)
		{
			if (FileDetails != null && 
					!FileDetails.SearchNext(TextToFind, searchUpToDown, CaseSensitive, WholeWord, UseRegex, UseWildcards))
				System.Media.SystemSounds.Beep.Play();
		}

		/// <summary>
		///		Reemplaza el texto buscado
		/// </summary>
		private void Replace()
		{
			if (FileDetails != null && 
					FileDetails.Replace(TextToFind, TextToReplace, CaseSensitive, WholeWord, UseRegex, UseWildcards))
				SearchNext(true);
		}

		/// <summary>
		///		Reemplaza todas las coincidencias
		/// </summary>
		private void ReplaceAll()
		{
			if (FileDetails != null)
				FileDetails.ReplaceAll(TextToFind, TextToReplace, CaseSensitive, WholeWord, UseRegex, UseWildcards);
		}

		/// <summary>
		///		Limpia la búsqueda
		/// </summary>
		public void Clear()
		{
			FoundItems.Clear();
			OpenedViews.Clear();
		}

		/// <summary>
		///		Texto a buscar
		/// </summary>
		public string TextToFind 
		{ 
			get { return _textToFind; }
			set { CheckProperty(ref _textToFind, value); }
		}

		/// <summary>
		///		Texto a reemplazar
		/// </summary>
		public string TextToReplace
		{ 
			get { return _textToReplace; }
			set { CheckProperty(ref _textToReplace, value); }
		}

		/// <summary>
		///		Tener en cuenta las mayúsculas
		/// </summary>
		public bool CaseSensitive
		{ 
			get { return _caseSensitive; }
			set { CheckProperty(ref _caseSensitive, value); }
		}

		/// <summary>
		///		Buscar la palabra completa
		/// </summary>
		public bool WholeWord
		{ 
			get { return _wholeWord; }
			set { CheckProperty(ref _wholeWord, value); }
		}

		/// <summary>
		///		Utiliza expresiones regulares en la búsqueda
		/// </summary>
		public bool UseRegex
		{ 
			get { return _useRegex; }
			set { CheckProperty(ref _useRegex, value); }
		}

		/// <summary>
		///		Utilizar wildcards
		/// </summary>
		public bool UseWildcards
		{ 
			get { return _useWildCards; }
			set { CheckProperty(ref _useWildCards, value); }
		}

		/// <summary>
		///		Indica si el cuadro de diálogo está abierto
		/// </summary>
		public bool IsOpened { get; set; }

		/// <summary>
		///		Vista de archivos activa
		/// </summary>
		private Files.FileDetailsView FileDetails { get; set; }

		/// <summary>
		///		Lista de elementos encontrados
		/// </summary>
		private List<FoundItem> FoundItems { get; } = new List<FoundItem>();

		/// <summary>
		///		Vistas de archivo abiertas
		/// </summary>
		private List<Files.FileDetailsView> OpenedViews { get; } = new List<Files.FileDetailsView>();

		/// <summary>
		///		Busca la siguiente coincidencia
		/// </summary>
		public BaseCommand FindNextCommand { get; }

		/// <summary>
		///		Busca la coincidencia anterior
		/// </summary>
		public BaseCommand FindPreviousCommand { get; }

		/// <summary>
		///		Reemplaza el texto
		/// </summary>
		public BaseCommand ReplaceCommand { get; }

		/// <summary>
		///		Reemplaza todas las coincidencias
		/// </summary>
		public BaseCommand ReplaceAllCommand { get; }
	}
}
