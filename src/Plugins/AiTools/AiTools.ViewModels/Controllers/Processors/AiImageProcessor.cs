using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.AiTools.Application.Models.Prompts;
using Bau.Libraries.AiTools.ViewModels.Prompts;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Requests;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;
using Bau.Libraries.LibAiImageGeneration.Models;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models.Processes;

namespace Bau.Libraries.AiTools.ViewModels.Controllers.Processors;

/// <summary>
///		Procesador para la generación de imágenes
/// </summary>
internal class AiImageProcessor : ProcessModel
{
	public AiImageProcessor(AiToolsViewModel mainViewModel, PromptVersionViewModel promptVersionViewModel, string positive) 
				: base("AiTools", "Images generation")
	{
		MainViewModel = mainViewModel;
		PromptVersionViewModel = promptVersionViewModel;
		Positive = positive;
	}

	/// <summary>
	///		Ejecuta la exportación
	/// </summary>
	public override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		PromptResultModel result = await ExecutePromptAsync(cancellationToken);
		 
			if (!result.CanProcess || string.IsNullOrWhiteSpace(result.Id))
				RaiseLog(LogEventArgs.Status.Error, $"Can't execute the prompt{Environment.NewLine}{result.Message}");
			else
			{
				// Log
				RaiseLog(LogEventArgs.Status.Info, $"Start images generation {PromptVersionViewModel.ImagesToGenerate:#,##0}");
				// Guarda el Id de tarea
				TaskId = result.Id;
				// Asigna el manejador de eventos
				MainViewModel.AiImageGenerationManager.ImageGenerationManager.Progress += (sender, args) => TreatProgress(args.Generation);
			}
	}

	/// <summary>
	///		Ejecuta el prompt
	/// </summary>
	private async Task<PromptResultModel> ExecutePromptAsync(CancellationToken cancellationToken)
	{
		PromptRequestModel prompt = ConvertToAiPrompt(PromptVersionViewModel.GetPrompt(), PromptVersionViewModel.PromptVersionListViewModel.PromptFileViewModel);

			// Inicializa el generador de imágenes
			await MainViewModel.AiImageGenerationManager.InitializeAsync(cancellationToken);
			// Ejecuta el prompt
			return await MainViewModel.AiImageGenerationManager.ImageGenerationManager.PromptAsync(prompt, cancellationToken);
	}

	/// <summary>
	///		Convierte el modelo de prompts
	/// </summary>
	private PromptRequestModel ConvertToAiPrompt(PromptModel prompt, PromptFileViewModel promptFileViewModel)
	{
		PromptRequestModel request = new(prompt.Provider, Path.GetDirectoryName(promptFileViewModel.PromptFile.FileName) ?? string.Empty,
										 promptFileViewModel.VersionsViewModel.SelectedItem?.GetFileImagePrefix() ?? string.Empty);

			// Asigna las propiedades
			request.Prompt = prompt.Positive;
			request.NegativePrompt = prompt.Negative;
			request.Nsfw = prompt.Nsfw;
			request.Models.Add(prompt.Model);
			request.Sampler = ConvertSampler(prompt.Sampler);
			request.CfgScale = prompt.CfgScale;
			request.DenoisingStrength = prompt.DenoisingStrength;
			request.Seed = prompt.Seed;
			request.Height = prompt.Height;
			request.Width = prompt.Width;
			request.PostProcessing.AddRange(ConvertPostprocessing(prompt.PostProcessing));
			request.Steps = prompt.Steps;
			request.ImagesToGenerate = prompt.ImagesToGenerate;
			// Devuelve la solicitud
			return request;
	}

	/// <summary>
	///		Convierte el modelo de sampleado
	/// </summary>
	private PromptRequestModel.SamplerType ConvertSampler(PromptModel.SamplerType sampler)
	{
		return sampler switch
					{
						PromptModel.SamplerType.k_dpm_fast => PromptRequestModel.SamplerType.k_dpm_fast,
						PromptModel.SamplerType.k_heun => PromptRequestModel.SamplerType.k_heun,
						PromptModel.SamplerType.k_dpmpp_sde => PromptRequestModel.SamplerType.k_dpmpp_sde,
						PromptModel.SamplerType.dpmsolver => PromptRequestModel.SamplerType.dpmsolver,
						PromptModel.SamplerType.k_dpm_2_a => PromptRequestModel.SamplerType.dpmsolver,
						PromptModel.SamplerType.k_dpm_adaptive => PromptRequestModel.SamplerType.k_dpm_adaptive,
						PromptModel.SamplerType.k_dpmpp_2s_a => PromptRequestModel.SamplerType.k_dpmpp_2s_a,
						PromptModel.SamplerType.k_lms => PromptRequestModel.SamplerType.k_lms,
						PromptModel.SamplerType.k_euler_a => PromptRequestModel.SamplerType.k_euler_a,
						PromptModel.SamplerType.lcm => PromptRequestModel.SamplerType.lcm,
						PromptModel.SamplerType.DDIM => PromptRequestModel.SamplerType.DDIM,
						PromptModel.SamplerType.k_dpmpp_2m => PromptRequestModel.SamplerType.k_dpmpp_2m,
						PromptModel.SamplerType.k_dpm_2 => PromptRequestModel.SamplerType.k_dpm_2,
						_ => PromptRequestModel.SamplerType.k_euler,
					};
	}

	/// <summary>
	///		Convierte la lista de tipos de postproceso
	/// </summary>
	private List<PromptRequestModel.PostProcessType> ConvertPostprocessing(List<PromptModel.PostProcessType> postProcessing)
	{
		List<PromptRequestModel.PostProcessType> result = new();

			// Añade los datos convertidos
			foreach (PromptModel.PostProcessType postProcess  in postProcessing)
				result.Add(ConvertPostprocess(postProcess));
			// Devuelve la lista
			return result;

			// Convierte el tipo de postproceso
			PromptRequestModel.PostProcessType ConvertPostprocess(PromptModel.PostProcessType type)
			{
				return type switch
						{
							PromptModel.PostProcessType.RealESRGAN_x4plus => PromptRequestModel.PostProcessType.RealESRGAN_x4plus,
							PromptModel.PostProcessType.RealESRGAN_x2plus => PromptRequestModel.PostProcessType.RealESRGAN_x2plus,
							PromptModel.PostProcessType.RealESRGAN_x4plus_anime_6B => PromptRequestModel.PostProcessType.RealESRGAN_x4plus_anime_6B,
							PromptModel.PostProcessType.NMKD_Siax => PromptRequestModel.PostProcessType.NMKD_Siax,
							PromptModel.PostProcessType.x4_AnimeSharp => PromptRequestModel.PostProcessType.x4_AnimeSharp,
							PromptModel.PostProcessType.CodeFormers => PromptRequestModel.PostProcessType.CodeFormers,
							PromptModel.PostProcessType.strip_background => PromptRequestModel.PostProcessType.strip_background,
							_ => PromptRequestModel.PostProcessType.GFPGAN,
						};
		}
	}

	/// <summary>
	///		Trata el progreso de generación de las imágenes
	/// </summary>
	private void TreatProgress(GenerationModel generation)
	{
		if (!string.IsNullOrWhiteSpace(generation.Id) && generation.Id.Equals(TaskId, StringComparison.CurrentCultureIgnoreCase))
			switch (generation.Status)
			{
				case GenerationModel.GenerationStatus.Error:
						RaiseLog(LogEventArgs.Status.Error, $"Error. {generation.Message}");
					break;
				case GenerationModel.GenerationStatus.Success:
						// Lanza el progreso
						RaiseProgress(4, 4);
						RaiseLog(LogEventArgs.Status.Success, "Success");
						// Carga las imágenes en la lista y en el árbol
						PromptVersionViewModel.LoadImages();
						PromptVersionViewModel.PromptVersionListViewModel.PromptFileViewModel.RefreshFiles();
					break;
				case GenerationModel.GenerationStatus.Processing:
						RaiseLog(LogEventArgs.Status.Info, "Processing", GetAdditionalInfo(generation));
						RaiseProgress(2, 4, $"Position: {generation.QueuePosition.ToString()}. Finished: {generation.Finished.ToString()}. Processing: {generation.Processing.ToString()}");
					break;
				case GenerationModel.GenerationStatus.Generated:
						RaiseProgress(3, 4, "Images generated. Start download");
					break;
			}

		// Obtiene la información adicional sobre la generación
		Dictionary<string, string> GetAdditionalInfo(GenerationModel generation)
		{
			return new Dictionary<string, string>
							{
								{ "Next query", $"{TimeZoneInfo.ConvertTimeFromUtc(generation.NextSearch, TimeZoneInfo.Local):HH:mm:ss}" },
								{ "TaskId", generation.Id.Left(10) }
							};
		}
	}

	/// <summary>
	///		ViewModel principal
	/// </summary>
	public AiToolsViewModel MainViewModel { get; }

	/// <summary>
	///		ViewModel de la versión del prompt
	/// </summary>
	public PromptVersionViewModel PromptVersionViewModel { get; }
	
	/// <summary>
	///		Texto positivo
	/// </summary>
	public string Positive { get; }
	
	/// <summary>
	///		Id de tarea
	/// </summary>
	public string TaskId { get; private set; } = Guid.NewGuid().ToString();
}
