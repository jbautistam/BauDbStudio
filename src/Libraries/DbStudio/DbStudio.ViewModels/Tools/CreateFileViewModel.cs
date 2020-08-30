using System;

using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;

namespace Bau.Libraries.DbStudio.ViewModels.Tools
{
	/// <summary>
	///		ViewModel para la creación de archivos
	/// </summary>
	public class CreateFileViewModel : BauMvvm.ViewModels.Forms.Dialogs.BaseDialogViewModel
	{
		// Enumerados privados
		private enum FileType
		{
			Sql,
			SqlExtended,
			Json,
			Xml,
			Other
		}
		// Variables privadas
		private string _fileName;
		private ComboViewModel _comboTypes;

		public CreateFileViewModel(Solutions.SolutionViewModel solutionViewModel, string path)
		{
			SolutionViewModel = solutionViewModel;
			Path = path;
			LoadComboTypes();
		}

		/// <summary>
		///		Carga el combo de tipos de archivos
		/// </summary>
		private void LoadComboTypes()
		{
			// Crea el combo
			ComboTypes = new ComboViewModel(this);
			// Añade los elementos
			ComboTypes.AddItem((int) FileType.Sql, "Sql");
			ComboTypes.AddItem((int) FileType.SqlExtended, "Sql extendido");
			ComboTypes.AddItem((int) FileType.Json, "Json");
			ComboTypes.AddItem((int) FileType.Xml, "Xml");
			ComboTypes.AddItem((int) FileType.Other, "Otros");
			// Asigna el manejador de eventos
			ComboTypes.PropertyChanged += (sender, args) =>
												{
													if (args.PropertyName.Equals(nameof(ComboTypes.SelectedItem)))
														UpdateFileExtension();
												};
			// Selecciona el primer elemento
			ComboTypes.SelectedItem = ComboTypes.Items[0];
		}

		/// <summary>
		///		Modifica la extensión del archivo
		/// </summary>
		private void UpdateFileExtension()
		{
			string file = FileName;

				// Cambia la extensión
				if (GetSelectedType() != FileType.Other)
				{
					// Inicializa el nombre de archivo
					if (string.IsNullOrWhiteSpace(file))
						file = "New file.sql";
					// Cambia la extensión
					file = System.IO.Path.GetFileNameWithoutExtension(file);
					file += GetExtension(GetSelectedType());
				}
				// Cambia el nombre de archivo
				FileName = file;
		}

		/// <summary>
		///		Obtiene la extensión asociada a un tipo de archivo
		/// </summary>
		private string GetExtension(FileType fileType)
		{
			switch (fileType)
			{
				case FileType.Json:
					return ".json";
				case FileType.SqlExtended:
					return ".sqlx";
				case FileType.Xml:
					return ".xml";
				default:
					return ".sql";
			}
		}

		/// <summary>
		///		Obtiene el tipo seleccionado en el combo
		/// </summary>
		private FileType GetSelectedType()
		{
			return (FileType) (ComboTypes.SelectedID ?? 0);
		}

		/// <summary>
		///		Comprueba los datos introducidos en el formulario
		/// </summary>
		private bool ValidateData()
		{
			bool validated = false;

				// Comprueba los datos
				if (string.IsNullOrWhiteSpace(FileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Introduzca el nombre de archivo");
				else if (System.IO.File.Exists(FullFileName))
					SolutionViewModel.MainViewModel.MainController.HostController.SystemController.ShowMessage("Ya existe este nombre de archivo en el directorio");
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
				RaiseEventClose(true);
		}

		/// <summary>
		///		ViewModel de la solución
		/// </summary>
		public Solutions.SolutionViewModel SolutionViewModel { get; }

		/// <summary>
		///		Directorio
		/// </summary>
		public string Path { get; }

		/// <summary>
		///		Nombre de archivo
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { CheckProperty(ref _fileName, value); }
		}

		/// <summary>
		///		Nombre completo del archivo
		/// </summary>
		public string FullFileName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(FileName))
					return System.IO.Path.Combine(Path, FileName);
				else
					return System.IO.Path.Combine(Path, "New file.sql");
			}
		}

		/// <summary>
		///		Tipos de archivo
		/// </summary>
		public ComboViewModel ComboTypes
		{
			get { return _comboTypes; }
			set { CheckObject(ref _comboTypes, value); }
		}
	}
}
