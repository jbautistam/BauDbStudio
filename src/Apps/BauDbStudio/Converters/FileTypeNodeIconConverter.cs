using System.Windows.Data;
using Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

namespace Bau.DbStudio.Converters;

/// <summary>
///		Conversor de iconos a partir del nombre de archivo
/// </summary>
public class FileTypeNodeIconConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is TreeFilesViewModel.NodeType nodeType)
			return nodeType switch
					{
						TreeFilesViewModel.NodeType.BookmarksRoot => GetUriApplicationImage("Bookmarks.png"),
						_ => GetUriApplicationImage("Computer.png")
					};
		else
			return null;

		// Obtiene la URL de carga del icono desde un recurso de la aplicación
		string GetUriApplicationImage(string icon) => $"pack://application:,,,/BauDbStudio;component/Resources/Images/{icon}";
	}

	/// <summary>
	///		Convierte un valor de vuelta
	/// </summary>
	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		throw new NotImplementedException();
	}
}
