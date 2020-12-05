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
								if (relation.Dimension.GlobalId.Equals(childQuery.SourceId, StringComparison.CurrentCultureIgnoreCase))
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
			QueryModel query = new QueryModel(dimensionRequest.DimensionId, QueryModel.QueryType.Dimension, dimension.GlobalId);

				// Prepara la consulta
				query.Prepare(dimension.DataSource);
				// Añade los campos clave
				foreach (DataSourceColumnModel column in dimension.DataSource.Columns)
					if (column.IsPrimaryKey)
						AddPrimaryKey(query, column.ColumnId);
				// Asigna los campos
				foreach (DimensionColumnRequestModel columnRequest in dimensionRequest.Columns)
					AddColumn(query, columnRequest.ColumnId, string.Empty, columnRequest);
				// Devuelve la consulta
				return query;
		}
	}
}
