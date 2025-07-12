namespace Bau.Libraries.JobsProcessor.Application.Models;

/// <summary>
///		Clase con los datos de contexto
/// </summary>
public class ContextModel
{
	/// <summary>
	///		Añade los datos de otro contexto
	/// </summary>
	internal void Add(ContextModel context)
	{
		foreach (ParameterModel parameter in context.Parameters)
			Add(parameter);
	}

	/// <summary>
	///		Añade / modifica un parámetro
	/// </summary>
	internal void Add(ParameterModel parameter)
	{
		ParameterModel? existing = Parameters.FirstOrDefault(item => item.Name.Equals(parameter.Name, StringComparison.CurrentCultureIgnoreCase));

			// Si no existe el parámetro se añade, si existe se modifica
			if (existing is null)
				Parameters.Add(parameter);
			else
				existing.Value = parameter.Value;
	}

	/// <summary>
	///		Id del contexto
	/// </summary>
	public string Id { get; } = Guid.NewGuid().ToString();

	/// <summary>
	///		Parámetros del contexto
	/// </summary>
	public List<ParameterModel> Parameters { get; } = [];
}
