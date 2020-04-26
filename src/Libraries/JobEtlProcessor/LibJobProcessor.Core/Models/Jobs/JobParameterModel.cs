using System;
using System.Text;

namespace Bau.Libraries.LibJobProcessor.Core.Models.Jobs
{
	/// <summary>
	///		Datos de un parámetro
	/// </summary>
	public class JobParameterModel
	{
		/// <summary>
		///		Tipo de parámetro
		///	</summary>
		public enum ParameterType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Numérico</summary>
			Numeric,
			/// <summary>Fecha</summary>
			DateTime,
			/// <summary>Cadena</summary>
			String,
			/// <summary>Lógico</summary>
			Boolean
		}
		/// <summary>
		///		Modo de cálculo de la fecha
		/// </summary>
		public enum ComputeDateMode
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>La fecha se obtiene a partir del día de hoy</summary>
			Today,
			/// <summary>La fecha es fija: se ha escrito en el archivo</summary>
			Fixed
		}

		/// <summary>
		///		Tipo de intervalo
		/// </summary>
		public enum IntervalType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Año</summary>
			Year,
			/// <summary>Mes</summary>
			Month,
			/// <summary>Día</summary>
			Day
		}

		/// <summary>
		///		Modo de ajuste del intervalo
		///	</summary>
		public enum IntervalMode
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Ajusta el intervalo al siguiente lunes</summary>
			NextMonday,
			/// <summary>Ajusta el intervalo al lunes anterior</summary>
			PreviousMonday,
			/// <summary>Ajusta el intervalo a final de mes</summary>
			MonthEnd,
			/// <summary>Ajusta el intervalo a inicio de mes</summary>
			MonthStart
		}

		/// <summary>
		///		Añade la información de depuración
		/// </summary>
		internal void Debug(StringBuilder builder, string indent)
		{
			builder.AppendLine($"{indent}{nameof(Key)}: {Key} - {nameof(Type)}: {Type.ToString()} - {nameof(Value)}: {Value?.ToString()}");
		}

		/// <summary>
		///		Clave del parámetro
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		///		Tipo de parámetro
		/// </summary>
		public ParameterType Type { get; set; }

		/// <summary>
		///		Modo de obtención de la fecha
		/// </summary>
		public ComputeDateMode DateMode { get; set; }

		/// <summary>
		///		Intervalo
		/// </summary>
		public IntervalType Interval { get; set; }

		/// <summary>
		///		Incremento
		/// </summary>
		public int Increment { get; set; }

		/// <summary>
		///		Modo
		/// </summary>
		public IntervalMode Mode { get; set; }

		/// <summary>
		///		Valor
		/// </summary>
		public object Value { get; set; }
	}
}
