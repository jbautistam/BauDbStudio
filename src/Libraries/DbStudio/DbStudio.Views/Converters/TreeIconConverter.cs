using System;
using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;

namespace Bau.Libraries.DbStudio.Views.Converters
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
				return GetIcon(iconName.GetEnum(TreeConnectionsViewModel.IconType.Unknown));
			else
				return null;
		}

		/// <summary>
		///		Obtiene la imagen asociada a un icono
		/// </summary>
		private object GetIcon(TreeConnectionsViewModel.IconType icon)
		{
			switch (icon)
			{ 
				case TreeConnectionsViewModel.IconType.Connection:
					return "/Resources/Images/Connection.png";
				case TreeConnectionsViewModel.IconType.Schema:
					return "/Resources/Images/Schema.png";
				case TreeConnectionsViewModel.IconType.Deployment:
					return "/Resources/Images/Deployment.png";
				case TreeConnectionsViewModel.IconType.Storage:
					return "/Resources/Images/Storage.png";
				case TreeConnectionsViewModel.IconType.Project:
					return "/Resources/Images/Project.png";
				case TreeConnectionsViewModel.IconType.Path:
					return "/Resources/Images/FolderNode.png";
				case TreeConnectionsViewModel.IconType.File:
					return "/Resources/Images/File.png";
				case TreeConnectionsViewModel.IconType.Table:
					return "/Resources/Images/Table.png";
				case TreeConnectionsViewModel.IconType.View:
					return "/Resources/Images/View.png";
				case TreeConnectionsViewModel.IconType.Key:
					return "/Resources/Images/Key.png";
				case TreeConnectionsViewModel.IconType.Field:
					return "/Resources/Images/Field.png";
				case TreeConnectionsViewModel.IconType.Error:
					return "/Resources/Images/DataError.png";
				case TreeConnectionsViewModel.IconType.Loading:
					return "/Resources/Images/Loading.png";
				case TreeConnectionsViewModel.IconType.DataSourceSql:
					return "/Resources/Images/FileSql.png";
				case TreeConnectionsViewModel.IconType.Report:
					return "/Resources/Images/Report.png";
				case TreeConnectionsViewModel.IconType.Dimension:
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
