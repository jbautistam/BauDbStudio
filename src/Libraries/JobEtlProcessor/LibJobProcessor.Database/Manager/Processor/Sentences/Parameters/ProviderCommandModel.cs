using System;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parameters
{
	/// <summary>
	///		Comando que se envía al proveedor
	/// </summary>
	internal class ProviderCommandModel
	{
		/// <summary>
		///		Nombre del comando
		/// </summary>
		internal string Name { get; set; }

		/// <summary>
		///		Valor del comando
		/// </summary>
		internal string Value { get; set; }
	}
}
