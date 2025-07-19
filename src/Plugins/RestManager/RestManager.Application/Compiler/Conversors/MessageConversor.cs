using Bau.Libraries.LibRestClient.Messages;
using Bau.Libraries.RestManager.Application.Models;

namespace Bau.Libraries.RestManager.Application.Compiler.Conversors;

/// <summary>
///		Conversor de mensajes
/// </summary>
internal class MessageConversor
{
	/// <summary>
	///		Crea una solicitud
	/// </summary>
	internal RequestMessage CreateRequest(RestStepModel step, ContextStackModel context)
	{
		RequestMessage request = new(ConvertMethod(step.Method), Parse(step.EndPoint, context), step.Timeout);

			// Asigna las propiedades
			AddHeaders(request, step.Headers, context);
			AddQueryStrings(request, step.QueryStrings, context);
			request.Content = Parse(step.Content, context);
			// Devuelve la solicitud
			return request;
	}

	/// <summary>
	///		Convierte el método
	/// </summary>
	private RequestMessage.MethodType ConvertMethod(RestStepModel.RestMethod method)
	{
		return method switch
				{
					RestStepModel.RestMethod.Post => RequestMessage.MethodType.Post,
					RestStepModel.RestMethod.Put => RequestMessage.MethodType.Put,
					RestStepModel.RestMethod.Patch => RequestMessage.MethodType.Patch,
					RestStepModel.RestMethod.Delete => RequestMessage.MethodType.Delete,
					RestStepModel.RestMethod.Options => RequestMessage.MethodType.Options,
					_ => RequestMessage.MethodType.Get
				};
	}

	/// <summary>
	///		Interpreta las cabeceras
	/// </summary>
	private void AddHeaders(RequestMessage request, ParametersCollectionModel headers, ContextStackModel context)
	{
		foreach (ParameterModel parameter in headers)
			if (!request.Headers.ContainsKey(parameter.Key))
				request.Headers.Add(parameter.Key, Parse(parameter.Value, context));
	}

	/// <summary>
	///		Interpreta las cadenas de consulta
	/// </summary>
	private void AddQueryStrings(RequestMessage request, ParametersCollectionModel queryStrings, ContextStackModel context)
	{
		foreach (ParameterModel queryString in queryStrings)
			if (!request.QueryStrings.ContainsKey(queryString.Key))
				request.QueryStrings.Add(queryString.Key, Parse(queryString.Value, context));
	}

	/// <summary>
	///		Interpreta un valor
	/// </summary>
	private string Parse(string? value, ContextStackModel context)
	{
		// Normaliza el valor
		value = context.Parse(value);
		// Devuelve el valor interpretado
		if (string.IsNullOrWhiteSpace(value))
			return string.Empty;
		else
			return value;
	}
}