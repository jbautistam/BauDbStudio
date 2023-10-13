using System.Windows;
using System.Windows.Controls;

using Bau.Libraries.ChessDataBase.ViewModels.Games;

namespace Bau.Libraries.ChessDataBase.Plugin.Views.ChessBoard;

/// <summary>
///		Control de usuario para mostrar el tablero con un archivo PGN
/// </summary>
public partial class ChessboardPgnView : UserControl
{
	public ChessboardPgnView(GameBoardPgnViewModel viewModel)
	{
		// Inicializa el viewModel
		DataContext = ViewModel = viewModel;
		// Inicializa los componentes
		InitializeComponent();
	}

	/// <summary>
	///		Inicializa el formulario
	/// </summary>
	private async Task InitFormAsync(CancellationToken cancellationToken)
	{
		// Inicializa el tablero
		udtListMovements.ViewModel = ViewModel.ChessGameViewModel.GameBoardViewModel;
		udtMovementInfoView.Init(ViewModel.ChessGameViewModel.GameBoardViewModel);
		udtBoard.Init(ViewModel.ChessGameViewModel.GameBoardViewModel);
		// Carga el juego predeterminado
		await LoadGameAsync(cancellationToken);
	}

	/// <summary>
	///		Carga el juego
	/// </summary>
	private async Task LoadGameAsync(CancellationToken cancellationToken)
	{
		(bool isLoaded, string error) = await ViewModel.ChessGameViewModel.LoadFileAsync(cancellationToken);

			if (!isLoaded)
				MessageBox.Show(error);
	}

	/// <summary>
	///		ViewModel
	/// </summary>
	public GameBoardPgnViewModel ViewModel { get; }

	private void lstMovements_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (sender is ListBox lstView && ViewModel.ChessGameViewModel.GameBoardViewModel?.MovementsList?.SelectedMovement != null)
			lstView.ScrollIntoView(ViewModel.ChessGameViewModel.GameBoardViewModel.MovementsList.SelectedMovement);
	}

	private async void UserControl_Initialized(object sender, EventArgs e)
	{
		await InitFormAsync(CancellationToken.None);
	}
}
