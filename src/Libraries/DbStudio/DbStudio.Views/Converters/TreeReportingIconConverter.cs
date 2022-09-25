using System;
using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

namespace Bau.Libraries.DbStudio.Views.Converters
{
	/// <summary>
	///		Conversor de iconos
	/// </summary>
	public class TreeReportingIconConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un tipo en un icono
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			if (value is string iconName)
				return GetIcon(iconName.GetEnum(TreeReportingViewModel.IconType.Unknown));
			else
				return null;
		}

		/// <summary>
		///		Obtiene la imagen asociada a un icono
		/// </summary>
		private object GetIcon(TreeReportingViewModel.IconType icon)
		{
			switch (icon)
			{ 
				case TreeReportingViewModel.IconType.DataWarehouse:
					return "/DbStudio.Views;component/Resources/Images/Connection.png";
				case TreeReportingViewModel.IconType.DataSourceRoot:
					return "/DbStudio.Views;component/Resources/Images/Schema.png";
				case TreeReportingViewModel.IconType.DataSourceTable:
					return "/DbStudio.Views;component/Resources/Images/Table.png";
				case TreeReportingViewModel.IconType.DataSourceView:
					return "/DbStudio.Views;component/Resources/Images/View.png";
				case TreeReportingViewModel.IconType.Key:
					return "/DbStudio.Views;component/Resources/Images/Key.png";
				case TreeReportingViewModel.IconType.Field:
					return "/DbStudio.Views;component/Resources/Images/Field.png";
				case TreeReportingViewModel.IconType.Error:
					return "/DbStudio.Views;component/Resources/Images/DataError.png";
				case TreeReportingViewModel.IconType.DataSourceSql:
					return "/DbStudio.Views;component/Resources/Images/FileSql.png";
				case TreeReportingViewModel.IconType.Report:
					return "/DbStudio.Views;component/Resources/Images/Report.png";
				case TreeReportingViewModel.IconType.Dimension:
					return "/DbStudio.Views;component/Resources/Images/ReportDimension.png";
				case TreeReportingViewModel.IconType.Folder:
					return "/DbStudio.Views;component/Resources/Images/FolderNode.png";
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
