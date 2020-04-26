using System;

namespace Bau.Libraries.LibJobProcessor.Core.Models.Errors
{
	/// <summary>
	///		Clase con los datos de un error
	/// </summary>
	public class ErrorModel
	{
		public ErrorModel(JobProjectModel project, Jobs.JobStepModel step, string message)
		{
			Project = project;
			Step = step;
			Message = message;
		}

		/// <summary>
		///		Obtiene la cadena con el error
		/// </summary>
		public override string ToString()
		{
			return $"[{Project?.Name}] [{Step?.Name}]: {Message}";
		}

		/// <summary>
		///		Proyecto en el que se ha encontrado el error
		/// </summary>
		public JobProjectModel Project { get; }
		
		/// <summary>
		///		Paso en el que se ha encontrado el error
		/// </summary>
		public Jobs.JobStepModel Step { get; }
		
		/// <summary>
		///		Mensaje de error
		/// </summary>
		public string Message { get; }
	}
}
