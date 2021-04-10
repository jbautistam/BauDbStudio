using System;

namespace Bau.Libraries.RestStudio.ViewModels
{
	/// <summary>
	///		ViewModel para RestStudio
	/// </summary>
	public class RestStudioViewModel
	{
		public RestStudioViewModel(Controllers.IRestStudioController restController)
		{
			// Asigna las propiedades
			RestController = restController;
			// Inicializa la solución
			SolutionViewModel = new(this);
		}

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.IRestStudioController RestController { get; }

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public Solutions.SolutionViewModel SolutionViewModel { get; }
	}
}
