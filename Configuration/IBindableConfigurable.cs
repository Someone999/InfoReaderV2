using System.ComponentModel;
using InfoReader.Configuration.Elements;

namespace InfoReader.Configuration
{
    public interface IBindableConfiguration :IConfigurable, INotifyPropertyChanged
    {
    }
}
