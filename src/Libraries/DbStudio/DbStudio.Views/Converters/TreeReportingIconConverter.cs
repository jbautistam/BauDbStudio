using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Explorers;

namespace Bau.Libraries.DbStudio.Views.Converters;

/// <summary>
///		Conversor de iconos
/// </summary>
public class TreeReportingIconConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is string iconName)
			return GetIcon(iconName.GetEnum(TreeReportingViewModel.IconType.Unknown));
		else
			return null;
	}

	/// <summary>
	///		Obtiene la imagen asociada a un icono
	/// </summary>
	private object? GetIcon(TreeReportingViewModel.IconType icon)
	{
		return icon switch
				{
					TreeReportingViewModel.IconType.DataWarehouse => "/DbStudio.Views;component/Resources/Images/Connection.png",
					TreeReportingViewModel.IconType.DataSourceRoot => "/DbStudio.Views;component/Resources/Images/Schema.png",
					TreeReportingViewModel.IconType.DataSourceTable => "/DbStudio.Views;component/Resources/Images/Table.png",
					TreeReportingViewModel.IconType.DataSourceView => "/DbStudio.Views;component/Resources/Images/View.png",
					TreeReportingViewModel.IconType.Key => "/DbStudio.Views;component/Resources/Images/Key.png",
					TreeReportingViewModel.IconType.Field => "/DbStudio.Views;component/Resources/Images/Field.png",
					TreeReportingViewModel.IconType.Error => "/DbStudio.Views;component/Resources/Images/DataError.png",
					TreeReportingViewModel.IconType.DataSourceSql => "/DbStudio.Views;component/Resources/Images/FileSql.png",
					TreeReportingViewModel.IconType.Report => "/DbStudio.Views;component/Resources/Images/Report.png",
					TreeReportingViewModel.IconType.Dimension => "/DbStudio.Views;component/Resources/Images/ReportDimension.png",
					TreeReportingViewModel.IconType.DimensionChild => "/DbStudio.Views;component/Resources/Images/DimensionChild.png",
					TreeReportingViewModel.IconType.Rules => "/DbStudio.Views;component/Resources/Images/File.png",
					TreeReportingViewModel.IconType.Folder => "/DbStudio.Views;component/Resources/Images/FolderNode.png",
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
