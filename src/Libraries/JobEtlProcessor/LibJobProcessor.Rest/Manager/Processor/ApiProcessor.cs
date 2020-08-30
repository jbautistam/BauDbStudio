using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.LibHttpClient;

namespace Bau.Libraries.LibJobProcessor.Rest.Manager.Processor
{
	/// <summary>
	///		Procesador de llamadas a la API
	/// </summary>
	internal class ApiProcessor
	{
		internal ApiProcessor(ScriptInterpreter interpreter)
		{
			Interpreter = interpreter;
		}

		/// <summary>
		///		Procesa una sentencia de llamada
		/// </summary>
		internal async Task<List<BaseSentence>> ProcessAsync(BlockLogModel block, CallApiSentence sentence, CancellationToken cancellationToken)
		{
			HttpClientManager client = new HttpClientManager(new Uri(TransformVariables(sentence.Url)), TransformVariables(sentence.User), TransformVariables(sentence.Password));
			List<BaseSentence> sentences = new List<BaseSentence>();

				// Ejecuta las sentencias
				foreach (CallApiMethodSentence method in sentence.Methods)
					if (!HasError && !cancellationToken.IsCancellationRequested)
						sentences.AddRange(await ExecuteMethodAsync(block, method, client, cancellationToken));
				// Devuelve las sentencias que hay que ejecutar
				return sentences;
		}

		/// <summary>
		///		Ejecuta el método
		/// </summary>
		private async Task<List<BaseSentence>> ExecuteMethodAsync(BlockLogModel block, CallApiMethodSentence method, HttpClientManager client, CancellationToken cancellationToken)
		{
			List<BaseSentence> sentences = new List<BaseSentence>();
			int result = -1;

				// Ejecuta el método
				switch (method.Method)
				{
					case CallApiMethodSentence.MethodType.Post:
							result = await client.PostAsync(method.EndPoint, TransformVariables(method.Body), cancellationToken);
						break;
				}
				// Trata el resultado
				if (result == -1)
					Errors.Add($"Error when call {method.EndPoint}: {method.Method.ToString()}");
				else
					sentences = GetContinuation(method, result);
				// Devuelve la lista de sentencias
				return sentences;
		}

		/// <summary>
		///		Obtiene las sentencias que se deben ejecutar al encontrar un resultado
		/// </summary>
		private List<BaseSentence> GetContinuation(CallApiMethodSentence method, int result)
		{
			// Obtiene el grupo de sentencias que tiene que tratar el resultado
			foreach (CallApiResultSentence apiResult in method.Results)
				if (apiResult.ResultFrom >= result && apiResult.ResultTo <= result)
					return apiResult.Sentences;
			// Si no ha encontrado nada, busca un resultado que tenga tanto FROM como TO a 0
			foreach (CallApiResultSentence apiResult in method.Results)
				if (apiResult.ResultFrom == 0 && apiResult.ResultTo == 0)
					return apiResult.Sentences;
			// Si ha llegado hasta aquí, es porque no ha encontrado nada a ejecutar
			return new List<BaseSentence>();
		}

		/// <summary>
		///		Transforma las variables
		/// </summary>
		private string TransformVariables(string name)
		{
			// Transforma la cadena teniendo en cuenta las variables
			if (!string.IsNullOrWhiteSpace(name))
				foreach ((string key, object value) in Interpreter.Parameters.Enumerate())
				{
					string variable = "{{" + key + "}}";
					
						if (name.IndexOf(variable, StringComparison.CurrentCultureIgnoreCase) >= 0)
							name = name.ReplaceWithStringComparison(variable, value?.ToString());
				}
			// Devuelve el valor modificado
			return name;
		}

		/// <summary>
		///		Intérprete
		/// </summary>
		internal ScriptInterpreter Interpreter { get; }

		/// <summary>
		///		Errores
		/// </summary>
		internal List<string> Errors { get; } = new List<string>();

		/// <summary>
		///		Indica si ha habido algún error
		/// </summary>
		internal bool HasError
		{
			get { return Errors.Count > 0; }
		}
	}
}
