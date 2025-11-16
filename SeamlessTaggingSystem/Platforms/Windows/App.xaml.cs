using Microsoft.UI.Xaml;
using SeamlessTaggingSystem.WinUI;
using MauiProgramAlias = ClothesTagger.MauiProgram; 

namespace SeamlessTaggingSystem.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp()
        {
            return MauiProgramAlias.CreateMauiApp();
        }
    }
}
