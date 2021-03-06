﻿using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Clase base para las columnas
	/// </summary>
	public abstract class BaseColumnRequestModel
	{
		/// <summary>
		///		Modo de ordenación
		/// </summary>
		public enum SortOrder
		{
			/// <summary>No se define ningún orden</summary>
			Undefined,
			/// <summary>Orden ascendente</summary>
			Ascending,
			/// <summary>Orden descendente</summary>
			Descending
		}

		/// <summary>
		///		Indica si esta columna es visible en la consulta final o sólo para los filtros
		/// </summary>
		public bool Visible { get; set; } = true;

		/// <summary>
		///		Modo de ordenación
		/// </summary>
		public SortOrder OrderBy { get; set; }

		/// <summary>
		///		Filtro para la cláusula WHERE
		/// </summary>
		public List<FilterRequestModel> FiltersWhere { get; } = new List<FilterRequestModel>();

		/// <summary>
		///		Filtro para la cláusula HAVING
		/// </summary>
		public List<FilterRequestModel> FiltersHaving { get; } = new List<FilterRequestModel>();
	}
}
