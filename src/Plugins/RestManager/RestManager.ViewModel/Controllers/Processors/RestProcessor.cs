using Bau.Libraries.RestManager.ViewModel.Project;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;
using Bau.Libraries.RestManager.Application;

namespace Bau.Libraries.RestManager.ViewModel.Controllers.Processors;

/// <summary>
///		Procesador para ejecución de un proyecto Rest
/// </summary>
internal class RestProcessor : ProcessModel
{
	public RestProcessor(RestFileViewModel restFileViewModel, Project.Steps.RestFileStepItemViewModel? stepViewModel) : base("RestProcessor", "Process rest file")
	{
		RestFileViewModel = restFileViewModel;
		StepViewModel = stepViewModel;
	}

	/// <summary>
	///		Ejecuta la exportación
	/// </summary>
	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		RestProjectManager manager = new();

			// Log de inicio
			RaiseLog(LogEventArgs.Status.Info, $"Start rest project");
			// Asigna el procesamiento de eventos
			manager.Log += (sender, args) => TreatLog(args);
			// Ejecuta el proceso
			try
			{
				if (StepViewModel is not null)
					await manager.ExecuteAsync(RestFileViewModel.RestProject, StepViewModel.RestStep, cancellationToken);
				else
					await manager.ExecuteAsync(RestFileViewModel.RestProject, cancellationToken);
				// Indica que se ha terminado
				RaiseLog(LogEventArgs.Status.Success, "End process rest project");
			}
			catch (Exception exception)
			{
				RaiseLog(LogEventArgs.Status.Error, $"Error when execute project {Path.GetFileName(RestFileViewModel.FileName)}. {exception.Message}");
			}

		// Trata el evento de log
		void TreatLog(Application.EventArguments.LogEventArgs args)
		{
			switch (args.State)
			{
				case Application.EventArguments.LogEventArgs.Status.Success:
						RaiseLog(LogEventArgs.Status.Success, args.Message);
					break;
				case Application.EventArguments.LogEventArgs.Status.Error:
						RaiseLog(LogEventArgs.Status.Error, args.Message);
					break;
				case Application.EventArguments.LogEventArgs.Status.Warning:
						RaiseLog(LogEventArgs.Status.Warning, args.Message);
					break;
				case Application.EventArguments.LogEventArgs.Status.Info:
						RaiseLog(LogEventArgs.Status.Info, args.Message);
					break;
			};
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public RestFileViewModel RestFileViewModel { get; }

	/// <summary>
	///		ViewModel del paso de ejecución
	/// </summary>
	public Project.Steps.RestFileStepItemViewModel? StepViewModel { get; }
	
	/// <summary>
	///		Id de tarea
	/// </summary>
	public string TaskId { get; private set; } = Guid.NewGuid().ToString();
}
