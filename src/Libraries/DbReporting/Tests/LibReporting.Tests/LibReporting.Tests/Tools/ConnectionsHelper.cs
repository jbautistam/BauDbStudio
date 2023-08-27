namespace LibReporting.Tests.Tools;

/// <summary>
///		Clase de ayuda para tratamiento de conexiones
/// </summary>
internal static class ConnectionsHelper
{
	// Cadena de conexión a la base de datos
	private static Dictionary<string, string> Connections = new()
														{
															{ "Test-Reporting-Roi.Reporting.xml", 
															  "Server=(local);Database=Roivolution_NunezDeArena_Reporting;Trusted_Connection=True;MultipleActiveResultSets=True"
															},
															{ "Test-Reporting-Sales.Reporting.xml", 
															  "Server=(local);Database=Roivolution_NunezDeArena_Reporting_Sales;Trusted_Connection=True;MultipleActiveResultSets=True"
															}
													    };

														
	/// <summary>
	///		Obtiene la cadena de conexión correspondiente a un archivo de esquema
	/// </summary>
	public static string GetConnectionStringForSchema(string schemaFile)
	{
		if (Connections.TryGetValue(Path.GetFileName(schemaFile), out string? connectionString))
			return connectionString;
		else
			return string.Empty;
	}
}
