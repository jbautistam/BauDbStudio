using System;
using System.Collections.Generic;

namespace Bau.Libraries.RestStudio.Models.Rest
{	
	/// <summary>
	///		Modelo de API rest
	/// </summary>
	public class RestModel
	{
		/// <summary>
		///		Nombre de la API
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Descripción de la API
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		///		Contextos
		/// </summary>
		public List<ContextModel> Contexts { get; }= new();

		/// <summary>
		///		Cabeceras predeterminadas
		/// </summary>
		public Dictionary<string, string> DefaultHeaders { get; } = new();

		/// <summary>
		///		Métodos
		/// </summary>
		public List<MethodModel> Methods { get; } = new();
	}
}
