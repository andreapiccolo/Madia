using Microsoft.Maui.Controls;

namespace Madia;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
}
