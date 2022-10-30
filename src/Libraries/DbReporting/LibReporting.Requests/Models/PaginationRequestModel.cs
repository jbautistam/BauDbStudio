using System;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Definición de la paginación
	/// </summary>
	public class PaginationRequestModel
	{
		/// <summary>
		///		Clona la definición de paginación
		/// </summary>
		internal void Clone(PaginationRequestModel basePagination)
		{
			MustPaginate = basePagination.MustPaginate;
			Page = basePagination.Page;
			RecordsPerPage = basePagination.RecordsPerPage;
		}

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
