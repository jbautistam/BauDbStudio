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
	[InlineData("ReportingSales/Test-Reporting-Schema.xml", "Test reporting schema")]
	public void load_datawarehouse(string fileName, string dataWarehouseName)
	{
		ReportingSolutionManager manager = new();

			// Añade el esquema
			manager.AddDataWarehouse(Tools.FileHelper.GetFullFileName(fileName));
			// Comprueba si tiene datos
			manager.Manager.Schema.DataWarehouses.Count.Should().Be(1);
			// Comprueba los datos de un almacén
			foreach (DataWarehouseModel dataWarehouse in manager.Manager.Schema.DataWarehouses.EnumerateValues())
				if (dataWarehouse.Name.Equals(dataWarehouseName, StringComparison.CurrentCultureIgnoreCase))
					dataWarehouse.Reports.Count.Should().Be(2);
	}

	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y un informe específico
	/// </summary>
	[Theory]
	[InlineData("ReportingSales/Test-Reporting-Schema.xml", "Test reporting schema", "Test_01.report.xml")]
	public void load_report_advanced(string fileName, string dataWarehoseName, string fileReport)
	{
		ReportingSolutionManager manager = new();
		bool found = false;

			// Añade el esquema
			manager.AddDataWarehouse(Tools.FileHelper.GetFullFileName(fileName));
			// Comprueba si tiene datos
			manager.Manager.Schema.DataWarehouses.Count.Should().Be(1);
			// Comprueba los datos de un almacén
			foreach (DataWarehouseModel dataWarehouse in manager.Manager.Schema.DataWarehouses.EnumerateValues())
				if (dataWarehouse.Name.Equals(dataWarehoseName, StringComparison.CurrentCultureIgnoreCase))
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
}