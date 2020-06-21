using System;
using System.Windows.Data;

using Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers;

namespace Bau.DbStudio.Converters
{
	/// <summary>
	///		Conversor de iconos
	/// </summary>
	public class FileIconConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un tipo en un icono
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			if (value is BaseTreeNodeViewModel.IconType icon)
				return GetIcon(icon);
			else
				return null;
		}

		/// <summary>
		///		Obtiene la imagen asociada a un icono
		/// </summary>
		private object GetIcon(BaseTreeNodeViewModel.IconType icon)
		{
			switch (icon)
			{ 
				case BaseTreeNodeViewModel.IconType.Connection:
					return "/Resources/Images/Connection.png";
				case BaseTreeNodeViewModel.IconType.Schema:
					return "/Resources/Images/Schema.png";
				case BaseTreeNodeViewModel.IconType.Deployment:
					return "/Resources/Images/Deployment.png";
				case BaseTreeNodeViewModel.IconType.Storage:
					return "/Resources/Images/Storage.png";
				case BaseTreeNodeViewModel.IconType.Project:
					return "/Resources/Images/Project.png";
				case BaseTreeNodeViewModel.IconType.Path:
					return "/Resources/Images/FolderNode.png";
				case BaseTreeNodeViewModel.IconType.File:
					return "/Resources/Images/File.png";
				case BaseTreeNodeViewModel.IconType.Table:
					return "/Resources/Images/Table.png";
				case BaseTreeNodeViewModel.IconType.Key:
					return "/Resources/Images/Key.png";
				case BaseTreeNodeViewModel.IconType.Field:
					return "/Resources/Images/Field.png";
				case BaseTreeNodeViewModel.IconType.Error:
					return "/Resources/Images/DataError.png";
				case BaseTreeNodeViewModel.IconType.Loading:
					return "/Resources/Images/Loading.png";
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
