using System.Collections.ObjectModel;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.LibReporting.Models.DataWarehouses.DataSets;
using Bau.Libraries.LibReporting.Models.DataWarehouses.Relations;

namespace Bau.Libraries.DbStudio.ViewModels.Details.Reporting.Relations;

/// <summary>
///		Arbol de <see cref="DimensionRelationModel"/>
/// </summary>
public class ListRelationViewModel : BaseObservableObject
{
	// Variables privadas
	private ObservableCollection<DimensionRelationViewModel> _listRelations = default!;
	private DimensionRelationViewModel? _selectedItem;

	public ListRelationViewModel(ReportingSolutionViewModel reportingSolutionViewModel, BaseDataSourceModel dataSource, List<DimensionRelationModel> relations)
	{
		// Asigna las propiedades
		ReportingSolutionViewModel = reportingSolutionViewModel;
		DataSource = dataSource;
		Relations = relations;
		// Prepara la lista
		ListRelations = new ObservableCollection<DimensionRelationViewModel>();
		// Asigna los comandos
		NewRelationCommand = new BaseCommand(_ => OpenRelation(null));
		OpenRelationCommand = new BaseCommand(_ => OpenRelation(SelectedItem), _ => SelectedItem != null)
									.AddListener(this, nameof(SelectedItem));
		DeleteRelationCommand = new BaseCommand(_ => DeleteRelation(), _ => SelectedItem != null)
									.AddListener(this, nameof(SelectedItem));
	}

	/// <summary>
	///		Carga las relaciones
	/// </summary>
	internal void Load()
	{
		// Limpia la lista
		ListRelations.Clear();
		// Añade las relaciones
		foreach (DimensionRelationModel relation in Relations)
			ListRelations.Add(new DimensionRelationViewModel(ReportingSolutionViewModel, DataSource, relation));
		// Indica que no ha habido modificaciones
		IsUpdated = false;
	}

	/// <summary>
	///		Abre el cuadro de diálogo de una relación
	/// </summary>
	private void OpenRelation(DimensionRelationViewModel? viewModel)
	{
		bool isNew = viewModel == null;

			// Crea el objeto si no existía
			if (viewModel is null)
				viewModel = new DimensionRelationViewModel(ReportingSolutionViewModel, DataSource, null);
			// Abre el cuadro de diálogo
			if (ReportingSolutionViewModel.SolutionViewModel.MainController.OpenDialog(viewModel) == BauMvvm.ViewModels.Controllers.SystemControllerEnums.ResultType.Yes)
			{
				// Si es nuevo, la añade
				if (isNew)
					ListRelations.Add(viewModel);
				// Indica que se ha modificado
				IsUpdated = true;
			}
	}

	/// <summary>
	///		Borra una relación
	/// </summary>
	private void DeleteRelation()
	{
		if (SelectedItem != null && ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowQuestion("¿Desea eliminar esta relación?"))
		{
			// Borra la relación
			ListRelations.Remove(SelectedItem);
			// Indica que se ha modificado
			IsUpdated = true;
		}
	}

	/// <summary>
	///		Comprueba los datos
	/// </summary>
	internal bool ValidateData()
	{
		bool validated = false;

			// Comprueba los datos
			if (ListRelations.Count == 0)
				ReportingSolutionViewModel.SolutionViewModel.MainController.SystemController.ShowMessage("No se ha definido ninguna dimensión");
			else
			{
				// Supone que los datos son correctos
				validated = true;
				// Valida las relaciones
				foreach (DimensionRelationViewModel relation in ListRelations)
					if (!relation.ValidateData())
						validated = false;
			}
			// Devuelve el valor que indica si los datos son correctos
			return validated;
	}

	/// <summary>
	///		Obtiene las relaciones
	/// </summary>
	internal List<DimensionRelationModel> GetRelations()
	{
		List<DimensionRelationModel> relations = new List<DimensionRelationModel>();

			// Añade las relaciones
			foreach (DimensionRelationViewModel dimensionRelation in ListRelations)
			{
				DimensionRelationModel relation = new DimensionRelationModel(DataSource.DataWarehouse);

					// Asigna las propiedades
					relation.DimensionId = dimensionRelation.GetDimension().Id;
					// Asigna las claves foráneas
					foreach (ListItemForeignKeyViewModel foreignKeyViewModel in dimensionRelation.ForeignKeys)
					{
						DataSourceColumnModel? relatedColumn = foreignKeyViewModel.GetRelatedColumn();

							if (relatedColumn != null)
								relation.ForeignKeys.Add(new RelationForeignKey
																	{
																		ColumnId = foreignKeyViewModel.SourceColumn.Id,
																		TargetColumnId = relatedColumn.Id
																	}
														);
					}
					// Añade la relación a la colección
					relations.Add(relation);
			}
			// Devuelve las relaciones
			return relations;
	}

	/// <summary>
	///		Solución
	/// </summary>
	public ReportingSolutionViewModel ReportingSolutionViewModel { get; }

	/// <summary>
	///		Origen de datos en el que se definen las relaciones
	/// </summary>
	public BaseDataSourceModel DataSource { get; }

	/// <summary>
	///		Lista de relaciones
	/// </summary>
	public List<DimensionRelationModel> Relations { get; }

	/// <summary>
	///		Relaciones
	/// </summary>
	public ObservableCollection<DimensionRelationViewModel> ListRelations 
	{
		get { return _listRelations; }
		set { CheckObject(ref _listRelations, value); }
	}

	/// <summary>
	///		Elemento seleccionado
	/// </summary>
	public DimensionRelationViewModel? SelectedItem
	{
		get { return _selectedItem; }
		set { CheckObject(ref _selectedItem, value); }
	}

	/// <summary>
	///		Comando para crear una nueva relación
	/// </summary>
	public BaseCommand NewRelationCommand { get; }

	/// <summary>
	///		Comando para abrir una relación
	/// </summary>
	public BaseCommand OpenRelationCommand { get; }

	/// <summary>
	///		Comando para borrar una relación
	/// </summary>
	public BaseCommand DeleteRelationCommand { get; }
}