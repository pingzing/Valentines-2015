using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Valentines2015.Annotations;

namespace Valentines2015.MVVM
{
    public class BindableWindowBase : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}