using Firebase.Database;
using Firebase.Database.Query;
using GestionEstudianteEva3.modelos.Modelos;
namespace GestionEstudianteEva3.AppMovil.Vista;

public partial class CrearAlumno : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://gestionestudiantes-96df3-default-rtdb.firebaseio.com/");

    public List<Curso> Cursos { get; set; }

    public CrearAlumno()
    {
        InitializeComponent();
        ListarCursos();
        BindingContext = this;
    }

    private void ListarCursos()
    {
        var cursos = client.Child("Cursos").OnceAsync<Curso>();

        Cursos = cursos.Result.Select(x => x.Object).ToList();
    }

    private async void guardarButton_Clicked(object sender, EventArgs e)
    {
        Curso curso = cursoPicker.SelectedItem as Curso;

        var alumno = new Alumno
        {
            PrimerNombre = primerNombreEntry.Text,
            SegundoNombre = segundoNombreEntry.Text,
            PrimerApellido = primerApellidoEntry.Text,
            SegundoApellido = segundoApellidoEntry.Text,
            CorreoElectronico = correoElectronicoEntry.Text,
            Edad = int.Parse(edadEntry.Text),
            Curso = curso
        };

        try
        {
            await client.Child("Alumnos").PostAsync(alumno);
            await DisplayAlert("Exito", $"El Alumno {alumno.PrimerNombre} {alumno.PrimerApellido} fue guardado correctamente", "ok");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "ok");
        }


    }
}