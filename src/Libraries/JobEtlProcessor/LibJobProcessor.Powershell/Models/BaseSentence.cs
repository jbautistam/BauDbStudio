using System;

namespace Bau.Libraries.LibJobProcessor.Powershell.Models
{
	/// <summary>
	///		Base para las sentencias
	/// </summary>
	internal abstract class BaseSentence
	{
		/// <summary>
		///		Comprueba los datos de la sentencia
		/// </summary>
		internal bool ValidateData(Core.Models.JobProjectModel project, out string error)
		{
			// Inicializa los argumentos de salida
			error = string.Empty;
			// Comprueba los datos
			if (Enabled)
				error = Validate(project);
			// Devuelve el valor que indica si los datos son correctos
			return string.IsNullOrWhiteSpace(error);
		}

		/// <summary>
		///		Comprueba los datos
		/// </summary>
		protected abstract string Validate(Core.Models.JobProjectModel project);

		/// <summary>
		///		Indica si se debe ejecutar la instrucción
		/// </summary>
		internal bool Enabled { get; set; } = true;

		/// <summary>
		///		Tiempo de espera
		/// </summary>
		internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
	}
}
