using System;
using System.Collections.Generic;

using Bau.Libraries.JobsProcessor.ViewModel;
using Bau.Libraries.PluginsStudio.ViewModels.Base.Models;
using Bau.Libraries.PluginsStudio.Views.Base.Interfaces;
using Bau.Libraries.PluginsStudio.Views.Base.Models;

namespace Bau.Libraries.JobsProcessor.Plugin
{
	/// <summary>
	///		Plugin para el procesador de consolas
	/// </summary>
	public class JobsProcessorPlugin : IPlugin
	{ 
		/// <summary>
		///		Inicializa el manager de vistas del lector de cómics
		/// </summary>
		public void Initialize(IAppViewsController appViewsController, PluginsStudio.ViewModels.Base.Controllers.IPluginsController pluginController)
		{
			AppViewsController = appViewsController;
			MainViewModel = new JobsProcessorViewModel(new Controllers.JobsProcessorController(this, pluginController));
			MainViewModel.Initialize();
		}

		/// <summary>
		///		Carga los datos del directorio
		/// </summary>
		public void Load(string path)
		{
			MainViewModel.Load(path);
		}

		/// <summary>
		///		Actualiza los exploradores y ventanas
		/// </summary>
		public void Refresh()
		{
			MainViewModel.Load(string.Empty);
		}

		/// <summary>
		///		Intenta abrir un archivo en un plugin
		/// </summary>
		public bool OpenFile(string fileName)
		{
			return MainViewModel.OpenFile(fileName);
		}

		/// <summary>
		///		Obtiene los paneles del plugin
		/// </summary>
		public List<PaneModel> GetPanes()
		{
			return new();
		}

		/// <summary>
		///		Obtiene las barras de herramientas del plugin
		/// </summary>
		public List<ToolBarModel> GetToolBars()
		{
			return new();
		}

		/// <summary>
		///		Obtiene los menús del plugin
		/// </summary>
		public List<MenuListModel> GetMenus()
		{
			return new();
		}

		/// <summary>
		///		Obtiene las opciones de menú asociadas a las extensiones de archivo y carpetas
		/// </summary>
		public List<FileOptionsModel> GetFilesOptions()
		{
			return new PluginsStudio.ViewModels.Base.Models.Builders.FileOptionsBuilder()
								.WithOption()
									.WithExtension("cmd.xml")
									.WithMenu(new MenuModel
														{
															Header = "Ejecutar",
															Icon = GetIconName("ArrowNext.png"),
															Command = MainViewModel.ExecuteFileCommand
														}
											 )
								.Build();
		}

		/// <summary>
		///		Obtiene las extensiones de archivo asociadas al plugin
		/// </summary>
		public List<PluginsStudio.ViewModels.Base.Models.FileAssignedModel> GetFilesAssigned()
		{
			List<PluginsStudio.ViewModels.Base.Models.FileAssignedModel> files = new List<FileAssignedModel>();

				// Asigna las extensiones
				files.Add(GetIcon(".cmd.xml", "FileBat.png"));
				// Devuelve la lista de archivos asignados
				return files;

				FileAssignedModel GetIcon(string extension, string name)
				{
					return new FileAssignedModel
									{
										Name = "Context command file",
										FileExtension = extension,
										Icon = GetIconName(name),
										Template = GetTemplateCmdXml()
									};
				}
		}

		/// <summary>
		///		Obtiene la plantilla del archivo
		/// </summary>
		private string GetTemplateCmdXml()
		{
			return @"
<?xml version=##1.0## encoding=##utf-8## ?>
<Project>
	<Contexts>
		<Context>
			<Parameter Name=##ParameterName## Value = ##ParameterValue## />
		</Context>
	</Contexts>
	<Commands>
		<Command FileName = ##Executable##>			
			<Argument Type=##string## Name=##Argument1## Value=##{{ParameterName}}##/>
			<Argument Type=##int## Name=##Argument2## Value=##40##/>
			<Environment Type=##json## Name=##Parameters##>
				<![CDATA[
					{
						##Argument3##: ##{{ParameterName}}##,
						##Argument4##: ##{{ParameterName}}##
					}
				]]>
			</Environment>
		</Command>
	</Commands>
</Project>"
				.Replace("##", "\"").Trim();
		}

		/// <summary>
		///		Obtiene un nombre de recurso
		/// </summary>
		private string GetIconName(string name)
		{
			return $"/JobsProcessor.Plugin;component/Resources/{name}";
		}

		/// <summary>
		///		Obtiene la vista de configuración del plugin
		/// </summary>
		public IPluginConfigurationView GetConfigurationView()
		{
			return null;
		}

		/// <summary>
		///		Controlador de aplicación
		/// </summary>
		internal IAppViewsController AppViewsController { get; private set; }

		/// <summary>
		///		ViewModel principal
		/// </summary>
		public JobsProcessorViewModel MainViewModel { get; private set; }
	}
}