using System.Windows.Controls;

using Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board;

namespace Bau.Libraries.ChessDataBase.Plugin.Views.ChessBoard.Controls;

/// <summary>
///		Control que muestra el icono y el texto de un movimiento
/// </summary>
public partial class MovementInfoView : UserControl
{
	public MovementInfoView()
	{
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa el ViewModel
	/// </summary>
	public void Init(GameBoardViewModel gameBoardViewModel)
	{
		DataContext = ViewModel = gameBoardViewModel;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public GameBoardViewModel ViewModel { get; private set; } = default!;
}
