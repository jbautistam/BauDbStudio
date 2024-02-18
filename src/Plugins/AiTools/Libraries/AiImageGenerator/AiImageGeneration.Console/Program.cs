/*
	Web de StableHorde: https://horde.koboldai.net/
	Descripción de la API de StableHorde: https://stablehorde.net/api/
*/
using Bau.Libraries.LibAiImageGeneration;
using Bau.Libraries.LibAiImageGeneration.Models;

CancellationToken cancellationToken = CancellationToken.None;
ImageGenerationManager manager = new();

// Añade el proveedor de Horde
manager.AddProvider("Horde", new Bau.Libraries.LibStableHorde.Api.StableHordeManager(new Uri("https://stablehorde.net"), "0000000000"));

// Asigna el manejador de eventos
manager.Generated += (sender, args) => WriteResult(args.Generation);

Console.Clear();
Console.ForegroundColor = ConsoleColor.Gray;
Console.WriteLine("Connecting to StableHorde ...");

if (!await manager.ConnectAsync(cancellationToken))
	Console.WriteLine("Can't connect to StableHorde or can't find models");
else
{
	string? prompt;

		//// Muestra los modelos activos
		//if (manager.Models.Count() > 1)
		//{
		//	Console.WriteLine("These models have been loaded:");
		//	foreach (Bau.Libraries.LibStableHorde.Api.Dtos.ModelDto item in manager.Models)
		//		Console.WriteLine($"\t{item.Name} - {item.Type.ToString()}");
		//}
		//// Muestra el modelo que se está utilizando
		//Console.WriteLine($"You are talking to {manager.Model} now.");
		// Procesa los input del usuario
		prompt = GetPrompt();
		while (!string.IsNullOrEmpty(prompt) && !prompt.Equals("bye", StringComparison.CurrentCultureIgnoreCase))
		{
			// Si realmente ha escrito algo
			if (!string.IsNullOrWhiteSpace(prompt))
			{
				// Cambia el color para la salida del prompt
				Console.ForegroundColor = ConsoleColor.Cyan;
				// Llama al generador de imágenes para obtener la respuesta
				try
				{
					if (await manager.PromptAsync("Horde", new Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests.PromptModel
												 					{
																		Prompt = prompt
																	}, 
												  cancellationToken))
						Console.WriteLine("Waiting prompt generation ...");
					else
						Console.WriteLine("Error prompt generation");
				}
				catch (Exception exception)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Error when call to Horde AI");
					Console.WriteLine($"Error: {exception.Message}");
				}
				// Salta de línea
				Console.WriteLine();
				// Obtiene un nuevo prompt
				prompt = GetPrompt();
			}
		}
		// Finaliza el proceso
		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine("Bye, bye");
}

// Obtiene una entrada del usuario
string? GetPrompt()
{
	Console.ForegroundColor = ConsoleColor.White;
	Console.Write("> ");
	return Console.ReadLine();
}

// Escribe el resultado
void WriteResult(GenerationModel generation)
{
	Console.WriteLine($"[{generation.Status.ToString()}] {generation.Message}");
	if (generation.Images.Count == 0)
		Console.WriteLine("No images");
	else
		foreach (GenerationImageModel image in generation.Images)
			Console.WriteLine($"\t{image.Model}\t{image.Image}");
}

