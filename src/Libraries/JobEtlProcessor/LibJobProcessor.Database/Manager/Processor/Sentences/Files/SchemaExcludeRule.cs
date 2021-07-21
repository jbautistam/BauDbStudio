using System;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Files
{
	/// <summary>
	///		Modelo para la exclusión de tablas en la importación / exportación de esquemas
	/// </summary>
	internal class SchemaExcludeRule
	{
		/// <summary>
		///		Modo de comparación
		/// </summary>
		internal enum Comparison
		{
			/// <summary>Comprueba si el nombre de la tabla contiene un valor</summary>
			Contains,
			/// <summary>Comprueba si el nombre de la tabla es igual que el valor</summary>
			Equals,
			/// <summary>Comprueba si el nombre de la tabla comienza por el valor</summary>
			Start,
			/// <summary>Comprueba si el nombre de la tabla finaliza con el valor</summary>
			End
		}

		/// <summary>
		///		Comprueba si se debe excluir una tabla atendiendo a esta regla
		/// </summary>
		internal bool CheckMustExclude(string table)
		{
			bool mustExclude = false;

				// Comprueba el valor de la regla con respecto al modo
				if (!CheckIsAtException(table))
					switch (Mode)
					{
						case Comparison.Contains:
								mustExclude = table.IndexOf(Name, StringComparison.CurrentCultureIgnoreCase) >= 0;
							break;
						case Comparison.End:
								mustExclude = table.EndsWith(Name, StringComparison.CurrentCultureIgnoreCase);
							break;
						case Comparison.Equals:
								mustExclude = table.Equals(Name, StringComparison.CurrentCultureIgnoreCase);
							break;
						case Comparison.Start:
								mustExclude = table.StartsWith(Name, StringComparison.CurrentCultureIgnoreCase);
							break;
					}
				// Devuelve el valor que indica si se debe excluir
				return mustExclude;
		}

		/// <summary>
		///		Comprueba si el nombre de tabla está entre las excepciones
		/// </summary>
		private bool CheckIsAtException(string table)
		{
			// Comprueba si está en la lista de excepciones
			if (!string.IsNullOrWhiteSpace(Except))
				foreach (string exception in Except.Split(','))
					if (table.Equals(exception, StringComparison.CurrentCultureIgnoreCase))
						return true;
			// Devuelve el valor que indica si está en la lista de excepciones
			return false;
		}

		/// <summary>
		///		Modo de comparación
		/// </summary>
		internal Comparison Mode { get; set; } = Comparison.Equals;

		/// <summary>
		///		Nombre
		/// </summary>
		internal string Name { get; set; }

		/// <summary>
		///		Nombres de tablas a las que no se aplica la regla
		/// </summary>
		internal string Except { get; set; }
	}
}
