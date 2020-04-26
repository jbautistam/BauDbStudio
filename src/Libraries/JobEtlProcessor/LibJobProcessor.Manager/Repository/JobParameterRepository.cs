using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Manager.Repository
{
	/// <summary>
	///		Clase de lectura de <see cref="JobParameterModel"/>
	/// </summary>
	internal class JobParameterRepository
	{
		// Constantes privadas
		internal const string TagParameter = "Parameter";
		private const string TagKey = "Key";
		private const string TagType = "Type";
		private const string TagValue = "Value";
		private const string TagComputeMode = "ComputeDateMode";
		private const string TagIncrement = "Increment";
		private const string TagInterval = "Interval";
		private const string TagMode = "Mode";

		/// <summary>
		///		Carga una colección de parámetros
		/// </summary>
		internal NormalizedDictionary<JobParameterModel> LoadParameters(MLNode rootML)
		{
			NormalizedDictionary<JobParameterModel> parameters = new NormalizedDictionary<JobParameterModel>();

				// Carga los parámetros
				foreach (MLNode nodeML in rootML.Nodes)
					if (nodeML.Name == TagParameter)
					{
						JobParameterModel parameter = LoadParameter(nodeML);

							parameters.Add(parameter.Key, parameter);
					}
				// Devuelve la colección de parámetros
				return parameters;
		}

		/// <summary>
		///		Carga un parámetro
		/// </summary>
		internal JobParameterModel LoadParameter(MLNode rootML)
		{
			JobParameterModel parameter = new JobParameterModel();

				// Asigna el tipo
				parameter.Key = rootML.Attributes[TagKey].Value.TrimIgnoreNull();
				parameter.Type = rootML.Attributes[TagType].Value.GetEnum(JobParameterModel.ParameterType.Unknown);
				// Obtiene el valor
				switch (parameter.Type)
				{
					case JobParameterModel.ParameterType.Numeric:
							parameter.Value = rootML.Attributes[TagValue].Value.GetDouble(0);
						break;
					case JobParameterModel.ParameterType.Boolean:
							parameter.Value = rootML.Attributes[TagValue].Value.GetBool();
						break;
					case JobParameterModel.ParameterType.DateTime:
							ConvertDate(rootML, parameter);
						break;
					default:
							if (string.IsNullOrWhiteSpace(rootML.Attributes[TagValue].Value))
								parameter.Value = rootML.Value.TrimIgnoreNull();
							else
								parameter.Value = rootML.Attributes[TagValue].Value.TrimIgnoreNull();
						break;
				}
				// Devuelve el parámetro
				return parameter;
		}

		/// <summary>
		///		Convierte una fecha
		/// </summary>
		private void ConvertDate(MLNode nodeML, JobParameterModel parameter)
		{
			DateTime date = DateTime.Now.Date;

				// Asigna los valores iniciales
				parameter.DateMode = nodeML.Attributes[TagComputeMode].Value.GetEnum(JobParameterModel.ComputeDateMode.Today);
				parameter.Interval = nodeML.Attributes[TagInterval].Value.GetEnum(JobParameterModel.IntervalType.Day);
				parameter.Increment = nodeML.Attributes[TagIncrement].Value.GetInt(0);
				parameter.Mode = nodeML.Attributes[TagMode].Value.GetEnum(JobParameterModel.IntervalMode.Unknown);
				// Se recoge la fecha (si se ha introducido alguna)
				if (parameter.DateMode == JobParameterModel.ComputeDateMode.Fixed)
					date = nodeML.Attributes[TagValue].Value.GetDateTime(DateTime.Now);
				// Ajusta el valor con los parámetros del XML
				if (parameter.Increment != 0)
					switch (parameter.Interval)
					{
						case JobParameterModel.IntervalType.Day:
								date = date.AddDays(parameter.Increment);
							break;
						case JobParameterModel.IntervalType.Month:
								date = date.AddMonths(parameter.Increment);
							break;
						case JobParameterModel.IntervalType.Year:
								date = date.AddYears(parameter.Increment);
							break;
					}
				// Ajusta la fecha
				switch (parameter.Mode)
				{
					case JobParameterModel.IntervalMode.PreviousMonday:
							date = GetPreviousMonday(date);
						break;
					case JobParameterModel.IntervalMode.NextMonday:
							date = GetNextMonday(date);
						break;
					case JobParameterModel.IntervalMode.MonthStart:
							date = GetFirstMonthDay(date);
						break;
					case JobParameterModel.IntervalMode.MonthEnd:
							date = GetLastMonthDay(date);
						break;
				}
				// Asigna la fecha calculada
				parameter.Value = date;
		}

		/// <summary>
		///		Obtiene el lunes anterior a una fecha (o la misma fecha si ya es lunes)
		/// </summary>
		private DateTime GetPreviousMonday(DateTime date)
		{
			// Busca el lunes anterior
			while (date.DayOfWeek != DayOfWeek.Monday)
				date = date.AddDays(-1);
			// Devuelve la fecha
			return date;
		}

		/// <summary>
		///		Obtiene el lunes siguiente a una fecha (o la misma fecha si ya es lunes)
		/// </summary>
		private DateTime GetNextMonday(DateTime date)
		{
			// Busca el lunes anterior
			while (date.DayOfWeek != DayOfWeek.Monday)
				date = date.AddDays(1);
			// Devuelve la fecha
			return date;
		}

		/// <summary>
		///		Obtiene el primer día del mes
		/// </summary>
		private DateTime GetFirstMonthDay(DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}

		/// <summary>
		///		Obtiene el último día del mes
		/// </summary>
		private DateTime GetLastMonthDay(DateTime date)
		{
			return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
		}

		/// <summary>
		///		Obtiene los nodos de parámetros
		/// </summary>
		internal MLNodesCollection GetParametersNodes(NormalizedDictionary<JobParameterModel> parameters)
		{
			MLNodesCollection nodesML = new MLNodesCollection();

				// Añade los parámetros
				foreach ((string key, JobParameterModel parameter) in parameters.Enumerate())
				{
					MLNode nodeML = nodesML.Add(TagParameter);

						// Añade las propiedades
						nodeML.Attributes.Add(TagKey, key);
						nodeML.Attributes.Add(TagType, parameter.Type.ToString());
						nodeML.Attributes.Add(TagValue, parameter.Value?.ToString());
						// Añade las propiedades de las fechas
						if (parameter.Type == JobParameterModel.ParameterType.DateTime)
						{
							nodeML.Attributes.Add(TagComputeMode, parameter.DateMode.ToString());
							nodeML.Attributes.Add(TagInterval, parameter.Interval.ToString());
							nodeML.Attributes.Add(TagIncrement, parameter.Increment);
							nodeML.Attributes.Add(TagMode, parameter.Mode.ToString());
						}
				}
				// Devuelve la colección de nodos
				return nodesML;
		}
	}
}