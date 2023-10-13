using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ChessDataBase.Models.Games;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Pgn;

/// <summary>
///		ViewModel para un <see cref="LibraryModel"/>
/// </summary>
public class PgnLibraryViewModel : BaseObservableObject
{
	// Variables privadas
	private string _fileName = default!;
	private PgnGameInfoViewModel _selectedGame = default!;

	public PgnLibraryViewModel(Board.GameBoardViewModel gameBoardViewModel)
	{
		// Inicializa las propiedades
		GameBoardViewModel = gameBoardViewModel;
		Init();
		// Inicializa los comandos
		NextGameCommand = new BaseCommand(_ => GoGame(true), _ => CanGoGame(true))
								.AddListener(this, FileName);
		PreviousGameCommand = new BaseCommand(_ => GoGame(false), _ => CanGoGame(false))
								.AddListener(this, FileName);
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	public void Init()
	{
		ConvertToViewModel(new LibraryModel());
	}

	/// <summary>
	///		Carga un archivo
	/// </summary>
	public async Task<(bool loaded, string error)> LoadAsync(string fileName, CancellationToken cancellationToken)
	{
		string error = string.Empty;

			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Inicializa el juego
			Library = new LibraryModel();
			// Carga el juego
			if (!string.IsNullOrEmpty(fileName))
			{
				// Carga el juego
				try
				{
					// Carga el juego
					Library = LoadLibrary(fileName);
					// Asigna el nombre de archivo
					FullFileName = fileName;
					FileName = Path.GetFileName(fileName);
				}
				catch (Exception exception)
				{
					error = $"Error al cargar el juego: {exception.Message}";
				}
			}
			else
				error = $"No existe el archivo: {fileName}";
			// y lo muestra (lo hace fuera del try porque debe crear el modelo vacío)
			ConvertToViewModel(Library);
			// Devuelve el valor que indica si los datos son correctos
			return (string.IsNullOrEmpty(error), error);
	}

	/// <summary>
	///		Carga la librería de un archivo PGN
	/// </summary>
	private LibraryModel LoadLibrary(string fileName)
	{
		LibPgnReader.Models.LibraryPgnModel libraryPgn = new LibPgnReader.GamePgnParser().Read(fileName);

			// Convierte los datos del archivo PGN
			return new LibPgn.Conversor.GamePgnConversor().Convert(libraryPgn);
	}

	/// <summary>
	///		Convierte la librería en ViewModels
	/// </summary>
	private void ConvertToViewModel(LibraryModel library)
	{
		// Asigna el archivo
		Library = library;
		// Limpia la lista
		Games.Clear();
		// Carga los juegos
		foreach (GameModel game in library.Games)
			Games.Add(new PgnGameInfoViewModel(this, game));
		// Selecciona un elemento
		if (Games.Count > 0)
			SelectedGame = Games[0];
		else
			SelectedGame = new PgnGameInfoViewModel(this, new GameModel());
	}

	/// <summary>
	///		Modifica el juego seleccionado
	/// </summary>
	private void UpdateSelectedGame()
	{
		GameBoardViewModel.LoadMovements(SelectedGame?.Game);
	}

	/// <summary>
	///		Pasa a la partida siguiente o anterior
	/// </summary>
	private void GoGame(bool next)
	{
		if (next)
			SelectedGame = Games[SelectedGameIndex + 1];
		else
			SelectedGame = Games[SelectedGameIndex - 1];
	}

	/// <summary>
	///		Indica si puede ir a la partida siguiente o anterior
	/// </summary>
	private bool CanGoGame(bool next)
	{
		return SelectedGameIndex != -1 && ((next && SelectedGameIndex < Games.Count - 1) ||
										   (!next && SelectedGameIndex > 0));
	}

	/// <summary>
	///		ViewModel del tablero
	/// </summary>
	public Board.GameBoardViewModel GameBoardViewModel { get; }

	/// <summary>
	///		Datos del archivo
	/// </summary>
	public LibraryModel Library { get; private set; } = default!;

	/// <summary>
	///		Nombre del archivo
	/// </summary>
	public string FullFileName { get; private set; } = default!;

	/// <summary>
	///		Nombre del archivo abierto
	/// </summary>
	public string FileName
	{
		get { return _fileName; }
		set { CheckProperty(ref _fileName, value); }
	}

	/// <summary>
	///		Partidas
	/// </summary>
	public ObservableCollection<PgnGameInfoViewModel> Games { get; } = new();

	/// <summary>
	///		Partida seleccionada
	/// </summary>
	public PgnGameInfoViewModel SelectedGame
	{
		get { return _selectedGame; }
		set 
		{ 
			if (CheckObject(ref _selectedGame, value))
				UpdateSelectedGame();
		}
	}

	/// <summary>
	///		Indice de la partida seleccionada
	/// </summary>
	public int SelectedGameIndex
	{
		get
		{
			if (Games == null || Games.Count == 0 || SelectedGame == null)
				return -1;
			else
				return Games.IndexOf(SelectedGame);
		}
	}

	/// <summary>
	///		Comando para cargar la partida anterior
	/// </summary>
	public BaseCommand PreviousGameCommand { get; }

	/// <summary>
	///		Comando para cargar la siguiente partida
	/// </summary>
	public BaseCommand NextGameCommand { get; }
}