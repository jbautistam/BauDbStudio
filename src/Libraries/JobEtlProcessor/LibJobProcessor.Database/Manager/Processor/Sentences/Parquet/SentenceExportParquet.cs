using System;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parquet
{
	/// <summary>
	///		Sentencia de exportación a arcivos Parquet
	/// </summary>
	internal class SentenceExportParquet : Compiler.LibInterpreter.Processor.Sentences.SentenceBase
	{
		///// <summary>
		/////		Comprueba los datos
		///// </summary>
		//protected override string Validate(ProjectModel project)
		//{
		//	string error = string.Empty;

		//		// Comprueba los datos
		//		if (string.IsNullOrWhiteSpace(Source))
		//			error += Environment.NewLine + $"{nameof(Source)} can't be empty";
		//		if (string.IsNullOrWhiteSpace(FileName))
		//			error += Environment.NewLine + $"{nameof(FileName)} can't be empty";
		//		if (string.IsNullOrWhiteSpace(LoadCommand))
		//			error += Environment.NewLine + $"Must define {nameof(LoadCommand)}";
		//		if (RecordsPerBlock < 1)
		//			error += Environment.NewLine + $"{nameof(RecordsPerBlock)} can't be less than 1";
		//		// Devuelve el error
		//		return error;
		//}

		/// <summary>
		///		Clave de la conexión origen
		/// </summary>
		internal string Source { get; set; }

		/// <summary>
		///		Nombre de archivo destino
		/// </summary>
		internal string FileName { get; set; }

		/// <summary>
		///		Comando de carga de datos para exportar
		/// </summary>
		internal Parameters.ProviderSentenceModel Command { get; set; }

		/// <summary>
		///		Número de registros por bloque de escritura
		/// </summary>
		internal int RecordsPerBlock { get; set; } = 200_000;

		/// <summary>
		///		Número de registros por grupo de filas
		/// </summary>
		internal int RowGroupSize { get; set; } = 45_000;

		/// <summary>
		///		Tiempo de espera para la ejecución de la sentencia
		/// </summary>
		internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);
	}
}
