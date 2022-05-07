using System;
using System.Collections.Generic;

using Bau.Libraries.LibPgnReader.Parsers.Sentences;
using Bau.Libraries.LibPgnReader.Models;
using Bau.Libraries.LibPgnReader.Models.Movements;

namespace Bau.Libraries.LibPgnReader
{
	/// <summary>
	///		Parser de un archivo PGN para obtener una lista de juegos
	/// </summary>
	public class GamePgnParser
	{
		/// <summary>
		///		Lee los juegos del archivo
		/// </summary>
		public LibraryPgnModel Read(string fileName, System.Text.Encoding encoding = null)
		{
			using (System.IO.StreamReader reader = new System.IO.StreamReader(fileName))
			{
				return Read(reader);
			}
		}

		/// <summary>
		///		Lee los juegos del archivo
		/// </summary>
		public LibraryPgnModel Read(System.IO.StreamReader reader)
		{
			LibraryPgnModel library = new LibraryPgnModel();
			Parsers.SentencesParser parser = new Parsers.SentencesParser(reader);

				// Recoge la lista de juegos
				using (IEnumerator<SentenceBaseModel> sentenceEnumerator = parser.Read().GetEnumerator())
				{
					SentenceBaseModel lastSentence = null;

						// Lee los juegos
						do
						{
							lastSentence = ReadGame(sentenceEnumerator, library);
						}
						while (!(lastSentence is SentenceEndModel) && !(lastSentence is SentenceErrorModel));
						// Añade el error a la librería
						if (lastSentence != null && lastSentence is SentenceErrorModel error)
							library.Errors.Add(error.Content);
				}
				// Ajusta las jugadas
				foreach (GamePgnModel game in library.Games)
					AdjustTurnPlay(game.Movements);
				// Devuelve la librería
				return library;
		}

		/// <summary>
		///		Lee los datos del juego
		/// </summary>
		private SentenceBaseModel ReadGame(IEnumerator<SentenceBaseModel> sentenceEnumerator, LibraryPgnModel library)
		{
			SentenceBaseModel lastSentence = null;

				// Lee el juego
				do
				{
					GamePgnModel game = new GamePgnModel();

						// Guarda la etiqueta leída como última sentencia de la partida anterior
						if (lastSentence != null && lastSentence is SentenceTagModel sentenceTag)
							game.Headers.Add(new HeaderPgnModel(sentenceTag.Tag, sentenceTag.Content));
						// Lee las cabeceras
						lastSentence = ReadHeaders(sentenceEnumerator, game);
						// Si es un salto de línea, obtiene la siguiente sentencia
						if (lastSentence is SentenceEmptyLine)
							lastSentence = GetNextSentence(sentenceEnumerator);
						// Si es un comentario, lee el comentario
						if (lastSentence is SentenceCommentModel)
							lastSentence = ReadComments(lastSentence, sentenceEnumerator, game);
						// Si es un salto de línea, obtiene la siguiente sentencia
						if (lastSentence is SentenceEmptyLine)
							lastSentence = GetNextSentence(sentenceEnumerator);
						// Añade los moviemientos
						if (lastSentence is SentenceTurnNumberModel || lastSentence is SentenceTurnPlayModel)
						{
							// Lee los movimientos
							lastSentence = ReadMovements(sentenceEnumerator, lastSentence, game.Movements);
							// Añade el juego
							library.Games.Add(game);
						}
						else
							lastSentence = new SentenceErrorModel("Can't find game movements");
				}
				while (!(lastSentence is SentenceEndModel) && !(lastSentence is SentenceErrorModel));
				// Devuelve el juego creado
				return lastSentence;
		}

		/// <summary>
		///		Lee las cabeceras
		/// </summary>
		private SentenceBaseModel ReadHeaders(IEnumerator<SentenceBaseModel> sentenceEnumerator, GamePgnModel game)
		{
			SentenceBaseModel sentence = GetNextSentence(sentenceEnumerator);

				// Obtiene las cabeceras
				do
				{
					// Trata la sentencia
					switch (sentence)
					{
						case SentenceTagModel sentenceTag:
								game.Headers.Add(new HeaderPgnModel(sentenceTag.Tag, sentenceTag.Content));
							break;
						case SentenceCommentModel sentenceComment:
								game.Comments.Add(sentenceComment.Content);
							break;
					}
					// Pasa a la siguiente sentencia
					sentence = GetNextSentence(sentenceEnumerator);
				}
				while (!(sentence is SentenceEndModel) && !(sentence is SentenceErrorModel) && 
					   !(sentence is SentenceTurnNumberModel) && !(sentence is SentenceEmptyLine));
				// Devuelve la última sentencia leída
				return sentence;
		}

		/// <summary>
		///		Lee los comentarios
		/// </summary>
		private SentenceBaseModel ReadComments(SentenceBaseModel sentence, IEnumerator<SentenceBaseModel> sentenceEnumerator, GamePgnModel game)
		{
			// Lee los comentarios
			do
			{
				// Trata la sentencia
				if (sentence is SentenceCommentModel sentenceComment)
					game.Comments.Add(sentenceComment.Content);
				// Pasa a la siguiente sentencia
				sentence = GetNextSentence(sentenceEnumerator);
			}
			while (!(sentence is SentenceEndModel) && (sentence is SentenceCommentModel || sentence is SentenceUnknownModel));
			// Devuelve la última sentencia leída
			return sentence;
		}

		/// <summary>
		///		Lee los movimientos
		/// </summary>
		private SentenceBaseModel ReadMovements(IEnumerator<SentenceBaseModel> sentenceEnumerator, SentenceBaseModel previousSentence, 
												List<BaseMovementModel> movements)
		{
			SentenceBaseModel sentence = previousSentence;
			BaseMovementModel lastMovement = null;
			TurnModel lastTurn = null;

				// Añade los movimientos
				do
				{
					// Trata la sentencia
					switch (sentence)
					{
						case SentenceTurnNumberModel sentenceTurn:
								lastMovement = null;
								lastTurn = new TurnModel(sentenceTurn.Content, TurnModel.TurnType.White);
							break;
						case SentenceTurnPlayModel sentencePlay:
								// Si no había separador entre el turno y el movimiento, leemos "1.d4", separamos el turno del movimiento
								if (sentencePlay.Content.IndexOf(".") >= 0)
								{
									string [] parts = sentencePlay.Content.Split('.');

										if (parts.Length == 2)
										{
											// Crea el movimiento
											lastMovement = null;
											lastTurn = new TurnModel(parts[0], TurnModel.TurnType.White);
											// Deja el contenido sin el número inicial
											sentencePlay = new SentenceTurnPlayModel(parts[1]);
										}
										else
											sentence = new SentenceErrorModel($"Unknow movement found: {sentencePlay.Content}");
								}
								// Creamos el movimiento
								if (lastTurn == null)
									sentence = new SentenceErrorModel($"There is not turn to add the play {sentencePlay.Content}");
								else
								{
									// Crea el movimiento
									lastMovement = new PieceMovementModel(lastTurn, sentencePlay.Content);
									// Añade el movimiento
									movements.Add(lastMovement);
								}
							break;
						case SentenceTurnResultModel sentenceResult:
								if (lastTurn == null)
									sentence = new SentenceErrorModel($"There is not turn to add the result {sentenceResult.Content}");
								else
								{
									// Crea el movimiento
									lastMovement = new ResultMovementModel(lastTurn, sentence.Content);
									// Añade el movimiento
									movements.Add(lastMovement);
								}
							break;
						case SentenceTurnInformationModel sentenceInformation:
								if (lastMovement == null)
									sentence = new SentenceErrorModel($"There is not movement to add the information {sentenceInformation.Content}");
								else
									lastMovement.Info.Add(new InfoMovementModel(sentence.Content));
							break;
						case SentenceCommentModel sentenceComment:
								if (lastMovement == null)
									sentence = new SentenceErrorModel($"There is not sentence to add the comment {sentenceComment.Content}");
								else
									lastMovement.Comments.Add(sentenceComment.Content);
							break;
						case SentenceTurnStartVariationModel sentenceVariation:
								if (lastMovement == null)
									sentence = new SentenceErrorModel("There is not sentence to add a variation");
								else
								{
									VariationModel variation = new VariationModel(lastMovement);

										// Limpia esta sentencia del enumerado
										sentence = GetNextSentence(sentenceEnumerator);
										// Añade los comentarios y la información a la variación
										while (sentence is SentenceTurnInformationModel || sentence is SentenceCommentModel)
										{
											// Añade la información de la sentencia
											switch (sentence)
											{
												case SentenceTurnInformationModel sentenceInfo:
														variation.Info.Add(new InfoMovementModel(sentenceInfo.Content));
													break;
												case SentenceCommentModel sentenceComment:
														variation.Comments.Add(sentenceComment.Content);
													break;
											}
											// Pasa a la siguiente sentencia
											sentence = GetNextSentence(sentenceEnumerator);
										}
										// Lee los movimientos de la variación
										sentence = ReadMovements(sentenceEnumerator, sentence, variation.Movements);
										// Añade la variación a la colección
										if (variation.Movements.Count > 0)
											lastMovement.Variations.Add(variation);
										// Pasa a la siguiente sentencia
										if (!(sentence is SentenceTurnEndVariationModel))
											sentence = new SentenceErrorModel("Can't find the end variation sentence");
								}
							break;
					}
					// Pasa a la siguiente sentencia
					if (!(sentence is SentenceErrorModel))
						sentence = GetNextSentence(sentenceEnumerator);
				}
				while (!(sentence is SentenceEndModel) && !(sentence is SentenceErrorModel) && !(sentence is SentenceTagModel) && !(sentence is SentenceTurnEndVariationModel));
				// Devuelve la última sentencia leída
				return sentence;
		}

		/// <summary>
		///		Obtiene la siguiente sentencia
		/// </summary>
		private SentenceBaseModel GetNextSentence(IEnumerator<SentenceBaseModel> sentenceEnumerator)
		{
			if (sentenceEnumerator.MoveNext())
				return sentenceEnumerator.Current;
			else
				return new SentenceEndModel();
		}

		/// <summary>
		///		Ajusta los turnos de juego
		/// </summary>
		private void AdjustTurnPlay(List<BaseMovementModel> movements)
		{
			bool isWhite = true;

				foreach (BaseMovementModel movement in movements)
					switch (movement)
					{
						case PieceMovementModel pieceMovement:
								// Ajusta para los cambios de turno en las variaciones
								if (pieceMovement.Turn.Number.EndsWith(".."))
									isWhite = false;
								// Asigna el turno
								if (isWhite)
									pieceMovement.SetType(TurnModel.TurnType.White);
								else
									pieceMovement.SetType(TurnModel.TurnType.Black);
								// Cambia el color
								isWhite = !isWhite;
								// Cambia las variaciones
								foreach (VariationModel variation in pieceMovement.Variations)
									AdjustTurnPlay(variation.Movements);
							break;
					}
		}
	}
}