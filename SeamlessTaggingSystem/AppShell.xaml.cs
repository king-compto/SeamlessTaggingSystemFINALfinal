namespace ClothesTagger
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
#if !NETSTANDARD
            InitializeComponent();
#endif
        }
    }
}
