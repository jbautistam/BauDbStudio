using FluentAssertions;
using Bau.Libraries.LibReporting.Solution;
using Bau.Libraries.LibReporting.Models.DataWarehouses;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports;

namespace LibReporting.Tests;

/// <summary>
///		Pruebas de generación de informes avanzados
/// </summary>
public class schemaLoad_should
{
	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes
	/// </summary>
	[Theory]
	[InlineData("ReportingSales/Reporting-Schema.Reporting Sales - Nuñez de Arenas.xml", "Schema.Reporting Sales - Nuñez de Arenas")]
	public void load_datawarehouse(string fileName, string name)
	{
		ReportingSolutionManager manager = new();

			// Añade el esquema
			manager.AddDataWarehouse(GetFullFileName(fileName));
			// Comprueba si tiene datos
			manager.Manager.Schema.DataWarehouses.Count.Should().Be(1);
			// Comprueba los datos de un almacén
			foreach (DataWarehouseModel dataWarehouse in manager.Manager.Schema.DataWarehouses.EnumerateValues())
				if (dataWarehouse.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
					dataWarehouse.Reports.Count.Should().Be(2);
	}
	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y un informe específico
	/// </summary>
	[Theory]
	[InlineData("ReportingSales/Reporting-Schema.Reporting Sales - Nuñez de Arenas.xml", "Schema.Reporting Sales - Nuñez de Arenas",
				"SalesAnalysisDateWithTransactions-NewSchema.report.xml")]
	public void load_report_advanced(string fileName, string name, string fileReport)
	{
		ReportingSolutionManager manager = new();
		bool found = false;

			// Añade el esquema
			manager.AddDataWarehouse(GetFullFileName(fileName));
			// Comprueba si tiene datos
			manager.Manager.Schema.DataWarehouses.Count.Should().Be(1);
			// Comprueba los datos de un almacén
			foreach (DataWarehouseModel dataWarehouse in manager.Manager.Schema.DataWarehouses.EnumerateValues())
				if (dataWarehouse.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
					foreach (ReportBaseModel report in dataWarehouse.Reports.EnumerateValues())
						if (report is ReportAdvancedModel reportAdvanced && 
								Path.GetFileName(reportAdvanced.FileName).Equals(fileReport, StringComparison.CurrentCultureIgnoreCase))
						{
							reportAdvanced.DataWarehouseKey.Should().BeEquivalentTo(dataWarehouse.Name);
							found = true;
						}
			// Comprueba que se haya encontrado el archivo
			found.Should().BeTrue();
	}

	/// <summary>
	///		Obtiene el nombre completo de un archivo
	/// </summary>
	private string GetFullFileName(string fileName)
	{
		return Path.Combine(GetExecutionPath(), "Data", fileName);
	}

	/// <summary>
	///		Obtiene el directorio de ejecución del proyecto
	/// </summary>
	private string GetExecutionPath()
	{
		return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? default!;
	}
}