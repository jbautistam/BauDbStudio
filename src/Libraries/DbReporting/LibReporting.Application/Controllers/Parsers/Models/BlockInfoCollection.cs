namespace Bau.Libraries.LibReporting.Application.Controllers.Parsers.Models;

/// <summary>
///		Colección de <see cref="BlockInfo"/>
/// </summary>
internal class BlockInfoCollection
{
	/// <summary>
	///		Interpreta una cadena con información de bloques
	/// </summary>
	public void Parse(string content)
	{ 
		// Limpia los bloques
		Blocks.Clear();
		// Añade los bloques convertidos
		Blocks.AddRange(CreateBlocks(ConvertLines(content)));
		// Separa la información de los bloques
		foreach (BlockInfo block in Blocks)
			block.Split();
	}

	/// <summary>
	///		Crea los bloques
	/// </summary>
	private List<BlockInfo> CreateBlocks(List<BlockInfo> readedBlocks)
	{ 
		List<BlockInfo> blocks = new();
		BlockInfo? actual = null;

			// Crea los bloques con las líneas a nivel del tabulador inicial
			foreach (BlockInfo block in readedBlocks)
			{
				if (actual is null)
				{
					blocks.Add(new(null, block.Tabs, block.Line));
					actual = blocks[blocks.Count - 1];
				}
				else if (block.Tabs > actual.Tabs)
				{ 
					actual.Blocks.Add(new(actual, block.Tabs, block.Line));
					actual = actual.Blocks[actual.Blocks.Count - 1];
				}
				else // ... tiene que añadírselo al padre
				{
					// Localiza al padre
					do
					{
						actual = actual.Parent;
					}
					while (actual is not null && actual?.Tabs >= block.Tabs);
					// Si es nulo, crea un bloque
					if (actual is null)
					{
						blocks.Add(new(null, block.Tabs, block.Line));
						actual = blocks[blocks.Count - 1];
					}
					else
					{
						actual.Blocks.Add(new(actual, block.Tabs, block.Line));
						actual = actual.Blocks[actual.Blocks.Count - 1];
					}
				}
			}
			// Devuelve la colección de bloques
			return blocks;
	}

	/// <summary>
	///		Convierte el contenido en líneas
	/// </summary>
	private List<BlockInfo> ConvertLines(string content)
	{ 
		string[] lines = content.Split('\r', '\n');
		List<BlockInfo> linesInfo = new();

			// Añade las líneas
			if (lines is not null)
				foreach (string line in lines)
					if (!string.IsNullOrWhiteSpace(line))
						linesInfo.Add(new BlockInfo(null, CountTabs(line), line.Trim()));
			// Devuelve las líneas
			return linesInfo;

		// Cuenta las tabulaciones o espacios que encabezan una línea
		int CountTabs(string line)
		{
			int index = 0, tabs = 0, spaces = 0;

				// Cuenta los tabuladores / espacios iniciales
				while (index < line.Length && (line[index] == '\t' || line[index] == ' '))
				{
					// Incrementa el número de espacios 
					if (line[index] == '\t')
						tabs ++;
					else
						spaces ++;
					// Incrementa el índice
					index++;
				}
				// Devuelve el número de espacios
				return tabs + spaces / 2;
		}
	}

	/// <summary>
	///		Obtiene la cadena de depuración
	/// </summary>
	internal string GetDebugString()
	{ 
		string debug = string.Empty;

			// Obtiene la información de depuración
			if (Blocks.Count == 0)
				debug = "No blocks info";
			else
				foreach (BlockInfo block in Blocks)
					debug += block.GetDebugString(0) + Environment.NewLine;
			// Devuelve la cadena de depuración
			return debug;
	}

	/// <summary>
	///		Bloques de la colección
	/// </summary>
	internal List<BlockInfo> Blocks { get; } = new();
}
