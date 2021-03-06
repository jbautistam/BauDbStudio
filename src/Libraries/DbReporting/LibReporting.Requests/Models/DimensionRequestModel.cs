﻿using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibReporting.Requests.Models
{
	/// <summary>
	///		Colunma solicitada de una dimensión
	/// </summary>
	public class DimensionRequestModel
	{
		/// <summary>
		///		Código de dimensión
		/// </summary>
		public string DimensionId { get; set; }

		/// <summary>
		///		Columnas
		/// </summary>
		public List<DimensionColumnRequestModel> Columns { get; } = new List<DimensionColumnRequestModel>();

		/// <summary>
		///		Solicitudes de dimensiones hija
		/// </summary>
		public List<DimensionRequestModel> Childs { get; } = new List<DimensionRequestModel>();
	}
}
