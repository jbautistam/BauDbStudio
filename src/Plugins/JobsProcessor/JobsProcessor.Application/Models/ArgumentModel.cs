namespace Bau.Libraries.JobsProcessor.Application.Models;

/// <summary>
///		Clase con los datos de un argumento
/// </summary>
public class ArgumentModel
{
	/// <summary>
	///		Tipo de argumento
	/// </summary>
	public enum ArgumentType
	{
		/// <summary>Cadena</summary>
		String,
		/// <summary>Cadena Json</summary>
		Json,
		/// <summary>Entero</summary>
		Int,
		/// <summary>Decimal</summary>
		Decimal,
		/// <summary>Fecha</summary>
		Date,
		/// <summary>Valor lógico</summary>
		Bool
	}

	/// <summary>
	///		Posición del argumento
	/// </summary>
	public enum ArgumentPosition
	{
		/// <summary>Línea de comandos</summary>
		CommandLine,
		/// <summary>Variable de entorno</summary>
		Environment
	}

	/// <summary>
	///		Devuelve los datos de un argumento
	/// </summary>
	public string GetArgumentValue(ContextModel context)
	{
		string value = Parameter.Value;

			// Reemplaza los valores del argumento con el contexto
			foreach (ParameterModel parameter in context.Parameters)
				if (!string.IsNullOrWhiteSpace(value))
					value = value.Replace("{{" + parameter.Name + "}}", parameter.Value, StringComparison.CurrentCultureIgnoreCase);
			// Reemplaza las cadenas de escape
			if (!string.IsNullOrWhiteSpace(value))
			{
				value = value.Replace("\\{\\{", "{{");
				value = value.Replace("\\}\\}", "}}");
			}
			// Devuelve el valor convertido
			return value;
	}

	/// <summary>
	///		Obtiene el valor convertido del argumento con los datos del contexto
	/// </summary>
	internal ArgumentModel GetConverted(ContextModel context)
	{
		ArgumentModel argument = new();

			// Asigna los datos
			argument.Position = Position;
			argument.Parameter.Name = Parameter.Name;
			argument.Parameter.Value = GetArgumentValue(context);
			// Devuelve los datos del argumento
			return argument;
	}

	/// <summary>
	///		Posición del argumento
	/// </summary>
	public ArgumentPosition Position { get; set; }

	/// <summary>
	///		Datos del parámetro
	/// </summary>
	public ParameterModel Parameter { get; } = new();
}
