using System;
using System.Windows.Data;

namespace Bau.DbStudio.Converters
{
	/// <summary>
	///		Conversor de iconos a partir del nombre de archivo
	/// </summary>
	public class FileNameIconConverter : IValueConverter
	{
		/// <summary>
		///		Convierte un tipo en un icono
		/// </summary>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			if (value is string fileName)
				return GetIcon(fileName);
			else
				return null;
		}

		/// <summary>
		///		Obtiene la imagen asociada a un icono
		/// </summary>
		private object GetIcon(string fileName)
		{
			string icon = "/Resources/Images/File.png";

				// Obtiene el icono dependiendo de la extensión del archivo
				if (!string.IsNullOrWhiteSpace(fileName))
				{
					if (System.IO.Directory.Exists(fileName))
						icon = "/Resources/Images/FolderNode.png";
					else if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
						icon = "/Resources/Images/FileParquet.png";
					else if (fileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
						icon = "/Resources/Images/FileCsv.png";
					else if (fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
						icon = "/Resources/Images/FileSql.png";
					else if (fileName.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
						icon = "/Resources/Images/FileJson.png";
					else if (fileName.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
						icon = "/Resources/Images/FileXml.png";
					else if (fileName.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase))
						icon = "/Resources/Images/FilePython.png";
					else if (fileName.EndsWith(".ps", StringComparison.CurrentCultureIgnoreCase) ||
							 fileName.EndsWith(".ps1", StringComparison.CurrentCultureIgnoreCase))
						icon = "/Resources/Images/FilePowershell.png";
					else if (fileName.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
						icon = "/Resources/Images/FileCsharp.png";
				}
				// Devuelve el icono
				return icon;
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
