using System.Windows.Data;

using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.DbStudio.Views.Converters;

/// <summary>
///		Conversor de iconos para el tipo de ordenación
/// </summary>
public class SortIconConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is ColumnRequestModel.SortOrder sort)
			return GetIcon(sort);
		else
			return null;
	}

	/// <summary>
	///		Obtiene la imagen asociada a la ordenación
	/// </summary>
	private object GetIcon(ColumnRequestModel.SortOrder sort)
	{
		return sort switch
				{
					ColumnRequestModel.SortOrder.Ascending => "/DbStudio.Views;component/Resources/Images/SortAscending.png",
					ColumnRequestModel.SortOrder.Descending => "/DbStudio.Views;component/Resources/Images/SortDescending.png",
					_ => "/DbStudio.Views;component/Resources/Images/NoSort.png",
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
