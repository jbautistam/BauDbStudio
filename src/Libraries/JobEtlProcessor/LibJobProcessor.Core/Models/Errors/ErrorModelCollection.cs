using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibJobProcessor.Core.Models.Errors
{
	/// <summary>
	///		Colección de <see cref="ErrorModel"/>
	/// </summary>
	public class ErrorModelCollection : List<ErrorModel>
	{
		/// <summary>
		///		Añade un error a la colección
		/// </summary>
		public void Add(JobProjectModel project, Jobs.JobStepModel step, string message)
		{
			Add(new ErrorModel(project, step, message));
		}
	}
}
