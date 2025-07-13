using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ACPF.UI.ViewModels
{
    /// <summary>
    /// Classe base para todos os ViewModels
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifica a UI sobre mudanças em propriedades
        /// </summary>
        /// <param name="propertyName">Nome da propriedade que mudou</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Define o valor de uma propriedade e notifica a UI se o valor mudou
        /// </summary>
        /// <typeparam name="T">Tipo da propriedade</typeparam>
        /// <param name="field">Campo privado</param>
        /// <param name="value">Novo valor</param>
        /// <param name="propertyName">Nome da propriedade</param>
        /// <returns>True se o valor mudou, false caso contrário</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
} 