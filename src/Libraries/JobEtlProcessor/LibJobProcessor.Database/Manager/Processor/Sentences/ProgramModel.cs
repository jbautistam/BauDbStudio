using System;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences
{
	/// <summary>
	///		Clase con las instrucciones de un programa
	/// </summary>
	internal class ProgramModel
	{
		/// <summary>
		///		Instrucciones del programa
		/// </summary>
		internal SentenceCollection Sentences { get; } = new SentenceCollection();
	}
}
