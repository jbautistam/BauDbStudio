namespace Bau.Libraries.LibStableHorde.Api.Communications;

/// <summary>
///		Extensiones de <see cref="HttpResponseMessage"/>
/// </summary>
internal static class HttpResponseMessageExtensions
{
	/// <summary>
	///		Obtiene un tipo del cuerpo del mensaje
	/// </summary>
    internal static async Task<T?> GetToAsync<T>(this HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {
		return JsonManager.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(cancellationToken));
	}

	/// <summary>
	///		Obtiene el cuerpo de la respuesta
	/// </summary>
	internal static async Task<string> GetContentAsync(this HttpResponseMessage responseMessage, CancellationToken cancellationToken) => await responseMessage.Content.ReadAsStringAsync(cancellationToken);
}