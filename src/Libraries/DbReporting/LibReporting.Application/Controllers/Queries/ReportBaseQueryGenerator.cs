using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibReporting.Models;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;
using Bau.Libraries.LibReporting.Requests.Models;
using Bau.Libraries.LibReporting.Application.Controllers.Queries.Models;

namespace Bau.Libraries.LibReporting.Application.Controllers.Queries
{
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
}
