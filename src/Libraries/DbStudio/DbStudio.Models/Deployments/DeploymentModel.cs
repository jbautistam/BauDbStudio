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
		///		Parámetros
		/// </summary>
		public string JsonParameters { get; set; }
	}
}
