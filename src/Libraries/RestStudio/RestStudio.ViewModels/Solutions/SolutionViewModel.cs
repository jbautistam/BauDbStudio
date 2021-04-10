using System;

using Bau.Libraries.RestStudio.Application;
using Bau.Libraries.RestStudio.Models;

namespace Bau.Libraries.RestStudio.ViewModels.Solutions
{
	/// <summary>
	///		ViewModel de una solución
	/// </summary>
	public class SolutionViewModel
	{
		public SolutionViewModel(RestStudioViewModel restStudioViewModel)
		{
			// Asigna las propiedades
			RestStudioViewModel = restStudioViewModel;
			Solution = new RestSolutionModel();
		}

		/// <summary>
		///		Carga una solución
		/// </summary>
		public void Load(string path)
		{
			// Guarda el directorio
			Path = path;
			// Carga el archivo
			Solution = new RestManager().Load(GetSolutionFileName());
		}

		/// <summary>
		///		Graba una solución
		/// </summary>
		public void Save()
		{
			new RestManager().Save(GetSolutionFileName(), Solution);
		}

		/// <summary>
		///		Obtiene el nombre del archivo de la solución
		/// </summary>
		private string GetSolutionFileName()
		{
			// Ajusta el directorio si estaba vacío
			if (string.IsNullOrWhiteSpace(Path))
				Path = RestStudioViewModel.RestController.AppPath;
			// Obtiene el nombre de archivo
			return System.IO.Path.Combine(Path, "RestSolution.rxml");
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public RestStudioViewModel RestStudioViewModel { get; }

		/// <summary>
		///		Solución
		/// </summary>
		public RestSolutionModel Solution { get; private set; }

		/// <summary>
		///		Directorio de la solución
		/// </summary>
		public string Path { get; private set; }
	}
}
