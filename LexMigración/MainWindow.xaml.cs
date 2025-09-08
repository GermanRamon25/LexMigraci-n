using LexMigración;
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
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace LexMigracion
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    // CORRECCIÓN CLAVE: La clase debe ser 'public partial' y heredar de 'Window'
    public partial class MainWindow : Window
    {
        // El resto de tu código está funcionalmente bien para tu lógica.
        // No necesita cambios.

        // Variables para controlar las ventanas abiertas
        private Anexo_A anexoWindow;
        private Protocolo protocoloWindow;
        private Indice indiceWindow;
        private Panel_Migraciones migracionesWindow;

        public MainWindow()
        {
            InitializeComponent(); // Esto ya no dará error.
            InitializeButtons();

            // Hacer la ventana arrastrable desde la barra superior
            // El evento ya está asignado en el XAML con MouseDown="MainWindow_MouseDown"
            // por lo que esta línea se puede eliminar para evitar duplicidad.
            // this.MouseDown += MainWindow_MouseDown; 
        }

        private void InitializeButtons()
        {
            // Agregar event handlers a los botones
            BtnAnexos.Click += BtnAnexos_Click;
            BtnProtocolos.Click += BtnProtocolos_Click;
            BtnIndice.Click += BtnIndice_Click;
            BtnMigraciones.Click += BtnMigraciones_Click;
        }

        // Event handlers para los botones de navegación
        private void BtnAnexos_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Anexo");
            if (anexoWindow == null || !anexoWindow.IsLoaded)
            {
                anexoWindow = new Anexo_A();
            }
            anexoWindow.Show();
            anexoWindow.Activate();
            UpdateButtonStyles(BtnAnexos);
        }

        private void BtnProtocolos_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Protocolo");
            if (protocoloWindow == null || !protocoloWindow.IsLoaded)
            {
                protocoloWindow = new Protocolo();
            }
            protocoloWindow.Show();
            protocoloWindow.Activate();
            UpdateButtonStyles(BtnProtocolos);
        }

        private void BtnIndice_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Indice");
            if (indiceWindow == null || !indiceWindow.IsLoaded)
            {
                indiceWindow = new Indice();
            }
            indiceWindow.Show();
            indiceWindow.Activate();
            UpdateButtonStyles(BtnIndice);
        }

        private void BtnMigraciones_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Migraciones");
            if (migracionesWindow == null || !migracionesWindow.IsLoaded)
            {
                migracionesWindow = new Panel_Migraciones();
            }
            migracionesWindow.Show();
            migracionesWindow.Activate();
            UpdateButtonStyles(BtnMigraciones);
        }

        // Método para cerrar otras ventanas
        private void CloseOtherWindows(string except)
        {
            if (except != "Anexo" && anexoWindow != null && anexoWindow.IsLoaded)
            {
                anexoWindow.Close();
            }
            if (except != "Protocolo" && protocoloWindow != null && protocoloWindow.IsLoaded)
            {
                protocoloWindow.Close();
            }
            if (except != "Indice" && indiceWindow != null && indiceWindow.IsLoaded)
            {
                indiceWindow.Close();
            }
            if (except != "Migraciones" && migracionesWindow != null && migracionesWindow.IsLoaded)
            {
                migracionesWindow.Close();
            }
        }

        // Método para actualizar estilos de botones (mostrar cuál está activo)
        private void UpdateButtonStyles(Button activeButton)
        {
            ResetButtonStyle(BtnAnexos);
            ResetButtonStyle(BtnProtocolos);
            ResetButtonStyle(BtnIndice);
            ResetButtonStyle(BtnMigraciones);
            SetActiveButtonStyle(activeButton);
        }

        private void ResetButtonStyle(Button button)
        {
            button.Background = System.Windows.Media.Brushes.Transparent;
            button.Opacity = 0.8;
        }

        private void SetActiveButtonStyle(Button button)
        {
            button.Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(41, 128, 185)); // Color azul más claro
            button.Opacity = 1.0;
        }

        // Event handlers para los controles de ventana
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Permitir arrastrar la ventana desde la barra superior
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}