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
	public static readonly RoutedUICommand Exit = new RoutedUICommand(nameof(Exit), nameof(Exit), typeof(CustomCommands), 
																	  new InputGestureCollection()
																			{
																				new KeyGesture(Key.F4, ModifierKeys.Alt)
																			}
																	  );

	/// <summary>
	///		Ejecutar un script
	/// </summary>
	public static readonly RoutedUICommand Execute = new RoutedUICommand(nameof(Execute), nameof(Execute), typeof(CustomCommands), 
																		 new InputGestureCollection()
																						{
																							new KeyGesture(Key.F5, ModifierKeys.None)
																						}
																		 );

	/// <summary>
	///		Renombrar un archivo
	/// </summary>
	public static readonly RoutedUICommand Rename = new RoutedUICommand(nameof(Rename), nameof(Rename), typeof(CustomCommands), 
																		new InputGestureCollection()
																						{
																							new KeyGesture(Key.F2)
																						}
																		);
}