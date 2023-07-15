namespace Bau.Libraries.LibReporting.Models.DataWarehouses.Reports.Blocks;

/// <summary>
///		Bloque que comprueba si se ha solicitado una dimensión
/// </summary>
public class BlockIfRequest : BaseBlockModel
{
    public BlockIfRequest(string key) : base(key) {}

    /// <summary>
    ///     Añade una serie de dimensiones
    /// </summary>
    public void AddDimensions(string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
            foreach (string part in key.Split(';'))
                if (!string.IsNullOrWhiteSpace(key))
                    DimensionKeys.Add(part.Trim());
    }

    /// <summary>
    ///     Añade una serie de expresiones
    /// </summary>
    public void AddExpressions(string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
            foreach (string part in key.Split(';'))
                if (!string.IsNullOrWhiteSpace(key))
                    ExpressionKeys.Add(part.Trim());
    }

    /// <summary>
    ///		Claves de las dimensiones que se comprueban
    /// </summary>
    public List<string> DimensionKeys { get; } = new();

    /// <summary>
    ///     Claves de las expresiones que se comprueban
    /// </summary>
    public List<string> ExpressionKeys { get; } = new();

    /// <summary>
    ///     Bloques a generar cuando se cumple la condición
    /// </summary>
    public List<BaseBlockModel> Then { get; } = new();

    /// <summary>
    ///     Bloques a generar cuando no se cumple la condición
    /// </summary>
    public List<BaseBlockModel> Else { get; } = new();
}
