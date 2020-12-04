using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Bau.Libraries.BauMvvm.ViewModels;
using Bau.Libraries.BauMvvm.ViewModels.Forms.ControlItems.ComboItems;
using Bau.Libraries.LibCharts.Models;

namespace Bau.Libraries.DbStudio.ViewModels.Solutions.Details.Queries
{
	/// <summary>
	///		ViewModel para visualización de un gráfico
	/// </summary>
	public class ChartViewModel : BaseObservableObject
	{
		// Enumerados públicos
		/// <summary>
		///		Tipo de gráfico
		/// </summary>
		public enum ChartType
		{
			Bar,
			StackedBar,
			Line,
			Area,
			Pie,
			Radar
		}
		/// <summary>
		///		Tipo de orientación
		/// </summary>
		public enum ChartOrientationType
		{
			Horizontal,
			Vertical
		}
		/// <summary>
		///		Ubicación de la leyenda
		/// </summary>
		public enum LegendLocationType
		{
			None,
			LowerCenter,
			LowerLeft,
			LowerRight,
			MiddleLeft,
			MiddleRight,
			UpperCenter,
			UpperLeft,
			UpperRight,
		}

		// Eventos públicos
		public event EventHandler ChartPrepared;
		// Variables privadas
		private bool _canDraw = false;
		private ChartModel _chart;
		private ComboViewModel _comboChartTypes, _comboOrientationTypes, _comboLegend;
		private ComboViewModel _comboFieldSeries;
		private ChartSeriesViewModel _seriesViewModel;

		public ChartViewModel(QueryViewModel queryViewModel) : base(false)
		{
			// Asigna las propiedades
			QueryViewModel = queryViewModel;
			// Carga los datos
			LoadComboChartTypes();
			LoadComboChartOrientation();
			LoadComboLegend();
			// Inicializa los comandos
			DrawCommand = new BaseCommand(_ => Draw(), _ => CanDraw)
									.AddListener(this, nameof(CanDraw));
		}

		/// <summary>
		///		Carga el combo de tipos de gráficos
		/// </summary>
		private void LoadComboChartTypes()
		{
			// Crea el combo
			ComboChartTypes = new ComboViewModel(this);	
			// Añade los datos
			ComboChartTypes.AddItem((int) ChartType.Bar, "Barra");
			ComboChartTypes.AddItem((int) ChartType.StackedBar, "Barra apilada");
			ComboChartTypes.AddItem((int) ChartType.Line, "Línea");
			ComboChartTypes.AddItem((int) ChartType.Area, "Area");
			ComboChartTypes.AddItem((int) ChartType.Pie, "Tarta");
			ComboChartTypes.AddItem((int) ChartType.Radar, "Radar");
			// Selecciona el primer elemento
			ComboChartTypes.SelectedId = (int) ChartType.Bar;
		}

		/// <summary>
		///		Carga el combo de tipos orientación
		/// </summary>
		private void LoadComboChartOrientation()
		{
			// Crea el combo
			ComboOrientationTypes = new ComboViewModel(this);	
			// Añade los datos
			ComboOrientationTypes.AddItem((int) ChartOrientationType.Vertical, "Vertical");
			ComboOrientationTypes.AddItem((int) ChartOrientationType.Horizontal, "Horizontal");
			// Selecciona el primer elemento
			ComboOrientationTypes.SelectedId = (int) ChartOrientationType.Vertical;
		}

		/// <summary>
		///		Carga el combo de posición de la leyenda
		/// </summary>
		private void LoadComboLegend()
		{
			// Crea el combo
			ComboLegend = new ComboViewModel(this);	
			// Añade los datos
			ComboLegend.AddItem((int) LegendLocationType.None, "Sin leyenda");
			ComboLegend.AddItem((int) LegendLocationType.LowerCenter, "Inferior - central");
			ComboLegend.AddItem((int) LegendLocationType.LowerLeft, "Inferior izquierda");
			ComboLegend.AddItem((int) LegendLocationType.LowerRight, "Inferior derecha");
			ComboLegend.AddItem((int) LegendLocationType.MiddleLeft, "Central izquierda");
			ComboLegend.AddItem((int) LegendLocationType.MiddleRight, "Central derecha");
			ComboLegend.AddItem((int) LegendLocationType.UpperCenter, "Superior central");
			ComboLegend.AddItem((int) LegendLocationType.UpperLeft, "Superior izquierda");
			ComboLegend.AddItem((int) LegendLocationType.UpperRight, "Superior derecha");
			// Selecciona el primer elemento
			ComboLegend.SelectedId = (int) LegendLocationType.None;
		}

		/// <summary>
		///		Prepara las series
		/// </summary>
		internal void PrepareSeries()
		{
			// Inicializa los datos
			ComboFieldSeries = new ComboViewModel(this);
			SeriesViewModel = new ChartSeriesViewModel(this);
			// Carga los combos de los campos
			if (QueryViewModel.DataResults != null)
				foreach (DataColumn field in QueryViewModel.DataResults.Columns)
					if (IsNumeric(field))
						SeriesViewModel.AddSerie(field.ColumnName);
					else
						ComboFieldSeries.AddItem(0, field.ColumnName);
			// Selecciona el primer elemento
			if (ComboFieldSeries.Items.Count > 0)
				ComboFieldSeries.SelectedItem = ComboFieldSeries.Items[0];
			// Indica que se puede dibujar
			CanDraw = true;
		}

		/// <summary>
		///		Comprueba si un campo es numérico
		/// </summary>
		private bool IsNumeric(DataColumn field)
		{
			return field.DataType == typeof(double) || field.DataType == typeof(float) || field.DataType == typeof(int) || field.DataType == typeof(Int64); 
		}

		/// <summary>
		///		Dibuja el gráfico
		/// </summary>
		private void Draw()
		{
			List<string> labels = GetLabels(ComboFieldSeries.SelectedText);

				// Crea el gráfico
				Chart = new ChartModel();
				// Asigna las propiedades
				Chart.Title = "Gráfico";
				Chart.LegendLocation = ConvertLegend(GetSelectedLegend());
				Chart.Type = ConvertType(GetSelectedChartType());
				Chart.Orientation = ConvertOrientation(GetSelectedChartOrientation());
				Chart.Labels.AddRange(labels);
				// Añade las series
				foreach (ChartSeriesItemViewModel item in SeriesViewModel.Series)
					if (item.IsChecked)
						Chart.Series.Add(CreateSerie(labels, ComboFieldSeries.SelectedText, item.Text));
				// Lanza el evento de dibujo
				ChartPrepared?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		///		Obtiene el tipo de leyenda seleccionada en el combo
		/// </summary>
		private LegendLocationType GetSelectedLegend()
		{
			return (LegendLocationType) (ComboLegend.SelectedId ?? (int) LegendLocationType.None);
		}

		/// <summary>
		///		Convierte la leyenda seleccionada
		/// </summary>
		private ChartModel.LegendLocationType ConvertLegend(LegendLocationType location)
		{
			switch (location)
			{
				case LegendLocationType.LowerCenter:
					return ChartModel.LegendLocationType.LowerCenter;
				case LegendLocationType.LowerLeft:
					return ChartModel.LegendLocationType.LowerLeft;
				case LegendLocationType.LowerRight:
					return ChartModel.LegendLocationType.LowerRight;
				case LegendLocationType.MiddleLeft:
					return ChartModel.LegendLocationType.MiddleLeft;
				case LegendLocationType.MiddleRight:
					return ChartModel.LegendLocationType.MiddleRight;
				case LegendLocationType.UpperCenter:
					return ChartModel.LegendLocationType.UpperCenter;
				case LegendLocationType.UpperLeft:
					return ChartModel.LegendLocationType.UpperLeft;
				case LegendLocationType.UpperRight:
					return ChartModel.LegendLocationType.UpperRight;
				default:
					return ChartModel.LegendLocationType.None;
			}
		}

		/// <summary>
		///		Obtiene las etiquetas de una serie
		/// </summary>
		private List<string> GetLabels(string field)
		{
			List<string> labels = new List<string>();

				// Obtiene las etiquetas
				if (!string.IsNullOrWhiteSpace(field))
				{
					// Obtiene las etiquetas de un campo
					foreach (DataRow row in QueryViewModel.DataResults.Rows)
					{
						string label = GetString(row[field]);

							// Añade la etiqueta si no existía
							if (labels.FirstOrDefault(item => item.Equals(label, StringComparison.CurrentCultureIgnoreCase)) == null)
								labels.Add(label);
					}
					// Ordena las etiquetas
					labels.Sort((first, second) => first.CompareTo(second));
				}
				// Devuelve la colección de etiquetas
				return labels;
		}

		/// <summary>
		///		Crea una serie
		/// </summary>
		private ChartSerieModel CreateSerie(List<string> labels, string nameField, string valueField)
		{
			Dictionary<string, double> values = new Dictionary<string, double>();
			ChartSerieModel serie = new ChartSerieModel();

				// Inicializa el diccionario
				foreach (string label in labels)
					values.Add(label, 0);
				// Asigna las propiedades
				serie.Name = valueField;
				// Añade los elementos al diccionario
				foreach (DataRow row in QueryViewModel.DataResults.Rows)
				{
					string label = GetString(row[nameField]);

						// Añade el valor al diccionario
						values[label] += GetDouble(row[valueField]);
				}
				// Añade los elementos a la serie
				for (int index = 0; index < labels.Count; index++)
					serie.Items.Add(new ChartSeriePointModel
												{
													X = index,
													Y = values[labels[index]]
												}
								   );
				// Devuelve la serie
				return serie;
		}

		/// <summary>
		///		Obtiene una cadena del valor de un campo
		/// </summary>
		private string GetString(object value)
		{
			if (value == null || value is DBNull)
				return string.Empty;
			else
				return value.ToString();
		}

		/// <summary>
		///		Obtiene un valor numérico de un campo
		/// </summary>
		private double GetDouble(object value)
		{
			if (value == null || value is DBNull)
				return 0;
			else if (!double.TryParse(value.ToString(), out double result))
				return 0;
			else
				return result;
		}

		/// <summary>
		///		Convierte el tipo de gráfico
		/// </summary>
		private ChartModel.ChartType ConvertType(ChartType chartType)
		{
			switch (chartType)
			{
				case ChartType.Bar:
					return ChartModel.ChartType.Bars;
				case ChartType.Line:
					return ChartModel.ChartType.Lines;
				case ChartType.Area:
					return ChartModel.ChartType.Areas;
				case ChartType.StackedBar:
					return ChartModel.ChartType.StackedBar;
				case ChartType.Pie:
					return ChartModel.ChartType.Pie;
				case ChartType.Radar:
					return ChartModel.ChartType.Radar;
				default:
					return ChartModel.ChartType.Bars;
			}
		}

		/// <summary>
		///		Convierte la orientación
		/// </summary>
		private ChartModel.OrientationType ConvertOrientation(ChartOrientationType orientation)
		{
			switch (orientation)
			{
				case ChartOrientationType.Horizontal:
					return ChartModel.OrientationType.Horizontal;
				default:
					return ChartModel.OrientationType.Vertical;
			}
		}
		/// <summary>
		///		Obtiene el tipo de gráfico seleccionado
		/// </summary>
		private ChartType GetSelectedChartType()
		{
			return (ChartType) (ComboChartTypes.SelectedId ?? (int) ChartType.Bar);
		}

		/// <summary>
		///		Obtiene la orientación seleccionada
		/// </summary>
		private ChartOrientationType GetSelectedChartOrientation()
		{
			return (ChartOrientationType) (ComboOrientationTypes.SelectedId ?? (int) ChartOrientationType.Vertical);
		}

		/// <summary>
		///		ViewModel de consulta
		/// </summary>
		public QueryViewModel QueryViewModel { get; }

		/// <summary>
		///		ViewModel del combo de tipo de gráficos
		/// </summary>
		public ComboViewModel ComboChartTypes
		{
			get { return _comboChartTypes; }
			set { CheckObject(ref _comboChartTypes, value); }
		}

		/// <summary>
		///		ViewModel del combo de orientación
		/// </summary>
		public ComboViewModel ComboOrientationTypes
		{
			get { return _comboOrientationTypes; }
			set { CheckObject(ref _comboOrientationTypes, value); }
		}

		/// <summary>
		///		ViewModel del combo de leyenda
		/// </summary>
		public ComboViewModel ComboLegend
		{
			get { return _comboLegend; }
			set { CheckObject(ref _comboLegend, value); }
		}

		/// <summary>
		///		ViewModel del combo con los campos para las series
		/// </summary>
		public ComboViewModel ComboFieldSeries
		{
			get { return _comboFieldSeries; }
			set { CheckObject(ref _comboFieldSeries, value); }
		}

		/// <summary>
		///		Indica si se puede dibujar
		/// </summary>
		public bool CanDraw
		{
			get { return _canDraw; }
			set { CheckProperty(ref _canDraw, value); }
		}

		/// <summary>
		///		ViewModel con la lista de series
		/// </summary>
		public ChartSeriesViewModel SeriesViewModel
		{
			get { return _seriesViewModel; }
			set { CheckObject(ref _seriesViewModel, value); }
		}

		/// <summary>
		///		Gráfico
		/// </summary>
		public ChartModel Chart
		{
			get { return _chart; }
			set { CheckObject(ref _chart, value); }
		}

		/// <summary>
		///		Comando de dibujo
		/// </summary>
		public BaseCommand DrawCommand { get; }
	}
}