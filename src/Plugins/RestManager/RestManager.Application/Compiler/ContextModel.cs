namespace Bau.Libraries.RestManager.Application.Compiler;

/// <summary>
///		Clase con los datos de contexto
/// </summary>
internal class ContextModel
{
	/// <summary>
	///		Añade un valor al contexto
	/// </summary>
	internal void Add(string name, string? value)
	{
		Variables.Add(name, value);
	}

	/// <summary>
	///		Clona un contexto
	/// </summary>
	internal ContextModel Clone()
	{
		ContextModel cloned = new();

			// Añade las variables
			foreach (KeyValuePair<string, string?> keyvalue in Variables)
				cloned.Add(keyvalue.Key, keyvalue.Value);
			// Devuelve el contexto clonado
			return cloned;
	}

	/// <summary>
	///		Interpreta un valor
	/// </summary>
	internal string? Parse(string? value)
	{
		string? result = value;

			// Interpreta el valor
			if (!string.IsNullOrWhiteSpace(result))
				foreach (KeyValuePair<string, string?> keyValue in Variables)
					result = result.Replace("{{" + keyValue.Key + "}}", keyValue.Value, StringComparison.CurrentCultureIgnoreCase);
			// Devuelve el resultado
			return result;
	}

	/// <summary>
	///		Variables
	/// </summary>
	private Dictionary<string, string?> Variables { get; } = new(StringComparer.InvariantCultureIgnoreCase);
}
