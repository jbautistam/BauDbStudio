﻿using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Explorers.Files
{
	/// <summary>
	///		ViewModel de un nodo de archivo
	/// </summary>
	public class NodeFileViewModel : BaseTreeNodeViewModel
	{
		// Variables privadas
		private string _fileName;
		private bool _isFolder;

		public NodeFileViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, string fileName, bool isFolder) 
					: base(trvTree, parent, string.Empty, NodeType.File, isFolder ? IconType.Path : IconType.File, 
						   fileName, isFolder, isFolder,
						   isFolder ? MvvmColor.Navy : MvvmColor.Black)
		{
			FileName = fileName;
			IsFolder = isFolder;
			if (!string.IsNullOrWhiteSpace(FileName))
			{
				Text = System.IO.Path.GetFileName(FileName);
				ToolTipText = FileName;
			}
			else
				Text = "...";
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected override void LoadNodes()
		{
			if (!string.IsNullOrWhiteSpace(FileName) && System.IO.Directory.Exists(FileName))
			{
				// Carga los directorios
				foreach (string fileName in System.IO.Directory.EnumerateDirectories(FileName))
					AddNode(fileName, true);
				// Carga los archivos
				foreach (string fileName in System.IO.Directory.EnumerateFiles(FileName))
					AddNode(fileName, false);
			}
		}

		/// <summary>
		///		Añade un nodo
		/// </summary>
		private void AddNode(string fileName, bool isFolder)
		{
			Children.Add(new NodeFileViewModel(TreeViewModel, this, fileName, isFolder));
		}

		/// <summary>
		///		Obtiene la cadena que se debe insertar en un cuadro de texto dependiendo del tipo de nodo seleccionado
		/// </summary>
		public string GetDroppedText(bool fullSql)
		{
			string result = string.Empty;

				// Obtiene la cadena SQL dependiendo del tipo de archivo
				if (FileName.EndsWith(".parquet", StringComparison.CurrentCultureIgnoreCase))
				{
					if (fullSql)
						result = GetSqlSelectParquet();
					else
						result += $"parquet.`{FileName}`";
				}
				else if (FileName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
					result += $"csv.`{FileName}`";
				else
					result += FileName;
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Obtiene la cadena que se debe insertar en un cuadro de texto dependiendo del tipo de nodo seleccionado
		/// </summary>
		public string GetAdvancedDroppedText(Details.Files.FileViewModel droppedFileViewModel, bool fullSql)
		{
			string result = GetDroppedText(fullSql);

				// Si como resultado tenemos el nombre de archivo, es porque aún no se ha convertido nada
				if (!string.IsNullOrWhiteSpace(result) || result.Equals(FileName, StringComparison.CurrentCultureIgnoreCase))
				{
					// Si lo estamos dejando sobre un archivo de markdown
					if (!string.IsNullOrWhiteSpace(droppedFileViewModel.FileName) && 
						droppedFileViewModel.FileName.EndsWith(".md", StringComparison.CurrentCultureIgnoreCase))
					{
						string name = System.IO.Path.GetFileName(FileName);

							if (LibHelper.Files.HelperFiles.CheckIsImage(FileName))
								result = $"![{name}]({FileName.Replace('\\', '/')} \"{name}\")";
							else
								result = $"[{name}]({System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FileName), System.IO.Path.GetFileNameWithoutExtension(FileName)).Replace('\\', '/')})";
					}
				}
				// Devuelve el resultado
				return result;
		}

		/// <summary>
		///		Obtiene la cadena SQL de consulta de un archivo parquet
		/// </summary>
		private string GetSqlSelectParquet()
		{
			string sql = "SELECT ";
			int length = 80;

				// Añade los nombres de campos
				using (LibParquetFiles.Readers.ParquetDataReader reader = new LibParquetFiles.Readers.ParquetDataReader(FileName))
				{
					// Abre el archivo
					reader.Open();
					// Añade los nombres de campo
					for (int index = 0; index < reader.FieldCount; index++)
					{
						// Añade un salto de línea si es necesario
						if (sql.Length > length)
						{
							sql += Environment.NewLine + "\t\t";
							length += 80;
						}
						// Nombre de campo
						sql += $" `{reader.GetName(index)}`";
						// Añade la coma si es necesario
						if (index < reader.FieldCount -1)
							sql += ", ";
					}
				}
				// Añade el nombre de tabla
				sql += Environment.NewLine + $"\tFROM parquet.`{FileName}`";
				// Devuelve la cadena creada
				return sql;
		}

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { CheckProperty(ref _fileName, value); }
		}

		/// <summary>
		///		Indica si se trata de una carpeta
		/// </summary>
		public bool IsFolder
		{
			get { return _isFolder; }
			set { CheckProperty(ref _isFolder, value); }
		}
	}
}
