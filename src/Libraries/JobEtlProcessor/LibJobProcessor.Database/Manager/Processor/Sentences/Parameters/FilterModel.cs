using System;

using Bau.Libraries.Compiler.LibInterpreter.Context.Variables;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Sentences.Parameters
{
	/// <summary>
	///		Clase con los datos de un filtro
	/// </summary>
	internal class FilterModel
	{
		/// <summary>
		///		Clona los datos del filtro
		/// </summary>
		internal FilterModel Clone()
		{
			FilterModel target = new FilterModel();

				// Asigna los datos
				target.VariableName = VariableName;
				target.Parameter = Parameter;
				target.ColumnType = ColumnType;
				target.Default = Default;
				// Devuelve el objeto clonado
				return target;
		}

		/// <summary>
		///		Nombre de la variable de la que se recoge el valor
		/// </summary>
		internal string VariableName { get; set; }

		/// <summary>
		///		Nombre del parámetro
		/// </summary>
		internal string Parameter { get; set; }

		/// <summary>
		///		Tipo de datos
		/// </summary>
		internal VariableModel.VariableType ColumnType { get; set; }

		/// <summary>
		///		Valor predeterminado
		/// </summary>
		internal object Default { get; set; }
	}
}
