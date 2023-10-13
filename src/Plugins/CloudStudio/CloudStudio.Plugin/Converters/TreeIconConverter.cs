using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.CloudStudio.ViewModels.Explorers.Cloud;

namespace Bau.Libraries.CloudStudio.Plugin.Converters;

/// <summary>
///		Conversor de iconos
/// </summary>
public class TreeIconConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is string iconName)
			return GetIcon(iconName.GetEnum(TreeStorageViewModel.IconType.Unknown));
		else
			return null;
	}

	/// <summary>
	///		Obtiene la imagen asociada a un icono
	/// </summary>
	private object? GetIcon(TreeStorageViewModel.IconType icon)
	{
		return icon switch
					{
						TreeStorageViewModel.IconType.Storage => "/CloudStudio.Views;component/Resources/Images/Storage.png",
						TreeStorageViewModel.IconType.Folder => "/CloudStudio.Views;component/Resources/Images/FolderNode.png",
						TreeStorageViewModel.IconType.File => "/CloudStudio.Views;component/Resources/Images/File.png",
						TreeStorageViewModel.IconType.Error => "/CloudStudio.Views;component/Resources/Images/DataError.png",
						TreeStorageViewModel.IconType.Loading => "/CloudStudio.Views;component/Resources/Images/Loading.png",
						_ => null,
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
