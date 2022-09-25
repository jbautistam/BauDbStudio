using System;
using System.Linq;

using Bau.Libraries.LibReporting.Models.DataWarehouses;

namespace Bau.Libraries.LibReporting.Solution
{
	/// <summary>
	///		Manager para las soluciones de reporting
	/// </summary>
	public class ReportingSolutionManager
	{
		public ReportingSolutionManager()
		{
			Manager = new Application.ReportingManager();
		}

		/// <summary>
		///		Carga una solución
		/// </summary>
		public void LoadSolution(string fileName)
		{
			// Limpia los archivos de la solución
			ReportingSolution.Clear();
			// Carga los nuevos archivos de solución
			new Repositories.ReportingRepository(this).Load(fileName);
			// Carga los esquemas de la solución
			foreach (string file in ReportingSolution.Files)
				AddDataWarehouse(file);
		}

		/// <summary>
		///		Añade un origen de datos
		/// </summary>
		public void AddDataWarehouse(string fileName)
		{
			DataWarehouseModel dataWarehouse = new Repositories.DataWarehouseRepository().Load(Manager.Schema, fileName);

				// Añade el datawarehouse al esquema y al diccionario
				if (dataWarehouse != null)
				{
					// Añade el almacén de datos
					Manager.AddDataWarehouse(dataWarehouse);
					// Añade el archivo al diccionario
					ReportingSolution.DataWarehousesFiles.Add((dataWarehouse.Id, fileName));
					// Añade el archivo a la lista
					if (ReportingSolution.Files.FirstOrDefault(item => item.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)) == null)
						ReportingSolution.Files.Add(fileName);
				}
		}

		/// <summary>
		///		Elimina un origen de datos
		/// </summary>
		public void RemoveDataWarehouse(DataWarehouseModel dataWarehouse)
		{
			// Elimina el archivo de la lista
			for (int index = ReportingSolution.DataWarehousesFiles.Count - 1; index >= 0; index--)
			{
				(string dataWarehouseId, string file) = ReportingSolution.DataWarehousesFiles[index];

					// Elimina el archivo
					if (dataWarehouse.Id.Equals(dataWarehouseId, StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrWhiteSpace(file))
					{
						// Elimina el archivo
						for (int indexFile = ReportingSolution.Files.Count - 1; indexFile >= 0; indexFile--)
							if (ReportingSolution.Files[indexFile].Equals(file, StringComparison.CurrentCultureIgnoreCase))
								ReportingSolution.Files.RemoveAt(indexFile);
						// Elimina el origen de datos del diccionario
						ReportingSolution.DataWarehousesFiles.RemoveAt(index);
					}
			}
			// Elimina el origen de datos del esquema
			Manager.RemoveDataWarehouse(dataWarehouse);
		}

		/// <summary>
		///		Graba una solución
		/// </summary>
		public void SaveSolution(string fileName)
		{
			new Repositories.ReportingRepository(this).Save(fileName);
		}

		/// <summary>
		///		Carga los <see cref="DataWarehouseModel"/> de un archivo
		/// </summary>
		public void LoadDataWarehouse(string fileName)
		{
			DataWarehouseModel dataWarehouse = new Repositories.DataWarehouseRepository().Load(Manager.Schema, fileName);

				// Añade el dashboard al esquema
				if (dataWarehouse != null)
					Manager.Schema.DataWarehouses.Add(dataWarehouse);
		}

		/// <summary>
		///		Carga un <see cref="DataWarehouseModel"/> a partir de un archivo de esquema de base de datos
		/// </summary>
		public DataWarehouseModel ConvertSchemaDbToDataWarehouse(string name, string fileName)
		{
			return new Converters.SchemaConverter().Convert(Manager.Schema, name, fileName);
		}

		/// <summary>
		///		Combina un <see cref="DataWarehouseModel"/> con un archivo de esquema de base de datos
		/// </summary>
		public void Merge(DataWarehouseModel source, string schemaFile)
		{
			new Converters.SchemaConverter().Merge(source, schemaFile);
		}

		/// <summary>
		///		Convierte un esquema en scripts de generación de la base de datos
		/// </summary>
		public void ConvertSchemaReportingToSql(string schemaFileName, string outputFileName)
		{
			DataWarehouseModel dataWarehouse = new Repositories.DataWarehouseRepository().Load(Manager.Schema, schemaFileName);

				if (dataWarehouse != null)
					new Converters.SchemaScriptsConverter().Convert(dataWarehouse, outputFileName);
				else
					throw new Exception($"Can't load datawarehouse from {schemaFileName}");
		}

		/// <summary>
		///		Graba los datos de un <see cref="Models.DataWarehouses.DataWarehouseModel"/> en un archivo
		/// </summary>
		public void SaveDataWarehouse(DataWarehouseModel dataWarehouse)
		{
			string fileName = ReportingSolution.GetFileName(dataWarehouse);

				// Graba el archivo
				if (string.IsNullOrWhiteSpace(fileName))
					throw new NotImplementedException($"Cant find file name for '{dataWarehouse.Name}'");
				else
					new Repositories.DataWarehouseRepository().Save(dataWarehouse, fileName);
		}

		/// <summary>
		///		Graba los datos de un <see cref="DataWarehouseModel"/> en un archivo
		/// </summary>
		public void SaveDataWarehouse(DataWarehouseModel dataWarehouse, string fileName)
		{
			new Repositories.DataWarehouseRepository().Save(dataWarehouse, fileName);
		}

		/// <summary>
		///		Obtiene la SQL resultante de procesar una solicitud de informe
		/// </summary>
		public string GetSqlResponse(Requests.Models.ReportRequestModel request)
		{
			return Manager.GetSqlResponse(request);
		}

		/// <summary>
		///		<see cref="Application.ReportingManager"/> con el que trabaja la aplicación
		/// </summary>
		public Application.ReportingManager Manager { get; private set; }

		/// <summary>
		///		Solución para informes
		/// </summary>
		public Models.ReportingSolutionModel ReportingSolution { get; } = new Models.ReportingSolutionModel();
	}
}
