using System;
using System.Windows;
using System.Windows.Threading;

namespace LexMigración
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 1. Crear y mostrar la pantalla de carga
            var splashScreen = new SplashScreen();
            splashScreen.Show();

            // 2. Usar un temporizador para simular la carga y dar tiempo a que se vea el logo
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3); // Durará 3 segundos
            timer.Tick += (s, args) =>
            {
                // 3. Cuando el tiempo se acaba, detenemos el temporizador
                timer.Stop();

                // 4. *** CAMBIO CLAVE AQUÍ: Creamos y mostramos la ventana de Login ***
                var loginWindow = new Login();
                loginWindow.Show();

                // 5. Cerramos la pantalla de carga
                splashScreen.Close();
            };

            // Iniciar el temporizador
            timer.Start();
        }
    }
}