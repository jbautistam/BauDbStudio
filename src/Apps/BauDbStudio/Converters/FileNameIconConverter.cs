using System.Windows.Data;

namespace Bau.DbStudio.Converters;

/// <summary>
///		Conversor de iconos a partir del nombre de archivo
/// </summary>
public class FileNameIconConverter : IValueConverter
{
	// Variables estáticas
	private static List<Libraries.PluginsStudio.ViewModels.Base.Models.FileAssignedModel>? _filesAssigned;
	private static Libraries.BauMvvm.Views.Wpf.Tools.ImagesCache _cache = new();

	/// <summary>
	///		Convierte un tipo en un icono
	/// </summary>
	public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{ 
		if (value is string fileName)
			return GetIcon(fileName);
		else
			return null;
	}

	/// <summary>
	///		Obtiene la imagen asociada a un icono
	/// </summary>
	private object? GetIcon(string fileName)
	{
		string icon = GetUriApplicationImage("File.png");

			// Obtiene el icono dependiendo de la extensión del archivo
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				if (System.IO.Directory.Exists(fileName))
					icon = GetUriApplicationImage("FolderNode.png");
				else
				{
					string pluginIcon = GetIconFilesAssigned(fileName);

						if (!string.IsNullOrWhiteSpace(pluginIcon))
							icon = GetUriPluginImage(pluginIcon);
						else if (fileName.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FileJson.png");
						else if (fileName.EndsWith(".xml", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FileXml.png");
						else if (fileName.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FilePython.png");
						else if (fileName.EndsWith(".ps", StringComparison.CurrentCultureIgnoreCase) ||
								 fileName.EndsWith(".ps1", StringComparison.CurrentCultureIgnoreCase) ||
								 fileName.EndsWith(".ps2", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FilePowershell.png");
						else if (fileName.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FileCsharp.png");
						else if (fileName.EndsWith(".md", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FileMd.png");
						else if (fileName.EndsWith(".pdf", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FilePdf.png");
						else if (fileName.EndsWith(".txt", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FileTxt.png");
						else if (fileName.EndsWith(".css", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FileCss.png");
						else if (fileName.EndsWith(".htm", StringComparison.CurrentCultureIgnoreCase) ||
								 fileName.EndsWith(".html", StringComparison.CurrentCultureIgnoreCase))
							icon = GetUriApplicationImage("FileHtml.png");
						else if (IsImage(fileName))
							icon = GetUriApplicationImage("FileImage.png");
				}
			}
			// Devuelve el icono
			return _cache.GetImage(icon, true);
	}

	/// <summary>
	///		Obtiene la URL de carga del icono desde un recurso de la aplicación
	/// </summary>
	private string GetUriApplicationImage(string icon) => $"pack://application:,,,/BauDbStudio;component/Resources/Images/{icon}";

	/// <summary>
	///		Obtiene la URL de carga del icono desde un recurso de un plugin.
	///		Por ejemplo: /EbooksReader.Plugin;component/Resources/FileEpub.png
	/// </summary>
	private string GetUriPluginImage(string icon)
	{
		if (!string.IsNullOrWhiteSpace(icon) && !icon.StartsWith("pack://"))
			return $"pack://application:,,,{icon}";
		else
			return icon;
	}

	/// <summary>
	///		Comprueba si la extensión de un archivo está asociado a una imagen
	/// </summary>
	private bool IsImage(string fileName)
	{
		string[] extensions = { ".bmp", ".gif", ".png", ".tif", ".tiff", ".jpg", ".jpeg", ".webp" };

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
		foreach (Libraries.PluginsStudio.ViewModels.Base.Models.FileAssignedModel fileAssigned in FilesAssignedIcons)
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
	private static List<Libraries.PluginsStudio.ViewModels.Base.Models.FileAssignedModel> FilesAssignedIcons
	{
		get
		{
			// Obtiene los datos si no existían
			if (_filesAssigned is null)
				_filesAssigned = MainWindow.DbStudioViewsManager.PluginsManager.GetFilesAssigned();
			// Devuelve la lista
			return _filesAssigned;
		}
	}
}
