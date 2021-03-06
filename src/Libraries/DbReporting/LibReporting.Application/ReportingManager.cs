using System;
using System.Linq;

namespace Bau.Libraries.LibReporting.Application
{
	/// <summary>
	///		Manager para reporting
	/// </summary>
	public class ReportingManager
	{
		public ReportingManager()
		{
			Schema = new Models.ReportingSchemaModel();
		}

		/// <summary>
		///		Carga una solución
		/// </summary>
		public void LoadSolution(string fileName)
		{
			// Limpia los archivos de la solución
			ReportingSolution.Clear();
			// Asigna el nombre de archivo
			ReportingSolution.FileName = fileName;
			// Carga los nuevos archivos de solución
			new Repositories.ReportingRepository(this).Load(fileName);
			// Limpia los esquemas
			Schema = new Models.ReportingSchemaModel();
			// Carga los esquemas de la solución
			foreach (string file in ReportingSolution.Files)
				AddDataWarehouse(file);
		}

		/// <summary>
		///		Añade un origen de datos
		/// </summary>
		public void AddDataWarehouse(string fileName)
		{
			Models.DataWarehouses.DataWarehouseModel dataWarehouse = new Repositories.DataWarehouseRepository().Load(Schema, fileName);

				// Añade el datawarehouse al esquema y al diccionario
				if (dataWarehouse != null)
				{
					// Añade el almacén de datos
					Schema.DataWarehouses.Add(dataWarehouse);
					// Añade el archivo al diccionario
					ReportingSolution.DataWarehousesFiles.Add(dataWarehouse.Id, fileName);
					// Añade el archivo a la lista
					if (ReportingSolution.Files.FirstOrDefault(item => item.Equals(fileName, StringComparison.CurrentCultureIgnoreCase)) == null)
						ReportingSolution.Files.Add(fileName);
				}
		}

		/// <summary>
		///		Elimina un origen de datos
		/// </summary>
		public void RemoveDataWarehouse(Models.DataWarehouses.DataWarehouseModel dataWarehouse)
		{
			string file = ReportingSolution.DataWarehousesFiles[dataWarehouse.Id];

				// Elimina el archivo
				if (!string.IsNullOrWhiteSpace(file))
				{
					// Elimina el origen de datos del diccionario
					ReportingSolution.DataWarehousesFiles.Remove(dataWarehouse.Id);
					// Elimina el archivo
					for (int index = ReportingSolution.Files.Count - 1; index >= 0; index--)
						if (ReportingSolution.Files[index].Equals(file, StringComparison.CurrentCultureIgnoreCase))
							ReportingSolution.Files.RemoveAt(index);
				}
				// Elimina el origen de datos del esquema
				Schema.DataWarehouses.Remove(dataWarehouse);
		}

		/// <summary>
		///		Graba una solución
		/// </summary>
		public void SaveSolution()
		{
			new Repositories.ReportingRepository(this).Save();
		}

		/// <summary>
		///		Carga los <see cref="Models.DataWarehouses.DataWarehouseModel"/> de un archivo
		/// </summary>
		public void LoadDataWarehouse(string fileName)
		{
			Models.DataWarehouses.DataWarehouseModel dataWarehouse = new Repositories.DataWarehouseRepository().Load(Schema, fileName);

				// Añade el dashboard al esquema
				if (dataWarehouse != null)
					Schema.DataWarehouses.Add(dataWarehouse);
		}

		/// <summary>
		///		Carga un <see cref="Models.DataWarehouses.DataWarehouseModel"/> a partir de un archivo de esquema de base de datos
		/// </summary>
		public Models.DataWarehouses.DataWarehouseModel ConvertSchemaDbToDataWarehouse(string name, string fileName)
		{
			return new Converters.SchemaConverter().Convert(Schema, name, fileName);
		}

		/// <summary>
		///		Graba los datos de un <see cref="Models.DataWarehouses.DataWarehouseModel"/> en un archivo
		/// </summary>
		public void SaveDataWarehouse(Models.DataWarehouses.DataWarehouseModel dataWarehouse)
		{
			string fileName = ReportingSolution.GetFileName(dataWarehouse);

				// Graba el archivo
				if (string.IsNullOrWhiteSpace(fileName))
					throw new NotImplementedException($"Cant find file name for '{dataWarehouse.Name}'");
				else
					new Repositories.DataWarehouseRepository().Save(dataWarehouse, fileName);
		}

		/// <summary>
		///		Graba los datos de un <see cref="Models.DataWarehouses.DataWarehouseModel"/> en un archivo
		/// </summary>
		public void SaveDataWarehouse(Models.DataWarehouses.DataWarehouseModel dataWarehouse, string fileName)
		{
			new Repositories.DataWarehouseRepository().Save(dataWarehouse, fileName);
		}

		/// <summary>
		///		Obtiene la SQL resultante de procesar una solicitud de informe
		/// </summary>
		public string GetSqlResponse(Requests.Models.ReportRequestModel request)
		{
			return new Controllers.ReportController(this).GetResponse(request);
		}

		/// <summary>
		///		<see cref="Models.ReportingSchemaModel"/> con el que trabaja la aplicación
		/// </summary>
		public Models.ReportingSchemaModel Schema { get; private set; }

		/// <summary>
		///		Solución para informes
		/// </summary>
		public Models.ReportingSolutionModel ReportingSolution { get; } = new Models.ReportingSolutionModel();
	}
}
