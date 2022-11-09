using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Queries
{
	/// <summary>
	///		ViewModel para ejecución de una consulta
	/// </summary>
	public class ExecuteQueryViewModel : BaseObservableObject, PluginsStudio.ViewModels.Base.Interfaces.IDetailViewModel
	{
		// Variables privadas
		private string _header;

		public ExecuteQueryViewModel(DbStudioViewModel solutionViewModel, string selectedConnection, string query, bool executeQueryByParts) : base(false)
		{
			// Asigna los viewModel
			SolutionViewModel = solutionViewModel;
			QueryViewModel = new QueryViewModel(solutionViewModel, selectedConnection, query, executeQueryByParts, false);
			// Asigna las propiedades
			Header = "Consulta";
		}

		/// <summary>
		///		Graba el archivo
		/// </summary>
		public void SaveDetails(bool newName)
		{
			// Graba el archivo
			QueryViewModel.SaveDetails(newName);
			// Graba el archivo
			if (string.IsNullOrWhiteSpace(QueryViewModel.FileName))
				Header = System.IO.Path.GetFileName(QueryViewModel.FileName);
		}

		/// <summary>
		///		Obtiene el mensaje que se debe mostrar al cerrar la ventana
		/// </summary>
		public string GetSaveAndCloseMessage()
		{
			return "¿Desea grabar la consulta antes de continuar?";
		}

		/// <summary>
		///		Cierra el viewmodel
		/// </summary>
		public void Close()
		{
			// ... no hace nada, sólo implementa la interface
		}

		/// <summary>
		///		Solución
		/// </summary>
		public DbStudioViewModel SolutionViewModel { get; }

		/// <summary>
		///		ViewModel para la consulta
		/// </summary>
		public QueryViewModel QueryViewModel { get; }

		/// <summary>
		///		Cabecera
		/// </summary>
		public string Header 
		{
			get { return _header; }
			set { CheckProperty(ref _header, value); }
		}

		/// <summary>
		///		Id de la ficha
		/// </summary>
		public string TabId 
		{ 
			get { return GetType().ToString() + "_" + Guid.NewGuid().ToString(); } 
		}
	}
}