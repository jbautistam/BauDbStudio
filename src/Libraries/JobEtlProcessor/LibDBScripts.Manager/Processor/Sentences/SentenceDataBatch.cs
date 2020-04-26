using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibDbScripts.Manager.Processor.Sentences
{
	/// <summary>
	///		Sentencia de inicio de lote
	/// </summary>
	internal class SentenceDataBatch : SentenceBase
	{
		/// <summary>
		///		Tipo de comando
		/// </summary>
		internal enum BatchCommand
		{
			/// <summary>Arranca una transacción</summary>
			BeginTransaction,
			/// <summary>Confirma una transacción</summary>
			CommitTransaction,
			/// <summary>Deshace la transacción</summary>
			RollbackTransaction
		}

		/// <summary>
		///		Proveedor sobre el que se ejecuta la transacción
		/// </summary>
		internal string Target { get; set; }

		/// <summary>
		///		Indica el tipo de sentencia
		/// </summary>
		internal BatchCommand Type { get; set; }
	}
}
