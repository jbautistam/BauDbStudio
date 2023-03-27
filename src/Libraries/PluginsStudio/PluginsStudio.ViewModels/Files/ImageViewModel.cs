namespace Bau.Libraries.PluginsStudio.ViewModels.Files
{
	/// <summary>
	///		ViewModel para un archivo de imagen
	/// </summary>
	public class ImageViewModel : Base.Files.BaseFileViewModel
	{
		public ImageViewModel(PluginsStudioViewModel mainViewModel, string fileName) : base(fileName)
		{
			MainViewModel = mainViewModel;
			FileName = fileName;
			IsUpdated = false;
		}

		/// <summary>
		///		Carga el arcchivo (en este caso se carga directamente en la vista)
		/// </summary>
		public override void Load()
		{
			IsUpdated = false;
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public override void SaveDetails(bool newName)
		{
			IsUpdated = false;
		}

		/// <summary>
		///		Cierra el viewmodel
		/// </summary>
		public override void Close()
		{
			// ... no hace nada, sólo implementa la interface
		}

		/// <summary>
		///		Solución
		/// </summary>
		public PluginsStudioViewModel MainViewModel { get; }
	}
}
