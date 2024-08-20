using Bau.Libraries.LibCsvFiles;

namespace Bau.Libraries.ToDoManager.Application.TimeManagement.Repositories;

/// <summary>
///		Repositorio para archivos de Excel
/// </summary>
internal class CsvRepository
{
	/// <summary>
	///		Graba los datos
	/// </summary>
	internal void Save(string inputFolder, DateTime start, DateTime end, string outputFileName)
	{
		Export(Load(inputFolder, start, end), outputFileName);
		
	}

	/// <summary>
	///		Carga los datos
	/// </summary>
	private Models.TimeControlModel Load(string folder, DateTime start, DateTime end)
	{
		Models.TimeControlModel timeControl = new(DateOnly.FromDateTime(start));
		
			// Carga los archivos de las fechas
			for (DateTime date = start.Date; date <= end.Date; date = date.AddDays(1))
				timeControl.Merge(new TimeRepository().Load(folder, DateOnly.FromDateTime(date)));
			// Devuelve el control horario
			return timeControl;
	}

	/// <summary>
	///		Exporta los datos
	/// </summary>
	private void Export(Models.TimeControlModel timeControl, string fileName)
	{
		using (CsvWriter writer = new(new LibCsvFiles.Models.FileModel()))
		{
			// Abre el archivo
			writer.Open(fileName);
			// Crea las cabeceras
			writer.WriteHeaders(new List<string>
										{
											"Project",
											"Task",
											"Start",
											"End",
											"Elapsed"
										}
								);
			// Escribe los datos
			foreach (Models.ProjectModel project in timeControl.Projects)
				foreach (Models.TaskModel task in project.Tasks)
					foreach (Models.TimeModel time in task.Times)
						writer.WriteRow(new List<object>
												{
													time.Task.Project.Name,
													time.Task.Name,
													$"{time.Start:yyyy-MM-dd HH:mm:ss}",
													$"{time.End:yyyy-MM-dd HH:mm:ss}",
													FormatElapsed(time.End - time.Start)
												}
									   );
		}

		// Formatea una cadena con horas
		string FormatElapsed(TimeSpan value) => $"{value.TotalHours:00}:{value.Minutes:00}:{value.Seconds:00}";
	}
}
