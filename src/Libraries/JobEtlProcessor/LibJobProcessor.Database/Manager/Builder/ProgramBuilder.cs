using System;

using Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Builder
{
	/// <summary>
	///		Generador de <see cref="ProgramModel"/>
	/// </summary>
	internal class ProgramBuilder
	{
		/// <summary>
		///		Genera un bloque en el programa
		/// </summary>
		internal ProgramSentencesBuilder WithBlock(string message)
		{
			ProgramSentencesBuilder builder = new ProgramSentencesBuilder(this, null, Program.Sentences);

				// Añade un bloque al generador
				return builder.WithBlock(message);
		}

		/// <summary>
		///		Genera el programa
		/// </summary>
		internal ProgramModel Build()
		{
			return Program;
		}

		/// <summary>
		///		Programa
		/// </summary>
		private ProgramModel Program { get; } = new ProgramModel();
	}
}
