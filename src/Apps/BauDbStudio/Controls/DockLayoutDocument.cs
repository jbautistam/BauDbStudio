using System;
using System.Windows.Controls;

using AvalonDock.Layout;

namespace Bau.DbStudio.Controls
{
	/// <summary>
	///		Documento o panel asociado a un <see cref="DockLayout"/>
	/// </summary>
	public class DockLayoutDocument
	{
		/// <summary>
		///		Tipo de documento
		/// </summary>
		public enum DocumentType
		{
			/// <summary>Panel</summary>
			Panel,
			/// <summary>Documento / ventana principal</summary>
			Document
		}

		public DockLayoutDocument(string id, string header, DocumentType type, LayoutContent layoutContent, UserControl userControl, object tag = null)
		{
			Id = id;
			Header = header;
			Type = type;
			LayoutContent = layoutContent;
			UserControl = userControl;
			Tag = tag;
		}

		/// <summary>
		///		Clave
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header { get; }

		/// <summary>
		///		Tipo de documento
		/// </summary>
		public DocumentType Type { get; }

		/// <summary>
		///		Layout al que se asocia la ventana
		/// </summary>
		public LayoutContent LayoutContent { get; }

		/// <summary>
		///		Control de usuario
		/// </summary>
		public UserControl UserControl { get; }

		/// <summary>
		///		Objeto asociado
		/// </summary>
		public object Tag { get; }
	}
}
