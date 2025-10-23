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
            
            string usuario = TxtUsuario.Text.Trim();
            string contra = PwdContra.Password;
            string confirmarContra = PwdConfirmarContra.Password;
            string rol = (CmbRol.SelectedItem as ComboBoxItem)?.Content.ToString();

          
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

           
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = usuario,
                Contrasena = contra, 
                Rol = rol
            };

            
            try
            {
                string resultado = _dbService.RegistrarUsuario(nuevoUsuario);
                if (resultado == "Éxito")
                {
                    MessageBox.Show("Usuario registrado exitosamente. Ahora puede iniciar sesión.", "Registro Completo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); 
                }
                else
                {
                    
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
            this.Close(); 
        }
    }
}