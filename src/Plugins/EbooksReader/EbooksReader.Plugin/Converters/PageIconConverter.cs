using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.EbooksReader.ViewModel.Reader.eBooks.Explorer;

namespace Bau.Libraries.EbooksReader.Plugin.Converters;

/// <summary>
///		Conversor de iconos a partir del tipo de página
/// </summary>
public class PageIconConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object Convert(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		return GetIcon((value?.ToString() ?? string.Empty).GetEnum(TreeEbookViewModel.NodeType.Unknown));
	}

	/// <summary>
	///		Obtiene la imagen asociada a un nodo
	/// </summary>
	private object GetIcon(TreeEbookViewModel.NodeType type)
	{
		return type switch
				{
					TreeEbookViewModel.NodeType.Page => "/EbooksReader.Plugin;component/Resources/File.png",
					_ => "/EbooksReader.Plugin;component/Resources/FolderNode.png",
				};
	}

	/// <summary>
	///		Convierte un valor de vuelta
	/// </summary>
	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		throw new NotImplementedException();
	}
}
