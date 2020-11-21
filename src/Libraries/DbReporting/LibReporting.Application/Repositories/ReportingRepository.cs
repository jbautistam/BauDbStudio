using System;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibReporting.Application.Repositories
{
	/// <summary>
	///		Repositorio de <see cref="Models.ReportingSolutionModel"/>
	/// </summary>
	internal class ReportingRepository
	{
		// Constantes privadas
		private const string TagRoot = "Reporting";
		private const string TagFile = "File";

		internal ReportingRepository(ReportingManager manager)
		{
			Manager = manager;
		}

		/// <summary>
		///		Carga un archivo
		/// </summary>
		internal void Load(string fileName)
		{
			if (System.IO.File.Exists(fileName))
			{
				MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

					if (fileML != null)
					{
						// Carga los archivos
						foreach (MLNode rootML in fileML.Nodes)
							if (rootML.Name == TagRoot)
								foreach (MLNode nodeML in rootML.Nodes)
									if (nodeML.Name == TagFile)
									{
										string file = nodeML.Value.TrimIgnoreNull();

											if (!string.IsNullOrWhiteSpace(fileName) && System.IO.File.Exists(file))
												Manager.ReportingSolution.Files.Add(file);
									}
					}
			}
		}

		/// <summary>
		///		Graba los archivos de la solución
		/// </summary>
		internal void Save()
		{
			MLFile fileML = new MLFile();
			MLNode rootML = fileML.Nodes.Add(TagRoot);

				// Añade los archivos
				foreach (string file in Manager.ReportingSolution.Files)
					rootML.Nodes.Add(TagFile, file);
				// Graba el archivo
				new LibMarkupLanguage.Services.XML.XMLWriter().Save(Manager.ReportingSolution.FileName, fileML);
		}

		/// <summary>
		///		Manager
		/// </summary>
		internal ReportingManager Manager { get; }
	}
}
