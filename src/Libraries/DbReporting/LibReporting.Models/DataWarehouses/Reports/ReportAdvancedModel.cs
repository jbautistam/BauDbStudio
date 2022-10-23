using System;
using System.Collections.Generic;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports
{
	/// <summary>
	///		Clase con los datos de un informe avanzado
	/// </summary>
	public class ReportAdvancedModel : ReportBaseModel
	{
		public ReportAdvancedModel(DataWarehouseModel dataWarehouse, string fileName) : base(dataWarehouse)
		{
			FileName = fileName;
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName { get; }

		/// <summary>
		///		Clave del DataWarehouse del archivo
		/// </summary>
		public string DataWarehouseKey { get; set; }

		/// <summary>
		///		Parámetros del informe
		/// </summary>
		public List<ReportParameterModel> Parameters { get; } = new();

		/// <summary>
		///		Claves de las dimensiones asociadas al informe
		/// </summary>
		public List<string> DimensionKeys { get; } = new();

		/// <summary>
		///		Bloques del informe
		/// </summary>
		public List<BaseBlockModel> Blocks { get; } = new();
	}
}
