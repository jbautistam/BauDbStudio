namespace Bau.Libraries.LibReporting.Requests.Models;

/// <summary>
///		Clase con los datos de solicitud de informe
/// </summary>
public class ReportRequestModel
{
	/// <summary>
	///		Clona los datos del objeto
	/// </summary>
	public ReportRequestModel Clone()
	{
		ReportRequestModel cloned = new()
										{
											ReportId = ReportId
										};

			// Clona las solicitudes de dimensions
			foreach (DimensionRequestModel dimensionRequest in Dimensions)
				cloned.Dimensions.Add(dimensionRequest.Clone());
			// Clona las solicitudes de expresiones
			foreach (ExpressionRequestModel expressionRequest in Expressions)
				cloned.Expressions.Add(expressionRequest.Clone());
			// Clona la paginación
			cloned.Pagination.Clone(Pagination);
			// Devuelve los datos clonados
			return cloned;
	}

	/// <summary>
	///		Obtiene los datos de solicitud de una dimensión
	/// </summary>
	public DimensionRequestModel? GetDimensionRequest(string dimensionKey)
	{
		// Busca la dimensión entre las solicitudes
		foreach (DimensionRequestModel request in Dimensions)
			if (request.DimensionId.Equals(dimensionKey, StringComparison.CurrentCultureIgnoreCase))
				return request;
		// Si ha llegado hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Comprueba si se ha solicitado alguna dimensión
	/// </summary>
	public bool IsRequestedDimension(List<string>? dimensionsKey)
	{
		// Comprueba si se ha solicitado alguna de las dimensiones
		if (dimensionsKey is not null)
			foreach (string key in dimensionsKey)
				foreach (DimensionRequestModel request in Dimensions)
					if (request.DimensionId.Equals(key, StringComparison.CurrentCultureIgnoreCase))
						return true;
		// Si ha llegado hasta aquí es porque no se ha encontrado ninguna de las dimensiones
		return false;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una dimensión
	/// </summary>
	public bool IsRequestedDimension(string dimensionKey) => IsRequestedDimension(new List<string> { dimensionKey });

	/// <summary>
	///		Comprueba si se ha solicitado alguna expresión
	/// </summary>
	public bool IsRequestedExpression(List<string> expressionKeys)
	{
		// Comprueba si se ha solicitado alguna de las expresiones
		if (expressionKeys is not null)
			foreach (string key in expressionKeys)
				foreach (ExpressionRequestModel request in Expressions)
					foreach (ExpressionColumnRequestModel column in request.Columns)
						if (column.ColumnId.Equals(key, StringComparison.CurrentCultureIgnoreCase))
							return true;
		// Si se ha llegado hasta aquí es porque no se ha encontrado ninguna de las dimensiones
		return false;
	}

	/// <summary>
	///		Comprueba si se ha solicitado una expresión
	/// </summary>
	public bool IsRequestedExpression(string expressionKey) => IsRequestedExpression(new List<string> { expressionKey });

	/// <summary>
	///		Añade una dimensión y un campo a una solicitud
	/// </summary>
	public void Add(string dimensionId, string columnId)
	{
		DimensionRequestModel? dimension = GetDimensionRequest(dimensionId);

			// Añade la dimensión si no se había solicitado
			if (dimension is null)
			{
				// Crea la dimensión
				dimension = new()
								{
									DimensionId = dimensionId
								};
				// Añade la dimensión a la solicitud
				Dimensions.Add(dimension);
			}
			// Añade la columna
			if (dimension.GetRequestColumn(columnId) is null)
				dimension.Columns.Add(new()
										{
											//DimensionId = dimensionId,
											ColumnId = columnId
										}
									 );
	}

	/// <summary>
	///		Código de informe solicitado
	/// </summary>
	public string ReportId { get; set; } =  string.Empty;

	/// <summary>
	///		Parámetros
	/// </summary>
	public Dictionary<string, object?> Parameters { get; } = new();

	/// <summary>
	///		Dimensiones solicitadas
	/// </summary>
	public List<DimensionRequestModel> Dimensions { get; } = new();

	/// <summary>
	///		Expresiones solicitadas
	/// </summary>
	public List<ExpressionRequestModel> Expressions { get; } = new();

	/// <summary>
	///		Paginación
	/// </summary>
	public PaginationRequestModel Pagination { get; } = new();
}