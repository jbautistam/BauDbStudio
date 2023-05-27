using Bau.Libraries.DbScripts.Manager.Models;
using Bau.Libraries.DbStudio.Models.Connections;

namespace Bau.Libraries.DbScripts.Manager.Builders;

/// <summary>
///		Generador de <see cref="QueryModel"/>
/// </summary>
public class QueryBuilder
{
	public QueryBuilder(ConnectionModel connection)
	{
		Query = new QueryModel(connection);
	}

	/// <summary>
	///		Asigna la cadena SQL
	/// </summary>
	public QueryBuilder WithSql(string sql, bool mustParse)
	{
		// Asigna la cadena SQL
		Query.Sql = sql;
		Query.ParseQuery = mustParse;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Asigna el tiempo de espera
	/// </summary>
	public QueryBuilder WithTimeout(TimeSpan timeout)
	{
		// Asigna el tiempo de espera
		if (timeout.TotalSeconds > 0)
			Query.Timeout = timeout;
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Asigna la paginación
	/// </summary>
	public QueryBuilder WithPagination(int page, int size)
	{
		// Asigna la paginación
		if (page == 0 || size == 0)
			Query.Pagination.MustPaginate = false;
		else
		{
			Query.Pagination.MustPaginate = true;
			Query.Pagination.Page = page;
			Query.Pagination.PageSize = size;
		}
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una constante a los argumentos
	/// </summary>
	public QueryBuilder WithConstant(string key, object value)
	{
		// Añade una constante a los parámetros
		Query.Arguments.Constants.Add(key, value);
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade un parámetro a los argumentos
	/// </summary>
	public QueryBuilder WithParameter(string key, object value)
	{
		// Añade un parámetro a los argumentos
		Query.Arguments.Parameters.Add(key, value);
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Añade una serie de argumentos
	/// </summary>
	public QueryBuilder WithArguments(ArgumentListModel arguments)
	{
		// Añade las constantes
		foreach ((string key, object value) in arguments.Constants.Enumerate())
			WithConstant(key, value);
		// Añade los parámetros
		foreach ((string key, object value) in arguments.Parameters.Enumerate())
			WithParameter(key, value);
		// Devuelve el generador
		return this;
	}

	/// <summary>
	///		Genera la consulta
	/// </summary>
	public QueryModel Build() => Query;

	/// <summary>
	///		Datos de la consulta que se está generando
	/// </summary>
	private QueryModel Query { get; }
}
