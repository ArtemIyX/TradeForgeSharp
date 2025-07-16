using System.Reflection;

namespace TradeForge
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            
            
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            window.Title = $"Trade Forge {version}";
            return window;
        }
    }
}
