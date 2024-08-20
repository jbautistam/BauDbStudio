using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbStudio.ViewModels.Explorers.Connections;

namespace Bau.Libraries.DbStudio.Views.Converters;

/// <summary>
///		Conversor de iconos para el árbol de conexiones
/// </summary>
public class TreeConnectionIconConverter : IValueConverter
{
	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is string iconName)
			return GetIcon(iconName.GetEnum(TreeConnectionsViewModel.IconType.Unknown));
		else
			return null;
	}

	/// <summary>
	///		Obtiene la imagen asociada a un icono
	/// </summary>
	private object? GetIcon(TreeConnectionsViewModel.IconType icon)
	{
		return icon switch
				{
					TreeConnectionsViewModel.IconType.Connection => "/DbStudio.Views;component/Resources/Images/Connection.png",
					TreeConnectionsViewModel.IconType.Schema => "/DbStudio.Views;component/Resources/Images/Schema.png",
					TreeConnectionsViewModel.IconType.Project => "/DbStudio.Views;component/Resources/Images/Project.png",
					TreeConnectionsViewModel.IconType.Table => "/DbStudio.Views;component/Resources/Images/Table.png",
					TreeConnectionsViewModel.IconType.View => "/DbStudio.Views;component/Resources/Images/View.png",
					TreeConnectionsViewModel.IconType.Key => "/DbStudio.Views;component/Resources/Images/Key.png",
					TreeConnectionsViewModel.IconType.Field => "/DbStudio.Views;component/Resources/Images/Field.png",
					TreeConnectionsViewModel.IconType.Error => "/DbStudio.Views;component/Resources/Images/DataError.png",
					TreeConnectionsViewModel.IconType.Loading => "/DbStudio.Views;component/Resources/Images/Loading.png",
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
