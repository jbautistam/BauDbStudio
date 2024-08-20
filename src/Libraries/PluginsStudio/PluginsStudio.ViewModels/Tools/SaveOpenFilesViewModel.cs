using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ListView;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Interfaces;

namespace Bau.Libraries.PluginsStudio.ViewModels.Tools;

/// <summary>
///		ViewModel para almacenar las ventanas que contienen archivos abiertos
/// </summary>
public class SaveOpenFilesViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private ControlGenericListViewModel<ControlItemViewModel> _listViews = default!;

	public SaveOpenFilesViewModel(PluginsStudioViewModel mainViewModel, List<IDetailViewModel> detailViewModels)
	{
		// Inicializa las propiedades
		MainViewModel = mainViewModel;
		DetailViewModels = detailViewModels;
		// Inicializa la lista
		InitControl();
		// Inicializa los comandos
		DiscardAllCommand = new BaseCommand(_ => Discard());
	}

	/// <summary>
	///		Inicializa el controls
	/// </summary>
	private void InitControl()
	{
		// Genera la lista
		ListViewItems = new ControlGenericListViewModel<ControlItemViewModel>();
		// Añade los controles
		foreach (IDetailViewModel detailViewModel in DetailViewModels)
			ListViewItems.Add(new ControlItemViewModel(detailViewModel.Header, detailViewModel)
										{
											IsChecked = true
										}
							 );
	}

	/// <summary>
	///		Descarta las grabaciones (simplemente indica que se cierre la ventana)
	/// </summary>
	private void Discard()
	{
		RaiseEventClose(true);
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	private bool ValidateData()
	{
		bool validated = false;

			// Comprueba si hay algún elemento seleccionado para grabar
			foreach (ControlItemViewModel item in ListViewItems.Items)
				if (item.IsChecked)
					validated = true;
			// Si no hay nada seleccionado, muestra el mensaje
			if (!validated)
				MainViewModel.MainController.MainWindowController.SystemController.ShowMessage("Seleccione al menos un archivo");
			// Devuelve el valor que indica si es correcto
			return validated;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{
			// Graba las ventanas
			foreach (ControlItemViewModel item in ListViewItems.Items)
				if (item.IsChecked && item.Tag is IDetailViewModel detailViewModel)
					detailViewModel.SaveDetails(false);
			// Lanza el evento de cierre
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public PluginsStudioViewModel MainViewModel { get; }

	/// <summary>
	///		ViewModels con datos para grabar
	/// </summary>
	public List<IDetailViewModel> DetailViewModels { get; }

	/// <summary>
	///		Tipos de archivo
	/// </summary>
	public ControlGenericListViewModel<ControlItemViewModel> ListViewItems
	{
		get { return _listViews; }
		set { CheckObject(ref _listViews, value); }
	}

	/// <summary>
	///		Comando para descartar todo
	/// </summary>
	public BaseCommand DiscardAllCommand { get; }
}
