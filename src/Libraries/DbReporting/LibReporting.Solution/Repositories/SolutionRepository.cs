using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibReporting.Solution.Repositories;

/// <summary>
///		Repositorio de <see cref="Models.ReportingSolutionModel"/>
/// </summary>
internal class SolutionRepository
{
	// Constantes privadas
	private const string TagRoot = "Reporting";
	private const string TagFile = "File";

	internal SolutionRepository(ReportingSolutionManager manager)
	{
		Manager = manager;
	}

	/// <summary>
	///		Carga un archivo
	/// </summary>
	internal void Load(string fileName)
	{
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			// Carga los archivos
			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							if (nodeML.Name == TagFile)
							{
								string file = nodeML.Value.TrimIgnoreNull();

									if (File.Exists(file))
										Manager.ReportingSolution.Files.Add(file);
							}
	}

	/// <summary>
	///		Graba los archivos de la solución
	/// </summary>
	internal void Save(string fileName)
	{
		MLFile fileML = new MLFile();
		MLNode rootML = fileML.Nodes.Add(TagRoot);

			// Añade los archivos
			foreach (string file in Manager.ReportingSolution.Files)
				rootML.Nodes.Add(TagFile, file);
			// Graba el archivo
			new LibMarkupLanguage.Services.XML.XMLWriter().Save(fileName, fileML);
	}

	/// <summary>
	///		Manager
	/// </summary>
	internal ReportingSolutionManager Manager { get; }
}
