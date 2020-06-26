using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.DbStudio.Models.Deployments;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Deployments
{
	/// <summary>
	///		ViewModel de datos de distribución
	/// </summary>
	public class DeploymentViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Variables privadas
		private string _name, _description, _sourcePath, _targetPath, _jsonParameters;
		private bool _isNew, _writeComments, _lowcaseFileNames, _replaceArguments;
		private ComboViewModel _comboTypes;

		public DeploymentViewModel(SolutionViewModel solutionViewModel, DeploymentModel deployment)
		{
			// Inicializa las propiedades
			SolutionViewModel = solutionViewModel;
			IsNew = deployment == null;
			Deployment = deployment ?? new DeploymentModel(solutionViewModel.Solution);
			// Inicializa el viewModel
			InitViewModel();
		}

		/// <summary>
		///		Inicializa el ViewModel
		/// </summary>
		private void InitViewModel()
		{
			// Carga el combo de tipos
			LoadComboTypes();
			// Asigna las propiedades
			Name = Deployment.Name;
			if (string.IsNullOrWhiteSpace(Name))
				Name = "Nueva distribución";
			Description = Deployment.Description;
			ComboTypes.SelectedID = (int) Deployment.Type;
			SourcePath = Deployment.SourcePath;
			TargetPath = Deployment.TargetPath;
			JsonParameters = Deployment.JsonParameters;
			WriteComments = Deployment.WriteComments;
			ReplaceArguments = Deployment.ReplaceArguments;
			LowcaseFileName = Deployment.LowcaseFileNames;
			// Si es nuevo, añade los parámetros predeterminados
			if (IsNew)
			{
				JsonParameters = "{" + Environment.NewLine;
				JsonParameters += "\t\"MountPath\": \"$mountpath\"," + Environment.NewLine;
				JsonParameters += "\t\"DbCompute\": \"$dbcompute\"" + Environment.NewLine;
				JsonParameters += "}";
			}
			// Indica que no ha habido modificaciones
			IsUpdated = false;
		}

		/// <summary>
		///		Carga el combo de tipos
		/// </summary>
		private void LoadComboTypes()
		{
			// Crea el combo
			ComboTypes = new ComboViewModel(this);
			// Añade los elementos
			ComboTypes.AddItem(null, "<Seleccione un tipo de distribución>");
			ComboTypes.AddItem((int) DeploymentModel.DeploymentType.Databricks, "Databricks");
			// Selecciona el primer elemento
			ComboTypes.SelectedItem = ComboTypes.Items[0];
		}

		/// <summary>
		///		Comprueba los datos introducidos
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos introducidos
				if (string.IsNullOrWhiteSpace(Name))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de la distribución");
				else if (ComboTypes.SelectedID == null)
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione un tipo");
				else if (string.IsNullOrWhiteSpace(SourcePath))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione el directorio origen");
				else if (!System.IO.Directory.Exists(SourcePath))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("El directorio origen no existe");
				else if (string.IsNullOrWhiteSpace(TargetPath))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Seleccione el directorio destino");
				else
					validated = true;
				// Devuelve el valor que indica si los datos son correctos
				return validated;
		}

		/// <summary>
		///		Graba los datos
		/// </summary>
		protected override void Save()
		{
			if (ValidateData())
			{
				// Asigna los datos al objeto
				Deployment.Name = Name;
				Deployment.Description = Description;
				Deployment.Type = (DeploymentModel.DeploymentType) (ComboTypes.SelectedID ?? 0);
				Deployment.SourcePath = SourcePath;
				Deployment.TargetPath = TargetPath;
				Deployment.JsonParameters = JsonParameters;
				Deployment.WriteComments = WriteComments;
				Deployment.ReplaceArguments = ReplaceArguments;
				Deployment.LowcaseFileNames = LowcaseFileName;
				// Añade los datos a la solución si es necesario
				if (IsNew)
					SolutionViewModel.Solution.Deployments.Add(Deployment);
				// Graba la solución
				SolutionViewModel.MainViewModel.SaveSolution();
				// Indica que ya no es nuevo y está grabado
				IsNew = false;
				IsUpdated = false;
				// Cierra la ventana
				RaiseEventClose(true);
			}
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Datos de distribución
		/// </summary>
		public DeploymentModel Deployment { get; }

		/// <summary>
		///		Indica si es nuevo
		/// </summary>
		public bool IsNew
		{
			get { return _isNew; }
			set { CheckProperty(ref _isNew, value); }
		}

		/// <summary>
		///		Nombre
		/// </summary>
		public string Name 
		{
			get { return _name; }
			set { CheckProperty(ref _name, value); }
		}

		/// <summary>
		///		Descripción
		/// </summary>
		public string Description
		{
			get { return _description; }
			set { CheckProperty(ref _description, value); }
		}

		/// <summary>
		///		Tipos de conexión
		/// </summary>
		public ComboViewModel ComboTypes
		{
			get { return _comboTypes; }
			set { CheckObject(ref _comboTypes, value); }
		}

		/// <summary>
		///		Directorio origen
		/// </summary>
		public string SourcePath
		{
			get { return _sourcePath; }
			set { CheckProperty(ref _sourcePath, value); }
		}

		/// <summary>
		///		Directorio destino
		/// </summary>
		public string TargetPath
		{
			get { return _targetPath; }
			set { CheckProperty(ref _targetPath, value); }
		}

		/// <summary>
		///		Json de parámetros para la distribución
		/// </summary>
		public string JsonParameters
		{
			get { return _jsonParameters; }
			set { CheckProperty(ref _jsonParameters, value); }
		}

		/// <summary>
		///		Indica si se deben escribir los comentarios en los archivos de salida
		/// </summary>
		public bool WriteComments
		{
			get { return _writeComments; }
			set { CheckProperty(ref _writeComments, value); }
		}

		/// <summary>
		///		Indica si se deben pasar los nombres de archivos a minúsculas
		/// </summary>
		public bool LowcaseFileName
		{
			get { return _lowcaseFileNames; }
			set { CheckProperty(ref _lowcaseFileNames, value); }
		}

		/// <summary>
		///		Indica si se deben reemplazar los argumentos $argument por GetArgument()
		/// </summary>
		public bool ReplaceArguments
		{
			get { return _replaceArguments; }
			set { CheckProperty(ref _replaceArguments, value); }
		}
	}
}