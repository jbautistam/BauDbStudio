using System;

namespace Bau.Libraries.DbStudio.Conversor.Transformers
{
	/// <summary>
	///		Traductor de archivos Python
	/// </summary>
	internal class PythonFileTransformer : BaseTransformer
	{
		internal PythonFileTransformer(DatabrickExporter exporter, string targetPath, string fileName) : base(exporter, targetPath, fileName)
		{
		}

		/// <summary>
		///		Transforma el contenido del archivo en un notebook adecuado para databricks
		/// </summary>
		internal override void Transform()
		{
			SaveFileWithoutBom(GetTargetFileName(".py"), LibHelper.Files.HelperFiles.LoadTextFile(Source));
		}
	}
}
