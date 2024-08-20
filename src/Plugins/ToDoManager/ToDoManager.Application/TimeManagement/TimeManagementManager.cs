namespace Bau.Libraries.ToDoManager.Application.TimeManagement;

/// <summary>
///		Manager de la administración
/// </summary>
public class TimeManagementManager
{
	public TimeManagementManager(ToDoManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Carga los datos del archivo de control horario de un día
	/// </summary>
	public Models.TimeControlModel Load(string folder, DateOnly date)
	{
		// Carga el archivo
		try
		{
			return new Repositories.TimeRepository().Load(folder, date);
		}
		catch (Exception exception)
		{
			System.Diagnostics.Debug.WriteLine($"Error when load TimeManagement file {folder} - {date:yyyy-MM-dd}. {exception.Message}");
		}
		// Si ha habido un error, devuelve un objeto vacío
		return new Models.TimeControlModel(date);
	}

	/// <summary>
	///		Graba los datos del archivo de control horario
	/// </summary>
	public void Save(Models.TimeControlModel scheduler, string folder)
	{
		new Repositories.TimeRepository().Save(scheduler, folder);
	}

	/// <summary>
	///		Exporta a Csv
	/// </summary>
	public void ExportToCsv(string inputFolder, DateTime start, DateTime end, string outputFileName)
	{
		new Repositories.CsvRepository().Save(inputFolder, start, end, outputFileName);
	}

	/// <summary>
	///		Manager
	/// </summary>
	public ToDoManager Manager { get; }
}
