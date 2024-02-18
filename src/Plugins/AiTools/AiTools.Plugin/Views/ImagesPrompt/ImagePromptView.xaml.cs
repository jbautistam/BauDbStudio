using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Bau.Libraries.AiTools.ViewModels.Prompts.Explorers;

namespace Bau.Libraries.AiTools.Plugin.Views.ImagesPrompt;

/// <summary>
///		Ventana principal
/// </summary>
public partial class ImagePromptView : UserControl
{
	// Variables privadas
	private bool _isViewModelLoaded;

	public ImagePromptView(ViewModels.Prompts.PromptFileViewModel viewModel)
	{
		InitializeComponent();
		DataContext = ViewModel = viewModel;
	}

	/// <summary>
	///		Inicializa la ventana
	/// </summary>
	private async Task InitWindowAsync(CancellationToken cancellationToken)
	{
		if (!_isViewModelLoaded)
		{
			// Inicializa los manejadores de eventos
			ViewModel.CopiedText += (sender, args) => CopyCategoryText(args);
			// Inicializa el viewModel
			await ViewModel.LoadAsync(GetFileNameCategories(), cancellationToken);
			// Indica que se ha cargado (para que no vuelva a cargarlo cuando gane el foco)
			_isViewModelLoaded = true;
		}
	}

	/// <summary>
	///		Obtiene el nombre de archivo de categorías
	/// </summary>
	private string GetFileNameCategories()
	{
		string folder = System.IO.Path.GetDirectoryName(Environment.ProcessPath) ?? string.Empty;

			return System.IO.Path.Combine(folder, "data/Prompts.xml");
	}

	/// <summary>
	///		Abre la imagen
	/// </summary>
	private void OpenImage()
	{
		if (ViewModel.VersionsViewModel.SelectedItem is not null)
			ViewModel.VersionsViewModel.SelectedItem.ImagesViewModel.SelectedItem?.OpenImage();
	}

	/// <summary>
	///		Borra una image
	/// </summary>
	private void DeleteImage()
	{
		if (ViewModel.VersionsViewModel.SelectedItem is not null)
			ViewModel.VersionsViewModel.SelectedItem.ImagesViewModel.SelectedItem?.Delete();
	}

	/// <summary>
	///		Copia el texto de una categoría
	/// </summary>
	private void CopyCategoryText(string text)
	{
		if (!string.IsNullOrWhiteSpace(text))
		{
			// Añade un separador
			if (!string.IsNullOrWhiteSpace(txtResultPositive.Text) && !string.IsNullOrWhiteSpace(text))
				txtResultPositive.AppendText(", ");
			// Añade el texto
			txtResultPositive.AppendText(text);
		}
	}

	private void trvCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		if (trvCategories.DataContext is TreeCategoriesViewModel viewModel)
			viewModel.SelectedNode = (sender as TreeView)?.SelectedItem as CategoryNodeViewModel;
	}

	private void trvCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		if (trvCategories.DataContext is TreeCategoriesViewModel viewModel && (sender as TreeView)?.SelectedItem is CategoryNodeViewModel node)
		{
			viewModel.SelectedNode = node;
			viewModel.CopyThisCommand.Execute(null);
		}
	}

	private void trvCategories_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (trvCategories.DataContext is TreeCategoriesViewModel viewModel && e.ChangedButton == MouseButton.Left)
			viewModel.SelectedNode = null;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ViewModels.Prompts.PromptFileViewModel ViewModel { get; }
	
	private async void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		await InitWindowAsync(CancellationToken.None);
	}

	private void cmdCopyPositive_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(txtResultPositive.Text);
	}

	private void cmdCopyNegative_Click(object sender, RoutedEventArgs e)
	{
		Clipboard.SetText(txtResultNegative.Text);
	}

	private void cmdDeletePositive_Click(object sender, RoutedEventArgs e)
	{
		txtResultPositive.Text = string.Empty;
	}

	private void cmdDeleteNegative_Click(object sender, RoutedEventArgs e)
	{
		txtResultNegative.Text = string.Empty;
	}

	private void lstThumbs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		OpenImage();
	}

	private void mnuImages_Open_Click(object sender, RoutedEventArgs e)
	{
		OpenImage();
	}

	private void mnuImages_Delete_Click(object sender, RoutedEventArgs e)
	{
		DeleteImage();
	}
}