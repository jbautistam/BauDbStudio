using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace Bau.Libraries.LibChessGame.Mvvm
{
	/// <summary>
	///		Base para objetos observables (implementan INotifyPropertyChanged)
	/// </summary>
	public abstract class BaseObservableObject : INotifyPropertyChanged
	{ 
		// Eventos públicos
		public event PropertyChangedEventHandler PropertyChanged;

		protected BaseObservableObject(bool changeUpdated = true)
		{
			ChangeUpdated = changeUpdated;
		}

		/// <summary>
		///		Comprueba si se debe modificar un valor de una propiedad
		/// </summary>
		protected bool CheckProperty<TypeData>(ref TypeData value, TypeData newValue, [CallerMemberName] string propertyName = null)
		{
			if (!object.Equals(value, newValue))
			{ 
				// Cambia el valor
				value = newValue;
				// Lanza el evento
				OnPropertyChanged(propertyName);
				// Devuelve el valor que indica que se ha modificado
				return true;
			}
			else // ... los objetos son iguales y no se modifica el valor
				return false;
		}

		/// <summary>
		///		Comprueba si se debe modificar un valor de una propiedad para un objeto
		/// </summary>
		public bool CheckObject<TypeData>(ref TypeData value, TypeData newValue, [CallerMemberName] string propertyName = null)
		{
			if (!ReferenceEquals(value, newValue))
			{ 
				// Cambia el valor
				value = newValue;
				// Lanza el evento
				OnPropertyChanged(propertyName);
				// Devuelve el valor que indica que se ha modificado
				return true;
			}
			else // ... los objetos son iguales y no se modifica el valor
				return false;
		}

		/// <summary>
		///		Lanza el evento de cambio de propiedad
		/// </summary>
		internal void RaiseEventPropertyChanged(string propertyName)
		{
			OnPropertyChanged(propertyName);
		}

		/// <summary>
		///		Lanza el evento <see cref="PropertyChanged"/>
		/// </summary>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			OnPropertyChanged(ChangeUpdated, propertyName);
		}

		/// <summary>
		///		Lanza el evento <see cref="PropertyChanged"/>
		/// </summary>
		protected virtual void OnPropertyChanged(bool changeUpdated, [CallerMemberName] string propertyName = null)
		{ 
			// Indica que se ha modificado
			//! Antes de lanzar el evento para darle la oportunidad a los objetos hijo a cambiar el valor de IsUpdated = false
			if (changeUpdated)
				IsUpdated = true;
			// Lanza el evento
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		///		Indica si los datos del modelo se han modificado
		/// </summary>
		public virtual bool IsUpdated { get; set; }

		/// <summary>
		///		Indica si se debe cambiar el valor de IsUpdated en el formulario
		/// </summary>
		public bool ChangeUpdated { get; set; }
	}
}