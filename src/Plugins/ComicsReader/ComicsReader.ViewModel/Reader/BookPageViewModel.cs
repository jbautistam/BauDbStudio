using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bau.Libraries.ComicsReader.ViewModel.Reader
{
	/// <summary>
	///		ViewModel para la página
	/// </summary>
	public class BookPageViewModel : Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel
	{
		// Variables privadas
		private string _fileName, _thumbFileName;
		private int _page;

		public BookPageViewModel(ComicContentViewModel comicViewModel, string fileName, string thumbFileName, int page) : base(page.ToString(), null)
		{
			ComicViewModel = comicViewModel;
			FileName = fileName;
			ThumbFileName = thumbFileName;
			Page = page;
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public ComicContentViewModel ComicViewModel { get; }

		/// <summary>
		///		Nombre de archivo de imagen
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { CheckProperty(ref _fileName, value); }
		}

		/// <summary>
		///		Nombre del archivo de thumb
		/// </summary>
		public string ThumbFileName
		{
			get { return _thumbFileName; }
			set { CheckProperty(ref _thumbFileName, value); }
		}

		/// <summary>
		///		Número de página
		/// </summary>
		public int Page
		{
			get { return _page; }
			set { CheckProperty(ref _page, value); }
		}
	}
}