using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels;
using Bau.Libraries.DbStudio.ViewModels.Core.Interfaces;
using Bau.DbStudio.Views.Files;

namespace Bau.DbStudio
{
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
			// Inicializa el controlador
			MainController = new Controllers.AppController("Bau.DbStudio", this, 
														   System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bau.DbStudio"));
			MainController.ConfigurationController.Load();
			// Inicializa el contexto y los controles
			DataContext = ViewModel = new MainViewModel(MainController.SparkSolutionController, MainController.ConfigurationController.LastWorkSpace);
			// Carga la última solución
			ViewModel.Load();
			ViewModel.LastPathSelected = MainController.ConfigurationController.LastPathSelected;
			ViewModel.LastFilesViewModel.Add(MainController.ConfigurationController.LastFiles);
			// Añade los manejadores de eventos
			ViewModel.WorkspacesChanged += (sender, args) => ShowMenuWorkspaces();
			// Añade los paneles
			dckManager.AddPane("TreeFilesExplorer", "Archivos", new Views.TreeFilesExplorer(ViewModel.SolutionViewModel.TreeFoldersViewModel), 
							   null, Controls.DockLayout.DockLayoutManager.DockPosition.Left);
			dckManager.AddPane("TreeConnectionsExplorer", "Conexiones", new Views.TreeConnectionsExplorer(ViewModel.SolutionViewModel.TreeConnectionsViewModel), 
							   null, Controls.DockLayout.DockLayoutManager.DockPosition.Left);
			dckManager.AddPane("TreeReportingExplorer", "Informes", 
							   new Views.Reporting.Explorers.TreeReportingExplorer(ViewModel.SolutionViewModel.ReportingSolutionViewModel.TreeReportingViewModel), 
							   null, Controls.DockLayout.DockLayoutManager.DockPosition.Left);
			dckManager.AddPane("TreeStorageExplorer", "Storage", new Views.TreeStoragesExplorer(ViewModel.SolutionViewModel.TreeStoragesViewModel), 
							   null, Controls.DockLayout.DockLayoutManager.DockPosition.Right);
			dckManager.AddPane("LogView", "Log", new Views.Tools.LogView(ViewModel.LogViewModel), null, Controls.DockLayout.DockLayoutManager.DockPosition.Bottom);
			dckManager.AddPane("SearchVew", "Buscar", new Views.Tools.Search.SearchView(ViewModel.SearchFilesViewModel), 
							   null, Controls.DockLayout.DockLayoutManager.DockPosition.Right);
			// Abre los paneles predefinidos
			dckManager.OpenGroup(Controls.DockLayout.DockLayoutManager.DockPosition.Left);
			// Asigna los manejadores de eventos
			MainController.SparkSolutionController.OpenWindowRequired += (sender, args) => OpenWindow(args);
			// Asigna los manejadores de eventos del docker de documentos
			dckManager.Closing += (sender, args) => CloseWindow(args);
			dckManager.ActiveDocumentChanged += (sender, args) => UpdateSelectedTab();
			// Cambia el tema
			SetTheme((Controls.DockLayout.DockLayoutManager.DockTheme) MainController.ConfigurationController.LastThemeSelected);
			// Muestra el número de versión
			lblVersion.Text = GetAssemblyVersion();
			// Carga los menús de espacios de trabajo
			ShowMenuWorkspaces();
		}

		/// <summary>
		///		Obtiene el código de versión del ensamblado (Properties.AssemblyVersion)
		/// </summary>
		private string GetAssemblyVersion()
		{
			// Obtiene la versión del ensamblado
			try
			{
				Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

					return $"{version.Major}.{version.Minor}.{version.Build}.{version.MinorRevision}";
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
			OpenWindow(new Libraries.DbStudio.ViewModels.Solutions.Details.Files.FileViewModel(ViewModel.SolutionViewModel, fileName));
		}

		/// <summary>
		///		Abre la ventana de detalles
		/// </summary>
		private void OpenWindow(IDetailViewModel detailsViewModel)
		{
			switch (detailsViewModel)
			{
				case Libraries.DbStudio.ViewModels.Solutions.Details.Files.ImageViewModel viewModel:
						AddTab(new ImageView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Files.FileViewModel viewModel:
						AddTab(new FileDetailsView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Files.Structured.BaseFileViewModel viewModel:
						AddTab(new DataTableFileView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Queries.ExecuteQueryViewModel viewModel:
						AddTab(new Views.Queries.ExecuteQueryView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Connections.ExecuteFilesViewModel viewModel:
						AddTab(new Views.Connections.ExecuteFilesView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.ExecuteEtlConsoleViewModel viewModel:
						AddTab(new Views.EtlProjects.ExecuteEtlConsoleView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.DataSources.DataSourceSqlViewModel viewModel:
						AddTab(new Views.Reporting.Details.DataSources.DataSourceSqlView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.DataSources.DataSourceTableViewModel viewModel:
						AddTab(new Views.Reporting.Details.DataSources.DataSourceTableView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Dimension.DimensionViewModel viewModel:
						AddTab(new Views.Reporting.Details.Dimensions.DimensionView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Queries.ReportViewModel viewModel:
						AddTab(new Views.Reporting.Queries.ReportView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Reporting.Reports.ReportViewModel viewModel:
						AddTab(new Views.Reporting.Details.Reports.ReportView(viewModel), viewModel);
					break;
			}
		}

		/// <summary>
		///		Añade una ficha al control
		/// </summary>
		private void AddTab(UserControl control, IDetailViewModel detailsViewModel)
		{
			dckManager.AddDocument(detailsViewModel.TabId, detailsViewModel.Header, control, detailsViewModel);
		}

		/// <summary>
		///		Modifica el TabId de un documento abierto
		/// </summary>
		public void UpdateTabId(string oldTabId, string newTabId, string newHeader)
		{
			dckManager.UpdateTabId(oldTabId, newTabId, newHeader);
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
			if (args.Document != null && args.Document.Tag != null && args.Document.Tag is IDetailViewModel detailViewModel && detailViewModel.IsUpdated)
			{
				Libraries.BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType result = MainController.SparkSolutionController.HostController.SystemController.ShowQuestionCancel
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
		///		Modifica la ventana de detalles seleccionada
		/// </summary>
		private void UpdateSelectedTab()
		{
			IDetailViewModel details = null;

				// Obtiene los detalles de la ficha seleccionada
				if (dckManager.ActiveDocument != null)
					details = dckManager.ActiveDocument.Tag as IDetailViewModel;
				// Cambia la ficha seleccionada en el ViewModel
				ViewModel.SelectedDetailsViewModel = details;
		}

		/// <summary>
		///		Obtiene la ventalla de detalles activa
		/// </summary>
		internal IDetailViewModel GetActiveDetails()
		{
			if (dckManager.ActiveDocument != null)
				return dckManager.ActiveDocument.Tag as IDetailViewModel;
			else
				return null;
		}

		/// <summary>
		///		Obtiene la lista de ventanas de detalles abiertas
		/// </summary>
		internal List<IDetailViewModel> GetOpenedDetails()
		{
			List<IDetailViewModel> detailViewModels = new List<IDetailViewModel>();

				// Obtiene la lista
				foreach (object view in dckManager.GetOpenedViews())
					if (view is IDetailViewModel viewModel)
						detailViewModels.Add(viewModel);
				// Devuelve la lista
				return detailViewModels;
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
			MainController.ConfigurationController.LastThemeSelected = (int) newTheme;
		}

		/// <summary>
		///		Abre la ventana de configuración
		/// </summary>
		private void OpenConfigurationWindow()
		{
			MainController.SparkSolutionController.HostHelperController.ShowDialog(this, new Views.Tools.ConfigurationView());
		}

		/// <summary>
		///		Abre la ventana de búsqueda
		/// </summary>
		private void OpenSearchWindow()
		{
			if (dckManager.ActiveDocument?.UserControl is FileDetailsView fileView)
				fileView.OpenSearch(true);
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
				foreach (Libraries.DbStudio.ViewModels.Tools.Workspaces.WorkSpaceViewModel workspace in ViewModel.WorkspacesViewModel.Items)
				{
					MenuItem mnuNewWorkspace = CreateMenu(workspace.Name, string.Empty, false, null, workspace);

						// Inserta el menú tras el separador
						mnuWorkspace.Items.Insert(++startIndex, mnuNewWorkspace);
						// Añade el manejador
						mnuNewWorkspace.Click += (sender, args) => {
																		Libraries.DbStudio.ViewModels.Tools.Workspaces.WorkSpaceViewModel workSpace = (sender as MenuItem).Tag as Libraries.DbStudio.ViewModels.Tools.Workspaces.WorkSpaceViewModel;

																			// Selecciona el espacio de trabajo
																			if (workSpace != null)
																				ViewModel.WorkspacesViewModel.Select(workSpace.Name);
																			// Cambia las marcas de check de los menús
																			CheckWorkSpaceMenu();
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
					if (mnuWorkspace.Items[index] is MenuItem child && child.Tag is Libraries.DbStudio.ViewModels.Tools.Workspaces.WorkSpaceViewModel workSpace)
						child.IsChecked = workSpace.Name.Equals(ViewModel.WorkspacesViewModel.SelectedItem.Name);
		}

		/// <summary>
		///		Crea una opción de menú
		/// </summary>
		private MenuItem CreateMenu(string text, string icon, bool isCheckable, System.Windows.Input.ICommand command, object tag = null)
		{
			MenuItem mnuNewItem = new MenuItem();

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
			// Comprueba si alguna de las vistas tiene modificaciones pendientes
			foreach (object view in dckManager.GetOpenedViews())
				if (view is IDetailViewModel viewModel && viewModel.IsUpdated)
				{
					// Mensaje para el usuario
					ViewModel.MainController.HostController.SystemController.ShowMessage("Grabe las últimas modificaciones antes de cerrar la aplicación");
					// Indica que no puede salir de la aplicación
					return false;
				}
			// Si ha llegado hasta aquí, se puede cerrar
			return true;
		}

		/// <summary>
		///		Sale de la aplicación
		/// </summary>
		private void ExitApp()
		{
			// Graba la configuración
			if (!string.IsNullOrWhiteSpace(ViewModel.LastPathSelected))
				MainController.ConfigurationController.LastPathSelected = ViewModel.LastPathSelected;
			MainController.ConfigurationController.LastWorkSpace = ViewModel.WorkspacesViewModel.SelectedItem.Name;
			MainController.ConfigurationController.LastFiles = ViewModel.LastFilesViewModel.GetFiles();
			MainController.ConfigurationController.Save();
			// Cierra la aplicación
			Close();
		}

		/// <summary>
		///		Abre la ventana Acerca de
		/// </summary>
		private void OpenAboutWindow()
		{
			Views.Tools.AboutView view = new Views.Tools.AboutView(GetAssemblyVersion());

				// Muestra la ventana
				view.Owner = this;
				view.ShowDialog();
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public MainViewModel ViewModel { get; private set; }

		/// <summary>
		///		Controlador principal
		/// </summary>
		public static Controllers.AppController MainController { get; private set; }

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

		private void SearchCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			OpenSearchWindow();
		}

		private void ExecuteScriptCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			ViewModel.SolutionViewModel.ConnectionExecutionViewModel.ExecuteScripCommand.Execute(null);
		}

		private void ExecuteScriptCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.SolutionViewModel.ConnectionExecutionViewModel.ExecuteScripCommand.CanExecute(null);
		}

		private void NewQueryCommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
		{
			ViewModel.SolutionViewModel.TreeConnectionsViewModel.NewQueryCommand.Execute(null);
		}

		private void NewQueryCommandBinding_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = ViewModel.SolutionViewModel.TreeConnectionsViewModel.NewQueryCommand.CanExecute(null);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = !CanExitApp();
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

		private void OpenWebBrowser_Click(object sender, RoutedEventArgs e)
		{
			dckManager.AddDocument("192830489", "Web", new Views.Web.WebExplorerView(null), null);
		}
	}
}
