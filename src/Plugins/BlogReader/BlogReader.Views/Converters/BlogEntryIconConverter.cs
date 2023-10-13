using System.Windows.Data;

using Bau.Libraries.LibBlogReader.Model;

namespace Bau.Libraries.BlogReader.Views.Views.Converters;

/// <summary>
///		Conversor para los iconos asociados a las entradas de un blog (leído, no leído, interesante)
/// </summary>
public class BlogEntryIconConverter : IValueConverter
{
	// Varaiables privadas
	private static BauMvvm.Views.Wpf.Tools.ImagesCache _cache = new();

	/// <summary>
	///		Convierte un elemento <see cref="LibBlogReader.ViewModel.Blogs.BlogEntryViewModel"/> en el icono asociado
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		string icon = string.Empty;

			// Convierte la entrada según el estado en el icono asociado
			if (value is EntryModel.StatusEntry status)
				switch (status)
				{
					case EntryModel.StatusEntry.Interesting:
							icon = GetIconRoute("EntryInteresting.png");
						break;
					case EntryModel.StatusEntry.NotRead:
							icon = GetIconRoute("EntryNotRead.png");
						break;
					case EntryModel.StatusEntry.Read:
							icon = GetIconRoute("EntryRead.png");
						break;
				}
			// Devuelve la dirección del icono
			return _cache.GetImage(icon, true);
	}

	/// <summary>
	///		Obtiene la ruta del icono
	/// </summary>
	private string GetIconRoute(string icon) => $"pack://application:,,,/BlogReader.Views;component/Resources/{icon}";

	/// <summary>
	///		Convierte el valor devuelto (no hace nada, simplemente implementa la interface)
	/// </summary>
	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => value;
}
