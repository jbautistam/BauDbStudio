using System;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Manager.Controllers
{
	/// <summary>
	///		Opciones de lectura / escritura de un archivo Excel
	/// </summary>
	internal class ExcelfileOptions
	{
		/// <summary>
		///		Indice de la hoja
		/// </summary>
		internal int SheetIndex { get; set; }

		/// <summary>
		///		Indica si la hoja tiene una línea de cabecera
		/// </summary>
		internal bool WithHeader { get; set; }
	}
}
