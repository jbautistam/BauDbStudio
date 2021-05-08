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
					return "/DbStudio.Views;component/Resources/Images/Connection.png";
				case TreeConnectionsViewModel.IconType.Schema:
					return "/DbStudio.Views;component/Resources/Images/Schema.png";
				case TreeConnectionsViewModel.IconType.Deployment:
					return "/DbStudio.Views;component/Resources/Images/Deployment.png";
				case TreeConnectionsViewModel.IconType.Storage:
					return "/DbStudio.Views;component/Resources/Images/Storage.png";
				case TreeConnectionsViewModel.IconType.Project:
					return "/DbStudio.Views;component/Resources/Images/Project.png";
				case TreeConnectionsViewModel.IconType.Path:
					return "/DbStudio.Views;component/Resources/Images/FolderNode.png";
				case TreeConnectionsViewModel.IconType.File:
					return "/DbStudio.Views;component/Resources/Images/File.png";
				case TreeConnectionsViewModel.IconType.Table:
					return "/DbStudio.Views;component/Resources/Images/Table.png";
				case TreeConnectionsViewModel.IconType.View:
					return "/DbStudio.Views;component/Resources/Images/View.png";
				case TreeConnectionsViewModel.IconType.Key:
					return "/DbStudio.Views;component/Resources/Images/Key.png";
				case TreeConnectionsViewModel.IconType.Field:
					return "/DbStudio.Views;component/Resources/Images/Field.png";
				case TreeConnectionsViewModel.IconType.Error:
					return "/DbStudio.Views;component/Resources/Images/DataError.png";
				case TreeConnectionsViewModel.IconType.Loading:
					return "/DbStudio.Views;component/Resources/Images/Loading.png";
				case TreeConnectionsViewModel.IconType.DataSourceSql:
					return "/DbStudio.Views;component/Resources/Images/FileSql.png";
				case TreeConnectionsViewModel.IconType.Report:
					return "/DbStudio.Views;component/Resources/Images/Report.png";
				case TreeConnectionsViewModel.IconType.Dimension:
					return "/DbStudio.Views;component/Resources/Images/ReportDimension.png";
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
