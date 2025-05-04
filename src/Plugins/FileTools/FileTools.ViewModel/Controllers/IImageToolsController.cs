namespace Bau.Libraries.FileTools.ViewModel.Controllers;

/// <summary>
///     Interface para las herramientas de tratamiento de imágenes
/// </summary>
public interface IImageToolsController
{
    /// <summary>
    ///     Divide una imagen en diferentes archivos
    /// </summary>
    void Split(string sourceFileName, string folder, string targetFileName, int rows, int columns);
}
