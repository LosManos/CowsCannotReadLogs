using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CowsCannotReadLogs.Client.Wpf
{
    /// <summary>This is the base class of all ViewModels.
    /// Handy so you don't have to write the notifying yourself.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                // Below can be simplified but don't; as it behaves better in a multi threaded environment
                // where PropertyChanged, which is public, can be nulled between the if statement and the handler call.
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}