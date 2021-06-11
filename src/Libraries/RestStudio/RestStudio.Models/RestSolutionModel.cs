using System;
using System.Collections.Generic;

namespace Bau.Libraries.RestStudio.Models
{
	/// <summary>
	///		Clase con los datos de una solución para proyectos REST
	/// </summary>
	public class RestSolutionModel
	{
		/// <summary>
		///		Apis de la solución
		/// </summary>
		public List<Rest.RestApiModel> RestApis { get; } = new();
	}
}
