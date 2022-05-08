using System;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Definición de la paginación
	/// </summary>
	public class PaginationRequestModel
	{
		/// <summary>
		///		Indica si se debe paginar
		/// </summary>
		public bool MustPaginate { get; set; }

		/// <summary>
		///		Página a consultar
		/// </summary>
		public int Page { get; set; }

		/// <summary>
		///		Registros por página
		/// </summary>
		public long RecordsPerPage { get; set; }
	}
}
