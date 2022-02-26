namespace InfoReader.Tools.Win32;

public interface IInjector
{
    bool Inject(int pid, string modulePath);
}