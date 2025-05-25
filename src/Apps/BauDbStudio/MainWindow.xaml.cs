using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Logging;

using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.DbStudio;

/// <summary>
///		Ventana principal de la aplicación
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa el formulario
	/// </summary>
	private void InitForm()
	{
		// Asigna el icono a la ventana
		Icon = System.Windows.Media.Imaging.BitmapFrame.Create(new Uri("pack://application:,,,./Resources/BauDbStudio.ico", UriKind.RelativeOrAbsolute)); 
		// Inicializa el contexto y los controles
		DbStudioViewsManager = new Controllers.DbStudioViewsManager("BauDbStudio", 
																	System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
																						   "Bau.DbStudio"),
																	this);
		DbStudioViewsManager.MainWindowsController.Logger.LogInformation("Start application");
		// Añade los plugins
		DbStudioViewsManager.AddPlugin(new Libraries.AiTools.Plugin.AiToolsPlugin());
		DbStudioViewsManager.AddPlugin(new Libraries.BlogReader.Views.BlogReaderPlugin());
		DbStudioViewsManager.AddPlugin(new Libraries.ChessDataBase.Plugin.ChessDataBasePlugin());
		DbStudioViewsManager.AddPlugin(new Libraries.DbStudio.Views.DbStudioViewManager());
		DbStudioViewsManager.AddPlugin(new Libraries.EbooksReader.Plugin.EBookReaderPlugin());
		DbStudioViewsManager.AddPlugin(new Libraries.FileTools.Plugin.FileToolsPlugin());
		DbStudioViewsManager.AddPlugin(new Libraries.JobsProcessor.Plugin.JobsProcessorPlugin());
		DbStudioViewsManager.AddPlugin(new Libraries.PasswordManager.Plugin.PasswordManagerPlugin());
		DbStudioViewsManager.AddPlugin(new Libraries.RestManager.Plugin.RestManagerPlugin());
		DbStudioViewsManager.AddPlugin(new Libraries.StructuredFilesStudio.Views.StructuredFilesStudioViewManager());
		DbStudioViewsManager.AddPlugin(new Libraries.ToDoManager.Plugin.ToDoManagerPlugin());
		DbStudioViewsManager.Initialize();
		// Inicializa el ViewModel
		DataContext = ViewModel = DbStudioViewsManager.PluginsStudioViewModel;
		// Carga los datos
		DbStudioViewsManager.Load(DbStudioViewsManager.ConfigurationController.PathData, DbStudioViewsManager.ConfigurationController.LastWorkSpace);
		ViewModel.LastFilesViewModel.Add(DbStudioViewsManager.ConfigurationController.LastFiles);
		// Añade los manejadores de eventos
		ViewModel.WorkspacesChanged += (sender, args) => ShowMenuWorkspaces();
		// Añade los paneles y barras de herramientas
		ShowPanes();
		ShowToolbars();
		ShowMenus();
		// Asigna los manejadores de eventos
		if (DbStudioViewsManager.AppViewController is Controllers.AppViewsController controller)
			controller.OpenDocumentRequired += (_, args) => AddTab(args.UserControl, args.ViewModel);
		// Asigna los manejadores de eventos del docker de documentos
		dckManager.Closing += (sender, args) => CloseWindow(args);
		dckManager.Closed += (sender, args) => DestroyWindow(args);
		dckManager.ActiveDocumentChanged += (sender, args) => UpdateSelectedTab();
		// Cambia el tema
		SetTheme((Controls.DockLayout.DockLayoutManager.DockTheme) DbStudioViewsManager.ConfigurationController.LastThemeSelected);
		// Muestra el número de versión
		lblVersion.Text = GetAssemblyVersion();
		// Carga los menús de espacios de trabajo
		ShowMenuWorkspaces();
		// Carga la configuración en los controladores
		ViewModel.MainController.MainWindowController.HostController.DialogsController.LastPathSelected = DbStudioViewsManager.ConfigurationController.LastPathSelected;
	}

	/// <summary>
	///		Muestra los paneles
	/// </summary>
	private void ShowPanes()
	{
		// Muestra los paneles
		foreach (Libraries.PluginsStudio.Views.Base.Models.PaneModel pane in DbStudioViewsManager.GetPanes())
			ShowPane(pane);
		// Abre los paneles predefinidos
		dckManager.OpenGroup(Controls.DockLayout.DockLayoutManager.DockPosition.Left);
	}

	/// <summary>
	///		Muestra un panel
	/// </summary>
	private bool ShowPane(Libraries.PluginsStudio.Views.Base.Models.PaneModel? pane)
	{
		bool showed = false;

			// Abre el panel
			if (pane is not null && pane.View is not null)
			{
				// Añade el panel al control
				dckManager.AddPane(pane.Id, pane.Title, pane.View, pane.ViewModel, ConvertPosition(pane.Position));
				// Añade el panel a la lista
				if (pane.View.DataContext is IPaneViewModel paneViewModel)
					ViewModel.WindowsViewModel.AddPane(paneViewModel.TabId, pane.Id, paneViewModel.Header);
				// Indica que se ha mostrado
				showed = true;
			}
			// Devuelve el valor que indica si se ha abierto
			return showed;
	}

	/// <summary>
	///		Convierte la posición
	/// </summary>
	private Controls.DockLayout.DockLayoutManager.DockPosition ConvertPosition(Libraries.PluginsStudio.Views.Base.Models.PaneModel.PositionType position)
	{
		return position switch
					{
						Libraries.PluginsStudio.Views.Base.Models.PaneModel.PositionType.Top => Controls.DockLayout.DockLayoutManager.DockPosition.Top,
						Libraries.PluginsStudio.Views.Base.Models.PaneModel.PositionType.Right => Controls.DockLayout.DockLayoutManager.DockPosition.Right,
						Libraries.PluginsStudio.Views.Base.Models.PaneModel.PositionType.Bottom => Controls.DockLayout.DockLayoutManager.DockPosition.Bottom,
						_ => Controls.DockLayout.DockLayoutManager.DockPosition.Left
					};
	}

	/// <summary>
	///		Muestra las barras de herramientas
	/// </summary>
	private void ShowToolbars()
	{
		foreach (Libraries.PluginsStudio.Views.Base.Models.ToolBarModel toolbar in DbStudioViewsManager.GetToolBars())
			if (toolbar?.ToolBar != null)
				tbMain.ToolBars.Add(toolbar.ToolBar);
	}

	/// <summary>
	///		Muestra los menús
	/// </summary>
	private void ShowMenus()
	{
		foreach (Libraries.PluginsStudio.ViewModels.Base.Models.MenuListModel menu in DbStudioViewsManager.GetMenus())
			switch (menu.Section)
			{
				case Libraries.PluginsStudio.ViewModels.Base.Models.MenuListModel.SectionType.NewItem:
						CreateChildMenus(mnuFilesNewItem, mnuStartFileNewItems, menu);
					break;
				case Libraries.PluginsStudio.ViewModels.Base.Models.MenuListModel.SectionType.Tools:
						CreateChildMenus(mnuTools, mnuToolsStart, menu);
					break;
			}
	}

	/// <summary>
	///		Crea elementos de menú hijo
	/// </summary>
	private void CreateChildMenus(MenuItem mainMenu, Separator separatorStart, Libraries.PluginsStudio.ViewModels.Base.Models.MenuListModel menu)
	{
		if (menu.Items.Count == 0)
			separatorStart.Visibility = Visibility.Collapsed;
		else
		{
			int startIndex = mainMenu.Items.IndexOf(separatorStart);

				foreach (Libraries.PluginsStudio.ViewModels.Base.Models.MenuModel menuItem in menu.Items)
					if (string.IsNullOrWhiteSpace(menuItem.Header))
						mainMenu.Items.Insert(++startIndex, new Separator());
					else
					{
						MenuItem newMenuItem = CreateMenu(menuItem.Header, menuItem.Icon, menuItem.IsCheckable, menuItem.Command, menuItem.Tag);

							// Inserta el menú tras el separador
							mainMenu.Items.Insert(++startIndex, newMenuItem);
					}
		}
	}

	/// <summary>
	///		Obtiene el código de versión del ensamblado (Properties.AssemblyVersion)
	/// </summary>
	private string GetAssemblyVersion()
	{
		// Obtiene la versión del ensamblado
		try
		{
			Version? version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

				return $"{version?.Major.ToString()}.{version?.Minor.ToString()}.{version?.Build.ToString()}.{version?.MinorRevision.ToString()}";
		}
		catch (Exception exception)
		{
			System.Diagnostics.Trace.TraceError($"Exception when get assembly version. {exception.Message}");
		}
		// Si ha llegado hasta aquí es porque no ha encontrado la versión
		return string.Empty;
	}

	/// <summary>
	///		Abre una ventana con un archivo
	/// </summary>
	private void OpenFile(string fileName)
	{
		DbStudioViewsManager.OpenFile(fileName);
	}

	/// <summary>
	///		Añade una ficha al control
	/// </summary>
	private void AddTab(UserControl control, IDetailViewModel detailsViewModel)
	{
		dckManager.AddDocument(detailsViewModel.TabId, detailsViewModel.Header, control, detailsViewModel);
		ViewModel.WindowsViewModel.AddDocument(detailsViewModel.TabId, detailsViewModel.Header);
	}

	/// <summary>
	///		Modifica el TabId de un documento abierto
	/// </summary>
	public void UpdateTabId(string oldTabId, string newTabId, string newHeader)
	{
		dckManager.UpdateTabId(oldTabId, newTabId, newHeader);
	}

	/// <summary>
	///		Muestra / oculta un panel
	/// </summary>
	public bool ShowPane(string tabId, string documentId, bool visible)
	{
		bool showed = false;

			// Oculta / muestra el panel
			if (!visible)
				showed = dckManager.HidePane(documentId);
			else
			{
				Libraries.PluginsStudio.Views.Base.Models.PaneModel? panel = DbStudioViewsManager.GetPane(documentId);

					if (panel is not null)
						showed = ShowPane(panel);
			}
			// Devuelve el valor que indica si se ha mostrado / ocultado el panel
			return showed;
	}

	/// <summary>
	///		Cierra una ventana
	/// </summary>
	internal void CloseTab(string tabId)
	{
		dckManager.CloseTab(tabId);
	}

	/// <summary>
	///		Cierra una ficha
	/// </summary>
	private void CloseWindow(Controls.DockLayout.EventArguments.ClosingEventArgs args)
	{
		if (args.Document != null && args.Document.Tag != null && args.Document.Tag is IDetailViewModel detailViewModel && 
			detailViewModel.IsUpdated)
		{
			Libraries.BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType result = DbStudioViewsManager.MainWindowsController.HostController.SystemController.ShowQuestionCancel
																										(detailViewModel.GetSaveAndCloseMessage());

				switch (result)
				{
					case Libraries.BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes:
							detailViewModel.SaveDetails(false);
						break;
					case Libraries.BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Cancel:
							args.Cancel = true;
						break;
				}
		}
	}

	/// <summary>
	///		Destruye una ficha
	/// </summary>
	private void DestroyWindow(Controls.DockLayout.EventArguments.ClosedEventArgs args)
	{
		if (args.Document.UserControl is not null && args.Document.UserControl.DataContext is IDocumentViewModel document)
		{
			// Cierra el documento
			document.Close();
			// Lo quita de la lista de ventanas
			ViewModel.WindowsViewModel.Close(document.TabId);
		}
	}

	/// <summary>
	///		Modifica la ventana de detalles seleccionada
	/// </summary>
	private void UpdateSelectedTab()
	{
		if (dckManager.ActiveDocument?.Tag is IDetailViewModel details)
		{
			ViewModel.SelectedDetailsViewModel = details;
			ViewModel.WindowsViewModel.SetActiveDocument(details);
		}
		else
			ViewModel.SelectedDetailsViewModel = null;
	}

	/// <summary>
	///		Obtiene la ventana de detalles activa
	/// </summary>
	internal IDetailViewModel? GetActiveDetails()
	{
		if (dckManager.ActiveDocument?.Tag is IDetailViewModel details)
			return details;
		else
			return null;
	}

	/// <summary>
	///		Obtiene el panel activo
	/// </summary>
	internal IPaneViewModel? GetActivePane()
	{
		if (dckManager.ActiveDocument?.UserControl?.DataContext is IPaneViewModel pane)
			return pane;
		else
			return null;
	}

	/// <summary>
	///		Obtiene la lista de ventanas de detalles abiertas
	/// </summary>
	internal List<IDetailViewModel> GetOpenedDetails()
	{
		List<IDetailViewModel> detailViewModels = [];

			// Obtiene la lista
			foreach (object view in dckManager.GetOpenedViews())
				if (view is IDetailViewModel viewModel)
					detailViewModels.Add(viewModel);
			// Devuelve la lista
			return detailViewModels;
	}

	/// <summary>
	///		Activa una ventana de detalles
	/// </summary>
	internal void ActivateDetails(string tabId, string documentId)
	{
		dckManager.ActivateDocument(documentId);
	}

	/// <summary>
	///		Cierra todas las ventanas
	/// </summary>
	private void CloseAllWindows()
	{
		dckManager.CloseAllDocuments();
	}

	/// <summary>
	///		Cambia el tema del layout
	/// </summary>
	private void SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme newTheme)
	{
		// Cambia el tema
		dckManager.SetTheme(newTheme);
		// Cambia los menús
		mnuThemeAero.IsChecked = newTheme == Controls.DockLayout.DockLayoutManager.DockTheme.Aero;
		mnuThemeMetro.IsChecked = newTheme == Controls.DockLayout.DockLayoutManager.DockTheme.Metro;
		mnuThemeVs2010.IsChecked = newTheme == Controls.DockLayout.DockLayoutManager.DockTheme.VS2010Theme;
		mnuThemeExpressionDark.IsChecked = newTheme == Controls.DockLayout.DockLayoutManager.DockTheme.ExpressionDark;
		mnuThemeExpressionLight.IsChecked = newTheme == Controls.DockLayout.DockLayoutManager.DockTheme.ExpressionLight;
		mnuThemeVs2013Blue.IsChecked = newTheme == Controls.DockLayout.DockLayoutManager.DockTheme.VS2013BlueTheme;
		mnuThemeVs2013Dark.IsChecked = newTheme == Controls.DockLayout.DockLayoutManager.DockTheme.VS2013DarkTheme;
		mnuThemeVs2013Light.IsChecked = newTheme == Controls.DockLayout.DockLayoutManager.DockTheme.VS2013LightTheme;
		// Cambia la configuración
		DbStudioViewsManager.ConfigurationController.LastThemeSelected = (int) newTheme;
	}

	/// <summary>
	///		Abre la ventana de configuración
	/// </summary>
	private void OpenConfigurationWindow()
	{
		DbStudioViewsManager.MainWindowsController.HostHelperController.ShowDialog(this, new Views.Tools.Configuration.ConfigurationView(DbStudioViewsManager));
	}

	/// <summary>
	///		Abre la ventana de búsqueda
	/// </summary>
	private void OpenSearchWindow()
	{
		if (dckManager.ActiveDocument?.UserControl is Views.Files.FileTextView fileView)
			fileView.OpenSearch(true);
		else
		{
			IDetailViewModel? viewModel = GetActiveDetails();

				if (viewModel is not null)
					viewModel.StartSearch();
		}
	}

	/// <summary>
	///		Muestra el menú de espacios de trabajo
	/// </summary>
	private void ShowMenuWorkspaces()
	{
		int startIndex = mnuWorkspace.Items.IndexOf(mnuStartWorkspaces);
		int indexEnd = mnuWorkspace.Items.IndexOf(mnuEndWorkspaces);

			// Borra las opciones de menú que se hubiesen creado anteriormente
			DeleteMenusBetween(mnuWorkspace, startIndex, indexEnd);
			// Muestra los menús
			foreach (Libraries.PluginsStudio.ViewModels.Tools.Workspaces.WorkSpaceViewModel workspace in ViewModel.WorkspacesViewModel.Items)
			{
				MenuItem mnuNewWorkspace = CreateMenu(workspace.Name, string.Empty, false, null, workspace);

					// Inserta el menú tras el separador
					mnuWorkspace.Items.Insert(++startIndex, mnuNewWorkspace);
					// Añade el manejador
					mnuNewWorkspace.Click += (sender, args) => 
												{
													if ((sender as MenuItem)?.Tag is Libraries.PluginsStudio.ViewModels.Tools.Workspaces.WorkSpaceViewModel workSpace)
													{
														// Selecciona el espacio de trabajo
														if (workSpace != null)
															DbStudioViewsManager.SelectWorkspace(workSpace.Name);
														// Cambia las marcas de check de los menús
														CheckWorkSpaceMenu();
													}
												};
					mnuNewWorkspace.Checked += (sender, args) => CheckWorkSpaceMenu();
					mnuNewWorkspace.IsChecked = false;
			}
			// Muestra el separador
			if (ViewModel.WorkspacesViewModel.Items.Count == 0)
				mnuEndWorkspaces.Visibility = Visibility.Collapsed;
			else
				mnuEndWorkspaces.Visibility = Visibility.Visible;
			// Selecciona el menú adecuado
			CheckWorkSpaceMenu();
	}

	/// <summary>
	///		Selecciona el menú asociado al workspace seleccionado
	/// </summary>
	private void CheckWorkSpaceMenu()
	{
		int startIndex = mnuWorkspace.Items.IndexOf(mnuStartWorkspaces);
		int indexEnd = mnuWorkspace.Items.IndexOf(mnuEndWorkspaces);

			// Recorre los menús seleccionando / deseleccionando
			for (int index = startIndex; index < indexEnd; index++)
				if (mnuWorkspace.Items[index] is MenuItem child && 
						child.Tag is Libraries.PluginsStudio.ViewModels.Tools.Workspaces.WorkSpaceViewModel workSpace)
					child.IsChecked = workSpace.Name.Equals(ViewModel.WorkspacesViewModel.SelectedItem?.Name, StringComparison.CurrentCultureIgnoreCase);
	}

	/// <summary>
	///		Crea una opción de menú
	/// </summary>
	private MenuItem CreateMenu(string text, string icon, bool isCheckable, System.Windows.Input.ICommand? command, object? tag = null)
	{
		MenuItem mnuNewItem = new();

			// Asigna las propiedades
			mnuNewItem.Header = text;
			if (!string.IsNullOrWhiteSpace(icon))
				mnuNewItem.Icon = new Libraries.BauMvvm.Views.Wpf.Tools.ToolsWpf().GetImage(icon);
			mnuNewItem.Tag = tag;
			mnuNewItem.IsCheckable = isCheckable;
			// Añade el comando
			mnuNewItem.Command = command;
			// Devuelve la opción de menú creada
			return mnuNewItem;
	}

	/// <summary>
	///		Borra los menús entre dos separadores
	/// </summary>
	private void DeleteMenusBetween(MenuItem mnuParent, int startIndex, int indexEnd)
	{
		for (int index = indexEnd - 1; index > startIndex; index--)
			mnuParent.Items.RemoveAt(index);
	}

	/// <summary>
	///		Comprueba si hay algún elemento sin guardar
	/// </summary>
	private bool CanExitApp()
	{
		List<IDetailViewModel> views = new();
		bool canExit = true;

			// Comprueba si alguna de las vistas tiene modificaciones pendientes
			foreach (object view in dckManager.GetOpenedViews())
				if (view is IDetailViewModel viewModel && viewModel.IsUpdated)
					views.Add(viewModel);
			// Muestra la ventana que solicita grabar las ventanas
			if (views.Count > 0)
			{
				bool cancel = ViewModel.MainController.OpenDialog(new Libraries.PluginsStudio.ViewModels.Tools.SaveOpenFilesViewModel(ViewModel, views)) 
								== Libraries.BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.No;

					// No se puede salir si se ha cancelado
					if (cancel)
						canExit = false;
			}
			// Devuelve el valor que indica si se puede grabar
			return canExit;
	}

	/// <summary>
	///		Sale de la aplicación
	/// </summary>
	private void ExitApp()
	{
		// Cierra los plugins
		DbStudioViewsManager.PluginsManager.CloseApp();
		// Graba la configuración
		if (!string.IsNullOrWhiteSpace(ViewModel.MainController.MainWindowController.HostController.DialogsController.LastPathSelected))
			DbStudioViewsManager.ConfigurationController.LastPathSelected = ViewModel.MainController.MainWindowController.HostController.DialogsController.LastPathSelected;
		DbStudioViewsManager.ConfigurationController.LastWorkSpace = ViewModel.WorkspacesViewModel.SelectedItem?.Name ?? string.Empty;
		DbStudioViewsManager.ConfigurationController.LastFiles = ViewModel.LastFilesViewModel.GetFiles();
		DbStudioViewsManager.ConfigurationController.Save();
		// Cierra la aplicación (puede que dé una excepción al cerrar porque ya se está cerrando en el evento Closing)
		try
		{
			Close();
		}
		catch {}
	}

	/// <summary>
	///		Abre la ventana Acerca de
	/// </summary>
	private void OpenAboutWindow()
	{
		Views.Tools.AboutView view = new(GetAssemblyVersion());

			// Muestra la ventana
			view.Owner = this;
			view.ShowDialog();
	}

	/// <summary>
	///		Manager de vistas de PluginsStudio
	/// </summary>
	public static Controllers.DbStudioViewsManager DbStudioViewsManager { get; private set; } = default!;

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public Libraries.PluginsStudio.ViewModels.PluginsStudioViewModel ViewModel { get; private set; } = default!;

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		InitForm();
	}

	private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
	{
		ExitApp();
	}

	private void CloseAllWindows_Click(object sender, RoutedEventArgs e)
	{
		CloseAllWindows();
	}

	private void ThemeAero_Click(object sender, RoutedEventArgs e)
	{
		SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme.Aero);
	}

	private void ThemeMetro_Click(object sender, RoutedEventArgs e)
	{
		SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme.Metro);
	}

	private void ThemeVS2010_Click(object sender, RoutedEventArgs e)
	{
		SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme.VS2010Theme);
	}

	private void mnuThemeExpressionDark_Click(object sender, RoutedEventArgs e)
	{
		SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme.ExpressionDark);
	}

	private void mnuThemeExpressionLight_Click(object sender, RoutedEventArgs e)
	{
		SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme.ExpressionLight);
	}

	private void mnuThemeVs2013Light_Click(object sender, RoutedEventArgs e)
	{
		SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme.VS2013LightTheme);
	}

	private void mnuThemeVs2013Blue_Click(object sender, RoutedEventArgs e)
	{
		SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme.VS2013BlueTheme);
	}

	private void mnuThemeVs2013Dark_Click(object sender, RoutedEventArgs e)
	{
		SetTheme(Controls.DockLayout.DockLayoutManager.DockTheme.VS2013DarkTheme);
	}

	private void Window_Unloaded(object sender, RoutedEventArgs e)
	{
		ExitApp();
	}

	private void Configuration_Click(object sender, RoutedEventArgs e)
	{
		OpenConfigurationWindow();
	}

	private void SaveCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		ViewModel.SaveCommand.Execute(null);
	}

	private void SaveCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = ViewModel.SaveCommand.CanExecute(null);
	}

	private void DeleteCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		ViewModel.DeleteCommand.Execute(null);
	}

	private void DeleteCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
	{
		if (ViewModel is not null)
			e.CanExecute = ViewModel.DeleteCommand.CanExecute(null);
	}

	private void SearchCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		OpenSearchWindow();
	}

	private void ExecuteCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		ViewModel.ExecuteCommand.Execute(null);
	}

	private void ExecuteCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
	{
		if (ViewModel is not null)
			e.CanExecute = ViewModel.ExecuteCommand.CanExecute(null);
	}

	private void NewQueryCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
	{
		ViewModel.MainController.HostPluginsController.ExecutePluginCommand("DbStudio", "TreeConnectionsViewModel", "NewQueryCommand");
	}

	private void NewQueryCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = ViewModel.MainController.HostPluginsController.CheckCanExecutePluginCommand("DbStudio", "TreeConnectionsViewModel", "NewQueryCommand");
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		e.Cancel = !CanExitApp();
		if (!e.Cancel)
			ExitApp();
	}

	private void dckManager_OpenFileRequired(object sender, Controls.DockLayout.EventArguments.OpenFileRequiredArgs e)
	{
		if (!string.IsNullOrWhiteSpace(e.FileName) && System.IO.File.Exists(e.FileName))
			OpenFile(e.FileName);
	}

	private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
	{
		OpenAboutWindow();
	}
}
