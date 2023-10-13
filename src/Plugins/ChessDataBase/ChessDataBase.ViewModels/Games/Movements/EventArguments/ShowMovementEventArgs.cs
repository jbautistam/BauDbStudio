namespace Bau.Libraries.ChessDataBase.ViewModels.Games.Movements.EventArguments;

/// <summary>
///		Argumentos del evento de mostrar movimientos
/// </summary>
public class ShowMovementEventArgs : EventArgs
{
	public ShowMovementEventArgs(List<Board.Actions.ActionBaseModel> actions, bool disableAnimations)
	{
		Actions = actions;
		DisableAnimations = disableAnimations;
	}

	/// <summary>
	///		Muestra las acciones
	/// </summary>
	public List<Board.Actions.ActionBaseModel> Actions { get; }

	/// <summary>
	///		Indica si se deben desactivar las animaciones
	/// </summary>
	public bool DisableAnimations { get; }
}
