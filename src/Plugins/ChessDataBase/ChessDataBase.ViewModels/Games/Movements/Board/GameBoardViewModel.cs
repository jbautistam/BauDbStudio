using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Games;
using Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Actions;
using Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Movements;
using Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Scapes;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board;

/// <summary>
///		ViewModel del tablero de juego con la partida
/// </summary>
public class GameBoardViewModel : BaseObservableObject
{
	// Eventos públicos
	public event EventHandler? ResetGame;
	public event EventHandler<EventArguments.ShowMovementEventArgs>? ShowMovements;
	// Variables privadas
	private ScapesBoardViewModel _scapes = default!;
	private MovementListViewModel _movementsList = default!;
	private int _actualMovementIndex;
	private MovementFigureModel _actualMovement = default!;
	private bool _isAtVariation, _isMoving;

	public GameBoardViewModel(ChessGameBaseViewModel chessGameViewModel)
	{
		// Inicializa los objetos
		ChessGameMainViewModel = chessGameViewModel;
		Scapes = new ScapesBoardViewModel(this);
		MovementsList = new MovementListViewModel(this);
		// Inicializa los comandos
		NextMovementCommand = new BaseCommand(_ => GoNextMovement(), _ => CanGoMovement(true))
									.AddListener(this, nameof(ActualMovementIndex))
									.AddListener(this, nameof(IsMoving))
									.AddListener(this, nameof(IsAtVariation));
		PreviousMovementCommand = new BaseCommand(_ => GoPreviousMovement(), _ => CanGoMovement(false))
									.AddListener(this, nameof(ActualMovementIndex))
									.AddListener(this, nameof(IsMoving))
									.AddListener(this, nameof(IsAtVariation));
		VariationExitCommand = new BaseCommand(_ => ExitVariation(), _ => CanExitVariation())
									.AddListener(this, nameof(ActualMovementIndex))
									.AddListener(this, nameof(IsMoving))
									.AddListener(this, nameof(IsAtVariation));
	}

	/// <summary>
	///		Carga los movimientos de un juego
	/// </summary>
	internal void LoadMovements(GameModel? game)
	{
		// Guarda el juego
		if (game is null)
			Game = new GameModel();
		else
			Game = game;
		// Inicializa el tablero
		Reset();
		// Carga las listas sobre las que se realizan los movimientos
		VariationGame = Game.Variation.Clone(null);
		ActualVariationGame = Game.Variation.Clone(null);
		// Carga el listView de movimientos
		LoadListViewMovements(ActualVariationGame.Movements);
	}

	/// <summary>
	///		Carga el listView con los movimientos de la variación del juego
	/// </summary>
	private void LoadListViewMovements(MovementModelCollection movements)
	{
		List<BaseMovementViewModel> movementViewModels = new List<BaseMovementViewModel>();

			// Crea los movimientos
			foreach (MovementBaseModel baseMovement in movements)
				switch (baseMovement)
				{
					case MovementFigureModel movement:
							movementViewModels.Add(new MovementFigureViewModel(MovementsList, movement));
						break;
					case MovementGameEndModel movement:
							movementViewModels.Add(new MovementGameEndViewModel(movement));
						break;
				}
			// Muestra los movimientos en la lista
			MovementsList.LoadMovements(movementViewModels);
	}

	/// <summary>
	///		Reinicia el tablero
	/// </summary>
	public void Reset()
	{
		// Limpia el tablero
		if (Game?.Board != null)
			Board.Reset(Game.Board);
		else 
			Board.Reset();
		// Inicializa los escaques
		Scapes.Reset();
		// Indica que estamos de nuevo en el primer movimiento
		MovementsList.SelectedMovement = null;
		MovementsList.ActualMovement = null;
		ActualMovementIndex = -1;
		// Lanza el evento de reset
		RaiseEventReset();
	}

	/// <summary>
	///		Realiza el siguiente movimiento
	/// </summary>
	private void GoNextMovement()
	{
		MovementFigureModel? movement = GetNextMovement();

			// Realiza el movimiento
			if (movement != null)
			{
				// Ejecuta el movimiento
				RaiseEventShowMovements(MakeMovement(movement), false);
				// Lo muestra en la lista
				MovementsList.SelectMovement(movement);
			}
	}

	/// <summary>
	///		Realiza el movimiento anterior
	/// </summary>
	private void GoPreviousMovement()
	{
		// Deshace el movimiento
		UndoMovement(ActualMovement, true);
		// Pasa al movimiento anterior
		ActualMovement = GetPreviousMovement();
		MovementsList.SelectMovement(ActualMovement);
	}

	/// <summary>
	///		Obtiene el siguiente movimiento
	/// </summary>
	private MovementFigureModel? GetNextMovement()
	{
		// Busca el siguiente movimiento de tipo figura (se salta los comentarios si había alguno)
		while (++ActualMovementIndex < ActualVariationGame.Movements.Count)
		{
			MovementBaseModel movement = ActualVariationGame.Movements[ActualMovementIndex];

				if (movement is MovementFigureModel movementFigure)
					return movementFigure;
		}
		// No ha encontrado ningún movimiento
		return null;
	}

	/// <summary>
	///		Obtiene el movimiento anterior
	/// </summary>
	private MovementFigureModel? GetPreviousMovement()
	{
		// Busca el siguiente movimiento de tipo figura (se salta los comentarios si había alguno)
		while (--ActualMovementIndex > -1)
		{
			MovementBaseModel movement = ActualVariationGame.Movements[ActualMovementIndex];

				if (movement is MovementFigureModel movementFigure)
					return movementFigure;
		}
		// No ha encontrado ningún movimiento anterior
		return null;
	}

	/// <summary>
	///		Coloca la partida en un movimiento
	/// </summary>
	internal void GoToMovement(MovementFigureViewModel movementViewModel)
	{
		bool end = false;
		List<ActionBaseModel> actions = new List<ActionBaseModel>();

			// Limpia el tablero
			Reset();
			// Realiza los movimientos
			while (!end)
			{
				MovementFigureModel? movement = GetNextMovement();

					if (movement is null)
						end = true;
					else
					{
						// Acumula las acciones
						actions.AddRange(MakeMovement(movement));
						// Si estamos en el movimiento buscado, terminamos
						if (movementViewModel.Movement == ActualMovement || ActualMovementIndex >= ActualVariationGame.Movements.Count)
						{
							// Selecciona el movimiento en la lista
							MovementsList.SelectMovement(ActualMovement);
							// Indica que ha terminado
							end = true;
						}
					}
			}
			// Ejecuta las acciones
			RaiseEventShowMovements(actions, true);
	}

	/// <summary>
	///		Realiza un movimiento
	/// </summary>
	private List<ActionBaseModel> MakeMovement(MovementFigureModel movement)
	{
		List<ActionBaseModel> actions = new ActionFactory().Create(movement);

			// Guarda el movimiento actual
			ActualMovement = movement;
			// Ejecuta el movimiento sobre el tablero
			Board.Move(movement);
			// Devuelve las acciones
			return actions;
	}

	/// <summary>
	///		Deshace un movimiento
	/// </summary>
	private void UndoMovement(MovementFigureModel movement, bool raiseActions)
	{
		List<ActionBaseModel> actions = new ActionFactory().CreateUndo(movement);

			// Guarda el movimiento actual
			IsMoving = true;
			// Lanza las acciones
			if (raiseActions)
				RaiseEventShowMovements(actions, false);
			// Ejecuta el movimiento sobre el tablero
			Board.Undo(movement);
	}

	/// <summary>
	///		Comprueba si puede ir al movimiento siguiente o anterior
	/// </summary>
	private bool CanGoMovement(bool next)
	{
		if (IsMoving)
			return false;
		else if (next)
			return ActualMovementIndex < ActualVariationGame.Movements.Count;
		else
			return ActualMovementIndex > -1;
	}

	/// <summary>
	///		Crea los movimientos hasta una variación
	/// </summary>
	internal void GoToVariation(MovementVariationModel variation, MovementFigureModel movement)
	{
		if (variation != null && movement != null)
		{
			// Cambia la variación actual y guarda el movimiento de la variación
			ActualVariationGame = VariationGame.CloneTo(null, variation, movement);
			VariationMovement = variation.Parent as MovementFigureModel;
			// y carga la lista
			LoadListViewMovements(ActualVariationGame.Movements);
			// Selecciona el movimiento en la lista y mueve
			MovementsList.SelectMovement(movement);
			GoToMovement(MovementsList.ActualMovement);
			// Indica que está en una variación
			IsAtVariation = true;
		}
	}

	/// <summary>
	///		Indica si puede salir de la variación
	/// </summary>
	private bool CanExitVariation() => IsAtVariation;

	/// <summary>
	///		Sale de la variación
	/// </summary>
	private void ExitVariation()
	{
		if (IsAtVariation)
		{
			// Cambia la variación actual
			ActualVariationGame = VariationGame.Clone(null);
			// y carga la lista
			LoadListViewMovements(ActualVariationGame.Movements);
			// Selecciona el movimiento de la variación y mueve
			MovementsList.SelectMovement(VariationMovement);
			GoToMovement(MovementsList.ActualMovement);
			VariationMovement = null;
			// Indica que ya no está en una variación
			IsAtVariation = false;
		}
	}

	/// <summary>
	///		lanza el evento de reset
	/// </summary>
	private void RaiseEventReset()
	{
		ResetGame?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///		Lanza el evento con las acciones de movimiento
	/// </summary>
	private void RaiseEventShowMovements(List<ActionBaseModel> actions, bool disableAnimations)
	{
		// Indica que está movimiento
		IsMoving = true;
		// Lanza el evento
		ShowMovements?.Invoke(this, new EventArguments.ShowMovementEventArgs(actions, disableAnimations));
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public ChessGameBaseViewModel ChessGameMainViewModel { get; }

	/// <summary>
	///		Juego cargado
	/// </summary>
	private GameModel Game { get; set; } = default!;

	/// <summary>
	///		Escaques
	/// </summary>
	public ScapesBoardViewModel Scapes
	{
		get { return _scapes; }
		set { CheckObject(ref _scapes, value); }
	}

	/// <summary>
	///		Tablero de juego
	/// </summary>
	public Models.Board.SquareModel Board { get; } = new Models.Board.SquareModel();

	/// <summary>
	///		Lista de movimientos del juego (inicial, sin tener en cuenta la variación actual que se está mostrando)
	/// </summary>
	private MovementVariationModel VariationGame { get; set; } = default!;

	/// <summary>
	///		Lista de movimientos de la variación actual (con la que se hacen los movimientos)
	/// </summary>
	private MovementVariationModel ActualVariationGame { get; set; } = default!;

	/// <summary>
	///		Movimiento a partir del que se hizo la variación anterior
	/// </summary>
	private MovementFigureModel? VariationMovement { get; set; }

	/// <summary>
	///		Movimiento actual
	/// </summary>
	private MovementFigureModel ActualMovement 
	{ 
		get { return _actualMovement; }
		set { CheckObject(ref _actualMovement, value); }
	}

	/// <summary>
	///		Indice del movimiento actual
	/// </summary>
	public int ActualMovementIndex
	{
		get { return _actualMovementIndex; }
		set { CheckObject(ref _actualMovementIndex, value); }
	}

	/// <summary>
	///		Indica si está realizando un movimiento
	/// </summary>
	public bool IsMoving
	{
		get { return _isMoving; }
		set { CheckProperty(ref _isMoving, value); }
	}

	/// <summary>
	///		Indica si está dentro de una variación
	/// </summary>
	public bool IsAtVariation
	{
		get { return _isAtVariation; }
		set { CheckProperty(ref _isAtVariation, value); }
	}

	/// <summary>
	///		Lista de movimientos del juego actual
	/// </summary>
	public MovementListViewModel MovementsList
	{
		get { return _movementsList; }
		set { CheckObject(ref _movementsList, value); }
	}

	/// <summary>
	///		Comando de siguiente movimiento
	/// </summary>
	public BaseCommand NextMovementCommand { get; }

	/// <summary>
	///		Comando de movimiento anterior
	/// </summary>
	public BaseCommand PreviousMovementCommand { get; }

	/// <summary>
	///		Comando para salir de la variación
	/// </summary>
	public BaseCommand VariationExitCommand { get; }
}