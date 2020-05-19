using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Csv
{
	/// <summary>
	///		Colección de <see cref="SchemaExcludeRule"/>
	/// </summary>
	internal class SchemaExcludeRuleCollection : List<SchemaExcludeRule>
	{
		/// <summary>
		///		Comprueba si se debe excluir un nombre de tabla considerando todas las reglas
		/// </summary>
		internal bool CheckMustExclude(string table)
		{
			// Comprueba las reglas
			foreach (SchemaExcludeRule rule in this)
				if (rule.CheckMustExclude(table))
					return true;
			// Si ha llegado hasta aquí es porque no se debe excluir
			return false;
		}
	}
}
