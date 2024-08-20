namespace Bau.Libraries.DbStudio.Models.Connections;

/// <summary>
///		Datos de una tabla
/// </summary>
public class ConnectionTableModel : LibDataStructures.Base.BaseExtendedModel
{
	public ConnectionTableModel(ConnectionModel connection)
	{
		Connection = connection;
	}

	/// <summary>
	///		Conexión a la que pertenece la tabla
	/// </summary>
	public ConnectionModel Connection { get; }

	/// <summary>
	///		Nombre del esquema
	/// </summary>
	public string Schema { get; set; } = string.Empty;

	/// <summary>
	///		Nombre completo de la tabla
	/// </summary>
	public string FullName
	{
		get 
		{
			if (!string.IsNullOrWhiteSpace(Schema))
				return $"{Schema}.{Name}";
			else
				return Name;
		}
	}

	/// <summary>
	///		Indica si es una tabla de sistema
	/// </summary>
	public bool IsSystem { get; set; }

	/// <summary>
	///		Campos
	/// </summary>
	public List<ConnectionTableFieldModel> Fields { get; } = new();
}
