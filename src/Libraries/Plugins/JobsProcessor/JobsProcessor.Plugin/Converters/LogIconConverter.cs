using System;
using System.Windows.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.JobsProcessor.ViewModel.Processor.LogTree;

namespace Bau.Libraries.JobsProcessor.Plugin.Converters
{
	/// <summary>
	///		Conversor de iconos a partir del tipo de página
	/// </summary>
	public class LogIconConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un tipo en un icono
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			return GetIcon((value?.ToString()).GetEnum(TreeLogViewModel.NodeType.Unknown));
		}

		/// <summary>
		///		Obtiene la imagen asociada a un nodo
		/// </summary>
		private object GetIcon(TreeLogViewModel.NodeType type)
		{
			switch (type)
			{
				case TreeLogViewModel.NodeType.Command:
					return "/JobsProcessor.Plugin;component/Resources/FileBat.png";
				case TreeLogViewModel.NodeType.Parameter:
					return "/JobsProcessor.Plugin;component/Resources/Parameter.png";
				case TreeLogViewModel.NodeType.Text:
					return "/JobsProcessor.Plugin;component/Resources/Log.png";
				default:
					return "/JobsProcessor.Plugin;component/Resources/FolderNode.png";
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
