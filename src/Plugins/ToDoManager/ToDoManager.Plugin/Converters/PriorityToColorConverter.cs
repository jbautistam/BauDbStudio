using System.Windows.Data;
using System.Windows.Media;

using Bau.Libraries.ToDoManager.Application.ToDo.Models;

namespace Bau.Libraries.ToDoManager.Plugin.Converters;

/// <summary>
///		Conversor de prioridad a color
/// </summary>
internal class PriorityToColorConverter : IValueConverter
{
	/// <summary>
	///		Convierte una prioridad en un color
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is TaskModel.PriorityType priority)
		{
			return priority switch
					{
						TaskModel.PriorityType.Normal => new SolidColorBrush(Colors.Yellow),
						TaskModel.PriorityType.Low => new SolidColorBrush(Colors.Olive),
						_ => new SolidColorBrush(Colors.Red),
					};
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

