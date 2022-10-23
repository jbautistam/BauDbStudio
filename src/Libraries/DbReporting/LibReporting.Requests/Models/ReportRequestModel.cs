using System;

using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Clase con los datos de solicitud de informe
	/// </summary>
	public class ReportRequestModel
	{
		/// <summary>
		///		Código de informe solicitado
		/// </summary>
		public string ReportId { get; set; }

		/// <summary>
		///		Obtiene los datos de solicitud de una dimensión
		/// </summary>
		public DimensionRequestModel GetDimensionRequest(string dimensionKey)
		{
			// Busca la dimensión entre las solicitudes
			foreach (DimensionRequestModel request in Dimensions)
				if (request.DimensionId.Equals(dimensionKey, StringComparison.CurrentCultureIgnoreCase))
					return request;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Comprueba si se ha solicitado alguna dimensión
		/// </summary>
		public bool IsRequested(List<string> dimensionsKey)
		{
			// Comprueba si se ha solicitado alguna de las dimensiones
			if (dimensionsKey is not null)
				foreach (string key in dimensionsKey)
					foreach (DimensionRequestModel request in Dimensions)
						if (request.DimensionId.Equals(key, StringComparison.CurrentCultureIgnoreCase))
							return true;
			// Si ha llegado hasta aquí es porque no se ha encontrado ninguna de las dimensiones
			return false;
		}

		/// <summary>
		///		Comprueba si se ha solicitado una dimensión
		/// </summary>
		public bool IsRequested(string dimensionKey)
		{
			return IsRequested(new List<string> { dimensionKey });
		}

		/// <summary>
		///		Dimensiones solicitadas
		/// </summary>
		public List<DimensionRequestModel> Dimensions { get; } = new List<DimensionRequestModel>();

		/// <summary>
		///		Expresiones solicitadas
		/// </summary>
		public List<ExpressionRequestModel> Expressions { get; } = new List<ExpressionRequestModel>();

		/// <summary>
		///		Paginación
		/// </summary>
		public PaginationRequestModel Pagination { get; } = new PaginationRequestModel();
	}
}