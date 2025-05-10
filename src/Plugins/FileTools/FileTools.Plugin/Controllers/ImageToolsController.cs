using Microsoft.Extensions.Logging;

namespace Bau.Libraries.FileTools.Plugin.Controllers;

/// <summary>
///		Controlador para las herramientas de imagen
/// </summary>
public class ImageToolsController(ViewModel.Controllers.IFileToolsController fileToolsController) : ViewModel.Controllers.IImageToolsController
{
    /// <summary>
    ///     Divide una imagen en diferentes archivos
    /// </summary>
    public void Split(string sourceFileName, string targetFolder, string targetFileName, int rows, int columns)
	{
		try
		{
			new Tools.SplitImagesController().Split(sourceFileName, targetFolder, targetFileName, rows, columns);
		}
		catch (Exception exception)
		{
			FileToolsController.Logger.LogError(exception, $"Error when split file {sourceFileName}. {exception.Message}");
		}
	}

    /// <summary>
    ///     Redimensiona una imagen sobre un archivo
    /// </summary>
    public void Resize(string sourceFileName, string targetFileName, int width, int height)
	{
		try
		{
			new Tools.ResizeImagesController().Resize(sourceFileName, targetFileName, width, height);
		}
		catch (Exception exception)
		{
			FileToolsController.Logger.LogError(exception, $"Error when resize file {sourceFileName}. {exception.Message}");
		}
	}
	/// <summary>
	///		Controlador principal de herramientas de archivos
	/// </summary>
	public ViewModel.Controllers.IFileToolsController FileToolsController { get; } = fileToolsController;
}