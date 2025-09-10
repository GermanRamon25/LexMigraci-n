using LexMigración;
using LexMigración.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LexMigración
{
    public partial class Login : Window
    {
        private readonly DatabaseService _dbService;

        public Login()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = TxtUsuario.Text.Trim();
            string contra = PwdContra.Password;
            string rol = (CmbRol.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(rol))
            {
                TxtMensaje.Text = "Por favor, selecciona un rol.";
                return;
            }

            bool esUsuarioValido = _dbService.ValidarUsuario(usuario, contra, rol);

            if (esUsuarioValido)
            {
                // --- CAMBIO CLAVE AQUÍ ---
                // Ahora le pasamos el 'usuario' a la MainWindow al crearla.
                MainWindow main = new MainWindow(usuario);
                main.Show();
                this.Close();
            }
            else
            {
                TxtMensaje.Foreground = Brushes.LightCoral;
                TxtMensaje.Text = "Usuario, contraseña o rol incorrectos.";
            }
        }

        private void BtnAbrirRegistro_Click(object sender, RoutedEventArgs e)
        {
            RegistroWindow registroWindow = new RegistroWindow();
            registroWindow.ShowDialog();
        }
    }
}