using System;
using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.PluginsStudio.ViewModels.Explorers.Files;

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
			if (value is string iconName)
				return GetIcon(iconName.GetEnum(TreeFilesViewModel.IconType.Unknown));
			else
				return null;
		}

		/// <summary>
		///		Obtiene la imagen asociada a un icono
		/// </summary>
		private object GetIcon(TreeFilesViewModel.IconType icon)
		{
			switch (icon)
			{ 
				case TreeFilesViewModel.IconType.Connection:
					return "/Resources/Images/Connection.png";
				case TreeFilesViewModel.IconType.Schema:
					return "/Resources/Images/Schema.png";
				case TreeFilesViewModel.IconType.Deployment:
					return "/Resources/Images/Deployment.png";
				case TreeFilesViewModel.IconType.Storage:
					return "/Resources/Images/Storage.png";
				case TreeFilesViewModel.IconType.Project:
					return "/Resources/Images/Project.png";
				case TreeFilesViewModel.IconType.Path:
					return "/Resources/Images/FolderNode.png";
				case TreeFilesViewModel.IconType.File:
					return "/Resources/Images/File.png";
				case TreeFilesViewModel.IconType.Table:
					return "/Resources/Images/Table.png";
				case TreeFilesViewModel.IconType.View:
					return "/Resources/Images/View.png";
				case TreeFilesViewModel.IconType.Key:
					return "/Resources/Images/Key.png";
				case TreeFilesViewModel.IconType.Field:
					return "/Resources/Images/Field.png";
				case TreeFilesViewModel.IconType.Error:
					return "/Resources/Images/DataError.png";
				case TreeFilesViewModel.IconType.Loading:
					return "/Resources/Images/Loading.png";
				case TreeFilesViewModel.IconType.DataSourceSql:
					return "/Resources/Images/FileSql.png";
				case TreeFilesViewModel.IconType.Report:
					return "/Resources/Images/Report.png";
				case TreeFilesViewModel.IconType.Dimension:
					return "/Resources/Images/ReportDimension.png";
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
