﻿using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parameters
{
	/// <summary>
	///		Sentencia que se envía al proveedor
	/// </summary>
	internal class ProviderSentenceModel
	{
		internal ProviderSentenceModel(string sql, TimeSpan timeout)
		{
			Sql = sql;
			Timeout = timeout;
		}

		/// <summary>
		///		Clona la sentencia
		/// </summary>
		internal ProviderSentenceModel Clone()
		{
			ProviderSentenceModel target = new ProviderSentenceModel(Sql, Timeout);

				// Añade los filtros
				foreach (FilterModel filter in Filters)
					target.Filters.Add(filter.Clone());
				// Devuelve el comando clonado
				return target;
		}

		/// <summary>
		///		Comandos
		/// </summary>
		internal string Sql { get; }

		/// <summary>
		///		Tiempo de espera del comando
		/// </summary>
		internal TimeSpan Timeout { get; }

		/// <summary>
		///		Filtros
		/// </summary>
		internal List<FilterModel> Filters { get; } = new List<FilterModel>();
	}
}
