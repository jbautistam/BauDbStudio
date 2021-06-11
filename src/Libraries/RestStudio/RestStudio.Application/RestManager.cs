using System;

namespace Bau.Libraries.RestStudio.Application
{
	/// <summary>
	///		Manager de Rest
	/// </summary>
	public class RestManager
	{
		/// <summary>
		///		Carga una solución
		/// </summary>
		public Models.RestSolutionModel Load(string fileName)
		{
			return new Repositories.RestRepository().Load(fileName);
		}

		/// <summary>
		///		Graba una solución
		/// </summary>
		public void Save(string fileName, Models.RestSolutionModel solution)
		{
			new Repositories.RestRepository().Save(fileName, solution);
		}
	}
}
