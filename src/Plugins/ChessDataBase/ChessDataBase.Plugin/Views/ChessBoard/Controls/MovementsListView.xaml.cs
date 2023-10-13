using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.Board;

namespace Bau.Libraries.ChessDataBase.Plugin.Views.ChessBoard.Controls;

/// <summary>
///		View para una lista de movimientos
/// </summary>
public partial class MovementsListView : UserControl
{
	// Propiedades
	public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(GameBoardViewModel),
																							  typeof(MovementsListView), new PropertyMetadata(null));
	// Variables privadas
	private GameBoardViewModel _gameBoardViewModel = default!;

	public MovementsListView()
	{
		InitializeComponent();
	}

	public void ShowComments(bool show)
	{
		if (show)
			brdMovements.Visibility = Visibility.Visible;
		else
			brdMovements.Visibility = Visibility.Collapsed;
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public GameBoardViewModel ViewModel 
	{ 
		get { return _gameBoardViewModel; }
		set 
		{
			if (!ReferenceEquals(_gameBoardViewModel, value))
				DataContext = _gameBoardViewModel = value;
		}
	}

	private void lstMovements_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ListBox lstView && DataContext is GameBoardViewModel gameBoardViewModel &&
				gameBoardViewModel?.MovementsList != null)
			lstView.ScrollIntoView(gameBoardViewModel.MovementsList.SelectedMovement);
	}
}
