using System;
using System.Collections.Generic;

using Bau.Libraries.LibPgnReader.Parsers.Tokens;

namespace Bau.Libraries.LibPgnReader.Parsers
{
	/// <summary>
	///		Lector de tokens
	/// </summary>
	internal class TokenParser
	{
		internal TokenParser(System.IO.StreamReader reader)
		{
			CharReader = new CharFileReader(reader);
		}

		/// <summary>
		///		Lee los tokens del archivo
		/// </summary>
		internal IEnumerable<TokenModel> Read()
		{
			string content = string.Empty;
			TokenModel.TokenType tokenReaded = TokenModel.TokenType.Unknown;
			bool endToken = false, specialChar = false;

				// Lee los caracteres y crea los tokens
				foreach ((CharFileReader.CharType type, char character) readed in CharReader.Read())
				{
					// Añade el carácter al contenido y cierra el token si es necesario
					switch (readed.type)
					{
						case CharFileReader.CharType.Character:
								// Lee el carácter
								if (readed.character == ' ' || readed.character == '\t' || readed.character == '\r' || readed.character == '\n')
									endToken = true;
								else // Trata los caracteres especiales
									switch (readed.character)
									{
										case '[':
												tokenReaded = TokenModel.TokenType.StartTag;
											break;
										case ']':
										case '}':
										case ')':
										case '>':
												specialChar = true;
											break;
										case '{':
												tokenReaded = TokenModel.TokenType.StartComment;
											break;
										case ';':
												tokenReaded = TokenModel.TokenType.StartCommentToEoL;
											break;
										case '(':
												tokenReaded = TokenModel.TokenType.StartRecursiveVariation;
											break;
										case '<':
												tokenReaded = TokenModel.TokenType.StartExpansion;
											break;
										case '"':
												specialChar = true;
											break;
										default:
												content += readed.character;
											break;
									}
								// Si se ha leído un carácter especial, se marca el final del token
								if (tokenReaded != TokenModel.TokenType.Unknown)
									endToken = true;
							break;
						case CharFileReader.CharType.EoL:
						case CharFileReader.CharType.EoF:
						case CharFileReader.CharType.EmptyLine:
								specialChar = true;
							break;
					}
					// Devuelve el token
					if (specialChar)
					{
						// Devuelve el token que teníamos hasta ahora
						if (tokenReaded != TokenModel.TokenType.Unknown || !string.IsNullOrEmpty(content))
							yield return new TokenModel(tokenReaded, content);
						// Devuelve un token con el carácter especial
						switch (readed.type)
						{
							case CharFileReader.CharType.EmptyLine:
									yield return new TokenModel(TokenModel.TokenType.EmptyLine, string.Empty);
								break;
							case CharFileReader.CharType.EoL:
									yield return new TokenModel(TokenModel.TokenType.EoL, string.Empty);
								break;
							case CharFileReader.CharType.EoF:
									yield return new TokenModel(TokenModel.TokenType.EoF, string.Empty);
								break;
							default:
									switch (readed.character)
									{
										case '"':
												yield return new TokenModel(TokenModel.TokenType.Quote, string.Empty);
											break;
										case '}':
												yield return new TokenModel(TokenModel.TokenType.EndComment, string.Empty);
											break;
										case ']':
												yield return new TokenModel(TokenModel.TokenType.EndTag, string.Empty);
											break;
										case ')':
												yield return new TokenModel(TokenModel.TokenType.EndRecursiveVariation, string.Empty);
											break;
										case '>':
												yield return new TokenModel(TokenModel.TokenType.EndExpansion, string.Empty);
											break;
									}
								break;
						}
					}
					// Si hemos llegado al final del token, lo devuelve
					if (endToken && (tokenReaded != TokenModel.TokenType.Unknown || !string.IsNullOrEmpty(content)))
						yield return new TokenModel(tokenReaded, content);
					// Inicializa los datos si es necesario
					if (specialChar || endToken)
					{
						tokenReaded = TokenModel.TokenType.Unknown;
						content = string.Empty;
						endToken = false;
						specialChar = false;
					}
				}
		}

		/// <summary>
		///		Lector de caracteres
		/// </summary>
		private CharFileReader CharReader { get; set; }
	}
}