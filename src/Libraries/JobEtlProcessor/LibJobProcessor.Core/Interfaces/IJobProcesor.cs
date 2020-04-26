using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bau.Libraries.LibJobProcessor.Core.Interfaces
{
	/// <summary>
	///		Interface para los procesadores de trabajo
	/// </summary>
	public interface IJobProcesor
	{
		/// <summary>
		///		Procesa un script
		/// </summary>
		Task<bool> ProcessAsync(List<Models.JobContextModel> contexts, Models.Jobs.JobStepModel job, System.Threading.CancellationToken cancellationToken);

		/// <summary>
		///		Clave del procesador (indica el tipo de scripts que ejecuta)
		/// </summary>
		string Key { get; }

		/// <summary>
		///		Errores durante la ejecución
		/// </summary>
		List<string> Errors { get; }
	}
}
