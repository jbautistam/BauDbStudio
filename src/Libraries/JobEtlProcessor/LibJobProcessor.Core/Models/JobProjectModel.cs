using System;
using System.Collections.Generic;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibDataStructures.Base;

namespace Bau.Libraries.LibJobProcessor.Core.Models
{
	/// <summary>
	///		Clase con los datos del proyecto
	/// </summary>
	public class JobProjectModel : BaseExtendedModel
	{
		// Constantes públicas
		public const string GlobalContext = "Global";

		public JobProjectModel(string projectWorkPath)
		{
			ProjectWorkPath = projectWorkPath;
		}

		/// <summary>
		///		Obtiene la cadena de depuración 
		/// </summary>
		public string Debug()
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			string indent = new string(' ', 4);

				// Añade los datos
				builder.AppendLine($"Project {nameof(Name)}: {Name}");
				builder.AppendLine($"{indent}{nameof(Description)}: {Description}");
				builder.AppendLine($"{indent}{nameof(ProjectWorkPath)}: {ProjectWorkPath}");
				// Añade la depuración de los contextos
				if (Contexts.Count > 0)
				{
					builder.AppendLine($"{indent}{nameof(Contexts)}:");
					foreach (JobContextModel context in Contexts)
						context.Debug(builder, indent + new string(' ', 4));
				}
				// Añade la depuración de los pasos
				if (Jobs.Count == 0)
					builder.AppendLine($"{indent}No steps");
				else
				{
					builder.AppendLine($"{indent}{nameof(Jobs)}:");
					foreach (Jobs.JobStepModel step in Jobs)
						step.Debug(builder, indent + new string(' ', 4));
				}
				// Devuelve la cadena
				return builder.ToString();
		}

		/// <summary>
		///		Validación del proyecto
		/// </summary>
		public Errors.ErrorModelCollection Validate()
		{
			Errors.ErrorModelCollection errors = new Errors.ErrorModelCollection();

				// Comprueba los datos del proyecto
				if (string.IsNullOrWhiteSpace(ProjectWorkPath))
					errors.Add(this, null, $"{nameof(ProjectWorkPath)} is not defined");
				else if (!System.IO.Directory.Exists(ProjectWorkPath))
					errors.Add(this, null, $"The path '{ProjectWorkPath}' doesn't exists");
				else // Valida los pasos
					foreach (Jobs.JobStepModel job in Jobs)
						errors.AddRange(job.Validate(1));
				// Devuelve la colección de errores
				return errors;
		}

		/// <summary>
		///		Obtiene el nombre completo de un archivo a partir de los datos de los contextos
		/// </summary>
		public string GetFullFileName(string fileName, List<JobContextModel> contexts = null)
		{
			string fullFileName = fileName;
			bool isUncPath = false;

				// Sustituye los directorios del contexto global
				foreach (JobContextModel context in contexts ?? Contexts)
					foreach ((string key, string value) in context.Paths.Enumerate())
						fullFileName = fullFileName.ReplaceWithStringComparison("{{" + key + "}}", value);
				// Sustituye los directorios "Fijos"
				fullFileName = fullFileName.ReplaceWithStringComparison("{{ProjectWorkPath}}", ProjectWorkPath);
				fullFileName = fullFileName.ReplaceWithStringComparison("{{ContextWorkPath}}", ContextWorkPath);
				// Sustituye los directorios "especiales"
				fullFileName = fullFileName.ReplaceWithStringComparison("{{Date}}", $"{StartExecution:yyyy-MM-dd}");
				fullFileName = fullFileName.ReplaceWithStringComparison("{{Time}}", $"{StartExecution:HH_mm_ss}");
				fullFileName = fullFileName.ReplaceWithStringComparison("{{DateTime}}", $"{StartExecution:yyyy-MM-dd HH_mm_ss}");
				// Comprueba si es un directorio UNC
				if (fullFileName.StartsWith("\\\\"))
				{
					isUncPath = true;
					fullFileName = fullFileName.Substring(2);
				}
				// Reemplaza los caracteres / por \ y quita los duplicados
				fullFileName = fullFileName.Replace('/', '\\');
				while (fullFileName.IndexOf("\\\\") >= 0)
					fullFileName = fullFileName.Replace("\\\\", "\\");
				// Añade los caracteres de un directorio UNC
				if (isUncPath)
					fullFileName = "\\\\" + fullFileName;
				// Devuelve el nombre de archivo
				return fullFileName;
		}

		/// <summary>
		///		Directorio de proyecto
		/// </summary>
		public string ProjectWorkPath { get;  }

		/// <summary>
		///		Directorio de trabajo del contexto
		/// </summary>
		public string ContextWorkPath { get; set; }

		/// <summary>
		///		Fecha de inicio de la ejecución
		/// </summary>
		public DateTime StartExecution { get; set; } = DateTime.Now;

		/// <summary>
		///		Contexto de los diferentes tipos de procesadores
		/// </summary>
		public List<JobContextModel> Contexts { get; } = new List<JobContextModel>();

		/// <summary>
		///		Trabajos
		/// </summary>
		public BaseExtendedModelCollection<Jobs.JobStepModel> Jobs { get; } = new BaseExtendedModelCollection<Jobs.JobStepModel>();
	}
}
