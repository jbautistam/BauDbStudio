using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ListView;
using Bau.Libraries.LibAiImageGeneration.Controllers;
using Bau.Libraries.LibAiImageGeneration.Domain.Models.Results;
using Bau.Libraries.AiTools.Application.Models.Prompts;

namespace Bau.Libraries.AiTools.ViewModels.Prompts;

/// <summary>
///		ViewModel del texto generado del prompt
/// </summary>
public class PromptVersionViewModel : BauMvvm.ViewModels.BaseObservableObject
{
	// Variables privadas
	private string _versionText = default!, _positive = default!, _negative = default!;
	private int _version;
	private bool _nsfw;
	private double _cfgScale = 7.5, _denoisingStrength = 0.75;
	private string _seed = default!;
	private int _height = 512, _width = 512;
	private bool _karras = true;
	private int _steps = 30, _imagesToGenerate = 4;
	private ComboViewModel _comboGenerators = default!;
	private ComboViewModel _comboModels = default!;
	private ComboViewModel _comboSampler = default!;
	private ControlListViewModel _listPostProcessing = default!;
	private Images.ImagesGeneratedListViewModel _imagesViewModel = default!;

	public PromptVersionViewModel(PromptVersionListViewModel promptVersionListViewModel, PromptModel prompt)
	{
		// Guarda los objetos
		PromptVersionListViewModel = promptVersionListViewModel;
		Prompt = prompt;
		ImagesViewModel = new Images.ImagesGeneratedListViewModel(this);
		// Inicializa el ViewModel
		Initialize();
	}

	/// <summary>
	///		Inicializa el viewModel
	/// </summary>
	public void Initialize()
	{
		// Inicializa los combos
		InitComboGenerators();
		InitComboSamplers();
		InitListPostProcess(Prompt.PostProcessing);
		// Inicializa los datos
		Version = Prompt.Version;
		Positive = Prompt.Positive;
		Negative = Prompt.Negative;
		Nsfw = Prompt.Nsfw;
		ComboSampler.SelectedId = (int) Prompt.Sampler;
		CfgScale = Prompt.CfgScale;
		DenoisingStrength = Prompt.DenoisingStrength;
		Seed = Prompt.Seed;
		Height = Prompt.Height;
		Width = Prompt.Width;
		Karras = Prompt.Karras;
		Steps = Prompt.Steps;
		ImagesToGenerate = Prompt.ImagesToGenerate;
		// Carga las imágenes
		ImagesViewModel.Load();
	}

	/// <summary>
	///		Inicializa el combo con los generadores
	/// </summary>
	private void InitComboGenerators()
	{
		int index = 0;

			// Limpia el combo
			ComboGenerators = new ComboViewModel(this);
			ComboGenerators.PropertyChanged += (sender, args) => {
																	if (!string.IsNullOrWhiteSpace(args.PropertyName) &&
																			args.PropertyName.Equals(nameof(ComboViewModel.SelectedItem)))
																		InitComboModels();
																 };
			// Añade los proveedores
			foreach (KeyValuePair<string, AiProvider> keyValue in PromptVersionListViewModel.PromptFileViewModel.MainViewModel.AiImageGenerationManager.ImageGenerationManager.AiProvidersManager.Providers)
				ComboGenerators.AddItem(index++, keyValue.Value.Name, keyValue.Value);
			// Selecciona el primer elemento
			if (ComboGenerators.Items.Count > 0)
				ComboGenerators.SelectedId = 0;
			// Selecciona el proveedor adecuado
			if (!string.IsNullOrWhiteSpace(Prompt.Provider))
				ComboGenerators.SelectedText = Prompt.Provider;
	}

	/// <summary>
	///		Inicializa el combo con los modelos
	/// </summary>
	private void InitComboModels()
	{
		AiProvider? provider = ComboGenerators.SelectedTag as AiProvider;
		int index = 0;

			// Limpia el combo
			ComboModels = new ComboViewModel(this);
			// Añade los proveedores
			if (provider is not null)
				foreach (DefinitionResultModel model in provider.Models)
					ComboModels.AddItem(index++, model.Name, model);
			// Selecciona el primer elemento
			if (ComboModels.Items.Count > 0)
				ComboModels.SelectedId = 0;
			// Selecciona el modelo adecuado
			if (!string.IsNullOrWhiteSpace(Prompt.Model))
				ComboModels.SelectedText = Prompt.Model;
	}

	/// <summary>
	///		Inicializa el combo de sampleadores
	/// </summary>
	private void InitComboSamplers()
	{
		// Limpia el combo
		ComboSampler = new ComboViewModel(this);
		// Añade los sampleadores
		foreach (PromptModel.SamplerType sampler in Enum.GetValues<PromptModel.SamplerType>())
			ComboSampler.AddItem((int) sampler, sampler.ToString());
		// Selecciona el primer elemento
		if (ComboSampler.Items.Count > 0)
			ComboSampler.SelectedId = 0;
	}

	/// <summary>
	///		Inicializa la lista de postproceso
	/// </summary>
	private void InitListPostProcess(List<PromptModel.PostProcessType> postProcessing)
	{
		// Limpia la lista
		ListViewPostProcessing = new ControlListViewModel();
		// Añade los sampleadores
		foreach (PromptModel.PostProcessType postProcess in Enum.GetValues<PromptModel.PostProcessType>())
		{
			BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel controlItem = new(postProcess.ToString(), postProcess);

				// Selecciona el elemento si es necesario
				controlItem.IsChecked = postProcessing.Any(item => item == postProcess);
				// Añade el elemento
				ListViewPostProcessing.Items.Add(controlItem);
		}
	}

	/// <summary>
	///		Obtiene el prompt
	/// </summary>
	internal PromptModel GetPrompt()
	{
		PromptModel prompt = new(ComboGenerators.SelectedText ?? string.Empty)
								{
									Version = Version,
									Positive = Positive,
									Negative = Negative,
									Model = ComboModels.SelectedText ?? string.Empty,
									Nsfw = Nsfw,
									CfgScale = CfgScale,
									DenoisingStrength = DenoisingStrength,
									Seed = Seed,
									Height = Height,
									Width = Width,
									Karras = Karras,
									Steps = Steps,
									ImagesToGenerate = ImagesToGenerate,
								};

			// Obtiene el sampleador
			if (ComboSampler.SelectedId is null)
				prompt.Sampler = PromptModel.SamplerType.k_euler;
			else
				prompt.Sampler = (PromptModel.SamplerType) ComboSampler.SelectedId;
			// Añade los datos de postproceso
			prompt.PostProcessing.Clear();
			foreach (BauMvvm.ViewModels.Forms.ControlItems.ControlItemViewModel item in ListViewPostProcessing.Items)
				if (item.IsChecked && item.Tag is PromptModel.PostProcessType type)
					prompt.PostProcessing.Add(type);
			// Devuelve el prompt
			return prompt;
	}

	/// <summary>
	///		Obtiene el prefijo de las imágenes del archivo
	/// </summary>
	internal string? GetFileImagePrefix()
	{
		string? expectedFilePrefix = Path.GetFileNameWithoutExtension(PromptVersionListViewModel.PromptFileViewModel.PromptFile.FileName);

			// Añade el código de versión
			if (!string.IsNullOrWhiteSpace(expectedFilePrefix))
				expectedFilePrefix += $".{VersionText}.";
			// Si ha llegado hasta aquí es porque no es un archivo de esta versión
			return expectedFilePrefix;
	}

	/// <summary>
	///		Carga las imágenes
	/// </summary>
	internal void LoadImages()
	{
		ImagesViewModel.Load();
	}

	/// <summary>
	///		Lista de versiones a la que se asocia
	/// </summary>
	public PromptVersionListViewModel PromptVersionListViewModel { get; }

	/// <summary>
	///		Prompt
	/// </summary>
	public PromptModel Prompt { get; }

	/// <summary>
	///		Combo con los generadores
	/// </summary>
	public ComboViewModel ComboGenerators
	{
		get { return _comboGenerators; }
		set { CheckObject(ref _comboGenerators, value); }
	}

	/// <summary>
	///		Combo con los modelos
	/// </summary>
	public ComboViewModel ComboModels
	{
		get { return _comboModels; }
		set { CheckObject(ref _comboModels, value); }
	}

	/// <summary>
	///		Combo con el tipo de sampleado
	/// </summary>
	public ComboViewModel ComboSampler
	{
		get { return _comboSampler; }
		set { CheckObject(ref _comboSampler, value); }
	}

	/// <summary>
	///		ListView con los tipos de postproceso
	/// </summary>
	public ControlListViewModel ListViewPostProcessing
	{
		get { return _listPostProcessing; }
		set { CheckObject(ref _listPostProcessing, value); }
	}

	/// <summary>
	///		Número de versión
	/// </summary>
	public int Version
	{
		get { return _version; }
		set
		{
			if (CheckProperty(ref _version, value))
				VersionText = $"V {_version.ToString()}";
		}
	}

	/// <summary>
	///		Versión
	/// </summary>
	public string VersionText
	{ 
		get { return _versionText; }
		set { CheckProperty(ref _versionText, value); }
	}

	/// <summary>
	///		Prompt positivo
	/// </summary>
	public string Positive
	{ 
		get { return _positive; }
		set { CheckProperty(ref _positive, value); }
	}

	/// <summary>
	///		Prompt negativo
	/// </summary>
	public string Negative
	{ 
		get { return _negative; }
		set { CheckProperty(ref _negative, value); }
	}

	/// <summary>
	///		Datos de Nsfw
	/// </summary>
	public bool Nsfw
	{
		get { return _nsfw; }
		set { CheckProperty(ref _nsfw, value); }
	}

	/// <summary>
	///		Datos de CfgScale
	/// </summary>
	public double CfgScale
	{
		get { return _cfgScale; }
		set { CheckProperty(ref _cfgScale, value); }
	}

	/// <summary>
	///		Datos de DenoisingStrength
	/// </summary>
	public double DenoisingStrength
	{
		get { return _denoisingStrength; }
		set { CheckProperty(ref _denoisingStrength, value); }
	}

	/// <summary>
	///		Datos de Seed
	/// </summary>
	public string Seed
	{
		get { return _seed; }
		set { CheckProperty(ref _seed, value); }
	}

	/// <summary>
	///		Datos de Height
	/// </summary>
	public int Height
	{
		get { return _height; }
		set { CheckProperty(ref _height, value); }
	}

	/// <summary>
	///		Datos de Width
	/// </summary>
	public int Width
	{
		get { return _width; }
		set { CheckProperty(ref _width, value); }
	}

	/// <summary>
	///		Datos de Karras
	/// </summary>
	public bool Karras
	{
		get { return _karras; }
		set { CheckProperty(ref _karras, value); }
	}

	/// <summary>
	///		Datos de Steps
	/// </summary>
	public int Steps
	{
		get { return _steps; }
		set { CheckProperty(ref _steps, value); }
	}

	/// <summary>
	///		Datos de ImagesToGenerate
	/// </summary>
	public int ImagesToGenerate
	{
		get { return _imagesToGenerate; }
		set { CheckProperty(ref _imagesToGenerate, value); }
	}

	/// <summary>
	///		ViewModel de las imágenes
	/// </summary>
	public Images.ImagesGeneratedListViewModel ImagesViewModel
	{
		get { return _imagesViewModel; }
		set { CheckObject(ref _imagesViewModel, value); }
	}
}
