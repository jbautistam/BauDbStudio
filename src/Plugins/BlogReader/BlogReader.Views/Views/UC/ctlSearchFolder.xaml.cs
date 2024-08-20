using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeFolders;
using Bau.Libraries.LibBlogReader.Model;
using Bau.Libraries.LibBlogReader.ViewModel.Blogs.TreeBlogs;

namespace Bau.Libraries.BlogReader.Views.Views.UC;

/// <summary>
///		Control para el árbol de carpetas
/// </summary>
public partial class ctlSearchFolder : UserControl
{ 
	// Propiedades de dependencia
	public static readonly DependencyProperty FolderProperty = DependencyProperty.Register(nameof(Folder), typeof(FolderModel), typeof(ctlSearchFolder),
																						   new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
																														 new PropertyChangedCallback(Folder_PropertyChanged)));

	public ctlSearchFolder()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa el control
	/// </summary>
	public void InitControl(LibBlogReader.ViewModel.BlogReaderViewModel mainViewModel)
	{
		MainViewModel = mainViewModel;
	}

	/// <summary>
	///		Abre el árbol de documentos
	/// </summary>
	private void OpenTreeDocuments()
	{ 
		if (MainViewModel is not null)
		{
			TreeFoldersViewModel tree = new(MainViewModel, null);

				// Carga los nodos
				tree.Load();
				// Asigna el dataContext
				wndPopUp.DataContext = tree;
				// Abre la ventana
				wndPopUp.IsOpen = true;
		}
	}

	/// <summary>
	///		Selecciona la carpeta
	/// </summary>
	private void SelectFolder(FolderNodeViewModel node)
	{ 
		// Asigna la carpeta
		Folder = node?.Folder;
		// Cierra la ventana
		wndPopUp.IsOpen = false;
	}

	private static void Folder_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		if (obj is ctlSearchFolder searchFolder)
		{
			FolderModel? folder = null;

				// Obtiene el valor del evento (evitando los nulos)
				if (e.NewValue != null)
					folder = e.NewValue as FolderModel;
				// Asigna el texto
				if (folder != null)
					searchFolder.txtPage.Text = folder.FullName;
				else
					searchFolder.txtPage.Text = "";
		}
	}

	private void cmdSearchPage_Click(object sender, RoutedEventArgs e)
	{
		OpenTreeDocuments();
	}

	private void cmdRemovePage_Click(object sender, RoutedEventArgs e)
	{
		Folder = null;
	}

	private void trvPages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		if (trvPages.SelectedItem is FolderNodeViewModel folder)
			SelectFolder(folder);
	}

	/// <summary>
	///		ViewModel de la ventana principal
	/// </summary>
	public LibBlogReader.ViewModel.BlogReaderViewModel? MainViewModel { get; private set; }

	/// <summary>
	///		Carpeta seleccionada
	/// </summary>
	public FolderModel? Folder
	{
		get { return (FolderModel) GetValue(FolderProperty); }
		set { SetValue(FolderProperty, value); }
	}
}