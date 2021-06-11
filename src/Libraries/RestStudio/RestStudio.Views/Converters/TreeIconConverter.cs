using System;
using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.RestStudio.ViewModels.Explorers;

namespace Bau.Libraries.RestStudio.Views.Converters
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
				return GetIcon(iconName.GetEnum(TreeRestApiViewModel.IconType.Unknown));
			else
				return null;
		}

		/// <summary>
		///		Obtiene la imagen asociada a un icono
		/// </summary>
		private object GetIcon(TreeRestApiViewModel.IconType icon)
		{
			switch (icon)
			{ 
				case TreeRestApiViewModel.IconType.RestApi:
					return "/RestStudio.Views;component/Resources/Images/RestApi.png";
				case TreeRestApiViewModel.IconType.ContextsRoot:
				case TreeRestApiViewModel.IconType.Context:
					return "/RestStudio.Views;component/Resources/Images/Context.png";
				case TreeRestApiViewModel.IconType.MethodsRoot:
				case TreeRestApiViewModel.IconType.Method:
					return "/RestStudio.Views;component/Resources/Images/Method.png";
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
