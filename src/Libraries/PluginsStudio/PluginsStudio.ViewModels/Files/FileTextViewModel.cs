using System;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Files
{
	/// <summary>
	///		ViewModel de un archivo de texto
	/// </summary>
	public class FileTextViewModel : Base.Files.BaseTextFileViewModel
	{
		public FileTextViewModel(PluginsStudioViewModel mainViewModel, string fileName) : base(mainViewModel.PluginsStudioController.PluginsController, fileName)
		{
			MainViewModel = mainViewModel;
		}

		/// <summary>
		///		Obtiene el texto asociado a un nodo arrastrado a la pantalla 
		/// </summary>
		public override string GetTextFromDroppedNode(BaseTreeNodeViewModel nodeViewModel, bool pressedShiftKey)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public PluginsStudioViewModel MainViewModel { get; }
	}
}
