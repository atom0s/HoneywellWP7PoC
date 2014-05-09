
namespace TotalConnect.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Notifiable model implementation that wraps the INotifyPropertyChanged
    /// class for easier usage. Makes property get/set methods cleaner.
    /// </summary>
    public class NotifiableModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Internal properties container.
        /// </summary>
        private readonly Dictionary<String, Object> _properties;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NotifiableModel()
        {
            this._properties = new Dictionary<String, Object>();
        }

        /// <summary>
        /// Event triggered when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method used to raise the PropertyChanged event.
        /// </summary>
        /// <param name="prop"></param>
        public void OnPropertyChanged(String prop)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Method to raise the PropertyChanged event.
        /// </summary>
        /// <param name="property"></param>
        protected void RaisePropertyChanged(string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException(property);
            this.OnPropertyChanged(property);
        }

        /// <summary>
        /// Gets a property from the internal container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected T Get<T>(String prop)
        {
            if (this._properties.ContainsKey(prop))
            {
                return (T)this._properties[prop];
            }
            return default(T);
        }

        /// <summary>
        /// Sets a property in the internal container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <param name="val"></param>
        protected void Set<T>(String prop, T val)
        {
            var curr = this.Get<T>(prop);
            if (Equals(curr, val))
                return;

            this._properties[prop] = val;
            this.OnPropertyChanged(prop);
        }
    }
}
