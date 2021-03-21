using System;
using System.Collections.Generic;

using Bau.Libraries.LibReporting.Requests.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries
{
	/// <summary>
	///		Generador de consultas para una dimensión
	/// </summary>
	internal class QueryDimensionGenerator : BaseQueryGenerator
	{
		internal QueryDimensionGenerator(ReportQueryGenerator generator) : base(generator) {}

		/// <summary>
		///		Obtiene la consulta para una dimensión del informe
		/// </summary>
		internal QueryModel GetQuery(DimensionRequestModel dimensionRequest)
		{
			List<QueryModel> childsQueries = new List<QueryModel>();
			DimensionModel dimension = GetDimension(dimensionRequest);
			QueryModel query = GetChildQuery(dimensionRequest);

				// Obtiene las consultas de las dimensiones hija: si hay algún campo solicitado de alguna dimensión hija, 
				// necesitaremos también la consulta de esta dimensión para poder hacer el JOIN posterior, por eso las
				// calculamos antes que la consulta de esta dimensión
				foreach (DimensionRequestModel childDimension in dimensionRequest.Childs)
				{
					QueryModel childQuery = GetQuery(childDimension);

						// Añade la query si realmente hay algo que añadir
						if (childQuery != null)
							childsQueries.Add(childQuery);
				}
				// Añade las consultas con las dimensiones hija
				if (childsQueries.Count > 0)
					foreach (QueryModel childQuery in childsQueries)
					{
						QueryJoinModel join = new QueryJoinModel(QueryJoinModel.JoinType.Inner, childQuery, $"child_{childQuery.Alias}");

							// Asigna las relaciones
							foreach (DimensionRelationModel relation in dimension.Relations)
								if (relation.Dimension.Id.Equals(childQuery.SourceId, StringComparison.CurrentCultureIgnoreCase))
									foreach (RelationForeignKey foreignKey in relation.ForeignKeys)
										join.Relations.Add(new QueryRelationModel(foreignKey.ColumnId, childQuery.FromAlias, foreignKey.TargetColumnId));
							// Añade la unión
							query.Joins.Add(join);
					}
				// Devuelve la consulta
				return query;
		}

		/// <summary>
		///		Obtiene la consulta de una dimensión
		/// </summary>
		private QueryModel GetChildQuery(DimensionRequestModel dimensionRequest)
		{
			DimensionModel dimension = GetDimension(dimensionRequest);
			QueryModel query = new QueryModel(dimensionRequest.DimensionId, QueryModel.QueryType.Dimension, dimension.Id);

				// Prepara la consulta
				query.Prepare(dimension.DataSource);
				// Añade los campos clave
				foreach (DataSourceColumnModel column in dimension.DataSource.Columns.EnumerateValues())
					if (column.IsPrimaryKey)
						query.AddPrimaryKey(dimensionRequest.GetRequestColumn(column.Id), column.Id, CheckIsColumnAtColumnRequested(column, dimensionRequest.Columns));
				// Asigna los campos
				foreach (DimensionColumnRequestModel columnRequest in dimensionRequest.Columns)
				{
					DataSourceColumnModel column = dimension.DataSource.Columns[columnRequest.ColumnId];

						if (column != null && !column.IsPrimaryKey)
							query.AddColumn(columnRequest.ColumnId, columnRequest);
				}
				// Devuelve la consulta
				return query;
		}

		/// <summary>
		///		Comprueba si la columna está entre las columnas solicitadas
		/// </summary>
		private bool CheckIsColumnAtColumnRequested(DataSourceColumnModel column, List<DimensionColumnRequestModel> columnRequests)
		{
			// Busca la columna entre las columnas solicitadas
			foreach (DimensionColumnRequestModel columnRequest in columnRequests)
				if (column.Id.Equals(columnRequest.ColumnId, StringComparison.CurrentCultureIgnoreCase))
					return true;
			// Si llega hasta aquí es porque no lo ha encontrado
			return false;
		}
	}
}
