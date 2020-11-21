using System;
using System.Threading.Tasks;

using Bau.Libraries.DbStudio.Models.Connections;
using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.LibDbSchema.Repository.Xml;

namespace Bau.Libraries.DbStudio.Application.Controllers.Schema
{
	/// <summary>
	///		Manager para los repositorios de esquema
	/// </summary>
	public class SchemaManager
	{
		public SchemaManager(SolutionManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Graba los datos del esquema de una conexión
		/// </summary>
		public async Task SaveAsync(ConnectionModel connection, string fileName)
		{
			new SchemaXmlManager().Save(await Manager.DbScriptsManager.GetDbSchemaAsync(connection, System.Threading.CancellationToken.None), 
										fileName);
		}

		/// <summary>
		///		Manager de la solución
		/// </summary>
		public SolutionManager Manager { get; }
	}
}
