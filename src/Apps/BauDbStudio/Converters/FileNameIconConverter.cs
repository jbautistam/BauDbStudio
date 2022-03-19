using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace Bau.DbStudio.Converters
{
	/// <summary>
	///		Conversor de iconos a partir del nombre de archivo
	/// </summary>
	public class FileNameIconConverter : IValueConverter
	{
		// Variables estáticas
		private static List<Bau.Libraries.PluginsStudio.ViewModels.Base.Models.FileAssignedModel> _filesAssigned;

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
					else
					{
						string pluginIcon = GetIconFilesAssigned(fileName);

							if (!string.IsNullOrWhiteSpace(pluginIcon))
								icon = pluginIcon;
							else if (fileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileParquet.png";
							else if (fileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileCsv.png";
							else if (fileName.EndsWith(".sqlx", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileSqlExtended.png";
							else if (fileName.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileSql.png";
							else if (fileName.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileJson.png";
							else if (fileName.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileXml.png";
							else if (fileName.EndsWith(".xlsx", StringComparison.CurrentCultureIgnoreCase) ||
									 fileName.EndsWith(".xls", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileExcel.png";
							else if (fileName.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FilePython.png";
							else if (fileName.EndsWith(".ps", StringComparison.CurrentCultureIgnoreCase) ||
									 fileName.EndsWith(".ps1", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FilePowershell.png";
							else if (fileName.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileCsharp.png";
							else if (fileName.EndsWith(".md", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileMd.png";
							else if (fileName.EndsWith(".cmd.xml", StringComparison.CurrentCultureIgnoreCase))
								icon = "/Resources/Images/FileCmd.png";
							else if (IsImage(fileName))
								icon = "/Resources/Images/FileImage.png";
					}
				}
				// Devuelve el icono
				return icon;
		}

		/// <summary>
		///		Comprueba si la extensión de un archivo está asociado a una imagen
		/// </summary>
		private bool IsImage(string fileName)
		{
			string[] extensions = { ".bmp", ".gif", ".png", ".tif", ".tiff", ".jpg", ".jpeg" };

				// Comprueba las extensiones
				foreach (string extension in extensions)
					if (fileName.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
						return true;
				// Si ha llegado hasta aquí es porque no está entre las extensiones
				return false;
		}

		/// <summary>
		///		Obtiene el icono asociado a la extnesión si está asociado a algún plugin
		/// </summary>
		private string GetIconFilesAssigned(string fileName)
		{
			// Comprueba si la extensión está asignada a algún plugin
			foreach (Bau.Libraries.PluginsStudio.ViewModels.Base.Models.FileAssignedModel fileAssigned in FilesAssignedIcons)
				if (fileName.EndsWith(fileAssigned.FileExtension, StringComparison.CurrentCultureIgnoreCase))
					return fileAssigned.Icon;
			// Si ha llegado hasta aquí es porque no está
			return string.Empty;
		}

		/// <summary>
		///		Convierte un valor de vuelta
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{ 
			throw new NotImplementedException();
		}

		/// <summary>
		///		Lista de archivos asignados a los diferentes plugins con sus iconos
		/// </summary>
		private static List<Bau.Libraries.PluginsStudio.ViewModels.Base.Models.FileAssignedModel> FilesAssignedIcons
		{
			get
			{
				// Obtiene los datos si no existían
				if (_filesAssigned == null)
					_filesAssigned = MainWindow.DbStudioViewsManager.PluginsManager.GetFilesAssigned();
				// Devuelve la lista
				return _filesAssigned;
			}
		}
	}
}
