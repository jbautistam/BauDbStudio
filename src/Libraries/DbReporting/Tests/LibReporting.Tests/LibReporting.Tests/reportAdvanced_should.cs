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
	[InlineData("ReportingSales/Test-Reporting-Schema.xml", 
				"ReportingSales/Requests/Request_1.xml")]
	public void convert_to_sql(string fileName, string fileRequest)
	{
		ReportingSolutionManager manager = new();
		ReportRequestModel request = manager.LoadRequest(Tools.FileHelper.GetFullFileName(fileRequest));

			// Agrega el dataWarehouse
			manager.AddDataWarehouse(Tools.FileHelper.GetFullFileName(fileName));
			// Comprueba que realmente se haya cargado una solicitud
			request.Should().NotBeNull();
			// Obtiene la SQL del informe
			AssertSqlWithFile(manager.GetSqlResponse(request), Tools.FileHelper.GetResponseFile(fileRequest));
	}

	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes y ejecutar la cadena SQL contra la base de datos
	/// </summary>
	[Theory]
	[InlineData("ReportingSales/Test-Reporting-Schema.xml", 
				"ReportingSales/Requests/Request_1.xml",
				"Server=(local);Database=Roivolution_NunezDeArena_Reporting_Sales;Trusted_Connection=True;MultipleActiveResultSets=True")]
	public void execute_to_sql(string fileName, string fileRequest, string connectionString)
	{
		ReportingSolutionManager manager = new();
		ReportRequestModel request = manager.LoadRequest(Tools.FileHelper.GetFullFileName(fileRequest));

			// Agrega el dataWarehouse
			manager.AddDataWarehouse(Tools.FileHelper.GetFullFileName(fileName));
			// Comprueba que realmente se haya cargado una solicitud
			request.Should().NotBeNull();
			// Obtiene la SQL del informe
			using (System.Data.SqlClient.SqlConnection connection = new(connectionString))
			{
				System.Data.SqlClient.SqlCommand command = connection.CreateCommand();

					// Añade los argumentos al comando
					foreach (KeyValuePair<string, object?> parameter in request.Parameters)
					{
						System.Data.SqlClient.SqlParameter sqlParameter = command.CreateParameter();
						string key = parameter.Key;

							// Normaliza el parámetro
							if (!key.StartsWith("@"))
								key = "@" + key;
							// Asigna el parámetro
							sqlParameter.ParameterName = key;
							sqlParameter.Value = parameter.Value;
							// Añade el parámetro a la colección
							command.Parameters.Add(sqlParameter);
					}
					// Asigna las cadena al comando
					command.CommandText = manager.GetSqlResponse(request);
					command.CommandType = System.Data.CommandType.Text;
					// Abre la conexión
					connection.Open();
					// Ejecuta la consulta SQL
					command.ExecuteReader();
					// Cierra la conexión
					connection.Close();
			}
	}

	/// <summary>
	///		Compara la SQL de respuesta con el contenido de un archivo
	/// </summary>
	private void AssertSqlWithFile(string sql, string fileResponse)
	{
		AssertSql(sql, File.ReadAllText(Tools.FileHelper.GetFullFileName(fileResponse)));
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
}
