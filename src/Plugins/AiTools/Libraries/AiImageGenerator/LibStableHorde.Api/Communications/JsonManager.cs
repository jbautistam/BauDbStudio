using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bau.Libraries.LibStableHorde.Api.Communications;

/// <summary>
///		Rutina de ayuda para la conversión de Json
/// </summary>
internal static class JsonManager
{
	/// <summary>
	///		Deserializa un tipo
	/// </summary>
	internal static T? Deserialize<T>(string json)
	{
		JsonSerializerOptions options = new() 
										{
											PropertyNameCaseInsensitive = true
										};

			// Añade el conversor para enumerados
			options.Converters.Add(new JsonStringEnumConverter());
			// Convierte la cadena JSON
			return JsonSerializer.Deserialize<T>(json, options);
	}
}
