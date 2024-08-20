using System;

namespace Bau.Libraries.ChessDataBase.Models.Board
{
	/// <summary>
	///		Clase con los datos de una celda
	/// </summary>
	public class CellModel
	{
		// Constantes privadas
		private const string Letter = "abcdefgh";
		private const string Digits = "87654321";

		public CellModel(CellModel source)
		{
			if (source != null)
			{
				Row = source.Row;
				Column = source.Column;
			}
		}

		public CellModel(int row, int column)
		{
			Row = row;
			Column = column;
		}

		public CellModel(string cell)
		{
			if (!string.IsNullOrEmpty(cell))
			{
				// Pasa la cadena a minúsculas
				cell = cell.ToLower();
				// Obtiene fila y columna
				if (cell.Length == 2)
				{
					Column = Letter.IndexOf(cell[0]);
					Row = Digits.IndexOf(cell[1]);
				}
				else if (cell.Length == 1)
				{
					// Inicializa fila y columna
					Column = -1;
					Row = -1;
					// Asigna la celda
					if (char.IsDigit(cell[0]))
						Row = Digits.IndexOf(cell[0]);
					else
						Column = Letter.IndexOf(cell[0]);
				}
			}
		}

		/// <summary>
		///		Fila
		/// </summary>
		public int Row { get; set; }

		/// <summary>
		///		Columna
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		///		Sobrescribe el método ToString
		/// </summary>
		public override string ToString()
		{
			if (Column == -1 && Row == -1)
				return "-";
			else if (Column != -1 && Row == -1)
				return Letter[Column].ToString();
			else if (Column == -1 && Row != -1)
				return Digits[Row].ToString();
			else
				return Letter[Column].ToString() + Digits[Row].ToString();
		}
	}
}
