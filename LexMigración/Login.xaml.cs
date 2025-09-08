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

            // Usuarios permitidos: 3 secretarias y 1 jefe
            var usuariosValidos = new[]
            {
                new { Usuario = "secretaria1", Contrasena = "1234" },
                new { Usuario = "secretaria2", Contrasena = "1234" },
                new { Usuario = "secretaria3", Contrasena = "1234" },
                new { Usuario = "jefe", Contrasena = "abcd" }
            };

            bool esUsuarioValido = false;

            foreach (var user in usuariosValidos)
            {
                if (user.Usuario == usuario && user.Contrasena == contra)
                {
                    esUsuarioValido = true;
                    break;
                }
            }

            if (esUsuarioValido)
            {
                TxtMensaje.Foreground = System.Windows.Media.Brushes.LightGreen;
                TxtMensaje.Text = $"Bienvenido {usuario}!";
                // Aquí puedes abrir la ventana principal o la que corresponda
                // Por ejemplo:
                // MainWindow main = new MainWindow();
                // main.Show();
                // this.Close();
            }
            else
            {
                TxtMensaje.Foreground = System.Windows.Media.Brushes.LightCoral;
                TxtMensaje.Text = "Usuario o contraseña incorrectos.";
            }
        }
    }
}
