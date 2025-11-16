using SeamlessTaggingSystem;

namespace ClothesTagger;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new MainPage(); // no Shell, simpler stack
    }
}
