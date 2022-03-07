using System.Windows.Forms;

namespace InfoReader.Configuration.Gui
{
    public interface IGuiConfigurable : IConfigurable
    {
        Form CreateConfigWindow();
    }
}
