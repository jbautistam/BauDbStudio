using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Bau.DbStudio.Controls
{
	/// <summary>
	///		Control que extiende DockingManager para ofrecer algunas funciones interesantes como el control activo
	/// </summary>
	public class DockLayout : Xceed.Wpf.AvalonDock.DockingManager
	{
		// Eventos públicos
		public event EventHandler<ClosingEventArgs> Closing;
		public event EventHandler ActiveDocumentChanged;
		/// <summary>
		///		Posición en la que se puede colocar un panel
		/// </summary>
		public enum DockPosition
		{
			/// <summary>Izquierda</summary>
			Left,
			/// <summary>Superior</summary>
			Top,
			/// <summary>Derecha</summary>
			Right,
			/// <summary>Abajo</summary>
			Bottom
		}
		/// <summary>
		///		Temas de las ventanas
		/// </summary>
		public enum DockTheme
		{
			Aero,
			Metro,
			VS2010Theme
		}
		// Variables privadas
		private DockLayoutDocument _activeDocument;

		public DockLayout()
		{
			ActiveContentChanged += (sender, evntArgs) =>
													{
														if (ActiveContent != null && Layout != null && Layout.ActiveContent != null)
															ActiveDocument = GetDocument(Layout.ActiveContent.ContentId);
														else
															ActiveDocument = null;
													};
		}

		/// <summary>
		///		Añade / selecciona un panel
		/// </summary>
		public void AddPane(string id, string header, UserControl control, object tag = null, DockPosition position = DockPosition.Bottom)
		{
			DockLayoutDocument previous = GetDocument(id);

				if (previous != null)
				{
					previous.LayoutContent.IsActive = true;
					ActiveDocument = previous;
				}
				else
				{
					LayoutAnchorGroup layoutGroup = GetGroupPane(Layout, position);
					LayoutAnchorable layoutPane = new LayoutAnchorable { Title = header, ToolTip = header };

						// Añade el contenido
						layoutPane.Content = control;
						layoutPane.ContentId = id;
						// Asigna los parámetros del panel
						layoutPane.FloatingHeight = 200;
						layoutPane.FloatingWidth = 200;
						// Añade el contenido al grupo
						layoutGroup.Children.Add(layoutPane);
						layoutPane.IsActive = true;
						layoutPane.IsVisible = true;
						// Añade el panel a la lista de documentos del controlador
						AddDocument(id, header, DockLayoutDocument.DocumentType.Panel, layoutPane, control, tag);
				}
		}

		/// <summary>
		///		Abre / cierra un panel lateral
		/// </summary>
		public void OpenGroup(DockPosition position)
		{
			LayoutAnchorGroup layoutGroup = GetGroupPane(Layout, position);

				// Abre el panel
				if (layoutGroup.Children.Count > 0)
				{
					// Cambia el ancho / alto del grupo
					switch (position)
					{
						case DockPosition.Left:
						case DockPosition.Right:
								layoutGroup.Children[0].FloatingWidth = ActualWidth / 4;
								layoutGroup.Children[0].AutoHideWidth = ActualWidth / 4;
								layoutGroup.Children[0].AutoHideMinWidth = 200;
							break;
						default:
								layoutGroup.Children[0].AutoHideHeight = ActualHeight / 6;
								layoutGroup.Children[0].AutoHideMinHeight = 200;
							break;
					}
					// Cambia el autohide para que aparezca
					//? Después de ejecutar esta instrucción, parece que se cambia el grupo, por tanto ya no podemos utilizar layoutGroup.Children[0]
					//? que provocaría una excepción
					layoutGroup.Children[0].ToggleAutoHide();
				}
		}

		/// <summary>
		///		Añade / selecciona un documento
		/// </summary>
		public void AddDocument(string id, string header, UserControl control, object tag = null)
		{
			DockLayoutDocument previous = GetDocument(id);

				if (previous != null)
				{
					previous.LayoutContent.IsActive = true;
					ActiveDocument = previous;
				}
				else
				{
					LayoutDocumentPane documentPane = Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
					LayoutDocument layoutDocument = new LayoutDocument { Title = header, ToolTip = header };

						// Crea un documento y le asigna el control de contenido
						if (documentPane != null)
						{ 
							// Asigna el control
							layoutDocument.Content = control;
							layoutDocument.ContentId = id;
							// Añade el nuevo LayoutDocument al array existente
							documentPane.Children.Add(layoutDocument);
							// Activa el documento
							layoutDocument.IsActive = true;
							layoutDocument.IsSelected = true;
							// Cambia el ancho y alto flotante
							layoutDocument.FloatingWidth = ActualWidth / 2;
							layoutDocument.FloatingHeight = ActualHeight;
							// Cambia el foco al control
							control.Focus();
							// Añade el documento al controlador
							AddDocument(id, header, DockLayoutDocument.DocumentType.Document, layoutDocument, control, tag);
						}
				}
		}

		/// <summary>
		///		Obtiene el <see cref="DockLayoutDocument"/> a partir de un ID de ventana
		/// </summary>
		private DockLayoutDocument GetDocument(string id)
		{
			if (Documents.TryGetValue(id, out DockLayoutDocument document))
				return document;
			else
				return null;
		}

		/// <summary>
		///		Modifica el tabId de un documento activo
		/// </summary>
		public void UpdateTabId(string oldTabId, string newTabId, string newHeader)
		{
			if (Documents.TryGetValue(oldTabId, out DockLayoutDocument document) && !Documents.ContainsKey(newTabId))
			{
				// Cambia el título
				document.LayoutContent.Title = newHeader;
				// Añade el nuevo Id al diccionario y le cambia el Id interno
				Documents.Add(newTabId, document);
				document.Id = newTabId;
				// Elimina el documento con el Id antiguo
				Documents.Remove(oldTabId);
			}
		}

		/// <summary>
		///		Añade un documento al control o lo selecciona
		/// </summary>
		private void AddDocument(string id, string header, DockLayoutDocument.DocumentType type, 
								 LayoutContent layoutContent, UserControl userControl, object tag = null)
		{ 
			DockLayoutDocument document = new DockLayoutDocument(id, header, type, layoutContent, userControl, tag);

				// Añade el documento al diccionario
				Documents.Add(document.Id, document);
				// Asigna los manejadores de evento a la vista
				layoutContent.Closing += (sender, args) => args.Cancel = !TreatEventCloseForm(document);
				layoutContent.IsActiveChanged += (sender, args) => RaiseEventChangeDocument(document);
				// Activa el documento
				document.LayoutContent.IsActive = true;
				ActiveDocument = document;
		}

		/// <summary>
		///		Obtiene el grupo de ventanas del panel de la posición especificada
		/// </summary>
		private LayoutAnchorGroup GetGroupPane(LayoutRoot layoutRoot, DockPosition position)
		{
			LayoutAnchorSide layoutSide = GetAnchorSide(layoutRoot, position);

				// Crea el panel si no existía
				if (layoutSide == null)
					layoutSide = new LayoutAnchorSide();
				// Añade un grupo si no existía
				if (layoutSide.Children == null || layoutSide.Children.Count == 0)
					layoutSide.Children.Add(new LayoutAnchorGroup());
				// Devuelve el primer grupo
				return layoutSide.Children[0];
		}

		/// <summary>
		///		Obtiene uno de los elementos laterales del gestor de ventanas
		/// </summary>
		private LayoutAnchorSide GetAnchorSide(LayoutRoot layoutRoot, DockPosition position)
		{
			switch (position)
			{
				case DockPosition.Right:
					return layoutRoot.RightSide;
				case DockPosition.Left:
					return layoutRoot.LeftSide;
				case DockPosition.Top:
					return layoutRoot.TopSide;
				default:
					return layoutRoot.BottomSide;
			}
		}

		/// <summary>
		///		Trata el evento de cierre de un documento
		/// </summary>
		private bool TreatEventCloseForm(DockLayoutDocument document)
		{
			bool canClose = true;

				// Si es un documento, antes de cerrar se pregunta a la ventana principal
				if (document.Type == DockLayoutDocument.DocumentType.Document)
				{
					ClosingEventArgs args = new ClosingEventArgs(document);

						// Llama al manejador de eventos
						Closing?.Invoke(this, args);
						// Si no se ha cancelado, se puede cerrar
						canClose = !args.Cancel;
				}
				// Si se debe cerrar, se quita del diccionario
				if (canClose && Documents.ContainsKey(document.Id))
					Documents.Remove(document.Id);
				// Devuelve el valor que indica si se puede cerrar el documento
				return canClose;
		}

		/// <summary>
		///		Cierra una ventana
		/// </summary>
		public void CloseTab(string tabId)
		{
			if (Documents.TryGetValue(tabId, out DockLayoutDocument document))
			{
				// Cierra el documento
				document.LayoutContent.Close();
				// Quita el elemento del diccionario
				Documents.Remove(tabId);
			}
		}

		/// <summary>
		///		Cierra todas las ventanas
		/// </summary>
		public void CloseAllDocuments()
		{
			List<DockLayoutDocument> documents = new List<DockLayoutDocument>();

				// Nota: Al cerrar un formulario se modifica la colección Documents, por tanto no se puede hacer un recorrido sobre Documents
				//			 porque da un error de colección modificada. Tampoco se puede hacer un recorrido for (int...) sobre Documents porque
				//			 es un diccionario y no tiene indizador. Por tanto, tenemos que copiar los elementos del diccionario sobre una lista
				//			 y después recorrer los elementos de esta lista copiada desde el final hasta el principio.
				// Añade el diccionario de documentos a la lista
				foreach (KeyValuePair<string, DockLayoutDocument> document in Documents)
					documents.Add(document.Value);
				// Recorre la lista cerrando todos los documentos abiertos
				for (int index = documents.Count - 1; index >= 0; index--)
					if (documents[index] != null && documents[index].Type == DockLayoutDocument.DocumentType.Document && 
							documents[index].LayoutContent != null)
						try
						{
							documents[index].LayoutContent.Close();
						}
						catch { }
		}

		/// <summary>
		///		Obtiene los tag de las vistas abiertas
		/// </summary>
		public List<object> GetOpenedViews()
		{
			List<object> tags = new List<object>();

				// Obtiene los objetos asociados a los documentos abiertos
				foreach (KeyValuePair<string, DockLayoutDocument> document in Documents)
					if (document.Value.Tag != null)
						tags.Add(document.Value.Tag);
				// Devuelve la lista
				return tags;
		}

		/// <summary>
		///		Cambia el tema del layout
		/// </summary>
		public void SetTheme(DockTheme theme)
		{
			switch (theme)
			{
				case DockTheme.Aero:
						Theme = new Xceed.Wpf.AvalonDock.Themes.AeroTheme();
					break;
				case DockTheme.Metro:
						Theme = new Xceed.Wpf.AvalonDock.Themes.MetroTheme();
					break;
				default:
						Theme = new Xceed.Wpf.AvalonDock.Themes.VS2010Theme();
					break;
			}
		}

		/// <summary>
		///		Lanza el evento de cambio de documento
		/// </summary>
		private void RaiseEventChangeDocument(DockLayoutDocument document)
		{
			if (document != null && (document.LayoutContent?.IsActive ?? false))
				ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///		Documentos
		/// </summary>
		public Dictionary<string, DockLayoutDocument> Documents { get; } = new Dictionary<string, DockLayoutDocument>();

		/// <summary>
		///		Documento activo
		/// </summary>
		public DockLayoutDocument ActiveDocument
		{
			get { return _activeDocument; }
			private set
			{
				_activeDocument = value;
				RaiseEventChangeDocument(value);
			}
		}
	}
}