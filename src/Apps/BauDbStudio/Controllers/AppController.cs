using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;
using Bau.Libraries.BauMvvm.ViewModels.Forms.Dialogs;

namespace Bau.DbStudio.Controllers
{
	/// <summary>
	///		Controlador principal de la aplicación
	/// </summary>
	public class AppController : Libraries.PluginsStudio.ViewModels.Base.Controllers.IAppController
	{
		public AppController(DbStudioViewsManager dbStudioViewManager)
		{
			DbStudioViewManager = dbStudioViewManager;
		}

		/// <summary>
		///		Abre una ventana
		/// </summary>
		public SystemControllerEnums.ResultType OpenWindow(Libraries.PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel detailViewModel)
		{
			// Abre la ventana
			switch (detailViewModel)
			{
				case Libraries.PluginsStudio.ViewModels.Files.ImageViewModel viewModel:
						DbStudioViewManager.AppViewController.OpenDocument(new Views.Files.ImageView(viewModel), viewModel);
					break;
				case Libraries.PluginsStudio.ViewModels.Base.Files.BaseTextFileViewModel viewModel:
						DbStudioViewManager.AppViewController.OpenDocument(new Views.Files.FileTextView(viewModel), viewModel);
					break;
				case Libraries.PluginsStudio.ViewModels.Tools.Web.WebViewModel viewModel:
						DbStudioViewManager.AppViewController.OpenDocument(new Views.Tools.Web.WebExplorerView(viewModel), viewModel);
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
				case Libraries.PluginsStudio.ViewModels.Tools.CreateFileViewModel viewModel:
					return DbStudioViewManager.AppViewController.OpenDialog(new Views.Files.CreateFileView(viewModel));
			}
			// Devuelve el valor predeterminado
			return SystemControllerEnums.ResultType.No;
		}

		/// <summary>
		///		Manager principal
		/// </summary>
		public DbStudioViewsManager DbStudioViewManager { get; }
	}
}
