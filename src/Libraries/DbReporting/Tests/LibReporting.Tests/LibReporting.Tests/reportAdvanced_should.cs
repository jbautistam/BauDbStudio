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
				"ReportingSales/Test_01/NoDimensions.request.xml")]
	[InlineData("ReportingSales/Test-Reporting-Schema.xml", 
				"ReportingSales/Test_02/Calendar_Products_PointsOfSale.request.xml")]
	[InlineData("ReportingSales/Test-Reporting-Schema.xml", 
				"ReportingSales/Test_Sales_Grouped/NoDimensions.request.xml")]
	[InlineData("ReportingSales/Test-Reporting-Schema.xml", 
				"ReportingSales/Test_Sales_Grouped/Calendar_Date.request.xml")]
	public void convert_to_sql(string schema, string fileRequest, string charStart = "[", string charEnd = "]")
	{
		string schemaFileName = Tools.FileHelper.GetFullFileName(schema);
		string requestFileName = Tools.FileHelper.GetFullFileName(fileRequest);

			if (!File.Exists(schemaFileName))
				throw new NotImplementedException($"Can't find the file {schemaFileName}");
			else if (!File.Exists(requestFileName))
				throw new NotImplementedException($"Can't find the file {requestFileName}");
			else
			{
				ReportingSolutionManager manager = new();
				ReportRequestModel request = manager.LoadRequest(requestFileName);
					
					// Cambia la configuración del proveedor
					manager.Manager.Schema.Configuration.CharFieldNameStart = charStart;
					manager.Manager.Schema.Configuration.CharFieldNameEnd = charEnd;
					// Agrega el dataWarehouse
					manager.AddDataWarehouse(schemaFileName);
					// Comprueba que realmente se haya cargado una solicitud
					request.Should().NotBeNull();
					// Obtiene la SQL del informe
					AssertSqlWithFile(manager.GetSqlResponse(request), Tools.FileHelper.GetResponseFile(fileRequest), charStart, charEnd);
			}
	}

	/// <summary>
	///		Comprueba si se puede cargar un esquema de base de datos y sus informes y ejecutar la cadena SQL contra la base de datos
	/// </summary>
	[Theory]
	[InlineData("ReportingSales/Test-Reporting-Schema.xml", 
				"ReportingSales/Test_01/NoDimensions.request.xml",
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
	private void AssertSqlWithFile(string sqlGenerated, string fileResponse, string charStart, string charEnd)
	{
		AssertSql(sqlGenerated, File.ReadAllText(Tools.FileHelper.GetFullFileName(fileResponse)), charStart, charEnd);
	}

	/// <summary>
	///		Compara las SQL
	/// </summary>
	private void AssertSql(string generated, string sqlFile, string charStart, string charEnd)
	{
		// Log
		System.Diagnostics.Debug.WriteLine("Compare generated " + new string('-', 80));
		System.Diagnostics.Debug.WriteLine(Normalize(generated, charStart, charEnd));
		System.Diagnostics.Debug.WriteLine("Compare source " + new string('-', 80));
		System.Diagnostics.Debug.WriteLine(Normalize(sqlFile, charStart, charEnd));
		System.Diagnostics.Debug.WriteLine(new string('-', 80));
		// Assert
		Normalize(generated, charStart, charEnd).Should().BeEquivalentTo(Normalize(sqlFile, charStart, charEnd));
	}

	/// <summary>
	///		Normaliza una cadena SQL
	/// </summary>
	private string Normalize(string sql, string charStart, string charEnd)
	{
		// Quita saltos de línea y tabuladores
		sql = sql.Replace('\n', ' ');
		sql = sql.Replace('\r', ' ');
		sql = sql.Replace('\t', ' ');
		// Quita espacios dobles
		while (!string.IsNullOrWhiteSpace(sql) && sql.IndexOf("  ") >= 0)
			sql = sql.Replace("  ", " ");
		// Quita los malos paréntesis, corchetes...
		sql = sql.Replace("( ", "(");
		sql = sql.Replace(" )", ")");
		sql = sql.Replace("[ ", "[");
		sql = sql.Replace(" ]", "]");
		//// Cambia los caracteres de inicio y fin
		//if (!string.IsNullOrWhiteSpace(charStart))
		//	sql = sql.Replace("[", charStart);
		//if (!string.IsNullOrWhiteSpace(charEnd))
		//	sql = sql.Replace("]", charEnd);
		// Quita espacios iniciales / finales
		if (!string.IsNullOrWhiteSpace(sql))
			sql = sql.Trim();
		// Devuelve la cadena normalizada
		return sql;
	}
}
