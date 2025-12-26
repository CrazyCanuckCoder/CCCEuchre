namespace Euchre.UILogic.Interfaces;

public interface IMainWindow
{
    IMainWindowViewModel ViewModel { get; }

    void InitializeComponent();
}