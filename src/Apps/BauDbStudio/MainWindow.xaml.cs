using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.DbStudio.ViewModels;
using Bau.Libraries.DbStudio.ViewModels.Solutions.Details;
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
			ViewModel.SolutionViewModel.Load();
			ViewModel.LastPathSelected = MainController.ConfigurationController.LastPathSelected;
			ViewModel.LastFilesViewModel.Add(MainController.ConfigurationController.LastFiles);
			// Añade los manejadores de eventos
			ViewModel.WorkspacesChanged += (sender, args) => ShowMenuWorkspaces();
			// Añade los paneles
			dckManager.AddPane("TreeFilesExplorer", "Archivos", new Views.TreeFilesExplorer(ViewModel.SolutionViewModel.TreeFoldersViewModel), 
							   null, Controls.DockLayout.DockPosition.Left);
			dckManager.AddPane("TreeConnectionsExplorer", "Conexiones", new Views.TreeConnectionsExplorer(ViewModel.SolutionViewModel.TreeConnectionsViewModel), 
							   null, Controls.DockLayout.DockPosition.Left);
			dckManager.AddPane("TreeStorageExplorer", "Storage", new Views.TreeStoragesExplorer(ViewModel.SolutionViewModel.TreeStoragesViewModel), 
							   null, Controls.DockLayout.DockPosition.Right);
			dckManager.AddPane("LogView", "Log", new Views.Tools.LogView(ViewModel.LogViewModel), null, Controls.DockLayout.DockPosition.Bottom);
			// Abre los paneles predefinidos
			dckManager.OpenGroup(Controls.DockLayout.DockPosition.Left);
			// Asigna los manejadores de eventos
			MainController.SparkSolutionController.OpenWindowRequired += (sender, args) => OpenWindow(args);
			// Asigna los manejadores de eventos del docker de documentos
			dckManager.Closing += (sender, args) => CloseWindow(args);
			dckManager.ActiveDocumentChanged += (sender, args) => UpdateSelectedTab();
			// Cambia el tema
			SetTheme((Controls.DockLayout.DockTheme) MainController.ConfigurationController.LastThemeSelected);
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
		///		Abre la ventana de detalles
		/// </summary>
		private void OpenWindow(IDetailViewModel detailsViewModel)
		{
			switch (detailsViewModel)
			{
				case Libraries.DbStudio.ViewModels.Solutions.Details.Files.FileViewModel viewModel:
						AddTab(new FileDetailsView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Files.BaseFileViewModel viewModel:
						AddTab(new DataTableFileView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Connections.ExecuteQueryViewModel viewModel:
						AddTab(new Views.Connections.ExecuteQueryView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.Connections.ExecuteFilesViewModel viewModel:
						AddTab(new Views.Connections.ExecuteFilesView(viewModel), viewModel);
					break;
				case Libraries.DbStudio.ViewModels.Solutions.Details.EtlProjects.ExecuteEtlConsoleViewModel viewModel:
						AddTab(new Views.EtlProjects.ExecuteEtlConsoleView(viewModel), viewModel);
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
		private void CloseWindow(Controls.ClosingEventArgs args)
		{
			if (args.Document != null && args.Document.Tag != null && args.Document.Tag is IDetailViewModel detailViewModel && detailViewModel.IsUpdated)
			{
				Libraries.BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType result = MainController.SparkSolutionController.HostController.SystemController.ShowQuestionCancel
																											("¿Desea grabar los datos antes de cerrar la ventana?");
				
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
		private void SetTheme(Controls.DockLayout.DockTheme newTheme)
		{
			// Cambia el tema
			dckManager.SetTheme(newTheme);
			// Cambia los menús
			mnuThemeAero.IsChecked = newTheme == Controls.DockLayout.DockTheme.Aero;
			mnuThemeMetro.IsChecked = newTheme == Controls.DockLayout.DockTheme.Metro;
			mnuThemeVs2010.IsChecked = newTheme == Controls.DockLayout.DockTheme.VS2010Theme;
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
			// Crea el viewModel de las ventanas de búsqueda si no existía
			if (FindViewModel == null)
				FindViewModel = new Views.Tools.FindViewModel(this);
			// Muestra el cuadro de búsqueda
			if (!FindViewModel.IsOpened && dckManager.ActiveDocument?.UserControl is FileDetailsView fileView)
			{
				Views.Tools.FindView view = new Views.Tools.FindView(FindViewModel);

					// Muestra el formulario activo
					view.Owner = this;
					view.ShowActivated = true;
					view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
					view.WindowStyle = WindowStyle.ToolWindow;
					view.ShowInTaskbar = false;
					view.ResizeMode = ResizeMode.NoResize;
					view.Top = Top;
					view.Left = Left + Width - view.Height;
					// view.IsActive = true;
					// Muestra la venta
					view.Show();
					// Inicializa el viewModel
					FindViewModel.Open(fileView);
			}
		}

		/// <summary>
		///		Muestra el menú de espacios de trabajo
		/// </summary>
		private void ShowMenuWorkspaces()
		{
			List<string> workspaces = GetWorkspaces(ViewModel.MainController.AppPath);
			int startIndex = mnuWorkspace.Items.IndexOf(mnuStartWorkspaces);
			int indexEnd = mnuWorkspace.Items.IndexOf(mnuEndWorkspaces);

				// Borra las opciones de menú que se hubiesen creado anteriormente
				DeleteMenusBetween(mnuWorkspace, startIndex, indexEnd);
				// Muestra los menús
				foreach (string workspace in workspaces)
				{
					MenuItem mnuNewWorkspace = CreateMenu(System.IO.Path.GetFileNameWithoutExtension(workspace), string.Empty, false,
													      null, workspace);

						// Inserta el menú tras el separador
						mnuWorkspace.Items.Insert(++startIndex, mnuNewWorkspace);
						// Añade el manejador
						mnuNewWorkspace.Click += (sender, args) => {
																		string file = (sender as MenuItem).Tag as string;

																			// Selecciona el espacio de trabajo
																			if (!string.IsNullOrWhiteSpace(file))
																				ViewModel.SolutionViewModel.UpdateWorkspace(System.IO.Path.GetFileNameWithoutExtension(file));
																			// Cambia las marcas de check de los menús
																			CheckWorkSpaceMenu();
																   };
						mnuNewWorkspace.Checked += (sender, args) => CheckWorkSpaceMenu();
				}
				// Muestra el separador
				if (workspaces.Count == 0)
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
					if (mnuWorkspace.Items[index] is MenuItem child && child.Tag is string file)
						child.IsChecked = file.EndsWith(ViewModel.SolutionViewModel.Workspace + ".xml", StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		///		Obtiene la lista de espacios de trabajo
		/// </summary>
		private List<string> GetWorkspaces(string path)
		{
			List<string> workspaces = new List<string>();

				// Obtiene los espacios de trabajo
				if (System.IO.Directory.Exists(path))
					foreach (string fileName in System.IO.Directory.GetFiles(path, "*.xml"))
						workspaces.Add(fileName);
				// Devuelve la lista de espacios de trabajo
				return workspaces;
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
			List<object> views = dckManager.GetOpenedViews();

				// Comprueba si alguna de las vistas tiene modificaciones pendientes
				foreach (object view in views)
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
			MainController.ConfigurationController.LastWorkSpace = ViewModel.SolutionViewModel.Workspace;
			MainController.ConfigurationController.LastFiles = ViewModel.LastFilesViewModel.GetFiles();
			MainController.ConfigurationController.Save();
			// Cierra la aplicación
			Close();
		}

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public MainViewModel ViewModel { get; private set; }

		/// <summary>
		///		Controlador principal
		/// </summary>
		public static Controllers.AppController MainController { get; private set; }

		/// <summary>
		///		ViewModel del cuadro de diálogo de búsqueda
		/// </summary>
		private Views.Tools.FindViewModel FindViewModel { get; set; }

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
			SetTheme(Controls.DockLayout.DockTheme.Aero);
		}

		private void ThemeMetro_Click(object sender, RoutedEventArgs e)
		{
			SetTheme(Controls.DockLayout.DockTheme.Metro);
		}

		private void ThemeVS2010_Click(object sender, RoutedEventArgs e)
		{
			SetTheme(Controls.DockLayout.DockTheme.VS2010Theme);
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
	}
}
