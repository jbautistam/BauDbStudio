using System;

namespace Bau.Libraries.LibPgnReader.Parsers.Tokens
{
	/// <summary>
	///		Modelo con los datos de un token
	/// </summary>
	internal class TokenModel
	{
		/// <summary>
		///		Tipo de token
		/// </summary>
		internal enum TokenType
		{
			/// <summary>Token desconocido</summary>
			Unknown,
			/// <summary>Fin de línea</summary>
			EoL,
			/// <summary>Fin de archivo</summary>
			EoF,
			/// <summary>Inicio de etiqueta</summary>
			StartTag,
			/// <summary>Fin de etiqueta</summary>
			EndTag,
			/// <summary>Inicio de comentario</summary>
			StartComment,
			/// <summary>Fin de comentario</summary>
			EndComment,
			/// <summary>Inicio de comentario hasta fin de línea</summary>
			StartCommentToEoL,
			/// <summary>Comillas</summary>
			Quote,
			/// <summary>Identificador de etiqueta</summary>
			IdentifierTag,
			/// <summary>Contenido de etiqueta</summary>
			ContentTag,
			/// <summary>Comienzo de una variación</summary>
			StartRecursiveVariation,
			/// <summary>Final de una variación</summary>
			EndRecursiveVariation,
			/// <summary>Comienzo de una cadena de expansión</summary>
			StartExpansion,
			/// <summary>Fin de una cadena de expansión</summary>
			EndExpansion,
			/// <summary>Línea vacía</summary>
			EmptyLine
		}

		internal TokenModel(TokenType type, string content)
		{
			Type = type;
			Content = content;
		}

		/// <summary>
		///		Tipo de token
		/// </summary>
		internal TokenType Type { get; }

		/// <summary>
		///		Contenido del token
		/// </summary>
		internal string Content { get; }
	}
}
