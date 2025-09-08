using LexMigracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls; // Necesario para ComboBoxItem

namespace LexMigración
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = TxtUsuario.Text.Trim();
            string contra = PwdContra.Password;
            string rol = (CmbRol.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Validar que se haya seleccionado un rol
            if (string.IsNullOrEmpty(rol))
            {
                TxtMensaje.Text = "Por favor, selecciona un rol.";
                return;
            }

            // CAMBIO: Ahora los usuarios también tienen un rol asociado
            var usuariosValidos = new[]
            {
                new { Usuario = "secretaria1", Contrasena = "123", Rol = "Secretaria" },
                new { Usuario = "secretaria2", Contrasena = "1234", Rol = "Secretaria" },
                new { Usuario = "secretaria3", Contrasena = "1235", Rol = "Secretaria" },
                new { Usuario = "Sergio", Contrasena = "123456", Rol = "Lic.Sergio" }
            };

            bool esUsuarioValido = false;

            foreach (var user in usuariosValidos)
            {
                // CAMBIO: La validación ahora incluye el rol
                if (user.Usuario == usuario && user.Contrasena == contra && user.Rol == rol)
                {
                    esUsuarioValido = true;
                    break;
                }
            }

            if (esUsuarioValido)
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
            else
            {
                TxtMensaje.Foreground = System.Windows.Media.Brushes.LightCoral;
                TxtMensaje.Text = "Usuario, contraseña o rol incorrectos.";
            }
        }
    }
}