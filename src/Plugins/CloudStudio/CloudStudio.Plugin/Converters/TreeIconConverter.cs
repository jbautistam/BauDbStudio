using System;
using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.CloudStudio.ViewModels.Explorers.Cloud;

namespace Bau.Libraries.CloudStudio.Plugin.Converters
{
	/// <summary>
	///		Conversor de iconos
	/// </summary>
	public class TreeIconConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un tipo en un icono
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			if (value is string iconName)
				return GetIcon(iconName.GetEnum(TreeStorageViewModel.IconType.Unknown));
			else
				return null;
		}

		/// <summary>
		///		Obtiene la imagen asociada a un icono
		/// </summary>
		private object GetIcon(TreeStorageViewModel.IconType icon)
		{
			switch (icon)
			{ 
				case TreeStorageViewModel.IconType.Storage:
					return "/CloudStudio.Views;component/Resources/Images/Storage.png";
				case TreeStorageViewModel.IconType.Folder:
					return "/CloudStudio.Views;component/Resources/Images/FolderNode.png";
				case TreeStorageViewModel.IconType.File:
					return "/CloudStudio.Views;component/Resources/Images/File.png";
				case TreeStorageViewModel.IconType.Error:
					return "/CloudStudio.Views;component/Resources/Images/DataError.png";
				case TreeStorageViewModel.IconType.Loading:
					return "/CloudStudio.Views;component/Resources/Images/Loading.png";
				default:
					return null;
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
