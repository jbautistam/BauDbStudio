using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Media;
using Bau.Libraries.ChessDataBase.Models.Board.Movements;
using Bau.Libraries.ChessDataBase.Models.Pieces;

namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board.Movements;

/// <summary>
///		ViewModel del movimiento de una figura
/// </summary>
public class MovementFigureViewModel : BaseMovementViewModel
{
	// Variables privadas
	private MovementFigureModel _movement = default!;
	private PieceBaseModel.PieceType _piece;
	private PieceBaseModel.PieceColor _color;
	private string _text = default!, _time = default!, _remarksText = default!;
	private bool _hasVariation, _selected, _hasRemarks;
	private MvvmColor _foreGround = default!, _backGround = default!;
	private MovementListSelectVariationViewModel _variationsList = default!;
	private ObservableCollection<MovementRemarkViewModel> _remarks = default!;

	public MovementFigureViewModel(MovementListViewModel movementListViewModel, MovementFigureModel movement)
	{
		// Inicializa las propiedades
		MovementListViewModel = movementListViewModel;
		Selected = false;
		Piece = movement.OriginPiece;
		Color = movement.Color;
		Movement = movement;
		Text = movement.Content;
		HasVariation = movement.Variations.Count > 0;
		Time = "01:07";
		// Añade las variaciones
		VariationsList = new MovementListSelectVariationViewModel(MovementListViewModel.GameBoardViewModel, movement);
		// Añade los comentarios
		Remarks = new ObservableCollection<MovementRemarkViewModel>();
		foreach (string comment in movement.Comments)
		{
			Remarks.Add(new MovementRemarkViewModel(comment));
			RemarksText += comment + Environment.NewLine;
		}
		HasRemarks = Remarks.Count > 0;
		// Inicializa la forma en que se debe ver el movimiento en la lista (tamaños de fuente, tamaños de imagen...)
		AssignViewSizes();
		// Inicializa los comandos
		SelectMovementCommand = new BaseCommand(_ => ExecuteMovement(), _ => CanExecuteMovement());
	}

	/// <summary>
	///		Asigna los tamaños y fuentes que se deben mostrar en la lista de movimientos dependiendo de la variación
	/// </summary>
	private void AssignViewSizes()
	{
		if (Selected)
		{
			Foreground = MvvmColor.White;
			Background = MvvmColor.Navy;
		}
		else
		{
			Foreground = MvvmColor.Black;
			Background = MvvmColor.Transparent;
		}
	}

	/// <summary>
	///		Ejecuta un movimiento
	/// </summary>
	private void ExecuteMovement()
	{
		Selected = true;
		MovementListViewModel.GameBoardViewModel.GoToMovement(this);
	}

	/// <summary>
	///		Indica si puede ejecutar el movimiento
	/// </summary>
	private bool CanExecuteMovement() => Movement is not null;

	/// <summary>
	///		Lista de movimientos
	/// </summary>
	private MovementListViewModel MovementListViewModel { get; }

	/// <summary>
	///		Movimiento
	/// </summary>
	public MovementFigureModel Movement 
	{ 
		get { return _movement; }
		set { CheckProperty(ref _movement, value); }
	}

	/// <summary>
	///		Indica si el movimiento está seleccionado
	/// </summary>
	public bool Selected
	{
		get { return _selected; }
		set 
		{ 
			if (CheckProperty(ref _selected, value))
				AssignViewSizes();
		}
	}

	/// <summary>
	///		Nombre de la pieza
	/// </summary>
	public PieceBaseModel.PieceType Piece 
	{ 
		get { return _piece; }
		set { CheckProperty(ref _piece, value); }
	}

	/// <summary>
	///		Color de la pieza
	/// </summary>
	public PieceBaseModel.PieceColor Color
	{
		get { return _color; }
		set { CheckProperty(ref _color, value); }
	}

	/// <summary>
	///		Texto del movimiento
	/// </summary>
	public string Text
	{
		get { return _text; }
		set { CheckProperty(ref _text, value); }
	}

	/// <summary>
	///		Indica si el movimiento tiene una variación
	/// </summary>
	public bool HasVariation
	{
		get { return _hasVariation; }
		set { CheckProperty(ref _hasVariation, value); }
	}

	/// <summary>
	///		Tiempo del movimiento
	/// </summary>
	public string Time
	{
		get { return _time; }
		set { CheckProperty(ref _time, value); }
	}

	/// <summary>
	///		Color de texto
	/// </summary>
	public MvvmColor Foreground
	{ 
		get { return _foreGround; }
		set { CheckObject(ref _foreGround, value); }
	}

	/// <summary>
	///		Color de fondo
	/// </summary>
	public MvvmColor Background
	{ 
		get { return _backGround; }
		set { CheckObject(ref _backGround, value); }
	}

	/// <summary>
	///		Comentarios
	/// </summary>
	public ObservableCollection<MovementRemarkViewModel> Remarks
	{
		get { return _remarks; }
		set { CheckObject(ref _remarks, value); }
	}

	/// <summary>
	///		Variaciones
	/// </summary>
	public MovementListSelectVariationViewModel VariationsList
	{
		get { return _variationsList; }
		set { CheckObject(ref _variationsList, value); }
	}

	/// <summary>
	///		Texto de los comentarios
	/// </summary>
	public string RemarksText
	{
		get { return _remarksText; }
		set { CheckProperty(ref _remarksText, value); }
	}

	/// <summary>
	///		Indica si tiene comentarios
	/// </summary>
	public bool HasRemarks
	{
		get { return _hasRemarks; }
		set { CheckProperty(ref _hasRemarks, value); }
	}

	/// <summary>
	///		Comando para selección del movimiento
	/// </summary>
	public BaseCommand SelectMovementCommand { get; }
}