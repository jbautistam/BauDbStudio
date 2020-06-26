using System;

namespace Bau.Libraries.DbStudio.Models.Deployments
{
	/// <summary>
	///		Modelo de distribución
	/// </summary>
	public class DeploymentModel : LibDataStructures.Base.BaseExtendedModel
	{
		/// <summary>
		///		Tipos de distribución
		/// </summary>
		public enum DeploymentType
		{
			/// <summary>Copia de archivos a databricks</summary>
			Databricks
		}

		public DeploymentModel(SolutionModel solution)
		{
			Solution = solution;
		}

		/// <summary>
		///		Solución
		/// </summary>
		public SolutionModel Solution { get; }

		/// <summary>
		///		Tipo
		/// </summary>
		public DeploymentType Type { get; set; }

		/// <summary>
		///		Directorio origen
		/// </summary>
		public string SourcePath { get; set; }

		/// <summary>
		///		Directorio destino
		/// </summary>
		public string TargetPath { get; set; }

		/// <summary>
		///		Indica si se deben escribir los comentarios en los archivos finales
		/// </summary>
		public bool WriteComments { get; set; } = true;

		/// <summary>
		///		Indica si se deben reemplazar los argumentos por GetArgument
		/// </summary>
		public bool ReplaceArguments { get; set; } = true;

		/// <summary>
		///		Indica si se deben pasar a minúsculas los nombres de archivos
		/// </summary>
		public bool LowcaseFileNames { get; set; } = true;

		/// <summary>
		///		Parámetros
		/// </summary>
		public string JsonParameters { get; set; }
	}
}
