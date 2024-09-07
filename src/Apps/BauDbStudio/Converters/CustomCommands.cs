using System.Windows.Input;

namespace Bau.DbStudio.Converters;

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

	/// <summary>
	///		Ejecutar un script
	/// </summary>
	public static readonly RoutedUICommand ExecuteScript = new RoutedUICommand("Execute", "Execute", typeof(CustomCommands), 
																			   new InputGestureCollection()
																						{
																							new KeyGesture(Key.F5, ModifierKeys.None)
																						}
																			  );

	/// <summary>
	///		Abrir una consulta
	/// </summary>
	public static readonly RoutedUICommand NewQuery = new RoutedUICommand("New query", "New query", typeof(CustomCommands), 
																		  new InputGestureCollection()
																						{
																							new KeyGesture(Key.Q, ModifierKeys.Control)
																						}
																		 );

	/// <summary>
	///		Abrir una consulta
	/// </summary>
	public static readonly RoutedUICommand Rename = new RoutedUICommand("Rename", "Rename", typeof(CustomCommands), 
																		  new InputGestureCollection()
																						{
																							new KeyGesture(Key.F2)
																						}
																		 );
}