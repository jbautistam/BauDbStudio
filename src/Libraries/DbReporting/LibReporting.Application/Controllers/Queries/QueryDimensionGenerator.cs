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
		internal QueryDimensionGenerator(ReportQueryGenerator generator, List<DimensionModel> dimensionsRequested)
						: base(generator, dimensionsRequested)
		{
		}

		/// <summary>
		///		Obtiene la consulta para una dimensión del informe
		/// </summary>
		internal QueryModel GetQuery(DimensionModel dimension)
		{
			List<QueryModel> childsQueries = new List<QueryModel>();

				// Obtiene las consultas de las dimensiones hija: si hay algún campo solicitado de alguna dimensión hija, 
				// necesitaremos también la consulta de esta dimensión para poder hacer el JOIN posterior, por eso las
				// calculamos antes que la consulta de esta dimensión
				foreach (DimensionRelationModel relation in dimension.Relations)
				{
					QueryModel childQuery = GetQuery(relation.Dimension);

						// Añade la query si realmente hay algo que añadir
						if (childQuery != null)
							childsQueries.Add(childQuery);
				}
				// Obtiene la consulta de esta dimensión
				if (childsQueries.Count > 0 || IsRequestedField(dimension))
				{
					QueryModel query = GetChildQuery(dimension);

						// Añade las consultas con las dimensiones hija
						if (childsQueries.Count > 0)
						{
							// Añade las consultas hija
							foreach (QueryModel childQuery in childsQueries)
							{
								QueryJoinModel join = new QueryJoinModel(QueryJoinModel.JoinType.Inner, childQuery, $"child{childQuery.Alias}");

									// Asigna las relaciones
									foreach (DimensionRelationModel relation in dimension.Relations)
										if (relation.Dimension.GlobalId.Equals(childQuery.SourceId, StringComparison.CurrentCultureIgnoreCase))
											foreach (RelationForeignKey foreignKey in relation.ForeignKeys)
												join.Relations.Add(new QueryRelationModel(foreignKey.ColumnId, childQuery.FromAlias, foreignKey.TargetColumnId));
									// Añade la unión
									query.Joins.Add(join);
							}
						}
						// Devuelve la consulta
						return query;
				}
				else // ... no es necesario que se ejecute ninguna consulta por esta dimensión
					return null;
		}

		/// <summary>
		///		Obtiene la consulta de una dimensión
		/// </summary>
		private QueryModel GetChildQuery(DimensionModel dimension)
		{
			QueryModel query = new QueryModel(dimension.GlobalId, QueryModel.QueryType.Dimension, dimension.GlobalId);

				// Prepara la consulta
				query.Prepare(dimension.DataSource);
				// Añade los campos clave
				foreach (DataSourceColumnModel column in dimension.DataSource.Columns)
					if (column.IsPrimaryKey)
						AddPrimaryKey(query, column.ColumnId);
				// Asigna los campos
				foreach (BaseColumnRequestModel baseColumnRequest in Generator.Request.Columns)
					if (IsDimensionColumn(dimension, baseColumnRequest))
						AddColumn(query, (baseColumnRequest as DimensionRequestModel).ColumnId, string.Empty, baseColumnRequest);
				// Devuelve la consulta
				return query;
		}

		/// <summary>
		///		Comprueba si se ha solicitado algún campo de esta dimensión
		/// </summary>
		private bool IsRequestedField(DimensionModel dimension)
		{
			// Si se ha solicitado algún campo
			foreach (BaseColumnRequestModel baseColumnRequest in Generator.Request.Columns)
				if (IsDimensionColumn(dimension, baseColumnRequest))
					return true;
			// Si ha llegado hasta aquí es porque no ha encontrado nada
			return false;
		}

		/// <summary>
		///		Comprueba si es una columna de dimensión
		/// </summary>
		private bool IsDimensionColumn(DimensionModel dimension, BaseColumnRequestModel baseColumnRequest)
		{
			return baseColumnRequest is DimensionRequestModel dimensionRequest && 
				   dimension.GlobalId.Equals(dimensionRequest.DimensionId, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}
