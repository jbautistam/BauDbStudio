using System;
using System.Collections.Generic;
using System.Linq;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries
{
	/// <summary>
	///		Clase base para los generadores de consultas
	/// </summary>
	internal abstract class QueryBaseGenerator
	{
		protected QueryBaseGenerator(ReportBaseQueryGenerator generator)
		{
			Generator = generator;
		}

		/// <summary>
		///		Obtiene una cadena concatenada con campos
		/// </summary>
		protected string GetSqlFields(List<string> fields)
		{
			string sql = string.Empty;

				// Crea la cadena SQL con los campos de dimensión
				foreach (string field in fields)
					sql = sql.AddWithSeparator($"[{field}]", ",");
				// Devuelve la cadena
				return sql;
		}

		/// <summary>
		///		Busca un origen de datos
		/// </summary>
		protected BaseDataSourceModel SearchDataSource(string dataSourceId)
		{
			// Busca el origen de datos
			foreach (DataWarehouseModel dataWarehouse in Generator.Schema.DataWarehouses.EnumerateValues())
			{
				BaseDataSourceModel dataSource = dataWarehouse.DataSources[dataSourceId];

					if (dataSource != null)
						return dataSource;
			}
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return null;
		}

		/// <summary>
		///		Obtiene la dimensión
		/// </summary>
		protected DimensionModel GetDimension(DimensionRequestModel dimensionRequest)
		{
			DimensionModel dimension = Generator.Report.DataWarehouse.Dimensions[dimensionRequest.DimensionId];

				// Devuelve la dimensión localizada o lanza una excepción
				if (dimension == null)
					throw new LibReporting.Models.Exceptions.ReportingException($"Cant find the dimension {dimensionRequest.DimensionId}");
				else
					return dimension;
		}

		/// <summary>
		///		Generador del informe
		/// </summary>
		protected ReportBaseQueryGenerator Generator { get; }
	}
}
