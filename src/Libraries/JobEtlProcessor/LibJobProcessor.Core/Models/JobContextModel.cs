using System;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDataStructures.Trees;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Core.Models
{
	/// <summary>
	///		Clase con el contexto de un procesador
	/// </summary>
	public class JobContextModel
	{
		/// <summary>
		///		Depuración del contexto
		/// </summary>
		internal void Debug(System.Text.StringBuilder builder, string indent)
		{
			// Añade los datos
			builder.AppendLine($"{indent}Context {nameof(ProcessorKey)}: {ProcessorKey}");
			// Muestra la información de los nodos
			if (Tree.Nodes.Count > 0)
			{
				builder.AppendLine($"{indent}    Nodes:");
				Tree.Debug(builder, indent + new string(' ', 8));
			}
			// Añade los parámetros
			if (Parameters.Count > 0)
			{
				builder.AppendLine($"{indent}   {nameof(Parameters)}:");
				foreach((string _, JobParameterModel parameter) in Parameters.Enumerate())
					parameter.Debug(builder, indent + new string(' ', 8));
			}
			// Añade los directorios
			if (Paths.Count > 0)
			{
				builder.AppendLine($"{indent}   {nameof(Paths)}:");
				foreach((string key, string path) in Paths.Enumerate())
					builder.AppendLine(indent + new string(' ', 8) + $"{key}: {path}");
			}
		}

		/// <summary>
		///		Clave del procesador
		/// </summary>
		public string ProcessorKey { get; set; } 

		/// <summary>
		///		Parámetros del procesador
		/// </summary>
		public TreeModel Tree { get; } = new TreeModel();

		/// <summary>
		///		Parámetros de ejecución del trabajo
		/// </summary>
		public NormalizedDictionary<JobParameterModel> Parameters { get; } = new NormalizedDictionary<JobParameterModel>();

		/// <summary>
		///		Directorios
		/// </summary>
		public NormalizedDictionary<string> Paths { get; } = new NormalizedDictionary<string>();
	}
}
