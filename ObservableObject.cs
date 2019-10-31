using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

//Event Design: http://msdn.microsoft.com/en-us/library/ms229011.aspx

namespace MicroMvvm
{
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        //采用新的set方式
        //protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        //{
        //    if (EqualityComparer<T>.Default.Equals(storage, value))
        //        return false;
        //    storage = value;
        //    this.RaisePropertyChanged(propertyName);
        //    return true;
        //}
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged = null, bool condition = true, [CallerMemberName] string propertyName = "")
        {

            if (!condition || EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }



        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 尽量不要用这个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpresssion"></param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpresssion);
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// 尽量不要用这个
        /// </summary>
        /// <param name="propertyName"></param>
        private protected void RaisePropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Warns the developer if this Object does not have a public property with
        /// the specified name. This method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // verify that the property name matches a real,  
            // public, instance property on this Object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                Debug.Fail("Invalid property name: " + propertyName);
            }
        }
    }
}
