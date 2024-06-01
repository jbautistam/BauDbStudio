namespace Bau.Libraries.ToDoManager.ViewModel.TimeManagement;

/// <summary>
///		ViewModel para el filtro de proyectos
/// </summary>
public class CsvFilterViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{
	// Variables privadas
	private string _fileName = default!;
	private DateTime _start = DateTime.Now.AddYears(-1), _end = DateTime.Now;

	public CsvFilterViewModel(TimeScheduleViewModel viewModel)
	{
		TimeScheduleViewModel = viewModel;
	}

	/// <summary>
	///		Acepta los datos del filtro
	/// </summary>
	protected override void Save()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			TimeScheduleViewModel.MainViewModel.ViewsController.SystemController.ShowMessage("Enter the file name");
		else
			RaiseEventClose(true);
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public TimeScheduleViewModel TimeScheduleViewModel { get; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Fecha de inicio del filtro
	/// </summary>
	public DateTime Start
	{
		get { return _start; }
		set { CheckProperty(ref _start, value); }
	}

	/// <summary>
	///		Fecha de fin del filtro
	/// </summary>
	public DateTime End
	{
		get { return _end; }
		set { CheckProperty(ref _end, value); }
	}
}
