using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibLogger.Models.Log;

namespace Bau.Libraries.LibJobProcessor.Rest.Manager
{
	/// <summary>
	///		Intérprete del script
	/// </summary>
	internal class ScriptInterpreter
	{
		internal ScriptInterpreter(JobRestManager manager, JobStepModel step)
		{
			Manager = manager;
			Step = step;
		}

		/// <summary>
		///		Procesa las sentencias del script
		/// </summary>
		internal async Task<bool> ProcessAsync(BlockLogModel block, List<BaseSentence> program, 
											   NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			// Guarda los parámetros de entrada
			Parameters = parameters;
			// Ejecuta el programa
			await ExecuteAsync(block, program, cancellationToken);
			// Devuelve el valor que indica si todo es correcto
			return !HasError;
		}

		/// <summary>
		///		Procesa los datos
		/// </summary>
		private async Task ExecuteAsync(BlockLogModel block, List<BaseSentence> sentences, CancellationToken cancellationToken)
		{
			foreach (BaseSentence sentenceBase in sentences)
				if (!HasError && sentenceBase.Enabled && !cancellationToken.IsCancellationRequested)
					switch (sentenceBase)
					{
						case BlockSentence sentence:
								await ProcessBlockAsync(block, sentence, cancellationToken);
							break;
						case ExceptionSentence sentence:
								ProcessException(block, sentence);
							break;
						case CallApiSentence sentence:
								await ProcessCallApiAsync(block, sentence, cancellationToken);
							break;
					}
		}

		/// <summary>
		///		Procesa un bloque de sentencias
		/// </summary>
		private async Task ProcessBlockAsync(BlockLogModel parent, BlockSentence sentence, CancellationToken cancellationToken)
		{
			using (BlockLogModel block = parent.CreateBlock(LogModel.LogType.Info, sentence.GetMessage("Start block")))
			{
				await ExecuteAsync(block, sentence.Sentences, cancellationToken);
			}
		}

		/// <summary>
		///		Procesa una excepción
		/// </summary>
		private void ProcessException(BlockLogModel parent, ExceptionSentence sentence)
		{
			if (string.IsNullOrWhiteSpace(sentence.Message))
				AddError(parent, "Unknown exception");
			else
				AddError(parent, sentence.Message);
		}

		/// <summary>
		///		Procesa una llamada a la API
		/// </summary>
		private async Task ProcessCallApiAsync(BlockLogModel block, CallApiSentence sentence, CancellationToken cancellationToken)
		{
			List<BaseSentence> sentences = await new Processor.ApiProcessor(this).ProcessAsync(block, sentence, cancellationToken);

				if (sentences.Count > 0)
					await ExecuteAsync(block, sentences, cancellationToken);
		}

		/// <summary>
		///		Añade un error a la colección
		/// </summary>
		private void AddError(BlockLogModel block, string message, Exception exception = null)
		{
			block.Error(message, exception);
			Errors.Add(message + Environment.NewLine + exception?.Message);
		}

		/// <summary>
		///		Manager
		/// </summary>
		private JobRestManager Manager { get; }

		/// <summary>
		///		Paso de ejecución
		/// </summary>
		private JobStepModel Step { get; }

		/// <summary>
		///		Parámetros de entrada
		/// </summary>
		internal NormalizedDictionary<object> Parameters { get; set; }

		/// <summary>
		///		Errores de proceso
		/// </summary>
		internal List<string> Errors { get; } = new List<string>();

		/// <summary>
		///		Indica si ha habido algún error
		/// </summary>
		public bool HasError 
		{ 
			get { return Errors.Count > 0; }
		}
	}
}