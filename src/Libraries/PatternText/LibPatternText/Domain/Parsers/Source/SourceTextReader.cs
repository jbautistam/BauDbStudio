namespace Bau.Libraries.LibPatternText.Domain.Parsers.Source;

/// <summary>
///		Lector de texto separado por caracters
/// </summary>
internal class SourceTextReader : IDisposable
{
	// Variables privadas
	private StreamReader _fileReader = default!;

	internal SourceTextReader(Models.PatternModel pattern)
	{
		Pattern = pattern;
		_fileReader = new StreamReader(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Pattern.Source)));
	}

	/// <summary>
	///		Interpreta la cabecera del archivo
	/// </summary>
	internal void ReadHeader()
	{
		// Lee la cabecera si realmente hay algo que leer
		if (!_fileReader.EndOfStream && Pattern.WithHeader)
		{
			string? line = _fileReader.ReadLine();

				if (!string.IsNullOrWhiteSpace(line))
				{
					// Quita los espacios
					line = line.Trim();
					// Interpreta las columnas (si no se han definido)
					foreach (string field in ParseLine(line))
						Columns.Add(field);
				}
		}
		// Si no se han leido columnas, rellena con valores fijos
		if (Columns.Count == 0)
			for (int index = 0; index < 1_000; index++)
				Columns.Add($"Column {index + 1.ToString()}");
	}

	/// <summary>
	///		Lee un registro
	/// </summary>
	internal bool Read()
	{
		bool readed = false;
		string line = ReadLine();

			// Limpia los valores
			RecordValues.Clear();
			// Interpreta los datos
			if (!string.IsNullOrWhiteSpace(line))
			{
				// Interpreta la línea
				RecordValues.AddRange(ParseLine(line));
				// Indica que se han leido datos
				readed = true;
			}
			// Devuelve el valor que indica si se han leído datos
			return readed;
	}

	/// <summary>
	///		Lee la siguiente línea no vacía del archivo
	/// </summary>
	private string ReadLine()
	{
		string line = string.Empty;
		bool mustReadNextLine = false;

			// Lee la siguiente línea no vacía y se salta las líneas de cabecera
			while (!_fileReader.EndOfStream && (string.IsNullOrWhiteSpace(line) || mustReadNextLine))
			{
				// Resetea el valor que indica que la siguiente vez se debe leer la siguiente línea
				mustReadNextLine = false;
				// Lee la línea
				line += _fileReader.ReadLine();
				// Se debe leer la siguiente línea si el número de caracteres de comillas no es par, eso quiere decir que ha habido un salto
				// de línea en un campo
				if (CountQuotes(line) % 2 != 0)
				{
					// Añade a la línea el salto de línea que ha borrado FileReader.ReadLine()
					line += Environment.NewLine;
					// Indica que se debe leer una línea más
					mustReadNextLine = true;
				}
			}
			// Quita los espacios de la línea
			if (!string.IsNullOrWhiteSpace(line))
				line = line.Trim();
			// Devuelve la línea leida
			return line;
	}

	/// <summary>
	///		Cuenta el número de comillas que hay en una línea
	/// </summary>
	private int CountQuotes(string line)
	{
		int number = 0;

			// Si hay algo que contar
			if (!string.IsNullOrWhiteSpace(line))
			{
				// Quita los caracteres de escape (\")
				line = line.Replace("\\\"", "");
				// Cuenta las comillas de la cadena
				foreach (char chr in line)
					if (chr == '"')
						number++;
			}
			// Devuelve el número contado
			return number;
	}

	/// <summary>
	///		Interpreta la línea separando los campos teniendo en cuenta las comillas
	/// </summary>
	private List<string> ParseLine(string line)
	{
		List<string> fields = new();
		string field = string.Empty;
		bool isInQuotes = false, scapeChar = false;

			// Interpreta las partes de la línea
			foreach (char actual in line)
			{
				// Trata el carácter
				if (isInQuotes)
				{
					if (actual == '\\')
						scapeChar = true;
					else if (actual == '"')
					{
						if (scapeChar)
						{
							field += actual;
							scapeChar = false;
						}
						else
						{
							isInQuotes = false;
							scapeChar = false;
						}
					}
					else
						field += actual;
				}
				else if (!isInQuotes)
				{
					if (actual == '"')
						isInQuotes = true;
					else if (actual == Pattern.Separator[0])
					{
						// Convierte la cadena
						fields.Add(Normalize(field));
						// Vacía la cadena intermedia e incrementa el índice del campo
						field = string.Empty;
					}
					else
						field += actual;
				}
				else
					field += actual;
			}
			// Añade el último campo
			fields.Add(Normalize(field));
			// Devuelve la lista de campos
			return fields;
	}

	/// <summary>
	///		Normaliza una cadena
	/// </summary>
	private string Normalize(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return string.Empty;
		else
			return value.Trim();
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			// Libera los datos
			if (disposing)
				_fileReader.Close();
			// Indica que se ha liberado
			IsDisposed = true;
		}
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
	}

	/// <summary>
	///		Origen de los datos
	/// </summary>
	internal Models.PatternModel Pattern { get; }

	/// <summary>
	///		Columnas del texto
	/// </summary>
	internal List<string> Columns { get; } = new();

	/// <summary>
	///		Valores del registro
	/// </summary>
	internal List<string> RecordValues { get; } = new();

	/// <summary>
	///		Indica si se ha liberado el recurso
	/// </summary>
	internal bool IsDisposed { get; private set; }
}
