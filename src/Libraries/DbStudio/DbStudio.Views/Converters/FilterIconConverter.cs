using System.Windows.Data;

namespace Bau.Libraries.DbStudio.Views.Converters;

/// <summary>
///		Conversor de iconos para el filtro activo o no
/// </summary>
public class FilterIconConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is bool hasFilter)
		{
			if (hasFilter)
				return "/DbStudio.Views;component/Resources/Images/FilterSelected.png";
			else
				return "/DbStudio.Views;component/Resources/Images/Filter.png";
		}
		else
			return null;
	}

	/// <summary>
	///		Convierte un valor de vuelta
	/// </summary>
	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
