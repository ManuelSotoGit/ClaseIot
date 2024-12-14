using GestionEstudianteEva3.AppMovil.Vista;

namespace GestionEstudianteEva3.AppMovil
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage (new ListarAlumno());
        }
    }
}
