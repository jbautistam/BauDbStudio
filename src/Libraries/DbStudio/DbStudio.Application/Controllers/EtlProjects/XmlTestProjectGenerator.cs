using System;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibLogger.Models.Log;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbStudio.Application.Controllers.EtlProjects
{
	/// <summary>
	///		Generador del proyecto de pruebas
	/// </summary>
	public class XmlTestProjectGenerator
	{
		public XmlTestProjectGenerator(SolutionManager manager, Models.Connections.ConnectionModel connection, string dataBase, string path)
		{
			Manager = manager;
			Connection = connection;
			DataBase = dataBase;
			Path = path;
		}

		/// <summary>
		///		Genera los archivos
		/// </summary>
		public async Task<bool> GenerateAsync(BlockLogModel block, string provider, string pathVariable, string databaseVariable, string sufixTestTables,
											  string fileNameProject, string fileNameTest, string fileNameAssert, CancellationToken cancellationToken)
		{
			// Limpia los errores
			Errors.Clear();
			// Carga el esquema
			await Manager.ConnectionManager.LoadSchemaAsync(Connection, cancellationToken);
			// Genera los archivos
			if (!cancellationToken.IsCancellationRequested)
			{
				// Genera el archivo de proyecto
				if (!string.IsNullOrWhiteSpace(fileNameProject))
					SaveProjectFile(block, fileNameProject, fileNameTest, fileNameAssert);
				// Genera el archivo de prueba
				if (!string.IsNullOrWhiteSpace(fileNameTest))
					SaveTestFile(block, fileNameTest, provider, pathVariable, databaseVariable, sufixTestTables);
				// Genera el archivo de comparación
				if (!string.IsNullOrWhiteSpace(fileNameAssert))
					SaveAssertFile(block, fileNameAssert, provider, pathVariable, databaseVariable, sufixTestTables);
			}
			// Devuelve el valor que indica si la generación ha sido correcta
			return Errors.Count == 0;
		}

		/// <summary>
		///		Graba el archivo de proyecto
		/// </summary>
		private void SaveProjectFile(BlockLogModel block, string fileNameProject, string fileNameTest, string fileNameAssert)
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add("EtlProject");

				// Log
				block.Info($"Start generation project '{fileNameProject}'");
				// Añade el nombre del proyecto
				rootML.Nodes.Add("Name", "Create test files");
				// Añade los archivos de scripts
				if (!string.IsNullOrWhiteSpace(fileNameTest))
					rootML.Nodes.Add(GetStepNode(fileNameTest, "Create test file", false));
				if (!string.IsNullOrWhiteSpace(fileNameAssert))
					rootML.Nodes.Add(GetStepNode(fileNameAssert, "Assert test data", false));
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(System.IO.Path.Combine(Path, fileNameProject), fileML);
				// Log
				block.Info($"End generation project '{fileNameProject}'");
		}

		/// <summary>
		///		Calcula el nodo de un paso de ejecución de script
		/// </summary>
		private MLNode GetStepNode(string fileName, string message, bool enabled)
		{
			MLNode rootML = new MLNode("Step");

				// Añade los atributos
				rootML.Attributes.Add("Processor", "DbManager");
				rootML.Attributes.Add("Script", "{{ProjectWorkPath}}/" + fileName);
				rootML.Attributes.Add("StartWithPreviousError", false);
				rootML.Attributes.Add("Enabled", enabled);
				// Añade el nombre
				rootML.Nodes.Add("Name", message);
				// Devuelve el nodo
				return rootML;
		}

		/// <summary>
		///		Graba un archivo de pruebas
		/// </summary>
		private void SaveTestFile(BlockLogModel block, string fileName, string provider, string pathVariable, string databaseVariable, string sufixTestTables)
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add("DbScript");

				// Log
				block.Info($"Start generate file '{fileName}'");
				// Crea los nodos de creación de los archivos de pruebas
				foreach (Models.Connections.ConnectionTableModel table in Connection.Tables)
					if (!string.IsNullOrWhiteSpace(databaseVariable) && databaseVariable.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
					{
						MLNode blockML = rootML.Nodes.Add("Block");
						MLNode nodeDropML = blockML.Nodes.Add("Execute");
						MLNode nodeCreateML = blockML.Nodes.Add("Execute");

							// Mensaje del bloque
							blockML.Attributes.Add("Message", $"Test table {table.Name}");
							// Crea el nodo para borrar la tabla
							nodeDropML.Attributes.Add("Target", provider);
							nodeDropML.Value = $"DROP TABLE IF EXISTS {table.Name}{sufixTestTables}";
							// Crea el nodo de creación de la tabla
							nodeCreateML.Attributes.Add("Target", provider);
							nodeCreateML.Value = Environment.NewLine + $"\t\t\tCREATE EXTERNAL TABLE {table.Name}{sufixTestTables} STORED AS parquet";
							nodeCreateML.Value += Environment.NewLine + "\t\t\t\tLOCATION '{{" + pathVariable + "}}" + $"/testing/{table.Name}{sufixTestTables}.parquet' AS ";
							nodeCreateML.Value += Environment.NewLine + "\t\t\t\tSELECT * FROM {{" + databaseVariable + "}}" + $".{table.Name}";
							nodeCreateML.Value += Environment.NewLine;
					}
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(System.IO.Path.Combine(Path, fileName), fileML);
				// Log
				block.Info($"End generation file '{fileName}'");
		}

		/// <summary>
		///		Graba el archivo de comparación de tablas
		/// </summary>
		private void SaveAssertFile(BlockLogModel block, string fileName, string provider, string pathVariable, string databaseVariable, string sufixTestTables)
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add("DbScript");

				// Log
				block.Info($"Start generate file '{fileName}'");
				// Crea los nodos de creación de los archivos de pruebas
				foreach (Models.Connections.ConnectionTableModel table in Connection.Tables)
					if (!string.IsNullOrWhiteSpace(databaseVariable) && databaseVariable.Equals(table.Schema, StringComparison.CurrentCultureIgnoreCase))
					{
						MLNode blockML = rootML.Nodes.Add("Block");
						MLNode nodeAssertML = blockML.Nodes.Add("AssertScalar");

							// Mensaje del bloque
							blockML.Attributes.Add("Message", $"Assert table {table.Name}");
							// Añade los atributos de la sentencia de comparación
							nodeAssertML.Attributes.Add("Target", provider);
							nodeAssertML.Attributes.Add("Message", $"Assert table {table.Name}");
							nodeAssertML.Attributes.Add("Result", 0);
							// Añade la cadena de consulta
							nodeAssertML.Value = GetSqlCompare(table, pathVariable, databaseVariable, sufixTestTables);
					}
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(System.IO.Path.Combine(Path, fileName), fileML);
				// Log
				block.Info($"End generation file '{fileName}'");
		}

		/// <summary>
		///		Obtiene la cadena SQL de comparación de tablas
		/// </summary>
		private string GetSqlCompare(ConnectionTableModel table, string pathVariable, string databaseVariable, string sufixTestTables)
		{
			string sql = string.Empty;

				// Crea la cadena SQL de comparación si hay algún campo en la tabla
				if (table.Fields.Count > 0)
				{
					sql += Environment.NewLine + "\t\t\tSELECT COUNT(*) AS Number";
					sql += Environment.NewLine + "\t\t\t\tFROM parquet.`{{" + pathVariable + "}}" + $"/testing/{table.Name}{sufixTestTables}.parquet` AS Test";
					sql += Environment.NewLine + "\t\t\t\tFULL OUTER JOIN {{" + databaseVariable + "}}.`" + table.Name + "` AS Target";
					sql += Environment.NewLine + "\t\t\t\t\tON " + GetSqlCompareFields(table).Trim();
					sql += Environment.NewLine + $"\t\t\t\tWHERE Target.`{table.Fields[0].Name}` IS NULL";
					sql += Environment.NewLine + $"\t\t\t\t\tOR Test.`{table.Fields[0].Name}` IS NULL";
					sql += Environment.NewLine;
				}
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Obtiene la cadena de comparación de campos
		/// </summary>
		private string GetSqlCompareFields(ConnectionTableModel table)
		{
			string sql = string.Empty;

				// Añade la comparación de campos
				foreach (ConnectionTableFieldModel field in table.Fields)
				{
					// Añade el operador si es necesario
					if (!string.IsNullOrWhiteSpace(sql))
						sql += "\t\t\t\t\t\tAND ";
					// Añade la comparación
					sql += $"IfNull(Target.`{field.Name}`, '') = IfNull(Test.`{field.Name}`, '')" + Environment.NewLine;
				}
				// Devuelve la cadena SQL
				return sql;
		}

		/// <summary>
		///		Controlador de la solución
		/// </summary>
		public SolutionManager Manager { get; }

		/// <summary>
		///		Conexión para la que se generan las pruebas
		/// </summary>
		public ConnectionModel Connection { get; }

		/// <summary>
		///		Base de datos para la que se generan las pruebas
		/// </summary>
		public string DataBase { get; }

		/// <summary>
		///		Directorio de los archivos de proyecto
		/// </summary>
		public string Path { get; }

		/// <summary>
		///		Errores de la generación
		/// </summary>
		public System.Collections.Generic.List<string> Errors { get; } = new System.Collections.Generic.List<string>();
	}
}
