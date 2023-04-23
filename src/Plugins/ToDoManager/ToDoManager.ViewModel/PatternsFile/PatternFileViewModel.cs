using System;
using Microsoft.Extensions.Logging;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.ToDoManager.ViewModel.PatternsFile;

/// <summary>
///		ViewModel para ver el contenido de un archivo de tareas
/// </summary>
public class PatternFileViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
{
	// Variables privadas
	private string _fileName, _source, _separator, _quoteChar, _formula;
	private bool _withHeader;

	public PatternFileViewModel(ToDoManagerViewModel mainViewModel, string fileName)
	{ 
		// Asigna las propiedades
		MainViewModel = mainViewModel;
		FileName = fileName;
	}

	/// <summary>
	///		Interpreta el archivo
	/// </summary>
	public bool LoadFile()
	{
		bool loaded = false;

			// Intenta cargar el archivo
			try
			{
				LibPatternText.Models.PatternModel pattern = new LibPatternText.Application.PatternTextApplication().Load(FileName);

					// Carga los datos
					Source = pattern.Source;
					WithHeader = pattern.WithHeader;
					Separator = pattern.Separator;
					QuoteChar = pattern.QuoteChar;
					Formula = pattern.Formula;
					// Indica que no ha habido cambios
					IsUpdated = false;
					// Indica que se ha cargado el archivo
					loaded = true;
			}
			catch (Exception exception)
			{
				MainViewModel.ViewsController.Logger.LogError(exception, $"Error when load {FileName}. {exception.Message}");
				MainViewModel.ViewsController.SystemController.ShowMessage($"Error when load {FileName}. {exception.Message}");
			}
			// Devuelve el valor que indica si ha podido cargar el archivo
			return loaded;
	}

	/// <summary>
	///		Obtiene el mensaje para grabar y cerrar
	/// </summary>
	public string GetSaveAndCloseMessage()
	{
		if (string.IsNullOrWhiteSpace(FileName))
			return "¿Desea grabar el archivo antes de continuar?";
		else
			return $"¿Desea grabar el archivo '{System.IO.Path.GetFileName(FileName)}' antes de continuar?";
	}

	/// <summary>
	///		Graba el archivo
	/// </summary>
	public void SaveDetails(bool newName)
	{
		// Graba el archivo
		if (string.IsNullOrWhiteSpace(FileName) || newName)
		{
			string newFileName = MainViewModel.ViewsController.DialogsController.OpenDialogSave
									(string.Empty, 
									 $"Archivo patrones (*{ToDoManagerViewModel.PatternFileExtension})|*{ToDoManagerViewModel.PatternFileExtension}|Todos los archivos (*.*)|*.*",
									 FileName, ToDoManagerViewModel.PatternFileExtension);

				// Cambia el nombre de archivo si es necesario
				if (!string.IsNullOrWhiteSpace(newFileName))
					FileName = newFileName;
		}
		// Graba el archivo
		if (!string.IsNullOrWhiteSpace(FileName))
		{
			// Graba el archivo
			new LibPatternText.Application.PatternTextApplication().Save(FileName, GetPattern());
			// Actualiza el árbol
			MainViewModel.ViewsController.HostPluginsController.RefreshFiles();
			// Añade el archivo a los últimos archivos abiertos
			MainViewModel.ViewsController.HostPluginsController.AddFileUsed(FileName);
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}
	}

	/// <summary>
	///		Obtiene los datos del patrón
	/// </summary>
	private LibPatternText.Models.PatternModel GetPattern()
	{
		return new LibPatternText.Models.PatternModel()
							{
								Source = Source,
								WithHeader = WithHeader,
								Separator = Separator,
								QuoteChar = QuoteChar,
								Formula = Formula
							};
	}

	/// <summary>
	///		Cierra la ventana de detalles
	/// </summary>
	public void Close()
	{
		// ... no hace nada, sólo implementa la interface
	}

	/// <summary>
	///		Ejecuta el patrón
	/// </summary>
	public string Execute()
	{
		// Ejecuta la fórmula
		try
		{
			return new LibPatternText.PatternManager().Convert(GetPattern());
		}
		catch (Exception exception)
		{
			MainViewModel.ViewsController.Logger.LogError(exception, $"Error when execute {FileName}. {exception.Message}");
			MainViewModel.ViewsController.SystemController.ShowMessage($"Error when execute {FileName}. {exception.Message}");
		}
		// Devuelve el resultado
		return string.Empty;
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ToDoManagerViewModel MainViewModel { get; }

	/// <summary>
	///		Cabecera
	/// </summary>
	public string Header { get; private set; }

	/// <summary>
	///		Id de la ficha en pantalla
	/// </summary>
	public string TabId => GetType().ToString() + "_" + FileName;

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set 
		{ 
			if (CheckProperty(ref _fileName, value))
			{
				if (string.IsNullOrWhiteSpace(_fileName))
					Header = "New filename";
				else
					Header = System.IO.Path.GetFileName(_fileName);
			}
		}
	}

	/// <summary>
	///		Origen de los datos
	/// </summary>
	public string Source
	{
		get { return _source; }
		set { CheckProperty(ref _source, value); }
	}

	/// <summary>
	///		Indica si el origen de los datos tiene cabecera
	/// </summary>
	public bool WithHeader
	{
		get { return _withHeader; }
		set { CheckProperty(ref _withHeader, value); }
	}

	/// <summary>
	///		Separador de columnas
	/// </summary>
	public string Separator
	{
		get { return _separator; }
		set { CheckProperty(ref _separator, value); }
	}

	/// <summary>
	///		Carácter utilizado como comillas
	/// </summary>
	public string QuoteChar
	{
		get { return _quoteChar; }
		set { CheckProperty(ref _quoteChar, value); }
	}

	/// <summary>
	///		Fórmula
	/// </summary>
	public string Formula
	{
		get { return _formula; }
		set { CheckProperty(ref _formula, value); }
	}
}
