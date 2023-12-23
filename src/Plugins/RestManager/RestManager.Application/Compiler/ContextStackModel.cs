namespace Bau.Libraries.RestManager.Application.Compiler;

/// <summary>
///		Pila de contextos
/// </summary>
internal class ContextStackModel
{
	internal ContextStackModel()
	{
		Contexts.Add(new ContextModel());
	}

	/// <summary>
	///		Limpia la pila de contextos (y añade el contexto inicial)
	/// </summary>
	internal void Clear()
	{
		Contexts.Clear();
		Contexts.Add(new ContextModel());
	}

	/// <summary>
	///		Añade un valor al contexto
	/// </summary>
	internal void Add(string key, string? value)
	{
		Actual.Add(key, value);
	}

	/// <summary>
	///		Interpreta un valor
	/// </summary>
	internal string? Parse(string? value) => Actual.Parse(value);

	/// <summary>
	///		Añade un contexto
	/// </summary>
	internal void Push()
	{
		Contexts.Add(Actual.Clone());
	}

	/// <summary>
	///		Elimina el contexto actual
	/// </summary>
	internal void Remove()
	{
		Contexts.RemoveAt(Contexts.Count - 1);
	}

	/// <summary>
	///		Contexto actual
	/// </summary>
	private ContextModel Actual
	{
		get
		{
			if (Contexts.Count > 0)
				return Contexts[Contexts.Count - 1];
			else
				throw new ArgumentException("Can't find any context");
		}
	}

	/// <summary>
	///		Contextos
	/// </summary>
	internal List<ContextModel> Contexts { get; } = new();
}
