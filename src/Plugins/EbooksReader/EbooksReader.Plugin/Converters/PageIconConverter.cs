using System;
using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
namespace Bau.Libraries.EbooksReader.Plugin.Converters
{
	/// <summary>
	///		Conversor de iconos a partir del tipo de página
	/// </summary>
	public class PageIconConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un tipo en un icono
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			return GetIcon((value?.ToString()).GetEnum(ViewModel.Reader.Explorer.TreeEbookViewModel.NodeType.Unknown));
		}

		/// <summary>
		///		Obtiene la imagen asociada a un nodo
		/// </summary>
		private object GetIcon(ViewModel.Reader.Explorer.TreeEbookViewModel.NodeType type)
		{
			switch (type)
			{
				case ViewModel.Reader.Explorer.TreeEbookViewModel.NodeType.Page:
					return "/EbooksReader.Plugin;component/Resources/File.png";
				default:
					return "/EbooksReader.Plugin;component/Resources/FolderNode.png";
			}
		}

		/// <summary>
		///		Convierte un valor de vuelta
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			throw new NotImplementedException();
		}
	}
}
