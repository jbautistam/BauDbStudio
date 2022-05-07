using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bau.Libraries.LibChessGame.Mvvm.Controllers
{
	/// <summary>
	///		Interface para el controlador de la ventana principal
	/// </summary>
	public interface IHostController
	{
		/// <summary>
		///		Abre la ventana de selección de un archivo
		/// </summary>
		Task<string> OpenFileAsync(string pathBase, string filter, string defaultExt);

		/// <summary>
		///		Obtiene un streamReader sobre un archivo
		/// </summary>
		Task<StreamReader> GetStreamReaderAsync(string fileName);

		/// <summary>
		///		Muestra un mensaje
		/// </summary>
		Task ShowMessageAsync(string message);

		/// <summary>
		///		Obtiene los directorios donde se encuentran las imágenes del tablero
		/// </summary>
		Task<List<string>> GetPathsBoardAsync();

		/// <summary>
		///		Obtiene los directorios donde se encuentran las imágenes de las piezas
		/// </summary>
		Task<List<string>> GetPathsPiecesAsync();
	}
}
