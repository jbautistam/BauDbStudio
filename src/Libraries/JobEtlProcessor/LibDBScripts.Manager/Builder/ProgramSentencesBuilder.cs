using System;
using System.Collections.Generic;

using Bau.Libraries.Compiler.LibInterpreter.Processor.Sentences;
using Bau.Libraries.LibDbScripts.Manager.Processor.Sentences;

namespace Bau.Libraries.LibDbScripts.Manager.Builder
{
	/// <summary>
	///		Generador para un <see cref=""/>
	/// </summary>
	internal class ProgramSentencesBuilder
	{
		internal ProgramSentencesBuilder(ProgramBuilder main, ProgramSentencesBuilder parent, SentenceCollection sentences)
		{
			Main = main;
			Parent = parent;
			Sentences = sentences;
		}

		/// <summary>
		///		Genera un bloque
		/// </summary>
		internal ProgramSentencesBuilder WithBlock(string message)
		{
			SentenceBlock block = new SentenceBlock
											{
												Name = message
											};

				// Añade el bloque a las sentencias
				Sentences.Add(block);
				// Crea un nuevo generador de sentencias
				return new ProgramSentencesBuilder(Main, this, block.Sentences);
		}

		/// <summary>
		///		Genera una sentencia de script
		/// </summary>
		internal ProgramSentencesBuilder WithScript(string target, string fileName, List<(string variable, string to)> mapping = null)
		{
			SentenceExecuteScript sentence = new SentenceExecuteScript();

				// Asigna los datos
				sentence.Target = target;
				sentence.FileName = fileName;
				if (mapping != null)
					sentence.Mapping.AddRange(mapping);
				// Añade la sentencia a la colección
				Sentences.Add(sentence);
				// Devuelve el generador
				return this;
		}

		/// <summary>
		///		Obtiene el generador anterior
		/// </summary>
		internal ProgramSentencesBuilder Back()
		{
			return Parent;
		}

		/// <summary>
		///		Generador principal
		/// </summary>
		private ProgramBuilder Main { get; }

		/// <summary>
		///		Generador padre
		/// </summary>
		private ProgramSentencesBuilder Parent { get; }

		/// <summary>
		///		Bloque de sentencias
		/// </summary>
		private SentenceCollection Sentences { get; }
	}
}