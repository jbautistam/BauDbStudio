using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Media;

namespace Bau.Libraries.PluginsStudio.ViewModels.Base.Explorers
{
	/// <summary>
	///		ViewModel base de un nodo del árbol de exploración de una solución
	/// </summary>
	public abstract class BaseTreeNodeViewModel : ControlHierarchicalViewModel
	{	
/*
		/// <summary>
		///		Tipo de nodo
		/// </summary>
		public enum NodeType
		{
			/// <summary>Desconocido. No se debería utilizar</summary>
			Unknown,
			/// <summary>Raíz de la conexión</summary>
			ConnectionRoot,
			/// <summary>Conexión</summary>
			Connection,
			/// <summary>Esquema de una conexión</summary>
			SchemaRoot,
			/// <summary>Tabla</summary>
			Table,
			/// <summary>Raíz de la distribución</summary>
			DeploymentRoot,
			/// <summary>Distribución</summary>
			Deployment,
			/// <summary>Raíz de archivos de proyecto</summary>
			FilesRoot,
			/// <summary>Archivo / directorio</summary>
			File,
			/// <summary>Conexión a storage</summary>
			Storage,
			/// <summary>Contenedor de storage</summary>
			StorageContainer,
			/// <summary>Mensaje (transitorio)</summary>
			Message,
			/// <summary>Almacén de datos</summary>
			DataWarehouse,
			/// <summary>Raíz de origen de datos</summary>
			DataSourcesRoot,
			/// <summary>Origen de datos</summary>
			DataSource,
			/// <summary>Raíz de dimensiones</summary>
			DimensionsRoot,
			/// <summary>Dimensión</summary>
			Dimension,
			/// <summary>Raíz de informes</summary>
			ReportsRoot,
			/// <summary>Informe</summary>
			Report
		}
		/// <summary>
		///		Tipo de icono
		/// </summary>
		public enum IconType
		{
			Connection,
			Deployment,
			Project,
			Path,
			File,
			Schema,
			Table,
			View,
			Key,
			Field,
			Error,
			Loading,
			Storage,
			Report,
			DataSourceSql,
			Dimension
		}
		// Variables privadas
		private IconType _icon;
*/

		public BaseTreeNodeViewModel(BaseTreeViewModel trvTree, IHierarchicalViewModel parent, string text, 
									 string type, string icon, object tag, 
									 bool lazyLoad, bool isBold = false, MvvmColor foreground = null) 
							: base(parent, text, tag, lazyLoad, isBold, foreground)
		{ 
			TreeViewModel = trvTree;
			Type = type;
			Icon = icon;
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		public override void LoadChildrenData()
		{
			LoadNodes();
		}

		/// <summary>
		///		Obtiene el calor pasado como parámetro o el color por defecto si no está activo
		/// </summary>
		public MvvmColor GetColor(MvvmColor defaultColor, bool enabled)
		{
			if (!enabled)
				return MvvmColor.Gray;
			else
				return defaultColor;
		}

		/// <summary>
		///		Obtiene el texto para las operaciones de drag & drop sobre el editor
		/// </summary>
		public virtual string GetTextForEditor(bool shiftPressed)
		{
			return string.Empty;
		}

		/// <summary>
		///		Carga los nodos hijo
		/// </summary>
		protected abstract void LoadNodes();

		/// <summary>
		///		ViewModel del árbol
		/// </summary>
		public BaseTreeViewModel TreeViewModel { get; }

		/// <summary>
		///		Tipo de nodo
		/// </summary>
		public string Type { get; }

		///// <summary>
		/////		Icono
		///// </summary>
		//public IconType Icon 
		//{ 
		//	get { return _icon; }
		//	set { CheckProperty(ref _icon, value); }
		//}
	}
}