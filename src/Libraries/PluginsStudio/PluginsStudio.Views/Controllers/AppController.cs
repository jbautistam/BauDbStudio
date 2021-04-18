using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;

namespace Bau.Libraries.PluginsStudio.Views.Controllers
{
	/// <summary>
	///		Controlador de plugins principal
	/// </summary>
	public class AppController : ViewModels.Base.Controllers.IAppController
	{
		public AppController(PluginsStudioViewManager pluginsStudioViewManager)
		{
			PluginsStudioViewManager = pluginsStudioViewManager;
		}

		/// <summary>
		///		Abre una ventana
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(ViewModels.Base.Interfaces.IDetailViewModel detailViewModel)
		{
			// Abre la ventana
			switch (detailViewModel)
			{
				case ViewModels.Files.ImageViewModel viewModel:
						PluginsStudioViewManager.AppViewController.OpenDocument(new Files.ImageView(viewModel), viewModel);
					break;
				case ViewModels.Base.Files.BaseTextFileViewModel viewModel:
						PluginsStudioViewManager.AppViewController.OpenDocument(new Files.FileTextView(viewModel), viewModel);
					break;
			}
			// Devuelve el resultado
			return SystemControllerEnums.ResultType.Yes;
		}

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		public SystemControllerEnums.ResultType OpenDialog(BaseDialogViewModel dialogViewModel)
		{
			// Muestra el cuadro de diálogo
			switch (dialogViewModel)
			{
				case ViewModels.Tools.CreateFileViewModel viewModel:
					return PluginsStudioViewManager.AppViewController.OpenDialog(new Files.CreateFileView(viewModel));
			}
			// Devuelve el valor predeterminado
			return SystemControllerEnums.ResultType.No;
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		public PluginsStudioViewManager PluginsStudioViewManager { get; }
	}
}
