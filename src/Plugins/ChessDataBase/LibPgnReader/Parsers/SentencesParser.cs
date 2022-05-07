using System;
using System.Collections.Generic;

using Bau.Libraries.LibPgnReader.Parsers.Tokens;
using Bau.Libraries.LibPgnReader.Parsers.Sentences;

namespace Bau.Libraries.LibPgnReader.Parsers
{
	/// <summary>
	///		Lector de movimientos
	/// </summary>
	internal class SentencesParser
	{
		internal SentencesParser(System.IO.StreamReader reader)
		{
			TokenParser = new TokenParser(reader);
		}

		/// <summary>
		///		Lee los tokens del archivo
		/// </summary>
		internal IEnumerable<SentenceBaseModel> Read()
		{
			using (IEnumerator<TokenModel> tokenEnumerator = TokenParser.Read().GetEnumerator())
			{
				SentenceBaseModel sentence;

					// Recorre los tokens
					do
					{
						// Lee el movimiento
						sentence = ReadSentence(tokenEnumerator);
						// Devuelve el movimiento
						yield return sentence;
					}
					while (!(sentence is SentenceEndModel) && !(sentence is SentenceErrorModel));
			}
		}

		/// <summary>
		///		Lee un movimiento
		/// </summary>
		private SentenceBaseModel ReadSentence(IEnumerator<TokenModel> tokenEnumerator)
		{
			TokenModel token = GetNextToken(tokenEnumerator);

				// Obtiene el movimiento
				switch (token.Type)
				{
					case TokenModel.TokenType.StartTag:
						return ReadTagSentence(tokenEnumerator);
					case TokenModel.TokenType.StartComment:
						return ReadCommentSentence(tokenEnumerator);
					case TokenModel.TokenType.StartCommentToEoL:
						return ReadCommentToEoLSentence(tokenEnumerator);
					case TokenModel.TokenType.StartRecursiveVariation:
						return new SentenceTurnStartVariationModel();
					case TokenModel.TokenType.EndRecursiveVariation:
						return new SentenceTurnEndVariationModel();
					case TokenModel.TokenType.Unknown:
						return ReadGameSentence(token);
					case TokenModel.TokenType.EmptyLine:
						return new SentenceEmptyLine();
					case TokenModel.TokenType.EoF:
						return new SentenceEndModel();
					default:
						return new SentenceUnknownModel(token);
				}
		}

		/// <summary>
		///		Obtiene el siguiente token
		/// </summary>
		private TokenModel GetNextToken(IEnumerator<TokenModel> tokenEnumerator)
		{
			if (tokenEnumerator.MoveNext())
				return tokenEnumerator.Current;
			else
				return new TokenModel(TokenModel.TokenType.EoF, string.Empty);
		}

		/// <summary>
		///		Obtiene el movimiento con una etiqueta: [Tag "content content"]
		/// </summary>
		private SentenceBaseModel ReadTagSentence(IEnumerator<TokenModel> tokenEnumerator)
		{
			string tag = string.Empty, content = string.Empty;
			TokenModel token = GetNextToken(tokenEnumerator);
			string error = string.Empty;

				// Obtiene el contenido de la etiqueta
				if (token.Type == TokenModel.TokenType.Unknown)
				{
					// Rellena la etiqueta
					tag = token.Content;
					// Obtiene el siguiente token
					token = GetNextToken(tokenEnumerator);
					if (token.Type == TokenModel.TokenType.Quote)
					{
						// Lee el contenido
						do
						{
							// Pasa al siguiente token
							token = GetNextToken(tokenEnumerator);
							// Añade el contenido
							if (token.Type != TokenModel.TokenType.Quote)
								content += token.Content + " ";
						}
						while (token.Type != TokenModel.TokenType.Quote && token.Type != TokenModel.TokenType.EoF);
						// Lee el siguiente token, que debe ser el final de tag
						if (token.Type != TokenModel.TokenType.EoF)
						{
							token = GetNextToken(tokenEnumerator);
							if (token.Type != TokenModel.TokenType.EndTag)
								error = "Cant find the end tag token";
						}
						else
							error = "Found eof before end quote";
					}
					else
						error = "Can't find the quote at tag";
				}
				// Comprueba los errores
				if (string.IsNullOrEmpty(error))
					return new SentenceTagModel(tag, content);
				else
					return new SentenceErrorModel(error);
		}

		/// <summary>
		///		Lee un comentario: {esto es un comentario}
		/// </summary>
		private SentenceBaseModel ReadCommentSentence(IEnumerator<TokenModel> tokenEnumerator)
		{
			string content = string.Empty;
			TokenModel token;

				// Obtiene el contenido del comentarios
				do
				{
					// Pasa al siguiente token
					token = GetNextToken(tokenEnumerator);
					// Añade el contendiod
					if (token.Type != TokenModel.TokenType.EndComment)
						content += token.Content + " ";
				}
				while (token.Type != TokenModel.TokenType.EndComment && token.Type != TokenModel.TokenType.EoF);
				// Comprueba que se haya leído el fin de comentarios
				if (token.Type == TokenModel.TokenType.EndComment)
					return new SentenceCommentModel(content);
				else
					return new SentenceErrorModel("Can't find end comment character");
		}

		/// <summary>
		///		Lee un comentario hasta el fin de línea: ; esto es un comentario
		/// </summary>
		private SentenceBaseModel ReadCommentToEoLSentence(IEnumerator<TokenModel> tokenEnumerator)
		{
			string content = string.Empty;
			TokenModel token;

				// Obtiene el contenido del comentarios
				do
				{
					// Pasa al siguiente token
					token = GetNextToken(tokenEnumerator);
					// Añade el contendiod
					if (token.Type != TokenModel.TokenType.EoL)
						content += token.Content + " ";
				}
				while (token.Type != TokenModel.TokenType.EoL && token.Type != TokenModel.TokenType.EoF);
				// Comprueba que se haya leído el fin de comentarios
				if (token.Type == TokenModel.TokenType.EoL || token.Type == TokenModel.TokenType.EoF)
					return new SentenceCommentModel(content);
				else
					return new SentenceErrorModel("Can't find end comment character");
		}

		/// <summary>
		///		Obtiene un movimiento de juego
		/// </summary>
		private SentenceBaseModel ReadGameSentence(TokenModel token)
		{
			if (!string.IsNullOrEmpty(token.Content))
			{
				string content = token.Content.Trim();

					// Obtiene el movimiento adecuado
					if (content.EndsWith("."))
					{
						if (content.Length > 1)
							return new SentenceTurnNumberModel(token.Content.Substring(0, token.Content.Length - 1));
						else
							return new SentenceErrorModel($"Illegal turn number: {token.Content}");
					}
					else if (IsEndSentence(content))
						return new SentenceTurnResultModel(token.Content);
					else if (content.StartsWith("$"))
					{
						if (content.Length > 1)
							return new SentenceTurnInformationModel(token.Content.Substring(1));
						else
							return new SentenceErrorModel($"Illegal information data: {token.Content}");
					}
					else
						return new SentenceTurnPlayModel(token.Content);
			}
			else
				return new SentenceErrorModel("Error when read Sentence");
		}

		/// <summary>
		///		Comprueba si es un movimiento de fin de juego
		/// </summary>
		private bool IsEndSentence(string content)
		{
			return content.Equals("1-0") || content.Equals("0-1") || content.Equals("1/2-1/2") || content.Equals("*");
		}

		/// <summary>
		///		Lector de tokens
		/// </summary>
		private TokenParser TokenParser { get; set; }
	}
}