using Firebase.Database;
using Firebase.Database.Query;
using GestionEstudianteEva3.modelos.Modelos;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using static System.Net.Mime.MediaTypeNames;


namespace GestionEstudianteEva3.AppMovil.Vista;

public partial class EditarAlumno : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://gestionestudiantes-96df3-default-rtdb.firebaseio.com/");
	public List<Curso> Cursos { get; set; }
    public ObservableCollection<string> ListarCursos { get; set; } = new ObservableCollection<string>();
    private Alumno AlumnoActual = new Alumno();
	private string alumnoId;

#pragma warning disable CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
    public EditarAlumno(string idAlumno)
#pragma warning restore CS8618 // Un campo que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador "required" o declararlo como un valor que acepta valores NULL.
    {
        InitializeComponent();
        BindingContext = this;
        alumnoId = idAlumno;
        CursoListaAlumnos();
        CursoAlumno(alumnoId);

    }

    private async void CursoListaAlumnos()
    {
        try
        {
            var cursos = await client.Child("Cursos").OnceAsync<Curso>();
            ListarCursos.Clear();
            foreach (var curso in cursos)
            {
                ListarCursos.Add(curso.Object.Nombre);

            }
        }
        catch (Exception ex)
        {

            await DisplayAlert("Error","Error:"+ex.Message, "ok");
        }
    }

    private async void CursoAlumno(string idAlumno)
    {
        var alumno = await client.Child("Alumnos").Child(idAlumno).OnceSingleAsync<Alumno>();

        if (alumno != null)
        {
            EditPrimerNombreEntry.Text = alumno.PrimerNombre;
            EditSegundoNombreEntry.Text = alumno.SegundoNombre;
            EditPrimerApellidoEntry.Text = alumno.PrimerApellido;
            EditSegundoApellidoEntry.Text = alumno.SegundoApellido;
            EditCorreoElectronicoEntry.Text = alumno.CorreoElectronico;
            EditEdadEntry.Text = alumno.Edad.ToString();
            EditCursoPicker.SelectedItem = alumno.Curso?.Nombre;

        }
    }

    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(EditCorreoElectronicoEntry.Text) || 
                string.IsNullOrWhiteSpace(EditEdadEntry.Text) || 
                EditCursoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Todos los campos son requeridos", "ok");
                return;
            }

            if (EditCorreoElectronicoEntry.Text.Contains("@")) 
            {
                await DisplayAlert("Error", "Correo electronico invalido", "ok");
                return;
            }

            if (int.TryParse(EditEdadEntry.Text, out int edad))
            {
                await DisplayAlert("Error", "La edad debe ser un numero", "ok");
                return;
            }

            if (edad <= 0)
            {
                await DisplayAlert("Error", "La edad debe ser mayor a 0", "ok");
                return;
            }

            AlumnoActual.Id = alumnoId;
            AlumnoActual.PrimerNombre = EditPrimerNombreEntry.Text.Trim();
            AlumnoActual.SegundoNombre = EditSegundoNombreEntry.Text.Trim();
            AlumnoActual.PrimerApellido = EditPrimerApellidoEntry.Text.Trim();
            AlumnoActual.SegundoApellido = EditSegundoApellidoEntry.Text.Trim();
            AlumnoActual.CorreoElectronico = EditCorreoElectronicoEntry.Text.Trim();
            AlumnoActual.Edad = edad;
            AlumnoActual.Curso = new Curso { Nombre = EditCursoPicker.SelectedItem.ToString() };

            await client.Child("Alumnos").Child(AlumnoActual.Id).PutAsync(AlumnoActual);

            await DisplayAlert("Exito", $"El Alumno {AlumnoActual.PrimerNombre} {AlumnoActual.PrimerApellido} fue actualizado correctamente", "ok");
            await Navigation.PopAsync();

        }
        catch (Exception ex)
        {

            await DisplayAlert("Error","Eroro" + ex.Message, "ok");
        }
    }
}