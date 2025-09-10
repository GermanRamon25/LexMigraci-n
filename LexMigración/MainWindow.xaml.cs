using LexMigración;
using System.Windows;
using System.Windows.Input;
using LexMigración.Services;
using System.Windows.Controls;
using System;
using System.Linq;

// --- LÍNEA CORREGIDA ---
// Asegúrate de que el namespace tenga la tilde, igual que en tus otros archivos.
namespace LexMigración
{
    public partial class MainWindow : Window
    {
        private DatabaseService _dbService;
        private Anexo_A anexoWindow;
        private Protocolo protocoloWindow;
        private Indice indiceWindow;
        private Panel_Migraciones migracionesWindow;

        public MainWindow(string nombreUsuario)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            InitializeButtons();

            // Esto ahora funcionará porque el namespace es correcto
            TxtUsuarioLogeado.Text = nombreUsuario;
        }

        private void InitializeButtons()
        {
            BtnAnexos.Click += BtnAnexos_Click;
            BtnProtocolos.Click += BtnProtocolos_Click;
            BtnIndice.Click += BtnIndice_Click;
            BtnMigraciones.Click += BtnMigraciones_Click;
        }

        // --- MÉTODOS COMPLETOS PARA LA NAVEGACIÓN ---
        private void BtnAnexos_Click(object sender, RoutedEventArgs e)
        {
            CloseOtherWindows("Anexo");
            if (anexoWindow == null || !anexoWindow.IsLoaded)
            {
                anexoWindow = new Anexo_A();
            }
            // Para mostrar la ventana dentro del ContentControl, deberías cambiar la lógica,
            // pero para abrirla como nueva ventana, esto funciona:
            anexoWindow.Show();
            anexoWindow.Activate();
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
        }

        private void CloseOtherWindows(string except)
        {
            if (except != "Anexo" && anexoWindow != null && anexoWindow.IsLoaded) anexoWindow.Close();
            if (except != "Protocolo" && protocoloWindow != null && protocoloWindow.IsLoaded) protocoloWindow.Close();
            if (except != "Indice" && indiceWindow != null && indiceWindow.IsLoaded) indiceWindow.Close();
            if (except != "Migraciones" && migracionesWindow != null && migracionesWindow.IsLoaded) migracionesWindow.Close();
        }

        // --- Métodos para controlar la ventana ---
        private void MinimizeWindow_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void MaximizeRestoreWindow_Click(object sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void CloseWindow_Click(object sender, RoutedEventArgs e) => this.Close();

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}