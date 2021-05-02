using System;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.RestStudio.Application;
using Bau.Libraries.RestStudio.Models;

namespace Bau.Libraries.RestStudio.ViewModels
{
	/// <summary>
	///		ViewModel para RestStudio
	/// </summary>
	public class RestStudioViewModel : BaseObservableObject
	{
		public RestStudioViewModel(Controllers.IRestStudioController restStudioController)
		{
			// Asigna las propiedades
			RestStudioController = restStudioController;
			// Inicializa la solución
			Solution = new RestSolutionModel();
			// Inicializa los objetos
			TreeRestApiViewModel = new Explorers.TreeRestApiViewModel(this);
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
			return System.IO.Path.Combine(Path, "RestSolution.rxml");
		}

		/// <summary>
		///		Abre un archivo (si reconoce la extensión)
		/// </summary>
		public bool OpenFile(string fileName)
		{
			return false;
		}

		/// <summary>
		///		Controlador principal
		/// </summary>
		public Controllers.IRestStudioController RestStudioController { get; }

		/// <summary>
		///		Solución
		/// </summary>
		public RestSolutionModel Solution { get; private set; }

		/// <summary>
		///		Directorio de la solución
		/// </summary>
		public string Path { get; private set; }

		/// <summary>
		///		Arbol de API
		/// </summary>
		public Explorers.TreeRestApiViewModel TreeRestApiViewModel { get; }
	}
}
