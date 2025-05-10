
using Firebase.Database;
using Firebase.Database.Query;
using GestionEstudianteEva3.modelos.Modelos;
using LiteDB;
using System.Collections.ObjectModel;

namespace GestionEstudianteEva3.AppMovil.Vista;

public partial class ListarAlumno : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://gestionestudiantes-96df3-default-rtdb.firebaseio.com/");

    public ObservableCollection<Alumno> Lista { get; set; } = new ObservableCollection<Alumno>();

    public ListarAlumno()
    {
        InitializeComponent();
        BindingContext = this;
        CargarLista();
    }

    private async void CargarLista()
    {
        Lista.Clear();
        var alumnos = await client.Child("Alumnos").OnceAsync<Alumno>();

        var alumnosActivos= alumnos.Where(e=> e.Object.Estado == true).ToList();

        foreach (var alumno in alumnosActivos)
        {
            Lista.Add(new Alumno
            { 
                Id= alumno.Key,
                PrimerNombre = alumno.Object.PrimerNombre,
                SegundoNombre = alumno.Object.SegundoNombre,
                PrimerApellido = alumno.Object.PrimerApellido,
                SegundoApellido = alumno.Object.SegundoApellido,
                CorreoElectronico = alumno.Object.CorreoElectronico,
                Edad = alumno.Object.Edad,
                Curso = alumno.Object.Curso,
                Estado = alumno.Object.Estado

            });
        }

        #region CodigoAntiguo
        //client.Child("Alumnos").AsObservable<Alumno>().Subscribe((alumno) =>
        //{
        //if (alumno != null)
        //{
        //Lista.Add(alumno.Object);
        //}
        //});
        #endregion
    }

    private void filtroSerchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSerchBar.Text.ToLower();
        
        if (filtro.Length > 0)
        {
            ListaCollection.ItemsSource = Lista.Where(x => x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            ListaCollection.ItemsSource = Lista;
        }
    }

    private async void nuevoAlumnoBoton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearAlumno());
    }

    private async void editarButton_Clicked(object sender, EventArgs e)
    {
        var boton= sender as ImageButton;
        var alumno = boton?.CommandParameter as Alumno;

        if (alumno != null && !string.IsNullOrEmpty(alumno.Id))
        {
            await Navigation.PushAsync(new EditarAlumno(alumno.Id));
        }
        else
        {
            await DisplayAlert("Error", "No se puede editar el alumno", "Ok");
        }
    }

    private async void eliminarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var alumno = boton?.CommandParameter as Alumno;

        if (alumno == null)
        {
            await DisplayAlert("Error", "No se puede eliminar el alumno", "Ok");
            return;

        }

        bool confirmacion = await DisplayAlert
            ("Confirmacion", $"¿Esta seguro de eliminar el alumno {alumno.NombreCompleto}?", "Si", "No");

        if (confirmacion)
        {
            try
            {
                alumno.Estado = false;
                await client.Child("Alumnos").Child(alumno.Id).PutAsync(alumno);
                await DisplayAlert("Exito", $"El Alumno {alumno.NombreCompleto} fue eliminado correctamente", "Ok");
                CargarLista();

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}