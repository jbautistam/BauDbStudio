using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibPgnReader.Parsers
{
	/// <summary>
	///		Lector de archivos, lee el archivo carácter por carácter
	/// </summary>
	internal class CharFileReader
	{
		/// <summary>
		///		Tipo de carácter leído
		/// </summary>
		internal enum CharType
		{
			/// <summary>Carácter</summary>
			Character,
			/// <summary>Final de línea</summary>
			EoL,
			/// <summary>Final de archivo</summary>
			EoF,
			/// <summary>Línea vacía</summary>
			EmptyLine
		}

		internal CharFileReader(System.IO.StreamReader streamReader)
		{
			StreamReader = streamReader;
		}

		/// <summary>
		///		Enumera los caracteres
		/// </summary>
		internal IEnumerable<(CharType type, char character)> Read()
		{
			string line;

				// Lee las líneas
				while((line = StreamReader.ReadLine()) != null)  
				{ 
					// Lee los caracteres de las líneas
					if (string.IsNullOrWhiteSpace(line))
						yield return (CharType.EmptyLine, ' ');
					else
					{
						// Lee los caracteres
						foreach (char character in line)
							yield return (CharType.Character, character);
						// Lanza un salto de línea
						yield return (CharType.EoL, ' ');
					}
				}
				// Devuelve el fin de archivo
				yield return (CharType.EoF, ' ');
		}

		/// <summary>
		///		Stream del archivo
		/// </summary>
		private System.IO.StreamReader StreamReader { get; set; }
	}
}