using System;

namespace Bau.Libraries.LibJobProcessor.Core.Models.Jobs
{
	/// <summary>
	///		Base para los pasos de un script
	/// </summary>
	public class JobStepModel : LibDataStructures.Base.BaseExtendedModel
	{
		public JobStepModel(JobProjectModel project, JobStepModel parent = null)
		{
			Project = project;
			Parent = parent;
		}

		/// <summary>
		///		Depuración de los datos de paso
		/// </summary>
		internal void Debug(System.Text.StringBuilder builder, string indent)
		{
			// Añade los datos
			builder.AppendLine($"{indent}Job {nameof(Name)}: {Name}");
			builder.AppendLine($"{indent}  {nameof(Description)}: {Description}");
			builder.AppendLine($"{indent}  {nameof(ProcessorKey)}: {ProcessorKey} - {nameof(Target)}: {Target}- {nameof(ScriptFileName)}: {ScriptFileName}");
			builder.AppendLine($"{indent}  {nameof(StartWithPreviousError)}: {StartWithPreviousError} - {nameof(Parallel)}: {Parallel} - {nameof(Enabled)}: {Enabled}");
			// Añade la depuración del contexto
			if (!string.IsNullOrWhiteSpace(Context.ProcessorKey))
			{
				builder.AppendLine($"{indent}  {nameof(Context)}:");
				Context.Debug(builder, indent + new string(' ', 4));
			}
			// Añade la depuración de los pasos
			if (Steps.Count > 0)
			{
				builder.AppendLine($"{indent}  {nameof(Steps)}:");
				foreach (JobStepModel step in Steps)
					step.Debug(builder, indent + new string(' ', 4));
			}
		}

		/// <summary>
		///		Valida los errores
		/// </summary>
		internal Errors.ErrorModelCollection Validate(int indent)
		{
			Errors.ErrorModelCollection errors = new Errors.ErrorModelCollection();

				// Sólo comprueba lo errores si está activo
				if (Enabled)
				{
					// Comprueba los errores
					if (string.IsNullOrWhiteSpace(ProcessorKey))
						errors.Add(Project, this, $"{ProcessorKey} is undefined");
					if (string.IsNullOrWhiteSpace(ScriptFileName))
						errors.Add(Project, this, $"{ScriptFileName} is undefined");
					else if (!System.IO.File.Exists(Project.GetFullFileName(ScriptFileName)))
						errors.Add(Project, this, $"Can't find the file {ScriptFileName}");
					// Comprueba los errores de los hijos
					foreach (JobStepModel step in Steps)
						errors.AddRange(step.Validate(indent + 1));
				}
				// Devuelve la colección de errores
				return errors;
		}

		/// <summary>
		///		Proyecto
		/// </summary>
		public JobProjectModel Project { get; }

		/// <summary>
		///		Paso padre
		/// </summary>
		public JobStepModel Parent { get; }

		/// <summary>
		///		Clave del procesador
		/// </summary>
		public string ProcessorKey { get; set; }

		/// <summary>
		///		Nombre del archivo de script
		/// </summary>
		public string ScriptFileName { get; set; }

		/// <summary>
		///		Clave del destino
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		///		Contexto
		/// </summary>
		public JobContextModel Context { get; set; } = new JobContextModel();

		/// <summary>
		///		Pasos hijo
		/// </summary>
		public LibDataStructures.Base.BaseExtendedModelCollection<JobStepModel> Steps { get; } = new LibDataStructures.Base.BaseExtendedModelCollection<JobStepModel>();

		/// <summary>
		///		Indica si se debe iniciar el paso si el anterior ha devuelto algún error
		/// </summary>
		public bool StartWithPreviousError { get; set; }

		/// <summary>
		///		Indica si se deben ejecutar sus pasos en paralelo
		/// </summary>
		public bool Parallel { get; set; }

		/// <summary>
		///		Indica si el paso está activo
		/// </summary>
		public bool Enabled { get; set; } = true;
	}
}
