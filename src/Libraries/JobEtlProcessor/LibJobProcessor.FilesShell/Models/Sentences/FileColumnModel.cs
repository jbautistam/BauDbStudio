using System;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences
{
	/// <summary>
	///		Clase con los datos de una columna de un archivo
	/// </summary>
	internal class FileColumnModel
	{
		/// <summary>
		///		Tipo de columna
		/// </summary>
		internal enum ColumnType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Valor numérico</summary>
			Numeric,
			/// <summary>Fecha / hora</summary>
			DateTime,
			/// <summary>Cadena</summary>
			String,
			/// <summary>Valor lógico</summary>
			Boolean
		}

		/// <summary>
		///		Nombre de la columna
		/// </summary>
		internal string Name { get; set; }

		/// <summary>
		///		Tipo de la columna
		/// </summary>
		internal ColumnType Type { get; set; }
	}
}
