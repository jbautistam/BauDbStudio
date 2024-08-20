using Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;
using Bau.Libraries.LibReporting.Application.Exceptions;
using Bau.Libraries.LibReporting.Models.Base;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Dimensions;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries;

/// <summary>
///		Controles con las funciones asociadas al manejo de las <see cref="ReportRequestModel"/>
/// </summary>
internal class ReportRequestController
{
	internal ReportRequestController(ReportQueryGenerator manager, ReportRequestModel request)
	{
		Manager = manager;
		Request = request;
	}

	/// <summary>
	///		Obtiene los campos solicitados para una dimensión
	/// </summary>
	internal QueryTableModelNew? GetRequestedTable(string table, string alias, string dimensionKey, bool includePrimaryKeys, bool includeRequestFields)
	{
		List<DataSourceColumnModel>? columns = GetRequestedFields(dimensionKey, includePrimaryKeys, includeRequestFields);

			if (columns is null)
				return null;
			else
			{
				QueryTableModelNew queryTable = new(table, alias);

					// Añade las columnas
					foreach (DataSourceColumnModel column in columns)
						queryTable.AddColumn(column.IsPrimaryKey, column.Id, column.Alias, column.Type);
					// Devuelve la tabla generada
					return queryTable;
			}
	}

	/// <summary>
	///		Obtiene los datos de una dimensión si se ha solicitado
	/// </summary>
	internal BaseDimensionModel? GetDimensionIfRequest(ParserDimensionModel parserDimension)
	{
		return GetDimensionIfRequest(parserDimension.DimensionKey, parserDimension.Required, parserDimension.RelatedDimensions, 
									 parserDimension.IfNotRequestDimensions);
	}

	/// <summary>
	///		Obtiene los datos de una dimensión si se ha solicitado
	/// </summary>
	internal BaseDimensionModel? GetDimensionIfRequest(string dimensionKey, bool required, List<string>? relatedDimensions, List<string>? notRequestedDimensions)
	{
		BaseDimensionModel? dimensionJoin = Manager.Report.DataWarehouse.Dimensions[dimensionKey];

			if (dimensionJoin is null)
				throw new ReportingParserException($"Can't find the dimension {dimensionKey}");
			else if (required || 
					 ((Request.IsRequestedDimension(dimensionJoin.Id) || Request.IsRequestedDimension(relatedDimensions)) && 
						!Request.IsRequestedDimension(notRequestedDimensions)))
				return dimensionJoin;
			else
				return null;
	}

	/// <summary>
	///		Obtiene los campos solicitados para una dimensión
	/// </summary>
	private List<DataSourceColumnModel>? GetRequestedFields(string dimensionKey, bool includePrimaryKeys, bool includeRequestFields)
	{
		DimensionRequestModel? dimensionRequest = Request.GetDimensionRequest(dimensionKey);

			if (dimensionRequest is not null)
				return GetRequestedFields(dimensionRequest, includePrimaryKeys, includeRequestFields);
			else
				return null;
	}

	/// <summary>
	///		Obtiene los campos solicitados para una dimensión
	/// </summary>
	private List<DataSourceColumnModel> GetRequestedFields(DimensionRequestModel dimensionRequest, bool includePrimaryKeys, bool includeRequestFields)
	{
		BaseDimensionModel dimension = GetDimension(dimensionRequest);
		BaseReportingDictionaryModel<DataSourceColumnModel> dimensionColumns = dimension.GetColumns();
		List<DataSourceColumnModel> columns = new();

			// Añade los campos clave
			if (includePrimaryKeys)
				foreach (DataSourceColumnModel column in dimensionColumns.EnumerateValues())
					if (column.IsPrimaryKey)
						columns.Add(column);
			// Asigna los campos
			if (includeRequestFields)
				foreach (DimensionColumnRequestModel columnRequest in dimensionRequest.Columns)
				{
					DataSourceColumnModel? column = dimensionColumns[columnRequest.ColumnId];

						if (column is not null && !column.IsPrimaryKey)
							columns.Add(column);
				}
			// Añade las columnas solicitadas para las dimensiones hija
			foreach (DimensionRequestModel childs in dimensionRequest.Childs)
				columns.AddRange(GetRequestedFields(childs, includePrimaryKeys, includeRequestFields));
			// Devuelve las columnas
			return columns;
	}
	
	/// <summary>
	///		Obtiene la dimensión
	/// </summary>
	private BaseDimensionModel GetDimension(DimensionRequestModel dimensionRequest)
	{
		BaseDimensionModel? dimension = Manager.Report.DataWarehouse.Dimensions[dimensionRequest.DimensionId];

			// Devuelve la dimensión localizada o lanza una excepción
			if (dimension is null)
				throw new LibReporting.Models.Exceptions.ReportingException($"Can't find the dimension {dimensionRequest.DimensionId}");
			else
				return dimension;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una dimensión
	/// </summary>
	internal bool CheckIfDimensionRequest(string dimensionKey) => Request.GetDimensionRequest(dimensionKey) is not null;

	/// <summary>
	///		Controlador de generación del informe
	/// </summary>
	internal ReportQueryGenerator Manager { get; } 
	
	/// <summary>
	///		Solicitud
	/// </summary>
	internal ReportRequestModel Request { get; }
}
