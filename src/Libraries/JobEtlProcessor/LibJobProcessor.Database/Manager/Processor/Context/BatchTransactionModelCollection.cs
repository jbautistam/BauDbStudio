using System;
using System.Collections.Generic;

using Bau.Libraries.DbAggregator.Models;

namespace Bau.Libraries.LibJobProcessor.Database.Manager.Processor.Context
{
	/// <summary>
	///		Colección de <see cref="BatchTransactionModel"/>
	/// </summary>
	internal class BatchTransactionModelCollection
	{
		/// <summary>
		///		Crea una transacción para un proveedor
		/// </summary>
		internal void Add(ProviderModel provider)
		{
			string key = Normalize(provider.Key);

				if (!Transactions.ContainsKey(key))
					Transactions.Add(key, new BatchTransactionModel(provider));
		}

		/// <summary>
		///		Añade un comando sobre las transacciones del proveedor
		/// </summary>
		internal void Add(ProviderModel provider, CommandModel command)
		{
			// Añade la transacción
			Add(provider);
			// Añade el comando
			if (Transactions.TryGetValue(Normalize(provider.Key), out BatchTransactionModel transaction))
				transaction.Commands.Add(command);
		}

		/// <summary>
		///		Comprueba si existe una transacción sobre un proveedor
		/// </summary>
		internal bool Exists(ProviderModel provider)
		{
			return Transactions.ContainsKey(Normalize(provider.Key));
		}

		/// <summary>
		///		Obtiene una transacción
		/// </summary>
		internal BatchTransactionModel Get(string key)
		{
			if (Transactions.TryGetValue(Normalize(key), out BatchTransactionModel transaction))
				return transaction;
			else
				return null;
		}

		/// <summary>
		///		Elimina los datos de una transacción
		/// </summary>
		internal void Remove(string key)
		{
			// Normaliza la clave
			key = Normalize(key);
			// Borra el elemento
			if (Transactions.ContainsKey(key))
				Transactions.Remove(key);
		}

		/// <summary>
		///		Limpia las transacciones
		/// </summary>
		internal void Clear()
		{
			Transactions.Clear();
		}

		/// <summary>
		///		Normaliza la clave
		/// </summary>
		private string Normalize(string key)
		{
			return key.ToUpper();
		}

		/// <summary>
		///		Diccionario de transacciones
		/// </summary>
		private Dictionary<string, BatchTransactionModel> Transactions { get; } = new Dictionary<string, BatchTransactionModel>();
	}
}
