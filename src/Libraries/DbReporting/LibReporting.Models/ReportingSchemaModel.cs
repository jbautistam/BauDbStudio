﻿namespace Bau.Libraries.LibReporting.Models;

/// <summary>
///		Clase con los datos del esquema de informes de BI
/// </summary>
public class ReportingSchemaModel
{
	/// <summary>
	///		Limpia el esquema
	/// </summary>
	public void Clear()
	{
		DataWarehouses.Clear();
	}

	/// <summary>
	///		Almacenes de datos
	/// </summary>
	public Base.BaseReportingDictionaryModel<DataWarehouses.DataWarehouseModel> DataWarehouses { get; } = new();
}
