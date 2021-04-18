using System;

using Bau.Libraries.BauMvvm.ViewModels.Controllers;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Controllers
{
	/// <summary>
	///		Controlador de aplicación
	/// </summary>
	public interface IAppController
	{
		/// <summary>
		///		Abre una ventana de detalles
		/// </summary>
		SystemControllerEnums.ResultType OpenWindow(Interfaces.IDetailViewModel detailsViewModel);

		/// <summary>
		///		Abre un cuadro de diálogo
		/// </summary>
		SystemControllerEnums.ResultType OpenDialog(BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel dialogViewModel);
	}
}
