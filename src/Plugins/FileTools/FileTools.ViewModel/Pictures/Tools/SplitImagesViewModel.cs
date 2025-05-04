namespace Bau.Libraries.FileTools.ViewModel.Pictures.Tools;

/// <summary>
///		ViewModel del cuadro de diálogo para la introducción de los datos para dividir una imagen en varias
/// </summary>
public class SplitImagesViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
{	
	// Variables privadas
	private string _outputFolder = default!, _targetFileName = default!;
	private int _rows, _columns;

	public SplitImagesViewModel(ImageEditViewModel imageEditViewModel)
	{
		ImageEditViewModel = imageEditViewModel;
		InitProperties();
	}

	/// <summary>
	///		Inicializa las propiedades
	/// </summary>
	private void InitProperties()
	{
		if (string.IsNullOrWhiteSpace(ImageEditViewModel.FileName))
		{
			OutputFolder = ImageEditViewModel.MainViewModel.MainController.MainWindowController.DialogsController.LastPathSelected;
			TargetFileName = "output.png";
		}
		else
		{
			OutputFolder = Path.GetDirectoryName(ImageEditViewModel.FileName)!;
			TargetFileName = $"_{Path.GetFileName(ImageEditViewModel.FileName)}";
		}
		Rows = 5;
		Columns = 5;
	}

	/// <summary>
	///		Comprueba que los datos introducidos sean correctos
	/// </summary>
	private bool ValidateData()
	{
		bool validate = false;

			// Comprueba los datos introducidos
			if (string.IsNullOrWhiteSpace(OutputFolder))
				ImageEditViewModel.MainViewModel.MainController.SystemController.ShowMessage("Enter the output folder");
			else if (string.IsNullOrWhiteSpace(TargetFileName))
				ImageEditViewModel.MainViewModel.MainController.SystemController.ShowMessage("Enter the target file name");
			else if (Rows < 1 || Columns < 1)
				ImageEditViewModel.MainViewModel.MainController.SystemController.ShowMessage("Enter the rows and columns");
			else
				validate = true;
			// Devuelve el valor que indica si los datos son correctos
			return validate;
	}

	/// <summary>
	///		Graba los datos
	/// </summary>
	protected override void Save()
	{
		if (ValidateData())
		{ 
			// Ejecuta la división de archivos
			ImageEditViewModel.MainViewModel.MainController.ImageToolsController.Split(ImageEditViewModel.FileName, OutputFolder, TargetFileName,
																					   Rows, Columns);
			// Cierra el formulario
			RaiseEventClose(true);
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ImageEditViewModel ImageEditViewModel { get; }

	/// <summary>
	///		Directorio de salida
	/// </summary>
	public string OutputFolder
	{
		get { return _outputFolder; }
		set { CheckProperty(ref _outputFolder, value); }
	}

	/// <summary>
	///		Nombre del archivo de salida
	/// </summary>
	public string TargetFileName
	{
		get { return _targetFileName; }
		set { CheckProperty(ref _targetFileName, value); }
	}

	/// <summary>
	///		Número de filas de la imagen de entrada
	/// </summary>
	public int Rows
	{
		get { return _rows; }
		set { CheckProperty(ref _rows, value); }
	}

	/// <summary>
	///		Número de columnas de la imagen de entrada
	/// </summary>
	public int Columns
	{
		get { return _columns; }
		set { CheckProperty(ref _columns, value); }
	}
}
