using System;

using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries.Models
{
	/// <summary>
	///		Campo de una dimensión
	/// </summary>
	internal class QueryForeignKeyFieldModel
	{
		internal QueryForeignKeyFieldModel(DimensionModel dimension, string columnDimension, string columnRelated)
		{
			Dimension = dimension;
			ColumnDimension = columnDimension;
			ColumnRelated = columnRelated;
		}

		/// <summary>
		///		Clave de la dimensión
		/// </summary>
		internal DimensionModel Dimension { get; }

		/// <summary>
		///		Nombre del campo en la dimensión
		/// </summary>
		internal string ColumnDimension { get; }

		/// <summary>
		///		Nombre del campo relacionado 
		/// </summary>
		internal string ColumnRelated { get; }
	}
}
