using System;

namespace Bau.Libraries.DbStudio.Models.Core.Workspaces
{
	/// <summary>
	///		Clase con los datos de un espacio de trabajo
	/// </summary>
	public class WorkspaceModel
	{
		/// <summary>
		///		Nombre del espacio de trabajo
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///		Nombre del espacio de trabajo normalizado
		/// </summary>
		public string NormalizedName
		{
			get
			{
				return Name;
			}
		}
	}
}
