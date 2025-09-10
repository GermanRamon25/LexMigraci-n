using LexMigración.Models;
using LexMigración.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LexMigración
{
    public partial class RegistroWindow : Window
    {
        private readonly DatabaseService _dbService;

        public RegistroWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            // 1. Recolectar datos del formulario
            string usuario = TxtUsuario.Text.Trim();
            string contra = PwdContra.Password;
            string confirmarContra = PwdConfirmarContra.Password;
            string rol = (CmbRol.SelectedItem as ComboBoxItem)?.Content.ToString();

            // 2. Validar los datos
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contra) || string.IsNullOrEmpty(rol))
            {
                TxtMensaje.Text = "Todos los campos son obligatorios.";
                return;
            }

            if (contra != confirmarContra)
            {
                TxtMensaje.Text = "Las contraseñas no coinciden.";
                return;
            }

            // 3. Crear el objeto Usuario
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = usuario,
                Contrasena = contra, // En un sistema real, la contraseña debe ser encriptada
                Rol = rol
            };

            // 4. Llamar al servicio para registrarlo
            try
            {
                string resultado = _dbService.RegistrarUsuario(nuevoUsuario);
                if (resultado == "Éxito")
                {
                    MessageBox.Show("Usuario registrado exitosamente. Ahora puede iniciar sesión.", "Registro Completo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); // Cerrar la ventana de registro
                }
                else
                {
                    // Mostrar el error devuelto por el servicio (ej: "El nombre de usuario ya existe")
                    TxtMensaje.Text = resultado;
                }
            }
            catch (System.Exception ex)
            {
                TxtMensaje.Text = "Error al conectar con la base de datos: " + ex.Message;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Simplemente cierra la ventana
        }
    }
}