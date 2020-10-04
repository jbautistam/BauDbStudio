using System;

using Bau.Libraries.BauMvvm.ViewModels;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Queries
{
	/// <summary>
	///		ViewModel para mostrar una lista de series
	/// </summary>
	public class ChartSeriesViewModel : BaseObservableObject
	{
		// Variables privadas
		private BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ChartSeriesItemViewModel> _series;

		public ChartSeriesViewModel(ChartViewModel chartViewModel) : base(false)
		{
			// Asigna las propiedades
			ChartViewModel = chartViewModel;
			// Crea la lista de series
			Series = new BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ChartSeriesItemViewModel>();
		}

		/// <summary>
		///		Limpia la lista de series
		/// </summary>
		internal void Clear()
		{
			Series.Clear();
		}

		/// <summary>
		///		Añade una serie
		/// </summary>
		internal void AddSerie(string serie)
		{
			Series.Add(new ChartSeriesItemViewModel(serie, serie));
		}

		/// <summary>
		///		ViewModel padre
		/// </summary>
		public ChartViewModel ChartViewModel { get; }

		/// <summary>
		///		Lista de series a ejecutar
		/// </summary>
		public BauMvvm.ViewModels.Forms.ControlItems.ControlItemCollectionViewModel<ChartSeriesItemViewModel> Series
		{
			get { return _series; }
			set { CheckObject(ref _series, value); }
		}
	}
}