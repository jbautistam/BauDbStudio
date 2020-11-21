using System;

using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Requests.Builders
{
	/// <summary>
	///		Generador para <see cref="BaseColumnRequestModel"/>
	/// </summary>
	public class ColumnRequestBuilder
	{
		public ColumnRequestBuilder(ReportRequestBuilder builder, BaseColumnRequestModel column)
		{
			// Obtiene los objetos
			Builder = builder;
			Column = column;
			// Agrega la columna al generador
			Builder.ReportRequest.Columns.Add(column);
		}

		/// <summary>
		///		Indica si esta columna debe estar oculta en el resultado final (sólo se utiliza para los filtros)
		/// </summary>
		public ColumnRequestBuilder Hidden()
		{
			// Asigna la ordenación
			Column.Visible = false;
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Asigna el ORDER BY
		/// </summary>
		public ColumnRequestBuilder WithOrderBy(BaseColumnRequestModel.SortOrder orderBy = BaseColumnRequestModel.SortOrder.Undefined)
		{
			// Asigna la ordenación
			Column.OrderBy = orderBy;
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Asigna la agregación
		/// </summary>
		public ColumnRequestBuilder WithAggregation(ExpressionRequestModel.AggregationType aggregation)
		{
			// Asigna la agregación
			if (Column is ExpressionRequestModel expression)
				expression.AggregatedBy = aggregation;
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Añade un filtro a la columna en el WHERE
		/// </summary>
		public ColumnRequestBuilder WithWhere(FilterRequestModel.ConditionType condition, object value)
		{
			FilterRequestBuilder filter = GetFilter(condition, FilterRequestBuilder.FilterEvaluation.Where);

				// Asigna el valor al filtro
				filter.WithValue(value);
				// Devuelve este generador
				return this;
		}

		/// <summary>
		///		Añade un filtro a la columna en el WHERE (y permite agregarle valores)
		/// </summary>
		public FilterRequestBuilder WithWhere(FilterRequestModel.ConditionType condition)
		{
			return GetFilter(condition, FilterRequestBuilder.FilterEvaluation.Where);
		}

		/// <summary>
		///		Añade un filtro a la columna en el HAVIG
		/// </summary>
		public ColumnRequestBuilder WithHaving(FilterRequestModel.ConditionType condition, object value)
		{
			FilterRequestBuilder filter = GetFilter(condition, FilterRequestBuilder.FilterEvaluation.Having);

				// Asigna el valor al filtro
				filter.WithValue(value);
				// Devuelve este generador
				return this;
		}

		/// <summary>
		///		Añade un filtro a la columna en el HAVING (y permite agregarle valores)
		/// </summary>
		public FilterRequestBuilder WithHaving(FilterRequestModel.ConditionType condition)
		{
			return GetFilter(condition, FilterRequestBuilder.FilterEvaluation.Having);
		}

		/// <summary>
		///		Obtiene un generador para el filtro
		/// </summary>
		private FilterRequestBuilder GetFilter(FilterRequestModel.ConditionType condition, FilterRequestBuilder.FilterEvaluation mode)
		{
			return new FilterRequestBuilder(this, condition, FilterRequestBuilder.FilterEvaluation.Where);
		}

		/// <summary>
		///		Salta al generador principal
		/// </summary>
		public ReportRequestBuilder Back()
		{
			return Builder;
		}

		/// <summary>
		///		Generador padre
		/// </summary>
		private ReportRequestBuilder Builder { get; }

		/// <summary>
		///		Filtro
		/// </summary>
		internal BaseColumnRequestModel Column { get; }
	}
}
