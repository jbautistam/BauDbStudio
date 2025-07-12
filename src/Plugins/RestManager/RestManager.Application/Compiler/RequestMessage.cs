using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.Application.Compiler;

/// <summary>
///		Mensaje de solicitud
/// </summary>
internal class RequestMessage
{
	public RequestMessage(RestStepModel step)
	{
		Step = step;
	}

	/// <summary>
	///		Obtiene la cadena de depuración
	/// </summary>
	public string Debug(int indent)
	{
		System.Text.StringBuilder builder = new();
		string textIndex = new string('\t', indent);

			// Escribe el método y la URL
			builder.AppendLine($"{textIndex}{Method.ToString()} - {Url}");
			// Cabeceras
			builder.AppendLine($"{textIndex}Headers");
			foreach (ParameterModel header in Headers)
				builder.AppendLine($"{textIndex}\t{header.Key}: {header.Value}");
			// Contenido
			if (string.IsNullOrWhiteSpace(Content))
				builder.AppendLine($"{textIndex}No content");
			else
			{
				builder.AppendLine($"{textIndex}Content");
				builder.AppendLine($"{textIndex}\t{Content}");
			}
			// Devuelve la cadena
			return builder.ToString();
	}

	/// <summary>
	///		Paso que se está ejecutando
	/// </summary>
	public RestStepModel Step { get; }

	/// <summary>
	///		Método
	/// </summary>
	public RestStepModel.RestMethod Method { get; set; }

	/// <summary>
	///		Url
	/// </summary>
	public string Url { get; set; } = default!;

	/// <summary>
	///		Cabeceras
	/// </summary>
	public ParametersCollectionModel Headers { get; } = [];

	/// <summary>
	///		Contenido
	/// </summary>
	public string? Content { get; set; }
}
