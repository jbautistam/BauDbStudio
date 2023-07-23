using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries;

/// <summary>
///		Base de los generadores de consultas SQL para un informe
/// </summary>
internal abstract class ReportBaseQueryGenerator
{
	protected ReportBaseQueryGenerator(ReportingSchemaModel schema, ReportBaseModel report, ReportRequestModel request)
	{
		Schema = schema;
		Report = report;
		Request = request;
	}

	/// <summary>
	///		Obtiene la cadena SQL de un informe para responder a una solicitud
	/// </summary>
	internal abstract string GetSql();

	/// <summary>
	///		Obtiene el nombre de un campo
	/// </summary>
	internal string GetFieldName(string schema, string table, string field)
	{
		return Normalize(schema).AddWithSeparator(Normalize(table), ".", false).AddWithSeparator(Normalize(field), ".", false);

		// Normaliza un nombre de esquema, tabla o campo
		string Normalize(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return string.Empty;
			else
				return $"[{value.Trim()}]";
		}
	}

	/// <summary>
	///		Obtiene el nombre de un campo
	/// </summary>
	internal string GetFieldName(string table, string field) => GetFieldName(string.Empty, table, field);

	/// <summary>
	///		Obtiene el nombre de un campo
	/// </summary>
	internal string GetFieldName(string field) => GetFieldName(string.Empty, string.Empty, field);

	/// <summary>
	///		Esquema para las consultas
	/// </summary>
	internal ReportingSchemaModel Schema { get; }

	/// <summary>
	///		Informe solicitado
	/// </summary>
	internal ReportBaseModel Report { get; }

	/// <summary>
	///		Datos de la solicitud
	/// </summary>
	internal ReportRequestModel Request { get; }
}
