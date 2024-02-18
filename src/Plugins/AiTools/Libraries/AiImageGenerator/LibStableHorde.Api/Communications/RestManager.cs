using System.Net.Http.Headers;
using System.Text.Json;

namespace Bau.Libraries.LibStableHorde.Api.Communications;

/// <summary>
///		Manager de conexión con la API
/// </summary>
internal class RestManager
{
	// Variables privadas
	private HttpClient? _client = null;

	internal RestManager(Uri url, string apiKey, TimeSpan timeout)
	{
		Url = url;
		ApiKey = apiKey;
		Timeout = timeout;
	}

	/// <summary>
	///		Ejecuta un post sobre una URL
	/// </summary>
	internal async Task<HttpResponseMessage> PostAsync(string url, object? data, CancellationToken cancellationToken)
	{
		using (StringContent content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json"))
		{
			return await Client.PostAsync(url, content, cancellationToken);
		}
	}

	/// <summary>
	///		Ejecuta un post sobre una URL y obtiene el resultado
	/// </summary>
	internal async Task<TypeData?> PostAndParseAsync<TypeData>(string url, object? data, CancellationToken cancellationToken)
	{
		using (HttpResponseMessage response = await PostAsync(url, data, cancellationToken))
		{
			// Se asegura que el resultado sea correcto
			response.EnsureSuccessStatusCode();
			// Devuelve el resultado
			return await response.GetToAsync<TypeData>(cancellationToken);
		}
	}

	/// <summary>
	///		Ejecuta un post sobre una URL y obtiene el resultado (sin comprobar la respuesta)
	/// </summary>
	internal async Task<TypeData?> PostAndParseNoCheckStatusAsync<TypeData>(string url, object? data, CancellationToken cancellationToken)
	{
		using (HttpResponseMessage response = await PostAsync(url, data, cancellationToken))
		{
			return await response.GetToAsync<TypeData>(cancellationToken);
		}
	}

	/// <summary>
	///		Ejecuta un post sobre una URL enviando un archivo
	/// </summary>
	internal async Task<HttpResponseMessage> PostWithFileAsync(string url, object data, string fileName, MemoryStream stream, CancellationToken cancellationToken)
	{
		using (MultipartFormDataContent content = new())
		{
			// Añade las cabeceras
			content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
			// Añade el cuerpo Json del contenido
			if (data != null)
				content.Add(new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json"));
			// Añade el archivo
			if (stream != null)
			{
				StreamContent streamContent = new StreamContent(stream);

					// Añade las cabeceras
					streamContent.Headers.Add("Content-Type", "application/octet-stream");
					streamContent.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + fileName + "\"");
					// Añade el stream al contenido
					content.Add(streamContent, "file", fileName);
			}
			// Envía los datos
			return await Client.PostAsync(url, content, cancellationToken);
		}
	}

	/// <summary>
	///		Ejecuta un post sobre una URL
	/// </summary>
	internal async Task PostAndCheckAsync(string url, object? data, CancellationToken cancellationToken)
	{
		await CheckResponseAsync(await PostAsync(url, data, cancellationToken), cancellationToken);
	}

	/// <summary>
	///		Ejecuta un delete sobre una URL
	/// </summary>
	private async Task<HttpResponseMessage> DeleteAsync(string url, int id, CancellationToken cancellationToken) => await Client.DeleteAsync($"{url}/{id}", cancellationToken);

	/// <summary>
	///		Ejecuta un delete sobre una URL y comprueba el resultado
	/// </summary>
	internal async Task DeleteAndCheckAsync(string url, object data, CancellationToken cancellationToken)
	{
		HttpRequestMessage request = new(HttpMethod.Delete, url)
												{
													Content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json")
												};

			// Envía el mensaje y comprueba el resultado
			await CheckResponseAsync(await Client.SendAsync(request, cancellationToken), cancellationToken);
	}

	/// <summary>
	///		Ejecuta un DELETE y comprueba el resultado
	/// </summary>
	internal async Task DeleteAndCheckAsync(string url, int id, CancellationToken cancellationToken)
	{
		await CheckResponseAsync(await DeleteAsync(url, id, cancellationToken), cancellationToken);
	}

	/// <summary>
	///		Obtiene el resultado de una consulta sobre una URL
	/// </summary>
	internal async Task<TypeResult?> GetResponseDataAsync<TypeResult>(string url, CancellationToken cancellationToken)
	{
		return await GetResponseDataAsync<TypeResult>(await Client.GetAsync(url, cancellationToken), cancellationToken);
	}

	/// <summary>
	///		Obtiene el resultado de una consulta sobre una URL
	/// </summary>
	internal async Task<TypeResult?> GetResponseNoCheckStatusDataAsync<TypeResult>(string url, CancellationToken cancellationToken)
	{
		using (HttpResponseMessage response = await Client.GetAsync(url, cancellationToken))
		{
			return await response.GetToAsync<TypeResult>(cancellationToken);
		}
	}

	/// <summary>
	///		Obtiene el resultado de una consulta
	/// </summary>
	private async Task<TypeResult?> GetResponseDataAsync<TypeResult>(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		await CheckResponseAsync(response, cancellationToken);
		return await response.GetToAsync<TypeResult>(cancellationToken);
	}

	/// <summary>
	///		Comprueba el resultado de una llamada
	/// </summary>
	internal async Task CheckResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
	{
		if (!response.IsSuccessStatusCode)
			throw new RestManagerException(await response.GetContentAsync(cancellationToken));
	}

	/// <summary>
	///		Url de la API
	/// </summary>
	internal Uri Url { get; }

	/// <summary>
	///		Clave de la API
	/// </summary>
	internal string ApiKey { get; }

	/// <summary>
	///		Timeout
	/// </summary>
	internal TimeSpan Timeout { get; }

	/// <summary>
	///		Cliente Http
	/// </summary>
	internal HttpClient Client 
	{ 
		get
		{
			// Obtiene el cliente
			if (_client is null)
			{
				// Crea el objeto
				_client = new HttpClient();
				// Asigna las propiedades
				_client.BaseAddress = Url;
				_client.DefaultRequestHeaders.Accept.Clear();
				_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				_client.DefaultRequestHeaders.Add("apikey", ApiKey);
				_client.Timeout = Timeout;
			}
			// Devuelve el cliente
			return _client;
		}
	}
}