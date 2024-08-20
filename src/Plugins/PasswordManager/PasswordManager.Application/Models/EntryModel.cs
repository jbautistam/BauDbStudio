namespace Bau.Libraries.PasswordManager.Application.Models;

/// <summary>
///		Clase con los datos de una entrada de contraseña
/// </summary>
public class EntryModel : LibDataStructures.Base.BaseExtendedModel
{ 
	/// <summary>
	///		Codificación de la clave
	/// </summary>
	public enum SecretEncoding
	{
		/// <summary>Texto plano</summary>
		Plain,
		/// <summary>Texto en Base32</summary>
		Base32,
		/// <summary>Texto en Base64</summary>
		Base64
	}
    /// <summary>
    ///     Algoritmo de Hash a utilizar para obtener el HMAC
    /// </summary>
    public enum HmacAlgorithm
    {
        /// <summary>Algoritmo Sha1</summary>
        Sha1,
        /// <summary>Algoritmo Sha256</summary>
        Sha256,
        /// <summary>Algoritmo Sha512</summary>
        Sha512
    }

	/// <summary>
	///		Url
	/// </summary>
	public string Url { get; set; } = default!;

	/// <summary>
	///		Notas
	/// </summary>
	public string Notes { get; set; } = default!;

	/// <summary>
	///		Usuario
	/// </summary>
	public string User { get; set; } = default!;

	/// <summary>
	///		Contraseña
	/// </summary>
	public string Password { get; set; } = default!;

    /// <summary>
    ///		Clave del servidor
    /// </summary>
    public string? AuthKey { get; set; }

    /// <summary>
    ///		Codificación de la clave
    /// </summary>
    public SecretEncoding Encoding { get; set; } = SecretEncoding.Plain;

    /// <summary>
    ///		Algoritmo
    /// </summary>
    public HmacAlgorithm HashAlgorithm { get; set; } = HmacAlgorithm.Sha1;

    /// <summary>
    ///		Dígitos
    /// </summary>
    public int Digits { get; set; } = 6;

    /// <summary>
    ///		Contador
    /// </summary>
    public long Counter { get; set; } = 1;

    /// <summary>
    ///		Intervalo (en segundos)
    /// </summary>
    public int Interval { get; set; } = 30;

	/// <summary>
	///		Fecha de creación
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.Now;
}