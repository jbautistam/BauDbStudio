using System.Windows;
using System.Windows.Data;

using Bau.Libraries.AiTools.ViewModels.TextPrompt;

namespace Bau.Libraries.AiTools.Plugin.Converters;

/// <summary>
///		Conversor de <see cref="ChatListItemViewModel.SourceType"/> a margen
/// </summary>
public class SourceMarginConverter : IValueConverter
{
	/// <summary>
	///		Convierte un <see cref="ChatListItemViewModel.SourceType"/> en margen
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		// Convierte el origen en margen
		if (value is ChatListItemViewModel.SourceType source)
			switch (source)
			{
				case ChatListItemViewModel.SourceType.ArtificialIntelligence:
					return new Thickness(150, 5, 5, 5);
				case ChatListItemViewModel.SourceType.Human:
					return new Thickness(5, 5, 150, 5);
			}
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Convierte el valor de vuelta
	/// </summary>
	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}