namespace Bau.Libraries.LibReporting.Models.Base;

/// <summary>
///		Diccionario normalizado
/// </summary>
public class BaseReportingDictionaryModel<TypeData> where TypeData : BaseReportingModel
{
	public BaseReportingDictionaryModel(bool replaceDuplicates = true)
	{
		ReplaceDuplicates = replaceDuplicates;
	}

	/// <summary>
	///		Añade un elemmento
	/// </summary>
	public void Add(TypeData value)
	{
		string key = Normalize(value.Id);

			// Añade / modifica la clave
			if (!InternalDictionary.ContainsKey(key))
				InternalDictionary.Add(key, value);
			else if (ReplaceDuplicates)
				this[key] = value;
			else
				throw new KeyNotFoundException();
	}

	/// <summary>
	///		Añade un diccionario a este diccionario
	/// </summary>
	public void AddRange(BaseReportingDictionaryModel<TypeData> data)
	{
		foreach ((string _, TypeData value) in data.Enumerate())
			Add(value);
	}

	/// <summary>
	///		Clona el diccionario
	/// </summary>
	public BaseReportingDictionaryModel<TypeData> Clone()
	{
		BaseReportingDictionaryModel<TypeData> target = new BaseReportingDictionaryModel<TypeData>();

			// Clona los registros
			foreach ((string _, TypeData value) in Enumerate())
				target.Add(value);
			// Devuelve el diccionario clonado
			return target;
	}

	/// <summary>
	///		Comprueba si contiene un elemento
	/// </summary>
	public bool ContainsKey(string key) => InternalDictionary.ContainsKey(Normalize(key));

	/// <summary>
	///		Intenta obtener un valor si existe la clave
	/// </summary>
	public bool TryGetValue(string key, out TypeData? value) => InternalDictionary.TryGetValue(key, out value);

	/// <summary>
	///		Elimina un elemento
	/// </summary>
	public void Remove(string key)
	{
		// Normaliza la clave
		key = Normalize(key);
		// Elimina el elemento
		if (InternalDictionary.ContainsKey(key))
			InternalDictionary.Remove(key);
	}

	/// <summary>
	///		Limpia los elementos
	/// </summary>
	public void Clear()
	{
		InternalDictionary.Clear();
	}

	/// <summary>
	///		Normaliza una clave
	/// </summary>
	private string Normalize(string key) => key.ToUpperInvariant();

	/// <summary>
	///		Elimina un elemento
	/// </summary>
	public void Remove(TypeData item)
	{
		InternalDictionary.Remove(Normalize(item.Id));
	}

	/// <summary>
	///		Obtiene los valores del diccionario
	/// </summary>
	public IEnumerable<(string Key, TypeData Value)> Enumerate()
	{
		foreach (KeyValuePair<string, TypeData> keyValue in InternalDictionary)
			yield return (keyValue.Key, keyValue.Value);
	}

	/// <summary>
	///		Obtiene los valores del diccionario
	/// </summary>
	public IEnumerable<TypeData> EnumerateValues()
	{
		foreach (KeyValuePair<string, TypeData> keyValue in InternalDictionary)
			yield return keyValue.Value;
	}

	/// <summary>
	///		Obtiene los valores del diccionario ordenados
	/// </summary>
	public IEnumerable<TypeData> EnumerateValuesSorted()
	{
		List<TypeData> items = new();

			// Añade los elementos
			foreach (TypeData item in EnumerateValues())
				items.Add(item);
			// Ordena los elementos
			items.Sort((first, second) => first.CompareTo(second));
			// Devuelve los elementos ordenados
			foreach (TypeData item in items)
				yield return item;
	}

	/// <summary>
	///		Obtiene un valor
	/// </summary>
	public TypeData? this[string key]
	{
		get
		{
			// Normaliza la clave
			key = Normalize(key);
			// Devuelve el valor
			return InternalDictionary.ContainsKey(key) ? InternalDictionary[key] : null;
		}
		set
		{
			// Normaliza la clave
			key = Normalize(key);
			// Elimina el valor antiguo
			if (InternalDictionary.ContainsKey(key))
				InternalDictionary.Remove(key);
			// Añade el valor modificado
			if (value is not null)
				InternalDictionary.Add(key, value);
		}
	}

	/// <summary>
	///		Obtiene el diccionario interno
	/// </summary>
	public Dictionary<string, TypeData> ToDictionary()
	{
		Dictionary<string, TypeData> converted = new();

			// Convierte los valores
			foreach ((string key, TypeData value) in Enumerate())
				converted.Add(key, value);
			// Devuelve el diccionario
			return converted;
	}

	/// <summary>
	///		Número de elementos
	/// </summary>
	public int Count => InternalDictionary.Count;

	/// <summary>
	///		Diccionario interno
	/// </summary>
	private Dictionary<string, TypeData> InternalDictionary { get; } = new();

	/// <summary>
	///		Reemplaza los duplicados
	/// </summary>
	public bool ReplaceDuplicates { get; set; }
}
