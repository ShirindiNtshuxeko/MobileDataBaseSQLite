using MobileDataBaseSQLite.View;

namespace MobileDataBaseSQLite
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ViewPage();
        }
    }
}
