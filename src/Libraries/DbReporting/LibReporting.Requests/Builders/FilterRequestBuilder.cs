using System;

using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Requests.Builders
{
	/// <summary>
	///		Generador para <see cref="FilterRequestModel"/>
	/// </summary>
	public class FilterRequestBuilder
	{
		/// <summary>
		///		Modo de evaluación del filtro
		/// </summary>
		public enum FilterEvaluation
		{
			/// <summary>Evalua el filtro en la consulta</summary>
			Where,
			/// <summary>Evalua el filtro tras la evaluación</summary>
			Having
		}

		public FilterRequestBuilder(ColumnRequestBuilder builder, FilterRequestModel.ConditionType condition, FilterEvaluation mode)
		{
			// Obtiene los objetos
			Builder = builder;
			Filter = new FilterRequestModel
								{
									Condition = condition
								};
			// Asigna el filtro a la solicitud al punto adecuado
			switch (mode)
			{
				case FilterEvaluation.Where:
						builder.Column.FiltersWhere.Add(Filter);
					break;
				case FilterEvaluation.Having:
						builder.Column.FiltersHaving.Add(Filter);
					break;
			}
		}

		/// <summary>
		///		Añade un valor al filtro
		/// </summary>
		public FilterRequestBuilder WithValue(object value)
		{
			// Añade el valor
			Filter.Values.Add(value);
			// Devuelve el generador
			return this;
		}

		/// <summary>
		///		Salta al generador padre
		/// </summary>
		public ColumnRequestBuilder Back()
		{
			return Builder;
		}

		/// <summary>
		///		Generador padre
		/// </summary>
		private ColumnRequestBuilder Builder { get; }

		/// <summary>
		///		Filtro
		/// </summary>
		private FilterRequestModel Filter { get; }
	}
}
