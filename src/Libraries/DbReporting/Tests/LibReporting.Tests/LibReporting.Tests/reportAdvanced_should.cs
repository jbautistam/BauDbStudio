using FluentAssertions;
using Bau.Libraries.LibReporting.Solution;
using Bau.Libraries.LibReporting.Requests.Models;

namespace LibReporting.Tests;

/// <summary>
///		Pruebas de generación de SQL de informes avanzados
/// </summary>
public class reportAdvanced_should
{
	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes
	/// </summary>
	[Theory]
	[InlineData("ReportingSales/Reporting-Schema.Reporting Sales - Nuñez de Arenas.xml", 
				"ReportingSales/Requests/Request_1.xml", "ReportingSales/Requests/Response_1.sql")]
	public void convert_to_sql(string fileName, string fileRequest, string fileResponse)
	{
		ReportingSolutionManager manager = new();
		ReportRequestModel request = manager.LoadRequest(GetFullFileName(fileRequest));

			// Agrega el dataWarehouse
			manager.AddDataWarehouse(GetFullFileName(fileName));
			// Comprueba que realmente se haya cargado una solicitud
			request.Should().NotBeNull();
			// Obtiene la SQL del informe
			AssertSqlWithFile(manager.GetSqlResponse(request), fileResponse);
	}

	/// <summary>
	///		Compara la SQL de respuesta con el contenido de un archivo
	/// </summary>
	private void AssertSqlWithFile(string sql, string fileResponse)
	{
		AssertSql(sql, File.ReadAllText(GetFullFileName(fileResponse)));
	}

	/// <summary>
	///		Compara las SQL
	/// </summary>
	private void AssertSql(string source, string target)
	{
		Normalize(source).Should().BeEquivalentTo(Normalize(target));
	}

	/// <summary>
	///		Normaliza una cadena SQL
	/// </summary>
	private string Normalize(string sql)
	{
		// Quita saltos de línea y tabuladores
		sql = sql.Replace('\n', ' ');
		sql = sql.Replace('\r', ' ');
		sql = sql.Replace('\t', ' ');
		// Quita espacios dobles
		while (!string.IsNullOrWhiteSpace(sql) && sql.IndexOf("  ") >= 0)
			sql = sql.Replace("  ", " ");
		// Quita espacios iniciales / finales
		if (!string.IsNullOrWhiteSpace(sql))
			sql = sql.Trim();
		// Devuelve la cadena normalizada
		return sql;
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
