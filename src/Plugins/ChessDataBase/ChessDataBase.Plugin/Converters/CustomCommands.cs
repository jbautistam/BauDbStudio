using System.Windows.Input;

namespace Bau.Libraries.ChessDataBase.Plugin.Converters;

/// <summary>
///		Comandos personalizados
/// </summary>
public static class CustomCommands
{
	/// <summary>
	///		Salir de la aplicación
	/// </summary>
	public static readonly RoutedUICommand Exit = new RoutedUICommand("Exit", "Exit", typeof(CustomCommands), 
																	  new InputGestureCollection()
																			{
																				new KeyGesture(Key.F4, ModifierKeys.Alt)
																			}
																	  );
}
