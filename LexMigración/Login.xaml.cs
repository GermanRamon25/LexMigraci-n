using LexMigración.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LexMigración
{
    public partial class Login : Window
    {
        private readonly DatabaseService _dbService;
        // Variable para evitar bucles infinitos al actualizar los campos
        private bool _isPasswordSyncing = false;

        public Login()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = TxtUsuario.Text.Trim();
            // Siempre leemos la contraseña desde el PasswordBox, que estará sincronizado
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

        // --- LÓGICA PARA MOSTRAR/OCULTAR CONTRASEÑA ---

        private void BtnMostrarContra_Checked(object sender, RoutedEventArgs e)
        {
            // Mostrar el TextBox y ocultar el PasswordBox
            TxtContraVisible.Visibility = Visibility.Visible;
            PwdContra.Visibility = Visibility.Collapsed;
            // Sincronizar el contenido
            TxtContraVisible.Text = PwdContra.Password;
        }

        private void BtnMostrarContra_Unchecked(object sender, RoutedEventArgs e)
        {
            // Ocultar el TextBox y mostrar el PasswordBox
            TxtContraVisible.Visibility = Visibility.Collapsed;
            PwdContra.Visibility = Visibility.Visible;
            // Sincronizar el contenido
            PwdContra.Password = TxtContraVisible.Text;
        }

        // --- MÉTODOS PARA MANTENER SINCRONIZADOS LOS CAMPOS ---

        private void PwdContra_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isPasswordSyncing)
            {
                _isPasswordSyncing = true;
                TxtContraVisible.Text = PwdContra.Password;
                _isPasswordSyncing = false;
            }
        }

        private void TxtContraVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isPasswordSyncing)
            {
                _isPasswordSyncing = true;
                PwdContra.Password = TxtContraVisible.Text;
                _isPasswordSyncing = false;
            }
        }

        // --- *** MÉTODO NUEVO AÑADIDO PARA CERRAR LA VENTANA *** ---
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}